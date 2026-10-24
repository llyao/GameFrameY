@echo off
rem 生成服务端配置：代码与数据(bytes) -> Server/Config/Generate（仅 s 组的表）
setlocal
set SCRIPT_DIR=%~dp0
set LUBAN_DLL=%SCRIPT_DIR%Tools\Luban\Luban.dll

dotnet "%LUBAN_DLL%" ^
    -t server -c cs-bin -d bin ^
    --conf "%SCRIPT_DIR%luban.conf" ^
    -x outputCodeDir="%SCRIPT_DIR%..\Server\Config\Generate" ^
    -x outputDataDir="%SCRIPT_DIR%..\Server\Config\Datas" ^
    %*

if %errorlevel% neq 0 (
    echo [gen-server] FAILED with code %errorlevel%
    exit /b %errorlevel%
)
echo [gen-server] OK
endlocal
