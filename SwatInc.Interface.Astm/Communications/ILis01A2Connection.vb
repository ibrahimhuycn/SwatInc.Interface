Public Interface ILis01A2Connection

    Event OnReceiveString As AstmConnectionReceivedDataEventHandler

    Sub ClearBuffers()

    Sub Connect()

    Sub DisConnect()

    Sub WriteData(ByVal value As String)

End Interface