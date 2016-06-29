@call executor.cmd build-native %*
if NOT [%ERRORLEVEL%]==[0] exit /b 1
@call executor.cmd build-managed %*
@exit /b %ERRORLEVEL%