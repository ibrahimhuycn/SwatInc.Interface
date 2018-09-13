Imports System.Runtime.Serialization

Namespace Communications

    ''' <summary>
    ''' A custom exception that is uses for all LIS01-A2 related exceptions.
    ''' </summary>
    Public Class Lis01A2ConnectionException
        Inherits AstmConnectionException

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(ByVal message As String, ByVal innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

        Public Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            MyBase.New(info, context)
        End Sub

    End Class

End Namespace