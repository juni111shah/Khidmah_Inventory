using Microsoft.AspNetCore.Http;

namespace Khidmah_Inventory.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string subdirectory, CancellationToken cancellationToken = default);
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string subdirectory, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
    string GetFileUrl(string relativePath);
}

