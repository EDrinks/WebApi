using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.Events.Products;
using EDrinks.WebApi.Dtos;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.ProductsController
{
    public class GetProduct : ServiceTest
    {
        public GetProduct(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestNonExistantProduct()
        {
            var response = await CallEndpoint(Guid.NewGuid());
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestGetProduct()
        {
            var productId = Guid.NewGuid();
            await WriteToStream(new ProductCreated() {ProductId = productId});

            var response = await CallEndpoint(productId);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await Deserialize<ProductDto>(response);
            Assert.NotNull(content);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid productId)
        {
            return await _fixture.Client.GetAsync($"/api/Products/{productId}");
        }
    }
}