using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.Events.Tabs;
using EDrinks.WebApi.Dtos;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.TabsController
{
    public class PutTab : ServiceTest
    {
        public PutTab(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestNonExistantTab()
        {
            var response = await CallEndpoint(Guid.NewGuid(), new TabDto() {Name = "new name"});

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestInvalidBody()
        {
            var tabId = Guid.NewGuid();
            await WriteToStream(new TabCreated() {TabId = tabId});

            var response = await CallEndpoint(tabId, "invalid body");
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestInvalidTab()
        {
            var tabId = Guid.NewGuid();
            await WriteToStream(new TabCreated() {TabId = tabId});

            var response = await CallEndpoint(tabId, new TabDto() { Name = ""});
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            response = await CallEndpoint(tabId, new TabDto() { Name = null});
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            response = await CallEndpoint(tabId, new TabDto() { Name = string.Join("a", new string[200])});
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestUpdateTab()
        {
            var tabId = Guid.NewGuid();
            await WriteToStream(new TabCreated() {TabId = tabId});

            var response = await CallEndpoint(tabId, new TabDto() { Name = "new name"});
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid tabId, object payload)
        {
            var httpContent = Serialize(payload);
            return await _fixture.Client.PutAsync($"/api/Tabs/{tabId}", httpContent);
        }
    }
}