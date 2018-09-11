Namespace Records.PartialRecords

    Public Class UniversalTestID
        Inherits AbstractPartialRecord

        Public Sub New(ByVal aLisString As String)
            MyBase.New(aLisString)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        'TODO: Optional Fields might have to be removed, I could not locate them on CLSI LIS2A2 Standard
        <RecordFieldIndex(4)>
        Public Property ManufacturerCode As String

        <RecordFieldIndex(5)>
        Public Property OptionalField1 As String

        <RecordFieldIndex(6)>
        Public Property OptionalField2 As String

        <RecordFieldIndex(7)>
        Public Property OptionalField3 As String

        <RecordFieldIndex(8)>
        Public Property OptionalField4 As String

        <RecordFieldIndex(1)>
        Public Property TestID As String

        <RecordFieldIndex(2)>
        Public Property TestName As String

        <RecordFieldIndex(3)>
        Public Property TestType As String

    End Class

End Namespace