### install chocolatey ###
iex ((new-object net.webclient).DownloadString("http://bit.ly/psChocInstall"))

# install nuget and ruby if they are missing
cinstm nuget.commandline
cinstm ruby.devkit

#perform ruby updates and get gems
gem update --system
gem install rake
gem install bundler

rake
