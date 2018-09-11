Public Class RecordFields
    Private fieldItems As String()

    Public Sub New(ByVal lisString As String, ByVal SeporatorChar As Char, ByVal NumberOfFields As Integer)
        MyBase.New()
        Me.Value = lisString
        Me.fieldItems = lisString.Split(New Char() {SeporatorChar}, NumberOfFields)
    End Sub

    Public Property Value As String

    Public Function GetField(ByVal indx As Integer) As String
        If (Me.fieldItems.Length < indx) Then
            Return String.Empty
        End If
        Dim Result As String = Me.fieldItems(indx - 1)
        Result = Result.Replace(String.Concat(String.Concat(New String(EscapeCharacter, 1), "F"), New String(EscapeCharacter, 1)), New String(FieldDelimiter, 1))
        Result = Result.Replace(String.Concat(String.Concat(New String(EscapeCharacter, 1), "S"), New String(EscapeCharacter, 1)), New String(ComponentDelimiter, 1))
        Result = Result.Replace(String.Concat(String.Concat(New String(EscapeCharacter, 1), "R"), New String(EscapeCharacter, 1)), New String(RepeatDelimiter, 1))
        Return Result.Replace(String.Concat(String.Concat(New String(EscapeCharacter, 1), "E"), New String(EscapeCharacter, 1)), New String(EscapeCharacter, 1))
    End Function

End Class