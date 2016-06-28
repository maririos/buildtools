@if "%_echo%" neq "on" echo off
setlocal

if not defined VisualStudioVersion (
    if defined VS140COMNTOOLS (
        call "%VS140COMNTOOLS%\VsDevCmd.bat"
        goto :Run
    )

    if defined VS120COMNTOOLS (
        call "%VS120COMNTOOLS%\VsDevCmd.bat"
        goto :Run
    )

    echo Error: Visual Studio 2013 or 2015 required.
    echo        Please see https://github.com/dotnet/corefx/blob/master/Documentation/building/windows-instructions.md for build instructions.
    exit /b 1
)

:Run
:: Restore the Tools directory
call %~dp0init-tools.cmd
if NOT [%ERRORLEVEL%]==[0] (
  exit /b 1
)

set _toolRuntime=%~dp0Tools
set _dotnet=%_toolRuntime%\dotnetcli\dotnet.exe

echo Running: %_dotnet% %_toolRuntime%\executor.exe %*
call %_dotnet% %_toolRuntime%\executor.exe %*
if NOT [%ERRORLEVEL%]==[0] (
  exit /b 1
)

exit /b 0