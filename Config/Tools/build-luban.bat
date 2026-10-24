@echo off
rem 从 LubanSource/（vendored v5.1.0 源码）重新构建 Luban 生成器到 Luban/（gen-*.bat 使用该目录下的 Luban.dll）
setlocal
set SCRIPT_DIR=%~dp0

dotnet build "%SCRIPT_DIR%LubanSource\src\Luban\Luban.csproj" -c Release -o "%SCRIPT_DIR%Luban" %*

if %errorlevel% neq 0 (
    echo [build-luban] FAILED with code %errorlevel%
    exit /b %errorlevel%
)
echo [build-luban] OK -^> %SCRIPT_DIR%Luban\Luban.dll
endlocal
