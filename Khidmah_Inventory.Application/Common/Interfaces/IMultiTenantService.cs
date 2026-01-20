namespace Khidmah_Inventory.Application.Common.Interfaces;

public interface IMultiTenantService
{
    Guid? GetCurrentCompanyId();
    void SetCurrentCompanyId(Guid companyId);
    bool IsMultiTenantEnabled { get; }
}

