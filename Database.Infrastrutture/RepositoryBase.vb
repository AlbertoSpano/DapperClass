Imports Dapper

Namespace Database.Infrastrutture

    Public MustInherit Class RepositoryBase(Of T As Class) : Implements IRepository(Of T) : Implements IRecord : Implements IPaging

        Private tabella As String
        Public cn As IDbConnection
        Public gen As GeneraSqlCRUD(Of T)

        Public Sub New(conn As IDbConnection)

            cn = conn

            gen = New GeneraSqlCRUD(Of T)

            tabella = gen.tableName

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

        Public MustOverride ReadOnly Property DisplayCols As Dictionary(Of String, String) Implements IRecord.DisplayCols

        Public MustOverride ReadOnly Property DESC_PROP As String Implements IRecord.DESC_PROP

        Public Overridable Function GetForSelect(Optional keyField As String = Nothing, Optional valueField As String = Nothing, Optional allText As String = Nothing) As Dictionary(Of Integer, String) Implements IRepository(Of T).GetForSelect
            allText = If(allText, "...")
            keyField = If(keyField, gen.propId.Name)
            valueField = If(valueField, DESC_PROP)
            Dim sql As String = String.Format("SELECT {0} AS [Key], {1} AS [Value] FROM {2} ORDER BY {1};", keyField, valueField, tabella)
            Return cn.Query(Of KeyValuePair(Of Integer, String))(sql).ToDictionary(Function(row) row.Key, Function(row) row.Value)
        End Function

        Public Sub GetPageCount(pageSize As Integer) Implements IPaging.PageCount

            ' ... numero pagine
            If pageSize = 0 Then
                CurrentPagesCount = 1
            Else
                pageSize = If(pageSize > CurrentRowsCount, CurrentRowsCount, pageSize)
                CurrentPagesCount = (CurrentRowsCount \ pageSize) + If(CurrentRowsCount Mod pageSize = 0, 0, 1)
            End If

        End Sub

        Public Property CurrentRowsCount As Integer Implements IPaging.CurrentRowsCount

        Public Property CurrentPagesCount As Integer Implements IPaging.CurrentPagesCount

    End Class

End Namespace