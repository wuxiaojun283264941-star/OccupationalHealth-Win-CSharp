using Microsoft.AspNetCore.Mvc;
using OccupationalHealth.Api.Middleware;
using OccupationalHealth.Api.Models;
using OccupationalHealth.Api.Services;

namespace OccupationalHealth.Api.Controllers;

[ApiController]
[Route("api/admin")]
[RequireAuth]
[RequireRole("admin")]
public class AdminController : ControllerBase
{
    private readonly UserService _userService;

    public AdminController(UserService userService) { _userService = userService; }

    [HttpGet("users")]
    public IActionResult GetUsers([FromQuery] string? role, [FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var (list, total) = _userService.GetUsers(role, keyword, page, pageSize);
            return ApiHelper.Paginated(list, total, page, pageSize);
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpPost("users")]
    public IActionResult CreateUser([FromBody] CreateUserRequest req)
    {
        try
        {
            var user = _userService.CreateUser(req);
            return ApiHelper.Success(user, "创建成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpGet("users/{id}")]
    public IActionResult GetUser(int id)
    {
        try
        {
            var user = _userService.GetUser(id);
            return ApiHelper.Success(new { user!.Id, user.Username, user.Role, user.Name, OrgName = user.OrgName, user.Phone, user.Status, user.CreatedAt, user.UpdatedAt });
        }
        catch (Exception ex)
        {
            return ApiHelper.NotFound(ex.Message);
        }
    }

    [HttpPut("users/{id}")]
    public IActionResult UpdateUser(int id, [FromBody] UpdateUserRequest req)
    {
        try
        {
            var user = _userService.UpdateUser(id, req);
            return ApiHelper.Success(user, "更新成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpPut("users/{id}/password")]
    public IActionResult ResetPassword(int id, [FromBody] ResetPasswordRequest req)
    {
        try
        {
            _userService.ResetPassword(id, req.Password);
            return ApiHelper.Success(message: "密码重置成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpPut("users/{id}/status")]
    public IActionResult ToggleStatus(int id, [FromBody] UpdateStatusRequest req)
    {
        try
        {
            var user = _userService.ToggleStatus(id, req.Status);
            return ApiHelper.Success(user, req.Status == "active" ? "已启用" : "已禁用");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpDelete("users/{id}")]
    public IActionResult DeleteUser(int id)
    {
        try
        {
            _userService.DeleteUser(id);
            return ApiHelper.Success(message: "删除成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }
}
