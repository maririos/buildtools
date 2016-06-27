@if "%_echo%" neq "on" echo off
call executor.cmd %* build-managed
exit /b %ERRORLEVEL%