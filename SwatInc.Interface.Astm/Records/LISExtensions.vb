Imports System.Globalization
Imports System.Runtime.CompilerServices

Friend Module LISExtensions

    <Extension>
    Public Function LisStringToDateTime(ByVal lisString As String, ByVal aLisDateTimeUsage As AstmDateTimeUsage) As DateTime
        Dim Result As DateTime = New DateTime()
        Select Case CInt(aLisDateTimeUsage)
            Case 0
                Result = DateTime.ParseExact(lisString, "yyyyMMddHHmmss", CultureInfo.InvariantCulture)
                Exit Select
            Case 1
                Result = DateTime.ParseExact(lisString, "yyyyMMdd", CultureInfo.InvariantCulture)
                Exit Select
            Case 2
                Result = DateTime.ParseExact(lisString, "HHmmss", CultureInfo.InvariantCulture)
                Exit Select
        End Select
        Return Result
    End Function

    <Extension>
    Public Function ToAstmDate(ByVal aDateTime As DateTime, ByVal aAstmDateTimeUsage As AstmDateTimeUsage) As String
        Dim Result As String = Nothing
        Dim year As Integer
        Select Case CInt(aAstmDateTimeUsage)
            Case 0
                year = aDateTime.Year
                Dim str As String = year.ToString("D4")
                year = aDateTime.Month
                Dim str1 As String = String.Concat(str, year.ToString("D2"))
                year = aDateTime.Day
                Dim str2 As String = String.Concat(str1, year.ToString("D2"))
                year = aDateTime.Hour
                Dim str3 As String = String.Concat(str2, year.ToString("D2"))
                year = aDateTime.Minute
                Dim str4 As String = String.Concat(str3, year.ToString("D2"))
                year = aDateTime.Second
                Result = String.Concat(str4, year.ToString("D2"))
                Exit Select
            Case 1
                year = aDateTime.Year
                Dim str5 As String = year.ToString("D4")
                year = aDateTime.Month
                Dim str6 As String = String.Concat(str5, year.ToString("D2"))
                year = aDateTime.Day
                Result = String.Concat(str6, year.ToString("D2"))
                Exit Select
            Case 2
                year = aDateTime.Hour
                Dim str7 As String = year.ToString("D2")
                year = aDateTime.Minute
                Dim str8 As String = String.Concat(str7, year.ToString("D2"))
                year = aDateTime.Second
                Result = String.Concat(str8, year.ToString("D2"))
                Exit Select
        End Select
        Return Result
    End Function

End Module