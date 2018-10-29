using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.SpendingsController
{
    public class PostOrderOnSpending : ServiceTest
    {
        public PostOrderOnSpending(ServiceFixture fixture) : base(fixture)
        {
            DeleteStream();
        }

        [Fact]
        public async Task TestInvalidPayload()
        {
            var spendingId = await CreateSpending();
            var response = await CallEndpoint(spendingId, "invalid payload");
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestUnknownSpending()
        {
            var response = await CallEndpoint(Guid.NewGuid(), new {quantity = 1});
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestInvalidQuantity()
        {
            var spendingId = await CreateSpending();

            var response = await CallEndpoint(spendingId, new {quantity = -1});
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            response = await CallEndpoint(spendingId, new {quantity = 0});
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            response = await CallEndpoint(spendingId, new {quantity = 1.3});
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestOrderAboveSpendingSize()
        {
            var spendingSize = 10;
            
            var spendingId = await CreateSpending(spendingSize);

            var response = await CallEndpoint(spendingId, new {quantity = spendingSize + 1});
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestOrderOnSpending()
        {
            var spendingId = await CreateSpending();

            var response = await CallEndpoint(spendingId, new {quantity = 1});
            
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var orderId = await Deserialize<string>(response);
            Assert.False(string.IsNullOrEmpty(orderId));
        }

        private async Task<Guid> CreateSpending(int quantity = 10)
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            return await _fixture.Generator.CreateSpending(tabId, productId, quantity);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid spendingId, object content)
        {
            var httpContent = Serialize(content);
            
            return await _fixture.Client.PostAsync($"/api/Spendings/{spendingId}/Orders", httpContent);
        }
    }
}