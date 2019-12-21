Imports System.Linq.Expressions
Imports Database.Infrastrutture

Public Class GroupBySQL(Of T As Class)

    Private _fieldName As String
    Private _tableName As String
    Private _sortSql As String

    Public Sub New(col As Expression(Of Func(Of T, String)))

        _fieldName = col.GetName

        _tableName = TableNameModel(Of T).Get()

    End Sub

    Public Function GetSQL() As String

        Return String.Format("[{0}].{1}", _tableName, _fieldName)

    End Function

End Class
