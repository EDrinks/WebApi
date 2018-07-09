using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.Events.Products;
using EDrinks.QueryHandlers.Model;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.ProductsController
{
    public class GetProducts : ServiceTest
    {
        public GetProducts(ServiceFixture fixture) : base(fixture)
        {
            DeleteStream();
        }

        [Fact]
        public async Task TestGetEmptyProducts()
        {
            var response = await CallEndpoint();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await Deserialize<List<Product>>(response);
            Assert.Empty(content);
        }

        [Fact]
        public async Task TestGetProducts()
        {
            int numOfProducts = 3;

            for (int i = 0; i < numOfProducts; i++)
            {
                await WriteToStream(new ProductCreated() {ProductId = Guid.NewGuid()});
            }

            var response = await CallEndpoint();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await Deserialize<List<Product>>(response);
            Assert.Equal(numOfProducts, content.Count);
        }

        private async Task<HttpResponseMessage> CallEndpoint()
        {
            return await _fixture.Client.GetAsync("/api/Products");
        }
    }
}