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

End Namespace