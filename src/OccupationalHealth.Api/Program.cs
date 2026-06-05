using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OccupationalHealth.Api.Data;
using OccupationalHealth.Api.Middleware;
using OccupationalHealth.Api.Services;
using OccupationalHealth.Api.Utils;

// ============================================================
// 应用构建
// ============================================================
var builder = WebApplication.CreateBuilder(args);

ServerConfig.Initialize(builder.Configuration);
var port = ServerConfig.Port;

// 数据库
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={ServerConfig.DbPath}"));

// JWT 认证
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "occupational_health_secret_key_2024";
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
builder.Services.AddAuthorization();

// 业务服务
builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<PositionService>();
builder.Services.AddScoped<HazardFactorService>();
builder.Services.AddScoped<OccupationService>();
builder.Services.AddScoped<FactoryContactService>();
builder.Services.AddScoped<ExamTaskService>();
builder.Services.AddScoped<ExamReportService>();
builder.Services.AddScoped<ExamPackageService>();
builder.Services.AddScoped<DashboardService>();

// 控制器 + Swagger + CORS
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// ============================================================
// 自动初始化
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any(u => u.Role == "admin"))
    {
        Console.WriteLine("[初始化] 首次启动，正在导入种子数据...");
        DataSeeder.Seed(db, scope.ServiceProvider);
        Console.WriteLine("[初始化] 种子数据导入完成");
    }
}

// ============================================================
// 中间件管道
// ============================================================
app.UseCors();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

// 桌面快捷方式（仅首次运行）
ShortcutHelper.TryCreate();

// ============================================================
// 启动
// ============================================================
app.Urls.Add($"http://0.0.0.0:{port}");

ServerConfig.PrintBanner(port);

app.Run();
