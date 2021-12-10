Imports System.Net
Imports System.Text.RegularExpressions
Public Structure HostGeoInfo
    Dim IP As String
    Dim ISP As String
    Dim Host As String
    Dim CountryCode As String
    Dim Country As String
    Dim City As String
    Dim Latitude As String
    Dim Longitude As String
End Structure
Class clsGeoIP

    Shared Function GetExternalIp() As String
        Try
            Dim ExternalIP As String
            ExternalIP = (New WebClient()).DownloadString("http://checkip.dyndns.org/")

            ExternalIP = (New Regex("\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")) _
                         .Matches(ExternalIP)(0).ToString()
            Return ExternalIP
        Catch
            Return Nothing
        End Try
    End Function

    Shared Function GetHostInfo(ByVal IP_Address As String) As HostGeoInfo
        Try
            Dim IP_Hostname = "http://api.geoiplookup.net/?query=" & IP_Address
            Dim GeoIPResults As String = New WebClient().DownloadString(IP_Hostname)
            Dim HostInfo As New HostGeoInfo

            With HostInfo
                .IP = SplitArgument(GeoIPResults, "><ip>", "</ip><host>")
                .CountryCode = SplitArgument(GeoIPResults, "<countrycode>", "</countrycode>")
            End With
            Return HostInfo
        Catch ex As Exception
            Debug.Print(Err.Description)
        End Try
        Return Nothing
    End Function
    Private Shared Function SplitArgument(ByVal sArgument As String, ByVal SplitterBegin As String, ByVal SplitterEnd As String) As String
        Dim ParseReturn As String = String.Empty
        ParseReturn = Split(sArgument, SplitterBegin)(1).Split(SplitterEnd)(0)
        ParseReturn = Replace(ParseReturn, vbTab, "")
        Return ParseReturn
    End Function

End Class