﻿Imports System.Linq.Expressions

Namespace Database.Infrastrutture

    Public Class SortSQL(Of T As Class)

        Private _fieldName As String
        Private _tableName As String
        Private _sortSql As String

        Public Sub New(sortExp As Expression(Of Func(Of T, String)), ordine As String)

            _fieldName = sortExp.GetName

            _tableName = TableNameModel(Of T).Get()

            _sortSql = String.Format("[{0}].{1} {2}", _tableName, _fieldName, ordine)

        End Sub

        Public Function GetSQL() As String

            Return _sortSql

        End Function

    End Class

End Namespace