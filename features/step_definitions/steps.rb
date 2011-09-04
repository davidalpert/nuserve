
require 'Win32/Process'
require File.join(File.expand_path(File.dirname(__FILE__)), 'DotNetConfigFileInfo.rb')

require 'test/unit/assertions'
include Test::Unit::Assertions

include FileUtils

def package_tool(package, tool)
	File.join(Dir.glob(File.join("./packages","#{package}.*")).sort.last, "tools", tool)
end

nuserve_startup_timeout_in_seconds = ENV["TIMEOUT"] || 8
api_key = 'secretKey'
project_root = '.'
project_packages_root = File.join(project_root, 'packages')
bin_root = File.join(project_root, 'temp')
nuserve_exe = File.expand_path(File.join(bin_root, 'nuserve.exe'))
$nuserve_exe_config = "#{nuserve_exe}.config"
$bin_packages_root = File.expand_path(File.join(bin_root, 'packages'))
temp_packages_root = './temp_packages'
nuget_exe = package_tool('NuGet.CommandLine', 'nuget.exe')
project_nupkg_files = Dir.glob("#{project_packages_root}/**/*.nupkg")

pipe = :nil
result = :nil

def update_config_set(key, val)
	puts "updating #{$nuserve_exe_config}:"
	puts "- <add key=\"#{key}\" value=\"#{val}\" />"

	config = DotNetConfigFileInfo.new($nuserve_exe_config)
	config.set_unique_appSetting!(key, val)
end

def get_config_setting(key)
	config = DotNetConfigFileInfo.new($nuserve_exe_config)
	config.get_appSetting(key)
end

Given /^nuserve is configured to manage packages at '(.+?)'$/ do |manage_packages_uri|
	update_config_set('EndpointSettings.PackageManagerUri', manage_packages_uri)
end

Given /^nuserve is configured to list packages at '(.+?)'$/ do |list_packages_uri|
	update_config_set('EndpointSettings.PackageListUri', list_packages_uri)
end

Given /^nuserve is configured to serve packages from '(.*?)'$/ do | pathToRepo |
	update_config_set('RepositorySettings.PathToServerPackageRepository', pathToRepo)
end

Given /^nuserve is configured to use '(.*?)' as an ApiKey$/ do | key |
	update_config_set('ApiSettings.ApiKey', key)
end

Given /^nuserve is running with an ApiKey$/ do
	Given "nuserve is running with an ApiKey of '#{api_key}'"
end

Given /^nuserve is running with an ApiKey of '(.*?)'$/ do | key |
	Given "nuserve is configured to use '#{key}' as an ApiKey"
	Given "nuserve is running"
end

Given /^nuserve is running$/ do
	pipe = IO.popen(nuserve_exe)
	puts "waiting #{nuserve_startup_timeout_in_seconds} seconds for nuserve to start..."
	(1..nuserve_startup_timeout_in_seconds.to_i).each do 
		#print '.'
		sleep 1
	end
	puts "... assuming that nuserve has started\n\n"

	list_uri = get_config_setting('EndpointSettings.PackageListUri') || '(default)'
	manage_uri = get_config_setting('EndpointSettings.PackageManagerUri') || '(default)'
	api_key = get_config_setting('ApiSettings.ApiKey') || '(default)'
	local_package_root = get_config_setting('RepositorySettings.PathToServerPacakgeRepository') || '(default)'

	puts "[#{pipe.pid}] #{nuserve_exe}"
	puts "listing on #{list_uri} and serving packages from #{local_package_root}\n\n"
	puts "managing on #{manage_uri} with an apikey of #{api_key}\n\n"
end

Given /^nuserve is running with no ApiKey$/ do

	puts "updating #{$nuserve_exe_config} to use no ApiKey"

	config = DotNetConfigFileInfo.new($nuserve_exe_config)
	config.remove_appSetting!('ApiSettings.ApiKey')

	Given 'nuserve is running'
end

Given /^there are (\d+) packages in the server's folder$/ do | n |

	puts "making '#{$bin_packages_root}'..."
	rm_rf $bin_packages_root
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
	When "I request a list of packages from 'http://localhost:8080/packages'"
end

When /^I request a list of packages from '(.*?)'$/ do | source |
	cmd = "#{nuget_exe} list -s #{source}"
	puts "\$ #{cmd}"
	result = `#{cmd}`
	puts result
end

When /^I push (\d+) package(s)?$/ do | n, plural |
	When "I push #{n} package#{plural} to 'http://localhost:8080' using an ApiKey of '#{api_key}'"
end

When /^I push (\d+) packages? to '(.*?)' using an ApiKey of '(.*?)'$/ do |n, source, key|
	packages_count = project_nupkg_files.length

	puts "found #{packages_count} packages in #{project_packages_root}"
	(packages_count >= n.to_i) or raise "Could not find #{n} packages."

	project_nupkg_files.first(n.to_i).each do |f|
		cmd = "#{nuget_exe} push #{f} #{key} -s #{source}"
		puts "\$ #{cmd}"
		result = `#{cmd}`
		puts result
	end
end

When /^I install a package locally$/ do
	When "I install a package from 'http://localhost:8080/packages/' into '#{temp_packages_root}'"
end

When /^I install a package from '(.*?)' into '(.*?)'$/ do | src, dest | 
	full_package_name = project_nupkg_files.first
	match = full_package_name.match(/^.*\/([a-zA-Z]+(\.[a-zA-Z]+)*)\.(\d+\.)+nupkg/i)
	package_id = $1

	rm_rf temp_packages_root

	cmd = "#{nuget_exe} install #{package_id} -s #{src} -OutputDirectory #{dest}"
	puts "\$ #{cmd}"
	result = `#{cmd}`
	puts result

	assert_match(/Successfully installed '[a-zA-Z]+(\.[a-zA-Z])* (\d+\.?)+/i, result)
end

Then /^I should have (\d+) packages? installed$/ do |n|
	Then "I should have #{n} packages installed in '#{temp_packages_root}'"
end

Then /^I should have (\d+) packages? installed in '(.*?)'$/ do |n, dest|
	installed_package_folders = Dir[File.join(dest, '*')].map { |a| File.basename(a) }
	assert_equal(n.to_i, installed_package_folders.length)
end

Then /^I should see (\d+) packages?$/ do | n |
	package_count = 0

	result.each_line do |line|
		package_count += 1 if line =~ /^(\w|\.)+\s+[0-9.]+/
	end

	assert_equal(n.to_i, package_count, "not the right number of packages")
end

# http://blog.robseaman.com/2008/12/12/sending-ctrl-c-to-a-subprocess-with-ruby
#def sysint_gpid; nil; end
#Process.kill( 'INT', sysint_gpid)

Before do
end

After do
	rm_rf $bin_packages_root

	Process.kill( 'KILL', pipe.pid ) unless pipe.nil?
	pipe.close unless pipe.nil?

	@keys = Array['ApiSettings.ApiKey',
                 'EndpointSettings.PackageManagerUri',
			        'EndpointSettings.PackageListUri',
			        'RepositorySettings.PathToServerPackageRepository']

	config = DotNetConfigFileInfo.new($nuserve_exe_config)
	@keys.each do |key|
		config.remove_appSetting(key)
	end
	config.save!
end

