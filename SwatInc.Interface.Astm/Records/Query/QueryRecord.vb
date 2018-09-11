Imports SwatInc.Interface.Astm.Records.PartialRecords

Namespace Records.Query

    Public Class QueryRecord
        Inherits AbstractAstmRecord

        Public Sub New(ByVal aLisString As String)
            MyBase.New(aLisString)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        <RecordFieldIndex(4)>
        Public Property EndingRange As StartingRange

        <RecordFieldIndex(2)>
        Public Property SequenceNumber As Integer

        <RecordFieldIndex(3)>
        Public Property StartingRange As StartingRange

        <RecordFieldIndex(5)>
        Public Property UniversalTestID As UniversalTestID

        Public Overrides Function ToAstmString() As String
            Return String.Concat(String.Concat("Q", New String(FieldDelimiter, 1)), MyBase.ToAstmString())
        End Function

    End Class

End Namespace