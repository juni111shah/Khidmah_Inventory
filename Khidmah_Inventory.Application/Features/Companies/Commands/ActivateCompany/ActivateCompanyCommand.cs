using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Companies.Models;

namespace Khidmah_Inventory.Application.Features.Companies.Commands.ActivateCompany;

public class ActivateCompanyCommand : IRequest<Result<CompanyDto>>
{
    public Guid Id { get; set; }
}
