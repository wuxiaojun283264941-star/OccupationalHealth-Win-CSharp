using Microsoft.EntityFrameworkCore;
using OccupationalHealth.Api.Data;
using OccupationalHealth.Api.Models;

namespace OccupationalHealth.Api.Services;

public class OccupationService
{
    private readonly AppDbContext _db;
    public OccupationService(AppDbContext db) { _db = db; }

    public List<OccupationDto> GetAll(string? keyword)
    {
        var query = _db.Occupations.Include(o => o.OccupationHazards).AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(o => o.Name.Contains(keyword) || o.Code.Contains(keyword) || o.Description.Contains(keyword));

        return query.OrderBy(o => o.Code).Select(o => new OccupationDto
        {
            Id = o.Id, Code = o.Code, Name = o.Name, Description = o.Description,
            HazardCount = o.OccupationHazards.Count
        }).ToList();
    }

    public Occupation Create(CreateOccupationRequest req)
    {
        if (string.IsNullOrEmpty(req.Code)) throw new Exception("编码不能为空");
        if (string.IsNullOrEmpty(req.Name)) throw new Exception("名称不能为空");
        if (_db.Occupations.Any(o => o.Code == req.Code)) throw new Exception("编码已存在");

        var occ = new Occupation { Code = req.Code, Name = req.Name, Description = req.Description ?? "" };
        _db.Occupations.Add(occ);
        _db.SaveChanges();
        return occ;
    }

    public Occupation Update(int id, UpdateOccupationRequest req)
    {
        var occ = _db.Occupations.Find(id);
        if (occ == null) throw new Exception("职业不存在");
        if (req.Code != null) occ.Code = req.Code;
        if (req.Name != null) occ.Name = req.Name;
        if (req.Description != null) occ.Description = req.Description;
        _db.SaveChanges();
        return occ;
    }

    public void Delete(int id)
    {
        var occ = _db.Occupations.Find(id);
        if (occ == null) throw new Exception("职业不存在");
        _db.Remove(occ);
        _db.SaveChanges();
    }

    public List<HazardFactor> GetHazards(int occupationId)
    {
        return _db.OccupationHazards.Include(oh => oh.HazardFactor)
            .Where(oh => oh.OccupationId == occupationId)
            .Select(oh => oh.HazardFactor!)
            .ToList();
    }

    public void BindHazards(int occupationId, List<int> hazardIds)
    {
        // Remove all existing bindings
        var existing = _db.OccupationHazards.Where(oh => oh.OccupationId == occupationId);
        _db.OccupationHazards.RemoveRange(existing);

        // Add new
        foreach (var hId in hazardIds)
        {
            _db.OccupationHazards.Add(new OccupationHazard { OccupationId = occupationId, HazardFactorId = hId });
        }
        _db.SaveChanges();
    }

    public void UnbindHazard(int occupationId, int hazardId)
    {
        var oh = _db.OccupationHazards.FirstOrDefault(o => o.OccupationId == occupationId && o.HazardFactorId == hazardId);
        if (oh == null) throw new Exception("绑定不存在");
        _db.OccupationHazards.Remove(oh);
        _db.SaveChanges();
    }
}
