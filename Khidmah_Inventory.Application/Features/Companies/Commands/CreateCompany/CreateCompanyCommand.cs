using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Companies.Models;

namespace Khidmah_Inventory.Application.Features.Companies.Commands.CreateCompany;

public class CreateCompanyCommand : IRequest<Result<CompanyDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Currency { get; set; }
    public string? TimeZone { get; set; }
}
