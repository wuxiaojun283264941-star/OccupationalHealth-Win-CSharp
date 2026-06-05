using Microsoft.AspNetCore.Mvc;
using OccupationalHealth.Api.Middleware;
using OccupationalHealth.Api.Models;
using OccupationalHealth.Api.Services;

namespace OccupationalHealth.Api.Controllers;

[ApiController]
[Route("api/occupations")]
[RequireAuth]
[RequireRole("admin")]
public class OccupationController : ControllerBase
{
    private readonly OccupationService _service;

    public OccupationController(OccupationService service) { _service = service; }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? keyword)
    {
        var list = _service.GetAll(keyword);
        return ApiHelper.Success(list);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateOccupationRequest req)
    {
        try
        {
            var occ = _service.Create(req);
            return ApiHelper.Success(occ, "创建成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateOccupationRequest req)
    {
        try
        {
            var occ = _service.Update(id, req);
            return ApiHelper.Success(occ, "更新成功");
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

    [HttpGet("{id}/hazards")]
    public IActionResult GetHazards(int id)
    {
        var hazards = _service.GetHazards(id);
        return ApiHelper.Success(hazards);
    }

    [HttpPost("{id}/hazards")]
    public IActionResult BindHazards(int id, [FromBody] BindHazardsRequest req)
    {
        try
        {
            _service.BindHazards(id, req.HazardFactorIds);
            return ApiHelper.Success(message: "绑定成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpDelete("{id}/hazards/{hazardId}")]
    public IActionResult UnbindHazard(int id, int hazardId)
    {
        try
        {
            _service.UnbindHazard(id, hazardId);
            return ApiHelper.Success(message: "解绑成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }
}
