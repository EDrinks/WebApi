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

        Task<List<Order>> GetOrders();
        
        Task<List<Settlement>> GetSettlements();
    }

    public class ReadModel : IReadModel
    {
        private readonly IEventStoreConnection _connection;
        private readonly IStreamResolver _streamResolver;
        private readonly IEventLookup _eventLookup;

        private bool _eventsLoaded = false;

        private Dictionary<Guid, Product> Products { get; set; }
        private Dictionary<Guid, Tab> Tabs { get; set; }
        private List<Order> Orders { get; set; }
        public Dictionary<Guid, Settlement> Settlements { get; set; }

        public ReadModel(IEventStoreConnection connection, IStreamResolver streamResolver, IEventLookup eventLookup)
        {
            _connection = connection;
            _streamResolver = streamResolver;
            _eventLookup = eventLookup;

            Products = new Dictionary<Guid, Product>();
            Tabs = new Dictionary<Guid, Tab>();
            Orders = new List<Order>();
            Settlements = new Dictionary<Guid, Settlement>();
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

        public async Task<List<Order>> GetOrders()
        {
            await ApplyAllEvents();
            return Orders;
        }

        public async Task<List<Settlement>> GetSettlements()
        {
            await ApplyAllEvents();
            return Settlements.Values.ToList();
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
                    Orders.Add(new Order()
                    {
                        Id = poot.OrderId,
                        ProductId = poot.ProductId,
                        TabId = poot.TabId,
                        Quantity = poot.Quantity,
                        DateTime = poot.MetaData.CreatedOn,
                        ProductPrice = Products[poot.ProductId].Price
                    });
                    break;
                case TabSettled ts:
                    if (!Settlements.ContainsKey(ts.SettlementId))
                    {
                        Settlements.Add(ts.SettlementId, new Settlement()
                        {
                            Id = ts.SettlementId,
                            DateTime = ts.MetaData.CreatedOn
                        });
                    }

                    var settlement = Settlements[ts.SettlementId];
                    var orders = Orders.Where(e => e.TabId == ts.TabId);
                    settlement.TabToOrders.Add(new TabToOrders()
                    {
                        Tab = Tabs[ts.TabId],
                        Orders = orders.ToList()
                    });

                    Orders.RemoveAll(e => e.TabId == ts.TabId);
                    break;
            }
        }
    }
}