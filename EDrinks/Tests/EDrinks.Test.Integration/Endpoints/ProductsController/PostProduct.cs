using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.WebApi.Dtos;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.ProductsController
{
    public class PostProduct : ServiceTest
    {
        public PostProduct(ServiceFixture fixture) : base(fixture)
        {
            DeleteStream();
        }

        [Fact]
        public async Task TestInvalidPayload()
        {
            var response = await CallEndpoint("invalid payload");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestInvalidProduct()
        {
            var response = await CallEndpoint(new ProductDto()
            {
                Name = "",
                Price = 1
            });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            response = await CallEndpoint(new ProductDto()
            {
                Name = null,
                Price = 1
            });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            response = await CallEndpoint(new ProductDto()
            {
                Name = "product",
                Price = -1
            });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestCreateProduct()
        {
            var response = await CallEndpoint(new ProductDto()
            {
                Name = "product",
                Price = 1
            });
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<HttpResponseMessage> CallEndpoint(object payload)
        {
            var httpContent = Serialize(payload);
            return await _fixture.Client.PostAsync("/api/Products", httpContent);
        }
    }
}