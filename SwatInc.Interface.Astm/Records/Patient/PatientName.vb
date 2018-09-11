Namespace Records.Patient

    Public Class PatientName
        Inherits AbstractPartialRecord

        Public Sub New(ByVal aLisString As String)
            MyBase.New(aLisString)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        <RecordFieldIndex(2)>
        Public Property FirstName As String

        <RecordFieldIndex(1)>
        Public Property LastName As String

        <RecordFieldIndex(3)>
        Public Property MiddleName As String

        <RecordFieldIndex(4)>
        Public Property Suffix As String

        <RecordFieldIndex(5)>
        Public Property Title As String

    End Class

End Namespace