using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.Events.Tabs;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.TabsController
{
    public class DeleteTab : ServiceTest
    {
        public DeleteTab(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestNonExistantTab()
        {
            var response = await CallEndpoint(Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestDeleteTab()
        {
            var tabId = Guid.NewGuid();
            await WriteToStream(new TabCreated() {TabId = tabId});

            var response = await CallEndpoint(tabId);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid tabId)
        {
            return await _fixture.Client.DeleteAsync($"/api/Tabs/{tabId}");
        }
    }
}