using Microsoft.AspNetCore.Http;

namespace Khidmah_Inventory.Application.Common.Interfaces;

public interface IFileValidationService
{
    void ValidateFile(IFormFile file, long maxSizeInBytes, string[] allowedExtensions);
    string[] GetAllowedImageExtensions();
    long GetMaxFileSizeInBytes();
}

