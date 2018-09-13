Namespace Records.Header

    Public Class HeaderRecord
        Inherits AbstractAstmRecord

        Public Sub New(ByVal aLisString As String)
            MyBase.New(aLisString)
            Me.Version = "LIS2-A2"
            Me.MessageDateTime = DateTime.Now
            FieldDelimiter = aLisString(1)
            RepeatDelimiter = aLisString(2)
            ComponentDelimiter = aLisString(3)
            EscapeCharacter = aLisString(4)
        End Sub

        Public Sub New()
            MyBase.New()
            Me.Version = "LIS2-A2"
            Me.MessageDateTime = DateTime.Now
        End Sub

        <RecordFieldIndex(4)>
        Public Property AccessPassword As String

        <RecordFieldIndex(9)>
        Public Property CharacteristicsOfSender As String

        <RecordFieldIndex(11)>
        Public Property Comment As String

        <RecordFieldIndex(3)>
        Public Property MessageControlID As String

        <RecordFieldIndex(14)>
        Public Property MessageDateTime As DateTime

        <RecordFieldIndex(12)>
        Public Property ProcessingID As HeaderProcessingID

        <RecordFieldIndex(10)>
        Public Property ReceiverID As String

        <RecordFieldIndex(5)>
        Public Property SenderID As String

        <RecordFieldIndex(6)>
        Public Property SenderStreetAddress As String

        <RecordFieldIndex(8)>
        Public Property SenderTelephoneNumber As String

        <RecordFieldIndex(13)>
        Public Property Version As String

        Public Overrides Function ToAstmString() As String
            Return String.Concat(String.Concat(String.Concat(String.Concat(String.Concat(String.Concat("H", New String(FieldDelimiter, 1)), New String(RepeatDelimiter, 1)), New String(ComponentDelimiter, 1)), New String(EscapeCharacter, 1)), New String(FieldDelimiter, 1)), MyBase.ToAstmString())
        End Function

    End Class

End Namespace