<AttributeUsage(AttributeTargets.Property)>
Public Class AstmDateTimeUsageAttribute
    Inherits Attribute

    Public Sub New(ByVal aDateTimeUsage As AstmDateTimeUsage)
        MyBase.New()
        Me.DateTimeUsage = aDateTimeUsage
    End Sub

    Public Property DateTimeUsage As AstmDateTimeUsage
End Class