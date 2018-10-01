using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using EDrinks.CommandHandlers.Tabs;
using EDrinks.Common;
using EDrinks.QueryHandlers.Settlements;
using EDrinks.QueryHandlers.Tabs;
using EDrinks.WebApi.Attributes;
using EDrinks.WebApi.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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

        [HttpGet]
        public async Task<IActionResult> GetSettlements([FromQuery] GetSettlementsQuery request)
        {
            var result = await _mediator.Send(request);

            return ResultToResponse(result);
        }

        [HttpGet("{settlementId}")]
        public async Task<IActionResult> GetSettlement([FromRoute] Guid settlementId)
        {
            var returnType = "default";

            StringValues acceptHeader;
            if (Request.Headers.TryGetValue("Accept", out acceptHeader))
            {
                if (acceptHeader.Any(e => e == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                {
                    returnType = "xlsx";
                }
                else if (acceptHeader.Any(e => e == "application/pdf"))
                {
                    returnType = "pdf";
                }
            }

            var result = await _mediator.Send(new GetSettlementQuery()
            {
                SettlementId = settlementId
            });

            if (result.ResultCode != ResultCode.Ok)
            {
                return ResultToResponse(result);
            }

            switch (returnType)
            {
                case "xlsx":
                    var stream = SettlementTransformer.SettlementToXlsxStream(result.Payload);
                    return File(stream, "application/octet-stream");
                case "pdf":
                    throw new NotImplementedException();
                default:
                    return ResultToResponse(result);
            }
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

            var settlementId = await _mediator.Send(new SettleTabsCommand()
            {
                TabIds = tabIds
            });

            return Created($"/api/Settlements/{settlementId}", settlementId);
        }
    }
}