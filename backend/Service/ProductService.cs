using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using pbl3_web_api.Entities;
using pbl3_web_api.Repository;
using pbl3_web_api.RequestDTO;

namespace pbl3_web_api.Service
{
    public class ProductService
    {
        private readonly Repository.ProductRepo _productRepo;

        public ProductService(Repository.ProductRepo productRepo)
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
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 12;

            var query = _productRepo.GetProductsQuery();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            if (brandId.HasValue)
            {
                query = query.Where(p => p.BrandId == brandId);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            var totalCount = await query.CountAsync();

            var products = await query
                .Include(p => p.Category)
                .Include(p => p.Brand)
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

            return (products, totalCount);
        }


        public async Task<ProductDetailResponse?> GetProductById(int id)
        {
            var product = await _productRepo.GetProductById(id);
            if (product == null)
                return null;

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
            if (request.price < 0 || request.stockQuantity < 0)
            {
                throw new Exception();
            }

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

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _productRepo.GetProductById(id);
            if (product == null)
                return false;

            product.IsActive = false;

            await _productRepo.Save();
            return true;
        }

        public async Task<bool> EditProduct(int id, EditProductRequest request)
        {
            if (request.price < 0 || request.stockQuantity < 0)
            {
                throw new Exception();
            }

            var product = await _productRepo.GetProductById(id);

            if (product == null)
                return false;

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
            return true;
        } 


    }
}
