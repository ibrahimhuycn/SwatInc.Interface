Public Module AstmDelimiters
    Public ComponentDelimiter As Char
    Public EscapeCharacter As Char
    Public FieldDelimiter As Char
    Public RepeatDelimiter As Char

    Sub New()
        FieldDelimiter = ChrW(124)
        RepeatDelimiter = ChrW(92)
        ComponentDelimiter = ChrW(94)
        EscapeCharacter = ChrW(38)
    End Sub

    Public Function AddFieldDelimiters(ByVal numberOfDelimiters As Integer) As String
        Return New String(FieldDelimiter, numberOfDelimiters)
    End Function

End Module