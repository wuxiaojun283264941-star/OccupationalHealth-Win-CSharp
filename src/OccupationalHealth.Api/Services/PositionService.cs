using Microsoft.EntityFrameworkCore;
using OccupationalHealth.Api.Data;
using OccupationalHealth.Api.Models;

namespace OccupationalHealth.Api.Services;

public class PositionService
{
    private readonly AppDbContext _db;

    public PositionService(AppDbContext db) { _db = db; }

    public List<PositionTreeNode> GetTree(int factoryId)
    {
        var all = _db.Positions
            .Include(p => p.Occupation)
            .Where(p => p.FactoryId == factoryId)
            .OrderBy(p => p.OrderIndex)
            .ToList();

        var dict = all.ToDictionary(p => p.Id, p => new PositionTreeNode
        {
            Id = p.Id, FactoryId = p.FactoryId, ParentId = p.ParentId,
            Name = p.Name, Level = p.Level, OccupationId = p.OccupationId,
            OccupationName = p.Occupation?.Name, OccupationCode = p.Occupation?.Code,
            OrderIndex = p.OrderIndex, ChildCount = all.Count(c => c.ParentId == p.Id),
            Children = new List<PositionTreeNode>()
        });

        var roots = new List<PositionTreeNode>();
        foreach (var node in dict.Values)
        {
            if (node.ParentId != null && dict.ContainsKey(node.ParentId.Value))
                dict[node.ParentId.Value].Children.Add(node);
            else
                roots.Add(node);
        }
        return roots;
    }

    public Position CreatePosition(int factoryId, CreatePositionRequest req)
    {
        if (string.IsNullOrEmpty(req.Name)) throw new Exception("名称不能为空");
        if (req.Level != "workshop" && req.Level != "section" && req.Level != "position")
            throw new Exception("无效的层级类型");

        var pos = new Position
        {
            FactoryId = factoryId, ParentId = req.ParentId,
            Name = req.Name, Level = req.Level, OccupationId = req.OccupationId
        };
        _db.Positions.Add(pos);
        _db.SaveChanges();

        // Auto-apply occupation hazards
        if (req.Level == "position" && req.OccupationId != null)
        {
            ApplyOccupationHazards(pos.Id, req.OccupationId.Value);
        }

        // Auto-inherit from parent workshop
        if (req.ParentId != null)
        {
            InheritWorkshopHazards(pos.Id, req.ParentId.Value);
        }

        return pos;
    }

    public Position UpdatePosition(int id, int factoryId, UpdatePositionRequest req)
    {
        var pos = _db.Positions.FirstOrDefault(p => p.Id == id && p.FactoryId == factoryId);
        if (pos == null) throw new Exception("岗位不存在");

        if (req.Name != null) pos.Name = req.Name;
        if (req.ParentId != null) pos.ParentId = req.ParentId;
        if (req.OrderIndex != null) pos.OrderIndex = req.OrderIndex.Value;
        
        if (req.OccupationId != null)
        {
            pos.OccupationId = req.OccupationId;
            _db.SaveChanges();
            // Re-apply occupation hazards
            _db.PositionHazards.RemoveRange(_db.PositionHazards.Where(ph => ph.PositionId == id && ph.Source == "occupation"));
            ApplyOccupationHazards(id, req.OccupationId.Value);
        }
        else
        {
            _db.SaveChanges();
        }
        return pos;
    }

    public void DeletePosition(int id, int factoryId)
    {
        var pos = _db.Positions.FirstOrDefault(p => p.Id == id && p.FactoryId == factoryId);
        if (pos == null) throw new Exception("岗位不存在");

        // Recursive delete: find all descendants
        var allIds = new List<int> { id };
        CollectDescendants(id, allIds);
        var positions = _db.Positions.Where(p => allIds.Contains(p.Id));
        _db.Positions.RemoveRange(positions);
        _db.SaveChanges();
    }

    public List<HazardWithBinding> GetEffectiveHazards(int positionId)
    {
        var position = _db.Positions.Find(positionId);
        if (position == null) throw new Exception("岗位不存在");

        var result = new List<HazardWithBinding>();

        // Direct bindings
        var directs = _db.PositionHazards.Include(ph => ph.HazardFactor)
            .Where(ph => ph.PositionId == positionId && ph.Source == "direct")
            .Select(ph => new HazardWithBinding
            {
                Id = ph.HazardFactor!.Id, Code = ph.HazardFactor.Code,
                Category = ph.HazardFactor.Category, Name = ph.HazardFactor.Name,
                BindingId = ph.Id, Source = "direct"
            }).ToList();
        result.AddRange(directs);

        // Occupation inherited
        if (position.OccupationId != null)
        {
            var occHazards = _db.OccupationHazards.Include(oh => oh.HazardFactor)
                .Where(oh => oh.OccupationId == position.OccupationId)
                .Select(oh => new HazardWithBinding
                {
                    Id = oh.HazardFactor!.Id, Code = oh.HazardFactor.Code,
                    Category = oh.HazardFactor.Category, Name = oh.HazardFactor.Name,
                    BindingId = oh.Id, Source = "occupation"
                }).ToList();
            foreach (var h in occHazards)
                if (!result.Any(r => r.Id == h.Id)) result.Add(h);
        }

        // Workshop inherited
        var workshop = FindWorkshop(position);
        if (workshop != null)
        {
            var wsHazards = _db.PositionHazards.Include(ph => ph.HazardFactor)
                .Where(ph => ph.PositionId == workshop.Id)
                .Select(ph => new HazardWithBinding
                {
                    Id = ph.HazardFactor!.Id, Code = ph.HazardFactor.Code,
                    Category = ph.HazardFactor.Category, Name = ph.HazardFactor.Name,
                    BindingId = ph.Id, Source = "workshop"
                }).ToList();
            foreach (var h in wsHazards)
                if (!result.Any(r => r.Id == h.Id)) result.Add(h);
        }

        return result;
    }

    public List<HazardWithBinding> GetDirectHazards(int positionId)
    {
        return _db.PositionHazards.Include(ph => ph.HazardFactor)
            .Where(ph => ph.PositionId == positionId)
            .Select(ph => new HazardWithBinding
            {
                Id = ph.HazardFactor!.Id, Code = ph.HazardFactor.Code,
                Category = ph.HazardFactor.Category, Name = ph.HazardFactor.Name,
                BindingId = ph.Id, Source = ph.Source
            }).ToList();
    }

    public void BindHazards(int positionId, List<int> hazardFactorIds)
    {
        var pos = _db.Positions.Find(positionId);
        if (pos == null) throw new Exception("岗位不存在");
        if (pos.Level != "position" && pos.Level != "workshop")
            throw new Exception("仅岗位和车间级别可以绑定危害因素");

        // Remove existing direct bindings
        var existing = _db.PositionHazards.Where(ph => ph.PositionId == positionId && ph.Source == "direct");
        _db.PositionHazards.RemoveRange(existing);

        // Add new bindings
        foreach (var hfId in hazardFactorIds)
        {
            _db.PositionHazards.Add(new PositionHazard
            {
                PositionId = positionId,
                HazardFactorId = hfId,
                Source = "direct"
            });
        }
        _db.SaveChanges();

        // If workshop, propagate to children
        if (pos.Level == "workshop")
            PropagateWorkshopHazards(positionId);
    }

    public void UnbindHazard(int positionId, int hazardId)
    {
        var binding = _db.PositionHazards.FirstOrDefault(ph => ph.PositionId == positionId && ph.HazardFactorId == hazardId);
        if (binding == null) throw new Exception("绑定不存在");
        _db.PositionHazards.Remove(binding);
        _db.SaveChanges();

        // If workshop, re-propagate
        var pos = _db.Positions.Find(positionId);
        if (pos?.Level == "workshop")
            PropagateWorkshopHazards(positionId);
    }

    // === Private helpers ===
    private void ApplyOccupationHazards(int positionId, int occupationId)
    {
        var ohs = _db.OccupationHazards.Where(oh => oh.OccupationId == occupationId).ToList();
        foreach (var oh in ohs)
        {
            if (!_db.PositionHazards.Any(ph => ph.PositionId == positionId && ph.HazardFactorId == oh.HazardFactorId))
            {
                _db.PositionHazards.Add(new PositionHazard
                {
                    PositionId = positionId, HazardFactorId = oh.HazardFactorId, Source = "occupation"
                });
            }
        }
        _db.SaveChanges();
    }

    private void InheritWorkshopHazards(int positionId, int parentId)
    {
        var parent = _db.Positions.Find(parentId);
        if (parent?.Level == "workshop")
        {
            var wsHazards = _db.PositionHazards.Where(ph => ph.PositionId == parentId).ToList();
            foreach (var wh in wsHazards)
            {
                if (!_db.PositionHazards.Any(ph => ph.PositionId == positionId && ph.HazardFactorId == wh.HazardFactorId))
                {
                    _db.PositionHazards.Add(new PositionHazard
                    {
                        PositionId = positionId, HazardFactorId = wh.HazardFactorId, Source = "workshop"
                    });
                }
            }
            _db.SaveChanges();
        }
    }

    private void PropagateWorkshopHazards(int workshopId)
    {
        var children = _db.Positions.Where(p => p.ParentId == workshopId || 
            _db.Positions.Any(pp => pp.Id == p.ParentId && pp.ParentId == workshopId)).ToList();
        
        foreach (var child in children)
        {
            // Remove old workshop-inherited
            var oldInherited = _db.PositionHazards.Where(ph => ph.PositionId == child.Id && ph.Source == "workshop");
            _db.PositionHazards.RemoveRange(oldInherited);
        }
        _db.SaveChanges();

        // Apply new
        var wsHazards = _db.PositionHazards.Where(ph => ph.PositionId == workshopId).ToList();
        foreach (var child in children)
        {
            foreach (var wh in wsHazards)
            {
                if (!_db.PositionHazards.Any(ph => ph.PositionId == child.Id && ph.HazardFactorId == wh.HazardFactorId))
                {
                    _db.PositionHazards.Add(new PositionHazard
                    {
                        PositionId = child.Id, HazardFactorId = wh.HazardFactorId, Source = "workshop"
                    });
                }
            }
        }
        _db.SaveChanges();
    }

    private void CollectDescendants(int parentId, List<int> result)
    {
        var children = _db.Positions.Where(p => p.ParentId == parentId).Select(p => p.Id).ToList();
        foreach (var childId in children)
        {
            result.Add(childId);
            CollectDescendants(childId, result);
        }
    }

    private Position? FindWorkshop(Position position)
    {
        var current = position;
        while (current.ParentId != null)
        {
            current = _db.Positions.Find(current.ParentId);
            if (current?.Level == "workshop") return current;
        }
        return null;
    }

    public List<Occupation> GetOccupations()
    {
        return _db.Occupations.OrderBy(o => o.Code).ToList();
    }
}
