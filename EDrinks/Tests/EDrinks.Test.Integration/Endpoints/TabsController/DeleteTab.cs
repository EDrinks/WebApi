using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.TabsController
{
    public class DeleteTab : ServiceTest
    {
        public DeleteTab(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestNonExistentTab()
        {
            var response = await CallEndpoint(Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestTabWithOpenOrders()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            await _fixture.Generator.OrderOnTab(tabId, productId);

            var response = await CallEndpoint(tabId);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestDeleteTab()
        {
            var tabId = await _fixture.Generator.CreateTab();

            var response = await CallEndpoint(tabId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid tabId)
        {
            return await _fixture.Client.DeleteAsync($"/api/Tabs/{tabId}");
        }
    }
}