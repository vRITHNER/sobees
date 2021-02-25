#test
##$msbuild="D:\Program Files (x86)\Microsoft Visual Studio\Preview\Enterprise\MSBuild\15.0\Bin\msbuild.exe"
##get-Childitem -path D:\DevTools\Wix\src -recurse -force  -filter *.proj | Foreach-Object {&$msbuild $_.FullName}


$appxFolder="C:\Work\!deskNET\Sobees\Sobees\SourcesWPF_RTM_4_5\Output\Appx"
$desktopFolder="SobeesDesktop"
$certName="auto-generated.cer"
$certPathCmd="$appxFolder\$desktopFolder\$certName"
$certName="sobees.com"
$certExe="C:\Program Files (x86)\Windows Kits\10\bin\x86\certmgr.exe" 


&$certExe -del -c -n $certName -s -r localmachine TrustedPeople
&$certExe -add  $certPathCmd -s -r localMachine trustedpeople
