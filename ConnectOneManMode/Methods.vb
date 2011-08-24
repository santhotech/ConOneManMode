Public Class Methods
    Dim tsa As New ThreadSafeUiAccessors
    Public Shared Function FindControlRecursive(ByVal list As List(Of Control), ByVal parent As Control, ByVal ctrlType As System.Type) As List(Of Control)
        If parent Is Nothing Then Return list
        If parent.GetType Is ctrlType Then
            list.Add(parent)
        End If
        For Each child As Control In parent.Controls
            FindControlRecursive(list, child, ctrlType)
        Next
        Return list
    End Function

    Sub clearAll(ByVal frm As Form)
        Dim allTxt As New List(Of Control)
        For Each txt As TextBox In FindControlRecursive(allTxt, frm, GetType(TextBox))
            txt.Text = ""
        Next
    End Sub

    Sub DisableAll(ByVal frm As Form)
        Dim allTxt As New List(Of Control)
        For Each btn As Button In FindControlRecursive(allTxt, frm, GetType(Button))
            tsa.AccessControlBtn(btn, 0)
        Next
    End Sub

    Sub EnableAll(ByVal frm As Form)
        Dim allTxt As New List(Of Control)
        For Each btn As Button In FindControlRecursive(allTxt, frm, GetType(Button))
            tsa.AccessControlBtn(btn, 1)
        Next
    End Sub

    Sub PopulateTextBox(ByVal strmArr() As String, ByVal txtBox As TextBox)
        Dim len As Integer = strmArr.Length
        For i = 0 To len - 1
            tsa.AccessControlTxt(txtBox, strmArr(i))
        Next
    End Sub

End Class
