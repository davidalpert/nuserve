require 'rexml/document'

class DotNetConfigFileInfo

	include FileUtils
	include REXML

	attr_accessor :file_path, :config_root
	@app_settings

	def initialize(file_path = nil)
		self.load(file_path)
	end

	def to_s
		@file_path
	end

	def load(file_path)
		raise "cannot load a null file_path" if file_path.nil?

		@file_path = file_path

		File.open(file_path) do | config_file |
			@config_root = Document.new(config_file)
		end

		ensure_appSettings_node
	end

	def save(file_path = nil)
		file_path = @file_path if file_path.nil?
		raise "cannot save to a null file_path" if file_path.nil?

		File.open(file_path, "w+") do | result | 
			formatter = REXML::Formatters::Default.new
			formatter.write(@config_root, result) rescue puts "error while writing config file"
		end
	end

	def add_appSetting(key, val)
		@app_settings.add_element('add', {'key' => key, 'value' => val })
	end

	def add_appSetting!(key, val)
		add_appSetting(key, val)
		save
	end

	def set_unique_appSetting(key, val)
		#e.g. <add key="ApiSettings.ApiKey" value="nuget"/>
		remove_appSetting(key)
		add_appSetting(key, val)
	end

	def set_unique_appSetting!(key, val)
		set_unique_appSetting(key,val)
		save
	end

	def remove_appSetting(key)
		@app_settings.delete_element("add[@key='#{key}']")
	end

	def remove_appSetting!(key)
		remove_appSetting(key)
		save
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
