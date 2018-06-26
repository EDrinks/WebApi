using System;
using System.Threading.Tasks;
using EDrinks.CommandHandlers;
using EDrinks.QueryHandlers;
using EDrinks.WebApi.Attributes;
using EDrinks.WebApi.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EDrinks.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IReadModel _readModel;

        public ProductsController(IMediator mediator, IReadModel readModel)
        {
            _mediator = mediator;
            _readModel = readModel;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _mediator.Send(new GetProductsQuery());
            
            return Ok(products);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            var result = await _mediator.Send(new CreateProductCommand()
            {
                Name = productDto.Name,
                Price = productDto.Price
            });

            if (!result) return StatusCode(500);

            return Ok();
        }

        [HttpPut("{productId}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid productId, [FromBody] ProductDto productDto)
        {
            if (!_readModel.Products.ContainsKey(productId))
            {
                return NotFound();
            }

            var result = await _mediator.Send(new UpdateProductCommand()
            {
                ProductId = productId,
                ProductName = productDto.Name,
                ProductPrice = productDto.Price
            });

            if (!result) return StatusCode(500);
            
            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid productId)
        {
            if (!_readModel.Products.ContainsKey(productId))
            {
                return NotFound();
            }

            var result = await _mediator.Send(new DeleteProductCommand()
            {
                ProductId = productId
            });

            if (!result) return StatusCode(500);
            
            return Ok();
        }
    }
}