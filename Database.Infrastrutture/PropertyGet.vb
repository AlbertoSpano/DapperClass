Imports System.Reflection
Imports Database.Infrastrutture.Attributi

Namespace Database.Infrastrutture

    Public Class PropertyGet(Of T As Class)

        Public ReadOnly propsWithoutId As List(Of PropertyInfo)
        Public ReadOnly propsAll As List(Of PropertyInfo)
        Public ReadOnly propId As PropertyInfo
        Public ReadOnly tableName As String

        Public Sub New()

            MyClass.New(Nothing)

        End Sub

        Public Sub New(IdPropertyName As String)

            ' ... tipo classe
            Dim tipo As Type = GetType(T)

            ' ... nome
            Dim tbAttribute As TableNameAttribute = CType(Attribute.GetCustomAttribute(tipo, GetType(TableNameAttribute)), TableNameAttribute)
            tableName = If(tbAttribute Is Nothing, tipo.Name, tbAttribute.Name)

            ' ... inizializza elenco propertyInfo esclusa la primaryKey
            propsWithoutId = New List(Of PropertyInfo)

            ' ... inizializza elenco propertyInfo 
            propsAll = New List(Of PropertyInfo)

            ' ... propertyInfo dei campi
            For Each p As PropertyInfo In tipo.GetProperties.Where(Function(x) x.PropertyType.IsValueType Or x.PropertyType Is GetType(System.String))
                propsAll.Add(p)
                ' ..
                If IdPropertyName Is Nothing Then
                    If Metodi.GetAttributeFrom(Of PrimaryKey)(tipo, p.Name) Is Nothing Then
                        propsWithoutId.Add(p)
                    Else
                        propId = p
                    End If
                Else
                    propId = tipo.GetProperty(IdPropertyName, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.IgnoreCase)
                End If
            Next

        End Sub

    End Class

End Namespace