using System;
using System.Threading.Tasks;
using EDrinks.CommandHandlers.Orders;
using EDrinks.Common;
using EDrinks.QueryHandlers.Orders;
using EDrinks.QueryHandlers.Products;
using EDrinks.QueryHandlers.Tabs;
using EDrinks.WebApi.Attributes;
using EDrinks.WebApi.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EDrinks.WebApi.Controllers
{
    [Route("api")]
    public class OrdersController : BaseController
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("Tabs/{tabId}/Orders")]
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromRoute] Guid tabId)
        {
            var tabResult = await _mediator.Send(new GetTabQuery() {TabId = tabId});
            if (tabResult.ResultCode != ResultCode.Ok) return ResultToResponse(tabResult);

            var result = await _mediator.Send(new GetOrdersOfTabQuery()
            {
                TabId = tabId
            });
            
            return ResultToResponse(result);
        }

        [Route("Tabs/{tabId}/Orders")]
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateOrder([FromRoute] Guid tabId, [FromBody] OrderDto order)
        {
            var tabResult = await _mediator.Send(new GetTabQuery() {TabId = tabId});
            if (tabResult.ResultCode != ResultCode.Ok) return ResultToResponse(tabResult);

            var productResult = await _mediator.Send(new GetProductQuery() {Id = order.ProductId});
            if (productResult.ResultCode != ResultCode.Ok) return BadRequest("Error fetching product");

            var result = await _mediator.Send(new OrderProductOnTabCommand()
            {
                TabId = tabId,
                ProductId = order.ProductId,
                Quantity = order.Quantity
            });

            return ResultToResponse(result);
        }
    }
}