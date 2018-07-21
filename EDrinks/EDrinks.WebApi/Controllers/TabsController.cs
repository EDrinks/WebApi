using System.Threading.Tasks;
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
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTab([FromBody] TabDto tab)
        {
            return null;
        }
    }
}