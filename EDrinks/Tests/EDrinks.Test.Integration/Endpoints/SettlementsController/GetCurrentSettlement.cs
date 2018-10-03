using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.QueryHandlers.Model;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.SettlementsController
{
    public class GetCurrentSettlement : ServiceTest
    {
        public GetCurrentSettlement(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestGetCurrentSettlement()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            await _fixture.Generator.OrderOnTab(tabId, productId, 4);

            var response = await CallEndpoint();
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var settlement = await Deserialize<Settlement>(response);
            Assert.Contains(settlement.TabToOrders, e => e.Tab.Id == tabId);
        }

        private async Task<HttpResponseMessage> CallEndpoint()
        {
            return await _fixture.Client.GetAsync("api/Settlements/Current");
        }
    }
}