using System.Threading.Tasks;
using EDrinks.WebApi.Attributes;
using EDrinks.WebApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EDrinks.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return null;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProduct createProduct)
        {
            return Ok();
        }
    }
}