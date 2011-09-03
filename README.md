# Getting the source

- Download the source from the link above; or 
- Clone it to your machine via "git clone git://github.com/davidalpert/nuserve.git"

# Building the source

This sample project uses several nuget packages that are not included in the source.  In order to build the projects, you will need to set up the ruby environment as described below or install those nugets yourself.

If ruby makes you nervous, we have a Winnipeg [support group](http://winnipegrb.org/ "The most fun you can have with your pants on") for that.  In the meantime you will have to install the nugets yourself from a command-line using the [nuget.exe](http://blog.davidebbo.com/2011/01/installing-nuget-packages-directly-from.html "Installing NuGet packages directly from the command line") tool.

If, on the other hand, ruby is exciting, follow the instructions below and the included rakefile will download and configure the missing dependencies for you.

Drop me a line if you run into trouble.

Happy spelunking!

## ... using Chocolatey and Rake

Thanks to Rob Reynolds and Chocolatey, setting up nuserve's development environment is easy peasy.

1. Open a console to the root of the project source and run 'setup'
1. Go grab a cup of coffee while Chocolatey does the rest.

Chocolatey is built on top of NuGet to provide an Windows install experience for executables similar to that of apt-get on ngix systems.

Nuserve's setup script will:

1. Set up a Chocolatey repository for NuGet application packages at C:\NuGet (the packages themselves will go in
c:\NuGet\lib\ with batch files in c:\NuGet\bin pointing to the individual pacakge executables) including
  * C:\NuGet\bin\chocolatey.bat
  * C:\NuGet\bin\cinst.bat
  * C:\NuGet\bin\cinstm.bat
  * C:\NuGet\bin\cup.bat
  * C:\NuGet\bin\clist.bat
  * C:\NuGet\bin\cver.bat
1. Install the chocolatey packages that bootstrap nuserve's development environment:
  * 7zip.commandline 
  * ruby
  * ruby.devkit
  * rubygems
  * bundler
1. run the default rake task which will
  1. download nuserve's nuget library dependencies
  1. build the source
  1. run the unit tests
  1. run the integration tests
  1. deploy nuserve to a {project_root}/temp/ folder
  1. invoke cucumber to run the features
  1. deploy nuserve to a {project_root}/build folder that contains everything you need to run a nuget server from the command line or as a windows service.

Once this setup script is complete, you can copy the entire contents of the {project_root}/build/ folder to the location
of your choice, customize the appSettings in nuserve.exe.config and you're all set!  Run nuserve.exe in the console or run 'nuserve install' to install it as a windows service.

If you are content to run nuserve from the build folder, the rakefile contains the following tasks to help you:
* rake install
* rake start
* rake stop
* rake uninstall

Just remember that the next time you build nuserve from source any packages stored in that build folder will be removed.

