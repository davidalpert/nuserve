### install chocolatey ###
$url = "http://packages.nuget.org/v1/Package/Download/Chocolatey/0.9.8.4"
$tempDir = "$env:TEMP\chocolatey\chocInstall"
if (![System.IO.Directory]::Exists($tempDir)) {[System.IO.Directory]::CreateDirectory($tempDir)}
$file = Join-Path $tempDir "chocolatey.zip"

# download the package
Write-Host "Downloading $url to $file"
$downloader = new-object System.Net.WebClient
$downloader.DownloadFile($url, $file)

# unzip the package
Write-Host "Extracting $file to $destination..."
$shellApplication = new-object -com shell.application 
$zipPackage = $shellApplication.NameSpace($file) 
$destinationFolder = $shellApplication.NameSpace($tempDir) 
$destinationFolder.CopyHere($zipPackage.Items(),0x10)

# call chocolatey install
Write-Host "Installing chocolatey on this machine"
$chocInstallPS1 = "$tempDir\tools\chocolateyInstall.ps1"
& $chocInstallPS1
### finish installing chocolatey ###

# update chocolatey to the latest version
Write-Host "Updating chocolatey to the latest version"
cup chocolatey

# install nuget and ruby if they are missing
cinstm nuget.commandline
cinstm ruby
cinstm ruby.devkit

#perform ruby updates and get gems
gem update --system
gem install rake
gem install bundler

#restore the nuget packages
$nugetConfigs = Get-ChildItem '.\' -Recurse | ?{$_.name -match "packages\.config"} | select
foreach ($nugetConfig in $nugetConfigs) {
  Write-Host "restoring packages from $($nugetConfig.FullName)"
  nuget install $($nugetConfig.FullName) /OutputDirectory packages
}

rake

#notes - missing dev-kit, required by json native gem
# error: C:/Ruby187/lib/ruby/site_ruby/1.8/rubygems/defaults/operating_system.rb:9: The 'json' native gem requires installed build tools. (Gem::InstallError)
#      Please update your PATH to include build tools or download the DevKit from 'http://rubyinstaller.org/downloads' and follow the instructions at 'http://github.com/oneclick/rubyinstaller/wiki/Development-Kit'