Namespace Records.Patient

    Public Class PatientRecord
        Inherits AbstractAstmRecord

        Public Sub New(ByVal aLisString As String)
            MyBase.New(aLisString)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        <RecordFieldIndex(14)>
        Public Property AttendingPhysicianID As String

        <AstmDateTimeUsage(AstmDateTimeUsage.Date)>
        <RecordFieldIndex(8)>
        Public Property Birthdate As Nullable(Of DateTime)

        <RecordFieldIndex(4)>
        Public Property LaboratoryAssignedPatientID As String

        <RecordFieldIndex(5)>
        Public Property PatientID3 As String

        <RecordFieldIndex(6)>
        Public Property PatientName As PatientName

        <RecordFieldIndex(9)>
        Public Property PatientSex As Nullable(Of PatientSex)

        <RecordFieldIndex(3)>
        Public Property PracticeAssignedPatientID As String

        <RecordFieldIndex(2)>
        Public Property SequenceNumber As Integer

        Public Overrides Function ToAstmString() As String
            Return String.Concat(String.Concat("P", New String(FieldDelimiter, 1)), MyBase.ToAstmString())
        End Function

    End Class

End Namespace