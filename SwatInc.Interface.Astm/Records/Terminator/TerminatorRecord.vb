Namespace Records

    Public Class TerminatorRecord
        Inherits AbstractAstmRecord

        Public Sub New(ByVal aLisString As String)
            MyBase.New(aLisString)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        <RecordFieldIndex(2)>
        Public Property SequenceNumber As Integer

        <RecordFieldIndex(3)>
        Public Property TerminationCode As TerminationCode

        Public Overrides Function ToAstmString() As String
            Return String.Concat(String.Concat("L", New String(FieldDelimiter, 1)), MyBase.ToAstmString())
        End Function

    End Class

End Namespace