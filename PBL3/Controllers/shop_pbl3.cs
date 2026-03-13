using Microsoft.EntityFrameworkCore;
using PBL3.Models;

namespace PBL3
{
    public class shop_pbl3 : DbContext
    {
        public shop_pbl3(DbContextOptions<shop_pbl3> options) : base(options)
        {
        }

        // Khai báo bảng Users
        public DbSet<User> Users { get; set; }

    }
}