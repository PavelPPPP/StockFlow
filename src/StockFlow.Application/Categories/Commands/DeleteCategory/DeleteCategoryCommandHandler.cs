using MediatR;
using StockFlow.Application.Common.Interfaces;

namespace StockFlow.Application.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

            // Null-forgiving (!): existence is already guaranteed by DeleteCategoryCommandValidator
            // (RuleFor(x => x.Id).MustAsync(Exist)), which runs earlier in the MediatR pipeline
            // via ValidationBehavior, so category is never null here in the normal flow.
            _categoryRepository.Remove(category!);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}