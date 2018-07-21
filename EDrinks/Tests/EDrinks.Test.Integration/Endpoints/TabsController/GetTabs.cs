using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.QueryHandlers.Model;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.TabsController
{
    public class GetTabs : ServiceTest
    {
        public GetTabs(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestGetEmptyTabs()
        {
            var response = await CallEndpoint();
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await Deserialize<List<Tab>>(response);
            Assert.Empty(content);
        }

        private async Task<HttpResponseMessage> CallEndpoint()
        {
            return await _fixture.Client.GetAsync("/api/Tabs");
        }
    }
}