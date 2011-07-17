
require 'Win32/Process'

include FileUtils

def package_tool(package, tool)
	File.join(Dir.glob(File.join("./packages","#{package}.*")).sort.last, "tools", tool)
end

nuserve_startup_timeout_in_seconds = 5
project_root = '.'
project_packages_root = File.join(project_root, 'packages')
bin_root = File.join(project_root, 'src', 'nuserve', 'bin', 'Debug')
nuserve_exe = File.expand_path(File.join(bin_root, 'nuserve.exe'))
$bin_packages_root = File.expand_path(File.join(bin_root, 'packages'))
nuget_exe = package_tool('NuGet.CommandLine', 'nuget.exe')
project_nupkg_files = Dir.glob("#{project_packages_root}/**/*.nupkg")

pipe = :nil
result = :nil

Given /^nuserve is running$/ do
	puts "started: [#{pipe.pid}] #{nuserve_exe}"
end

Given /^there are (\d+) packages in the server's folder$/ do | n |

	rm_bin_packages
	FileUtils.mkdir_p $bin_packages_root

	packages_count = project_nupkg_files.length

	puts "found #{packages_count} packages in #{project_packages_root}"
	(packages_count >= n.to_i) or raise "Could not find #{n} packages."

	project_nupkg_files.first(n.to_i).each do |f|
		cp f,$bin_packages_root
		puts "- using: #{f}"
	end

end

When /^I request a list of packages$/ do
	result = `#{nuget_exe} list -s http://localhost:5656/packages`
end

Then /^I should see (\d+) packages$/ do | n |
	puts result
end

# http://blog.robseaman.com/2008/12/12/sending-ctrl-c-to-a-subprocess-with-ruby
#def sysint_gpid; nil; end
#Process.kill( 'INT', sysint_gpid)

Before do
	pipe = IO.popen(nuserve_exe)
	print "\nwaiting for nuserve to start"
	(1..nuserve_startup_timeout_in_seconds).each do 
		print '.'
		sleep 1
	end
	print "assuming that nuserve has started\n\n"
end

After do
	rm_bin_packages
	Process.kill( 'KILL', pipe.pid )
	pipe.close
end

def rm_bin_packages()
	# clean the packages root
	rm_rf $bin_packages_root
end

