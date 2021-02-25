call %comspec% /C ""C:\Program Files\Microsoft Visual Studio 9.0\VC\vcvarsall.bat"" x86
set mageexe="C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\Mage.exe"
set product=bDule
set productapplication=%product%.application
set supporturl="http://www.sobees.com" 
set majorversion=0
set minorversion=1
set buildversion=1
set revisionversion=45
set version=%majorversion%.%minorversion%.%buildversion%.%revisionversion%
set bDuleFolderversion=Application Files\bDule_%majorversion%_%minorversion%_%buildversion%_%revisionversion%

set fromfolder="C:\Work\!DeskNET\bDule\Output\WPF\Debug"
set deployfolder="C:\Work\!DeskNET\bDule\Output\WPF\DebugObfuscated\app.publish\%bDuleFolderversion%"

robocopy %fromfolder% %deployfolder% /s /v /xd app.publish Themes /xf *.dsomap *.pdb *vshost*.* Sobees.Library.BFacebookLib.xml %productapplication% /purge

rem copy obfuscated files
set fromfolder="C:\Work\!DeskNET\bDule\Output\WPF\DebugObfuscated\Debug"
robocopy %fromfolder% %deployfolder% /s /v 

rem pause
set deployfolder="C:\Work\!DeskNET\bDule\Output\WPF\DebugObfuscated\app.publish"
set deployurl=http://www.bdule.com/setup/bDule.application

set manifest="%bDuleFolderversion%\bDule.exe.manifest"
set iconFile=%version%\sobees.ico
set certFile=C:\Work\!DeskNET\bDule\Sources\Globals\Globals\Delivery\bDule.pfx
set pwd=Eludb1015
cd %deployfolder%

%mageexe% -New Application -ToFile %manifest% -Name %product% -Version %version% -IconFile %iconFile% -FromDirectory "%bDuleFolderversion%" -TrustLevel FullTrust -wpf false

%mageexe% -sign %manifest% -CertFile %certFile% -Pwd %pwd%

%mageexe% -New Deployment -Install true -ToFile %productapplication% -Name %product% -Version %version% -AppManifest %manifest% -Publisher "sobees ltd" -SupportURL %supporturl% -providerUrl %deployurl% 

%mageexe% -Update %productapplication% -AppManifest %manifest% -CertFile %certFile% -Pwd %pwd%

%mageexe% -sign %productapplication% -CertFile %certFile% -Pwd %pwd%

pause