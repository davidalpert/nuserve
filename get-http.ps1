function get-http([string]$url)
{
	$routine  = "get-http:"

	trap
	{
		Write-Host "$routine Trap: access URL $url."
		Write-Host "$routine Error = $_"
		Write-Host "$routine $_.InvocationInfo.PositionMessage"
		continue
	}

	$request  = [System.Net.WebRequest]::Create($url);
	#$request.UseDefaultCredentials = $true

	$response = $request.GetResponse()
	if ($response)
	{
		Write-Host $routine "response statuscode = " $response.StatusDescription
		Write-Host $routine "response characterset = " $response.CharacterSet
		Write-Host $routine "response length in bytes = " $response.ContentLength.ToString()
		$response.Close()
	}
}
