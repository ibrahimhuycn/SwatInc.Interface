Namespace Records.Result

    Public Enum ResultAbnormalFlags
        None

        <AstmEnum("L")>
        BelowLowNormal

        <AstmEnum("H")>
        AboveHighNormal

        <AstmEnum("LL")>
        BelowPanicNormal

        <AstmEnum("HH")>
        AbovePanicHigh

        <AstmEnum("<")>
        BelowAbsoluteLow

        <AstmEnum(">")>
        AboveAbsoluteHigh

        <AstmEnum("N")>
        Normal

        <AstmEnum("A")>
        Abnormal

        <AstmEnum("U")>
        SignificantChangeUp

        <AstmEnum("D")>
        SignificantChangeDown

        <AstmEnum("B")>
        Better

        <AstmEnum("W")>
        Worse

    End Enum

End Namespace