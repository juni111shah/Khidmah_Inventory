using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveCompanySettings;

public class SaveCompanySettingsCommand : IRequest<Result<CompanySettingsDto>>
{
    public CompanySettingsDto Settings { get; set; } = null!;
}

