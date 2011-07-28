try {

  $psFile = Join-Path "$(Split-Path -parent $MyInvocation.MyCommand.Definition)" 'nuserveInstall.ps1'	
  Start-ChocolateyProcessAsAdmin "& `'$psFile`'"
  
  Write-ChocolateySuccess 'nuserve'
} catch {
  Write-ChocolateyFailure 'nuserve' "$($_.Exception.Message)"
  throw 
}