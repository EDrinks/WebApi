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
    public class GetOrders : ServiceTest
    {
        public GetOrders(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestNonExistantTab()
        {
            var response = await CallEndpoint(Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestGetOrdersOfTab()
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

            var response = await CallEndpoint(tabId);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var orders = await Deserialize<List<Order>>(response);
            Assert.Single(orders);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid tabId)
        {
            return await _fixture.Client.GetAsync($"/api/Tabs/{tabId}/Orders");
        }
    }
}