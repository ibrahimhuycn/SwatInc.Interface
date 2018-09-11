Imports System.Reflection
Imports System.Text

Namespace Records

    Public MustInherit Class AbstractAstmRecord

        Public Sub New(ByVal frame As String)
            MyBase.New()
            Dim type As Type
            Dim isPartialRecord As Boolean = TypeOf Me Is AbstractPartialRecord
            Dim delimiterInUse As Char = If(Not isPartialRecord, FieldDelimiter, ComponentDelimiter)
            Dim abstractRecord As AbstractAstmRecord = Me
            If abstractRecord IsNot Nothing Then
                type = abstractRecord.[GetType]()
            Else
                type = Nothing
            End If
            Dim frameProperties As PropertyInfo() = type.GetProperties()
            Dim recordFields As RecordFields = New RecordFields(frame, delimiterInUse, If(Not isPartialRecord, 2147483647, frameProperties.Length))
            Dim numberFields As Integer = 0

            Dim propertyInfoArray As PropertyInfo() = frameProperties
            If (propertyInfoArray IsNot Nothing) Then
                While numberFields < propertyInfoArray.Length
                    Dim propertyInfo As PropertyInfo = propertyInfoArray(numberFields)
                    Dim attribs As Object() = propertyInfo.GetCustomAttributes(GetType(RecordFieldIndexAttribute), False)
                    If (attribs.Length > 0) Then
                        Dim attrib As RecordFieldIndexAttribute = DirectCast(attribs(0), RecordFieldIndexAttribute)
                        Dim field As String = recordFields.GetField(attrib.FieldIndex)
                        If (Not String.IsNullOrEmpty(field)) Then
                            Dim propType As Type = propertyInfo.PropertyType
                            Dim nullablePropType As Type = Nullable.GetUnderlyingType(propType)
                            If (nullablePropType IsNot Nothing) Then
                                propType = nullablePropType
                            End If
                            If (propType = GetType(Integer)) Then
                                propertyInfo.SetValue(Me, Integer.Parse(Me.RemoveOptionalPartialFields(field)), Nothing)
                            ElseIf (propType = GetType(String)) Then
                                propertyInfo.SetValue(Me, field, Nothing)
                            ElseIf (propType = GetType(DateTime)) Then
                                Dim dateTimeUsage As AstmDateTimeUsage = AstmDateTimeUsage.DateTime
                                attribs = propertyInfo.GetCustomAttributes(GetType(AstmDateTimeUsageAttribute), False)
                                If (attribs.Length = 1) Then
                                    dateTimeUsage = DirectCast(attribs(0), AstmDateTimeUsageAttribute).DateTimeUsage
                                End If
                                'propertyInfo.SetValue(Me, Me.fRemoveOptionalSubFields(field).LisStringToDateTime(dateTimeUsage), Nothing)
                            ElseIf (Not propType.IsEnum) Then
                                If (propType.BaseType <> GetType(AbstractPartialRecord)) Then
                                    Throw New FormatException("The LIS String was not of the correct format.")
                                End If
                                propertyInfo.SetValue(Me, Me.CreatePartialRecord(field, propType), Nothing)
                            Else
                                propertyInfo.SetValue(Me, Me.CreateAstmEnum(Me.RemoveOptionalPartialFields(field), propType), Nothing)
                            End If
                        End If
                    End If
                    numberFields += 1
                End While
            End If
        End Sub

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overridable Function ToAstmString() As String
            Dim str As String
            Dim field As String
            Dim type As System.Type
            Dim isPartialRecord As Boolean = TypeOf Me Is AbstractPartialRecord
            Dim sepChar As Char = If(Not isPartialRecord, FieldDelimiter, ComponentDelimiter)
            Dim sb As StringBuilder = New StringBuilder()
            Dim fieldList As Dictionary(Of Integer, String) = New Dictionary(Of Integer, String)()
            Dim abstractLisRecord As AbstractAstmRecord = Me
            If (abstractLisRecord IsNot Nothing) Then
                type = abstractLisRecord.[GetType]()
            Else
                type = Nothing
            End If
            Dim props As PropertyInfo() = type.GetProperties()
            Dim maxFieldIndex As Integer = -2147483648
            Dim minFieldIndex As Integer = 2147483647
            Dim i As Integer = 0
            Dim propertyInfoArray As PropertyInfo() = props
            If (propertyInfoArray IsNot Nothing) Then
                While i < propertyInfoArray.Length
                    Dim prop As PropertyInfo = propertyInfoArray(i)
                    Dim attribs As Object() = prop.GetCustomAttributes(GetType(RecordFieldIndexAttribute), False)
                    If (attribs.Length > 0) Then
                        Dim attrib As RecordFieldIndexAttribute = DirectCast(attribs(0), RecordFieldIndexAttribute)
                        Dim propType As System.Type = prop.PropertyType
                        Dim nullablePropType As System.Type = Nullable.GetUnderlyingType(propType)
                        If (nullablePropType IsNot Nothing) Then
                            propType = nullablePropType
                        End If
                        field = Nothing
                        Dim propVal As Object = prop.GetValue(Me, Nothing)
                        If (propVal IsNot Nothing) Then
                            If (propType = GetType(DateTime)) Then
                                Dim dateTimeUsage As AstmDateTimeUsage = AstmDateTimeUsage.DateTime
                                attribs = prop.GetCustomAttributes(GetType(AstmDateTimeUsageAttribute), False)
                                If (attribs.Length = 1) Then
                                    dateTimeUsage = DirectCast(attribs(0), AstmDateTimeUsageAttribute).DateTimeUsage
                                End If
                                field = DirectCast(propVal, DateTime).ToAstmDate(dateTimeUsage)
                            ElseIf (Not propType.IsEnum) Then
                                field = If(propType.BaseType <> GetType(AbstractPartialRecord), propVal.ToString(), TryCast(propVal, AbstractPartialRecord).ToAstmString())
                            Else
                                field = Me.GetEnumAstmString(propVal)
                            End If
                        End If
                        If (Not String.IsNullOrEmpty(field)) Then
                            fieldList.Add(attrib.FieldIndex, field)
                            If (attrib.FieldIndex > maxFieldIndex) Then
                                maxFieldIndex = attrib.FieldIndex
                            End If
                        End If
                        If (attrib.FieldIndex < minFieldIndex) Then
                            minFieldIndex = attrib.FieldIndex
                        End If
                    End If
                    i = i + 1
                End While
            End If
            If (minFieldIndex <= maxFieldIndex) Then
                Dim num As Integer = maxFieldIndex - 1
                i = minFieldIndex
                If (i <= num) Then
                    num = num + 1
                    Do
                        fieldList.TryGetValue(i, field)
                        If (Not String.IsNullOrEmpty(field)) Then
                            sb.Append(Me.EscapeString(field, isPartialRecord))
                        End If
                        sb.Append(sepChar)
                        i = i + 1
                    Loop While i <> num
                End If
            End If
            fieldList.TryGetValue(maxFieldIndex, str)
            If (Not String.IsNullOrEmpty(str)) Then
                sb.Append(str)
            End If
            If (Not isPartialRecord) Then
                sb.Append(Strings.ChrW(13))
            End If
            Return sb.ToString()

        End Function

        Public Overrides Function ToString() As String
            Dim type As Type
            Dim stringBuilder As StringBuilder = New StringBuilder()
            Dim abstractAstmRecord As AbstractAstmRecord = Me
            If (abstractAstmRecord IsNot Nothing) Then
                type = abstractAstmRecord.[GetType]()
            Else
                type = Nothing
            End If
            Dim PropertyInformation As PropertyInfo() = type.GetProperties()
            Dim numberFields As Integer = 0
            Dim propertyInfoArray As PropertyInfo() = PropertyInformation
            If (propertyInfoArray IsNot Nothing) Then
                While numberFields < propertyInfoArray.Length
                    Dim prop As PropertyInfo = propertyInfoArray(numberFields)
                    If (prop.GetCustomAttributes(GetType(RecordFieldIndexAttribute), False).Length > 0) Then
                        Dim propVal As Object = prop.GetValue(Me, Nothing)
                        Dim propString As String = Nothing
                        If (propVal IsNot Nothing) Then
                            propString = propVal.ToString()
                        End If
                        If (Not String.IsNullOrEmpty(propString)) Then
                            stringBuilder.Append(prop.Name)
                            stringBuilder.Append(": ")
                            stringBuilder.AppendLine(propString)
                        End If
                    End If
                    numberFields = numberFields + 1
                End While
            End If
            Return stringBuilder.ToString()
        End Function

        Private Function CreateAstmEnum(ByVal aString As String, ByVal aType As Type) As Object
            Dim Id As String
            Dim fields As FieldInfo()
            Dim fi As FieldInfo
            Dim num As Integer
            Dim attribs As AstmEnumAttribute()
            Dim Result As Object = Nothing
            If (aType.GetCustomAttributes(GetType(FlagsAttribute), False).Length <= 0) Then
                Id = Nothing
                num = 0
                fields = aType.GetFields()
                If (fields IsNot Nothing) Then
                    While num < fields.Length
                        fi = fields(num)
                        attribs = TryCast(fi.GetCustomAttributes(GetType(AstmEnumAttribute), False), AstmEnumAttribute())
                        If (attribs.Length > 0) Then
                            Id = attribs(0).Id
                        End If
                        If (String.Compare(Id, aString, True) = 0) Then
                            Return [Enum].Parse(aType, fi.Name)
                        End If
                        num = num + 1
                    End While
                End If
            Else
                Id = String.Empty
                Dim enumStringValue As String = Nothing
                num = 0
                fields = aType.GetFields()
                If (fields IsNot Nothing) Then
                    While num < fields.Length
                        fi = fields(num)
                        attribs = TryCast(fi.GetCustomAttributes(GetType(AstmEnumAttribute), False), AstmEnumAttribute())
                        If (attribs.Length > 0) Then
                            enumStringValue = attribs(0).Id
                        End If
                        If (aString IsNot Nothing) Then
                            Dim enumerator As CharEnumerator = aString.GetEnumerator()
                            If (enumerator IsNot Nothing) Then
                                Try
                                    While enumerator.MoveNext()
                                        Dim ch As Char = enumerator.Current
                                        If (String.Compare(enumStringValue, New String(ch, 1), True) = 0) Then
                                            Id = String.Concat(String.Concat(Id, fi.Name), ",")
                                        End If
                                    End While
                                Finally
                                    enumerator.Dispose()
                                End Try
                            End If
                        End If
                        num = num + 1
                    End While
                End If
                If (Id.Length > 0) Then
                    Id = Id.Remove(Id.Length - 1, 1)
                    Return [Enum].Parse(aType, Id)
                End If
            End If
            Return Result
        End Function

        Private Function CreatePartialRecord(ByVal aString As String, ByVal aType As Type) As AbstractPartialRecord
            Return DirectCast(Activator.CreateInstance(aType, New Object() {aString}), AbstractPartialRecord)
        End Function

        Private Function EscapeString(ByVal aString As String, ByVal aPartialRecord As Boolean) As String
            Dim Result As String = aString
            Result = Result.Replace(New String(EscapeCharacter, 1), String.Concat(String.Concat(New String(EscapeCharacter, 1), "E"), New String(EscapeCharacter, 1)))
            Result = Result.Replace(New String(FieldDelimiter, 1), String.Concat(String.Concat(New String(EscapeCharacter, 1), "F"), New String(EscapeCharacter, 1)))
            If (aPartialRecord) Then
                Result = Result.Replace(New String(ComponentDelimiter, 1), String.Concat(String.Concat(New String(EscapeCharacter, 1), "S"), New String(EscapeCharacter, 1)))
            End If
            Return Result
        End Function

        Private Function GetEnumAstmString(ByVal aEnum As Object) As String
            Dim fi As FieldInfo
            Dim attribs As AstmEnumAttribute()
            Dim type As System.Type
            Dim Id As String
            Dim obj As Object = aEnum
            If (obj IsNot Nothing) Then
                type = obj.[GetType]()
            Else
                type = Nothing
            End If
            Dim enumType As System.Type = type
            If (enumType.GetCustomAttributes(GetType(FlagsAttribute), False).Length <= 0) Then
                fi = enumType.GetField(aEnum.ToString())
                attribs = TryCast(fi.GetCustomAttributes(GetType(AstmEnumAttribute), False), AstmEnumAttribute())
                If (attribs.Length <= 0) Then
                    Id = Nothing
                Else
                    Id = attribs(0).Id
                End If
                Return Id
            End If
            Dim Result As String = String.Empty
            Dim inputVals As String() = aEnum.ToString().Split(New Char() {","c})
            Dim enumValues As FieldInfo() = enumType.GetFields()
            Dim num As Integer = 0
            Dim fieldInfoArray As FieldInfo() = enumValues
            If (fieldInfoArray IsNot Nothing) Then
                While num < fieldInfoArray.Length
                    fi = fieldInfoArray(num)
                    attribs = TryCast(fi.GetCustomAttributes(GetType(AstmEnumAttribute), False), AstmEnumAttribute())
                    If (attribs.Length > 0) Then
                        Dim num1 As Integer = 0
                        Dim strArrays As String() = inputVals
                        If (strArrays IsNot Nothing) Then
                            While num1 < strArrays.Length
                                If (strArrays(num1).Trim() = fi.Name) Then
                                    Result = String.Concat(Result, attribs(0).Id)
                                End If
                                num1 = num1 + 1
                            End While
                        End If
                    End If
                    num = num + 1
                End While
            End If
            Return Result

        End Function

        Private Function RemoveOptionalPartialFields(ByVal aString As String) As String
            If (Not aString.Contains(New String(ComponentDelimiter, 1))) Then
                Return aString
            End If
            Return aString.Split(New Char() {ComponentDelimiter})(0)
        End Function

    End Class

End Namespace