Imports Dapper

Namespace Database.Infrastrutture

    Public MustInherit Class RepositoryBase(Of T As Class) : Implements IRepository(Of T)

        Private cn As IDbConnection
        Private tabella As String
        Public gen As GeneraSql(Of T)

        Public Sub New(conn As IDbConnection, tableName As String, Optional sqlBase As String = Nothing)

            cn = conn

            tabella = tableName

            gen = New GeneraSql(Of T)(tableName, sqlBase)

        End Sub

        Private Function argsFindById(Id As Integer) As DynamicParameters
            Return gen.argsFindById(Id)
        End Function

        Private Function argsAdd(record As T) As DynamicParameters
            Return gen.argsAdd(record)
        End Function

        Private Function argsUpdate(record As T) As DynamicParameters
            Return gen.argsUpdate(record)
        End Function

        Private Function argsDelete(Id As Integer) As DynamicParameters
            Return gen.argsDelete(Id)
        End Function
        Private Function sqlFindById() As String
            Return gen.sqlFindById
        End Function

        Private Function sqlAdd() As String
            Return gen.sqlAdd
        End Function

        Private Function sqlUpdate() As String
            Return gen.sqlUpdate
        End Function

        Private Function sqlDelete() As String
            Return gen.sqlDelete
        End Function

        Private Function sqlGetAll() As String
            Return gen.sqlGetAll()
        End Function

        Public Function GetAll() As List(Of T) Implements IRepository(Of T).GetAll
            Return cn.Query(Of T)(sqlGetAll())
        End Function

        Public Function FindById(Id As Integer) As T Implements IRepository(Of T).FindById
            Return cn.Query(Of T)(sqlFindById, argsFindById(Id)).FirstOrDefault
        End Function

        Public Function Add(recordNew As T) As Integer Implements IRepository(Of T).Add
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

        Public Function Update(recordEdit As T) As Boolean Implements IRepository(Of T).Update
            Try
                Return cn.Execute(sqlUpdate, argsUpdate(recordEdit)) = 1
            Catch ex As Exception
                MsgBox(ex.Message)
                Return 0
            End Try
        End Function

        Public Function Delete(Id As Integer) As Boolean Implements IRepository(Of T).Delete
            Return cn.Execute(sqlDelete, argsDelete(Id)) = 1
        End Function

        Public Overridable Function GetForSelect() As List(Of KeyValue) Implements IRepository(Of T).GetForSelect
            Throw New NotImplementedException()
        End Function

        Public MustOverride Function View(sort As List(Of SortInfo), where As List(Of WhereInfo), Optional pagina As Integer = 0, Optional righeperpagina As Integer = 0) As List(Of T)

    End Class

End Namespace