Public Class Methods
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

End Class
