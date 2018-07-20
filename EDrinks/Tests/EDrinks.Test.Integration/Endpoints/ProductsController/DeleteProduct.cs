using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.Events.Products;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.ProductsController
{
    public class DeleteProduct : ServiceTest
    {
        public DeleteProduct(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestNonExistantProduct()
        {
            var response = await CallEndpoint(Guid.NewGuid());
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestDeleteProduct()
        {
            var productId = Guid.NewGuid();
            await WriteToStream(new ProductCreated() {ProductId = productId});

            var response = await CallEndpoint(productId);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid productId)
        {
            return await _fixture.Client.DeleteAsync($"/api/Products/{productId}");
        }
    }
}