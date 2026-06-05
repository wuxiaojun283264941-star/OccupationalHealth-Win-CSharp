using Microsoft.AspNetCore.Mvc;
using OccupationalHealth.Api.Middleware;
using OccupationalHealth.Api.Models;
using OccupationalHealth.Api.Services;

namespace OccupationalHealth.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[RequireAuth]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _service;

    public DashboardController(DashboardService service) { _service = service; }

    private CurrentUser Cu => (HttpContext.Items["CurrentUser"] as CurrentUser)!;

    [HttpGet("admin")]
    [RequireRole("admin")]
    public IActionResult AdminDashboard()
    {
        var data = _service.GetAdminDashboard();
        return ApiHelper.Success(data);
    }

    [HttpGet("factory")]
    [RequireRole("factory")]
    public IActionResult FactoryDashboard()
    {
        var data = _service.GetFactoryDashboard(Cu.Id);
        return ApiHelper.Success(data);
    }

    [HttpGet("health-agent")]
    [RequireRole("health_agent")]
    public IActionResult HealthAgentDashboard()
    {
        var data = _service.GetHealthAgentDashboard(Cu.Id);
        return ApiHelper.Success(data);
    }

    [HttpGet("c-unit")]
    [RequireRole("c_unit")]
    public IActionResult CUnitDashboard()
    {
        var data = _service.GetCUnitDashboard();
        return ApiHelper.Success(data);
    }
}
