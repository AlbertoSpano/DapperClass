Imports System.Linq.Expressions
Imports Database.Infrastrutture
Imports Database.Infrastrutture.Attributi

Public Class WhereSQL(Of T1 As Class)

    Private fieldName As String
    Private tableName As String
    Private _Clause As String
    Private _ParamName As String
    Private _ParamValue As Object

    Public ReadOnly Property ParamName As String
        Get
            Return _ParamName
        End Get
    End Property

    Public ReadOnly Property ParamValue As Object
        Get
            Return _ParamValue
        End Get
    End Property

    Public ReadOnly Property Clause As String
        Get
            Return _Clause
        End Get
    End Property

    Public Sub New(exp1 As Expression(Of Func(Of T1, Boolean)))

        Dim expr As Espressione = GetBoolExpression(exp1)
        fieldName = expr.FieldName

        Dim tbAttribute As TableNameAttribute = CType(Attribute.GetCustomAttribute(GetType(T1), GetType(TableNameAttribute)), TableNameAttribute)
        tableName = If(tbAttribute Is Nothing, GetType(T1).Name, tbAttribute.Name)

        _ParamName = String.Format("@{0}", fieldName)
        _ParamValue = expr.Value
        _Clause = String.Format("[{0}].{1} {2} {3}", tableName, fieldName, expr.Operation, _ParamName)

    End Sub

End Class
