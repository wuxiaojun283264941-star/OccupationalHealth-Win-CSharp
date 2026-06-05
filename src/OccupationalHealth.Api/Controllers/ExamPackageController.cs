using Microsoft.AspNetCore.Mvc;
using OccupationalHealth.Api.Middleware;
using OccupationalHealth.Api.Models;
using OccupationalHealth.Api.Services;

namespace OccupationalHealth.Api.Controllers;

[ApiController]
[Route("api/exam-packages")]
[RequireAuth]
[RequireRole("health_agent")]
public class ExamPackageController : ControllerBase
{
    private readonly ExamPackageService _service;
    public ExamPackageController(ExamPackageService service) { _service = service; }
    private int AgentId => (HttpContext.Items["CurrentUser"] as CurrentUser)!.Id;

    [HttpGet]
    public IActionResult GetAll()
    {
        var list = _service.GetAll(AgentId);
        return ApiHelper.Success(list);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateExamPackageRequest req)
    {
        try { var p = _service.Create(AgentId, req); return ApiHelper.Success(p, "创建成功"); }
        catch (Exception ex) { return ApiHelper.Error(ex.Message); }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateExamPackageRequest req)
    {
        try { var p = _service.Update(id, AgentId, req); return ApiHelper.Success(p, "更新成功"); }
        catch (Exception ex) { return ApiHelper.Error(ex.Message); }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try { _service.Delete(id, AgentId); return ApiHelper.Success(message: "删除成功"); }
        catch (Exception ex) { return ApiHelper.Error(ex.Message); }
    }
}
