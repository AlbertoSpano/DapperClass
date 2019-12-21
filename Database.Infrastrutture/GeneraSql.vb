Imports System.Linq.Expressions
Imports System.Reflection
Imports Dapper

Namespace Database.Infrastrutture

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

        Public Sub New()

            ' ... classe con proprietà della classe T
            propGet = New PropertyGet(Of T)

            _tabella = propGet.tableName

            join = Nothing

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

#End Region


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
        Private params As New DynamicParameters
        Private sel As String = String.Empty
        Private join As String = Nothing
        Private wh As String = String.Empty
        Private sh As String = String.Empty
        Private ph As String = String.Empty
        Private gb As String = String.Empty

#Region " Group By "

        Public Function GroupBy(Of TG As Class)(Col As Expression(Of Func(Of TG, String))) As GeneraSql(Of T)

            Dim gbSel As New GroupBySQL(Of TG)(Col)

            gb += If(String.IsNullOrEmpty(gb), "", ",") + gbSel.GetSQL

            Return Me

        End Function

#End Region

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

#Region " Paging "

        Public Function Paging(pagina As Integer, pageSize As Integer) As GeneraSql(Of T)
            If pagina = 0 Or pageSize = 0 Then Return Me
            ph = String.Format("OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", (pagina - 1) * pageSize, pageSize)
            Return Me
        End Function

#End Region

#Region " Methods "

        Public Sub Clear()
            params = New DynamicParameters
            sel = String.Empty
            join = String.Empty
            wh = String.Empty
            sh = String.Empty
            ph = String.Empty
            gb = String.Empty
        End Sub

        Public Function GetSql() As String

            If String.IsNullOrEmpty(sel) Then sel = String.Format("[{0}].*", propGet.tableName)
            If String.IsNullOrEmpty(join) Then join = propGet.tableName
            If Not String.IsNullOrEmpty(wh) Then wh = If(wh.Contains("WHERE"), wh, "WHERE " + wh.Trim)
            If Not String.IsNullOrEmpty(sh) Then sh = If(sh.Contains("ORDER BY"), sh, "ORDER BY " + sh.Trim)
            If Not String.IsNullOrEmpty(gb) Then gb = If(gb.Contains("GROUP BY"), gb.Trim, "GROUP BY " + gb.Trim)

            Return String.Format("SELECT {0} FROM {1} {2} {3} {4} {5};", sel, join, wh, gb, sh, ph)
        End Function

        Public Function GetSqlCount() As String
            If join.Length = 0 Then join = propGet.tableName
            If wh.Length > 0 Then wh = If(wh.Contains("WHERE"), wh, " WHERE " + wh)
            If gb.Length > 0 Then gb = If(gb.Contains("GROUP BY"), gb, " GROUP BY " + gb)
            Return String.Format("SELECT COUNT(*) FROM {0} {1} {2};", join, wh, gb)
        End Function

        Public Function GetParams() As DynamicParameters
            Return params
        End Function

#End Region

    End Class

End Namespace