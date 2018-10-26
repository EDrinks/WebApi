using System;
using System.Threading.Tasks;
using EDrinks.CommandHandlers.Spendings;
using EDrinks.Common;
using EDrinks.QueryHandlers.Products;
using EDrinks.QueryHandlers.Spendings;
using EDrinks.QueryHandlers.Tabs;
using EDrinks.WebApi.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EDrinks.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class SpendingsController : BaseController
    {
        private readonly IMediator _mediator;

        public SpendingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetSpendings()
        {
            var result = await _mediator.Send(new GetSpendingsQuery());
            
            return ResultToResponse(result);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateSpending([FromBody] CreateSpendingCommand command)
        {
            var tabResult = await _mediator.Send(new GetTabQuery() {TabId = command.TabId});
            var productResult = await _mediator.Send(new GetProductQuery() {Id = command.ProductId});

            if (tabResult.ResultCode != ResultCode.Ok)
            {
                ModelState.AddModelError("tabId", "Invalid Tab");
            }

            if (productResult.ResultCode != ResultCode.Ok)
            {
                ModelState.AddModelError("productId", "Invalid Product");
            }

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var spendingId = await _mediator.Send(command);
            
            return Created($"/api/Tabs/{spendingId}", spendingId);
        }

        [HttpPost("{spendingId}/Orders")]
        [ValidateModel]
        public async Task<IActionResult> CreateOrderOnSpending([FromRoute] Guid spendingId, 
            [FromBody] OrderOnSpendingCommand command)
        {
            var spending = await _mediator.Send(new GetSpendingQuery() {SpendingId = spendingId});

            if (spending.ResultCode == ResultCode.NotFound)
            {
                return NotFound();
            }

            if (spending.Payload.Current + command.Quantity > spending.Payload.Quantity)
            {
                return BadRequest("Order exceeds spending amount");
            }

            command.SpendingId = spendingId;
            await _mediator.Send(command);
            
            return Ok();
        }

        [HttpPost("{spendingId}")]
        public async Task<IActionResult> DeleteSpending(Guid spendingId)
        {
            return null;
        }
    }
}