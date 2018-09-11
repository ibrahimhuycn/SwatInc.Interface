<AttributeUsage(AttributeTargets.Field)>
Friend Class AstmEnumAttribute
    Inherits Attribute

    Public Sub New(ByVal id As String)
        MyBase.New()
        Me.Id = id
    End Sub

    Public Property Id As String
End Class