using System;
using System.Threading.Tasks;
using EDrinks.QueryHandlers.Statistics;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EDrinks.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class StatisticsController : BaseController
    {
        private readonly IMediator _mediator;

        public StatisticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("TopTen")]
        public async Task<IActionResult> GetTopTenCurrent([FromQuery] Guid productId, [FromQuery] bool current = true)
        {
            var result = await _mediator.Send(new GetTopTenQuery()
            {
                ProductId = productId,
                Current = current
            });
            
            return ResultToResponse(result);
        }
    }
}