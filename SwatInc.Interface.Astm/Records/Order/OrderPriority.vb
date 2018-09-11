Namespace Records.Order

    Public Enum OrderPriority
        None

        <AstmEnum("S")>
        Stat

        <AstmEnum("A")>
        ASAP

        <AstmEnum("R")>
        Routine

        <AstmEnum("C")>
        CallBack

        <AstmEnum("P")>
        PreOperative

    End Enum

End Namespace