using Microsoft.AspNetCore.Mvc;
using OccupationalHealth.Api.Middleware;
using OccupationalHealth.Api.Models;
using OccupationalHealth.Api.Services;

namespace OccupationalHealth.Api.Controllers;

[ApiController]
[Route("api/exam-tasks")]
[RequireAuth]
public class ExamTaskController : ControllerBase
{
    private readonly ExamTaskService _service;

    public ExamTaskController(ExamTaskService service) { _service = service; }

    private CurrentUser Cu => (HttpContext.Items["CurrentUser"] as CurrentUser)!;

    [HttpPost("push")]
    [RequireRole("factory")]
    public IActionResult Push([FromBody] PushExamTaskRequest req)
    {
        try
        {
            var task = _service.PushTask(Cu.Id, req);
            return ApiHelper.Success(task, "推送成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpGet("pushed")]
    [RequireRole("factory")]
    public IActionResult GetPushed([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? exam_type = null)
    {
        try
        {
            var (list, total) = _service.GetPushedTasks(Cu.Id, page, pageSize, exam_type);
            return ApiHelper.Paginated(list, total, page, pageSize);
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpGet("pending")]
    [RequireRole("health_agent")]
    public IActionResult GetPending([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var (list, total) = _service.GetPendingTasks(Cu.Id, page, pageSize);
            return ApiHelper.Paginated(list, total, page, pageSize);
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpGet("history")]
    [RequireRole("health_agent")]
    public IActionResult GetHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var (list, total) = _service.GetHistoryTasks(Cu.Id, page, pageSize);
            return ApiHelper.Paginated(list, total, page, pageSize);
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpGet("cunit-list")]
    [RequireRole("c_unit")]
    public IActionResult GetCUnitList([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? factory_name = null, [FromQuery] string? keyword = null)
    {
        try
        {
            var (list, total) = _service.GetCUnitTasks(page, pageSize, factory_name, keyword);
            return ApiHelper.Paginated(list, total, page, pageSize);
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetDetail(int id)
    {
        try
        {
            var detail = _service.GetTaskDetail(id, Cu.Id, Cu.Role);
            return ApiHelper.Success(detail);
        }
        catch (UnauthorizedAccessException)
        {
            return ApiHelper.Forbidden();
        }
        catch (Exception ex)
        {
            return ApiHelper.NotFound(ex.Message);
        }
    }

    [HttpPost("{id}/accept")]
    [RequireRole("health_agent")]
    public IActionResult Accept(int id)
    {
        try
        {
            _service.AcceptTask(id, Cu.Id);
            return ApiHelper.Success(message: "已接受");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpPost("{id}/schedule")]
    [RequireRole("health_agent")]
    public IActionResult Schedule(int id, [FromBody] ScheduleExamRequest req)
    {
        try
        {
            _service.ScheduleTask(id, Cu.Id, req.ScheduledDate);
            return ApiHelper.Success(message: "已安排");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpPost("{id}/complete")]
    [RequireRole("health_agent")]
    public IActionResult Complete(int id)
    {
        try
        {
            _service.CompleteTask(id, Cu.Id);
            return ApiHelper.Success(message: "已完成");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }
}
