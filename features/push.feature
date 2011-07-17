Feature: Pushing a package to the server and optionally publishing it

	Scenario: Push a new package to the server (and publish it)
		Given nuserve is running
		  And there are 0 packages in the server's folder
		 When I push 1 package
		  And I request a list of packages
		 Then I should see 1 package

