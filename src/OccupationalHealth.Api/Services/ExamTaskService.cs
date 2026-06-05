using Microsoft.EntityFrameworkCore;
using OccupationalHealth.Api.Data;
using OccupationalHealth.Api.Models;

namespace OccupationalHealth.Api.Services;

public class ExamTaskService
{
    private readonly AppDbContext _db;

    public ExamTaskService(AppDbContext db) { _db = db; }

    public ExamTask PushTask(int factoryId, PushExamTaskRequest req)
    {
        if (req.EmployeeIds == null || req.EmployeeIds.Count == 0)
            throw new Exception("请选择至少一名员工");

        // Validate employees belong to factory
        var validCount = _db.Employees.Count(e => req.EmployeeIds.Contains(e.Id) && e.FactoryId == factoryId);
        if (validCount != req.EmployeeIds.Count)
            throw new Exception("部分员工不属于本工厂");

        var task = new ExamTask
        {
            FactoryId = factoryId,
            HealthAgentId = req.HealthAgentId,
            FactoryContactId = req.FactoryContactId,
            ExamType = req.ExamType,
            Status = "pushed",
            PushedAt = DateTime.UtcNow
        };
        _db.ExamTasks.Add(task);
        _db.SaveChanges();

        foreach (var empId in req.EmployeeIds)
        {
            _db.ExamTaskEmployees.Add(new ExamTaskEmployee
            {
                ExamTaskId = task.Id,
                EmployeeId = empId,
                ExamStatus = "pending"
            });
        }
        _db.SaveChanges();

        // Mock notification
        Console.WriteLine($"[NOTIFY] Task #{task.Id} pushed to health_agent #{req.HealthAgentId}");
        return task;
    }

    public (List<ExamTaskDto> list, int total) GetPushedTasks(int factoryId, int page, int pageSize, string? examType)
    {
        var query = _db.ExamTasks
            .Include(t => t.HealthAgent)
            .Where(t => t.FactoryId == factoryId);

        if (!string.IsNullOrEmpty(examType))
            query = query.Where(t => t.ExamType == examType);

        var total = query.Count();
        var list = query.OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(t => new ExamTaskDto
            {
                Id = t.Id, FactoryId = t.FactoryId, HealthAgentId = t.HealthAgentId,
                HealthAgentName = t.HealthAgent != null ? t.HealthAgent.Name : null,
                HealthAgentCenter = t.HealthAgent != null ? t.HealthAgent.OrgName : null,
                ExamType = t.ExamType, Status = t.Status, ScheduledDate = t.ScheduledDate,
                EmployeeCount = t.ExamTaskEmployees.Count, CreatedAt = t.CreatedAt
            }).ToList();

        return (list, total);
    }

    public (List<ExamTaskDto> list, int total) GetPendingTasks(int healthAgentId, int page, int pageSize)
    {
        var query = _db.ExamTasks
            .Include(t => t.Factory)
            .Where(t => t.HealthAgentId == healthAgentId && 
                        (t.Status == "pushed" || t.Status == "accepted" || t.Status == "in_progress"));

        var total = query.Count();
        var list = query.OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(t => new ExamTaskDto
            {
                Id = t.Id, FactoryId = t.FactoryId, FactoryName = t.Factory != null ? t.Factory.Name : null,
                HealthAgentId = t.HealthAgentId, ExamType = t.ExamType, Status = t.Status,
                ScheduledDate = t.ScheduledDate,
                EmployeeCount = t.ExamTaskEmployees.Count, CreatedAt = t.CreatedAt
            }).ToList();

        return (list, total);
    }

    public (List<ExamTaskDto> list, int total) GetHistoryTasks(int healthAgentId, int page, int pageSize)
    {
        var query = _db.ExamTasks
            .Include(t => t.Factory)
            .Where(t => t.HealthAgentId == healthAgentId && t.Status == "completed");

        var total = query.Count();
        var list = query.OrderByDescending(t => t.CompletedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(t => new ExamTaskDto
            {
                Id = t.Id, FactoryId = t.FactoryId, FactoryName = t.Factory?.Name,
                HealthAgentId = t.HealthAgentId, ExamType = t.ExamType, Status = t.Status,
                ScheduledDate = t.ScheduledDate,
                EmployeeCount = t.ExamTaskEmployees.Count, CreatedAt = t.CreatedAt
            }).ToList();

        return (list, total);
    }

    public (List<ExamTaskDto> list, int total) GetCUnitTasks(int page, int pageSize, string? factoryName, string? keyword)
    {
        var query = _db.ExamTasks
            .Include(t => t.Factory)
            .Include(t => t.HealthAgent)
            .Where(t => t.Status == "completed");

        if (!string.IsNullOrEmpty(factoryName))
            query = query.Where(t => t.Factory != null && t.Factory.Name.Contains(factoryName));
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(t => (t.Factory != null && t.Factory.Name.Contains(keyword)) ||
                                     (t.HealthAgent != null && t.HealthAgent.Name.Contains(keyword)));

        var total = query.Count();
        var list = query.OrderByDescending(t => t.CompletedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(t => new ExamTaskDto
            {
                Id = t.Id, FactoryId = t.FactoryId, FactoryName = t.Factory != null ? t.Factory.Name : null,
                HealthAgentId = t.HealthAgentId, HealthAgentName = t.HealthAgent != null ? t.HealthAgent.Name : null,
                ExamType = t.ExamType, Status = t.Status, ScheduledDate = t.ScheduledDate,
                EmployeeCount = t.ExamTaskEmployees.Count, CreatedAt = t.CreatedAt
            }).ToList();

        return (list, total);
    }

    public ExamTaskDetailDto? GetTaskDetail(int taskId, int userId, string userRole)
    {
        var task = _db.ExamTasks
            .Include(t => t.Factory)
            .Include(t => t.HealthAgent)
            .FirstOrDefault(t => t.Id == taskId);
        if (task == null) throw new Exception("任务不存在");

        // Permission check
        if (userRole == "factory" && task.FactoryId != userId)
            throw new UnauthorizedAccessException("无权限查看");
        if (userRole == "health_agent" && task.HealthAgentId != userId)
            throw new UnauthorizedAccessException("无权限查看");

        var employees = _db.ExamTaskEmployees
            .Include(te => te.Employee)
            .Where(te => te.ExamTaskId == taskId)
            .Select(te => new TaskEmployeeInfo
            {
                Id = te.Id, EmployeeId = te.EmployeeId,
                EmployeeName = te.Employee != null ? te.Employee.Name : "",
                IdCard = te.Employee != null ? te.Employee.IdCard : null,
                Phone = te.Employee != null ? te.Employee.Phone : null,
                ExamStatus = te.ExamStatus
            }).ToList();

        return new ExamTaskDetailDto
        {
            Id = task.Id, FactoryId = task.FactoryId, FactoryName = task.Factory?.Name,
            HealthAgentId = task.HealthAgentId,
            HealthAgentName = task.HealthAgent?.Name, HealthAgentCenter = task.HealthAgent?.OrgName,
            ExamType = task.ExamType, Status = task.Status, ScheduledDate = task.ScheduledDate,
            PushedAt = task.PushedAt, AcceptedAt = task.AcceptedAt, CompletedAt = task.CompletedAt,
            Employees = employees
        };
    }

    public void AcceptTask(int taskId, int healthAgentId)
    {
        var task = _db.ExamTasks.FirstOrDefault(t => t.Id == taskId && t.HealthAgentId == healthAgentId);
        if (task == null) throw new Exception("任务不存在");
        if (task.Status != "pushed") throw new Exception("任务状态不正确");
        task.Status = "accepted";
        task.AcceptedAt = DateTime.UtcNow;
        _db.SaveChanges();
    }

    public void ScheduleTask(int taskId, int healthAgentId, DateTime scheduledDate)
    {
        var task = _db.ExamTasks.FirstOrDefault(t => t.Id == taskId && t.HealthAgentId == healthAgentId);
        if (task == null) throw new Exception("任务不存在");
        if (task.Status != "accepted" && task.Status != "in_progress")
            throw new Exception("任务状态不正确");
        task.Status = "in_progress";
        task.ScheduledDate = scheduledDate;
        _db.SaveChanges();
    }

    public void CompleteTask(int taskId, int healthAgentId)
    {
        var task = _db.ExamTasks.FirstOrDefault(t => t.Id == taskId && t.HealthAgentId == healthAgentId);
        if (task == null) throw new Exception("任务不存在");
        if (task.Status != "in_progress") throw new Exception("任务状态不正确");
        task.Status = "completed";
        task.CompletedAt = DateTime.UtcNow;
        _db.SaveChanges();
        Console.WriteLine($"[NOTIFY] Task #{taskId} completed by health_agent #{healthAgentId}");
    }

    public List<HealthAgentInfo> GetBoundHealthAgents(int factoryId)
    {
        return _db.ExamTasks
            .Include(t => t.HealthAgent)
            .Where(t => t.FactoryId == factoryId && t.HealthAgent!.Status == "active")
            .Select(t => t.HealthAgent!)
            .Distinct()
            .Select(u => new HealthAgentInfo { Id = u.Id, Name = u.Name, OrgName = u.OrgName, Phone = u.Phone })
            .ToList();
    }
}
