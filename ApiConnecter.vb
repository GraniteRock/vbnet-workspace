Imports System.Net.Http
Imports System.Web.Script.Serialization
Imports System.Net
Imports System.Security.Cryptography.X509Certificates
Imports System.Net.Security
 
Namespace DefaultNamespace
    ''' <summary>
    ''' API接続クラス
    ''' </summary>
    ''' <remarks></remarks>
    MustOverride Class ApiConnecter
 
        ''' <summary>
        ''' ベースとなるプロパティ
        ''' </summary>
        ''' <remarks></remarks>
        Private _baseUri As String
 
        ''' <summary>
        ''' ベースとなるURL（プロパティ）
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BaseUri As String
            Get
                Return _baseUri
            End Get
            Set(value As String)
                _baseUri = value
            End Set
        End Property
 
        ''' <summary>
        ''' SSL通信かどうかの判定
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsSSL As Boolean
            Get
                Return _isSSL
            End Get
            Set(value As Boolean)
                _isSSL = value
            End Set
        End Property
 
        ''' <summary>
        ''' SSL通信
        ''' </summary>
        ''' <remarks></remarks>
        Private _isSSL As Boolean
 
        ''' <summary>
        ''' テストモードかどうかプロパティ
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsDebug As Boolean
            Get
                Return _isDebug
            End Get
            Set(value As Boolean)
                _isDebug = value
            End Set
        End Property
 
        ''' <summary>
        ''' テストモードかどうか
        ''' </summary>
        ''' <remarks></remarks>
        Private _isDebug As Boolean
        
         ''' <summary>
        ''' タイムアウト時間
        ''' </summary>
        ''' <remarks></remarks>
        Private _dblTimeout As Double       
 
        ''' <summary>
        ''' クエリパラメータ
        ''' </summary>
        ''' <remarks></remarks>
        Private _queryParamList As New Dictionary(Of String, String)()
 
 
        ''' <summary>
        ''' 結果のData
        ''' </summary>
        ''' <remarks></remarks>
        Private _resultData As String
 
        ''' <summary>
        ''' 結果のData（プロパティ）
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ResultData As String
            Get
                Return _resultData
            End Get
            Set(value As String)
                _resultData = value
            End Set
        End Property
 
        ''' <summary>
        ''' コンストラクタ
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            Call ClearQueryParam()
            _isDebug = False
            _isSSL = False
            _dblTimeout = 30.0
        End Sub
 
        ''' <summary>
        ''' クエリパラメータのクリア
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ClearQueryParam()
            _queryParamList.Clear()
        End Sub
 
        ''' <summary>
        ''' クエリパラメータの追加
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Public Sub AddQueryParam(ByVal key As String, ByVal value As String)
            _queryParamList.Add(key, value)
        End Sub
 
 
 
        ''' <summary>
        ''' 信頼できないSSL証明書を「問題なし」にするメソッド
        ''' </summary>
        ''' <param name="sender">リモート証明書を検証するコールバック</param>
        ''' <param name="certificate">X509証明書関連</param>
        ''' <param name="chain">X509証明書関連</param>
        ''' <param name="sslPolicyErrors">SSLポリシー</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function OnRemoteCertificateValidationCallback( _
          ByVal sender As Object, _
          ByVal certificate As X509Certificate, _
          ByVal chain As X509Chain, _
          ByVal sslPolicyErrors As SslPolicyErrors _
        ) As Boolean
 
            'テストモードの場合は信頼できないSSL証明書でも問題なしとする
            If _isDebug = True Then
                Return True  ' 「SSL証明書の使用は問題なし」と示す
            Else
                Return False '不正なSSL証明書は問題ありとする
            End If
 
        End Function
 
        ''' <summary>
        ''' クエリパラメータの作成
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function MakeQuery(ByVal data As Dictionary(Of String, String)) As Task(Of String)
            Using content = New FormUrlEncodedContent(data)
 
                Dim result As Task(Of String) = content.ReadAsStringAsync()
 
                Return result
 
            End Using
        End Function
 
        ''' <summary>
        ''' Webの返ってきた内容を返す
        ''' </summary>
        ''' <returns>HTTP結果文字列を返す</returns>
        ''' <remarks></remarks>
        Public Async Function GetWebResultData() As Task(Of String)
 
            Dim result As String = String.Empty
            Dim param As String = String.Empty
 
            Dim query As String = String.Empty
 
            Using client = New HttpClient()
 
                ' タイムアウトをセット（オプション）
                client.Timeout = TimeSpan.FromSeconds(_dblTimeout)
 
                Try
 
                    'SSLの場合はコールバック関数を呼び出す
                    If _isSSL Then
                        ServicePointManager.ServerCertificateValidationCallback = _
                            New RemoteCertificateValidationCallback( _
                                AddressOf OnRemoteCertificateValidationCallback)
 
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
 
                    End If
 
                    '' Webページを取得するのは、事実上この1行だけ
                    client.BaseAddress = New System.Uri(_baseUri)
                    query = String.Format("?{0}", Await MakeQuery(_queryParamList))
 
                    ' ConfigureAwaitがないと、
                    ' DLL化したときに動いてくれない。（返ってこない）
                    Dim res As String = Await client.GetStringAsync(query).ConfigureAwait(False)
 
                    result = res
 
                    'いかなる例外もスローする
                Catch ex As Exception
                    Throw ex
                End Try
 
            End Using
 
            Return result
        End Function
 
    End Class
End Namespace