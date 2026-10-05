using MediatR;
using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Categories.Commands.CreateCategory;
using StockFlow.Application.Categories.Queries.GetCategoryById;

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
            return CreatedAtAction(nameof(GetById), new { id = categoryId }, categoryId);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var category = await _mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);
            return category is null ? NotFound() : Ok(category);
        }
    }
}