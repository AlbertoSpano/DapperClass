
Imports System.Linq.Expressions
Imports Database.Infrastrutture
Imports Database.Infrastrutture.Attributi

Public Class JoinSQL(Of TPK As Class, TFK As Class)

    Private _pkTableName As String
    Private _fkTableName As String
    Private _pkColumnName As String
    Private _fkColumnName As String

    Public Sub New(pkCol As Expression(Of Func(Of TPK, String)), fkCol As Expression(Of Func(Of TFK, String)))

        Dim tbAttribute As TableNameAttribute

        tbAttribute = CType(Attribute.GetCustomAttribute(GetType(TPK), GetType(TableNameAttribute)), TableNameAttribute)
        _pkTableName = If(tbAttribute Is Nothing, GetType(TPK).Name, tbAttribute.Name)

        tbAttribute = CType(Attribute.GetCustomAttribute(GetType(TFK), GetType(TableNameAttribute)), TableNameAttribute)
        _fkTableName = If(tbAttribute Is Nothing, GetType(TFK).Name, tbAttribute.Name)

        _fkColumnName = fkCol.GetName
        _pkColumnName = pkCol.GetName

    End Sub

    Public Function GetSQL(tipoJoin As TipiJoin, Optional partialJoin As String = Nothing) As String
        If String.IsNullOrEmpty(partialJoin) Then
            Return String.Format("([{0}] {1} JOIN [{2}] ON [{0}].{3} = [{2}].{4})", _pkTableName, tipoJoin, _fkTableName, _pkColumnName, _fkColumnName)
        Else
            Return String.Format("({5} {1} JOIN [{2}] ON [{0}].{3} = [{2}].{4})", _pkTableName, tipoJoin, _fkTableName, _pkColumnName, _fkColumnName, partialJoin)
        End If
    End Function

End Class
