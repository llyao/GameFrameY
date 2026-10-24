@echo off
rem 生成客户端配置：代码 -> Client/Assets/GameFrameY/Config/Generate，数据(bytes) -> Client/Assets/Bundles/Config
setlocal
set SCRIPT_DIR=%~dp0
set LUBAN_DLL=%SCRIPT_DIR%Tools\Luban\Luban.dll

dotnet "%LUBAN_DLL%" ^
    -t client -c cs-bin -d bin ^
    --conf "%SCRIPT_DIR%luban.conf" ^
    -x outputCodeDir="%SCRIPT_DIR%..\Client\Assets\GameFrameY\Config\Generate" ^
    -x outputDataDir="%SCRIPT_DIR%..\Client\Assets\Bundles\Config" ^
    %*

if %errorlevel% neq 0 (
    echo [gen-client] FAILED with code %errorlevel%
    exit /b %errorlevel%
)
echo [gen-client] OK
endlocal
