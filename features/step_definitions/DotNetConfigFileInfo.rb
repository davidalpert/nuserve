require 'rexml/document'

class DotNetConfigFileInfo

	include FileUtils
	include REXML

	attr_accessor :file_path, :config_root
	@app_settings

	def initialize
		@file_path = :nil
		@config_root = :nil
	end

	def to_s
		@file_path
	end

	def load(file_path)
		File.open(file_path) do | config_file |
			@config_root = Document.new(config_file)
		end

		ensure_appSettings_node
	end

	def save(file_path)
		File.open(file_path, "w+") do | result | 
			formatter = REXML::Formatters::Default.new
			formatter.write(@config_root, result) rescue puts "error while writing config file"
		end
	end

	def set_unique_appSetting(key, val)
		#<add key="ApiSettings.ApiKey" value="nuget"/>
		@app_settings.delete_element("add[@key='#{key}']")
		@app_settings.add_element('add', {'key' => key, 'value' => val })
	end

	private 
	def ensure_appSettings_node
		@app_settings = @config_root.root.elements['appSettings']

		if @app_settings.nil?
			@config_root.root.elements << Element.new('appSettings')
			@app_settings = @config_root.root.elements['appSettings']
		end
	end

end
