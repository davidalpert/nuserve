Feature: Pulling a package from the server to install locally

	Scenario: Install a package that exists on the server
		Given nuserve is running
		  And there are 3 packages in the server's folder
		 When I install a package locally
		 Then I should have 1 package installed

	Scenario: Install a package that exists on the server from a configured endpoint into a custom folder
		Given nuserve is configured to list packages at 'http://localhost:8081'
		  And nuserve is running
		  And there are 3 packages in the server's folder
		 When I install a package from 'http://localhost:8081' into './temp/installed_packages'
		 Then I should have 1 package installed in './temp/installed_packages'
