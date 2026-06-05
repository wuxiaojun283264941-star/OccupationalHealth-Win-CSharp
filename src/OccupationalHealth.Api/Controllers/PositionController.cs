using Microsoft.AspNetCore.Mvc;
using OccupationalHealth.Api.Middleware;
using OccupationalHealth.Api.Models;
using OccupationalHealth.Api.Services;

namespace OccupationalHealth.Api.Controllers;

[ApiController]
[Route("api/positions")]
[RequireAuth]
[RequireRole("factory")]
public class PositionController : ControllerBase
{
    private readonly PositionService _service;

    public PositionController(PositionService service) { _service = service; }

    private int FactoryId => (HttpContext.Items["CurrentUser"] as CurrentUser)!.Id;

    [HttpGet]
    public IActionResult GetTree()
    {
        var tree = _service.GetTree(FactoryId);
        return ApiHelper.Success(tree);
    }

    [HttpGet("occupations")]
    public IActionResult GetOccupations()
    {
        var list = _service.GetOccupations();
        return ApiHelper.Success(list);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreatePositionRequest req)
    {
        try
        {
            var pos = _service.CreatePosition(FactoryId, req);
            return ApiHelper.Success(pos, "创建成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdatePositionRequest req)
    {
        try
        {
            var pos = _service.UpdatePosition(id, FactoryId, req);
            return ApiHelper.Success(pos, "更新成功");
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
            _service.DeletePosition(id, FactoryId);
            return ApiHelper.Success(message: "删除成功");
        }
        catch (Exception ex)
        {
            return ApiHelper.Error(ex.Message);
        }
    }

    [HttpGet("{id}/hazards")]
    public IActionResult GetDirectHazards(int id)
    {
        var hazards = _service.GetDirectHazards(id);
        return ApiHelper.Success(hazards);
    }

    [HttpGet("{id}/effective-hazards")]
    public IActionResult GetEffectiveHazards(int id)
    {
        var hazards = _service.GetEffectiveHazards(id);
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
