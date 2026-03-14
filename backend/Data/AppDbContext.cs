using Microsoft.EntityFrameworkCore;
using pbl3_web_api.Entities; // Trỏ đến thư mục chứa class Category

namespace pbl3_web_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Khai báo bảng Categories trong CSDL
        public DbSet<Category> Categories { get; set; }
        // Thêm DbSet cho Brands để sử dụng repository/service mới
        public DbSet<Brand> Brands { get; set; }
    }
}