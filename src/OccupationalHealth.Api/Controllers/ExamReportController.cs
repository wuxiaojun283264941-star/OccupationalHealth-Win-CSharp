using Microsoft.AspNetCore.Mvc;
using OccupationalHealth.Api.Middleware;
using OccupationalHealth.Api.Models;
using OccupationalHealth.Api.Services;

namespace OccupationalHealth.Api.Controllers;

[ApiController]
[Route("api/exam-reports")]
[RequireAuth]
public class ExamReportController : ControllerBase
{
    private readonly ExamReportService _service;

    public ExamReportController(ExamReportService service) { _service = service; }

    private CurrentUser Cu => (HttpContext.Items["CurrentUser"] as CurrentUser)!;

    [HttpPost("upload")]
    [RequireRole("health_agent")]
    public async Task<IActionResult> Upload()
    {
        try
        {
            var file = Request.Form.Files.FirstOrDefault();
            if (file == null) throw new Exception("请选择文件");

            var taskIdStr = Request.Form["task_id"].ToString();
            var empIdStr = Request.Form["employee_id"].ToString();
            if (!int.TryParse(taskIdStr, out var taskId) || !int.TryParse(empIdStr, out var empId))
                throw new Exception("参数错误");

            using var stream = file.OpenReadStream();
            var report = _service.UploadReport(taskId, empId, Cu.Id, file.FileName, stream);
            return ApiHelper.Success(report, "上传成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpGet("task/{taskId}")]
    public IActionResult GetTaskReports(int taskId)
    {
        var reports = _service.GetTaskReports(taskId);
        return ApiHelper.Success(reports);
    }

    [HttpGet("{id}")]
    public IActionResult GetReport(int id)
    {
        try
        {
            var report = _service.GetReport(id);
            return ApiHelper.Success(report);
        }
        catch (Exception ex)
        {
            return ApiHelper.NotFound(ex.Message);
        }
    }

    [HttpGet("{id}/download")]
    public IActionResult Download(int id)
    {
        try
        {
            var (stream, fileName, contentType) = _service.DownloadReport(id);
            return File(stream, contentType, fileName);
        }
        catch (Exception ex)
        {
            return ApiHelper.NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [RequireRole("health_agent")]
    public IActionResult Delete(int id)
    {
        try
        {
            _service.DeleteReport(id);
            return ApiHelper.Success(message: "删除成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }
}
