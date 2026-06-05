using Microsoft.EntityFrameworkCore;
using OccupationalHealth.Api.Data;
using OccupationalHealth.Api.Models;

namespace OccupationalHealth.Api.Services;

public class UserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db) { _db = db; }

    public (List<User> list, int total) GetUsers(string? role, string? keyword, int page, int pageSize)
    {
        var query = _db.Users.AsQueryable();
        if (!string.IsNullOrEmpty(role))
            query = query.Where(u => u.Role == role);
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(u => u.Name.Contains(keyword) || u.Username.Contains(keyword) || u.Phone.Contains(keyword) || u.OrgName.Contains(keyword));

        var total = query.Count();
        var list = query.OrderByDescending(u => u.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return (list, total);
    }

    public User CreateUser(CreateUserRequest req)
    {
        if (string.IsNullOrEmpty(req.Username)) throw new Exception("用户名不能为空");
        if (string.IsNullOrEmpty(req.Password) || req.Password.Length < 6) throw new Exception("密码至少6位");
        if (_db.Users.Any(u => u.Username == req.Username)) throw new Exception("用户名已存在");

        var user = new User
        {
            Username = req.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = req.Role,
            Name = req.Name,
            OrgName = req.OrgName ?? "",
            Phone = req.Phone ?? "",
            Status = "active"
        };
        _db.Users.Add(user);
        _db.SaveChanges();
        return user;
    }

    public User? GetUser(int id)
    {
        var u = _db.Users.Find(id);
        if (u == null) throw new Exception("用户不存在");
        return u;
    }

    public User UpdateUser(int id, UpdateUserRequest req)
    {
        var u = _db.Users.Find(id);
        if (u == null) throw new Exception("用户不存在");
        if (req.Name != null) u.Name = req.Name;
        if (req.OrgName != null) u.OrgName = req.OrgName;
        if (req.Phone != null) u.Phone = req.Phone;
        u.UpdatedAt = DateTime.UtcNow;
        _db.SaveChanges();
        return u;
    }

    public void ResetPassword(int id, string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 6) throw new Exception("密码至少6位");
        var u = _db.Users.Find(id);
        if (u == null) throw new Exception("用户不存在");
        u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        u.UpdatedAt = DateTime.UtcNow;
        _db.SaveChanges();
    }

    public User ToggleStatus(int id, string status)
    {
        if (status != "active" && status != "disabled") throw new Exception("无效的状态值");
        var u = _db.Users.Find(id);
        if (u == null) throw new Exception("用户不存在");
        u.Status = status;
        u.UpdatedAt = DateTime.UtcNow;
        _db.SaveChanges();
        return u;
    }

    public void DeleteUser(int id)
    {
        var u = _db.Users.Find(id);
        if (u == null) throw new Exception("用户不存在");
        if (u.Role == "admin")
        {
            var adminCount = _db.Users.Count(x => x.Role == "admin");
            if (adminCount <= 1) throw new Exception("不能删除最后一个管理员账号");
        }
        _db.Users.Remove(u);
        _db.SaveChanges();
    }

    public List<User> GetUsersByRole(string role)
    {
        return _db.Users.Where(u => u.Role == role && u.Status == "active").OrderByDescending(u => u.CreatedAt).ToList();
    }

    public int CountByRole(string role)
    {
        return _db.Users.Count(u => u.Role == role && u.Status == "active");
    }
}
