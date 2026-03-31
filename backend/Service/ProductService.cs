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
        Task<(IEnumerable<ProductsResponse> products, int totalCount)> GetProductsAsync(
            int page,
            int pageSize,
            string? search,
            int? categoryId,
            int? brandId,
            decimal? minPrice,
            decimal? maxPrice);
        Task<ProductDetailResponse?> GetProductByIdAsync(int id);
        Task<int> AddProductAsync(AddProductRequest request);
        Task DeleteProductAsync(int id);
        Task<List<ProductsResponse>> GetDeletedProductsAsync();
        Task RestoreProductAsync(int id);
        Task EditProductAsync(int id, EditProductRequest request);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;

        public ProductService(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<(IEnumerable<ProductsResponse> products, int totalCount)> GetProductsAsync(
            int page,
            int pageSize,
            string? search,
            int? categoryId,
            int? brandId,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var baseQuery = _productRepo.GetProductsQuery();

            var filteredQuery = ApplyFilters(baseQuery, search, categoryId, brandId, minPrice, maxPrice);

            var totalCount = await filteredQuery.CountAsync();

            var products = await GetPagedProductsAsync(filteredQuery, page, pageSize);

            return (products, totalCount);
        }


        public async Task<ProductDetailResponse?> GetProductByIdAsync(int id)
        {
            ValidateId(id, "ProductId");

            var product = await _productRepo.GetProductByIdAsync(id)
                ?? throw new KeyNotFoundException("Product Not Found");

            return MapToProductDetailResponse(product);
        }

        private async Task<string?> SaveImageAsync(IFormFile? file, string? oldImageUrl = null)
        {
            if (file == null) return oldImageUrl; // Nếu không có file mới, giữ nguyên URL cũ

            // 1. Xóa ảnh cũ nếu có
            if (!string.IsNullOrEmpty(oldImageUrl))
            {
                // Chuyển URL (/images/products/abc.jpg) thành đường dẫn vật lý trên ổ đĩa
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImageUrl.TrimStart('/'));

                if (File.Exists(oldFilePath))
                {
                    try
                    {
                        File.Delete(oldFilePath);
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi nếu không xóa được (do file đang bị khóa bởi tiến trình khác...)
                        Console.WriteLine($"Không thể xóa ảnh cũ: {ex.Message}");
                    }
                }
            }

            // 2. Lưu ảnh mới (Giữ nguyên logic của bạn)
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/products/{fileName}";
        }


        public async Task<int> AddProductAsync(AddProductRequest request)
        {
            await ValidateAddProductRequestAsync(request);

            var newProduct = MapToProduct(request);

            if (request.ImageFile != null)
            {
                newProduct.ImageUrl = await SaveImageAsync(request.ImageFile);
            }

            await _productRepo.AddProductAsync(newProduct);

            await _productRepo.SaveAsync();

            return newProduct.ProductId;
        }

        public async Task DeleteProductAsync(int id)
        {
            ValidateId(id, "ProductId");

            var product = await _productRepo.GetProductByIdAsync(id)
                ?? throw new KeyNotFoundException("Product Not Found");

            product.IsActive = false;

            await _productRepo.SaveAsync();
        }

        public async Task EditProductAsync(int id, EditProductRequest request)
        {
            ValidateEditProductRequest(request);

            var product = await _productRepo.GetProductByIdAsync(id)
                ?? throw new KeyNotFoundException("Product Not Found");

            if (request.ImageFile != null)
            {
                product.ImageUrl = await SaveImageAsync(request.ImageFile, product.ImageUrl);
            }

            ApplyChange(product, request);

            await _productRepo.SaveAsync();
        }
         
        public async Task<List<ProductsResponse>> GetDeletedProductsAsync()
        {
            IQueryable<Product> products = _productRepo.GetProductsQuery();

            products = products.Where(p => !p.IsActive);

            var deletedProducts = await products.Select(p => new ProductsResponse
            {
                productId = p.ProductId,
                productName = p.ProductName,
                productCode = p.ProductCode,
                price = p.Price,
                stockQuantity = p.StockQuantity,
                imageURL = p.ImageUrl,
                categoryName = p.Category!.CategoryName,
                brandName = p.Brand!.BrandName,
                isActive = p.IsActive,
                specifications = p.Specifications
            })
            .ToListAsync();

            return deletedProducts;
        }

        public async Task RestoreProductAsync(int id)
        {
            ValidateId(id, "ProductId");

            var product = await _productRepo.GetProductByIdAsync(id)
                ?? throw new KeyNotFoundException("Product Not Found");

            product.IsActive = true;

            await _productRepo.SaveAsync();
        }

        private IQueryable<Product> ApplyFilters(
           IQueryable<Product> query,
           string? search,
           int? categoryId,
           int? brandId,
           decimal? minPrice,
           decimal? maxPrice)
        {
            query = query.Where(p => p.IsActive);
            query = query.OrderBy(p => Guid.NewGuid());
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
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == brandId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return query;
        }

        private async Task<List<ProductsResponse>> GetPagedProductsAsync(
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
                    isActive = p.IsActive,
                    specifications = p.Specifications
                })
                .ToListAsync();
        }


        private void ValidateId(int id, string name)
        {
            if (id <= 0)
                throw new ArgumentException($"{name} must be a non-negative integer.");
        }

        private ProductDetailResponse MapToProductDetailResponse(Product product)
        {
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

        private void ValidateEditProductRequest(EditProductRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.price.HasValue && request.price < 0)
                throw new ArgumentException("Price cannot be negative");

            if (request.stockQuantity.HasValue && request.stockQuantity < 0)
                throw new ArgumentException("StockQuantity cannot be negative");
        }

        private void ApplyChange(Product product, EditProductRequest request)
        {
            product.ProductName = request.productName ?? product.ProductName;
            product.CategoryId = request.categoryId ?? product.CategoryId;
            product.BrandId = request.brandId ?? product.BrandId;
            product.Description = request.description ?? product.Description;
            product.Price = request.price ?? product.Price;
            product.StockQuantity = request.stockQuantity ?? product.StockQuantity;
            product.Specifications = request.specifications ?? product.Specifications;
            product.IsActive = request.isActive ?? product.IsActive;
            product.UpdatedAt = DateTime.UtcNow;
        }

        private async Task ValidateAddProductRequestAsync(AddProductRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!await _productRepo.CheckCategoryAndBrandExistAsync(request.categoryId, request.brandId))
                throw new ArgumentException("Category or Brand does not exist");

            if (request.price < 0)
                throw new ArgumentException("Price cannot be negative");

            if (request.stockQuantity < 0)
                throw new ArgumentException("StockQuantity cannot be negative");

        }

        private Product MapToProduct(AddProductRequest request)
        {
            return new Product
            {
                ProductName = request.productName,
                ProductCode = request.productCode,
                CategoryId = request.categoryId,
                BrandId = request.brandId,
                Description = request.description,
                Price = request.price,
                StockQuantity = request.stockQuantity,
                Specifications = request.specifications,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
