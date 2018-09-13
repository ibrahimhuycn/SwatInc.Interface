Imports SwatInc.Interface.Astm
Imports SwatInc.Interface.Astm.Communications
Imports SwatInc.Interface.Astm.Records
Imports SwatInc.Interface.Astm.Records.Header
Imports SwatInc.Interface.Astm.Records.Order
Imports SwatInc.Interface.Astm.Records.PartialRecords
Imports SwatInc.Interface.Astm.Records.Patient

Public Class Form1

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        Me.Refresh()
        Dim someIP = "192.168.1.2"
        Dim somePort As UInt16 = 1111
        Dim lowLevelConnection = New Lis01A02TCPConnection(someIP, somePort)
        Dim astmConnection = New Lis01A2Connection(lowLevelConnection)

        Dim astmParser = New AstmParser(astmConnection)
        astmParser.Connection.Connect()
        Dim a As New Test(astmConnection)


    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim someIP = "192.168.1.2"
        Dim somePort As UInt16 = 1111
        Dim lowLevelConnection = New Lis01A02TCPConnection(someIP, somePort)
        Dim astmConnection = New Lis01A2Connection(lowLevelConnection)

        Dim astmParser = New AstmParser(astmConnection)
        astmParser.Connection.Connect()
        Test(astmParser)
    End Sub

    Private Sub Test(cmx As AstmParser)
        Dim lisRecordList = New List(Of AbstractAstmRecord)()
        Dim hr = New HeaderRecord()
        hr.SenderID = "Some Sender ID Code"
        hr.ProcessingID = HeaderProcessingID.Production
        lisRecordList.Add(hr)
        Dim pr = New PatientRecord()
        pr.SequenceNumber = 1
        pr.LaboratoryAssignedPatientID = "Sam001"
        lisRecordList.Add(pr)
        Dim orderRec = New OrderRecord()
        orderRec.SequenceNumber = 1
        orderRec.SpecimenID = "Sam001"
        orderRec.TestID = New UniversalTestID()
        orderRec.TestID.ManufacturerCode = "T001"
        orderRec.ReportType = OrderReportType.Final
        lisRecordList.Add(orderRec)
        pr = New PatientRecord()
        pr.SequenceNumber = 2
        pr.LaboratoryAssignedPatientID = "Sam002"
        lisRecordList.Add(pr)
        orderRec = New OrderRecord()
        orderRec.SequenceNumber = 1
        orderRec.SpecimenID = "Sam002"
        orderRec.TestID = New UniversalTestID()
        orderRec.TestID.ManufacturerCode = "T001"
        orderRec.ReportType = OrderReportType.Final
        lisRecordList.Add(orderRec)
        Dim tr = New TerminatorRecord()
        lisRecordList.Add(tr)

        cmx.SendRecords(lisRecordList)
    End Sub

End Class