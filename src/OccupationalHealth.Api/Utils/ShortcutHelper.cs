using System.Reflection;
using System.Runtime.InteropServices;

namespace OccupationalHealth.Api.Utils;

/// <summary>
/// 桌面快捷方式创建工具 - 首次运行时调用
/// </summary>
public static class ShortcutHelper
{
    private static readonly string MarkerFile = Path.Combine(AppContext.BaseDirectory, ".shortcut_created");

    /// <summary>检查是否已创建过快捷方式</summary>
    public static bool IsAlreadyCreated => File.Exists(MarkerFile);

    /// <summary>在桌面创建快捷方式（仅首次运行）</summary>
    public static void TryCreate()
    {
        if (IsAlreadyCreated) return;

        try
        {
            Create();
            File.WriteAllText(MarkerFile, DateTime.UtcNow.ToString("o"));
            Console.WriteLine("[快捷方式] 桌面快捷方式已创建");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[快捷方式] 创建失败: {ex.Message}");
        }
    }

    private static void Create()
    {
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var shortcutPath = Path.Combine(desktop, "职业健康体检管理平台.lnk");
        var exePath = Environment.ProcessPath ?? Path.Combine(AppContext.BaseDirectory, "Server.exe");
        var workDir = Path.GetDirectoryName(exePath) ?? AppContext.BaseDirectory;

        // 从嵌入资源提取图标
        var iconPath = ExtractIcon();

        // 使用 WScript.Shell COM 创建 .lnk
        var shellType = Type.GetTypeFromProgID("WScript.Shell");
        if (shellType == null)
        {
            Console.WriteLine("[快捷方式] WScript.Shell COM 不可用，跳过");
            return;
        }

        dynamic shell = Activator.CreateInstance(shellType)!;
        var shortcut = shell.CreateShortcut(shortcutPath);
        shortcut.TargetPath = exePath;
        shortcut.WorkingDirectory = workDir;
        shortcut.Description = "职业健康体检管理平台 v3.0\n基于 .NET 8 + SQLite\n端口: 3001 | 默认账号: admin / admin123";

        if (File.Exists(iconPath))
            shortcut.IconLocation = $"{iconPath},0";

        shortcut.Save();
        Marshal.ReleaseComObject(shortcut);
        Marshal.ReleaseComObject(shell);
    }

    private static string ExtractIcon()
    {
        var iconDir = Path.Combine(AppContext.BaseDirectory, "data");
        Directory.CreateDirectory(iconDir);
        var iconPath = Path.Combine(iconDir, "favicon.ico");

        if (!File.Exists(iconPath))
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("OccupationalHealth.Api.wwwroot.favicon.ico");
            if (stream != null)
            {
                using var fs = new FileStream(iconPath, FileMode.Create, FileAccess.Write);
                stream.CopyTo(fs);
            }
        }

        return iconPath;
    }
}
