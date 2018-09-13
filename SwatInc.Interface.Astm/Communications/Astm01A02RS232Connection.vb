Imports System.IO.Ports

Namespace Communications

    Public Class Lis01A02RS232Connection
        Implements ILis01A2Connection

        Private e_OnReceiveString As AstmConnectionReceivedDataEventHandler
        Private fComport As SerialPort

        Public Sub New(ByVal aComPort As SerialPort)
            Me.ComPort = aComPort
        End Sub

        Public Custom Event OnReceiveString As AstmConnectionReceivedDataEventHandler
            AddHandler(ByVal value As AstmConnectionReceivedDataEventHandler)
                Me.e_OnReceiveString = TryCast(System.Delegate.Combine(Me.e_OnReceiveString, value), AstmConnectionReceivedDataEventHandler)
            End AddHandler
            RemoveHandler(ByVal value As AstmConnectionReceivedDataEventHandler)
                Me.e_OnReceiveString = TryCast(System.Delegate.Remove(Me.e_OnReceiveString, value), AstmConnectionReceivedDataEventHandler)
            End RemoveHandler
            RaiseEvent(ByVal sender As Object, ByVal e As EventArgs)
            End RaiseEvent
        End Event

        Private Event ILis01A2Connection_OnReceiveString As AstmConnectionReceivedDataEventHandler Implements ILis01A2Connection.OnReceiveString

        Public Property ComPort() As SerialPort
            Get
                Return Me.fComport
            End Get
            Set(ByVal value As SerialPort)
                If Me.fComport IsNot Nothing AndAlso Me.fComport IsNot value Then
                    RemoveHandler fComport.DataReceived, AddressOf COMPortDataReceived
                End If
                Me.fComport = value
                AddHandler fComport.DataReceived, AddressOf COMPortDataReceived
                Me.fComport.ReadTimeout = 15000
            End Set
        End Property

        Public Sub ClearBuffers() Implements ILis01A2Connection.ClearBuffers
            Me.fComport.DiscardInBuffer()
            Me.fComport.DiscardOutBuffer()
        End Sub

        Public Sub Connect() Implements ILis01A2Connection.Connect
            Me.fComport.Open()
        End Sub

        Public Sub DisConnect() Implements ILis01A2Connection.DisConnect
            Me.fComport.Close()
        End Sub

        Public Sub WriteData(ByVal value As String) Implements ILis01A2Connection.WriteData
            Me.fComport.Write(value)
        End Sub

        Private Sub COMPortDataReceived(ByVal Sender As Object, ByVal e As SerialDataReceivedEventArgs)
            Dim receivedData As String = Me.ComPort.ReadExisting()
            Dim OnReceiveString As AstmConnectionReceivedDataEventHandler = Me.e_OnReceiveString
            If OnReceiveString IsNot Nothing Then
                OnReceiveString(Me, New AstmConnectionReceivedDataEventArgs(receivedData))
            Else
            End If
        End Sub

    End Class

End Namespace