using pbl3_web_api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace pbl3_web_api.Repository;

public class UserRepo : IUserRepo
{
    private readonly ShopPbl3Context _db;

    public UserRepo(ShopPbl3Context db)
    {
        _db = db;
    }

    public async Task<bool> CheckUsernameExistsAsync(string username)
    {
        return await _db.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> CheckEmailExistsAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.UserId == id);
    }

    public async Task<int> AddUserAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user.UserId;
    }
    public async Task UpdateUserAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }
}
