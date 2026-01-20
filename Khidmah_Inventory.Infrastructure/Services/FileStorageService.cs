using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _baseUploadPath;

    public FileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
        _baseUploadPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads");
        Directory.CreateDirectory(_baseUploadPath);
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subdirectory, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty or null");
        }

        var directoryPath = Path.Combine(_baseUploadPath, subdirectory);
        Directory.CreateDirectory(directoryPath);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(directoryPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        return GetFileUrl(Path.Combine(subdirectory, fileName).Replace("\\", "/"));
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string subdirectory, CancellationToken cancellationToken = default)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new ArgumentException("File stream is empty or null");
        }

        var directoryPath = Path.Combine(_baseUploadPath, subdirectory);
        Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(directoryPath, fileName);

        using (var outputStream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(outputStream, cancellationToken);
        }

        return GetFileUrl(Path.Combine(subdirectory, fileName).Replace("\\", "/"));
    }

    public Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = Path.Combine(_baseUploadPath, filePath.Replace("/", "\\"));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public string GetFileUrl(string relativePath)
    {
        return $"/uploads/{relativePath.TrimStart('/')}";
    }
}

