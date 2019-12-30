Imports System.Linq.Expressions

Namespace Database.Infrastrutture

    Public Class SortSQL(Of T As Class)

        Private _fieldName As String
        Private _tableName As String
        Private _methodName As String
        Private _ordine As String

        Public Sub New(sortExp As Expression(Of Func(Of T, String)), ordine As TipiOrderBy)

            Dim e As ExpressionParams = sortExp.GetParams
            _fieldName = e.FieldName
            _methodName = e.MethodName

            _tableName = TableNameModel(Of T).Get()

            _ordine = If(ordine = TipiOrderBy.Default, "", ordine.ToString)

        End Sub

        Public Function GetSQL() As String

            If _methodName Is Nothing Then
                Return String.Format("[{0}].{1} {2}", _tableName, _fieldName, _ordine)
            Else
                Return String.Format("{3}([{0}].{1}) {2}", _tableName, _fieldName, _ordine, _methodName)
            End If

        End Function

    End Class

End Namespace
