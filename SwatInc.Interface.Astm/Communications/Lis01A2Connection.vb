Imports log4net
Imports System.Text
Imports System.Threading
Imports System.Timers

Namespace Communications

    ''' <summary>
    ''' This is the LIS01-A2 connection class, it encodes data according to CLSI LIS01-A2 Chapter 6
    ''' </summary>
    Public Class Lis01A2Connection
        Implements IAstmConnection

        Protected Friend fIsDisposed As Boolean
        Private Const ACK As Char = ChrW(&H6)
        Private Const CR As Char = ControlChars.Cr
        Private Const ENQ As Char = ChrW(&H5)
        Private Const EOT As Char = ChrW(&H4)
        Private Const ETB As Char = ChrW(&H17)
        Private Const ETX As Char = ChrW(&H3)
        Private Const LF As Char = ControlChars.Lf
        Private Const MAXFRAMESIZE As Integer = 63993
        Private Const NAK As Char = ChrW(&H15)
        Private Const STX As Char = ChrW(&H2)
        Private e_OnLISConnectionClosed As EventHandler
        Private e_OnReceiveString As AstmConnectionReceivedDataEventHandler
        Private e_OnReceiveTimeOut As EventHandler
        Private fAckReceived As Boolean
        Private fACKWaitObject As New EventWaitHandle(True, EventResetMode.ManualReset)
        Private fConnection As ILis01A2Connection
        Private fENQWaitObject As New EventWaitHandle(True, EventResetMode.ManualReset)
        Private fEOTWaitObject As New EventWaitHandle(True, EventResetMode.ManualReset)
        Private fFrameNumber As Integer
        Private fLastFrameWasIntermediate As Boolean
        Private fLog As ILog
        Private fReceiveTimeOutTimer As New Timers.Timer()
        Private fTempIntermediateFrameBuffer As String
        Private fTempReceiveBuffer As New StringBuilder()

        ''' <summary>
        '''  Creates a new Lis01A2 Connection object.
        ''' </summary>
        ''' <param name="aConnection">An implementation of the ILis01A2Connection Interface.</param>
        Public Sub New(ByVal aConnection As ILis01A2Connection)
            Me.New(aConnection, 30)
        End Sub

        ''' <summary>
        ''' Creates a new Lis01A2 Connection object.
        ''' </summary>
        ''' <param name="aConnection">An implementation of the ILis01A2Connection Interface.</param>
        ''' <param name="aTimeOut">A receive timeout in seconds.</param>
        Public Sub New(ByVal aConnection As ILis01A2Connection, ByVal aTimeOut As Integer)
            Me.Connection = aConnection
            Me.fReceiveTimeOutTimer.Interval = CDbl(aTimeOut * 1000)
            Me.fReceiveTimeOutTimer.Enabled = False
            AddHandler fReceiveTimeOutTimer.Elapsed, AddressOf fReceiveTimeOutTimer_Elapsed
        End Sub

        Protected Overrides Sub Finalize()
            Me.Dispose(False)
        End Sub

        ''' <summary>
        ''' This event is fired when the low level connection is successfully closed.
        ''' </summary>
        Public Custom Event OnAstmConnectionClosed As EventHandler Implements IAstmConnection.OnAstmConnectionClosed
            AddHandler(ByVal value As EventHandler)
                Me.e_OnLISConnectionClosed = TryCast(System.Delegate.Combine(Me.e_OnLISConnectionClosed, value), EventHandler)
            End AddHandler
            RemoveHandler(ByVal value As EventHandler)
                Me.e_OnLISConnectionClosed = TryCast(System.Delegate.Remove(Me.e_OnLISConnectionClosed, value), EventHandler)
            End RemoveHandler
            RaiseEvent(ByVal sender As System.Object, ByVal e As System.EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' This event is triggered when incoming message is received.
        ''' </summary>
        Public Custom Event OnReceiveString As AstmConnectionReceivedDataEventHandler Implements IAstmConnection.OnReceiveString
            AddHandler(ByVal value As AstmConnectionReceivedDataEventHandler)
                Me.e_OnReceiveString = TryCast(System.Delegate.Combine(Me.e_OnReceiveString, value), AstmConnectionReceivedDataEventHandler)
            End AddHandler
            RemoveHandler(ByVal value As AstmConnectionReceivedDataEventHandler)
                Me.e_OnReceiveString = TryCast(System.Delegate.Remove(Me.e_OnReceiveString, value), AstmConnectionReceivedDataEventHandler)
            End RemoveHandler
            RaiseEvent(ByVal sender As Object, ByVal e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' This event fires when a receive timeout is detected, see CLSI LIS01-A2 Chapter 6.5.2
        ''' </summary>
        Public Custom Event OnReceiveTimeOut As EventHandler Implements IAstmConnection.OnReceiveTimeOut
            AddHandler(ByVal value As EventHandler)
                Me.e_OnReceiveTimeOut = TryCast(System.Delegate.Combine(Me.e_OnReceiveTimeOut, value), EventHandler)
            End AddHandler
            RemoveHandler(ByVal value As EventHandler)
                Me.e_OnReceiveTimeOut = TryCast(System.Delegate.Remove(Me.e_OnReceiveTimeOut, value), EventHandler)
            End RemoveHandler
            RaiseEvent(ByVal sender As System.Object, ByVal e As System.EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' This is the low level connection object that is used to put the frames on the wire, e.g. a COM-Port or TCP Socket connection.
        ''' </summary>
        ''' <value>An implementation of the ILis01A2Connection Interface.</value>
        Public Property Connection() As ILis01A2Connection
            Get
                Return Me.fConnection
            End Get
            Set(ByVal value As ILis01A2Connection)
                If Me.fConnection IsNot Nothing AndAlso Me.fConnection IsNot value Then
                    RemoveHandler fConnection.OnReceiveString, AddressOf ConnectionDataReceived
                End If
                Me.fConnection = value
                AddHandler fConnection.OnReceiveString, AddressOf ConnectionDataReceived
            End Set
        End Property

        ''' <summary>
        ''' The state of the connection
        ''' </summary>
        ''' <value>Idle, Sending, Receiving, Establishing</value>
        Public Property Status() As AstmConnectionStatus

        Private Property IAstmConnection_Status As AstmConnectionStatus Implements IAstmConnection.Status
            Get
                Throw New NotImplementedException()
            End Get
            Set(value As AstmConnectionStatus)
                Throw New NotImplementedException()
            End Set
        End Property

        ''' <summary>
        ''' Connects the low level connection, e.g. opens the COM-port or opens the TCP/IP Socket
        ''' </summary>
        Public Sub Connect() Implements IAstmConnection.Connect

            Try
                'Me.fLog.Info("Connecting To LIS...")
                Me.Connection.Connect()
                'Me.fLog.Info("Connected")
            Catch exception As Exception
                Dim ex As Exception = exception
                'Me.fLog.Error(String.Concat(String.Concat("Error Opening Connection", Environment.NewLine), ex.Message))
                Throw New Lis01A2ConnectionException("Error opening Connection.", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Disconnects the low level connection, e.g. closes the COM-port or closes the TCP/IP Socket
        ''' </summary>
        Public Sub DisConnect() Implements IAstmConnection.DisConnect
            If (If(CInt(Math.Truncate(Me.Status)) = CInt(0), False, Not Me.fEOTWaitObject.WaitOne(15000))) Then
                Throw New Lis01A2ConnectionException("Error closing Connection, no EOT received.")
            End If
            Try
                Me.fLog.Info("Disconnecting...")
                Me.Connection.DisConnect()
                Me.fLog.Info("Disconnected")
                If Me.e_OnLISConnectionClosed IsNot Nothing Then
                    Me.e_OnLISConnectionClosed(Me, New EventArgs())
                End If
            Catch exception As Exception
                Dim ex As Exception = exception
                Me.fLog.Error(String.Concat(String.Concat("Error Closing Connection", Environment.NewLine), ex.Message))
                Throw New Lis01A2ConnectionException("Error closing Connection.", ex)
            End Try
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Me.Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        ''' <summary>
        ''' Initiates the Establishment Phase, see CLSI LIS01-A2 Chapter 6.2
        ''' </summary>
        ''' <returns>True when the receiving system answered with an ACK</returns>
        Public Function EstablishSendMode() As Boolean Implements IAstmConnection.EstablishSendMode
            Dim Result As Boolean = True
            Me.fFrameNumber = 1
            If CInt(Math.Truncate(Me.Status)) <> CInt(0) Then
                Me.fLog.Error("Connection not idle when trying to establish send mode")
                Throw New Lis01A2ConnectionException("Connection not idle when trying to establish send mode")
            End If
            Me.Status = AstmConnectionStatus.Establishing
            'Me.fLog.Info("Establishing send mode")
            Me.Connection.ClearBuffers()
            Me.fENQWaitObject.Reset()
            Me.Connection.WriteData(ChrW(&H5).ToString())
            'Me.fLog.Debug("send <ENQ>")
            Me.fENQWaitObject.WaitOne(15000, False)
            If Not CInt(Math.Truncate(Me.Status)) = CInt(1) Then
                Return Result
            End If
            Me.StopSendMode()
            Return False
        End Function

        ''' <summary>
        ''' Transmits a message to the receiver.
        ''' </summary>
        ''' <param name="aMessage">The message to be transmitted, messages are sent in frames; each frame contains a maximum of 64 000 characters (including frame
        ''' overhead). Messages longer than 64 000 characters are divided between two or more frames.</param>
        Public Sub SendMessage(ByVal aMessage As String) Implements IAstmConnection.SendMessage
            Do While aMessage.Length > 63993
                Dim intermediateFrame As String = aMessage.Substring(0, 63993)
                Me.SendIntermediateFrame(Me.fFrameNumber, intermediateFrame)
                aMessage = aMessage.Remove(0, 63993)
                Me.fFrameNumber += 1
                If Me.fFrameNumber > 7 Then
                    Me.fFrameNumber = 0
                End If
            Loop
            Me.SendEndFrame(Me.fFrameNumber, aMessage)
            Me.fFrameNumber += 1
            If Me.fFrameNumber > 7 Then
                Me.fFrameNumber = 0
            End If
        End Sub

        ''' <summary>
        ''' Starts the Receive Timeout counter, see CLSI LIS01-A2 Chapter 6.5.2
        ''' </summary>
        Public Sub StartReceiveTimeoutTimer() Implements IAstmConnection.StartReceiveTimeoutTimer
            Me.fReceiveTimeOutTimer.Start()
        End Sub

        ''' <summary>
        ''' Transmits the EOT transmission control character and then regards the data link to be in a neutral state.
        ''' </summary>
        Public Sub StopSendMode() Implements IAstmConnection.StopSendMode
            Me.Connection.WriteData(ChrW(&H4).ToString())
            Me.fLog.Debug("send <EOT>")
            Me.Status = AstmConnectionStatus.Idle
        End Sub

        ''' <summary>
        ''' Disposes this object.
        ''' </summary>
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.fIsDisposed Then
                'INSTANT VB NOTE: The variable connection was renamed since Visual Basic does not handle local variables named the same as class members well:
                Dim connection_Renamed As IDisposable = TryCast(Me.Connection, IDisposable)
                If connection_Renamed IsNot Nothing Then
                    connection_Renamed.Dispose()
                Else
                End If
                Me.fIsDisposed = True
            End If
        End Sub

        Private Function CalculateChecksum(ByVal aString As String) As String
            Dim total As Integer = 0
            If aString IsNot Nothing Then
                Dim enumerator As CharEnumerator = aString.GetEnumerator()
                If enumerator IsNot Nothing Then
                    Try
                        Do While enumerator.MoveNext()
                            Dim i As Char = enumerator.Current
                            total = CInt(CUInt(total) + CUInt(AscW(i)))
                        Loop
                    Finally
                        enumerator.Dispose()
                    End Try
                End If
            End If
            Return CByte(total Mod 256).ToString("X2")
        End Function

        Private Function CheckChecksum(ByVal aLine As String) As Boolean
            Dim Result As Boolean = False
            Dim lineLength As Integer = aLine.Length
            If lineLength < 5 Then
                Return Result
            End If
            If aLine.Chars(0) <> ChrW(&H2) Then
                Return Result
            End If
            If aLine.Chars(lineLength - 1) <> ControlChars.Lf Then
                Return Result
            End If
            If aLine.Chars(lineLength - 2) <> ControlChars.Cr Then
                Return Result
            End If
            Dim etxOrEtb As Char = aLine.Chars(lineLength - 5)
            If (If(etxOrEtb = ChrW(&H3), False, etxOrEtb <> ChrW(&H17))) Then
                Return Result
            End If
            Me.fLastFrameWasIntermediate = etxOrEtb = ChrW(&H17)
            If aLine.Chars(lineLength - 6) <> ControlChars.Cr Then
                Return Result
            End If
            If aLine.Substring(lineLength - 4, 2) <> Me.CalculateChecksum(aLine.Substring(1, lineLength - 5)) Then
                Return Result
            End If
            Return True
        End Function

        Private Sub ConnectionDataReceived(ByVal Sender As Object, ByVal e As AstmConnectionReceivedDataEventArgs) Handles Me.OnReceiveString
            Dim buffer As String = e.ReceivedData
            If buffer IsNot Nothing Then
                Dim enumerator As CharEnumerator = buffer.GetEnumerator()
                If enumerator IsNot Nothing Then
                    Try
                        Do While enumerator.MoveNext()
                            Dim ch As Char = enumerator.Current
                            Select Case CInt(Math.Truncate(Me.Status))
                                Case 0
                                    If ch <> ChrW(&H5) Then
                                        Me.fLog.Debug("send <NAK>")
                                        Me.Connection.WriteData(ChrW(&H15).ToString())
                                    Else
                                        Me.fLog.Debug("received <ENQ>")
                                        Me.Connection.WriteData(ChrW(&H6).ToString())
                                        Me.fLog.Debug("send <ACK>")
                                        Me.Status = AstmConnectionStatus.Receiving
                                        Me.fEOTWaitObject.Reset()
                                        Me.fTempReceiveBuffer.Clear()
                                        Me.fTempIntermediateFrameBuffer = String.Empty
                                        Me.fReceiveTimeOutTimer.Enabled = True
                                    End If
                                    Exit Select
                                Case 1
                                    Me.fAckReceived = ch = ChrW(&H6)
                                    Me.fACKWaitObject.Set()
                                    Exit Select
                                Case 2
                                    Me.fReceiveTimeOutTimer.Stop()
                                    Me.fReceiveTimeOutTimer.Start()
                                    If ch = ChrW(&H5) Then
                                        Me.fLog.Debug("received <ENQ>")
                                        Me.Connection.WriteData(ChrW(&H15).ToString())
                                        Me.fLog.Debug("send <NAK>")
                                        Return
                                    ElseIf ch <> ChrW(&H4) Then
                                        If ch <> ControlChars.NullChar Then
                                            Me.fTempReceiveBuffer.Append(ch)
                                        End If
                                        If ch = ControlChars.Lf Then
                                            Dim tempReceiveBuffer As String = Me.fTempReceiveBuffer.ToString()
                                            If Not Me.CheckChecksum(tempReceiveBuffer) Then
                                                Me.fLog.Debug("send <NAK>")
                                                Me.Connection.WriteData(ChrW(&H15).ToString())
                                                Me.fTempReceiveBuffer.Clear()
                                            Else
                                                Me.fLog.Debug("send <ACK>")
                                                Me.Connection.WriteData(ChrW(&H6).ToString())
                                                Dim cleanReceiveBuffer As String = tempReceiveBuffer.Substring(2, tempReceiveBuffer.Length - 7)
                                                If Me.fLastFrameWasIntermediate Then
                                                    Me.fTempIntermediateFrameBuffer = String.Concat(Me.fTempIntermediateFrameBuffer, cleanReceiveBuffer)
                                                ElseIf Me.e_OnReceiveString IsNot Nothing Then
                                                    Dim line As String = String.Concat(Me.fTempIntermediateFrameBuffer, cleanReceiveBuffer)
                                                    Dim arg As New AstmConnectionReceivedDataEventArgs(line)
                                                    Me.e_OnReceiveString(Me, arg)
                                                    Me.fLog.Info(String.Concat("received: ", line))
                                                End If
                                                Me.fTempReceiveBuffer.Clear()
                                                Me.fTempIntermediateFrameBuffer = String.Empty
                                            End If
                                        End If
                                        Exit Select
                                    Else
                                        Me.fReceiveTimeOutTimer.Stop()
                                        Me.fLog.Debug("received <EOT>")
                                        Me.Status = AstmConnectionStatus.Idle
                                        Me.fEOTWaitObject.Set()
                                        Return
                                    End If
                                Case 3
                                    If ch = ChrW(&H6) Then
                                        Me.fLog.Debug("received <ACK>")
                                        Me.Status = AstmConnectionStatus.Sending
                                        Me.fENQWaitObject.Set()
                                        Return
                                    ElseIf ch <> ChrW(&H5) Then
                                        Exit Select
                                    Else
                                        Me.fLog.Debug("received <ENQ>")
                                        Thread.Sleep(1000)
                                        Me.fLog.Debug("send <ENQ>")
                                        Me.Connection.WriteData(ChrW(&H5).ToString())
                                        Return
                                    End If
                            End Select
                        Loop
                    Finally
                        enumerator.Dispose()
                    End Try
                End If
            End If
        End Sub

        Private Sub fReceiveTimeOutTimer_Elapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs)
            Me.Status = AstmConnectionStatus.Idle
            Me.fReceiveTimeOutTimer.Stop()
            If Me.e_OnReceiveTimeOut IsNot Nothing Then
                Me.e_OnReceiveTimeOut(Me, New EventArgs())
            End If
        End Sub

        Private Sub SendEndFrame(ByVal frameNumber As Integer, ByVal aString As String)
            'Me.fLog.Info(String.Concat(frameNumber.ToString(), aString))
            ' Me.fLog.Debug("send <ETX>")
            Me.SendString(String.Concat(String.Concat(frameNumber.ToString(), aString), ChrW(&H3).ToString()))
        End Sub

        Private Sub SendIntermediateFrame(ByVal frameNumber As Integer, ByVal aString As String)
            Me.fLog.Info(String.Concat(frameNumber.ToString(), aString))
            Me.fLog.Debug("send <ETB>")
            Me.SendString(String.Concat(String.Concat(frameNumber.ToString(), aString), ChrW(&H17).ToString()))
        End Sub

        Private Sub SendString(ByVal aString As String)
            If CInt(Math.Truncate(Me.Status)) <> CInt(1) Then
                'Me.fLog.Error("Connection not in Send mode when trying to send data.")
                MsgBox("Connection not in Send mode when trying to send data.")
                Throw New Lis01A2ConnectionException("Connection not in Send mode when trying to send data.")
            End If
            Me.Connection.ClearBuffers()
            Dim tryCounter As Integer = 0
            Dim tempSendString As String = String.Concat(String.Concat(String.Concat(String.Concat(ChrW(&H2).ToString(), aString), Me.CalculateChecksum(aString)), vbCr), vbLf)
            Me.Connection.WriteData(tempSendString)
            Do While Not Me.WaitForACK()
                tryCounter += 1
                If tryCounter > 5 Then
                    Me.StopSendMode()
                    Me.fLog.Error("Max number of send retries reached.")
                    Throw New Lis01A2ConnectionException("Max number of send retries reached.")
                End If
                Me.Connection.WriteData(tempSendString)
            Loop
        End Sub

        Private Function WaitForACK() As Boolean
            Me.fACKWaitObject.Reset()
            If Not Me.fACKWaitObject.WaitOne(15000) Then
                Me.StopSendMode()
                Me.fLog.Error("No response from LIS within timeout period.")
                Throw New Lis01A2ConnectionException("No response from LIS within timeout period.")
            End If
            Me.fLog.Debug("Received <ACK>")
            Return Me.fAckReceived
        End Function

    End Class

End Namespace