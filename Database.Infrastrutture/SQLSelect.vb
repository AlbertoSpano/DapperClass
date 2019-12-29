Imports System.Linq.Expressions
Imports System.Reflection

Namespace Database.Infrastrutture

    Public Enum AggregateFunction
        [NOTHING]
        AVG
        COUNT
        MAX
        MIN
        SUM
    End Enum

    Public Class SelectSQL(Of T As Class)

        Private tableName As String
        Private variableName As String
        Private fieldName As String
        Private methodName As String
        Private col As Expression(Of Func(Of T, String))
        Private aliasName As String
        Private aggregateFunction As AggregateFunction

        Public Sub New(Optional col As Expression(Of Func(Of T, String)) = Nothing, Optional AggregateFunction As AggregateFunction = AggregateFunction.NOTHING, Optional AliasName As String = Nothing)

            tableName = TableNameModel(Of T).Get

            Me.aliasName = AliasName

            Me.col = col

            Me.aggregateFunction = AggregateFunction

        End Sub

        Public Function GetSql() As String

            If col IsNot Nothing Then Return ColSql()

            Dim prop As New PropertyGet(Of T)

            Dim sel As String = String.Empty
            Dim virgola As String = String.Empty

            ' ... rinomina i campi chiave esterna secondo la codifica <nomeTabella><nomeCampo>
            If prop.fkList.Count = 0 And prop.mustMappedList.Count = 0 Then
                sel += String.Format("{1} [{0}].*", tableName, virgola)
                virgola = ","
            Else
                For Each p As PropertyInfo In prop.propsAll
                    ' ... exclude readonly fields
                    If Not p.CanWrite Then Continue For
                    ' .. field name
                    Dim name As String = p.Name
                    If prop.fkList.FirstOrDefault(Function(x) String.Compare(x.FKColumnName, name, True) = 0) IsNot Nothing Then
                        ' .. foreign key
                        name = String.Format("{1} AS {0}{1}", tableName, name)
                    ElseIf prop.mustMappedList.FirstOrDefault(Function(x) String.Compare(x, p.Name, True) = 0) IsNot Nothing Then
                        ' .. rimapped field
                        name = String.Format("{1} AS {0}{1}", tableName, name)
                    End If
                    ' .. selection field
                    sel += String.Format("{2} [{0}].{1}", tableName, name, virgola)
                    virgola = ","
                Next
            End If

            Return sel

        End Function

        Private Function ColSql() As String

            Dim expr As ExpressionParams = GetBoolExpression(col)
            fieldName = expr.FieldName
            methodName = expr.MethodName
            variableName = expr.VariableName

            Dim sql As String = String.Format("[{0}].{1}", tableName, fieldName)

            If methodName IsNot Nothing Then
                sql = String.Format("{0}({1})", methodName, sql)
            End If

            If aggregateFunction <> AggregateFunction.NOTHING Then
                sql = String.Format("{0}({1})", aggregateFunction, sql)
            End If

            If aliasName IsNot Nothing Then
                sql = String.Format("{0} AS {1}", sql, aliasName)
            End If

            Return sql

        End Function

    End Class

    Public Class SelectSQL(Of T1 As Class, T2 As Class)

        Public Function GetSql()

            Dim sel1 As String = (New SelectSQL(Of T1)).GetSql
            Dim sel2 As String = (New SelectSQL(Of T2)).GetSql
            Dim virgola As String = If(Not String.IsNullOrEmpty(sel1) And Not String.IsNullOrEmpty(sel2), ",", String.Empty)

            Return sel1 + virgola + sel2

        End Function

    End Class

    Public Class SelectSQL(Of T1 As Class, T2 As Class, T3 As Class)

        Public Function GetSql()

            Dim sel1 As String = (New SelectSQL(Of T1, T2)).GetSql
            Dim sel2 As String = (New SelectSQL(Of T3)).GetSql
            Dim virgola As String = If(Not String.IsNullOrEmpty(sel1) And Not String.IsNullOrEmpty(sel2), ",", String.Empty)

            Return sel1 + virgola + sel2

        End Function

    End Class

    Public Class SelectSQL(Of T1 As Class, T2 As Class, T3 As Class, T4 As Class)

        Public Function GetSql()

            Dim sel1 As String = (New SelectSQL(Of T1, T2)).GetSql
            Dim sel2 As String = (New SelectSQL(Of T3, T4)).GetSql
            Dim virgola As String = If(Not String.IsNullOrEmpty(sel1) And Not String.IsNullOrEmpty(sel2), ",", String.Empty)

            Return sel1 + virgola + sel2

        End Function

    End Class

    Public Class SelectSQL(Of T1 As Class, T2 As Class, T3 As Class, T4 As Class, T5 As Class)

        Public Function GetSql()

            Dim sel1 As String = (New SelectSQL(Of T1, T2, T3)).GetSql
            Dim sel2 As String = (New SelectSQL(Of T4, T5)).GetSql
            Dim virgola As String = If(Not String.IsNullOrEmpty(sel1) And Not String.IsNullOrEmpty(sel2), ",", String.Empty)

            Return sel1 + virgola + sel2

        End Function

    End Class


End Namespace