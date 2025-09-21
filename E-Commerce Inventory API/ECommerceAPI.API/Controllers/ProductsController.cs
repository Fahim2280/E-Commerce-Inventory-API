﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.Services;
using ECommerceAPI.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger, IImageService imageService, IUnitOfWork unitOfWork)
        {
            _productService = productService;
            _logger = logger;
            _imageService = imageService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products");
                return StatusCode(500, new { message = "An error occurred while retrieving products", details = ex.Message });
            }
        }
        
        [HttpPost("with-file")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductDto>> CreateProductWithFile([FromForm] CreateProductWithFileDto model)
        {
            try
            {
                var createProductDto = new CreateProductDto
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Stock = model.Stock,
                    CategoryId = model.CategoryId
                };

                // Handle image upload if provided
                if (model.ImageFile != null)
                {
                    if (!_imageService.IsValidImage(model.ImageFile))
                    {
                        return BadRequest(new { message = "Invalid image file. Please upload a valid image file." });
                    }

                    createProductDto.ImageBase64 = await _imageService.ConvertToBase64Async(model.ImageFile);
                }

                var product = await _productService.CreateProductAsync(createProductDto);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product with file: {Error}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while creating the product", details = ex.Message });
            }
        }

        [HttpPut("{id}/with-file")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductDto>> UpdateProductWithFile(int id, [FromForm] UpdateProductWithFileDto model)
        {
            try
            {
                var updateProductDto = new UpdateProductDto
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Stock = model.Stock,
                    CategoryId = model.CategoryId
                };

                // Handle image upload if provided
                if (model.ImageFile != null)
                {
                    if (!_imageService.IsValidImage(model.ImageFile))
                    {
                        return BadRequest(new { message = "Invalid image file. Please upload a valid image file." });
                    }

                    updateProductDto.ImageBase64 = await _imageService.ConvertToBase64Async(model.ImageFile);
                }

                var product = await _productService.UpdateProductAsync(id, updateProductDto);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with file: {Error}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while updating the product", details = ex.Message });
            }
        }


        [HttpPost("{id}/upload-image")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductDto>> UploadProductImage(int id, [FromForm] IFormFile imageFile)
        {
            try
            {
                if (imageFile == null)
                {
                    return BadRequest(new { message = "Image file is required" });
                }

                if (!_imageService.IsValidImage(imageFile))
                {
                    return BadRequest(new { message = "Invalid image file. Please upload a valid image file." });
                }

                // Get existing product
                var existingProduct = await _productService.GetProductByIdAsync(id);
                
                var updateProductDto = new UpdateProductDto
                {
                    Name = existingProduct.Name,
                    Description = existingProduct.Description,
                    Price = existingProduct.Price,
                    Stock = existingProduct.Stock,
                    CategoryId = existingProduct.CategoryId,
                    ImageBase64 = await _imageService.ConvertToBase64Async(imageFile)
                };

                var product = await _productService.UpdateProductAsync(id, updateProductDto);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading product image: {Error}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while uploading the image", details = ex.Message });
            }
        }


        [HttpPost("{id}/upload-image-advanced")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProductDto>> UploadProductImageAdvanced(int id, [FromForm] IFormFile imageFile, [FromForm] bool useFileSystem = true)
        {
            try
            {
                if (imageFile == null)
                {
                    return BadRequest(new { message = "Image file is required" });
                }

                if (!_imageService.IsValidImage(imageFile))
                {
                    return BadRequest(new { message = "Invalid image file. Please upload a valid image file." });
                }

                // Get existing product
                var existingProduct = await _productService.GetProductByIdAsync(id);
                
                var updateProductDto = new UpdateProductDto
                {
                    Name = existingProduct.Name,
                    Description = existingProduct.Description,
                    Price = existingProduct.Price,
                    Stock = existingProduct.Stock,
                    CategoryId = existingProduct.CategoryId
                };

                if (useFileSystem)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingProduct.ImagePath))
                    {
                        await _imageService.DeleteImageFromFileSystemAsync(existingProduct.ImagePath);
                    }
                    
                    // Save to file system
                    updateProductDto.ImageBase64 = null; // Clear Base64 data
                    var imagePath = await _imageService.SaveImageToFileSystemAsync(imageFile, "products");
                    
                    // We'll need to update the Product entity directly since UpdateProductDto doesn't have ImagePath
                    var product = await _productService.UpdateProductAsync(id, updateProductDto);
                    
                    // Manually update ImagePath (this is a limitation of current DTO structure)
                    var productEntity = await _unitOfWork.Products.GetByIdAsync(id);
                    if (productEntity != null)
                    {
                        productEntity.ImagePath = imagePath;
                        productEntity.ImageBase64 = null;
                        productEntity.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.Products.Update(productEntity);
                        await _unitOfWork.SaveAsync();
                    }
                    
                    return Ok(await _productService.GetProductByIdAsync(id));
                }
                else
                {
                    // Store as Base64
                    updateProductDto.ImageBase64 = await _imageService.ConvertToBase64Async(imageFile);
                    
                    // Clear file path if exists
                    var productEntity = await _unitOfWork.Products.GetByIdAsync(id);
                    if (productEntity != null && !string.IsNullOrEmpty(productEntity.ImagePath))
                    {
                        await _imageService.DeleteImageFromFileSystemAsync(productEntity.ImagePath);
                        productEntity.ImagePath = null;
                    }
                    
                    var product = await _productService.UpdateProductAsync(id, updateProductDto);
                    return Ok(product);
                }
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading product image: {Error}", ex.Message);
                return StatusCode(500, new { message = "An error occurred while uploading the image", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {ProductId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the product", details = ex.Message });
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(categoryId);
                return Ok(products);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products" });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string q)
        {
            try
            {
                var products = await _productService.SearchProductsAsync(q);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products with keyword: {Keyword}", q);
                return StatusCode(500, new { message = "An error occurred while searching products", details = ex.Message });
            }
        }
     
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                var product = await _productService.UpdateProductAsync(id, updateProductDto);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the product" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                    return NotFound(new { message = $"Product with ID {id} not found" });

                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the product" });
            }
        }

    }
}
