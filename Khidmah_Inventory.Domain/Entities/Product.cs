using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Product : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string SKU { get; private set; } = string.Empty;
    public string? Barcode { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Guid? BrandId { get; private set; }
    public Guid UnitOfMeasureId { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public decimal SalePrice { get; private set; }
    public decimal? CostPrice { get; private set; }
    public decimal? MinStockLevel { get; private set; }
    public decimal? MaxStockLevel { get; private set; }
    public decimal? ReorderPoint { get; private set; }
    public bool TrackQuantity { get; private set; } = true;
    public bool TrackBatch { get; private set; } = false;
    public bool TrackExpiry { get; private set; } = false;
    public bool IsActive { get; private set; } = true;
    public string? ImageUrl { get; private set; }
    public decimal Weight { get; private set; }
    public string? WeightUnit { get; private set; }
    public decimal? Length { get; private set; }
    public decimal? Width { get; private set; }
    public decimal? Height { get; private set; }
    public string? DimensionsUnit { get; private set; }

    // Navigation properties
    public virtual Category? Category { get; private set; }
    public virtual Brand? Brand { get; private set; }
    public virtual UnitOfMeasure UnitOfMeasure { get; private set; } = null!;
    public virtual ICollection<ProductVariant> Variants { get; private set; } = new List<ProductVariant>();
    public virtual ICollection<ProductImage> Images { get; private set; } = new List<ProductImage>();
    public virtual ICollection<StockTransaction> StockTransactions { get; private set; } = new List<StockTransaction>();

    private Product() { }

    public Product(
        Guid companyId,
        string name,
        string sku,
        Guid unitOfMeasureId,
        decimal purchasePrice,
        decimal salePrice,
        Guid? categoryId = null,
        Guid? brandId = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Name = name;
        SKU = sku;
        UnitOfMeasureId = unitOfMeasureId;
        PurchasePrice = purchasePrice;
        SalePrice = salePrice;
        CategoryId = categoryId;
        BrandId = brandId;
    }

    public void Update(
        string name,
        string? description,
        string? barcode,
        Guid? categoryId,
        Guid? brandId,
        decimal purchasePrice,
        decimal salePrice,
        decimal? costPrice,
        decimal? minStockLevel,
        decimal? maxStockLevel,
        decimal? reorderPoint,
        bool trackQuantity,
        bool trackBatch,
        bool trackExpiry,
        Guid? updatedBy = null)
    {
        Name = name;
        Description = description;
        Barcode = barcode;
        CategoryId = categoryId;
        BrandId = brandId;
        PurchasePrice = purchasePrice;
        SalePrice = salePrice;
        CostPrice = costPrice;
        MinStockLevel = minStockLevel;
        MaxStockLevel = maxStockLevel;
        ReorderPoint = reorderPoint;
        TrackQuantity = trackQuantity;
        TrackBatch = trackBatch;
        TrackExpiry = trackExpiry;
        UpdateAuditInfo(updatedBy);
    }

    public void Activate(Guid? updatedBy = null)
    {
        IsActive = true;
        UpdateAuditInfo(updatedBy);
    }

    public void Deactivate(Guid? updatedBy = null)
    {
        IsActive = false;
        UpdateAuditInfo(updatedBy);
    }

    public void GenerateBarcode()
    {
        if (string.IsNullOrEmpty(Barcode))
        {
            Barcode = $"BC{SKU}{DateTime.UtcNow:yyyyMMddHHmmss}";
        }
    }
}

