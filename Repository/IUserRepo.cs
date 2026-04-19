using pbl3_web_api.Entities;
using System.Threading.Tasks;

namespace pbl3_web_api.Repository;

public interface IUserRepo
{
    Task<bool> CheckUsernameExistsAsync(string username);
    Task<bool> CheckEmailExistsAsync(string email);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByIdAsync(int id);
    Task<int> AddUserAsync(User user);
    Task UpdateUserAsync(User user);
 
}
