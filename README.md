# 职业健康体检管理平台 — OccupationalHealth-Win-CSharp

> **基于 ASP.NET Core 8 的 C# Windows 桌面版**  
> 原项目迁移自 Node.js + Fastify + React 全栈版本  
> 一键部署，自带数据库，无需环境配置

---

## 目录

- [系统概述](#系统概述)
- [技术栈](#技术栈)
- [功能模块](#功能模块)
- [角色权限](#角色权限)
- [快速开始](#快速开始)
- [部署说明](#部署说明)
- [API 文档](#api-文档)
- [数据库说明](#数据库说明)
- [开发指南](#开发指南)
- [常见问题](#常见问题)
- [更新日志](#更新日志)

---

## 系统概述

职业健康体检管理平台是一套面向**工厂用人单位、职业健康检查机构（体检中心）和职业卫生托管机构（C单位）** 的三方协同管理系统。系统覆盖从员工信息管理、岗位危害因素识别、体检任务推送、体检报告上传到数据统计归档的全业务流程。

### 核心流程图

```
工厂录入员工 → 关联岗位(含危害因素) → 推送体检任务 → 体检中心接收
                                                              ↓
工厂查看报告 ← 体检中心上传报告PDF ← 安排体检 ← 接受任务
                                                              ↓
                                                    C单位/托管机构查看统计
```

### 主要业务场景

| 场景 | 说明 |
|------|------|
| **员工档案管理** | 工厂录入员工信息（姓名、身份证、岗位关联），支持 Excel 批量导入 |
| **岗位树管理** | 支持三级岗位树（车间 → 工段 → 岗位），自动关联国家职业分类 |
| **危害因素管理** | 内置 GBZ188 标准 46 项职业病危害因素（粉尘/化学/物理/生物/放射性） |
| **危害因素继承** | 三源合并：直接绑定 > 职业继承 > 车间继承，自动计算岗位实际危害因素 |
| **体检任务推送** | 工厂选择员工推送到体检中心，全程状态跟踪 |
| **体检报告管理** | 体检中心上传/下载 PDF 报告，支持在线预览 |
| **体检套餐管理** | 体检中心自定义套餐模板（项目、价格） |
| **多角色仪表盘** | 超管、工厂、体检中心、C单位各自独立的统计面板 |
| **数据批量处理** | 员工 Excel 批量导入、任务批量关联 |

---

## 技术栈

### 后端

| 技术 | 版本 | 用途 |
|------|------|------|
| ASP.NET Core | 8.0 | Web API 框架 |
| Entity Framework Core | 8.0 | ORM 数据访问层 |
| SQLite | 自包含 | 嵌入式数据库 |
| JWT Bearer | .NET 内置 | 身份认证（24h 过期） |
| BCrypt.Net-Next | 4.0 | 密码加密 |
| Swashbuckle | 6.6 | Swagger API 文档 |

### 前端

| 技术 | 用途 |
|------|------|
| 纯 HTML / CSS / JS | 单文件 SPA，无框架依赖 |
| 自适应布局 | 支持桌面和平板端访问 |
| 角色化界面 | 登录后自动识别角色展示对应控制台 |

### 部署

| 特性 | 说明 |
|------|------|
| 发布方式 | `dotnet publish` 自包含单文件 EXE |
| 运行时 | 内置 .NET 8 运行时（无需预装） |
| 目标平台 | Windows x64 （兼容 Windows 10 / 11） |
| 启动方式 | 双击 Server.exe 即运行 |
| 自动初始化 | 首次启动自动创建数据库 + 种子数据 |
| 桌面快捷方式 | 首次运行自动创建桌面快捷方式 |

---

## 功能模块

### 1. 系统管理（超管 admin）

- **用户管理** — 创建/编辑/删除各角色账号，启用/禁用，重置密码
- **危害因素管理** — 按 GBZ188 标准维护职业病危害因素字典
- **职业字典管理** — 管理职业分类，预设职业病危害因素关联

### 2. 工厂端（factory）

- **仪表盘** — 员工数、任务状态统计、最近任务列表
- **岗位管理** — 车间→工段→岗位三级树，关联职业和危害因素
- **员工管理** — 员工 CRUD，Excel 批量导入
- **联系人管理** — 工厂对接人信息维护
- **体检任务** — 选择员工推送到指定体检中心
- **体检中心** — 查看已合作和全部体检机构

### 3. 体检中心（health_agent）

- **仪表盘** — 待处理/已完成任务统计
- **待处理任务** — 接受任务、安排体检日期、完成任务
- **历史任务** — 查看已完成的体检任务
- **体检套餐** — 自定义套餐模板和定价

### 4. 托管机构/C单位（c_unit）

- **仪表盘** — 各工厂任务完成统计
- **已完成任务** — 按工厂/关键词筛选查看
- **员工报告** — 查询指定员工的历史体检报告

---

## 角色权限

| 菜单功能 | admin | factory | health_agent | c_unit |
|---------|:-----:|:-------:|:------------:|:------:|
| 系统概况 | ✓ | ✓ | ✓ | ✓ |
| 用户管理 | ✓ | — | — | — |
| 危害因素管理 | ✓ | — | — | — |
| 职业字典管理 | ✓ | — | — | — |
| 岗位管理 | — | ✓ | — | — |
| 员工管理 | — | ✓ | — | — |
| 联系人管理 | — | ✓ | — | — |
| 体检任务 (推送) | — | ✓ | — | — |
| 体检任务 (接收/处理) | — | — | ✓ | — |
| 体检报告上传 | — | — | ✓ | — |
| 体检套餐管理 | — | — | ✓ | — |
| 历史任务查看 | — | — | ✓ | ✓ |
| 员工报告查询 | — | — | — | ✓ |

---

## 快速开始

### 环境要求

- **Windows 10 / 11**（64位）
- 无需安装 .NET SDK、无需 Node.js、无需数据库
- 端口 `3001` 未被占用

### 构建方法

#### 方式一：一键构建（推荐）

```bash
# 1. 双击 build.bat（需安装 .NET 8 SDK）
#    自动完成：生成图标 → 恢复包 → 编译 → 发布为独立 EXE

# 2. 进入 publish\ 目录
cd publish

# 3. 双击 Server.exe
#    浏览器访问: http://localhost:3001
```

#### 方式二：手动构建

```bash
# 需要 .NET 8 SDK
dotnet restore
dotnet build -c Release
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

### 预置测试账号

| 用户名 | 密码 | 角色 | 说明 |
|--------|:----:|:----:|------|
| `admin` | `admin123` | 系统管理员 | 用户/危害因素/职业字典管理 |
| `factory1` | `123456` | 工厂A | 测试工厂 - 含预制岗位和员工 |
| `factory2` | `123456` | 工厂B | 测试工厂 - 空白数据 |
| `agent1` | `123456` | 体检中心 | 市中心体检中心 |
| `agent2` | `123456` | 体检中心 | 市第二体检中心 |
| `cunit1` | `123456` | 托管机构 | 市卫生托管中心 |

> 首次启动自动创建以上预置数据，包含 46 项 GBZ188 危害因素、8 个职业分类、3 个岗位、3 名员工。

---

## 部署说明

### 服务器部署

将 `publish\` 目录复制到目标服务器，运行 `Server.exe` 即可。

```
publish\                    ← 复制整个目录到服务器
├── Server.exe              ← 主程序（双击运行）
├── *.dll                   ← 运行时依赖
├── *.json                  ← 配置文件
└── ...
    首次运行后自动创建：
    data\
    ├── occupational_health.db  ← SQLite 数据库
    └── favicon.ico             ← 桌面图标缓存
    uploads\reports\            ← 上传的体检报告 PDF
```

### 自定义配置

编辑 `appsettings.json`：

```json
{
  "Jwt": {
    "Secret": "更换为你自己的密钥",       // JWT 签名密钥
    "ExpiresInHours": 24                // Token 过期时间
  },
  "ConnectionStrings": {
    "Default": "Data Source=data/occupational_health.db"  // 数据库路径
  },
  "UploadSettings": {
    "MaxFileSize": 20971520,            // 文件上传限制（20MB）
    "UploadDir": "uploads/reports"      // 报告存储目录
  }
}
```

### 自定义端口

```bash
# 方式一：环境变量
set PORT=8080
Server.exe

# 方式二：修改 launchSettings.json 中的 applicationUrl
```

---

## API 文档

运行后访问 Swagger UI：`http://localhost:3001/swagger`

### API 端点总览

| 模块 | 路由前缀 | 认证 | 角色 |
|------|---------|:----:|:----:|
| 认证 | `POST /api/auth/login` | 无 | 公开 |
| 认证 | `POST /api/auth/send-code` | 无 | 公开 |
| 认证 | `GET /api/auth/me` | JWT | 全部 |
| 用户管理 | `GET/POST /api/admin/users` | JWT | admin |
| 用户管理 | `PUT/DELETE /api/admin/users/:id` | JWT | admin |
| 危害因素 | `GET/POST /api/hazard-factors` | JWT | admin |
| 职业字典 | `GET/POST /api/occupations` | JWT | admin |
| 职业-危害绑定 | `POST/DELETE /api/occupations/:id/hazards` | JWT | admin |
| 岗位树 | `GET /api/positions` | JWT | factory |
| 岗位管理 | `POST/PUT/DELETE /api/positions/:id` | JWT | factory |
| 岗位危害因素 | `GET/POST/DELETE /api/positions/:id/hazards` | JWT | factory |
| 员工管理 | `GET/POST /api/employees` | JWT | factory |
| 员工导入 | `POST /api/employees/import` | JWT | factory |
| 工厂联系人 | `GET/POST /api/factory-contacts` | JWT | factory |
| 体检中心列表 | `GET /api/health-agents` | JWT | factory |
| 推送任务 | `POST /api/exam-tasks/push` | JWT | factory |
| 任务列表 | `GET /api/exam-tasks/pushed` | JWT | factory |
| 待处理任务 | `GET /api/exam-tasks/pending` | JWT | health_agent |
| 接受/完成 | `POST /api/exam-tasks/:id/accept\|complete` | JWT | health_agent |
| 报告上传 | `POST /api/exam-reports/upload` | JWT | health_agent |
| 报告下载 | `GET /api/exam-reports/:id/download` | JWT | 全部 |
| 套餐管理 | `GET/POST /api/exam-packages` | JWT | health_agent |
| 仪表盘 | `GET /api/dashboard/{role}` | JWT | 按角色 |
| C单位查询 | `GET /api/cunit/tasks` | JWT | c_unit |

### 统一响应格式

```json
// 成功
{ "code": 0, "data": { ... }, "message": "success" }

// 业务错误
{ "code": -1, "data": null, "message": "错误说明" }

// 需要登录
{ "code": 401, "data": null, "message": "未登录或登录已过期" }

// 无权限
{ "code": 403, "data": null, "message": "无权限访问" }
```

### 分页响应

```json
{
  "code": 0,
  "data": {
    "list": [ ... ],
    "total": 100,
    "page": 1,
    "pageSize": 20
  },
  "message": "success"
}
```

---

## 数据库说明

### 数据库文件

- 位置：`data/occupational_health.db`
- 类型：SQLite（无需额外安装数据库服务）
- 引擎：WAL 模式，外键约束开启

### 表结构

| 表名 | 说明 | 关键字段 |
|------|------|---------|
| `users` | 统一用户表（4角色） | username, password_hash, role (admin/factory/health_agent/c_unit), status |
| `factory_contacts` | 工厂联系人 | factory_id, name, phone |
| `positions` | 岗位树（自引用） | factory_id, parent_id, name, level (workshop/section/position), occupation_id |
| `hazard_factors` | 危害因素字典 | code, category, name, exam_frequency |
| `occupations` | 职业字典 | code, name |
| `occupation_hazards` | 职业-危害关联 | occupation_id, hazard_factor_id |
| `position_hazards` | 岗位-危害绑定（含继承来源） | position_id, hazard_factor_id, source (direct/occupation/workshop) |
| `employees` | 员工信息 | factory_id, name, id_card, position_id, entry_date, hazard_start_date |
| `exam_tasks` | 体检任务 | factory_id, health_agent_id, exam_type, status |
| `exam_task_employees` | 任务-员工关联 | exam_task_id, employee_id, exam_status |
| `exam_reports` | 体检报告 | exam_task_id, employee_id, file_path |
| `exam_packages` | 体检套餐 | health_agent_id, name, price, exam_items |
| `sms_codes` | 短信验证码（临时） | phone, code, expire_at |

### 危害因素三源合并

岗位的有效危害因素来自三个来源，按优先级去重合并：

```
① direct    ← 直接在岗位上绑定的危害
② occupation ← 从关联职业（工种）继承的危害
③ workshop  ← 从所属车间继承的危害

结果 = ① ∪ ② ∪ ③（ID 去重）
```

---

## 开发指南

### 本地开发

```bash
# 1. 安装 .NET 8 SDK
#    https://dotnet.microsoft.com/download/dotnet/8.0

# 2. 恢复依赖
dotnet restore

# 3. 开发模式运行（自动热重载）
cd src/OccupationalHealth.Api
dotnet watch run

# 4. 访问
#    http://localhost:3001 (前端)
#    http://localhost:3001/swagger (API文档)
```

### 项目结构

```
OccupationalHealth-Win-CSharp/
├── OccupationalHealth.sln          ← 解决方案文件
├── build.bat                       ← 一键构建脚本
├── generate_icon.py                ← 图标生成工具
├── icon_preview.png                ← 图标预览
│
└── src/OccupationalHealth.Api/     ← 主项目
    ├── Program.cs                  ← 应用入口
    ├── appsettings.json            ← 配置文件
    │
    ├── Models/                     ← 数据模型
    │   ├── Entities.cs             ←   EF Core 实体（13个）
    │   └── Dtos.cs                 ←   请求/响应 DTO
    │
    ├── Data/                       ← 数据访问层
    │   ├── AppDbContext.cs          ←   DbContext 配置
    │   └── DataSeeder.cs           ←   种子数据
    │
    ├── Middleware/                 ← 中间件
    │   └── AuthMiddleware.cs        ←   JWT认证 + 角色守卫
    │
    ├── Services/                   ← 业务逻辑层
    │   ├── JwtService.cs           ←   JWT 签发/验证
    │   ├── AuthService.cs          ←   登录认证
    │   ├── UserService.cs          ←   用户管理
    │   ├── EmployeeService.cs      ←   员工管理 + Excel导入
    │   ├── PositionService.cs      ←   岗位树 + 危害因素继承
    │   ├── HazardFactorService.cs  ←   危害因素
    │   ├── OccupationService.cs    ←   职业字典
    │   ├── FactoryContactService.cs←   工厂联系人
    │   ├── ExamTaskService.cs      ←   体检任务流
    │   ├── ExamReportService.cs    ←   报告上传/下载
    │   ├── ExamPackageService.cs   ←   体检套餐
    │   └── DashboardService.cs     ←   仪表盘
    │
    ├── Controllers/                ← API 控制器
    │   ├── AuthController.cs       ←   /api/auth
    │   ├── AdminController.cs      ←   /api/admin
    │   ├── HazardFactorController.cs
    │   ├── OccupationController.cs
    │   ├── PositionController.cs
    │   ├── EmployeeController.cs
    │   ├── FactoryContactController.cs
    │   ├── HealthAgentController.cs
    │   ├── ExamTaskController.cs
    │   ├── ExamReportController.cs
    │   ├── ExamPackageController.cs
    │   ├── DashboardController.cs
    │   └── CUnitController.cs
    │
    └── wwwroot/                    ← 前端静态文件
        ├── index.html              ←   SPA 单页应用
        └── favicon.ico             ←   浏览器图标
```

### 扩展新模块

1. 在 `Models/Entities.cs` 中添加实体类
2. 在 `Data/AppDbContext.cs` 中添加 DbSet 和 Fluent API 配置
3. 在 `Services/` 中添加业务服务
4. 在 `Controllers/` 中添加控制器
5. 在 `Program.cs` 中注册服务

---

## 常见问题

### Q: 运行时提示端口被占用
```bash
# 修改端口，在 Server.exe 所在目录执行：
set PORT=3002
Server.exe
```

### Q: 如何重置数据？
删除 `data/occupational_health.db` 文件，重新启动 Server.exe 即可自动重建并初始化数据。

### Q: 如何备份数据？
复制 `data/occupational_health.db` 文件即可，所有数据都在这个文件中。

### Q: 如何修改 JWT 密钥？
编辑 `appsettings.json` 中的 `Jwt.Secret` 字段。

### Q: 支持 HTTPS 吗？
当前为 HTTP 模式，如需 HTTPS 可在 `appsettings.json` 中配置 Kestrel 的 HTTPS 端点，或使用反向代理（如 Nginx/IIS）。

### Q: 上传文件大小限制？
默认 20MB，可在 `appsettings.json` 的 `UploadSettings.MaxFileSize` 中修改。

### Q: 如何更换桌面图标？
修改 `wwwroot/favicon.ico`，重新运行 `generate_icon.py` 生成新图标，然后重新构建。

---

## 更新日志

### v3.0 (2026-06-05)

- ✨ **完整 C# 迁移** — 从 Node.js + Fastify + React 迁移到 ASP.NET Core 8
- ✨ **自包含 EXE 部署** — 单文件发布，内置 .NET 运行时
- ✨ **嵌入式 SQLite** — 无需安装数据库，内置一键初始化
- ✨ **职业健康图标** — 盾牌+红十字+齿轮元素的专业桌面图标
- ✨ **桌面快捷方式** — 首次运行自动创建
- ✨ **角色自动路由** — 登录后自动识别角色切换控制台
- ✨ **Swagger 文档** — 内置 API 文档页面
- ♻️ **架构优化** — 三层架构（Controller → Service → EF Core）
- ♻️ **危害因素继承** — 三源合并算法（direct > occupation > workshop）

### v2.0 (原始 Node.js 版)

- 统一用户表，支持 4 角色
- 岗位树 + GBZ188 危害因素标准
- 体检任务全生命周期管理
- 报告上传 + PDF 下载
- 多角色仪表盘

---

## 许可证

本项目代码仅供学习和参考。

---

*© 2026 职业健康体检管理平台 Team*
