Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class Lis01A02TCPConnection
    Implements ILis01A2Connection
    Public Task
    Private e_OnReceiveString As AstmConnectionReceivedDataEventHandler
    Private fClient As TcpClient
    Private fIsInServerMode As Boolean = False
    Private fSocket As Socket

    Public Sub New(ByVal aNetWorkAddress As String, ByVal aNetWorkPort As UShort)
        Me.NetWorkAddress = aNetWorkAddress
        Me.NetWorkPort = aNetWorkPort
    End Sub

    Public Custom Event OnReceiveString As AstmConnectionReceivedDataEventHandler Implements ILis01A2Connection.OnReceiveString
        AddHandler(ByVal value As AstmConnectionReceivedDataEventHandler)
            Me.e_OnReceiveString = TryCast(System.Delegate.Combine(Me.e_OnReceiveString, value), AstmConnectionReceivedDataEventHandler)
        End AddHandler
        RemoveHandler(ByVal value As AstmConnectionReceivedDataEventHandler)
            Me.e_OnReceiveString = TryCast(System.Delegate.Remove(Me.e_OnReceiveString, value), AstmConnectionReceivedDataEventHandler)
        End RemoveHandler
        RaiseEvent(ByVal sender As Object, ByVal e As EventArgs)
        End RaiseEvent
    End Event

    Public Property NetWorkAddress() As String

    Public Property NetWorkPort() As UShort

    Public Sub ClearBuffers() Implements ILis01A2Connection.ClearBuffers
    End Sub

    Public Sub Connect() Implements ILis01A2Connection.Connect
        If Me.fIsInServerMode Then
            Return
        End If
        Me.fSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        If Me.fSocket.Connected Then
            Throw New Lis01A02TCPConnectionException("Could not connect to server because the connection is already open")
        End If
        Me.fSocket.Connect(Me.NetWorkAddress, CInt(Math.Truncate(Me.NetWorkPort)))
        If Me.fSocket.Connected Then
            Me.ReceiveLoop()
        End If
    End Sub

    Public Sub DisConnect() Implements ILis01A2Connection.DisConnect
        If Me.fSocket.Connected Then
            Me.fSocket.Close()
        End If
        'INSTANT VB TODO TASK: 'ref locals' are not converted by Instant VB:
        'ORIGINAL LINE: ref Socket socketPointer = ref this.fSocket;
        Dim socketPointer As Socket = Me.fSocket
        Dim disposable As IDisposable = TryCast(socketPointer, IDisposable)
        If disposable Is Nothing Then
        Else
            disposable.Dispose()
        End If
        socketPointer = Nothing
    End Sub

    Public Function StartListeningAsync() As Task

        Me.fIsInServerMode = True
        Dim server As New TcpListener(IPAddress.Parse(Me.NetWorkAddress), CInt(Math.Truncate(Me.NetWorkPort)))
        server.Start()
        Dim bytes(1023) As Byte
        Dim data As String = Nothing
        Do
            Me.fClient = server.AcceptTcpClient()
            Me.fSocket = Me.fClient.Client
            data = Nothing
            Dim stream As NetworkStream = Me.fClient.GetStream()
            Dim i As Integer = stream.Read(bytes, 0, bytes.Length)
            Do While i <> 0
                data = Encoding.ASCII.GetString(bytes, 0, i)
                Dim u0040eOnReceiveString As AstmConnectionReceivedDataEventHandler = Me.e_OnReceiveString
                If u0040eOnReceiveString IsNot Nothing Then
                    u0040eOnReceiveString(Me, New AstmConnectionReceivedDataEventArgs(data))
                Else
                End If
                i = stream.Read(bytes, 0, bytes.Length)
            Loop
            Me.fClient.Close()
        Loop
    End Function

    Public Sub WriteData(ByVal value As String) Implements ILis01A2Connection.WriteData
        Dim buffer() As Byte = Encoding.UTF8.GetBytes(value)
        Me.fSocket.Send(buffer)
    End Sub

    Private Function ReceiveLoop() As Task
#Disable Warning BC42105 ' Function doesn't return a value on all code paths
    End Function

#Enable Warning BC42105 ' Function doesn't return a value on all code paths

End Class