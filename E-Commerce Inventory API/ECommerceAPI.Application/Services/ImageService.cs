using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _baseUrl;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _baseUrl = "/images/"; // This will be configurable
        }
        public int MaxFileSizeBytes => 5 * 1024 * 1024; // 5 MB

        public string[] AllowedExtensions => new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

        public async Task<string> ConvertToBase64Async(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is null or empty");

            if (!IsValidImage(file))
                throw new ArgumentException("Invalid image file");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();
            
            var mimeType = GetMimeType(file.FileName);
            var base64String = Convert.ToBase64String(fileBytes);
            
            return $"data:{mimeType};base64,{base64String}";
        }

        public bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > MaxFileSizeBytes)
                return false;

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                return false;

            // Additional validation: check if content type is image
            if (!file.ContentType.StartsWith("image/"))
                return false;

            return true;
        }

        public async Task<string> SaveImageToFileSystemAsync(IFormFile file, string subfolder = "products")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is null or empty");

            if (!IsValidImage(file))
                throw new ArgumentException("Invalid image file");

            // Create unique filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            
            // Create directory structure
            var uploadPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "images", subfolder);
            Directory.CreateDirectory(uploadPath);
            
            // Full file path
            var filePath = Path.Combine(uploadPath, fileName);
            
            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            
            // Return relative path
            return Path.Combine("images", subfolder, fileName).Replace("\\", "/");
        }

        public async Task<bool> DeleteImageFromFileSystemAsync(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return false;

            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, imagePath);
                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public string GetImageUrl(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return string.Empty;
                
            return $"{_baseUrl}{imagePath}";
        }

        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "image/jpeg" // Default fallback
            };
        }
    }
}