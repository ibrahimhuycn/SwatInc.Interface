Imports System.Runtime.Serialization

Namespace Essy.LIS.LIS02A2

    Public Class AstmParserEstablishmentFailedException
        Inherits AstmParserException

        Public Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            MyBase.New(info, context)
        End Sub

        Public Sub New(ByVal message As String, ByVal innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

    End Class

End Namespace