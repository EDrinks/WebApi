using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.Events;
using EDrinks.Events.Orders;
using EDrinks.Events.Products;
using EDrinks.Events.Spendings;
using EDrinks.Events.Tabs;
using EDrinks.QueryHandlers.Model;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EDrinks.QueryHandlers
{
    public interface IReadModel
    {
        void RegisterHandler(Action<BaseEvent> handler);

        Task ApplyAllEvents();
    }

    public class ReadModel : IReadModel
    {
        private readonly IEventStoreConnection _connection;
        private readonly IStreamResolver _streamResolver;
        private readonly IEventLookup _eventLookup;
        private readonly IDataContext _dataContext;

        private bool _eventsLoaded = false;

        private List<Action<BaseEvent>> _eventHandlers;

        public ReadModel(IEventStoreConnection connection, IStreamResolver streamResolver, IEventLookup eventLookup,
            IDataContext dataContext)
        {
            _connection = connection;
            _streamResolver = streamResolver;
            _eventLookup = eventLookup;
            _dataContext = dataContext;

            _eventHandlers = new List<Action<BaseEvent>>();
        }

        public void RegisterHandler(Action<BaseEvent> handler)
        {
            _eventHandlers.Add(handler);
        }

        public async Task ApplyAllEvents()
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

                    foreach (var eventHandler in _eventHandlers)
                    {
                        eventHandler(obj);
                    }
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
                    _dataContext.Products.Add(product);
                    break;
                case ProductNameChanged pnc:
                    _dataContext.Products.FirstOrDefault(e => e.Id == pnc.ProductId)?.Apply(pnc);
                    break;
                case ProductPriceChanged pcc:
                    _dataContext.Products.FirstOrDefault(e => e.Id == pcc.ProductId)?.Apply(pcc);
                    break;
                case ProductDeleted pd:
                    _dataContext.Products.RemoveAll(e => e.Id == pd.ProductId);
                    break;
                case TabCreated tc:
                    var tab = new Tab();
                    tab.Apply(tc);
                    _dataContext.Tabs.Add(tab);
                    break;
                case TabNameChanged tnc:
                    _dataContext.Tabs.FirstOrDefault(e => e.Id == tnc.TabId)?.Apply(tnc);
                    break;
                case TabDeleted td:
                    _dataContext.Tabs.RemoveAll(e => e.Id == td.TabId);
                    break;
                case ProductOrderedOnTab poot:
                    HandleEvent(poot);
                    break;
                case TabSettled ts:
                    HandleEvent(ts);
                    break;
                case OrderDeleted od:
                    HandleEvent(od);
                    break;
                case SpendingCreated sc:
                    HandleEvent(sc);
                    break;
                case ProductOrderedOnSpending poos:
                    HandleEvent(poos);
                    break;
                case SpendingClosed sc:
                    HandleEvent(sc);
                    break;
            }
        }

        private void HandleEvent(ProductOrderedOnTab poot)
        {
            var order = new Order()
            {
                Id = poot.OrderId,
                ProductId = poot.ProductId,
                TabId = poot.TabId,
                Quantity = poot.Quantity,
                DateTime = poot.MetaData.CreatedOn,
                ProductPrice = _dataContext.Products.FirstOrDefault(e => e.Id == poot.ProductId)?.Price ?? 0
            };

            _dataContext.CurrentOrders.Add(order);
            _dataContext.AllOrders.Add(order);

            if (_dataContext.CurrentSettlement.TabToOrders.All(e => e.Tab.Id != poot.TabId))
            {
                _dataContext.CurrentSettlement.TabToOrders.Add(new TabToOrders()
                {
                    Tab = _dataContext.Tabs.FirstOrDefault(e => e.Id == poot.TabId)
                });
            }

            var tabToOrders = _dataContext.CurrentSettlement.TabToOrders.Single(e => e.Tab.Id == poot.TabId);
            tabToOrders.Orders.Add(order);
        }

        private void HandleEvent(TabSettled ts)
        {
            if (_dataContext.Settlements.All(e => e.Id != ts.SettlementId))
            {
                _dataContext.Settlements.Add(new Settlement()
                {
                    Id = ts.SettlementId,
                    DateTime = ts.MetaData.CreatedOn
                });
            }

            var settlement = _dataContext.Settlements.Single(e => e.Id == ts.SettlementId);
            var orders = _dataContext.CurrentOrders.Where(e => e.TabId == ts.TabId);
            settlement.TabToOrders.Add(new TabToOrders()
            {
                Tab = _dataContext.Tabs.FirstOrDefault(e => e.Id == ts.TabId),
                Orders = orders.ToList()
            });

            _dataContext.CurrentOrders.RemoveAll(e => e.TabId == ts.TabId);
            _dataContext.CurrentSettlement.TabToOrders.RemoveAll(e => e.Tab.Id == ts.TabId);
        }

        private void HandleEvent(OrderDeleted od)
        {
            _dataContext.CurrentOrders.RemoveAll(e => e.Id == od.OrderId);
            _dataContext.AllOrders.RemoveAll(e => e.Id == od.OrderId);
            foreach (var tabToOrders in _dataContext.CurrentSettlement.TabToOrders)
            {
                tabToOrders.Orders.RemoveAll(e => e.Id == od.OrderId);
            }
        }

        private void HandleEvent(SpendingCreated sc)
        {
            _dataContext.Spendings.Add(new Spending()
            {
                Id = sc.SpendingId,
                TabId = sc.TabId,
                ProductId = sc.ProductId,
                Quantity = sc.Quantity,
                Current = 0
            });
        }

        private void HandleEvent(ProductOrderedOnSpending poos)
        {
            var spending = _dataContext.Spendings.First(e => e.Id == poos.SpendingId);
            spending.Current += poos.Quantity;
        }
        
        private void HandleEvent(SpendingClosed sc)
        {
            _dataContext.Spendings.RemoveAll(e => e.Id == sc.SpendingId);
        }
    }
}