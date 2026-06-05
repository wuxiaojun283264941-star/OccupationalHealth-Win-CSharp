namespace OccupationalHealth.Api.Utils;

/// <summary>
/// 服务器运行时配置 - 从环境变量/配置文件中读取
/// </summary>
public static class ServerConfig
{
    /// <summary>获取服务器端口（支持环境变量 PORT 覆盖 appsettings.json）</summary>
    public static int Port { get; private set; } = 3001;

    /// <summary>数据库文件路径</summary>
    public static string DbPath { get; private set; } = "";

    /// <summary>上传目录路径</summary>
    public static string UploadDir { get; private set; } = "";

    /// <summary>应用根目录</summary>
    public static string BaseDir => AppContext.BaseDirectory;

    /// <summary>初始化所有运行时配置</summary>
    public static void Initialize(IConfiguration config)
    {
        // 端口：环境变量优先
        var envPort = Environment.GetEnvironmentVariable("PORT");
        Port = !string.IsNullOrEmpty(envPort) && int.TryParse(envPort, out var p) ? p
             : config.GetValue<int?>("PORT") ?? 3001;

        // 数据库路径
        var dbDir = Path.Combine(BaseDir, "data");
        Directory.CreateDirectory(dbDir);
        DbPath = Path.Combine(dbDir, "occupational_health.db");

        // 上传目录
        var uploadSetting = config["UploadSettings:UploadDir"] ?? "uploads/reports";
        UploadDir = Path.Combine(BaseDir, uploadSetting);
        Directory.CreateDirectory(UploadDir);
    }

    /// <summary>打印启动信息</summary>
    public static void PrintBanner(int port)
    {
        Console.WriteLine(@"
  ╔══════════════════════════════════════════╗
  ║    职业健康体检管理平台  Server v3.0     ║
  ║    基于 ASP.NET Core 8 + SQLite          ║
  ╚══════════════════════════════════════════╝");
        Console.WriteLine($"  运行地址: http://0.0.0.0:{port}");
        Console.WriteLine($"  数据库:   {DbPath}");
        Console.WriteLine($"  上传目录: {UploadDir}");
        Console.WriteLine($"  预置账号: admin / admin123");
        Console.WriteLine($"  按 Ctrl+C 停止服务器");
        Console.WriteLine("");
    }
}
