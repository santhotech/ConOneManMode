Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Public Class Form1
    Dim devIp As String
    Dim prt As Integer
    Dim met As New Methods
    Dim tsa As New ThreadSafeUiAccessors

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
        met.PopulateTextBox(strmArr, TextBox3)
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

        Dim subNetIndex As Integer = Array.IndexOf(strmArr, "netmask")
        subNetTxt.Text = strmArr(subNetIndex + 1)




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
        met.PopulateTextBox(strmArr, TextBox3)
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




    Private Sub macIdChng_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles macIdChng.Click
        Dim newMacId As String = macId.Text
        Dim newCmd As String = "* net 0 mac=" + newMacId + ";"
        sndCommand(newCmd, "Mac Id Changed Successfully")
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim newac1Name As String = ac1name.Text
        Dim newCmd As String = "* ai 0 name=" + newac1Name + ";"
        sndCommand(newCmd, "ac0 name Changed Successfully")
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim newac1Gain As String = ac1gain.Text
        Dim newCmd As String = "* ai 0 gain=" + newac1Gain + ";"
        sndCommand(newCmd, "ac0 gain Changed Successfully")
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Dim newac1Offset As String = ac1offset.Text
        Dim newCmd As String = "* ai 0 offset=" + newac1Offset + ";"
        sndCommand(newCmd, "ac0 Offset Changed Successfully")
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Dim newac1Tole As String = ac1tolerance.Text
        Dim newCmd As String = "* ai 0 tolerance=" + newac1Tole + ";"
        sndCommand(newCmd, "ac0 tolerance Changed Successfully")
    End Sub

    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        Dim newac2Name As String = ac2name.Text
        Dim newCmd As String = "* ai 1 name=" + newac2Name + ";"
        sndCommand(newCmd, "ac1 name Changed Successfully")
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        Dim newac2Gain As String = ac2gain.Text
        Dim newCmd As String = "* ai 1 gain=" + newac2Gain + ";"
        sndCommand(newCmd, "ac1 gain Changed Successfully")
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        Dim newac2Offset As String = ac2offset.Text
        Dim newCmd As String = "* ai 1 offset=" + newac2Offset + ";"
        sndCommand(newCmd, "ac1 Offset Changed Successfully")
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Dim newac2Tole As String = ac2tolerance.Text
        Dim newCmd As String = "* ai 1 tolerance=" + newac2Tole + ";"
        sndCommand(newCmd, "ac1 tolerance Changed Successfully")
    End Sub

    Private Sub Button18_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button18.Click
        Dim newac3Name As String = ac3name.Text
        Dim newCmd As String = "* ai 2 name=" + newac3Name + ";"
        sndCommand(newCmd, "ac2 name Changed Successfully")
    End Sub

    Private Sub Button17_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button17.Click
        Dim newac3Gain As String = ac3gain.Text
        Dim newCmd As String = "* ai 2 gain=" + newac3Gain + ";"
        sndCommand(newCmd, "ac2 gain Changed Successfully")
    End Sub

    Private Sub Button16_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button16.Click
        Dim newac3Offset As String = ac3offset.Text
        Dim newCmd As String = "* ai 2 offset=" + newac3Offset + ";"
        sndCommand(newCmd, "ac2 Offset Changed Successfully")
    End Sub

    Private Sub Button15_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button15.Click
        Dim newac3Tole As String = ac3tolerance.Text
        Dim newCmd As String = "* ai 2 tolerance=" + newac3Tole + ";"
        sndCommand(newCmd, "ac2 tolerance Changed Successfully")
    End Sub

    Private Sub Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button14.Click
        Dim newac4Name As String = ac4name.Text
        Dim newCmd As String = "* ai 3 name=" + newac4Name + ";"
        sndCommand(newCmd, "ac3 name Changed Successfully")
    End Sub

    Private Sub Button13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button13.Click
        Dim newac4Gain As String = ac4gain.Text
        Dim newCmd As String = "* ai 3 gain=" + newac4Gain + ";"
        sndCommand(newCmd, "ac3 gain Changed Successfully")
    End Sub

    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        Dim newac4Offset As String = ac4offset.Text
        Dim newCmd As String = "* ai 3 offset=" + newac4Offset + ";"
        sndCommand(newCmd, "ac3 Offset Changed Successfully")
    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        Dim newac4Tole As String = ac4tolerance.Text
        Dim newCmd As String = "* ai 3 tolerance=" + newac4Tole + ";"
        sndCommand(newCmd, "ac3 tolerance Changed Successfully")
    End Sub

    Private Sub Button34_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button34.Click
        Dim newdi1name As String = di1name.Text
        Dim newCmd As String = "* di 0 name=" + newdi1name + ";"
        sndCommand(newCmd, "di0 name Changed Successfully")
    End Sub

    Private Sub Button23_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button23.Click
        Dim newdi2name As String = di2name.Text
        Dim newCmd As String = "* di 1 name=" + newdi2name + ";"
        sndCommand(newCmd, "di1 name Changed Successfully")
    End Sub

    Private Sub Button20_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button20.Click
        Dim newdo1name As String = do1name.Text
        Dim newCmd As String = "* do 0 name=" + newdo1name + ";"
        sndCommand(newCmd, "do0 name Changed Successfully")
    End Sub

    Private Sub Button19_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button19.Click
        Dim newdo2name As String = do2name.Text
        Dim newCmd As String = "* do 1 name=" + newdo2name + ";"
        sndCommand(newCmd, "do1 name Changed Successfully")
    End Sub

    Private Sub Button22_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button22.Click
        Dim newParam As String = u0name.Text
        Dim newCmd As String = "* uart 0 dev=" + newParam + ";"
        sndCommand(newCmd, "uart0 dev Changed Successfully")
    End Sub

    Private Sub Button24_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button24.Click
        Dim newParam As String = u0baud.Text
        Dim newCmd As String = "* uart 0 baud=" + newParam + ";"
        sndCommand(newCmd, "uart0 baud rate Changed Successfully")
    End Sub

    Private Sub Button25_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button25.Click
        Dim newParam As String = u0parity.Text
        Dim newCmd As String = "* uart 0 parity=" + newParam + ";"
        sndCommand(newCmd, "uart0 parity Changed Successfully")
    End Sub

    Private Sub Button26_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button26.Click
        Dim newParam As String = u0data.Text
        Dim newCmd As String = "* uart 0 data=" + newParam + ";"
        sndCommand(newCmd, "uart0 data Changed Successfully")
    End Sub

    Private Sub Button27_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button27.Click
        Dim newParam As String = u0stop.Text
        Dim newCmd As String = "* uart 0 stop=" + newParam + ";"
        sndCommand(newCmd, "uart0 stop Changed Successfully")
    End Sub

    Private Sub Button32_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button32.Click
        Dim newParam As String = u1name.Text
        Dim newCmd As String = "* uart 1 dev=" + newParam + ";"
        sndCommand(newCmd, "uart1 dev Changed Successfully")
    End Sub

    Private Sub Button31_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button31.Click
        Dim newParam As String = u1baud.Text
        Dim newCmd As String = "* uart 1 baud=" + newParam + ";"
        sndCommand(newCmd, "uart1 baud rate Changed Successfully")
    End Sub

    Private Sub Button30_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button30.Click
        Dim newParam As String = u1parity.Text
        Dim newCmd As String = "* uart 1 parity=" + newParam + ";"
        sndCommand(newCmd, "uart1 parity Changed Successfully")
    End Sub

    Private Sub Button29_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button29.Click
        Dim newParam As String = u1data.Text
        Dim newCmd As String = "* uart 1 data=" + newParam + ";"
        sndCommand(newCmd, "uart1 data Changed Successfully")
    End Sub

    Private Sub Button28_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button28.Click
        Dim newParam As String = u1stop.Text
        Dim newCmd As String = "* uart 1 stop=" + newParam + ";"
        sndCommand(newCmd, "uart1 stop Changed Successfully")
    End Sub

    Private Sub Button39_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button39.Click
        Dim newParam As String = u2name.Text
        Dim newCmd As String = "* uart 2 dev=" + newParam + ";"
        sndCommand(newCmd, "uart2 dev Changed Successfully")
    End Sub

    Private Sub Button38_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button38.Click
        Dim newParam As String = u2baud.Text
        Dim newCmd As String = "* uart 2 baud=" + newParam + ";"
        sndCommand(newCmd, "uart2 baud rate Changed Successfully")
    End Sub

    Private Sub Button37_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button37.Click
        Dim newParam As String = u2parity.Text
        Dim newCmd As String = "* uart 2 parity=" + newParam + ";"
        sndCommand(newCmd, "uart2 parity Changed Successfully")
    End Sub

    Private Sub Button36_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button36.Click
        Dim newParam As String = u2data.Text
        Dim newCmd As String = "* uart 2 data=" + newParam + ";"
        sndCommand(newCmd, "uart2 data Changed Successfully")
    End Sub

    Private Sub Button33_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button33.Click
        Dim newParam As String = u2stop.Text
        Dim newCmd As String = "* uart 2 stop=" + newParam + ";"
        sndCommand(newCmd, "uart2 stop Changed Successfully")
    End Sub

    Private Sub Button44_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button44.Click
        Dim newParam As String = u3name.Text
        Dim newCmd As String = "* uart 3 dev=" + newParam + ";"
        sndCommand(newCmd, "uart3 dev Changed Successfully")
    End Sub

    Private Sub Button43_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button43.Click
        Dim newParam As String = u3baud.Text
        Dim newCmd As String = "* uart 3 baud=" + newParam + ";"
        sndCommand(newCmd, "uart3 baud rate Changed Successfully")
    End Sub

    Private Sub Button42_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button42.Click
        Dim newParam As String = u3parity.Text
        Dim newCmd As String = "* uart 3 parity=" + newParam + ";"
        sndCommand(newCmd, "uart3 parity Changed Successfully")
    End Sub

    Private Sub Button41_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button41.Click
        Dim newParam As String = u3data.Text
        Dim newCmd As String = "* uart 3 data=" + newParam + ";"
        sndCommand(newCmd, "uart3 data Changed Successfully")
    End Sub

    Private Sub Button40_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button40.Click
        Dim newParam As String = u3stop.Text
        Dim newCmd As String = "* uart 3 stop=" + newParam + ";"
        sndCommand(newCmd, "uart3 stop Changed Successfully")
    End Sub

    Private Sub Button47_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button47.Click
        Dim newParam As String = baudMt.Text
        Dim newCmd As String = "* umt 0 baud=" + newParam + ";"
        sndCommand(newCmd, "MT baud rate Changed Successfully")
    End Sub

    Private Sub Button46_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button46.Click
        Dim newParam As String = parityMt.Text
        Dim newCmd As String = "* umt 0 parity" + newParam + ";"
        sndCommand(newCmd, "MT parity Changed Successfully")
    End Sub

    Private Sub Button45_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button45.Click
        Dim newParam As String = dataMt.Text
        Dim newCmd As String = "* umt 0 data" + newParam + ";"
        sndCommand(newCmd, "MT data Changed Successfully")
    End Sub

    Private Sub Button21_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button21.Click
        Dim newParam As String = stopMt.Text
        Dim newCmd As String = "* umt 0 stop" + newParam + ";"
        sndCommand(newCmd, "MT stop Changed Successfully")
    End Sub

    Private Sub Button51_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button51.Click
        Dim newCmd As String = "* time 0 " + yrtxt.Text + "/" + mntxt.Text + "/" + dttxt.Text + "T" + hrtxt.Text + ":" + mitxt.Text + ":" + sctxt.Text + ";"
        sndCommand(newCmd, "Device time stamp Changed Successfully")
    End Sub

    Private Sub Button48_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button48.Click
        If RadioButton1.Checked = True Then
            Dim newCmd As String = "* net 0 ts=1;"
            sndCommand(newCmd, "Device Timestamp Enabled")
        End If
        If RadioButton2.Checked = True Then
            Dim newCmd As String = "* net 0 ts=0;"
            sndCommand(newCmd, "Device Timestamp Disabled")
        End If
    End Sub

    Private Sub Button50_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button50.Click
        Dim newCmd As String = "* sdread 0 " + TextBox16.Text + "-" + TextBox15.Text + "-" + TextBox14.Text + ".txt;"
        sndCommand(newCmd, "File retrieve request sent to device")
    End Sub

    Private Sub Button49_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button49.Click
        Dim newCmd As String = "* sdremf 0 " + TextBox13.Text + "-" + TextBox12.Text + "-" + TextBox11.Text + ".txt;"
        sndCommand(newCmd, "File remove request sent to device")
    End Sub


    Private Sub Button52_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button52.Click
        Dim newParam As String = ctPrimary.Text
        Dim newCmd As String = "* energy 0 hreg_ct_primary_value=" + newParam + ";"
        sndCommand2(newCmd, "CT primary Changed successfully", Button52, newParam)
    End Sub

    Sub sndCommand1(ByVal cmdToExec As String, ByVal msg As String, ByVal btn As Button)
        Dim tc As New TcpClient
        tc.Connect(devIp, prt)
        Try
            Dim outStream As Byte() = System.Text.Encoding.ASCII.GetBytes(cmdToExec)
            tc.GetStream().Write(outStream, 0, outStream.Length)
            System.Threading.Thread.Sleep(2000)
            Dim rest As String = "* reset 0;"
            Dim kStream As Byte() = System.Text.Encoding.ASCII.GetBytes(rest)
            tc.GetStream().Write(kStream, 0, kStream.Length)
            System.Threading.Thread.Sleep(25000)
            execTransfer()
            tsa.AccessControlTxt(chngLog, msg)
            MessageBox.Show(msg)
            tsa.AccessControlBtn(btn, 1)
        Catch ex As Exception
            MessageBox.Show("Cannot access management mode.Please try again")
        End Try
    End Sub

    Sub sndCommand2(ByVal cmdToExec As String, ByVal msg As String, ByVal btn As Button, ByVal prm As String)
        Dim t As Thread
        Dim parameter(4) As Object
        parameter(0) = cmdToExec
        parameter(1) = msg
        parameter(2) = btn
        parameter(3) = prm
        met.DisableAll(Me)
        btn.Text = "wait"
        btn.Enabled = False

        t = New Thread(AddressOf ExecThread)
        t.IsBackground = True
        t.Start(parameter)



    End Sub

    Sub ExecThread(ByVal state As Object)
        Dim cmdToExec As String = state(0)
        Dim msg As String = state(1)
        Dim btn As Button = state(2)
        Dim prm As String = state(3)
        Dim tc As New TcpClient
        tc.Connect(devIp, prt)
        Try
            Dim outStream As Byte() = System.Text.Encoding.ASCII.GetBytes(cmdToExec)
            tc.GetStream().Write(outStream, 0, outStream.Length)
            System.Threading.Thread.Sleep(2000)
            Dim rest As String = "* reset 0;"
            Dim kStream As Byte() = System.Text.Encoding.ASCII.GetBytes(rest)
            tc.GetStream().Write(kStream, 0, kStream.Length)
            System.Threading.Thread.Sleep(25000)
            tsa.AccessControlTxt(chngLog, msg)
            Dim s As String
            If Equals(Button52, btn) Then
                s = GetCTData(2)
                tsa.AccessControlTxt(ctPrimary, s)
            Else
                s = GetCTData(1)
                changeRadio(s)
            End If
            Dim newPrm As String = s
            tsa.AccessControlTxt(btn, "GO")
            met.EnableAll(Me)
            tsa.AccessControlBtn(btn, 1)
            If newPrm = prm Then
                MessageBox.Show("CT Configuration successful")
            Else
                MessageBox.Show("CT Configuration Failed")
            End If
        Catch ex As Exception
            MessageBox.Show("Cannot access management mode.Please try again")
        End Try
    End Sub
    Sub ChangeRadio(ByVal s As String)
        Dim i As Integer = Int32.Parse(s)
        If i = 3 Then
            tsa.AccessControlRadio(ctTypeDelta)
        ElseIf i = 4 Then
            tsa.AccessControlRadio(ctTypeStar)
        End If
    End Sub

    Private Sub Button53_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button53.Click
        If ctTypeDelta.Checked = True Then
            Dim newCmd As String = "* energy 0 star_delta_value=3;"
            sndCommand2(newCmd, "Setting CT Mode to delta", Button53, "3")
        End If
        If ctTypeStar.Checked = True Then
            Dim newCmd As String = "* energy 0 star_delta_value=4;"
            sndCommand2(newCmd, "Setting CT Mode to star", Button53, "4")
        End If
    End Sub

    Private Sub Button54_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button54.Click
        Dim newIp As String = ipAddrTxt.Text
        sndCommand("* net 0 ip=" + newIp + ";", "Ip changed successfully")
        sndCommand("* reset 0;", "Restarting...")
        devIp = newIp
        Application.Restart()
    End Sub

    Private Sub Button55_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button55.Click
        Dim newParam As String = subNetTxt.Text
        Dim newCmd As String = "* net 0 subnet=" + newParam + ";"
        sndCommand(newCmd, "Subnet Changed Successfully")
    End Sub
End Class