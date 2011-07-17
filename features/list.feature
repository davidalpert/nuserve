Feature: Display a list of packages from the server

	Scenario: List packages when there are packages available
		Given nuserve is running
		  And there are 3 packages in the server's folder
		 When I request a list of packages
		 Then I should see 3 packages
