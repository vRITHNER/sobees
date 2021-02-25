set deliFolder=C:\Work\!deskNET\Sobees\DeploiementV45\WPF\Delivery
set msi=C:\Work\!deskNET\Sobees\Sobees\SourcesWPF_RTM_4_5\Setup\SobeesMsiSetup\sobeessetup.msi
set appxFolder=C:\Work\!deskNET\Sobees\Sobees\SourcesWPF_RTM_4_5\Output\Appx
rem DesktopAppConverter.exe -Setup -BaseImage Q:\Users\vrith\Downloads\BaseImage-15055.wim
DesktopAppConverter.exe -Installer %msi% -Destination %appxFolder% -PackageName "SobeesDesktop" -Publisher "CN=sobees.com" -Version 0.8.9.2 -MakeAppx -PackageArch x86 -sign
pause