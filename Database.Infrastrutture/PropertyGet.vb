Imports System.Reflection
Imports Database.Infrastrutture.Attributi

Namespace Database.Infrastrutture

    Public Class PropertyGet(Of T As Class)

        Public ReadOnly propsAll As List(Of PropertyInfoEx)
        Public ReadOnly propId As PropertyInfo
        Public ReadOnly tableName As String
        Public ReadOnly mappedPk As String
        Private ReadOnly _mustMappedList As List(Of String)
        Public Property fkList As List(Of FK)

        Public ReadOnly Property mustMappedList As List(Of String)
            Get
                Return propsAll.Where(Function(x) x.IsMustMapped = True).Select(Function(x) x.PropertyInfo.Name).ToList
            End Get
        End Property

        Public ReadOnly Property PropsWithoutId As List(Of PropertyInfoEx)
            Get
                Return propsAll.Where(Function(x) x.IsPrimaryKey = False).ToList
            End Get
        End Property

        Public Sub New()

            ' ... tipo classe
            Dim tipo As Type = GetType(T)

            ' ... tabella
            tableName = TableNameModel(Of T).Get

            ' ... inizializza elenco propertyInfo 
            propsAll = New List(Of PropertyInfoEx)

            ' ... inizializza foreign key
            fkList = New List(Of FK)

            ' ... propertyInfo dei campi
            For Each p As PropertyInfo In tipo.GetProperties.Where(Function(x) x.PropertyType.IsValueType Or x.PropertyType Is GetType(System.String))

                ' ... not mapped
                Dim noMap As NotMapped = GetAttributeFrom(Of NotMapped)(tipo, p.Name)
                If noMap IsNot Nothing Then Continue For

                ' ... campo primario
                Dim pk As PrimaryKey = GetAttributeFrom(Of PrimaryKey)(tipo, p.Name)

                ' ... fkk foreign key
                Dim fkk As ForeignKeyAttribute = GetAttributeFrom(Of ForeignKeyAttribute)(tipo, p.Name)

                ' ... fkk foreign key
                Dim als As MustMappedAttribute = GetAttributeFrom(Of MustMappedAttribute)(tipo, p.Name)

                ' ...
                Dim pNew As New PropertyInfoEx With {
                .PropertyInfo = p,
                .IsPrimaryKey = pk IsNot Nothing,
                .IsForegnKey = fkk IsNot Nothing,
                .IsMustMapped = als IsNot Nothing
                }

                ' ..
                propsAll.Add(pNew)

                ' ..
                If pk IsNot Nothing Then
                    propId = p
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