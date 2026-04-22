using pbl3_web_api.Entities;

namespace pbl3_web_api.Service
{
    public interface IUserService
    {
        Task<object> GetAllAsync(int page, int pageSize);
        Task<object> GetDeletedAsync(int page, int pageSize);
        Task<User?> GetByIdAsync(int id);
        Task<object> CreateAsync(CreateUserDto request);
        Task<object> UpdateAsync(int id, UpdateUserDto request);
        Task<object> ToggleActiveAsync(int id);
        Task<object> DeleteAsync(int id);
    }

}
