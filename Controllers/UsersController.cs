using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pbl3_web_api.Service;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 20)
    {
        var result = await _service.GetAllAsync(page, pageSize);
        return Ok(result.Data);
    }

    [HttpGet("deleted")]
    public async Task<IActionResult> GetDelete(int page = 1, int pageSize = 20)
    {
        var result = await _service.GetDeletedAsync(page, pageSize);
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);

        if (!result.Success)
            return NotFound(new { message = "Không tìm thấy người dùng!" });

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto request)
    {
        var result = await _service.CreateAsync(request);
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }
        return StatusCode(201, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateUserDto request)
    {
        var result = await _service.UpdateAsync(id, request);

        if (!result.Success)
            return NotFound(new { message = "Không tìm thấy người dùng!" });

        return Ok(new
        {
            message = result.Message,
            user = result.Data
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> Toggle(int id)
    {
        var result = await _service.ToggleActiveAsync(id);

        if (!result.Success)
            return NotFound(new { message = "Không tìm thấy người dùng!" });

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result.Success)
            return NotFound(new { message = "Không tìm thấy người dùng!" });

        return Ok(result);
    }
}
