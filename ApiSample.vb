Imports System
Imports DefaultNamespace

Public Class ApiSample
	Public Shared Sub Main()
		' your code goes here
		Dim apicaller = new ApiChildClass()

		Dim result as String 

		try
		    With apicaller
			.BaseUri = "https://demo.sample.com/api"
			.IsDebug = True 
			.IsSSL = True 

			// query param
			.AddQueryParam("a","1")
			.AddQueryParam("b","demo")
			.AddQueryParam("c","100")
			.AddQueryParam("type","xml")

			result = Await .Calling()
		    End With

		'いかなる例外もスローする
		Catch ex As Exception
		    Debug.WriteLine(ex.message)
		End Try
	End Sub
End Class
