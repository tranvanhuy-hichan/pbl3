using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using System;
using System.Linq;
using System.Threading.Tasks;
using PBL3.Models;
using PBL3;
namespace YourProjectName.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // Khai báo Database Context (Giả sử tên là AppDbContext)
        // private readonly AppDbContext _context;
        // public UsersController(AppDbContext context) { _context = context; }

        // 1. Lấy danh sách người dùng (GET /api/users)

        private readonly shop_pbl3 _context;

        // 2. Tạo hàm khởi tạo (Constructor) để tiêm Database vào
        public UsersController(shop_pbl3 context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            // Lấy danh sách từ Database (có phân trang cơ bản)
            // Giả sử bảng của bạn tên là Users (hoặc AppUsers)
            var users = await _context.Users
                                      .Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();

            var totalCount = await _context.Users.CountAsync();

            return Ok(new { data = users, totalCount = totalCount });
        }


        // 2. Lấy chi tiết 1 người dùng (GET /api/users/{id})

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Tìm người dùng trong DB có UserID trùng khớp
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng này!" }); // Trả về lỗi 404
            }

            return Ok(user); // Trả về 200 kèm toàn bộ thông tin người đó
        }

        // 3. Admin tạo tài khoản mới (POST /api/users)

        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Create([FromBody] CreateUserDto request)
        {
            // Bước 1: Chuyển dữ liệu DTO từ Frontend gửi lên thành đối tượng User thật
            var newUser = new User
            {
                Username = request.username,
                Password = request.password,
                FullName = request.fullName,
                Email = request.email,
                PhoneNumber = request.phoneNumber,
                Address = request.address,
                DateOfBirth = request.dateOfBirth,
                Role = request.role ?? "User",
                IsActive = true,

                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "Tạo tài khoản thành công", userId = newUser.UserID });
        }

        // 4. Cập nhật thông tin (PUT /api/users/{id})

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng để cập nhật!" });
            }

            // Chỉ cập nhật những trường mà Giao diện có gửi lên (khác null)
            if (request.fullName != null) user.FullName = request.fullName;
            if (request.email != null) user.Email = request.email;
            if (request.phoneNumber != null) user.PhoneNumber = request.phoneNumber;
            if (request.address != null) user.Address = request.address;
            if (request.dateOfBirth != null) user.DateOfBirth = request.dateOfBirth;
            if (request.role != null) user.Role = request.role;

            // Cập nhật lại thời gian sửa đổi cho đúng thực tế
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync(); // Lưu vào SQL

            return Ok(new { message = "Cập nhật thành công", updatedUser = user });
        }

        // 5. Khóa / Mở khóa tài khoản (PATCH /api/users/{id}/toggle-active)

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng!" });
            }

            // Đảo ngược trạng thái: Nếu đang True thì thành False, đang False thì thành True
            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã thay đổi trạng thái hoạt động", isActive = user.IsActive });
        }


        // 6. Xóa tài khoản (DELETE /api/users/{id})
 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng để xóa!" });
            }

            _context.Users.Remove(user); // Lệnh xóa
            await _context.SaveChangesAsync(); // Lưu thay đổi xuống SQL

            return Ok(new { message = "Đã xóa tài khoản thành công vĩnh viễn!" });
        }
    }
}