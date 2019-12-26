Imports System.Linq.Expressions

Namespace Database.Infrastrutture

    Public Enum TipiWhere
        [NOTHING]
        [AND]
        [OR]
        [NOT]
    End Enum

    Public Class WhereSQL(Of T1 As Class)

        Private variableName As String
        Private fieldName As String
        Private tableName As String
        Private methodName As String

        Public ReadOnly Property ParamName As String

        Public ReadOnly Property ParamValue As Object

        Public ReadOnly Property Clause As String

        Public Sub New(exp1 As Expression(Of Func(Of T1, Boolean)))

            Dim expr As ExpressionParams = GetBoolExpression(exp1)
            fieldName = expr.FieldName
            methodName = expr.MethodName
            variableName = expr.VariableName

            tableName = TableNameModel(Of T1).Get

            ParamName = String.Format("@{0}", If(variableName, fieldName))
            ParamValue = expr.Value

            If methodName Is Nothing Then
                Clause = String.Format("[{0}].{1} {2} {3}", tableName, fieldName, expr.Operation, ParamName)
            Else
                Clause = String.Format("{4}([{0}].{1}) {2} {3}", tableName, fieldName, expr.Operation, ParamName, methodName)
            End If

        End Sub

    End Class

End Namespace
