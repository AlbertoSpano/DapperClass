Imports Dapper.FluentMap.Mapping
Imports System.Reflection
Imports Database.Infrastrutture
Imports System.Linq.Expressions

Public Class MapBase(Of T As Class)
    Inherits EntityMap(Of T)

    Public Sub New()
        Dim props As New PropertyGet(Of T)
        Dim tableName As String = props.tableName

        For Each p As PropertyInfoEx In props.PropsWithoutId

            ' .. exclude readonly field
            If Not p.CanWrite Then Continue For

            ' .. include only foreign key and mustMapped attribute
            If Not p.IsForegnKey And Not p.IsMustMapped Then Continue For

            ' .. create lambda expression
            Dim parameter As ParameterExpression = Expression.Parameter(GetType(T), "x")
            Dim member As MemberExpression = Expression.Property(parameter, p.Name)
            Dim body As Expression = Expression.Convert(member, GetType(Object))
            Dim finalExpression As Expression = Expression.Lambda(Of Func(Of T, Object))(body, parameter)

            ' .. map fieldName as tablename+fieldName
            Map(finalExpression).ToColumn(tableName + p.Name)

        Next
    End Sub

End Class
