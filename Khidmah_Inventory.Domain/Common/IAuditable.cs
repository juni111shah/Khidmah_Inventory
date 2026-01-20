namespace Khidmah_Inventory.Domain.Common;

public interface IAuditable
{
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
    Guid? CreatedBy { get; }
    Guid? UpdatedBy { get; }
}

