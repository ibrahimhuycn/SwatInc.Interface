Imports SwatInc.Interface.Astm.Records.PartialRecords

Namespace Records.Order

    Public Class OrderRecord
        Inherits AbstractAstmRecord

        Public Sub New(ByVal aLisString As String)
            MyBase.New(aLisString)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        <RecordFieldIndex(12)>
        Public Property ActionCode As OrderActionCode

        <RecordFieldIndex(6)>
        Public Property Priority As OrderPriority

        <RecordFieldIndex(26)>
        Public Property ReportType As OrderReportType

        <RecordFieldIndex(2)>
        Public Property SequenceNumber As Integer

        <RecordFieldIndex(3)>
        Public Property SpecimenID As String

        <RecordFieldIndex(5)>
        Public Property TestID As UniversalTestID

        Public Overrides Function ToAstmString() As String
            Return String.Concat(String.Concat("O", New String(FieldDelimiter, 1)), MyBase.ToAstmString())
        End Function

    End Class

End Namespace