using Microsoft.AspNetCore.Mvc;
using OccupationalHealth.Api.Middleware;
using OccupationalHealth.Api.Models;
using OccupationalHealth.Api.Services;

namespace OccupationalHealth.Api.Controllers;

[ApiController]
[Route("api/employees")]
[RequireAuth]
[RequireRole("factory")]
public class EmployeeController : ControllerBase
{
    private readonly EmployeeService _service;

    public EmployeeController(EmployeeService service) { _service = service; }

    private int FactoryId => (HttpContext.Items["CurrentUser"] as CurrentUser)!.Id;

    [HttpGet]
    public IActionResult GetEmployees([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? keyword = null, [FromQuery] string? position_id = null)
    {
        try
        {
            var (list, total) = _service.GetEmployees(FactoryId, page, pageSize, keyword, position_id);
            return ApiHelper.Paginated(list, total, page, pageSize);
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetEmployee(int id)
    {
        try
        {
            var emp = _service.GetEmployee(id, FactoryId);
            return ApiHelper.Success(emp);
        }
        catch (Exception ex)
        {
            return ApiHelper.NotFound(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateEmployeeRequest req)
    {
        try
        {
            var emp = _service.CreateEmployee(FactoryId, req);
            return ApiHelper.Success(emp, "创建成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateEmployeeRequest req)
    {
        try
        {
            var emp = _service.UpdateEmployee(id, FactoryId, req);
            return ApiHelper.Success(emp, "更新成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            _service.DeleteEmployee(id, FactoryId);
            return ApiHelper.Success(message: "删除成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import()
    {
        try
        {
            var file = Request.Form.Files.FirstOrDefault();
            if (file == null) throw new Exception("请上传文件");
            using var stream = file.OpenReadStream();
            var result = _service.ImportExcel(FactoryId, stream);
            return ApiHelper.Success(result);
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }
}
