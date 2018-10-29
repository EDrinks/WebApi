using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.QueryHandlers.Model;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.SpendingsController
{
    public class GetSpending : ServiceTest
    {
        public GetSpending(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestNonExistentSpending()
        {
            var response = await CallEndpoint(Guid.NewGuid());
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestGetSpending()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            var spendingId = await _fixture.Generator.CreateSpending(tabId, productId, 10);

            var response = await CallEndpoint(spendingId);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var spending = await Deserialize<Spending>(response);
            Assert.Equal(spendingId, spending.Id);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid spendingId)
        {
            return await _fixture.Client.GetAsync($"/api/Spendings/{spendingId}");
        }
    }
}