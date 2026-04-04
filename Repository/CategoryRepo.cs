using Microsoft.EntityFrameworkCore;
using pbl3_web_api.Entities;
using pbl3_web_api.Data;
namespace pbl3_web_api.Repository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
    public class CategoryRepo : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }
        public async Task<Category> AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }
        public async Task UpdateAsync(Category category)
        {
            // Đánh dấu thực thể này đã bị thay đổi
            _context.Categories.Update(category);
            // Lưu thay đổi xuống CSDL
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Category category)
        {
            // Đánh dấu thực thể này là cần bị xoá
            _context.Categories.Remove(category);

            // Thực thi câu lệnh DELETE xuống CSDL
            await _context.SaveChangesAsync();
        }
    }
}
