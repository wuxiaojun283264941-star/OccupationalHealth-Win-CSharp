@echo off
chcp 65001 >nul
title 职业健康体检管理平台 - 构建脚本
setlocal enabledelayedexpansion

echo ========================================
echo  职业健康体检管理平台 - 构建脚本
echo  结构化文件夹发布
echo ========================================
echo.
cd /d "%~dp0"

echo [检查] 系统中是否有 .NET SDK...
dotnet --version >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo ⚠ 未检测到 .NET SDK
    echo.
    echo   请先安装 .NET 8 SDK:
    echo   https://dotnet.microsoft.com/download/dotnet/8.0
    echo.
    echo   安装后重新运行本脚本即可。
    pause
    exit /b 1
)
echo   已检测到 .NET SDK
echo.

echo [1/4] 恢复 NuGet 包...
dotnet restore OccupationalHealth.sln
if %ERRORLEVEL% NEQ 0 (
    echo 错误: NuGet 包恢复失败
    pause
    exit /b 1
)

echo [2/4] 编译项目...
dotnet build OccupationalHealth.sln -c Release
if %ERRORLEVEL% NEQ 0 (
    echo 错误: 编译失败
    pause
    exit /b 1
)

echo [3/4] 发布为文件夹...
dotnet publish src/OccupationalHealth.Api/OccupationalHealth.Api.csproj -c Release -o publish
if %ERRORLEVEL% NEQ 0 (
    echo 错误: 发布失败
    pause
    exit /b 1
)

echo [4/4] 整理发布结构...
set "OUTPUT=%~dp0OccupationalHealth-发布"

REM 清理旧目录
if exist "%OUTPUT%" rmdir /s /q "%OUTPUT%"

REM 创建新的目录结构
mkdir "%OUTPUT%\客户端"
mkdir "%OUTPUT%\服务器端"
mkdir "%OUTPUT%\系统部署和支持库"
mkdir "%OUTPUT%\软件说明"

REM 复制服务器端（publish目录的全部内容到 服务器端\）
xcopy "%~dp0publish\*" "%OUTPUT%\服务器端\" /E /I /Y >nul

REM 复制客户端文件
xcopy "%~dp0deploy\客户端\*" "%OUTPUT%\客户端\" /E /I /Y >nul

REM 复制部署支持库
xcopy "%~dp0deploy\系统部署和支持库\*" "%OUTPUT%\系统部署和支持库\" /E /I /Y >nul

REM 复制软件说明
copy "%~dp0README.md" "%OUTPUT%\软件说明\README.md" >nul 2>&1
copy "%~dp0icon_preview.png" "%OUTPUT%\软件说明\软件预览.png" >nul 2>&1

REM 清理临时 publish 文件夹
rmdir /s /q "%~dp0publish"

echo.
echo ========================================
echo  构建成功！
echo ========================================
echo.
echo  发布目录: %OUTPUT%
echo.
echo  %OUTPUT%\
echo  ├── 客户端\                  ← 给使用者
echo  │   ├── 启动客户端.bat       ← 双击打开浏览器
echo  │   └── 服务器配置.txt        ← 编辑服务器IP和端口
echo  │
echo  ├── 服务器端\                ← 部署到服务器
echo  │   └── Server.exe           ← 双击启动
echo  │
echo  ├── 系统部署和支持库\        ← 部署文档
echo  │   └── 部署说明.txt
echo  │
echo  └── 软件说明\               ← 详细文档
echo      └── README.md
echo.
echo  ▸ 单机使用: 直接双击 客户端\启动客户端.bat
echo  ▸ 局域网部署: 修改 客户端\服务器配置.txt → 改IP
echo  ▸ 默认账号: admin / admin123
echo.
echo  提示：整个 "OccupationalHealth-发布" 文件夹
echo  可以直接复制到任何 Windows 电脑上使用。
echo.
pause
