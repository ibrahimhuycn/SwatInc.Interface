Public Class AstmConnectionReceivedDataEventArgs
    Inherits EventArgs

    Public Sub New(ByVal aDataLine As String)
        MyBase.New()
        Me.ReceivedData = aDataLine
    End Sub

    Public Property ReceivedData As String
End Class