using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events;
using EDrinks.Events.Orders;
using EDrinks.Events.Products;
using EDrinks.Events.Tabs;
using EDrinks.QueryHandlers.Model;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EDrinks.QueryHandlers
{
    public interface IReadModel
    {
        Task<List<Product>> GetProducts();
        
        Task<List<Tab>> GetTabs();

        Task<List<Order>> GetOrdersOfTab(Guid tabId);
    }

    public class ReadModel : IReadModel
    {
        private readonly IEventStoreConnection _connection;
        private readonly IStreamResolver _streamResolver;
        private readonly IEventLookup _eventLookup;

        private bool _eventsLoaded = false;

        public Dictionary<Guid, Product> Products { get; set; }
        public Dictionary<Guid, Tab> Tabs { get; set; }
        public Dictionary<Guid, List<Order>> TabToOrders { get; set; }

        public ReadModel(IEventStoreConnection connection, IStreamResolver streamResolver, IEventLookup eventLookup)
        {
            _connection = connection;
            _streamResolver = streamResolver;
            _eventLookup = eventLookup;

            Products = new Dictionary<Guid, Product>();
            Tabs = new Dictionary<Guid, Tab>();
            TabToOrders = new Dictionary<Guid, List<Order>>();
        }

        public async Task<List<Product>> GetProducts()
        {
            await ApplyAllEvents();
            return Products.Values.ToList();
        }

        public async Task<List<Tab>> GetTabs()
        {
            await ApplyAllEvents();
            return Tabs.Values.ToList();
        }

        public async Task<List<Order>> GetOrdersOfTab(Guid tabId)
        {
            await ApplyAllEvents();
            return TabToOrders.ContainsKey(tabId) ? TabToOrders[tabId] : new List<Order>();
        }

        private async Task ApplyAllEvents()
        {
            if (_eventsLoaded) return;
            
            var events = new List<ResolvedEvent>();

            StreamEventsSlice currentSlice;
            var stream = _streamResolver.GetStream();
            var nextSliceStart = (long) StreamPosition.Start;
            do
            {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(stream,
                    nextSliceStart, 200, false);
                nextSliceStart = currentSlice.NextEventNumber;

                events.AddRange(currentSlice.Events);
            } while (!currentSlice.IsEndOfStream);

            foreach (var resolvedEvent in events)
            {
                var data = Encoding.UTF8.GetString(resolvedEvent.Event.Data);
                var metaData = Encoding.UTF8.GetString(resolvedEvent.Event.Metadata);
                Type eventType = _eventLookup.GetType(resolvedEvent.Event.EventType);
                if (eventType != null)
                {
                    var obj = (BaseEvent) JsonConvert.DeserializeObject(data, eventType);
                    obj.MetaData = JsonConvert.DeserializeObject<MetaData>(metaData);
                    await EventAppeared(obj);
                }
            }

            _eventsLoaded = true;
        }

        private async Task EventAppeared(BaseEvent evt)
        {
            switch (evt)
            {
                case ProductCreated pc:
                    var product = new Product();
                    product.Apply(pc);
                    Products.Add(pc.ProductId, product);
                    break;
                case ProductNameChanged pnc:
                    Products[pnc.ProductId].Apply(pnc);
                    break;
                case ProductPriceChanged pcc:
                    Products[pcc.ProductId].Apply(pcc);
                    break;
                case ProductDeleted pd:
                    Products.Remove(pd.ProductId);
                    break;
                case TabCreated tc:
                    var tab = new Tab();
                    tab.Apply(tc);
                    Tabs.Add(tc.TabId, tab);
                    break;
                case TabNameChanged tnc:
                    Tabs[tnc.TabId].Apply(tnc);
                    break;
                case TabDeleted td:
                    Tabs.Remove(td.TabId);
                    break;
                case ProductOrderedOnTab poot:
                    if (!TabToOrders.ContainsKey(poot.TabId))
                    {
                        TabToOrders.Add(poot.TabId, new List<Order>());
                    }
                    TabToOrders[poot.TabId].Add(new Order()
                    {
                        Id = poot.OrderId,
                        ProductId = poot.ProductId,
                        Quantity = poot.Quantity,
                        DateTime = poot.MetaData.CreatedOn,
                        ProductPrice = Products[poot.ProductId].Price
                    });
                    break;
            }
        }
    }
}