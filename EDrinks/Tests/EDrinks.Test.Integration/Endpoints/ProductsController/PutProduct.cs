using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.Events.Products;
using EDrinks.WebApi.Dtos;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.ProductsController
{
    public class PutProduct : ServiceTest
    {
        public PutProduct(ServiceFixture fixture) : base(fixture)
        {
            DeleteStream();
        }

        [Fact]
        public async Task TestNonExistantProduct()
        {
            var response = await CallEndpoint(Guid.NewGuid(), GetValidProduct());

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestInvalidPayload()
        {
            var productId = Guid.NewGuid();
            await WriteToStream(new ProductCreated() {ProductId = productId});

            var response = await CallEndpoint(productId, "invalid payload");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestInvalidProduct()
        {
            var productId = Guid.NewGuid();
            await WriteToStream(new ProductCreated() {ProductId = productId});
            
            var product = GetValidProduct();
            product.Name = "";
            var response = await CallEndpoint(productId, product);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            product = GetValidProduct();
            product.Name = null;
            response = await CallEndpoint(productId, product);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            product = GetValidProduct();
            product.Price = -1;
            response = await CallEndpoint(productId, product);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestValidProduct()
        {
            var productId = Guid.NewGuid();
            await WriteToStream(new ProductCreated() {ProductId = productId});
            var product = GetValidProduct();

            var response = await CallEndpoint(productId, product);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid productId, object payload)
        {
            var httpContent = Serialize(payload);
            return await _fixture.Client.PutAsync($"/api/Products/{productId}", httpContent);
        }

        private ProductDto GetValidProduct()
        {
            return new ProductDto()
            {
                Name = "product",
                Price = 1
            };
        }
    }
}