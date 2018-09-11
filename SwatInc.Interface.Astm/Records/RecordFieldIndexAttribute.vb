<AttributeUsage(AttributeTargets.Property)>
Friend Class RecordFieldIndexAttribute
    Inherits Attribute

    Public Sub New(ByVal fieldIndex As Integer)
        MyBase.New()
        Me.FieldIndex = fieldIndex
    End Sub

    Public Property FieldIndex As Integer
End Class