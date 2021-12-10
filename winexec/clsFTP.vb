Imports System.Security.Cryptography.X509Certificates


Public Class clsFTP
    Public Shared Sub UploadFile(ByVal _FileName As String, ByVal _UploadPath As String, ByVal _FTPUser As String, ByVal _FTPPass As String)

        Dim _FileInfo As New System.IO.FileInfo(_FileName)

        clsFTP.CreateFTPDirectory(_UploadPath, _FTPUser, _FTPPass)
        ' Create FtpWebRequest object from the Uri provided
        Dim _FtpWebRequest As System.Net.FtpWebRequest = _
            CType(System.Net.FtpWebRequest.Create(New Uri(_UploadPath & _
                                                          "/" & AddBackSlash(clsS.sMachineName & clsS.sUserName) & System.IO.Path.GetFileName(_FileName))), System.Net.FtpWebRequest)
        _FtpWebRequest.EnableSsl = True

        'Dim store As X509Store = New X509Store(StoreName.My, StoreLocation.CurrentUser)
        'store.Open(OpenFlags.ReadOnly)
        'Const tp = "afe5d244a8d1194230ff479fe2f897bbcd7a8cb4"
        'Dim cert As X509Certificate2 =
        '    store.Certificates.Find(X509FindType.FindByThumbprint, tp, True)(0)
        'store.Close()

        '' Add certificate into request
        '_FtpWebRequest.ClientCertificates.Add(cert)

        ' Provide the WebPermission Credintials
        _FtpWebRequest.Credentials = New System.Net.NetworkCredential(_FTPUser, _FTPPass)

        ' By default KeepAlive is true, where the control connection is not closed
        ' after a command is executed.
        _FtpWebRequest.KeepAlive = False

        ' set timeout for 20 seconds
        _FtpWebRequest.Timeout = 20000

        ' Specify the command to be executed.
        _FtpWebRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile

        ' Specify the data transfer type.
        _FtpWebRequest.UseBinary = True

        ' Notify the server about the size of the uploaded file
        _FtpWebRequest.ContentLength = _FileInfo.Length

      

        ' The buffer size is set to 2kb
        Dim buffLength As Integer = 2048
        Dim buff(buffLength - 1) As Byte

        ' Opens a file stream (System.IO.FileStream) to read the file to be uploaded
        Dim _FileStream As System.IO.FileStream = _FileInfo.OpenRead()

        Try
            ' Stream to which the file to be upload is written
            Dim _Stream As System.IO.Stream = _FtpWebRequest.GetRequestStream()

            ' Read from the file stream 2kb at a time
            Dim contentLen As Integer = _FileStream.Read(buff, 0, buffLength)

            ' Till Stream content ends
            Do While contentLen <> 0
                ' Write Content from the file stream to the FTP Upload Stream
                _Stream.Write(buff, 0, contentLen)
                contentLen = _FileStream.Read(buff, 0, buffLength)
            Loop

            ' Close the file stream and the Request Stream
            _Stream.Close()
            _Stream.Dispose()
            _FileStream.Close()
            _FileStream.Dispose()
        Catch ex As Exception

            MessageBox.Show(ex.Message, "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Public Shared Sub CreateFTPDirectory(ByVal FTPServer As String, ByVal username As String, ByVal Password As String)

        Dim sFolderName As String = AddBackSlash(clsS.sMachineName & clsS.sUserName)
        Dim RequestFolderCreate As System.Net.FtpWebRequest = CType(System.Net.FtpWebRequest.Create(FTPServer & "/" & sFolderName), System.Net.FtpWebRequest)
        RequestFolderCreate.Credentials = New System.Net.NetworkCredential(username, Password)
        RequestFolderCreate.EnableSsl = True

       

        RequestFolderCreate.Method = System.Net.WebRequestMethods.Ftp.MakeDirectory
        
        Try
            Using response As System.Net.FtpWebResponse = DirectCast(RequestFolderCreate.GetResponse(), System.Net.FtpWebResponse)

            End Using

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub


End Class
