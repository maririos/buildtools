@call run.cmd build-native %*
if NOT [%ERRORLEVEL%]==[0] exit /b 1
@call run.cmd build-managed %*
@exit /b %ERRORLEVEL%