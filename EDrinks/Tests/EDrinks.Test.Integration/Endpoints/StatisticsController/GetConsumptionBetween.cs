using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.QueryHandlers.Model;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.StatisticsController
{
    public class GetConsumptionBetween : ServiceTest
    {
        public GetConsumptionBetween(ServiceFixture fixture) : base(fixture)
        {
            DeleteStream();
        }

        [Fact]
        public async Task TestNonExistentProduct()
        {
            var response = await CallEndpoint(Guid.NewGuid(), DateTime.Now.AddDays(-1), DateTime.Now);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var dataPoints = await Deserialize<List<DataPoint>>(response);
            Assert.Equal(2, dataPoints.Count);
            Assert.Equal(0, dataPoints[0].Value);
            Assert.Equal(0, dataPoints[1].Value);
        }

        [Fact]
        public async Task TestStartAfterEnd()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            await _fixture.Generator.OrderOnTab(tabId, productId, 1);
            
            var response = await CallEndpoint(productId, DateTime.Now.AddDays(1), DateTime.Now);
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task TestRangeTooWide()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            await _fixture.Generator.OrderOnTab(tabId, productId, 1);
            
            var response = await CallEndpoint(productId, new DateTime(2000, 1, 1), new DateTime(2010, 1, 1));
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestGetConsumptionBetween()
        {
            var tabId = await _fixture.Generator.CreateTab();
            var productId = await _fixture.Generator.CreateProduct();
            await _fixture.Generator.OrderOnTab(tabId, productId, 1);

            var response = await CallEndpoint(productId, DateTime.Now.AddDays(-1), DateTime.Now);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var dataPoints = await Deserialize<List<DataPoint>>(response);
            Assert.Contains(dataPoints, e => e.Value == 1);
        }

        [Fact]
        public async Task TestGetConsumptionBetweenAllProducts()
        {
            var tabOne = await _fixture.Generator.CreateTab();
            var tabTwo = await _fixture.Generator.CreateTab();
            var productOneId = await _fixture.Generator.CreateProduct();
            var productTwoId = await _fixture.Generator.CreateProduct();

            await _fixture.Generator.OrderOnTab(tabOne, productOneId);
            await _fixture.Generator.OrderOnTab(tabTwo, productTwoId);

            var response = await CallEndpoint(null, DateTime.Now.AddDays(-1), DateTime.Now);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var dataPoints = await Deserialize<List<DataPoint>>(response);
            Assert.Contains(dataPoints, e => e.Value == 2); // two products ordered overall
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid? productId, DateTime start, DateTime end)
        {
            string dateFormat = "yyyy-MM-dd";

            return await _fixture.Client.GetAsync("/api/Statistics/ConsumptionBetween"
                                                  + $"?productId={productId}"
                                                  + $"&start={start.ToString(dateFormat)}"
                                                  + $"&end={end.ToString(dateFormat)}");
        }
    }
}