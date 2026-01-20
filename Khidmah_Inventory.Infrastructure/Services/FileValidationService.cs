using Microsoft.AspNetCore.Http;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.Infrastructure.Services;

public class FileValidationService : IFileValidationService
{
    private const long DEFAULT_MAX_FILE_SIZE = 5 * 1024 * 1024; // 5MB
    private static readonly string[] DEFAULT_IMAGE_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp" };

    public void ValidateFile(IFormFile file, long maxSizeInBytes, string[] allowedExtensions)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty or null");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            throw new ArgumentException(
                $"File type {extension} is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
        }

        if (file.Length > maxSizeInBytes)
        {
            var maxSizeInMB = maxSizeInBytes / (1024.0 * 1024.0);
            throw new ArgumentException($"File size exceeds {maxSizeInMB:F1}MB limit");
        }
    }

    public string[] GetAllowedImageExtensions()
    {
        return DEFAULT_IMAGE_EXTENSIONS;
    }

    public long GetMaxFileSizeInBytes()
    {
        return DEFAULT_MAX_FILE_SIZE;
    }
}

