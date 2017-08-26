Namespace DefaultNamespace

    ''' <summary>
    ''' API呼び出し派生先クラス
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ApiChildClass
        Inherits ApiConnecter

        ''' <summary>
        ''' API名
        ''' </summary>
        ''' <remarks></remarks>
        Private _ApiName As String = "API Name"

        ''' <summary>
        ''' コンストラクタ
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' API処理
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Async Function Calling() As Task(Of Boolean)

            Dim result As Boolean = False
            Dim data As String = String.Empty

            // API DEFAULT QUERY Param SETTING
            // Call MyBase.AddQueryParam("Service", "WebAPI")
            // Call MyBase.AddQueryParam("Operation", _ApiName)

            // http info setting 
            // MyBase.BaseUri = "https://demo.sample.com/api"
            // MyBase.IsDebug = True Or FALSE 
            // MyBase.IsSSL = True as https False as http

            data = Await MyBase.GetWebResultData()

            If Not String.IsNullOrEmpty(data) Then
                ResultData = data
            End If

            result = True

            Return result
        End Function

    End Class
End Namespace