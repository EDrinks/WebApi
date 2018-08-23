using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.Events.Orders;
using EDrinks.Events.Products;
using EDrinks.Events.Tabs;
using EDrinks.QueryHandlers.Model;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.OrdersController
{
    public class GetCurrentOrders : ServiceTest
    {
        public GetCurrentOrders(ServiceFixture fixture) : base(fixture)
        {
            DeleteStream();
        }

        [Fact]
        public async Task TestGetCurrentOrders()
        {
            for (int i = 0; i < 2; i++)
            {
                var tabId = Guid.NewGuid();
                await WriteToStream(new TabCreated() {TabId = tabId});
                var productId = Guid.NewGuid();
                await WriteToStream(new ProductCreated() {ProductId = productId});
                await WriteToStream(new ProductOrderedOnTab()
                {
                    TabId = tabId,
                    ProductId = productId,
                    Quantity = 1,
                    OrderId = Guid.NewGuid()
                });
            }

            var response = await CallEndpoint();
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var orders = await Deserialize<List<Order>>(response);
            
            Assert.Equal(2, orders.Count);
        }

        private async Task<HttpResponseMessage> CallEndpoint()
        {
            return await _fixture.Client.GetAsync("/api/Orders");
        }
    }
}