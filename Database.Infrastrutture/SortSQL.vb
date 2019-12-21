Imports System.Linq.Expressions
Imports Database.Infrastrutture
Imports Database.Infrastrutture.Attributi

Public Class SortSQL(Of T As Class)

    Private _fieldName As String
    Private _tableName As String
    Private _sortSql As String

    Public Sub New(sortExp As Expression(Of Func(Of T, String)), ordine As String)

        _fieldName = sortExp.GetName

        Dim tbAttribute As TableNameAttribute = CType(Attribute.GetCustomAttribute(GetType(T), GetType(TableNameAttribute)), TableNameAttribute)
        _tableName = If(tbAttribute Is Nothing, GetType(T).Name, tbAttribute.Name)

        _sortSql = String.Format("[{0}].{1} {2}", _tableName, _fieldName, ordine)

    End Sub

    Public Function GetSQL() As String

        Return _sortSql

    End Function

End Class
