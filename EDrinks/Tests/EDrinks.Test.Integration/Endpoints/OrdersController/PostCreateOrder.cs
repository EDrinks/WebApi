using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EDrinks.Events.Products;
using EDrinks.Events.Tabs;
using EDrinks.WebApi.Dtos;
using Xunit;

namespace EDrinks.Test.Integration.Endpoints.OrdersController
{
    public class PostCreateOrder : ServiceTest
    {
        public PostCreateOrder(ServiceFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestNonExistantTab()
        {
            var response = await CallEndpoint(Guid.NewGuid(), await GetValidDto());

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestNonExistantProduct()
        {
            var tabId = Guid.NewGuid();
            await WriteToStream(new TabCreated() {TabId = tabId});

            var response = await CallEndpoint(tabId, new OrderDto()
            {
                ProductId = Guid.NewGuid(),
                Quantity = 1
            });
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestInvalidQuantity()
        {
            var tabId = Guid.NewGuid();
            await WriteToStream(new TabCreated() {TabId = tabId});
            var dto = await GetValidDto();
            dto.Quantity = 0;

            var response = await CallEndpoint(tabId, dto);
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestCreateOrder()
        {
            var tabId = Guid.NewGuid();
            await WriteToStream(new TabCreated() {TabId = tabId});
            var dto = await GetValidDto();

            var response = await CallEndpoint(tabId, dto);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<OrderDto> GetValidDto()
        {
            var productId = Guid.NewGuid();
            await WriteToStream(new ProductCreated() {ProductId = productId});

            return new OrderDto()
            {
                ProductId = productId,
                Quantity = 1
            };
        }

        private async Task<HttpResponseMessage> CallEndpoint(Guid tabId, object payload)
        {
            var httpContent = Serialize(payload);
            return await _fixture.Client.PostAsync($"/api/Tabs/{tabId}/Orders", httpContent);
        }
    }
}