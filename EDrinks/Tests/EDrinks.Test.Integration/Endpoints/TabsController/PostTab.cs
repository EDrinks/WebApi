using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.QueryHandlers.Model;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.TabsController
{
    public class PostTab : ServiceTest
    {
        public PostTab(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestInvalidPayload()
        {
            var response = await CallEndpoint("invalid payload");
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestInvalidDto()
        {
            var tab = GetValidTab();
            tab.Name = null;

            var response = await CallEndpoint(tab);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            tab = GetValidTab();
            tab.Name = "ab";
            response = await CallEndpoint(tab);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestValidRequest()
        {
            var tab = GetValidTab();

            var response = await CallEndpoint(tab);
            
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Contains(response.Headers, e => e.Key == "Location");
        }

        private Tab GetValidTab()
        {
            return new Tab()
            {
                Name = "valid name"
            };
        }

        private async Task<HttpResponseMessage> CallEndpoint(object payload)
        {
            var httpContent = Serialize(payload);
            return await _fixture.Client.PostAsync("/api/Tabs", httpContent);
        }
    }
}