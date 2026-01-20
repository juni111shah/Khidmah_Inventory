namespace Khidmah_Inventory.Domain.Common;

public interface IMultiTenant
{
    Guid CompanyId { get; }
}

