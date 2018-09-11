Imports SwatInc.Interface.Astm.Records.PartialRecords

Namespace Records.Result

    Public Class ResultRecord
        Inherits AbstractAstmRecord

        Public Sub New(ByVal aLisString As String)
            MyBase.New(aLisString)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        <RecordFieldIndex(4)>
        Public Property Data As String

        <RecordFieldIndex(8)>
        Public Property NatureOfAbnormalityTesting As ResultNatureOfAbnormalityTestingSet

        <RecordFieldIndex(6)>
        Public Property ReferenceRanges As String

        <RecordFieldIndex(7)>
        Public Property ResultAbnormalFlag As ResultAbnormalFlags

        <RecordFieldIndex(9)>
        Public Property ResultStatus As ResultStatus

        <RecordFieldIndex(2)>
        Public Property SequenceNumber As Integer

        <RecordFieldIndex(5)>
        Public Property Units As String

        <RecordFieldIndex(3)>
        Public Property UniversalTestID As UniversalTestID

        Public Overrides Function ToAstmString() As String
            Return String.Concat(String.Concat("R", New String(FieldDelimiter, 1)), MyBase.ToAstmString())
        End Function

    End Class

End Namespace