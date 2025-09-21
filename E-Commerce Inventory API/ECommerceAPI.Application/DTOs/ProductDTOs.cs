﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ECommerceAPI.Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageBase64 { get; set; }
        public string? ImagePath { get; set; }
        public string? ImageUrl { get; set; } // Full URL to access the image
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 200 characters")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative number")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Valid category must be selected")]
        public int CategoryId { get; set; }

        public string? ImageBase64 { get; set; }
    }

    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 200 characters")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative number")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Valid category must be selected")]
        public int CategoryId { get; set; }

        public string? ImageBase64 { get; set; }
    }

    public class CreateProductWithFileDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 200 characters")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative number")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Valid category must be selected")]
        public int CategoryId { get; set; }

        public IFormFile? ImageFile { get; set; }
    }

    public class UpdateProductWithFileDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 200 characters")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative number")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Valid category must be selected")]
        public int CategoryId { get; set; }

        public IFormFile? ImageFile { get; set; }
    }

    public class ProductImageUploadDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Image file is required")]
        public IFormFile ImageFile { get; set; }
    }

    public class ImageStorageOptionsDto
    {
      
        public bool UseBase64Storage { get; set; } = false; // Default to file system

        public string SubFolder { get; set; } = "products";
    }
}

