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
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductCode = product.ProductCode,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageURL = product.ImageUrl,
                Specifications = product.Specifications,

                Category = new CategoryProductResponse
                {
                    Id = product.Category!.CategoryId,
                    Name = product.Category.CategoryName
                },

                Brand = new BrandProductResponse
                {
                    Id = product.Brand!.BrandId,
                    Name = product.Brand.BrandName
                }
            };
        }

        public async Task<int> AddProduct(AddProductRequest request)
        {
            if (request.Price < 0 || request.StockQuantity < 0)
            {
                throw new Exception();
            }

            var newProduct = new Product
            {
                ProductName = request.ProductName,
                ProductCode = request.ProductCode,
                CategoryId = request.CategoryId,
                BrandId = request.BrandId,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                ImageUrl = request.ImageURL,
                Specifications = request.Specifications
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
            if (request.Price < 0 || request.StockQuantity < 0)
            {
                throw new Exception();
            }

            var product = await _productRepo.GetProductById(id);

            if (product == null)
                return false;


            product.ProductName = request.ProductName;
            product.CategoryId = request.CategoryId;
            product.BrandId = request.BrandId;
            product.Description = request.Description;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.ImageUrl = request.ImageURL;
            product.Specifications = request.Specifications;

            await _productRepo.Save();
            return true;
        } 


    }
}
