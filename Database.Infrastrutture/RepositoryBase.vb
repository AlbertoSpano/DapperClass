Imports Dapper
Imports System.Linq.Expressions

Namespace Database.Infrastrutture

    Public Class RepositoryBase(Of T As Class) : Implements IRepository(Of T)

        Private tabella As String
        Private gen As GeneraSql(Of T)

        Public cn As IDbConnection

        Public Sub New(conn As IDbConnection, tableName As String)

            cn = conn

            tabella = tableName

            gen = New GeneraSql(Of T)(tableName)

        End Sub

        Public Function argsFindById(Id As Integer) As DynamicParameters Implements IRepository(Of T).argsFindById
            Return gen.argsFindById(Id)
        End Function

        Private Function argsAdd(record As T) As DynamicParameters Implements IRepository(Of T).argsAdd
            Return gen.argsAdd(record)
        End Function

        Private Function argsUpdate(record As T) As DynamicParameters Implements IRepository(Of T).argsUpdate
            Return gen.argsUpdate(record)
        End Function

        Private Function argsDelete(Id As Integer) As DynamicParameters Implements IRepository(Of T).argsDelete
            Return gen.argsDelete(Id)
        End Function

        Public Function sqlFindById() As String Implements IRepository(Of T).sqlFindById
            Return gen.sqlFindById
        End Function

        Private Function sqlAdd() As String Implements IRepository(Of T).sqlAdd
            Return gen.sqlAdd
        End Function

        Private Function sqlUpdate() As String Implements IRepository(Of T).sqlUpdate
            Return gen.sqlUpdate
        End Function

        Private Function sqlDelete() As String Implements IRepository(Of T).sqlDelete
            Return gen.sqlDelete
        End Function

        Private Function sqlGetAll(Optional sortExpression As List(Of SortInfo) = Nothing) As String Implements IRepository(Of T).sqlGetAll
            Return gen.sqlGetAll(sortExpression)
        End Function

        Public Function sqlGetPage(pagina As Integer, righePerPagina As Integer, WhereExpression As List(Of WhereInfo), sortExpression As List(Of SortInfo)) As String Implements IRepository(Of T).sqlGetPage
            Return gen.sqlGetPage(pagina, righePerPagina, WhereExpression, sortExpression)
        End Function

        Public Function sqlGetFilter(WhereExpression As List(Of WhereInfo), sortExpression As List(Of SortInfo)) As String Implements IRepository(Of T).sqlGetFilter
            Return gen.sqlGetFilter(WhereExpression, sortExpression)
        End Function

        Private Function sqlGetCount(whereExpression As List(Of WhereInfo)) As String
            Return gen.sqlGetCount(whereExpression)
        End Function

        Private Function sortClause(sortExpression As List(Of SortInfo)) As String
            Dim se As String = String.Empty
            If sortExpression IsNot Nothing Then
                For Each si As SortInfo In sortExpression
                    If se.Length > 0 Then se += ","
                    se += String.Format("{0}{1}", si.campo, If(si.crescente, "", " DESC"))
                Next
                se = " ORDER BY " + se
            End If
            Return se
        End Function

        Private Function whereClause(whereExpression As List(Of WhereInfo)) As String
            Dim se As String = String.Empty
            If whereExpression IsNot Nothing Then
                For Each si As WhereInfo In whereExpression
                    If se.Length > 0 Then se += " AND "
                    se += String.Format("{0} {1} @{0}", si.campo, If(si.like, "LIKE", "="))
                Next
                se = " WHERE " + se
            End If
            Return se
        End Function

        Private Function whereArgs(whereExpression As List(Of WhereInfo)) As DynamicParameters
            Dim se As New DynamicParameters
            If whereExpression IsNot Nothing Then
                For Each si As WhereInfo In whereExpression
                    se.Add("@" + si.campo, si.valore)
                Next
            End If
            Return se
        End Function

        Public Function View(sqlString As String) As List(Of T)
            Return cn.Query(Of T)(sqlString)
        End Function

        Public Overridable Function GetAll(Optional sortExpression As List(Of SortInfo) = Nothing) As List(Of T) Implements IRepository(Of T).GetAll
            Return cn.Query(Of T)(sqlGetAll(sortExpression))
        End Function

        Public Function GetFilter(WhereExpression As List(Of WhereInfo), sortExpression As List(Of SortInfo)) As List(Of T) Implements IRepository(Of T).GetFilter
            Return cn.Query(Of T)(sqlGetFilter(WhereExpression, sortExpression), whereArgs(WhereExpression))
        End Function

        Public Function GetPage(pagina As Integer, righePerPagina As Integer, WhereExpression As List(Of WhereInfo), sortExpression As List(Of SortInfo)) As List(Of T) Implements IRepository(Of T).GetPage
            Return cn.Query(Of T)(sqlGetPage(pagina, righePerPagina, WhereExpression, sortExpression), whereArgs(WhereExpression))
        End Function

        Public Function GetCount(WhereExpression As List(Of WhereInfo)) As Integer Implements IRepository(Of T).GetCount
            Return cn.Query(Of Integer)(sqlGetCount(WhereExpression), whereArgs(WhereExpression)).First
        End Function

        Public Overridable Function FindById(Id As Integer) As T Implements IRepository(Of T).FindById
            Return cn.Query(Of T)(sqlFindById, argsFindById(Id)).FirstOrDefault
        End Function

        Public Overridable Function Add(recordNew As T) As Integer Implements IRepository(Of T).Add
            Try
                If cn.Execute(sqlAdd, argsAdd(recordNew)) = 1 Then
                    Return cn.Query(Of Integer)("Select @@IDENTITY;").First
                Else
                    Return 0
                End If
            Catch ex As Exception
                MsgBox(ex.Message)
                Return 0
            End Try

        End Function

        Public Overridable Function Update(recordEdit As T) As Boolean Implements IRepository(Of T).Update
            Try
                Return cn.Execute(sqlUpdate, argsUpdate(recordEdit)) = 1
            Catch ex As Exception
                MsgBox(ex.Message)
                Return 0
            End Try
        End Function

        Public Overridable Function Delete(Id As Integer) As Boolean Implements IRepository(Of T).Delete
            Return cn.Execute(sqlDelete, argsDelete(Id)) = 1
        End Function

        Public Overridable Function GetForSelect() As List(Of KeyValue) Implements IRepository(Of T).GetForSelect
            Throw New NotImplementedException()
        End Function

    End Class

End Namespace