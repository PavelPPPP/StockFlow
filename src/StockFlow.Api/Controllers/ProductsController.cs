using MediatR;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Products.Commands.CreateProduct;

namespace StockFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var productId = await _mediator.Send(command, cancellationToken);

            // Temporarily: in the file feature/get-product-by-id-query, replace with CreatedAtAction.
            return Created($"/api/products/{productId}", productId);
        }
    }
}