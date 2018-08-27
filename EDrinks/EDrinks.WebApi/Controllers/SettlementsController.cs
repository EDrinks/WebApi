using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDrinks.WebApi.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace EDrinks.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class SettlementsController : BaseController
    {
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> SettleTabs([FromBody] List<Guid> tabIds)
        {
            return Ok();
        }
    }
}