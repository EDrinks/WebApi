using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.CommandHandlers.Spendings;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.SpendingsController
{
    public class CreateSpending : ServiceTest
    {
        public CreateSpending(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        private async Task TestInvalidPayload()
        {
            var response = await CallEndpoint(null);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            response = await CallEndpoint("invalid body");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestInvalidTab()
        {
            var productId = await _fixture.Generator.CreateProduct();
            var response = await CallEndpoint(new CreateSpendingCommand()
            {
                TabId = Guid.NewGuid(),
                ProductId = productId,
                Quantity = 5
            });
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task TestInvalidProduct()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var response = await CallEndpoint(new CreateSpendingCommand()
            {
                TabId = tabId,
                ProductId = Guid.NewGuid(),
                Quantity = 5
            });
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task TestInvalidQuantity()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            var response = await CallEndpoint(new CreateSpendingCommand()
            {
                TabId = tabId,
                ProductId = productId,
                Quantity = -1
            });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            response = await CallEndpoint(new CreateSpendingCommand()
            {
                TabId = tabId,
                ProductId = productId,
                Quantity = 0
            });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestCreateSpending()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            var response = await CallEndpoint(new CreateSpendingCommand()
            {
                TabId = tabId,
                ProductId = productId,
                Quantity = 5
            });
            
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        private async Task<HttpResponseMessage> CallEndpoint(object payload)
        {
            var httpPayload = Serialize(payload);
            return await _fixture.Client.PostAsync("/api/Spendings", httpPayload);
        }
    }
}