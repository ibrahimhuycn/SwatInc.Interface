Imports SwatInc.Interface.Astm
Imports SwatInc.Interface.Astm.AstmDelimiters

Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim aLisString As String = TextBox1.Text
        Dim result As New Records.HeaderRecord(aLisString)
    End Sub

End Class