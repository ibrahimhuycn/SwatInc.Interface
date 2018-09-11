Namespace Records.Query

    Public Class StartingRange
        Inherits AbstractPartialRecord

        Public Sub New(ByVal aLisString As String)
            MyBase.New(aLisString)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        <RecordFieldIndex(1)>
        Public Property PatientID As String

        <RecordFieldIndex(3)>
        Public Property Reserved As String

        <RecordFieldIndex(2)>
        Public Property SpecimenID As String

    End Class

End Namespace