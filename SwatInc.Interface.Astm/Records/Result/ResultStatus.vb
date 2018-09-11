Namespace Records.Result

    Public Enum ResultStatus
        None

        <AstmEnum("C")>
        Correction

        <AstmEnum("P")>
        PreliminaryResults

        <AstmEnum("F")>
        FinalResults

        <AstmEnum("X")>
        CannotBeDone

        <AstmEnum("I")>
        ResultsPending

        <AstmEnum("S")>
        PartialResults

        <AstmEnum("M")>
        MICLevel

        <AstmEnum("R")>
        PreviouslyTransmitted

        <AstmEnum("N")>
        NecessaryInformation

        <AstmEnum("Q")>
        ResponseToOutstandingQuery

        <AstmEnum("V")>
        ApprovedResult

        <AstmEnum("W")>
        Warning

    End Enum

End Namespace