Imports System.Reflection
Imports Dapper
Imports Database.Infrastrutture.Attributi

Namespace Database.Infrastrutture

    Public Class GeneraSql(Of T As Class)

        Private ReadOnly Property tabella() As String
        Private ReadOnly props As List(Of PropertyInfo)
        Private ReadOnly propId As PropertyInfo

        Public Sub New(tableName As String)
            _tabella = tableName

            ' ... tipo classe
            Dim tipo As Type = GetType(T)

            ' ... inizializza elenco propertyInfo esclusa la primaryKey
            props = New List(Of PropertyInfo)

            ' ... propertyInfo dei campi
            props = New List(Of PropertyInfo)
            For Each p As PropertyInfo In tipo.GetRuntimeProperties
                If Metodi.GetAttributeFrom(Of PrimaryKey)(tipo, p.Name) Is Nothing Then
                    props.Add(p)
                Else
                    propId = p
                End If
            Next

        End Sub

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

        Public Function sqlFindById() As String
            Return String.Format("SELECT * FROM {0} WHERE {1}=@{1};", tabella, propId.Name)
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

            Return String.Format("INSERT INTO {0} ({1}) VALUES ({2});", tabella, campi, params)

        End Function

        Public Function sqlUpdate() As String

            Dim params As String = String.Empty
            For Each p As PropertyInfo In props
                If params.Length > 0 Then params += ","
                params += String.Format("{0}=@{0}", p.Name)
            Next
            params += String.Format(" WHERE {0}=@{0}", propId.Name)

            Return String.Format("UPDATE {0} SET {1};", tabella, params)

        End Function

        Public Function sqlDelete() As String
            Return String.Format("DELETE FROM {0} WHERE {1}=@{1};", tabella, propId.Name)
        End Function

        Public Function sqlGetAll(sortExpression As List(Of SortInfo)) As String
            Return String.Format("SELECT * FROM {0}{1};", tabella, sortClause(sortExpression))
        End Function

        Public Function sqlGetCount(whereExpression As List(Of WhereInfo)) As String
            Return String.Format("SELECT COUNT({0}) FROM {1} {2};", propId.Name, tabella, whereClause(whereExpression))
        End Function

        Public Function sqlGetFilter(WhereExpression As List(Of WhereInfo), sortExpression As List(Of SortInfo)) As String
            'If WhereExpression Is Nothing OrElse WhereExpression.Count = 0 Then Return sqlGetAll(sortExpression)
            Return String.Format("{0}{1}{2};", sqlGetAll(Nothing).TrimEnd(";"), whereClause(WhereExpression), sortClause(sortExpression))
        End Function

        Public Function sqlGetPage(pagina As Integer, righePerPagina As Integer, WhereExpression As List(Of WhereInfo), sortExpression As List(Of SortInfo)) As String
            Return String.Format("{0} OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY;", sqlGetFilter(WhereExpression, sortExpression).TrimEnd(";"), (pagina - 1) * righePerPagina, righePerPagina)
        End Function

        Private Function sortClause(sortExpression As List(Of SortInfo)) As String
            Dim se As String = String.Empty
            If sortExpression IsNot Nothing AndAlso sortExpression.Count > 0 Then
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
            If whereExpression IsNot Nothing AndAlso whereExpression.Count > 0 Then
                For Each si As WhereInfo In whereExpression
                    If se.Length > 0 Then se += " AND "
                    se += String.Format("{0} {1} @{0}", si.campo, If(si.like, "LIKE", "="))
                Next
                se = " WHERE " + se
            End If
            Return se
        End Function

        Public Function whereArgs(whereExpression As List(Of WhereInfo)) As DynamicParameters
            Dim se As New DynamicParameters
            If whereExpression IsNot Nothing AndAlso whereExpression.Count > 0 Then
                For Each si As WhereInfo In whereExpression
                    se.Add("@" + si.campo, si.valore)
                Next
            End If
            Return se
        End Function


    End Class

End Namespace