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
        }

        [Fact]
        public async Task TestNonExistentProduct()
        {
            var response = await CallEndpoint(Guid.NewGuid(), true);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var entries = await Deserialize<List<BarChartEntry>>(response);
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
            var entries = await Deserialize<List<BarChartEntry>>(response);
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
            var entries = await Deserialize<List<BarChartEntry>>(response);
            Assert.Equal(2, entries.Count);
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid productId, bool current)
        {
            return await _fixture.Client.GetAsync($"/api/Statistics/TopTen?productId={productId}&current={current}");
        }
    }
}