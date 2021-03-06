using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.Events.Tabs;
using EDrinks.QueryHandlers.Model;
using EDrinks.QueryHandlers.Settlements;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.SettlementsController
{
    public class GetSettlements : ServiceTest
    {
        public GetSettlements(ServiceFixture fixture) : base(fixture)
        {
            DeleteStream();
        }

        [Fact]
        public async Task TestPageSizeTooBig()
        {
            var response = await CallEndpoint(new GetSettlementsQuery()
            {
                PageSize = 101
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestPageSizeNegative()
        {
            var response = await CallEndpoint(new GetSettlementsQuery()
            {
                PageSize = -1
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestOffsetNegative()
        {
            var response = await CallEndpoint(new GetSettlementsQuery()
            {
                Offset = -1
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestEndBeforeStart()
        {
            var response = await CallEndpoint(new GetSettlementsQuery()
            {
                Start = DateTime.Now.AddDays(1),
                End = DateTime.Now
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestGetSettlements()
        {
            var tabId = Guid.NewGuid();
            await WriteToStream(new TabCreated() {TabId = tabId});
            await WriteToStream(new TabSettled() {SettlementId = Guid.NewGuid(), TabId = tabId});

            var response = await CallEndpoint(new GetSettlementsQuery()
            {
                Start = DateTime.Now.AddDays(-5),
                End = DateTime.Now.AddDays(5)
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var settlements = await Deserialize<List<Settlement>>(response);
            Assert.NotEmpty(settlements);
        }

        private async Task<HttpResponseMessage> CallEndpoint(GetSettlementsQuery request)
        {
            string dateFormat = "yyyy-MM-dd";

            return await _fixture.Client.GetAsync("/api/Settlements"
                                                  + $"?start={request.Start.ToString(dateFormat)}"
                                                  + $"&end={request.End.ToString(dateFormat)}"
                                                  + $"&pageSize={request.PageSize}"
                                                  + $"&offset={request.Offset}");
        }
    }
}