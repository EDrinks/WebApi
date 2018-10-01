using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.Events.Tabs;
using EDrinks.QueryHandlers.Model;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.SettlementsController
{
    public class GetSettlement : ServiceTest
    {
        public GetSettlement(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestUnknownSettlement()
        {
            var response = await CallEndpoint(Guid.NewGuid());
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestGetSettlement()
        {
            var tabId = Guid.NewGuid();
            var settlementId = Guid.NewGuid();
            await WriteToStream(new TabCreated() {TabId = tabId});
            await WriteToStream(new TabSettled() {SettlementId = settlementId, TabId = tabId});

            var response = await CallEndpoint(settlementId);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var settlement = await Deserialize<Settlement>(response);
            Assert.NotNull(settlement);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid settlementId)
        {
            return await _fixture.Client.GetAsync($"/api/Settlements/{settlementId}");
        }
    }
}