Imports System.Linq.Expressions
Imports Dapper

Namespace Database.Infrastrutture

    Public Interface IRepository(Of T As Class)

        Function GetAll() As List(Of T)
        Function FindById(Id As Integer) As T
        Function Add(customer As T) As Integer
        Function Update(customer As T) As Boolean
        Function Delete(Id As Integer) As Boolean

        Function GetForSelect(Optional keyField As String = Nothing, Optional valueField As String = Nothing, Optional allText As String = Nothing) As Dictionary(Of Integer, String)

    End Interface

    Public Class KeyValue
        Public key As Object
        Public value As Object
    End Class

    Public Class SortInfo
        Public campo As String
        Public crescente As Boolean
    End Class

    Public Class WhereInfo
        Public campo As String
        Public valore As Object
        Public [like] As Boolean
    End Class

End Namespace