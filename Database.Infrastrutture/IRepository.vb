Imports System.Linq.Expressions
Imports Dapper

Namespace Database.Infrastrutture

    Public Interface IRepository(Of T As Class)

        Function argsFindById(Id As Integer) As DynamicParameters
        Function argsAdd(record As T) As DynamicParameters
        Function argsUpdate(record As T) As DynamicParameters
        Function argsDelete(Id As Integer) As DynamicParameters

        Function sqlGetAll(Optional sortExpression As List(Of SortInfo) = Nothing) As String
        Function sqlGetFilter(WhereExpression As List(Of WhereInfo), sortExpression As List(Of SortInfo)) As String
        Function sqlGetPage(pagina As Integer, righePerPagina As Integer, WhereExpression As List(Of WhereInfo), sortExpression As List(Of SortInfo)) As String
        Function sqlFindById() As String
        Function sqlAdd() As String
        Function sqlUpdate() As String
        Function sqlDelete() As String

        Function GetCount(WhereExpression As List(Of WhereInfo)) As Integer
        Function GetAll(Optional sortExpression As List(Of SortInfo) = Nothing) As List(Of T)
        Function GetFilter(WhereExpression As List(Of WhereInfo), sortExpression As List(Of SortInfo)) As List(Of T)
        Function GetPage(pagina As Integer, righePerPagina As Integer, WhereExpression As List(Of WhereInfo), sortExpression As List(Of SortInfo)) As List(Of T)
        Function GetForSelect() As List(Of KeyValue)
        Function FindById(Id As Integer) As T
        Function Add(customer As T) As Integer
        Function Update(customer As T) As Boolean
        Function Delete(Id As Integer) As Boolean

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