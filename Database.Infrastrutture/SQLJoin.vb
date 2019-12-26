Imports System.Linq.Expressions
Imports Database.Infrastrutture

Namespace Database.Infrastrutture

    Public Enum TipiJoin
        INNER
        LEFT
        RIGHT
    End Enum

    Public Class JoinSQL(Of TPK As Class, TFK As Class)

        Private _pkTableName As String
        Private _fkTableName As String
        Private _pkColumnName As String
        Private _fkColumnName As String

        Public Sub New(pkCol As Expression(Of Func(Of TPK, String)), fkCol As Expression(Of Func(Of TFK, String)))

            _pkTableName = TableNameModel(Of TPK).Get
            _fkTableName = TableNameModel(Of TFK).Get

            _fkColumnName = fkCol.GetParams.FieldName
            _pkColumnName = pkCol.GetParams.FieldName

        End Sub

        Public Function GetSQL(tipoJoin As TipiJoin, Optional partialJoin As String = Nothing) As String
            If String.IsNullOrEmpty(partialJoin) Then
                Return String.Format("([{0}] {1} JOIN [{2}] ON [{0}].{3} = [{2}].{4})", _pkTableName, tipoJoin, _fkTableName, _pkColumnName, _fkColumnName)
            Else
                Return String.Format("({5} {1} JOIN [{2}] ON [{0}].{3} = [{2}].{4})", _pkTableName, tipoJoin, _fkTableName, _pkColumnName, _fkColumnName, partialJoin)
            End If
        End Function

    End Class

End Namespace
