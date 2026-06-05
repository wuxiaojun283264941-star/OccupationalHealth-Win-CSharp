using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OccupationalHealth.Api.Middleware;
using OccupationalHealth.Api.Models;
using OccupationalHealth.Api.Services;

namespace OccupationalHealth.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService) { _authService = authService; }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        try
        {
            var result = _authService.Login(req);
            return ApiHelper.Success(result, "登录成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpPost("send-code")]
    public IActionResult SendCode([FromBody] SendCodeRequest req)
    {
        try
        {
            _authService.SendCode(req.Phone);
            return ApiHelper.Success(new { phone = req.Phone }, "验证码已发送");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpGet("me")]
    [RequireAuth]
    public IActionResult Me()
    {
        var user = HttpContext.Items["CurrentUser"] as CurrentUser;
        return ApiHelper.Success(user);
    }
}
