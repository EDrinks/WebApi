using System;
using System.Threading.Tasks;
using EDrinks.CommandHandlers.Tabs;
using EDrinks.Common;
using EDrinks.QueryHandlers.Tabs;
using EDrinks.WebApi.Attributes;
using EDrinks.WebApi.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EDrinks.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class TabsController : BaseController
    {
        private readonly IMediator _mediator;

        public TabsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetTabsQuery());
            
            return ResultToResponse(result);
        }

        [HttpGet("{tabId}")]
        public async Task<IActionResult> GetSingle([FromRoute] Guid tabId)
        {
            var result = await _mediator.Send(new GetTabQuery() {TabId = tabId});

            return ResultToResponse(result);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateTab([FromBody] TabDto tab)
        {
            var tabId = await _mediator.Send(new CreateTabCommand()
            {
                Name = tab.Name
            });

            return Created($"/api/Tabs/{tabId}", tabId);
        }
        
        [HttpPut("{tabId}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateTab([FromRoute] Guid tabId, [FromBody] TabDto tab)
        {
            var readResult = await _mediator.Send(new GetTabQuery() {TabId = tabId});
            if (readResult.ResultCode != ResultCode.Ok) return ResultToResponse(readResult);
            
            var result = await _mediator.Send(new UpdateTabCommand()
            {
                TabId = tabId,
                Name = tab.Name
            });
            
            return ResultToResponse(result);
        }

        [HttpDelete("{tabId}")]
        public async Task<IActionResult> DeleteTab([FromRoute] Guid tabId)
        {
            var readResult = await _mediator.Send(new GetTabQuery() {TabId = tabId});
            if (readResult.ResultCode != ResultCode.Ok) return ResultToResponse(readResult);

            var result = await _mediator.Send(new DeleteTabCommand()
            {
                TabId = tabId
            });
            
            return ResultToResponse(result);
        }
    }
}