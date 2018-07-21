using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.Events.Tabs;
using EDrinks.QueryHandlers.Model;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.TabsController
{
    public class GetTabs : ServiceTest
    {
        public GetTabs(ServiceFixture fixture) : base(fixture)
        {
            DeleteStream();
        }

        [Fact]
        public async Task TestGetEmptyTabs()
        {
            var response = await CallEndpoint();
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await Deserialize<List<Tab>>(response);
            Assert.Empty(content);
        }

        [Fact]
        public async Task TestGetTabs()
        {
            int numOfTabs = 3;

            for (int i = 0; i < numOfTabs; i++)
            {
                await WriteToStream(new TabCreated() {TabId = Guid.NewGuid()});
            }

            var response = await CallEndpoint();
            
            var content = await Deserialize<List<Tab>>(response);
            Assert.Equal(numOfTabs, content.Count);
        }

        private async Task<HttpResponseMessage> CallEndpoint()
        {
            return await _fixture.Client.GetAsync("/api/Tabs");
        }
    }
}