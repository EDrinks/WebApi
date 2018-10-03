using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.OrdersController
{
    public class DeleteOrder : ServiceTest
    {
        public DeleteOrder(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestNonExistentTab()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            var orderId = await _fixture.Generator.OrderOnTab(tabId, productId);

            var response = await CallEndpoint(Guid.NewGuid(), orderId);
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task TestNonExistentOrder()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            var orderId = await _fixture.Generator.OrderOnTab(tabId, productId);

            var response = await CallEndpoint(tabId, Guid.NewGuid());
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task TestOrderBelongingToOtherTab()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            await _fixture.Generator.OrderOnTab(tabId, productId);
            
            var tabId2 = await _fixture.Generator.CreateTab();
            var orderId2 = await _fixture.Generator.OrderOnTab(tabId2, productId);

            var response = await CallEndpoint(tabId, orderId2);
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async Task TestDeleteOrder()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            var orderId = await _fixture.Generator.OrderOnTab(tabId, productId);

            var response = await CallEndpoint(tabId, orderId);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public async Task TestDeleteOrderMultipleTimes()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            var orderId = await _fixture.Generator.OrderOnTab(tabId, productId);

            var response = await CallEndpoint(tabId, orderId);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            response = await CallEndpoint(tabId, orderId);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid tabId, Guid orderId)
        {
            return await _fixture.Client.DeleteAsync($"/api/Tabs/{tabId}/Orders/{orderId}");
        }
    }
}