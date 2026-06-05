using System.Data;
using Microsoft.EntityFrameworkCore;
using OccupationalHealth.Api.Data;
using OccupationalHealth.Api.Models;

namespace OccupationalHealth.Api.Services;

public class EmployeeService
{
    private readonly AppDbContext _db;

    public EmployeeService(AppDbContext db) { _db = db; }

    public (List<EmployeeDto> list, int total) GetEmployees(int factoryId, int page, int pageSize, string? keyword, string? positionId)
    {
        var query = _db.Employees
            .Include(e => e.Position)
            .Where(e => e.FactoryId == factoryId);

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(e => e.Name.Contains(keyword) || e.IdCard.Contains(keyword) || e.Phone.Contains(keyword));
        if (!string.IsNullOrEmpty(positionId) && int.TryParse(positionId, out var posId) && posId > 0)
            query = query.Where(e => e.PositionId == posId);

        var total = query.Count();
        var list = query.OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FactoryId = e.FactoryId,
                Name = e.Name,
                Gender = e.Gender,
                BirthDate = e.BirthDate,
                IdCard = e.IdCard,
                Phone = e.Phone,
                PositionId = e.PositionId,
                PositionName = e.Position != null ? e.Position.Name : null,
                PositionLevel = e.Position != null ? e.Position.Level : null,
                PositionParentName = e.Position != null && e.Position.Parent != null ? e.Position.Parent.Name : null,
                EntryDate = e.EntryDate,
                HazardStartDate = e.HazardStartDate,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            })
            .ToList();

        return (list, total);
    }

    public EmployeeDto? GetEmployee(int id, int factoryId)
    {
        var e = _db.Employees.Include(x => x.Position).ThenInclude(p => p!.Parent)
            .FirstOrDefault(x => x.Id == id && x.FactoryId == factoryId);
        if (e == null) throw new Exception("员工不存在");

        return new EmployeeDto
        {
            Id = e.Id, FactoryId = e.FactoryId, Name = e.Name, Gender = e.Gender,
            BirthDate = e.BirthDate, IdCard = e.IdCard, Phone = e.Phone,
            PositionId = e.PositionId, PositionName = e.Position?.Name,
            PositionLevel = e.Position?.Level,
            PositionParentName = e.Position?.Parent?.Name,
            EntryDate = e.EntryDate, HazardStartDate = e.HazardStartDate,
            CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt
        };
    }

    public Employee CreateEmployee(int factoryId, CreateEmployeeRequest req)
    {
        if (string.IsNullOrEmpty(req.Name) || string.IsNullOrEmpty(req.IdCard))
            throw new Exception("姓名和身份证号不能为空");

        var employee = new Employee
        {
            FactoryId = factoryId,
            Name = req.Name,
            Gender = req.Gender ?? "",
            BirthDate = req.BirthDate,
            IdCard = req.IdCard,
            Phone = req.Phone ?? "",
            PositionId = req.PositionId,
            EntryDate = req.EntryDate,
            HazardStartDate = req.HazardStartDate
        };
        _db.Employees.Add(employee);
        _db.SaveChanges();
        return employee;
    }

    public Employee UpdateEmployee(int id, int factoryId, UpdateEmployeeRequest req)
    {
        var e = _db.Employees.FirstOrDefault(x => x.Id == id && x.FactoryId == factoryId);
        if (e == null) throw new Exception("员工不存在");

        if (req.Name != null) e.Name = req.Name;
        if (req.Gender != null) e.Gender = req.Gender;
        if (req.BirthDate != null) e.BirthDate = req.BirthDate;
        if (req.IdCard != null) e.IdCard = req.IdCard;
        if (req.Phone != null) e.Phone = req.Phone;
        if (req.PositionId != null) e.PositionId = req.PositionId;
        if (req.EntryDate != null) e.EntryDate = req.EntryDate;
        if (req.HazardStartDate != null) e.HazardStartDate = req.HazardStartDate;
        e.UpdatedAt = DateTime.UtcNow;
        _db.SaveChanges();
        return e;
    }

    public void DeleteEmployee(int id, int factoryId)
    {
        var e = _db.Employees.FirstOrDefault(x => x.Id == id && x.FactoryId == factoryId);
        if (e == null) throw new Exception("员工不存在");
        _db.Employees.Remove(e);
        _db.SaveChanges();
    }

    public ImportResult ImportExcel(int factoryId, Stream fileStream)
    {
        var result = new ImportResult();
        using var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(fileStream);
        var ds = reader.AsDataSet();
        var table = ds.Tables[0];
        if (table.Rows.Count < 2) throw new Exception("文件中没有数据");

        for (int i = 1; i < table.Rows.Count; i++)
        {
            var row = table.Rows[i];
            result.Total++;
            try
            {
                var name = row[0]?.ToString() ?? "";
                var gender = row[1]?.ToString() ?? "";
                var idCard = row[3]?.ToString() ?? "";
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(idCard))
                {
                    result.Errors.Add($"第{i + 1}行: 姓名和身份证号不能为空");
                    continue;
                }
                CreateEmployee(factoryId, new CreateEmployeeRequest
                {
                    Name = name, Gender = gender, IdCard = idCard,
                    Phone = row[4]?.ToString(), BirthDate = null, PositionId = null,
                    EntryDate = null, HazardStartDate = null
                });
                result.Imported++;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"第{i + 1}行: {ex.Message}");
            }
        }

        return result;
    }
}
