@echo off
mkdir C:\users\%USERNAME%\chrysalis
xcopy /O /X /E /H /K cenc C:\users\%USERNAME%\chrysalis
setx PATH=%PATH;C:/users/%USERNAME%/chrysalis/cenc/bin/Debug
echo "Installed cenc. Goto http://erwijet.github.io/chrysalis/cenc for more information"
pause