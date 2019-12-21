Imports Database.Infrastrutture.Attributi

Namespace Database.Infrastrutture
    Public Class TableNameModel(Of T As Class)
        Public Shared Function [Get]() As String
            Dim tbAttribute As TableNameAttribute = CType(Attribute.GetCustomAttribute(GetType(T), GetType(TableNameAttribute)), TableNameAttribute)
            Return If(tbAttribute Is Nothing, GetType(T).Name, tbAttribute.Name)
        End Function
    End Class
End Namespace