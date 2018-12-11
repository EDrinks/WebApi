using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.QueryHandlers.Model;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.StatisticsController
{
    public class GetTopTen : ServiceTest
    {
        public GetTopTen(ServiceFixture fixture) : base(fixture)
        {
            DeleteStream();
        }

        [Fact]
        public async Task TestNonExistentProduct()
        {
            var response = await CallEndpoint(Guid.NewGuid(), true);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var entries = await Deserialize<List<DataPoint>>(response);
            Assert.Empty(entries);
        }

        [Fact]
        public async Task TestGetCurrentTopTen()
        {
            var tabOne = await _fixture.Generator.CreateTab();
            var tabTwo = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();

            await _fixture.Generator.OrderOnTab(tabOne, productId);
            await _fixture.Generator.CreateSettlement(new[] {tabOne});
            await _fixture.Generator.OrderOnTab(tabTwo, productId);

            var response = await CallEndpoint(productId, true);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var entries = await Deserialize<List<DataPoint>>(response);
            Assert.Single(entries);
        }
        
        [Fact]
        public async Task TestGetAllTopTen()
        {
            var tabOne = await _fixture.Generator.CreateTab();
            var tabTwo = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();

            await _fixture.Generator.OrderOnTab(tabOne, productId);
            await _fixture.Generator.CreateSettlement(new[] {tabOne});
            await _fixture.Generator.OrderOnTab(tabTwo, productId);

            var response = await CallEndpoint(productId, false);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var entries = await Deserialize<List<DataPoint>>(response);
            Assert.Equal(2, entries.Count);
        }

        [Fact]
        public async Task TestGetAllProductsTopTen()
        {
            var tabOne = await _fixture.Generator.CreateTab();
            var tabTwo = await _fixture.Generator.CreateTab();
            var productOneId = await _fixture.Generator.CreateProduct();
            var productTwoId = await _fixture.Generator.CreateProduct();

            await _fixture.Generator.OrderOnTab(tabOne, productOneId);
            await _fixture.Generator.OrderOnTab(tabTwo, productTwoId);

            var response = await CallEndpoint(null, false);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var entries = await Deserialize<List<DataPoint>>(response);
            Assert.Equal(2, entries.Count);
            Assert.Equal(1, entries[0].Value);
            Assert.Equal(1, entries[1].Value);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid? productId, bool current)
        {
            return await _fixture.Client.GetAsync($"/api/Statistics/TopTen?productId={productId}&current={current}");
        }
    }
}