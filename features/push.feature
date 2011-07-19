Feature: Pushing a package to the server and optionally publishing it

	Scenario: Push a new package to the server (and publish it)
		Given nuserve is running with an ApiKey
		  And there are 0 packages in the server's folder
		 When I push 1 package
		  And I request a list of packages
		 Then I should see 1 package

	Scenario: Push a new package without any ApiKey
		Given nuserve is running with no ApiKey
		  And there are 0 packages in the server's folder
		 When I push 1 package
		  And I request a list of packages
		 Then I should see 0 package

	Scenario: Push a new package with an empty ApiKey
		Given nuserve is running with an ApiKey of ''
		  And there are 0 packages in the server's folder
		 When I push 1 package
		  And I request a list of packages
		 Then I should see 0 package

	Scenario: Push a new package with an empty ApiKey
		Given nuserve is configured to list packages at 'http://localhost:5656/packages'
		  And nuserve is configured to manage packages at 'http://localhost:5656/' 
		  And nuserve is configured to use 'myKey' as an ApiKey
		  And there are 0 packages in the server's folder
		  And nuserve is running 
		 When I push 1 package to 'http://localhost:5656/' using an ApiKey of 'myKey'
		  And I request a list of packages
		 Then I should see 1 package

