Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Public Class Form1
    Dim devIp As String
    Dim prt As Integer
    Dim met As New Methods
    Private Sub Button55_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button55.Click
        met.clearAll(Me)
    End Sub
End Class