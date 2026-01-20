using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Suppliers.Models;

namespace Khidmah_Inventory.Application.Features.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommand : IRequest<Result<SupplierDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? TaxId { get; set; }
    public string? PaymentTerms { get; set; }
    public decimal? CreditLimit { get; set; }
}

