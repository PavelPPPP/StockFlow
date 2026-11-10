using MediatR;
using StockFlow.Domain.Enums;

namespace StockFlow.Application.Products.Commands.CreateProduct
{
    public record CreateProductCommand(
        string Sku,
        string Name,
        string? Description,
        Guid CategoryId,
        UnitOfMeasure UnitOfMeasure,
        string? Barcode) : IRequest<Guid>;
}