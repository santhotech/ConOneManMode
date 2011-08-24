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
End Class