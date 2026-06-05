using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OccupationalHealth.Api.Data;
using OccupationalHealth.Api.Middleware;
using OccupationalHealth.Api.Models;

namespace OccupationalHealth.Api.Controllers;

[ApiController]
[Route("api/health-agents")]
[RequireAuth]
public class HealthAgentController : ControllerBase
{
    private readonly AppDbContext _db;
    public HealthAgentController(AppDbContext db) { _db = db; }
    private int FactoryId => (HttpContext.Items["CurrentUser"] as CurrentUser)!.Id;

    [HttpGet]
    [RequireRole("factory")]
    public IActionResult GetBound()
    {
        var agents = _db.ExamTasks
            .Include(t => t.HealthAgent)
            .Where(t => t.FactoryId == FactoryId && t.HealthAgent!.Status == "active")
            .Select(t => t.HealthAgent!)
            .Distinct()
            .Select(u => new { u.Id, u.Name, OrgName = u.OrgName, u.Phone })
            .ToList();
        return ApiHelper.Success(agents);
    }

    [HttpGet("all")]
    [RequireRole("factory")]
    public IActionResult GetAll()
    {
        var agents = _db.Users
            .Where(u => u.Role == "health_agent" && u.Status == "active")
            .Select(u => new { u.Id, u.Name, OrgName = u.OrgName, u.Phone })
            .ToList();
        return ApiHelper.Success(agents);
    }
}
