Imports System.Linq.Expressions
Imports Database.Infrastrutture

Public Class GroupBySQL(Of T As Class)

    Private _fieldName As String
    Private _methodName As String

    Public tableName As String

    Public Sub New(col As Expression(Of Func(Of T, String)))

        Dim e As ExpressionParams = col.GetParams
        _fieldName = e.FieldName
        _methodName = e.MethodName

        tableName = TableNameModel(Of T).Get()

    End Sub

    Public Function GetSQL(Optional AliasName As String = Nothing) As String

        AliasName = If(AliasName Is Nothing, String.Empty, String.Format("AS {0}", AliasName))

        If _methodName Is Nothing Then
            Return String.Format("[{0}].{1} {2}", tableName, _fieldName, AliasName).Trim
        Else
            Return String.Format("{2}([{0}].{1}) {3}", tableName, _fieldName, _methodName, AliasName).Trim
        End If

    End Function

End Class
