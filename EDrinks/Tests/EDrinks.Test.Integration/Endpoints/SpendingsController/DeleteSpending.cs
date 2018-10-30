using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.SpendingsController
{
    public class DeleteSpending : ServiceTest
    {
        public DeleteSpending(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestNonExistentSpending()
        {
            var response = await CallEndpoint(Guid.NewGuid());

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestDeleteSpending()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            var spendingId = await _fixture.Generator.CreateSpending(tabId, productId, 10);

            var response = await CallEndpoint(spendingId);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid spendingId)
        {
            return await _fixture.Client.DeleteAsync($"/api/Spendings/{spendingId}");
        }
    }
}