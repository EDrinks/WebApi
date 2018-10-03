using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using EDrinks.Events;
using EDrinks.Events.Orders;
using EDrinks.Events.Products;
using EDrinks.Events.Tabs;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EDrinks.Test.Integration.DataGenerator
{
    public class Generator
    {
        private readonly IEventStoreConnection _connection;
        private readonly string _stream;
        private Faker _faker;

        public Generator(IEventStoreConnection connection, string stream)
        {
            _connection = connection;
            _stream = stream;
            _faker = new Faker();
        }

        public async Task<Guid> CreateTab()
        {
            var tabId = Guid.NewGuid();
            await WriteToStream(new TabCreated() {TabId = tabId});
            await WriteToStream(new TabNameChanged() {TabId = tabId, Name = _faker.Name.FirstName()});

            return tabId;
        }

        public async Task<Guid> CreateProduct()
        {
            var productId = Guid.NewGuid();
            await WriteToStream(new ProductCreated() {ProductId = productId});
            await WriteToStream(new ProductNameChanged() {ProductId = productId, Name = _faker.Commerce.Product()});
            await WriteToStream(new ProductPriceChanged()
                {ProductId = productId, Price = _faker.Random.Decimal(0.5M, 3.0M)});

            return productId;
        }

        public async Task<Guid> OrderOnTab(Guid tabId, Guid productId, int quantity = 1)
        {
            var orderId = Guid.NewGuid();

            await WriteToStream(new ProductOrderedOnTab()
            {
                TabId = tabId,
                ProductId = productId,
                Quantity = quantity,
                OrderId = orderId
            });

            return orderId;
        }

        public async Task<Guid> CreateSettlement(IEnumerable<Guid> tabIds)
        {
            var settlementId = Guid.NewGuid();

            foreach (var tabId in tabIds)
            {
                await WriteToStream(new TabSettled()
                {
                    SettlementId = settlementId,
                    TabId = tabId
                });
            }

            return settlementId;
        }

        private async Task WriteToStream(BaseEvent evt)
        {
            var metaDataStr = JsonConvert.SerializeObject(evt.MetaData);
            var contentStr = JsonConvert.SerializeObject(evt);

            var eventData = new EventData(Guid.NewGuid(), evt.GetType().Name, true,
                Encoding.UTF8.GetBytes(contentStr), Encoding.UTF8.GetBytes(metaDataStr));

            await _connection.AppendToStreamAsync(_stream, ExpectedVersion.Any, eventData);
            Thread.Sleep(50);
        }
    }
}