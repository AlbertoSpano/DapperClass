Imports System.Linq.Expressions

Namespace Database.Infrastrutture

    Public Class WhereSQL(Of T1 As Class)

        Public ReadOnly Property ParamName As String

        Public ReadOnly Property ParamValue As Object

        Public ReadOnly Property Clause As String

        Public Sub New(exp1 As Expression(Of Func(Of T1, Boolean)))

            Dim expr As ExpressionParams = GetBoolExpression(exp1)
            Dim tableName As String = TableNameModel(Of T1).Get

            If expr.Value Is Nothing Then
                expr.Operation = "Is Null"
            Else
                ParamName = String.Format("@{0}", If(expr.VariableName, expr.FieldName))
                ParamValue = expr.Value
            End If

            If expr.MethodName Is Nothing Then
                Clause = String.Format("[{0}].{1} {2} {3}", tableName, expr.FieldName, expr.Operation, ParamName).Trim
            Else
                Clause = String.Format("{4}([{0}].{1}) {2} {3}", tableName, expr.FieldName, expr.Operation, ParamName, expr.MethodName).Trim
            End If

        End Sub

    End Class

End Namespace
