Public Interface IAstmConnection
    Inherits IDisposable

    Event OnAstmConnectionClosed As EventHandler

    Event OnReceiveString As AstmConnectionReceivedDataEventHandler

    Event OnReceiveTimeOut As EventHandler

    Property Status As AstmConnectionStatus

    Sub Connect()

    Sub DisConnect()

    Function EstablishSendMode() As Boolean

    Sub SendMessage(ByVal aMessage As String)

    Sub StartReceiveTimeoutTimer()

    Sub StopSendMode()

End Interface