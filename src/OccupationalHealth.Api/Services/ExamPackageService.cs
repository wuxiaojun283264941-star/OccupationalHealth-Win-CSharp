using OccupationalHealth.Api.Data;
using OccupationalHealth.Api.Models;

namespace OccupationalHealth.Api.Services;

public class ExamPackageService
{
    private readonly AppDbContext _db;
    public ExamPackageService(AppDbContext db) { _db = db; }

    public List<ExamPackage> GetAll(int healthAgentId)
    {
        return _db.ExamPackages.Where(p => p.HealthAgentId == healthAgentId).OrderByDescending(p => p.CreatedAt).ToList();
    }

    public ExamPackage Create(int healthAgentId, CreateExamPackageRequest req)
    {
        if (string.IsNullOrEmpty(req.Name)) throw new Exception("请输入套餐名称");
        var pkg = new ExamPackage
        {
            HealthAgentId = healthAgentId, Name = req.Name, Description = req.Description ?? "",
            Price = req.Price, ExamItems = req.ExamItems ?? "", IsActive = 1
        };
        _db.ExamPackages.Add(pkg);
        _db.SaveChanges();
        return pkg;
    }

    public ExamPackage Update(int id, int healthAgentId, UpdateExamPackageRequest req)
    {
        var pkg = _db.ExamPackages.FirstOrDefault(p => p.Id == id && p.HealthAgentId == healthAgentId);
        if (pkg == null) throw new Exception("套餐不存在");
        if (req.Name != null) pkg.Name = req.Name;
        if (req.Description != null) pkg.Description = req.Description;
        if (req.Price != null) pkg.Price = req.Price.Value;
        if (req.ExamItems != null) pkg.ExamItems = req.ExamItems;
        if (req.IsActive != null) pkg.IsActive = req.IsActive.Value;
        pkg.UpdatedAt = DateTime.UtcNow;
        _db.SaveChanges();
        return pkg;
    }

    public void Delete(int id, int healthAgentId)
    {
        var pkg = _db.ExamPackages.FirstOrDefault(p => p.Id == id && p.HealthAgentId == healthAgentId);
        if (pkg == null) throw new Exception("套餐不存在");
        _db.ExamPackages.Remove(pkg);
        _db.SaveChanges();
    }
}
