using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDrinks.Common;
using EDrinks.QueryHandlers.Tabs;
using EDrinks.WebApi.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EDrinks.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class SettlementsController : BaseController
    {
        private readonly IMediator _mediator;

        public SettlementsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> SettleTabs([FromBody] List<Guid> tabIds)
        {
            foreach (var tabId in tabIds)
            {
                var readResult = await _mediator.Send(new GetTabQuery() {TabId = tabId});
                if (readResult.ResultCode != ResultCode.Ok) return BadRequest("One of the tabs does not exist");
            }

            return Ok();
        }
    }
}