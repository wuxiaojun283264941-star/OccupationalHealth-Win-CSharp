using Microsoft.EntityFrameworkCore;
using OccupationalHealth.Api.Data;
using OccupationalHealth.Api.Models;

namespace OccupationalHealth.Api.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    public AuthService(AppDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public LoginResponse Login(LoginRequest req)
    {
        User? user = null;

        // Mode 1: phone + SMS code
        if (!string.IsNullOrEmpty(req.Phone) && !string.IsNullOrEmpty(req.Code))
        {
            var smsCode = _db.SmsCodes
                .Where(s => s.Phone == req.Phone && s.ExpireAt > DateTime.UtcNow)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefault();

            if (smsCode == null || smsCode.Code != req.Code)
                throw new Exception("验证码错误或已过期");

            user = _db.Users.FirstOrDefault(u => u.Phone == req.Phone && u.Role == "health_agent");
            if (user == null)
                throw new Exception("手机号未注册");

            if (user.Role != "health_agent")
                throw new Exception("验证码登录仅限体检中心使用");
        }
        // Mode 2: username + password
        else if (!string.IsNullOrEmpty(req.Username) && !string.IsNullOrEmpty(req.Password))
        {
            user = _db.Users.FirstOrDefault(u => u.Username == req.Username);
            if (user == null)
                throw new Exception("用户名或密码错误");

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                throw new Exception("用户名或密码错误");

            if (user.Status != "active")
                throw new Exception("账号已被禁用，请联系管理员");
        }
        else
        {
            throw new Exception("请提供用户名密码或手机验证码");
        }

        var token = _jwt.GenerateToken(user.Id, user.Role, user.Name, user.OrgName);

        return new LoginResponse
        {
            Token = token,
            User = new UserInfo
            {
                Id = user.Id,
                Role = user.Role,
                Name = user.Name,
                Username = user.Username,
                OrgName = user.OrgName,
                Phone = user.Phone,
                Status = user.Status
            }
        };
    }

    public void SendCode(string phone)
    {
        if (string.IsNullOrEmpty(phone))
            throw new Exception("请输入手机号");

        // Mock SMS: always use 123456
        _db.SmsCodes.Add(new SmsCode
        {
            Phone = phone,
            Code = "123456",
            ExpireAt = DateTime.UtcNow.AddMinutes(5)
        });
        _db.SaveChanges();
        Console.WriteLine($"[SMS] Sending code 123456 to {phone}");
    }
}
