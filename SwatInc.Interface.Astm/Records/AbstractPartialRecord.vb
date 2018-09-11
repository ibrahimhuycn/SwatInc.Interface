Namespace Records

    Public MustInherit Class AbstractPartialRecord
        Inherits AbstractAstmRecord

        Protected Sub New(ByVal frame As String)
            MyBase.New(frame)
        End Sub

        Protected Sub New()
            MyBase.New()
        End Sub

    End Class

End Namespace