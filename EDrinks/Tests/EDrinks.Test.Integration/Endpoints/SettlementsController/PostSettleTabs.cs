using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.Events.Tabs;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.SettlementsController
{
    public class PostSettleTabs : ServiceTest
    {
        public PostSettleTabs(ServiceFixture fixture) : base(fixture)
        {
            DeleteStream();
        }

        [Fact]
        public async Task TestInvalidPayload()
        {
            var response = await CallEndpoint("invalid payload");
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestContainingUnknownTab()
        {
            var tabId = Guid.NewGuid();
            await WriteToStream(new TabCreated() {TabId = tabId});

            var response = await CallEndpoint(new[] {tabId, Guid.NewGuid()});
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestSettleTabs()
        {
            var tabId = Guid.NewGuid();
            await WriteToStream(new TabCreated() {TabId = tabId});

            var response = await CallEndpoint(new[] {tabId});
            
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Contains(response.Headers, e => e.Key == "Location");
        }

        private async Task<HttpResponseMessage> CallEndpoint(object payload)
        {
            var httpContent = Serialize(payload);
            return await _fixture.Client.PostAsync("/api/Settlements", httpContent);
        }
    }
}