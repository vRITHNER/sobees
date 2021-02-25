@echo off
set appxFolder=C:\Work\!deskNET\Sobees\Sobees\SourcesWPF_RTM_4_5\Output\Appx
set desktopFolder=SobeesDesktop
set certName=auto-generated.cer
set certPathCmd=%appxFolder%\%desktopFolder%\%certName%
set certName="sobees.com"
set runas=C:\windows\system32\runas.exe /env /savecred /user:vrithner@hotmail.com
set certExe="certmgr.exe"
set path=%path%;C:\Program Files (x86)\Windows Kits\10\bin\x86\

@echo on
%certExe% -del -c -n %certName% -s -r localmachine TrustedPeople"
%certExe% -add  %certPathCmd% -s -r localMachine trustedpeople"


pause