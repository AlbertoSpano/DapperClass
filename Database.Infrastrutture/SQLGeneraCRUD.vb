Imports System.Linq.Expressions
Imports System.Reflection
Imports Dapper

Namespace Database.Infrastrutture

    Public Class GeneraSqlCRUD(Of T As Class)

        Public Property propGet As PropertyGet(Of T)
        Public ReadOnly Property tableName() As String

        Public ReadOnly Property props As List(Of PropertyInfo)
            Get
                Return propGet.propsWithoutId
            End Get
        End Property

        Public ReadOnly Property propId As PropertyInfo
            Get
                Return propGet.propId
            End Get
        End Property

        Public Sub New()

            ' ... classe con proprietà della classe T
            propGet = New PropertyGet(Of T)

            tableName = TableNameModel(Of T).Get

        End Sub

#Region " CRUD parameters "

        Public Function argsFindById(Id As Integer) As DynamicParameters

            Dim args = New DynamicParameters
            args.Add("@" + propId.Name, Id)
            Return args

        End Function

        Public Function argsAdd(record As T) As DynamicParameters

            Dim args = New DynamicParameters
            For Each p As PropertyInfo In props
                args.Add("@" + p.Name, p.GetValue(record))
            Next
            Return args

        End Function

        Public Function argsUpdate(record As T) As DynamicParameters

            Dim args = New DynamicParameters
            For Each p As PropertyInfo In props
                args.Add("@" + p.Name, p.GetValue(record))
            Next
            ' ... id
            args.Add("@" + propId.Name, propId.GetValue(record))
            Return args

        End Function

        Public Function argsDelete(Id As Integer) As DynamicParameters

            Dim args = New DynamicParameters
            args.Add("@" + propId.Name, Id)
            Return args

        End Function

#End Region

#Region " CRUD sql "

        Public Function sqlFindById() As String
            Return String.Format("{0} WHERE {1}=@{1};", sqlGetAll.TrimEnd(";"), propId.Name)
        End Function

        Public Function sqlAdd() As String

            Dim campi As String = String.Empty
            Dim params As String = String.Empty
            For Each p As PropertyInfo In props
                If params.Length > 0 Then params += ","
                params += "@" + p.Name
                If campi.Length > 0 Then campi += ","
                campi += p.Name
            Next

            Return String.Format("INSERT INTO {0} ({1}) VALUES ({2});", tableName, campi, params)

        End Function

        Public Function sqlUpdate() As String

            Dim params As String = String.Empty
            For Each p As PropertyInfo In props
                If params.Length > 0 Then params += ","
                params += String.Format("{0}=@{0}", p.Name)
            Next
            params += String.Format(" WHERE {0}=@{0}", propId.Name)

            Return String.Format("UPDATE {0} SET {1};", tableName, params)

        End Function

        Public Function sqlDelete() As String
            Return String.Format("DELETE FROM {0} WHERE {1}=@{1};", tableName, propId.Name)
        End Function

        Public Function sqlGetAll() As String
            Return String.Format("SELECT * FROM {0};", tableName)
        End Function

#End Region


    End Class

End Namespace