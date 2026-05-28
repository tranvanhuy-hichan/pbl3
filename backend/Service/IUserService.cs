using pbl3_web_api.Entities;

namespace pbl3_web_api.Service
{
    public interface IUserService
    {
        Task<ServiceResult<object>> GetAllAsync(int page, int pageSize);

        Task<ServiceResult<object>> GetDeletedAsync(int page, int pageSize);

        Task<ServiceResult<User>> GetByIdAsync(int id);

        Task<ServiceResult<object>> CreateAsync(CreateUserDto request);

        Task<ServiceResult<User>> UpdateAsync(int id, UpdateUserDto request);

        Task<ServiceResult<object>> ToggleActiveAsync(int id);

        Task<ServiceResult<object>> DeleteAsync(int id);
    }
}