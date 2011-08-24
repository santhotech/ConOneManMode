Public Class ThreadSafeUiAccessors
    Public Delegate Sub AccessControlBtnDelegate(ByVal btn As Button, ByVal i As Integer)
    Public Sub AccessControlBtn(ByVal btn As Button, ByVal i As Integer)
        If btn.InvokeRequired Then
            Dim delegate2 As New AccessControlBtnDelegate(AddressOf AccessControlBtn)
            Dim parameters(1) As Object
            parameters(0) = btn
            parameters(1) = i
            'Ask form me to call this sub using the delegate with the parameters.
            btn.Invoke(delegate2, parameters)
        Else
            If i = 1 Then
                btn.Enabled = True
            ElseIf i = 0 Then
                btn.Enabled = False
            ElseIf i = 2 Then
                btn.BackColor = Color.Lime
            End If
        End If
    End Sub
    Public Delegate Sub AccessControlTxtDelegate(ByVal txt As Control, ByVal str As String)
    Public Sub AccessControlTxt(ByVal txt As Control, ByVal str As String)
        If txt.InvokeRequired Then
            Dim delegate3 As New AccessControlTxtDelegate(AddressOf AccessControlTxt)
            Dim parameters(1) As Object
            parameters(0) = txt
            parameters(1) = str
            'Ask form me to call this sub using the delegate with the parameters.
            txt.Invoke(delegate3, parameters)
        Else
            If TypeOf (txt) Is TextBox Then
                txt.Text = txt.Text + vbNewLine + str
            Else
                txt.Text = str
            End If
        End If
    End Sub
    Public Delegate Sub AccessControlRadioDelegate(ByVal rdo As RadioButton)
    Public Sub AccessControlRadio(ByVal rdo As RadioButton)
        If rdo.InvokeRequired Then
            Dim delegate4 As New AccessControlRadioDelegate(AddressOf AccessControlRadio)
            Dim parameters(0) As Object
            parameters(0) = rdo
            'Ask form me to call this sub using the delegate with the parameters.
            rdo.Invoke(delegate4, parameters)
        Else
            rdo.Checked = True
        End If
    End Sub
End Class
