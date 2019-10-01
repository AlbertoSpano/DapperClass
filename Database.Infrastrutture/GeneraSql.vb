Imports System.Reflection
Imports Dapper
Imports Database.Infrastrutture.Attributi

Namespace Database.Infrastrutture

    Public Enum TipiWhere
        [NOTHING]
        [AND]
        [OR]
        [NOT]
    End Enum

    Public Enum TipiJoin
        INNER
        LEFT
        RIGHT
    End Enum

    Public Class GeneraSql(Of T As Class)

        Public ReadOnly Property tabella() As String
        Private _sqlBase As String
        Private propGet As PropertyGet(Of T)

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

        Public Sub New(Optional tableName As String = Nothing, Optional sqlBase As String = Nothing)

            ' ... classe con proprietà della classe T
            propGet = New PropertyGet(Of T)

            If tableName Is Nothing Then tableName = propGet.name
            _tabella = tableName

            If sqlBase Is Nothing Then sqlBase = String.Format("SELECT * FROM {0};", tabella)
            _sqlBase = sqlBase

            _sqlBase = _sqlBase.TrimEnd(";")

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
            Return String.Format("{0} WHERE {1}=@{1};", _sqlBase, propId.Name)
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

        Public Function sqlGetAll() As String
            Return String.Format("{0};", _sqlBase)
        End Function


        ''' <summary>
        ''' Una volta istanziata la classe, è possibile creare i join (INNER, LEFT, RIGHT)
        ''' tra la tabella dell'istanza e la tabella corrispondente alla classe TJoin
        ''' Una volta creati i join, il metodo GetSql restituisce la sgtringa sql
        ''' Il metodo GetSql riceve i seguenti parametri:
        '''     - where: lista delle condizioni where da applicare
        '''     - sort:  lista delle colonne da ordinare
        '''     - pagina:   il numero di pagina da estrarre
        '''     - pageSize: il numero di righe da restituire
        ''' </summary>
        ''' 
        Private sel As String = String.Empty
        Private join As String = String.Empty
        Private wh As String = String.Empty
        Private params As New DynamicParameters
        Private sh As String = String.Empty
        Private ph As String = String.Empty
        Private nj As String = String.Empty
        Private gb As String = String.Empty

        Public Sub Clear()
            sel = String.Empty
            join = String.Empty
            wh = String.Empty
            params = New DynamicParameters
            sh = String.Empty
            ph = String.Empty
            nj = String.Empty
            gb = String.Empty
        End Sub

        Public Function GroupBy(sqlGroup As String) As GeneraSql(Of T)

            If gb.Length > 0 Then Throw New Exception("Group By già impostato!")

            gb = sqlGroup

            Return Me

        End Function

        Public Function JoinNested(tipoJoin As TipiJoin, sqlNested As String, [Alias] As String, colPrincipal_ON As String, colSecondary_ON As String) As GeneraSql(Of T)

            If nj.Length > 0 Then Throw New Exception("Nested Join già impostato!")

            nj = sqlNested

            nj = nj.TrimStart("(")
            nj = nj.TrimEnd(")")

            nj = String.Format("{0} JOIN ({1}) AS {2} ON {3}.{4}={2}.{5}", tipoJoin, nj, [Alias], propGet.name, colPrincipal_ON, colSecondary_ON)

            Return Me

        End Function

        Public Function InnerJoin(Of TJoin As Class)(Optional colPrincipal_ON As String = Nothing, Optional colSecondary_ON As String = Nothing, Optional colsPrincipal As List(Of String) = Nothing, Optional colsSecondary As List(Of String) = Nothing) As GeneraSql(Of T)

            Dim pJoin As New PropertyGet(Of TJoin)

            colPrincipal_ON = If(colPrincipal_ON, pJoin.propId.Name)
            colSecondary_ON = If(colSecondary_ON, colPrincipal_ON)

            GenericJoin(Of TJoin)(TipiJoin.INNER, colPrincipal_ON, colSecondary_ON, pJoin, colsPrincipal, colsSecondary)

            Return Me

        End Function

        Public Function RightJoin(Of TJoin As Class)(Optional colRight As String = Nothing, Optional colLeft As String = Nothing, Optional colsPrincipal As List(Of String) = Nothing, Optional colsSecondary As List(Of String) = Nothing) As GeneraSql(Of T)

            Dim pJoin As New PropertyGet(Of TJoin)

            colRight = If(colRight, pJoin.propId.Name)
            colLeft = If(colLeft, colRight)

            GenericJoin(Of TJoin)(TipiJoin.RIGHT, colLeft, colRight, pJoin, colsPrincipal, colsSecondary)

            Return Me

        End Function

        Public Function LeftJoin(Of TJoin As Class)(Optional colLeft As String = Nothing, Optional colRight As String = Nothing, Optional colsPrincipal As List(Of String) = Nothing, Optional colsSecondary As List(Of String) = Nothing) As GeneraSql(Of T)

            Dim pJoin As New PropertyGet(Of TJoin)

            colLeft = If(colLeft, pJoin.propId.Name)
            colRight = If(colRight, colLeft)

            GenericJoin(Of TJoin)(TipiJoin.LEFT, colLeft, colRight, pJoin, colsPrincipal, colsSecondary)

            Return Me

        End Function

        Public Function WhereWithParameters(tipoWhere As TipiWhere, whereList As List(Of WhereInfo)) As GeneraSql(Of T)

            If tipoWhere = TipiWhere.NOTHING Then
                wh = String.Empty
                params = New DynamicParameters
            End If

            If whereList IsNot Nothing AndAlso whereList.Count > 0 Then
                For Each si As WhereInfo In whereList
                    ' ... condizione where
                    If wh.Length > 0 Then wh += String.Format(" {0} ", tipoWhere)
                    wh += String.Format("{0} {1} @{2}", si.campo, If(si.like, "LIKE", "="), si.campo.Replace(".", "_"))
                    ' ... elenco parametri
                    params.Add("@" + si.campo.Replace(".", "_"), si.valore)
                Next
                wh = String.Format("({0})", wh)
            End If

            Return Me

        End Function

        Public Function Where(tipoWhere As TipiWhere, campo As String, valore As Object) As GeneraSql(Of T)

            If tipoWhere = TipiWhere.NOTHING Then
                wh = String.Empty
                params = New DynamicParameters
            End If

            Dim delimiter As String = ""
            If TypeOf valore Is String Then
                delimiter = "'"
                valore = valore.ToString.Replace("'", "''")
            End If

            If Not campo.Contains(".") Then
                campo = String.Format("{0}.{1}", tabella, campo)
            End If

            ' ... condizione where
            If wh.Length > 0 Then wh += String.Format(" {0} ", tipoWhere)
            wh += String.Format("{0} = {1}{2}{1}", campo, delimiter, valore)
            wh = String.Format("({0})", wh)

            Return Me

        End Function

        Public Function Sort(sortList As List(Of SortInfo)) As GeneraSql(Of T)

            If sortList IsNot Nothing AndAlso sortList.Count > 0 Then
                For Each si As SortInfo In sortList
                    If sh.Length > 0 Then sh += ","
                    sh += String.Format("{0}{1}", si.campo, If(si.crescente, "", " DESC"))
                Next
            End If

            Return Me

        End Function

        Public Function Paging(pagina As Integer, pageSize As Integer) As GeneraSql(Of T)
            If pagina = 0 Or pageSize = 0 Then Return Me
            ph = String.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", (pagina - 1) * pageSize, pageSize)
            Return Me
        End Function

        Public Function GetSql() As String
            If sel.Length = 0 Then sel = propGet.name + ".*"
            If join.Length = 0 Then join = propGet.name
            If wh.Length > 0 Then wh = If(wh.Contains("WHERE"), wh, " WHERE " + wh)
            If sh.Length > 0 Then sh = If(sh.Contains("ORDER BY"), sh, " ORDER BY " + sh)
            If gb.Length > 0 Then gb = If(gb.Contains("GROUP BY"), gb, " GROUP BY " + gb)
            Return String.Format("SELECT {0} FROM {1} {2} {3} {4} {5} {6};", sel, join, nj, wh, gb, sh, ph)
        End Function

        Public Function GetSqlCount() As String
            If join.Length = 0 Then join = propGet.name
            If wh.Length > 0 Then wh = If(wh.Contains("WHERE"), wh, " WHERE " + wh)
            If gb.Length > 0 Then gb = If(gb.Contains("GROUP BY"), gb, " GROUP BY " + gb)
            Return String.Format("SELECT COUNT(*) FROM {0} {1} {2} {3};", join, nj, wh, gb)
        End Function

        Public Function GetSql(sql As String) As String
            Return String.Format("{0} {1};", sql, ph)
        End Function

        Public Function GetParams() As DynamicParameters
            Return params
        End Function

        Private Sub GenericJoin(Of TJoin As Class)(tipojOIN As TipiJoin, colPrincipal As String, colSecondary As String, pJoin As PropertyGet(Of TJoin), colsPrincipal As List(Of String), colsSecondary As List(Of String))

            If propGet.propsAll.FirstOrDefault(Function(x) String.Compare(x.Name, colPrincipal, True) = 0) Is Nothing Then
                Throw New Exception(String.Format("La colonna {0} non appartiene alla classe {1}!", colPrincipal, propGet.name))
            End If

            If pJoin.propsAll.FirstOrDefault(Function(x) String.Compare(x.Name, colSecondary, True) = 0) Is Nothing Then
                Throw New Exception(String.Format("La colonna {0} non appartiene alla classe {1}!", colSecondary, pJoin.name))
            End If

            ' ... genera lista campi select
            If sel.Length = 0 Then
                If colsPrincipal Is Nothing Then
                    sel = propGet.name + ".*"
                Else
                    For Each c As String In colsPrincipal
                        sel += If(sel.Length = 0, "", ",")
                        sel += String.Format("{0}.{1}", propGet.name, c)
                    Next
                End If
            End If
            If colsSecondary Is Nothing Then
                sel += If(sel.Length = 0, "", ",")
                sel += pJoin.name + ".*"
            Else
                For Each c As String In colsSecondary
                    sel += If(sel.Length = 0, "", ",")
                    sel += String.Format("{0}.{1}", pJoin.name, c)
                Next
            End If

            ' ... genera il join
            If join.Length = 0 Then join = propGet.name
            join += String.Format(" {1} JOIN {2} ON {0}.{3} = {2}.{4}", propGet.name, tipojOIN, pJoin.name, colPrincipal, colSecondary)
            join = "(" + join + ")"

        End Sub

    End Class

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