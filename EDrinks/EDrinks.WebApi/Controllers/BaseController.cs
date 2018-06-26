﻿using EDrinks.Common;
using Microsoft.AspNetCore.Mvc;

namespace EDrinks.WebApi.Controllers
{
    public class BaseController : Controller
    {
        protected IActionResult ResultToResponse(HandlerResult handlerResult)
        {
            switch (handlerResult.ResultCode)
            {
                case ResultCode.Ok:
                    return Ok();
                case ResultCode.Error:
                    return StatusCode(500);
                default:
                    return StatusCode(500);
            }
        }
    }
}