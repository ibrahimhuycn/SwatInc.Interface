Imports System.Runtime.CompilerServices
Imports System.Text

Module ToString

    ''' <summary>
    ''' Returns a delimited string from an Array.
    ''' </summary>
    ''' <param name="stringArray"></param>
    ''' <param name="delimiter"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    <Extension()> Public Function ToDelimitedString(ByVal stringArray As String(), ByVal delimiter As String) As String
        Dim delimitedStringBuilder As New StringBuilder

        For Each stringBuffer As String In stringArray
            delimitedStringBuilder.Append(stringBuffer)
            delimitedStringBuilder.Append(delimiter)
        Next

        ' The final delimiter is trimmed off since there is no record after that item
        Return delimitedStringBuilder.ToString.Trim(delimiter)
    End Function

End Module