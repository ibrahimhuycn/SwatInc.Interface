Namespace Records.Order

    Public Enum OrderReportType
        None

        <AstmEnum("O")>
        Order

        <AstmEnum("C")>
        Correction

        <AstmEnum("P")>
        Preliminary

        <AstmEnum("F")>
        Final

        <AstmEnum("X")>
        Cancelled

        <AstmEnum("I")>
        Pending

        <AstmEnum("Y")>
        NoOrder

        <AstmEnum("Z")>
        NoRecord

        <AstmEnum("Q")>
        Response

    End Enum

End Namespace