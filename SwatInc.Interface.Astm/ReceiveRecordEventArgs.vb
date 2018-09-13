Imports SwatInc.Interface.Astm.Records

Public Class ReceiveRecordEventArgs
    Inherits EventArgs

    Public Sub New(ByVal aReceivedRecord As AbstractAstmRecord, ByVal aLisRecordType As AstmRecordType)
        MyBase.New()
        Me.ReceivedRecord = aReceivedRecord
        Me.RecordType = aLisRecordType
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Property ReceivedRecord As AbstractAstmRecord

    Public Property RecordType As AstmRecordType
End Class