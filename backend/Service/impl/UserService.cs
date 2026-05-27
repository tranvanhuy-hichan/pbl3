using pbl3_web_api.Entities;
using pbl3_web_api.Repository;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}

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

public class UserService : IUserService
{
    private readonly IUserRepo _repo;

    public UserService(IUserRepo repo)
    {
        _repo = repo;
    }

    public async Task<ServiceResult<object>> GetAllAsync(int page, int pageSize)
    {
        var users = await _repo.GetAllActiveAsync(page, pageSize);
        var total = await _repo.CountAsync();

        return new ServiceResult<object>
        {
            Success = true,
            Data = new { data = users, totalCount = total }
        };
    }

    public async Task<ServiceResult<object>> GetDeletedAsync(int page, int pageSize)
    {
        var users = await _repo.GetAllNoActiveAsync(page, pageSize);
        var total = await _repo.CountAsync();

        return new ServiceResult<object>
        {
            Success = true,
            Data = new { data = users, totalCount = total }
        };
    }

    public async Task<ServiceResult<User>> GetByIdAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id);

        if (user == null)
        {
            return new ServiceResult<User>
            {
                Success = false,
                Message = "NOT_FOUND"
            };
        }

        return new ServiceResult<User>
        {
            Success = true,
            Data = user
        };
    }

    public async Task<ServiceResult<object>> CreateAsync(CreateUserDto request)
    {
        var user = new User
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

        await _repo.AddAsync(user);
        await _repo.SaveAsync();

        return new ServiceResult<object>
        {
            Success = true,
            Message = "Tạo tài khoản thành công",
            Data = new { userId = user.UserId }
        };
    }

    public async Task<ServiceResult<User>> UpdateAsync(int id, UpdateUserDto request)
    {
        var user = await _repo.GetByIdAsync(id);

        if (user == null)
        {
            return new ServiceResult<User>
            {
                Success = false,
                Message = "NOT_FOUND"
            };
        }

        if (request.fullName != null) user.FullName = request.fullName;
        if (request.email != null) user.Email = request.email;
        if (request.phoneNumber != null) user.PhoneNumber = request.phoneNumber;
        if (request.address != null) user.Address = request.address;
        if (request.dateOfBirth != null) user.DateOfBirth = request.dateOfBirth;
        if (request.role != null) user.Role = request.role;
        if (!string.IsNullOrEmpty(request.password)) user.Password = request.password;
        if (request.isActive != null) user.IsActive = request.isActive.Value;

        user.UpdatedAt = DateTime.Now;

        await _repo.SaveAsync();

        return new ServiceResult<User>
        {
            Success = true,
            Message = "Cập nhật thành công",
            Data = user
        };
    }

    public async Task<ServiceResult<object>> ToggleActiveAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id);

        if (user == null)
        {
            return new ServiceResult<object>
            {
                Success = false,
                Message = "NOT_FOUND"
            };
        }

        user.IsActive = !user.IsActive;
        user.UpdatedAt = DateTime.Now;

        await _repo.SaveAsync();

        return new ServiceResult<object>
        {
            Success = true,
            Message = "Đã đổi trạng thái",
            Data = new { isActive = user.IsActive }
        };
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id);

        if (user == null)
        {
            return new ServiceResult<object>
            {
                Success = false,
                Message = "NOT_FOUND"
            };
        }

        // soft delete
        user.IsActive = false;

        await _repo.SaveAsync();

        return new ServiceResult<object>
        {
            Success = true,
            Message = "Đã xóa (soft delete)"
        };
    }
}
