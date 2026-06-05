using Microsoft.EntityFrameworkCore;
using OccupationalHealth.Api.Data;
using OccupationalHealth.Api.Models;

namespace OccupationalHealth.Api.Services;

public class HazardFactorService
{
    private readonly AppDbContext _db;
    public HazardFactorService(AppDbContext db) { _db = db; }

    public List<HazardFactor> GetAll(string? category, string? keyword)
    {
        var query = _db.HazardFactors.AsQueryable();
        if (!string.IsNullOrEmpty(category))
            query = query.Where(h => h.Category == category);
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(h => h.Name.Contains(keyword) || h.Code.Contains(keyword) || h.Description.Contains(keyword));
        return query.OrderBy(h => h.Category).ThenBy(h => h.Code).ToList();
    }

    public List<string> GetCategories()
    {
        return _db.HazardFactors.Select(h => h.Category).Distinct().OrderBy(c => c).ToList();
    }

    public HazardFactor Create(CreateHazardFactorRequest req)
    {
        if (string.IsNullOrEmpty(req.Code)) throw new Exception("编码不能为空");
        if (string.IsNullOrEmpty(req.Name)) throw new Exception("名称不能为空");
        if (_db.HazardFactors.Any(h => h.Code == req.Code)) throw new Exception("编码已存在");

        var h = new HazardFactor
        {
            Code = req.Code, Category = req.Category, Name = req.Name,
            Description = req.Description ?? "", ExamFrequency = req.ExamFrequency ?? ""
        };
        _db.HazardFactors.Add(h);
        _db.SaveChanges();
        return h;
    }

    public HazardFactor Update(int id, UpdateHazardFactorRequest req)
    {
        var h = _db.HazardFactors.Find(id);
        if (h == null) throw new Exception("危害因素不存在");
        if (req.Code != null) h.Code = req.Code;
        if (req.Category != null) h.Category = req.Category;
        if (req.Name != null) h.Name = req.Name;
        if (req.Description != null) h.Description = req.Description;
        if (req.ExamFrequency != null) h.ExamFrequency = req.ExamFrequency;
        _db.SaveChanges();
        return h;
    }

    public void Delete(int id)
    {
        var h = _db.HazardFactors.Find(id);
        if (h == null) throw new Exception("危害因素不存在");
        _db.Remove(h);
        _db.SaveChanges();
    }
}
