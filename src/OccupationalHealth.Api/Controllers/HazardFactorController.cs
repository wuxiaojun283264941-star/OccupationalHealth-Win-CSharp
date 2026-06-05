using Microsoft.AspNetCore.Mvc;
using OccupationalHealth.Api.Middleware;
using OccupationalHealth.Api.Models;
using OccupationalHealth.Api.Services;

namespace OccupationalHealth.Api.Controllers;

[ApiController]
[Route("api/hazard-factors")]
[RequireAuth]
[RequireRole("admin")]
public class HazardFactorController : ControllerBase
{
    private readonly HazardFactorService _service;

    public HazardFactorController(HazardFactorService service) { _service = service; }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? category, [FromQuery] string? keyword)
    {
        var list = _service.GetAll(category, keyword);
        return ApiHelper.Success(list);
    }

    [HttpGet("categories")]
    public IActionResult GetCategories()
    {
        var cats = _service.GetCategories();
        return ApiHelper.Success(cats);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateHazardFactorRequest req)
    {
        try
        {
            var h = _service.Create(req);
            return ApiHelper.Success(h, "创建成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateHazardFactorRequest req)
    {
        try
        {
            var h = _service.Update(id, req);
            return ApiHelper.Success(h, "更新成功");
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
            _service.Delete(id);
            return ApiHelper.Success(message: "删除成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }
}
