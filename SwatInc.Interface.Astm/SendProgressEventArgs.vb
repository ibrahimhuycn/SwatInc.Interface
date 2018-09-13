Public Class SendProgressEventArgs
    Inherits EventArgs
    Public Property Progress As Double

    Public Property ThreadGuid As Guid

    Public Sub New(ByVal aProgress As Double, ByVal aThreadGuid As Guid)
        MyBase.New()
        Me.Progress = aProgress
        Me.ThreadGuid = aThreadGuid
    End Sub
End Class