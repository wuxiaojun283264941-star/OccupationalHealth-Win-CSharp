using Microsoft.AspNetCore.Mvc;

namespace OccupationalHealth.Api.Models;

/// <summary>
/// 统一响应帮助类 - 简化控制器中的重复代码
/// </summary>
public static class ApiHelper
{
    public static OkObjectResult Success<T>(T? data = default, string message = "success")
        => new(new ApiResponse<T> { Code = 0, Data = data, Message = message });

    public static OkObjectResult Success(object? data = null, string message = "success")
        => new(new ApiResponse<object> { Code = 0, Data = data, Message = message });

    public static OkObjectResult Paginated<T>(List<T> list, int total, int page, int pageSize)
        => new(new ApiResponse<PaginatedData<T>>
        {
            Code = 0,
            Data = new PaginatedData<T> { List = list, Total = total, Page = page, PageSize = pageSize },
            Message = "success"
        });

    public static BadRequestObjectResult Error(string message)
        => new(new ApiResponse<object> { Code = -1, Data = null, Message = message });

    public static NotFoundObjectResult NotFound(string message)
        => new(new ApiResponse<object> { Code = -1, Data = null, Message = message });

    public static UnauthorizedObjectResult Unauthorized(string message = "未登录或登录已过期")
        => new(new ApiResponse<object> { Code = 401, Data = null, Message = message });

    public static ObjectResult Forbidden(string message = "无权限访问")
        => new(new ApiResponse<object> { Code = 403, Data = null, Message = message }) { StatusCode = 403 };
}
