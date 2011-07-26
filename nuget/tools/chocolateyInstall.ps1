try {
  # shut down and remove the service
  write-host "Shutting down and uninstalling the service if it is installed"
  try {
    sc.exe stop nuserve
    sc.exe delete nuserve
  } catch {
    write-host "NOTE: NuServe not installed."
  }
  
  # download and unpack a zip file
  $nuservePath = "$env:SystemDrive\tools\nuserve"
  Install-ChocolateyZipPackage 'nuserve' 'https://github.com/downloads/davidalpert/nuserve/nuserve-0.3.0-d776d7aa017.zip' "$nuservePath"

  $nuservePackages = Join-Path $nuservePath "packages"
  if (![System.IO.Directory]::Exists($nuservePackages)) {[System.IO.Directory]::CreateDirectory($nuservePackages)}
  $nuserveLogs = Join-Path $nuservePath "logs"
  if (![System.IO.Directory]::Exists($nuserveLogs)) {[System.IO.Directory]::CreateDirectory($nuserveLogs)}
  
  #------- ADDITIONAL SETUP -------#
  write-host "Installing NuServe"
  $nuserveExe = Join-Path $nuservePath "nuserve.exe"
  & $nuserveExe install
  
  write-host "Starting NuServe service."
  try {
    sc.exe start nuserve
  } catch {
    write-host "Not able to start NuServe: $($_.Exception.Message). Other than that, everything was a success."
  }
  
  Write-ChocolateySuccess 'nuserve'
} catch {
  Write-ChocolateyFailure 'nuserve' "$($_.Exception.Message)"
  throw 
}