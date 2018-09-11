Namespace Records.Order

    Public Enum OrderActionCode
        None

        <AstmEnum("C")>
        Cancel

        <AstmEnum("A")>
        Add

        <AstmEnum("N")>
        [New]

        <AstmEnum("P")>
        Pending

        <AstmEnum("L")>
        Reserved

        <AstmEnum("X")>
        InProcess

        <AstmEnum("Q")>
        QC

    End Enum

End Namespace