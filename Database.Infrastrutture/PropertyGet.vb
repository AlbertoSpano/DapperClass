Imports System.Reflection
Imports Database.Infrastrutture.Attributi

Namespace Database.Infrastrutture

    Public Class PropertyGet(Of T As Class)

        Public ReadOnly propsWithoutId As List(Of PropertyInfo)
        Public ReadOnly propsAll As List(Of PropertyInfo)
        Public ReadOnly propId As PropertyInfo
        Public ReadOnly tableName As String
        Public ReadOnly mappedPk As String
        Public Property fkList As List(Of FK)
        Public Property mustMappedList As List(Of String)

        Public Sub New()

            ' ... tipo classe
            Dim tipo As Type = GetType(T)

            ' ... tabella
            Dim tbAttribute As TableNameAttribute = CType(Attribute.GetCustomAttribute(tipo, GetType(TableNameAttribute)), TableNameAttribute)
            tableName = If(tbAttribute Is Nothing, tipo.Name, tbAttribute.Name)

            ' ... inizializza elenco propertyInfo esclusa la primaryKey
            propsWithoutId = New List(Of PropertyInfo)

            ' ... inizializza elenco propertyInfo 
            propsAll = New List(Of PropertyInfo)

            ' ... inizializza foreign key
            fkList = New List(Of FK)

            ' ... inizializza gli alias
            mustMappedList = New List(Of String)

            ' ... propertyInfo dei campi
            For Each p As PropertyInfo In tipo.GetProperties.Where(Function(x) x.PropertyType.IsValueType Or x.PropertyType Is GetType(System.String))

                ' ... not mapped
                Dim noMap As NotMapped = GetAttributeFrom(Of NotMapped)(tipo, p.Name)
                If noMap IsNot Nothing Then Continue For

                ' ..
                propsAll.Add(p)

                ' ... campo primario
                Dim pk As PrimaryKey = GetAttributeFrom(Of PrimaryKey)(tipo, p.Name)

                ' ... fkk foreign key
                Dim fkk As ForeignKeyAttribute = GetAttributeFrom(Of ForeignKeyAttribute)(tipo, p.Name)

                ' ... fkk foreign key
                Dim als As MustMappedAttribute = GetAttributeFrom(Of MustMappedAttribute)(tipo, p.Name)

                ' ..
                If pk Is Nothing Then
                    propsWithoutId.Add(p)
                Else
                    propId = p
                    mappedPk = If(String.IsNullOrEmpty(pk.FieldName), p.Name, pk.FieldName)
                End If

                ' ..
                If fkk IsNot Nothing Then
                    fkList.Add(New FK() With {
                               .PKColumnName = fkk.PKColumnName,
                               .PKTableName = fkk.PKTableName,
                               .FKColumnName = p.Name,
                               .FKTableName = tableName
                               })
                End If

                ' .. alias
                If als IsNot Nothing Then mustMappedList.Add(p.Name)

            Next

        End Sub

    End Class

    Public Class FK
        Public Property PKColumnName As String
        Public Property PKTableName As String
        Public Property FKColumnName As String
        Public Property FKTableName As String
    End Class

End Namespace