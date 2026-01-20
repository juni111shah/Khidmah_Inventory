using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Products.Models;

namespace Khidmah_Inventory.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<Result<ProductDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? MinStockLevel { get; set; }
    public decimal? MaxStockLevel { get; set; }
    public decimal? ReorderPoint { get; set; }
    public bool TrackQuantity { get; set; } = true;
    public bool TrackBatch { get; set; } = false;
    public bool TrackExpiry { get; set; } = false;
    public string? ImageUrl { get; set; }
    public decimal Weight { get; set; }
    public string? WeightUnit { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public string? DimensionsUnit { get; set; }
}

