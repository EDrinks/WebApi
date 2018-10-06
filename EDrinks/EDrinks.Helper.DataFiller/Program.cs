using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Bogus;
using EDrinks.Events;
using EDrinks.Events.Orders;
using EDrinks.Events.Products;
using EDrinks.Events.Tabs;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EDrinks.Helper.DataFiller
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true);

            var configuration = builder.Build();

            var ipAddress = configuration.GetValue<string>("EventStore:IPAddress");
            var port = configuration.GetValue<int>("EventStore:Port");

            var settings = ConnectionSettings.Create();
            var connection = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Parse(ipAddress), port));
            connection.ConnectAsync().Wait();

            string stream = args[0];
            int numberOfTabs = Convert.ToInt32(args[1]);
            int numberOfProducts = Convert.ToInt32(args[2]);
            int numberOfOrders = Convert.ToInt32(args[3]);

            FillTabs(connection, stream, numberOfTabs, numberOfProducts, numberOfOrders);
        }

        static void FillTabs(IEventStoreConnection connection, string stream, int numberOfTabs, int numberOfProducts,
            int numberOfOrders)
        {
            var faker = new Faker();
            var tabIds = new List<Guid>();
            var productIds = new List<Guid>();

            for (int i = 0; i < numberOfTabs; i++)
            {
                var tabId = Guid.NewGuid();
                tabIds.Add(tabId);

                connection.AppendToStreamAsync(stream, ExpectedVersion.Any, GetEventData(new TabCreated()
                {
                    TabId = tabId
                })).Wait();
                connection.AppendToStreamAsync(stream, ExpectedVersion.Any, GetEventData(new TabNameChanged()
                {
                    TabId = tabId,
                    Name = $"{faker.Name.FirstName()} {faker.Name.LastName()}"
                })).Wait();
            }

            for (int i = 0; i < numberOfProducts; i++)
            {
                var productId = Guid.NewGuid();
                productIds.Add(productId);

                connection.AppendToStreamAsync(stream, ExpectedVersion.Any, GetEventData(new ProductCreated()
                {
                    ProductId = productId
                })).Wait();
                connection.AppendToStreamAsync(stream, ExpectedVersion.Any, GetEventData(new ProductNameChanged()
                {
                    ProductId = productId,
                    Name = faker.Commerce.Product()
                })).Wait();
                connection.AppendToStreamAsync(stream, ExpectedVersion.Any, GetEventData(new ProductPriceChanged()
                {
                    ProductId = productId,
                    Price = faker.Random.Decimal(0.5M, 3.0M)
                })).Wait();
            }

            if (tabIds.Count == 0 || productIds.Count == 0)
            {
                return;
            }

            var tabsWithOrders = new List<Guid>();
            for (int i = 0; i < numberOfOrders; i++)
            {
                if (i > 0 && i % 100 == 0)
                {
                    var settlementId = Guid.NewGuid();

                    foreach (var tabToSettle in tabsWithOrders)
                    {
                        connection.AppendToStreamAsync(stream, ExpectedVersion.Any, GetEventData(new TabSettled()
                        {
                            SettlementId = settlementId,
                            TabId = tabToSettle
                        }));
                    }

                    tabsWithOrders.Clear();
                    
                    Console.WriteLine($"{i}/{numberOfOrders}");
                }

                var tabId = tabIds[faker.Random.Int(0, tabIds.Count - 1)];
                var productId = productIds[faker.Random.Int(0, productIds.Count - 1)];

                tabsWithOrders.Add(tabId);
                connection.AppendToStreamAsync(stream, ExpectedVersion.Any, GetEventData(new ProductOrderedOnTab()
                {
                    OrderId = Guid.NewGuid(),
                    TabId = tabId,
                    ProductId = productId,
                    Quantity = faker.Random.Int(1, 10)
                })).Wait();
            }
        }

        static EventData GetEventData(BaseEvent evt)
        {
            var metaDataStr = JsonConvert.SerializeObject(evt.MetaData);
            var contentStr = JsonConvert.SerializeObject(evt);

            return new EventData(Guid.NewGuid(), evt.GetType().Name, true,
                Encoding.UTF8.GetBytes(contentStr), Encoding.UTF8.GetBytes(metaDataStr));
        }
    }
}