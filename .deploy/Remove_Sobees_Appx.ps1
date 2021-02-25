##Get-AppxPackage -allusers | where {$_.Name -like "*sobees*" } | out-gridview -PassThru | Remove-AppxPackage


Get-AppxPackage -allusers | where {$_.Name -like "*sobees*" } | Remove-AppxPackage


