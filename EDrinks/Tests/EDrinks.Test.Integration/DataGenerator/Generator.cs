using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using EDrinks.Events;
using EDrinks.Events.Orders;
using EDrinks.Events.Products;
using EDrinks.Events.Spendings;
using EDrinks.Events.Tabs;
using EDrinks.EventSourceSql.Model;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EDrinks.Test.Integration.DataGenerator
{
    public class Generator
    {
        private readonly DomainContext _context;
        private Faker _faker;

        public Generator(DomainContext context)
        {
            _context = context;
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

        public async Task<Guid> OrderOnSpending(Guid spendingId, int quantity = 1)
        {
            var orderId = Guid.NewGuid();

            await WriteToStream(new ProductOrderedOnSpending()
            {
                OrderId = orderId,
                SpendingId = spendingId,
                Quantity = quantity
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

        public async Task<Guid> CreateSpending(Guid tabId, Guid productId, int quantity)
        {
            var spendingId = Guid.NewGuid();

            await WriteToStream(new SpendingCreated()
            {
                SpendingId = spendingId,
                TabId = tabId,
                ProductId = productId,
                Quantity = quantity
            });

            return spendingId;
        }

        private async Task WriteToStream(BaseEvent evt)
        {
            await _context.DomainEvents.AddAsync(new DomainEvent()
            {
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "system",
                EventType = evt.GetType().Name,
                Content = JsonConvert.SerializeObject(evt)
            });
            await _context.SaveChangesAsync();

            Thread.Sleep(50);
        }
    }
}