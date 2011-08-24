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

    Sub execTransfer()
        'this.Invoke((MethodInvoker)delegate {
        TextBox3.Text = String.Empty
        'Dim strm As String
        Dim strmArr() As String = GetDumpData()
        PopulateTextBox(strmArr)
        TextBox4.Text = strmArr(1)
        macId.Text = strmArr(2)
        'changeLogEnter("Settings fetched Successfully")
        Dim k As Integer = Array.IndexOf(strmArr, "energy-modbus")
        ipAddrTxt.Text = strmArr(k + 2)
        Dim anaCh1 As String()
        anaCh1 = strmArr(5).Split(",")
        ac1name.Text = anaCh1(0)
        ac1gain.Text = anaCh1(1)
        ac1offset.Text = anaCh1(2)
        ac1tolerance.Text = anaCh1(3)
        Dim anaCh2 As String()
        anaCh2 = strmArr(7).Split(",")
        ac2name.Text = anaCh2(0)
        ac2gain.Text = anaCh2(1)
        ac2offset.Text = anaCh2(2)
        ac2tolerance.Text = anaCh2(3)
        Dim anaCh3 As String()
        anaCh3 = strmArr(9).Split(",")
        ac3name.Text = anaCh3(0)
        ac3gain.Text = anaCh3(1)
        ac3offset.Text = anaCh3(2)
        ac3tolerance.Text = anaCh3(3)
        Dim anaCh4 As String()
        anaCh4 = strmArr(11).Split(",")
        ac4name.Text = anaCh4(0)
        ac4gain.Text = anaCh4(1)
        ac4offset.Text = anaCh4(2)
        ac4tolerance.Text = anaCh4(3)
        Dim dich1 As String()
        dich1 = strmArr(13).Split(",")
        di1name.Text = dich1(2)
        di1ch.Text = dich1(1)
        di1polarity.Text = dich1(3)
        Dim dich2 As String()
        dich2 = strmArr(15).Split(",")
        di2name.Text = dich2(2)
        di2ch.Text = dich2(1)
        di2polarity.Text = dich2(3)
        Dim doch1 As String()
        doch1 = strmArr(17).Split(",")
        do1name.Text = doch1(2)
        do1ch.Text = doch1(1)
        do1polarity.Text = doch1(0)
        Dim doch2 As String()
        doch2 = strmArr(19).Split(",")
        do2name.Text = doch2(2)
        do2ch.Text = doch2(1)
        do2polarity.Text = doch2(0)
        initPorts(strmArr)
    End Sub
    Sub initPorts(ByVal strmArr As String())
        Dim nameBoxes As TextBox()
        nameBoxes = New TextBox() {u0name, u1name, u2name, u3name}
        Dim baudBoxes As ComboBox()
        baudBoxes = New ComboBox() {u0baud, u1baud, u2baud, u3baud}
        Dim parityBoxes As ComboBox()
        parityBoxes = New ComboBox() {u0parity, u1parity, u2parity, u3parity}
        Dim dataBoxes As ComboBox()
        dataBoxes = New ComboBox() {u0data, u1data, u2data, u3data}
        Dim stopBoxes As ComboBox()
        stopBoxes = New ComboBox() {u0stop, u1stop, u2stop, u3stop}
        For i = 0 To 3
            Dim curNameCon As String = "u" & i & "name"
            Dim curUart As String = "uart" & i
            Dim a As Integer = Array.IndexOf(strmArr, curUart)
            nameBoxes(i).Text = strmArr(a + 1)
            baudBoxes(i).Text = strmArr(a + 3)
            parityBoxes(i).Text = strmArr(a + 5)
            dataBoxes(i).Text = strmArr(a + 7)
            stopBoxes(i).Text = strmArr(a + 9)
        Next
        Dim b As Integer = Array.IndexOf(strmArr, "mt-br")
        baudMt.Text = strmArr(b + 1)
        parityMt.Text = strmArr(b + 3)
        dataMt.Text = strmArr(b + 5)
        stopMt.Text = strmArr(b + 7)
        Dim c As Integer = Array.IndexOf(strmArr, "ts")
        Dim tsFlag As String = strmArr(c + 1)
        If tsFlag = "1" Then
            RadioButton1.Checked = True
        ElseIf tsFlag = "0" Then
            RadioButton2.Checked = True
        End If
        Dim k As Integer = Array.IndexOf(strmArr, "energy-modbus")
        Dim str() As String = strmArr(k + 1).Split(";")
        Dim str1() As String = str(2).Split(",")
        ctPrimary.Text = str1(2)
        Dim str2() As String = str(3).Split(",")
        'Dim isStarorDelta As Integer
        Dim tempSD = str2(2).Substring(0, 1)
        If tempSD = 3 Then
            ctTypeDelta.Checked = True
        ElseIf tempSD = 4 Then
            ctTypeStar.Checked = True
        End If
    End Sub

    Function GetCTData(ByVal i As Integer)
        Dim retvalue As String = String.Empty
        Dim strmArr() As String = GetDumpData()
        PopulateTextBox(strmArr)
        Dim k As Integer = Array.IndexOf(strmArr, "energy-modbus")
        Dim str() As String = strmArr(k + 1).Split(";")
        Dim str1() As String = str(2).Split(",")
        ' ctPrimary.Text = str1(2)
        Dim str2() As String = str(3).Split(",")
        'Dim isStarorDelta As Integer
        Dim tempSD = str2(2).Substring(0, 1)
        If i = 1 Then
            If tempSD = 3 Then
                retvalue = "3"
            ElseIf tempSD = 4 Then
                retvalue = "4"
            End If
        End If
        If i = 2 Then
            retvalue = str1(2)
        End If
        Return retvalue
    End Function

End Class