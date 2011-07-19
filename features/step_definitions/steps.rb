
require 'Win32/Process'
require File.join(File.expand_path(File.dirname(__FILE__)), 'DotNetConfigFileInfo.rb')

require 'test/unit/assertions'
World(Test::Unit::Assertions) # not sure why this is needed?

include FileUtils

def package_tool(package, tool)
	File.join(Dir.glob(File.join("./packages","#{package}.*")).sort.last, "tools", tool)
end

nuserve_startup_timeout_in_seconds = 3
api_key = 'secretKey'
project_root = '.'
project_packages_root = File.join(project_root, 'packages')
bin_root = File.join(project_root, 'temp')
nuserve_exe = File.expand_path(File.join(bin_root, 'nuserve.exe'))
$bin_packages_root = File.expand_path(File.join(bin_root, 'packages'))
nuget_exe = package_tool('NuGet.CommandLine', 'nuget.exe')
project_nupkg_files = Dir.glob("#{project_packages_root}/**/*.nupkg")

pipe = :nil
result = :nil

Given /^nuserve is running$/ do

	nuserve_exe_config = "#{nuserve_exe}.config"

	config = DotNetConfigFileInfo.new
	config.load(nuserve_exe_config)
	config.set_unique_appSetting('ApiSettings.ApiKey', 'secretKey')
	config.save(nuserve_exe_config)

	pipe = IO.popen(nuserve_exe)
	puts "waiting for nuserve to start..."
	(1..nuserve_startup_timeout_in_seconds).each do 
		#print '.'
		sleep 1
	end
	puts "... assuming that nuserve has started\n\n"
	puts "[#{pipe.pid}] #{nuserve_exe}"
end

Given /^there are (\d+) packages in the server's folder$/ do | n |

	puts "making '#{$bin_packages_root}'..."
	rm_bin_packages
	FileUtils.mkdir_p $bin_packages_root

	packages_count = project_nupkg_files.length

	# puts "found #{packages_count} packages in #{project_packages_root}"
	(packages_count >= n.to_i) or raise "Could not find #{n} packages."

	project_nupkg_files.first(n.to_i).each do |f|
		cp f,$bin_packages_root
		puts "- using: #{f}"
	end

end

When /^I request a list of packages$/ do
	cmd = "#{nuget_exe} list -s http://localhost:5656/packages"
	puts "\$ #{cmd}"
	result = `#{cmd}`
	puts result
end

When /^I push (\d+) packages?$/ do | n |

	packages_count = project_nupkg_files.length

	puts "found #{packages_count} packages in #{project_packages_root}"
	(packages_count >= n.to_i) or raise "Could not find #{n} packages."

	project_nupkg_files.first(n.to_i).each do |f|
		cmd = "#{nuget_exe} push #{f} #{api_key} -s http://localhost:5656"
		puts "\$ #{cmd}"
		result = `#{cmd}`
		puts result
	end
end

Then /^I should see (\d+) packages?$/ do | n |
	package_count = 0

	result.each_line do |line|
		package_count += 1 if line.match(/^(\w|\.)+\s+[0-9.]+/)
	end

	assert_equal(package_count, n.to_i, "not the right number of packages")
end

# http://blog.robseaman.com/2008/12/12/sending-ctrl-c-to-a-subprocess-with-ruby
#def sysint_gpid; nil; end
#Process.kill( 'INT', sysint_gpid)

Before do
end

After do
	rm_bin_packages
	Process.kill( 'KILL', pipe.pid ) unless pipe.nil?
	pipe.close unless pipe.nil?
end

def rm_bin_packages()
	# clean the packages root
	rm_rf $bin_packages_root
end

