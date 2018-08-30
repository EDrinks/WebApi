using System;
using System.Threading.Tasks;
using EDrinks.CommandHandlers.Products;
using EDrinks.Common;
using EDrinks.QueryHandlers.Products;
using EDrinks.WebApi.Attributes;
using EDrinks.WebApi.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EDrinks.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : BaseController
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetProductsQuery());

            return ResultToResponse(result);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetById([FromRoute] Guid productId)
        {
            var result = await _mediator.Send(new GetProductQuery() {Id = productId});

            return ResultToResponse(result);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            var productId = await _mediator.Send(new CreateProductCommand()
            {
                Name = productDto.Name,
                Price = productDto.Price
            });

            return Created($"/api/Products/{productId}", productId);
        }

        [HttpPut("{productId}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid productId, [FromBody] ProductDto productDto)
        {
            var readResult = await _mediator.Send(new GetProductQuery() {Id = productId});
            if (readResult.ResultCode != ResultCode.Ok) return ResultToResponse(readResult);

            var result = await _mediator.Send(new UpdateProductCommand()
            {
                ProductId = productId,
                ProductName = productDto.Name,
                ProductPrice = productDto.Price
            });

            return ResultToResponse(result);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid productId)
        {
            var readResult = await _mediator.Send(new GetProductQuery() {Id = productId});
            if (readResult.ResultCode != ResultCode.Ok) return ResultToResponse(readResult);

            var result = await _mediator.Send(new DeleteProductCommand()
            {
                ProductId = productId
            });

            return ResultToResponse(result);
        }
    }
}