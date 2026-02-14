using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Companies.Models;

namespace Khidmah_Inventory.Application.Features.Companies.Queries.GetCompaniesList;

public class GetCompaniesListQuery : IRequest<Result<PagedResult<CompanyDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
    public bool? IsActive { get; set; }
}
