@echo off
chcp 65001 >nul
title 职业健康体检管理平台 - 客户端启动器
setlocal enabledelayedexpansion

:start
cls
echo ╔══════════════════════════════════════════╗
echo ║    职业健康体检管理平台 - 客户端          ║
echo ║    启动后自动连接服务器                    ║
echo ╚══════════════════════════════════════════╝
echo.
echo  当前时间: %date% %time%
echo.

REM ===== 自动检测配置文件的路径 =====
set "CONFIG_FILE=服务器配置.txt"

REM 检查当前目录是否存在
if not exist "%CONFIG_FILE%" (
    REM 尝试找上级目录
    if exist "..\服务器配置.txt" set "CONFIG_FILE=..\服务器配置.txt"
    if exist "..\客户端\服务器配置.txt" set "CONFIG_FILE=..\客户端\服务器配置.txt"
)

echo  配置文件: %CONFIG_FILE%
echo.

if not exist "%CONFIG_FILE%" (
    echo ⚠ 未找到服务器配置.txt 文件！
    echo   将使用默认配置连接...
    echo.
    set "IP=localhost"
    set "PORT=3001"
    set "PROTOCOL=http"
    goto :connect
)

REM ===== 读取服务器配置 =====
set "IP="
set "PORT="
set "PROTOCOL="

for /f "usebackq tokens=1,* delims==" %%a in ("%CONFIG_FILE%") do (
    set "_key=%%a"
    set "_val=%%b"
    REM 去除首尾空格
    for /f "tokens=*" %%x in ("!_key!") do set "_key=%%x"
    for /f "tokens=*" %%x in ("!_val!") do set "_val=%%x"
    
    if /i "!_key!"=="IP地址" set "IP=!_val!"
    if /i "!_key!"=="端口" set "PORT=!_val!"
    if /i "!_key!"=="协议" set "PROTOCOL=!_val!"
)

REM 使用默认值（如果配置项缺失）
if "%IP%"=="" set "IP=localhost"
if "%PORT%"=="" set "PORT=3001"
if "%PROTOCOL%"=="" set "PROTOCOL=http"

:connect
set "URL=%PROTOCOL%://%IP%:%PORT%"

echo  ┌─────────────────────────────────┐
echo  │  服务器地址: %URL%
echo  │  状态: 正在打开浏览器...
echo  └─────────────────────────────────┘
echo.

REM ===== 启动浏览器 =====
start "" "%URL%"

echo  浏览器已打开。
echo.
echo  ▸ 如果无法访问，请检查：
echo    1. 服务器电脑是否已启动 Server.exe
echo    2. 本配置文件中的 IP 和端口是否正确
echo    3. 防火墙是否放行了该端口
echo    4. 网络是否互通（可用 ping 命令测试）
echo.
echo  ▸ 修改服务器地址：编辑"服务器配置.txt"后重新启动本程序
echo.
echo  ========================================
echo  菜单：
echo    [1] 重新连接
echo    [2] 编辑服务器配置
echo    [3] 查看使用说明
echo    [0] 退出
echo  ========================================
echo.
set /p "choice=请输入 [0-3]: "

if "%choice%"=="1" goto :start
if "%choice%"=="2" (
    if exist "..\客户端\服务器配置.txt" (
        notepad "..\客户端\服务器配置.txt"
    ) else if exist "服务器配置.txt" (
        notepad "服务器配置.txt"
    ) else (
        echo 配置文件未找到！
    )
    echo.
    pause
    goto :start
)
if "%choice%"=="3" (
    if exist "客户端使用说明.txt" (
        type "客户端使用说明.txt"
    ) else (
        echo 使用说明文件未找到。
    )
    echo.
    pause
    goto :start
)
exit /b 0
