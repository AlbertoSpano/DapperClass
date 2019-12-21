Imports System.Linq.Expressions

Namespace Database.Infrastrutture

    Public Enum TipiWhere
        [NOTHING]
        [AND]
        [OR]
        [NOT]
    End Enum

    Public Class WhereSQL(Of T1 As Class)

        Private fieldName As String
        Private tableName As String

        Public ReadOnly Property ParamName As String

        Public ReadOnly Property ParamValue As Object

        Public ReadOnly Property Clause As String

        Public Sub New(exp1 As Expression(Of Func(Of T1, Boolean)))

            Dim expr As Espressione = GetBoolExpression(exp1)
            fieldName = expr.FieldName

            tableName = TableNameModel(Of T1).Get

            ParamName = String.Format("@{0}", fieldName)
            ParamValue = expr.Value
            Clause = String.Format("[{0}].{1} {2} {3}", tableName, fieldName, expr.Operation, ParamName)

        End Sub

    End Class

End Namespace
