cd /d "%~dp0"
net session >nul 2>&1
if %errorlevel% neq 0 (
    powershell -Command "Start-Process cmd -ArgumentList '/c %~f0' -Verb RunAs"
    exit /b
)

start /wait msiexec.exe /i "yggdrasil-0.5.12-x64.msi"
timeout 9
for /f %%a in ('PowerShell -Command "Get-Date -Format yyyyMMddHHmmss"') do set datetime=%%a
echo %datetime%
dll\sed -i "s/Peers: \[\]/Peers: [\n    tls:\/\/ip4.01.ekb.ru.dioni.su:9003\n  ]/g" %ALLUSERSPROFILE%\Yggdrasil\yggdrasil.conf
dll\sed -i "s/NodeInfo: {}/NodeInfo: {\n    name: calinka%datetime%\n  }/g" %ALLUSERSPROFILE%\Yggdrasil\yggdrasil.conf
net stop "Yggdrasil Service"
net start "Yggdrasil Service"
cd /d "%~dp0"
del sed*
"C:\Program Files\Yggdrasil\yggdrasilctl.exe" getPeers
timeout 9