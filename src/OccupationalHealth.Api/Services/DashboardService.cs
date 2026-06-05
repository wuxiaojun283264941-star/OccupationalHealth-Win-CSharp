using Microsoft.EntityFrameworkCore;
using OccupationalHealth.Api.Data;
using OccupationalHealth.Api.Models;

namespace OccupationalHealth.Api.Services;

public class DashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db) { _db = db; }

    public AdminDashboard GetAdminDashboard()
    {
        return new AdminDashboard
        {
            FactoryCount = _db.Users.Count(u => u.Role == "factory" && u.Status == "active"),
            AgentCount = _db.Users.Count(u => u.Role == "health_agent" && u.Status == "active"),
            CUnitCount = _db.Users.Count(u => u.Role == "c_unit" && u.Status == "active"),
            EmployeeCount = _db.Employees.Count(),
            TaskCount = _db.ExamTasks.Count(),
            CompletedCount = _db.ExamTasks.Count(t => t.Status == "completed"),
            ReportCount = _db.ExamReports.Count()
        };
    }

    public FactoryDashboard GetFactoryDashboard(int factoryId)
    {
        var recentTasks = _db.ExamTasks
            .Include(t => t.HealthAgent)
            .Where(t => t.FactoryId == factoryId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .Select(t => new RecentTask
            {
                Id = t.Id, ExamType = t.ExamType, Status = t.Status,
                AgentName = t.HealthAgent != null ? t.HealthAgent.Name : "",
                AgentCenter = t.HealthAgent != null ? t.HealthAgent.OrgName : "",
                CreatedAt = t.CreatedAt
            }).ToList();

        return new FactoryDashboard
        {
            EmployeeCount = _db.Employees.Count(e => e.FactoryId == factoryId),
            TaskCount = _db.ExamTasks.Count(t => t.FactoryId == factoryId),
            PushedCount = _db.ExamTasks.Count(t => t.FactoryId == factoryId && t.Status == "pushed"),
            InProgressCount = _db.ExamTasks.Count(t => t.FactoryId == factoryId && (t.Status == "accepted" || t.Status == "in_progress")),
            CompletedCount = _db.ExamTasks.Count(t => t.FactoryId == factoryId && t.Status == "completed"),
            RecentTasks = recentTasks
        };
    }

    public HealthAgentDashboard GetHealthAgentDashboard(int agentId)
    {
        var now = DateTime.UtcNow;
        return new HealthAgentDashboard
        {
            PendingCount = _db.ExamTasks.Count(t => t.HealthAgentId == agentId && 
                (t.Status == "pushed" || t.Status == "accepted" || t.Status == "in_progress")),
            CompletedCount = _db.ExamTasks.Count(t => t.HealthAgentId == agentId && t.Status == "completed"),
            ReportCount = _db.ExamReports.Count(r => r.UploadedBy == agentId),
            MonthCompleted = _db.ExamTasks.Count(t => t.HealthAgentId == agentId && 
                t.Status == "completed" && t.CompletedAt != null && 
                t.CompletedAt.Value.Year == now.Year && t.CompletedAt.Value.Month == now.Month)
        };
    }

    public CUnitDashboard GetCUnitDashboard()
    {
        var byFactory = _db.ExamTasks
            .Include(t => t.Factory)
            .Where(t => t.Status == "completed")
            .GroupBy(t => t.Factory!.Name)
            .Select(g => new FactorySummary { FactoryName = g.Key, TaskCount = g.Count() })
            .OrderByDescending(s => s.TaskCount)
            .ToList();

        return new CUnitDashboard
        {
            CompletedCount = _db.ExamTasks.Count(t => t.Status == "completed"),
            ReportCount = _db.ExamReports.Count(),
            ByFactory = byFactory
        };
    }
}
