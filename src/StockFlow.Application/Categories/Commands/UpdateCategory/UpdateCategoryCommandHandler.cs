using MediatR;
using StockFlow.Application.Common.Interfaces;

namespace StockFlow.Application.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

            // Null-forgiving (!): existence is already guaranteed by UpdateCategoryCommandValidator
            // (RuleFor(x => x.Id).MustAsync(Exist)), which runs earlier in the MediatR pipeline
            // via ValidationBehavior — category is never null here in the normal flow.
            category!.UpdateDetails(request.Name, request.ParentCategoryId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}