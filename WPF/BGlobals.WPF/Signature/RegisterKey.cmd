@echo off

:: BatchGotAdmin
:-------------------------------------
REM  --> Check for permissions
>nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"

REM --> If error flag set, we do not have admin.
if '%errorlevel%' NEQ '0' (
    echo Requesting administrative privileges...
    goto UACPrompt
) else ( goto gotAdmin )

:UACPrompt
    echo Set UAC = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
    set params = %*:"=""
    echo UAC.ShellExecute "%~s0", "%params%", "", "runas", 1 >> "%temp%\getadmin.vbs"

    "%temp%\getadmin.vbs"
    exit /B

:gotAdmin
    if exist "%temp%\getadmin.vbs" ( del "%temp%\getadmin.vbs" )
    pushd "%CD%"
    CD /D "%~dp0"
:--------------------------------------

rem set sdkPath=%HOMEPATH%\Documents\Visual Studio 2012\Projects\AIMP7\SIG\SigAppSolution\SDK
set sdkPath=D:\Work\!deskNET\Sobees\Sobees\SourcesWPF_RTM_4_5\WPF\BGlobals.WPF
set snDir=C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\x64
"%snDir%\sn.exe" -d SobeesCodeSignKey
"%snDir%\sn.exe" -i "%sdkPath%\Signature/SobeesKey.snk" SobeesCodeSignKey
"%snDir%\sn.exe" -m y
set snDir=-
set sdkPath=
pause