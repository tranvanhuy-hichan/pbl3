using Microsoft.EntityFrameworkCore;
using PBL3.Models; // Để nó nhận diện được class User chúng ta tạo lúc nãy

// Lưu ý: Đổi chữ PBL3 thành đúng tên Project của bạn nếu cần
namespace PBL3
{
    // BÍ QUYẾT LÀ ĐÂY: Phải có chữ ": DbContext" để C# biết đây là file Database
    public class shop_pbl3 : DbContext
    {
        // Bắt buộc phải có đoạn này để file Program.cs truyền được "Chìa khóa" (ConnectionString) vào
        public shop_pbl3(DbContextOptions<shop_pbl3> options) : base(options)
        {
        }

        // Khai báo bảng Users
        public DbSet<User> Users { get; set; }

        // Nếu sau này nhóm có thêm bảng Sản phẩm, Danh mục... thì cứ viết tiếp xuống dưới:
        // public DbSet<Product> Products { get; set; }
    }
}