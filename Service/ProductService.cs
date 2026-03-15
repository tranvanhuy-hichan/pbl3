using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using pbl3_web_api.DTO.Product;
using pbl3_web_api.Entities;
using pbl3_web_api.Repository;

namespace pbl3_web_api.Service
{
    public interface IProductService
    {
        Task<(IEnumerable<ProductsResponse> products, int totalCount)> GetProducts(
            int page,
            int pageSize,
            string? search,
            int? categoryId,
            int? brandId,
            decimal? minPrice,
            decimal? maxPrice);
        Task<ProductDetailResponse?> GetProductById(int id);
        Task<int> AddProduct(AddProductRequest request);
        Task DeleteProduct(int id);
        Task EditProduct(int id, EditProductRequest request);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;

        public ProductService(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<(IEnumerable<ProductsResponse> products, int totalCount)> GetProducts(
            int page,
            int pageSize,
            string? search,
            int? categoryId,
            int? brandId,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var productsQuery = _productRepo.GetProductsQuery();

            var query = ApplyFilters(productsQuery, search, categoryId, brandId, minPrice, maxPrice);

            var totalCount = await query.CountAsync();

            var products = await GetPagedProducts(query, page, pageSize);

            return (products, totalCount);
        }


        public async Task<ProductDetailResponse?> GetProductById(int id)
        {
            ValidateId(id, "ProductId");

            var product = await _productRepo.GetProductById(id)
                ?? throw new KeyNotFoundException("Product Not Found");

            return new ProductDetailResponse
            {
                productId = product.ProductId,
                productName = product.ProductName,
                productCode = product.ProductCode,
                description = product.Description,
                price = product.Price,
                stockQuantity = product.StockQuantity,
                imageURL = product.ImageUrl,
                specifications = product.Specifications,

                category = new CategoryProductResponse
                {
                    id = product.Category!.CategoryId,
                    name = product.Category.CategoryName
                },

                brand = new BrandProductResponse
                {
                    id = product.Brand!.BrandId,
                    name = product.Brand.BrandName
                }
            };
        }

        public async Task<int> AddProduct(AddProductRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (! await _productRepo.CheckCategoryAndBrandExist(request.categoryId, request.brandId))
                throw new ArgumentException("Category or Brand does not exist");

            if (request.price < 0)
                throw new ArgumentException("Price cannot be negative");

            if (request.stockQuantity < 0)
                throw new ArgumentException("StockQuantity cannot be negative");

            var newProduct = new Product
            {
                ProductName = request.productName,
                ProductCode = request.productCode,
                CategoryId = request.categoryId,
                BrandId = request.brandId,
                Description = request.description,
                Price = request.price,
                StockQuantity = request.stockQuantity,
                ImageUrl = request.imageURL,
                Specifications = request.specifications,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _productRepo.Add(newProduct);
            return newProduct.ProductId;
        }

        public async Task DeleteProduct(int id)
        {
            ValidateId(id, "ProductId");

            var product = await _productRepo.GetProductById(id)
                ?? throw new KeyNotFoundException("Product Not Found");

            product.IsActive = false;
            await _productRepo.Save();
        }

        public async Task EditProduct(int id, EditProductRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.price.HasValue && request.price < 0)
                throw new ArgumentException("Price cannot be negative");

            if (request.stockQuantity.HasValue && request.stockQuantity < 0)
                throw new ArgumentException("StockQuantity cannot be negative");

            var product = await _productRepo.GetProductById(id)
                ?? throw new KeyNotFoundException("Product Not Found");

            product.ProductName = request.productName ?? product.ProductName;
            product.CategoryId = request.categoryId ?? product.CategoryId;
            product.BrandId = request.brandId ?? product.BrandId;
            product.Description = request.description ?? product.Description;
            product.Price = request.price ?? product.Price;
            product.StockQuantity = request.stockQuantity ?? product.StockQuantity;
            product.ImageUrl = request.imageURL ?? product.ImageUrl;
            product.Specifications = request.specifications ?? product.Specifications;
            product.IsActive = request.isActive ?? product.IsActive;
            product.UpdatedAt = DateTime.Now;

            await _productRepo.Save();
        }

        private IQueryable<Product> ApplyFilters(
           IQueryable<Product> query,
           string? search,
           int? categoryId,
           int? brandId,
           decimal? minPrice,
           decimal? maxPrice)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = search.ToLower();

                query = query.Where(p =>
                    p.ProductName.ToLower().Contains(keyword) ||
                    (p.Description != null && p.Description.ToLower().Contains(keyword)) ||
                    (p.Specifications != null && p.Specifications.ToLower().Contains(keyword)) ||
                    (p.Brand != null && p.Brand.BrandName.ToLower().Contains(keyword)) ||
                    (p.Category != null && p.Category.CategoryName.ToLower().Contains(keyword))
                );
            }

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == brandId);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            return query;
        }

        private async Task<List<ProductsResponse>> GetPagedProducts(
            IQueryable<Product> query,
            int page,
            int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 12;

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductsResponse
                {
                    productId = p.ProductId,
                    productName = p.ProductName,
                    productCode = p.ProductCode,
                    price = p.Price,
                    stockQuantity = p.StockQuantity,
                    imageURL = p.ImageUrl,
                    categoryName = p.Category!.CategoryName,
                    brandName = p.Brand!.BrandName,
                    isActive = p.IsActive
                })
                .ToListAsync();
        }
        private void ValidateId(int id, string name)
        {
            if (id <= 0)
                throw new ArgumentException($"{name} must be a non-negative integer.");
        }
    }
}
