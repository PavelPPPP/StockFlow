using MediatR;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Categories.Commands.CreateCategory;

namespace StockFlow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var categoryId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(Create), new { id = categoryId }, categoryId);
        }
    }
}