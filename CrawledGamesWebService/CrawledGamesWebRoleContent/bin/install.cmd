REM Set the value of netfx to install appropriate .NET Framework. 
REM ***** To install .NET 4.5.2 set the variable netfx to "NDP452" *****
REM ***** To install .NET 4.6 set the variable netfx to "NDP46" *****
set netfx="NDP452"

REM ***** Setup .NET filenames and registry keys *****
if %netfx%=="NDP46" goto NDP46
    set netfxinstallfile="NDP452-KB2901954-Web.exe"
    set netfxregkey="0x5cbf5"
    goto logtimestamp
:NDP46
set netfxinstallfile="NDP46-KB3045560-Web.exe"
set netfxregkey="0x60051"

:logtimestamp
REM ***** Setup LogFile with timestamp *****
set timehour=%time:~0,2%
set timestamp=%date:~-4,4%%date:~-10,2%%date:~-7,2%-%timehour: =0%%time:~3,2%
set startuptasklog=%PathToInstallLogs%startuptasklog-%timestamp%.txt
set netfxinstallerlog=%PathToInstallLogs%NetFXInstallerLog-%timestamp%
echo Logfile generated at: %startuptasklog% >> %startuptasklog%

REM ***** Check if .NET is installed *****
echo Checking if .NET (%netfx%) is installed >> %startuptasklog%
reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v Release | Find %netfxregkey%
if %ERRORLEVEL%== 0 goto end

REM ***** Installing .NET *****
echo Installing .NET. Logfile: %netfxinstallerlog% >> %startuptasklog%
start /wait %~dp0%netfxinstallfile% /q /serialdownload /log %netfxinstallerlog% >> %startuptasklog% 2>>&1

:end
echo install.cmd completed: %date:~-4,4%%date:~-10,2%%date:~-7,2%-%timehour: =0%%time:~3,2% >> %startuptasklog%
