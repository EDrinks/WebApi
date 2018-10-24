using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.QueryHandlers.Model;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.SpendingsController
{
    public class GetSpendings : ServiceTest
    {
        public GetSpendings(ServiceFixture fixture) : base(fixture)
        {
            DeleteStream();
        }

        [Fact]
        public async Task TestGetEmptySpendings()
        {
            var response = await CallEndpoint();
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await Deserialize<List<Spending>>(response);
            Assert.Empty(content);
        }

        [Fact]
        public async Task TestGetSpendings()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            var spendingId = await _fixture.Generator.CreateSpending(tabId, productId, 10);

            var response = await CallEndpoint();
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await Deserialize<List<Spending>>(response);
            Assert.Single(content);
            Assert.Contains(content, e => e.Id == spendingId);
        }

        private async Task<HttpResponseMessage> CallEndpoint()
        {
            return await _fixture.Client.GetAsync("/api/Spendings");
        }
    }
}