Imports SwatInc.Interface.Astm

Public Class Test
    Inherits AstmParser
    Implements IDisposable

    Sub New(aConnection As IAstmConnection)
        MyBase.New(aConnection)

    End Sub

    Sub Dispose() Implements IDisposable.Dispose
    End Sub

    Public Sub Received(ByVal Sender As Object, ByVal e As ReceiveRecordEventArgs) Handles Me.OnReceivedRecord
        MsgBox("booyyaaa")
    End Sub

End Class