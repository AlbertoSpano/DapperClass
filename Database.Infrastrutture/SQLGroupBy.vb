Imports System.Linq.Expressions

Namespace Database.Infrastrutture

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

            Dim ret As String

            If _methodName Is Nothing Then
                ret = String.Format("[{0}].{1}", tableName, _fieldName).Trim
            Else
                ret = String.Format("{2}([{0}].{1})", tableName, _fieldName, _methodName).Trim
            End If

            Return String.Format("{0} {1}", ret, AliasName).Trim

        End Function

    End Class

End Namespace