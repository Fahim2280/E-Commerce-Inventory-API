using AutoMapper;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Domain.Entities;
using ECommerceAPI.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(product.CategoryId);
                var productDto = _mapper.Map<ProductDto>(product);
                productDto.CategoryName = category?.Name;
                productDtos.Add(productDto);
            }

            return productDtos;
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            var category = await _unitOfWork.Categories.GetByIdAsync(product.CategoryId);
            var productDto = _mapper.Map<ProductDto>(product);
            productDto.CategoryName = category?.Name;

            return productDto;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {categoryId} not found");

            var products = await _unitOfWork.Products.FindAsync(p => p.CategoryId == categoryId);
            var productDtos = products.Select(product =>
            {
                var productDto = _mapper.Map<ProductDto>(product);
                productDto.CategoryName = category.Name;
                return productDto;
            });

            return productDtos;
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllProductsAsync();

            var products = await _unitOfWork.Products.FindAsync(p => 
                p.Name.ToLower().Contains(keyword.ToLower()) || 
                (p.Description != null && p.Description.ToLower().Contains(keyword.ToLower())));
                
            var productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(product.CategoryId);
                var productDto = _mapper.Map<ProductDto>(product);
                productDto.CategoryName = category?.Name;
                productDtos.Add(productDto);
            }

            return productDtos;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(createProductDto.CategoryId);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {createProductDto.CategoryId} not found");

            var product = _mapper.Map<Product>(createProductDto);
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveAsync();

            var productDto = _mapper.Map<ProductDto>(product);
            productDto.CategoryName = category.Name;

            return productDto;
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            var category = await _unitOfWork.Categories.GetByIdAsync(updateProductDto.CategoryId);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {updateProductDto.CategoryId} not found");

            _mapper.Map(updateProductDto, product);
            product.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveAsync();

            var productDto = _mapper.Map<ProductDto>(product);
            productDto.CategoryName = category.Name;

            return productDto;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return false;

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
