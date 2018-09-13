Imports log4net
Imports SwatInc.Interface.Astm.Essy.LIS.LIS02A2
Imports SwatInc.Interface.Astm.Records
Imports SwatInc.Interface.Astm.Records.Header
Imports SwatInc.Interface.Astm.Records.Order
Imports SwatInc.Interface.Astm.Records.Patient
Imports SwatInc.Interface.Astm.Records.Query
Imports SwatInc.Interface.Astm.Records.Result
Imports System.Threading

Public Class AstmParser
    Private e_OnSendProgress As EventHandler(Of SendProgressEventArgs)
    Private fConnection As IAstmConnection
    Private fLog As ILog
    Private fOnExceptionHappened As ThreadExceptionEventHandler
    Private fOnReceivedRecord As ReceiveRecordEventHandler

    Public Sub New(ByVal aConnection As IAstmConnection)
        Me.Connection = aConnection
        'Me.fLog = LISLogger.Log
    End Sub

    Public Custom Event OnExceptionHappened As ThreadExceptionEventHandler
        AddHandler(ByVal value As ThreadExceptionEventHandler)
            Me.fOnExceptionHappened = TryCast(System.Delegate.Combine(Me.fOnExceptionHappened, value), ThreadExceptionEventHandler)
        End AddHandler
        RemoveHandler(ByVal value As ThreadExceptionEventHandler)
            Me.fOnExceptionHappened = TryCast(System.Delegate.Remove(Me.fOnExceptionHappened, value), ThreadExceptionEventHandler)
        End RemoveHandler
        RaiseEvent(ByVal sender As System.Object, ByVal e As System.Threading.ThreadExceptionEventArgs)
        End RaiseEvent
    End Event

    Public Custom Event OnReceivedRecord As ReceiveRecordEventHandler
        AddHandler(ByVal value As ReceiveRecordEventHandler)
            Me.fOnReceivedRecord = TryCast(System.Delegate.Combine(Me.fOnReceivedRecord, value), ReceiveRecordEventHandler)
        End AddHandler
        RemoveHandler(ByVal value As ReceiveRecordEventHandler)
            Me.fOnReceivedRecord = TryCast(System.Delegate.Remove(Me.fOnReceivedRecord, value), ReceiveRecordEventHandler)
        End RemoveHandler
        RaiseEvent(ByVal sender As Object, ByVal e As EventArgs)
        End RaiseEvent
    End Event

    Public Custom Event OnSendProgress As EventHandler(Of SendProgressEventArgs)
        AddHandler(ByVal value As EventHandler(Of SendProgressEventArgs))
            Me.e_OnSendProgress = TryCast(System.Delegate.Combine(Me.e_OnSendProgress, value), EventHandler(Of SendProgressEventArgs))
        End AddHandler
        RemoveHandler(ByVal value As EventHandler(Of SendProgressEventArgs))
            Me.e_OnSendProgress = TryCast(System.Delegate.Remove(Me.e_OnSendProgress, value), EventHandler(Of SendProgressEventArgs))
        End RemoveHandler
        RaiseEvent(ByVal sender As System.Object, ByVal e As SendProgressEventArgs)
        End RaiseEvent
    End Event

    Public Property Connection() As IAstmConnection
        Get
            Return Me.fConnection
        End Get
        Set(ByVal value As IAstmConnection)
            If Me.fConnection IsNot Nothing AndAlso Me.fConnection IsNot value Then
                RemoveHandler fConnection.OnReceiveString, AddressOf ReceivedData
                RemoveHandler fConnection.OnReceiveTimeOut, AddressOf Connection_OnReceiveTimeOut
            End If
            Me.fConnection = value
            AddHandler fConnection.OnReceiveString, AddressOf ReceivedData
            AddHandler fConnection.OnReceiveTimeOut, AddressOf Connection_OnReceiveTimeOut
        End Set
    End Property

    Public Property ThreadGuid() As Guid

    Public Sub SendRecords(ByVal records As IEnumerable(Of AbstractAstmRecord))
        Try
            If records IsNot Nothing Then
                If Not Me.Connection.EstablishSendMode() AndAlso Not Me.Connection.EstablishSendMode() Then
                    'Me.fLog.Error("The LIS did not acknowledge the ENQ request.")
                    MsgBox("The LIS did not acknowledge the ENQ request.")
                    Throw New AstmParserEstablishmentFailedException("The LIS did not acknowledge the ENQ request.")
                End If
                ' Me.fLog.Info("Send Mode Established")
                Dim recCount As Double = 0
                If TypeOf records Is IList Then
                    recCount = CDbl((TryCast(records, IList)).Count)
                End If
                If records IsNot Nothing Then
                    Dim sendCounter As Integer = 0
                    Dim enumerator As IEnumerator(Of AbstractAstmRecord) = records.GetEnumerator()
                    If enumerator IsNot Nothing Then
                        Try
                            Do While enumerator.MoveNext()
                                Dim rec As AbstractAstmRecord = enumerator.Current
                                Me.Connection.SendMessage(rec.ToAstmString())
                                If (If(Me.e_OnSendProgress Is Nothing, False, recCount > 0)) Then
                                    Me.e_OnSendProgress(Me, New SendProgressEventArgs(CDbl(sendCounter) / recCount, Me.ThreadGuid))
                                End If
                                sendCounter += 1
                            Loop
                        Finally
                            enumerator.Dispose()
                        End Try
                    End If
                End If
                Me.Connection.StopSendMode()
            End If
        Catch exception As Exception
            Dim ex As Exception = exception
            MsgBox(ex.Message & vbCrLf & ex.StackTrace)
            'Me.fLog.Error(ex.Message)
            If Me.fOnExceptionHappened IsNot Nothing Then
                Dim ea As New ThreadExceptionEventArgs(ex)
                Me.fOnExceptionHappened(Me, ea)
            End If
        End Try
    End Sub

    Private Sub Connection_OnReceiveTimeOut(ByVal sender As Object, ByVal e As EventArgs)
        If Me.fOnExceptionHappened IsNot Nothing Then
            Me.fLog.Error("No incoming data within timeout")
            Dim ea As New ThreadExceptionEventArgs(New AstmParserReceiveTimeOutException("No incoming data within timeout"))
            Me.fOnExceptionHappened(Me, ea)
        End If
    End Sub

    Private Sub ParseReceivedRecord(ByVal aReceivedRecordString As String)
        Dim tempArgs As New ReceiveRecordEventArgs()
        Dim RecordTypeChar As Char = aReceivedRecordString.Chars(0)
        Select Case AscW(RecordTypeChar)
            Case 79
                tempArgs.ReceivedRecord = New OrderRecord(aReceivedRecordString)
                tempArgs.RecordType = AstmRecordType.Order
                Me.fOnReceivedRecord(Me, tempArgs)
                Exit Select
            Case 80
                tempArgs.ReceivedRecord = New PatientRecord(aReceivedRecordString)
                tempArgs.RecordType = AstmRecordType.Patient
                Me.fOnReceivedRecord(Me, tempArgs)
                Exit Select
            Case 81
                tempArgs.ReceivedRecord = New QueryRecord(aReceivedRecordString)
                tempArgs.RecordType = AstmRecordType.Query
                Me.fOnReceivedRecord(Me, tempArgs)
                Exit Select
            Case 82
                tempArgs.ReceivedRecord = New ResultRecord(aReceivedRecordString)
                tempArgs.RecordType = AstmRecordType.Result
                Me.fOnReceivedRecord(Me, tempArgs)
                Exit Select
            Case Else
                If RecordTypeChar = "H"c Then
                    tempArgs.ReceivedRecord = New HeaderRecord(aReceivedRecordString)
                    tempArgs.RecordType = AstmRecordType.Header
                    Me.fOnReceivedRecord(Me, tempArgs)
                    Exit Select
                ElseIf RecordTypeChar = "L"c Then
                    tempArgs.ReceivedRecord = New TerminatorRecord(aReceivedRecordString)
                    tempArgs.RecordType = AstmRecordType.Terminator
                    Me.fOnReceivedRecord(Me, tempArgs)
                    Exit Select
                Else
                    Exit Select
                End If
        End Select
    End Sub

    Private Sub ReceivedData(ByVal Sender As Object, ByVal e As AstmConnectionReceivedDataEventArgs)
        If Me.fOnReceivedRecord Is Nothing Then
            Return
        End If
        Try
            Dim records() As String = e.ReceivedData.Split(New Char() {ControlChars.Cr})
            Dim num As Integer = 0
            Dim strArrays() As String = records
            If strArrays IsNot Nothing Then
                Do While num < strArrays.Length
                    Dim r As String = strArrays(num)
                    If Not String.IsNullOrEmpty(r) Then
                        Me.ParseReceivedRecord(r)
                    End If
                    num += 1
                Loop
            End If
        Catch exception As Exception
            Dim ex As Exception = exception
            If Me.fOnExceptionHappened IsNot Nothing Then
                Me.fLog.Error(ex.Message)
                Dim ea As New ThreadExceptionEventArgs(ex)
                Me.fOnExceptionHappened(Me, ea)
            End If
        End Try
    End Sub

End Class