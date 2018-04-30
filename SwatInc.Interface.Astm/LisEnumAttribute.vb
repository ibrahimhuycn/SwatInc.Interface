<AttributeUsage(AttributeTargets.Field)>
Friend Class LisEnumAttribute
    Inherits Attribute
    Public Property LisId As String

    Public Sub New(ByVal aLisId As String)
        MyBase.New()
        LisId = aLisId
    End Sub

End Class