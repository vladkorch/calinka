cd /d "%~dp0"\..
set CALINKA=%cd%
mkdir temp data apps
powershell -Command "Start-Process cmd -Verb RunAs -ArgumentList '/c setx -m CALINKA %cd% & setx -m PATH \"%cd%\assets;%PATH%\"'"
powershell -Command "Start-Process powershell -Verb RunAs -ArgumentList '& {Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Force};'"
set PATH=%PATH%;%cd%\assets
wget -O windowsdesktop-runtime-win-x64.exe https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/8.0.18/windowsdesktop-runtime-8.0.18-win-x64.exe
start /w assets\windowsdesktop-runtime-win-x64.exe /install /quiet /norestart
start /w assets\MicrosoftEdgeWebview2Setup.exe /silent /install
wget -O IPFS-Desktop.exe https://github.com/ipfs/ipfs-desktop/releases/download/v0.44.0/ipfs-desktop-setup-0.44.0-win-x64.exe
start /w assets\IPFS-Desktop.exe
start /w msiexec.exe /i "yggdrasil-0.5.12-x64.msi"
net session >nul 2>&1
if %errorlevel% neq 0 (
    powershell -Command "Start-Process cmd -ArgumentList '/c %~f0' -Verb RunAs"
    exit /b
)
for /f %%a in ('PowerShell -Command "(Get-Date).ToUniversalTime().ToString('yyyyMMddHHmmss')"') do set "datetime=%%a"; echo %datetime%
dll\sed -i "s/Peers: \[\]/Peers: [\n    tls:\/\/ip4.01.ekb.ru.dioni.su:9003\n  ]/g" %ALLUSERSPROFILE%\Yggdrasil\yggdrasil.conf
dll\sed -i "s/NodeInfo: {}/NodeInfo: {\n    name: calinka%datetime%\n  }/g" %ALLUSERSPROFILE%\Yggdrasil\yggdrasil.conf
net stop "Yggdrasil Service"
net start "Yggdrasil Service"
cd /d "%~dp0"
del sed*
"C:\Program Files\Yggdrasil\yggdrasilctl.exe" getPeers
timeout 9
copy assets\mstart.cmd "C:\Users\%USERNAME%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\mstart.cmd"
rd /s /q temp
mkdir temp
shutdown /r /f /t 60
