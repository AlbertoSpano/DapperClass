Imports System.Linq.Expressions
Imports System.Reflection
Imports Dapper

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

        Public Sub New(Optional tableName As String = Nothing)

            ' ... classe con proprietà della classe T
            propGet = New PropertyGet(Of T)

            If tableName Is Nothing Then tableName = propGet.tableName
            _tabella = tableName

            join = Nothing

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
            Return String.Format("SELECT * FROM {0};", tabella)
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
        Private join As String = Nothing
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

#Region " SELECT "

        Public Function [Select]() As GeneraSql(Of T)

            Dim s As New SelectSQL(Of T)

            sel += If(String.IsNullOrEmpty(sel), "", ",") + s.GetSql

            Return Me

        End Function

        Public Function [Select](Of TT As Class)() As GeneraSql(Of T)

            Dim s As New SelectSQL(Of TT)

            sel += If(String.IsNullOrEmpty(sel), "", ",") + s.GetSql

            Return Me

        End Function

        Public Function [Select](Of TT1 As Class, TT2 As Class)() As GeneraSql(Of T)

            Dim s As New SelectSQL(Of TT1, TT2)

            sel += If(String.IsNullOrEmpty(sel), "", ",") + s.GetSql

            Return Me

        End Function

        Public Function [Select](Of TT1 As Class, TT2 As Class, TT3 As Class)() As GeneraSql(Of T)

            Dim s As New SelectSQL(Of TT1, TT2, TT3)

            sel += If(String.IsNullOrEmpty(sel), "", ",") + s.GetSql

            Return Me

        End Function

        Public Function [Select](Of TT1 As Class, TT2 As Class, TT3 As Class, TT4 As Class)() As GeneraSql(Of T)

            Dim s As New SelectSQL(Of TT1, TT2, TT3, TT4)

            sel += If(String.IsNullOrEmpty(sel), "", ",") + s.GetSql

            Return Me

        End Function


        Public Function [Select](Of TT1 As Class, TT2 As Class, TT3 As Class, TT4 As Class, TT5 As Class)() As GeneraSql(Of T)

            Dim s As New SelectSQL(Of TT1, TT2, TT3, TT4, TT5)

            sel += If(String.IsNullOrEmpty(sel), "", ",") + s.GetSql

            Return Me

        End Function

#End Region

#Region " JOIN "

        Public Function JoinNested(tipoJoin As TipiJoin, sqlNested As String, [Alias] As String, colPrincipal_ON As String, colSecondary_ON As String) As GeneraSql(Of T)

            If nj.Length > 0 Then Throw New Exception("Nested Join già impostato!")

            nj = sqlNested

            nj = nj.TrimStart("(")
            nj = nj.TrimEnd(")")

            nj = String.Format("{0} JOIN ({1}) AS {2} ON {3}.{4}={2}.{5}", tipoJoin, nj, [Alias], propGet.tableName, colPrincipal_ON, colSecondary_ON)

            Return Me

        End Function

        Private Function JoinBase(Of TPK As Class, TFK As Class)(pkCol As Expression(Of Func(Of TPK, String)),
                                                                fkCol As Expression(Of Func(Of TFK, String)),
                                                                tipoJoin As TipiJoin) As GeneraSql(Of T)

            Dim j As New JoinSQL(Of TPK, TFK)(pkCol, fkCol)

            join = j.GetSQL(tipoJoin, join)

            Return Me

        End Function

        Public Function Inner(Of TPK As Class, TFK As Class)(pkCol As Expression(Of Func(Of TPK, String)), fkCol As Expression(Of Func(Of TFK, String))) As GeneraSql(Of T)

            Return JoinBase(Of TPK, TFK)(pkCol, fkCol, TipiJoin.INNER)

        End Function

        Public Function Right(Of TPK As Class, TFK As Class)(pkCol As Expression(Of Func(Of TPK, String)), fkCol As Expression(Of Func(Of TFK, String))) As GeneraSql(Of T)

            Return JoinBase(Of TPK, TFK)(pkCol, fkCol, TipiJoin.RIGHT)

        End Function

        Public Function Left(Of TPK As Class, TFK As Class)(pkCol As Expression(Of Func(Of TPK, String)), fkCol As Expression(Of Func(Of TFK, String))) As GeneraSql(Of T)

            Return JoinBase(Of TPK, TFK)(pkCol, fkCol, TipiJoin.LEFT)

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

#End Region

#Region " WHERE "

        Public Function Where(w1 As Expression(Of Func(Of T, Boolean))) As GeneraSql(Of T)

            'Dim w As New WhereSQL(Of T)(w1)

            'wh += w.Clause

            'If Not String.IsNullOrEmpty(w.ParamName) Then params.Add(w.ParamName, w.ParamValue)

            'Return Me

            Return WhereBase(Of T)(w1)

        End Function

        Public Function WhereAND(w1 As Expression(Of Func(Of T, Boolean))) As GeneraSql(Of T)

            'Dim w As New WhereSQL(Of T)(w1)

            'wh = String.Format("({0} AND {1})", wh, w.Clause)

            'If Not String.IsNullOrEmpty(w.ParamName) Then params.Add(w.ParamName, w.ParamValue)

            'Return Me

            Return WhereBase(Of T)(w1, TipiWhere.AND)

        End Function

        Public Function WhereOR(w1 As Expression(Of Func(Of T, Boolean))) As GeneraSql(Of T)

            'Dim w As New WhereSQL(Of T)(w1)

            'wh = String.Format("({0} OR {1})", wh, w.Clause)

            'If Not String.IsNullOrEmpty(w.ParamName) Then params.Add(w.ParamName, w.ParamValue)

            'Return Me

            Return WhereBase(Of T)(w1, TipiWhere.OR)

        End Function

        Public Function Where(Of T1 As Class)(w1 As Expression(Of Func(Of T1, Boolean))) As GeneraSql(Of T)

            'Dim w As New WhereSQL(Of T1)(w1)

            'wh += w.Clause

            'If Not String.IsNullOrEmpty(w.ParamName) Then params.Add(w.ParamName, w.ParamValue)

            'Return Me

            Return WhereBase(Of T1)(w1)

        End Function

        Private Function WhereBase(Of T1 As Class)(w1 As Expression(Of Func(Of T1, Boolean)), Optional tipo As TipiWhere = Nothing) As GeneraSql(Of T)

            Dim w As New WhereSQL(Of T1)(w1)

            Select Case tipo

                Case TipiWhere.NOTHING
                    wh = w.Clause

                Case TipiWhere.AND, TipiWhere.OR
                    If String.IsNullOrEmpty(wh) Then
                        Throw New Exception("Manca operatore AND o OR!")
                    Else
                        wh = String.Format("({0} {1} {2})", wh, tipo, w.Clause)
                    End If

                Case TipiWhere.NOT
                    wh = String.Format("({0} {1} {2})", wh, tipo, w.Clause)

            End Select

            If Not String.IsNullOrEmpty(w.ParamName) Then params.Add(w.ParamName, w.ParamValue)

            Return Me

        End Function

#End Region

#Region " SORT "

        Public Function SortBy(Of TS As Class)(sortExp As Expression(Of Func(Of TS, String)), Optional ordine As String = "") As GeneraSql(Of T)

            Dim s As New SortSQL(Of TS)(sortExp, ordine)

            sh += If(String.IsNullOrEmpty(sh), "", ",") + s.GetSQL()

            Return Me

        End Function

        Public Function SortBy(sortClause As String) As GeneraSql(Of T)

            sh += If(String.IsNullOrEmpty(sh), "", ",") + String.Format("{0}", sortClause)

            Return Me

        End Function

#End Region

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

        Public Function Where(clause As String) As GeneraSql(Of T)

            ' ... condizione where
            wh = clause

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
                campo = String.Format("[{0}].{1}", tabella, campo)
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
            If String.IsNullOrEmpty(sel) Then sel = String.Format("[{0}].*", propGet.tableName)
            If String.IsNullOrEmpty(join) Then join = propGet.tableName
            If wh.Length > 0 Then wh = If(wh.Contains("WHERE"), wh, " WHERE " + wh)
            If sh.Length > 0 Then sh = If(sh.Contains("ORDER BY"), sh, " ORDER BY " + sh)
            If gb.Length > 0 Then gb = If(gb.Contains("GROUP BY"), gb, " GROUP BY " + gb)
            Return String.Format("SELECT {0} FROM {1} {2} {3} {4} {5} {6};", sel, join, nj, wh, gb, sh, ph)
        End Function

        Public Function GetSqlCount() As String
            If join.Length = 0 Then join = propGet.tableName
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
                Throw New Exception(String.Format("La colonna {0} non appartiene alla classe {1}!", colPrincipal, propGet.tableName))
            End If

            If pJoin.propsAll.FirstOrDefault(Function(x) String.Compare(x.Name, colSecondary, True) = 0) Is Nothing Then
                Throw New Exception(String.Format("La colonna {0} non appartiene alla classe {1}!", colSecondary, pJoin.tableName))
            End If

            ' ... genera lista campi select
            If sel.Length = 0 Then
                If colsPrincipal Is Nothing Then
                    sel = String.Format("[{0}].*", propGet.tableName)
                Else
                    For Each c As String In propGet.propsAll.Select(Function(x) x.Name) '.Where(Function(x) String.Compare(x.Name, colPrincipal, True) <> 0).Select(Function(x) x.Name)
                        sel += If(sel.Length = 0, "", ",")
                        sel += String.Format("[{0}].{1}", propGet.tableName, c)
                    Next
                End If
            End If
            If colsSecondary Is Nothing Then
                sel += If(sel.Length = 0, "", ",")
                sel += String.Format("[{0}].*", pJoin.tableName)
            Else
                For Each c As String In pJoin.propsAll.Select(Function(x) x.Name) '.Where(Function(x) String.Compare(x.Name, colSecondary, True) <> 0).Select(Function(x) x.Name)
                    sel += If(sel.Length = 0, "", ",")
                    sel += String.Format("[{0}].{1}", pJoin.tableName, c)
                Next
            End If

            ' ... genera il join
            If String.IsNullOrEmpty(join) Then join = propGet.tableName
            join += String.Format(" {1} JOIN [{2}] ON [{0}].{3} = [{2}].{4}", propGet.tableName, tipojOIN, pJoin.tableName, colPrincipal, colSecondary)
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

    Public Class TableColumn
        Public Property tableName As String
        Public Property column As String
        Public Property isPrimary As Boolean
    End Class

End Namespace