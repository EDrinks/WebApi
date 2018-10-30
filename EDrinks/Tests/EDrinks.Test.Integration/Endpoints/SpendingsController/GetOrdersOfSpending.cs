using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.QueryHandlers.Model;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.SpendingsController
{
    public class GetOrdersOfSpending : ServiceTest
    {
        public GetOrdersOfSpending(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestNonExistentSpending()
        {
            var response = await CallEndpoint(Guid.NewGuid());
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestGetOrdersOfSpendingWithoutOrders()
        {
            var spendingId = await CreateSpending();

            var response = await CallEndpoint(spendingId);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var orders = await Deserialize<List<Order>>(response);
            Assert.Empty(orders);
        }
        
        [Fact]
        public async Task TestGetOrdersOfSpendingWithOrders()
        {
            var spendingId = await CreateSpending();
            var orderId = await _fixture.Generator.OrderOnSpending(spendingId);

            var response = await CallEndpoint(spendingId);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var orders = await Deserialize<List<Order>>(response);
            Assert.Single(orders);
            Assert.Contains(orders, order => order.Id == orderId);
        }

        private async Task<Guid> CreateSpending()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();

            return await _fixture.Generator.CreateSpending(tabId, productId, 10);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid spendingId)
        {
            return await _fixture.Client.GetAsync($"/api/Spendings/{spendingId}/Orders");
        }
    }
}