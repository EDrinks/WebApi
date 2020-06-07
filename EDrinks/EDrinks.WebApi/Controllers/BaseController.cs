using EDrinks.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EDrinks.WebApi.Controllers
{
    [Authorize]
    public class BaseController : ControllerBase
    {
        protected IActionResult ResultToResponse(HandlerResult handlerResult)
        {
            switch (handlerResult.ResultCode)
            {
                case ResultCode.Ok:
                    return Ok();
                case ResultCode.NotFound:
                    return NotFound();
                case ResultCode.Error:
                    return StatusCode(500);
                default:
                    return StatusCode(500);
            }
        }
        
        protected IActionResult ResultToResponse<TReturn>(HandlerResult<TReturn> handlerResult)
        {
            switch (handlerResult.ResultCode)
            {
                case ResultCode.Ok:
                    return Ok(handlerResult.Payload);
                case ResultCode.Created:
                    return Created("", handlerResult.Payload);
                case ResultCode.NotFound:
                    return NotFound();
                case ResultCode.Error:
                    return BadRequest(handlerResult.ErrorMessages);
                default:
                    return StatusCode(500);
            }
        }
    }
}