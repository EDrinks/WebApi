using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDrinks.WebApi.Dtos;
using Xunit;

namespace EDrinks.Test.Integration.Workflows
{
    public class SettlementWorkflows : ServiceTest
    {
        public SettlementWorkflows(ServiceFixture fixture) : base(fixture)
        {
            DeleteStream();
        }

        [Fact]
        public async Task TestAfterSettlementTabShouldNotHaveActiveOrders()
        {
            var tabCreationResponse = await _fixture.Client.PostAsync("/api/Tabs", Serialize(new TabDto()
            {
                Name = "TestTab"
            }));
            var tabId = await Deserialize<Guid>(tabCreationResponse);

            var productCreationResponse = await _fixture.Client.PostAsync("/api/Products", Serialize(new ProductDto()
            {
                Name = "TestProduct",
                Price = 1.5M
            }));
            var productId = await Deserialize<Guid>(productCreationResponse);

            await _fixture.Client.PostAsync($"/api/Tabs/{tabId}/Orders", Serialize(new OrderDto()
            {
                ProductId = productId,
                Quantity = 1
            }));

            var ordersResponse = await _fixture.Client.GetAsync($"/api/Tabs/{tabId}/Orders");
            var orders = await Deserialize<List<OrderDto>>(ordersResponse);
            Assert.NotEmpty(orders);

            await _fixture.Client.PostAsync("/api/Settlements", Serialize(new[] {tabId}));
            ordersResponse = await _fixture.Client.GetAsync($"/api/Tabs/{tabId}/Orders");
            orders = await Deserialize<List<OrderDto>>(ordersResponse);
            Assert.Empty(orders);
        }
    }
}