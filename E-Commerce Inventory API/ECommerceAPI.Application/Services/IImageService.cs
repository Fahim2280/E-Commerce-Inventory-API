using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Services
{
    public interface IImageService
    {
     
        Task<string> ConvertToBase64Async(IFormFile file);

        Task<string> SaveImageToFileSystemAsync(IFormFile file, string subfolder = "products");

        Task<bool> DeleteImageFromFileSystemAsync(string imagePath);

        string GetImageUrl(string imagePath);

        bool IsValidImage(IFormFile file);

        int MaxFileSizeBytes { get; }

        string[] AllowedExtensions { get; }
    }
}