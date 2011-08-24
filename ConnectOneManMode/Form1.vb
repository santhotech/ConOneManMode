Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Public Class Form1
    Dim devIp As String
    Dim prt As Integer
    Dim met As New Methods
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim butText As String = Button1.Text
        devIp = cnctIpAddrTxt.Text
        prt = prtTxt.Text
        If butText = "Connect" Then
            Try
                execTransfer()
                Button1.Text = "Disconnect"
                MainPanel.Visible = True
            Catch ex As Exception
                MsgBox("Device is not reachable. Check whether the IP is correct and is in the same network as client.")
            End Try
        ElseIf butText = "Disconnect" Then
            Application.Restart()
        End If
    End Sub

    Sub sndCommand(ByVal cmdToExec As String, ByVal msg As String)
        Dim tc As New TcpClient
        Try
            tc.Connect(devIp, prt)
            Dim outStream As Byte() = System.Text.Encoding.ASCII.GetBytes(cmdToExec)
            tc.GetStream().Write(outStream, 0, outStream.Length)
            System.Threading.Thread.Sleep(2000)
            execTransfer()
            chngLog.Text = chngLog.Text + vbNewLine + msg
            MessageBox.Show(msg)
        Catch ex As Exception
            MessageBox.Show("Cannot access management mode.Please try again")
        End Try
        tc.Close()
    End Sub

    Function GetDumpData()
        Dim tc As New TcpClient
        tc.Connect(devIp, prt)
        Dim strmArr As String()
        Dim dumpCmd As String = "* dump 0;"
        Dim inStream(tc.ReceiveBufferSize) As Byte
        Dim outStream As Byte() = System.Text.Encoding.ASCII.GetBytes(dumpCmd)
        tc.GetStream().Write(outStream, 0, outStream.Length)
        System.Threading.Thread.Sleep(3000)
        tc.GetStream().Read(inStream, 0, CInt(tc.ReceiveBufferSize))
        Dim returndata As String = _
        System.Text.Encoding.ASCII.GetString(inStream)
        tc.Close()
        Dim rawData As String = returndata
        Dim indx As Integer = rawData.IndexOf("connect-one")
        indx = indx - 24
        Dim indy As Integer = rawData.IndexOf("sdCardLockEnable")
        indy = indy + 19
        Dim str As String = rawData.Substring(indx, indy - indx)
        'MessageBox.Show(indx)
        'MessageBox.Show(str)
        strmArr = str.Split("|")
        Return strmArr
    End Function

End Class