Imports System.Linq.Expressions

Namespace Database.Infrastrutture

    Public Enum TipiOrderBy
        [Default]
        DESC
    End Enum

    Public Class SortSQL(Of T As Class)

        Private _fieldName As String
        Private _tableName As String
        Private _sortSql As String

        Public Sub New(sortExp As Expression(Of Func(Of T, String)), ordine As TipiOrderBy)

            _fieldName = sortExp.GetName

            _tableName = TableNameModel(Of T).Get()

            _sortSql = String.Format("[{0}].{1} {2}", _tableName, _fieldName, If(ordine = TipiOrderBy.Default, "", ordine))

        End Sub

        Public Function GetSQL() As String

            Return _sortSql

        End Function

    End Class

End Namespace
