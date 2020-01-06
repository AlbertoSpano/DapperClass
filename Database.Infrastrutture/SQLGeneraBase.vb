Imports System.Linq.Expressions
Imports System.Reflection
Imports Dapper

Namespace Database.Infrastrutture

    Public Class GeneraSql(Of T As Class)

        Public ReadOnly Property tableName() As String

        Public Sub New()

            Clear()

            tableName = TableNameModel(Of T).Get

        End Sub


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
        Private Property params As New DynamicParameters
        Private Property sel As String = String.Empty
        Private Property join As String = Nothing
        Private Property wh As String = String.Empty
        Private Property hv As String = String.Empty
        Private Property sh As String = String.Empty
        Private Property ph As String = String.Empty
        Private Property gb As String = String.Empty
        Private Property gbFrom As String = String.Empty
        Private Property gbSel As String = String.Empty

#Region " SELECT "

        Public Function [Select]() As GeneraSql(Of T)

            Return [Select](Of T)()

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

        Public Function [Select](col As Expression(Of Func(Of T, String)),
                                 Optional AggregateFunction As AggregateFunction = AggregateFunction.NOTHING,
                                 Optional AliasName As String = Nothing) As GeneraSql(Of T)

            Return [Select](Of T)(col, AggregateFunction:=AggregateFunction, AliasName:=AliasName)

        End Function

        Public Function [Select](Of TT As Class)(col As Expression(Of Func(Of TT, String)),
                                                 Optional AggregateFunction As AggregateFunction = AggregateFunction.NOTHING,
                                                 Optional AliasName As String = Nothing) As GeneraSql(Of T)


            Dim s As New SelectSQL(Of TT)(col, AggregateFunction:=AggregateFunction, AliasName:=AliasName)

            sel += If(String.IsNullOrEmpty(sel), "", ",") + s.GetSql

            Return Me

        End Function

        Public Function RemoveSelectAll() As GeneraSql(Of T)

            sel = String.Empty

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

        Public Function InnerJoin(Of TPK As Class, TFK As Class)(pkCol As Expression(Of Func(Of TPK, String)), fkCol As Expression(Of Func(Of TFK, String))) As GeneraSql(Of T)

            Return JoinBase(Of TPK, TFK)(pkCol, fkCol, TipiJoin.INNER)

        End Function

        Public Function RightJoin(Of TPK As Class, TFK As Class)(pkCol As Expression(Of Func(Of TPK, String)), fkCol As Expression(Of Func(Of TFK, String))) As GeneraSql(Of T)

            Return JoinBase(Of TPK, TFK)(pkCol, fkCol, TipiJoin.RIGHT)

        End Function

        Public Function LeftJoin(Of TPK As Class, TFK As Class)(pkCol As Expression(Of Func(Of TPK, String)), fkCol As Expression(Of Func(Of TFK, String))) As GeneraSql(Of T)

            Return JoinBase(Of TPK, TFK)(pkCol, fkCol, TipiJoin.LEFT)

            Return Me

        End Function

        Public Function RemoveJoinAll() As GeneraSql(Of T)

            join = String.Empty

            Return Me

        End Function

#End Region

#Region " WHERE "

        Public Function Where(w1 As Expression(Of Func(Of T, Boolean))) As GeneraSql(Of T)

            Return WhereBase(Of T)(w1)

        End Function

        Public Function WhereAND(w1 As Expression(Of Func(Of T, Boolean))) As GeneraSql(Of T)

            Return WhereBase(Of T)(w1, TipiWhere.AND)

        End Function

        Public Function WhereOR(w1 As Expression(Of Func(Of T, Boolean))) As GeneraSql(Of T)

            Return WhereBase(Of T)(w1, TipiWhere.OR)

        End Function

        Public Function Where(Of T1 As Class)(w1 As Expression(Of Func(Of T1, Boolean))) As GeneraSql(Of T)

            Return WhereBase(Of T1)(w1)

        End Function

        Public Function WhereAND(Of T1 As Class)(w1 As Expression(Of Func(Of T1, Boolean))) As GeneraSql(Of T)

            Return WhereBase(Of T1)(w1, TipiWhere.AND)

        End Function

        Public Function WhereOR(Of T1 As Class)(w1 As Expression(Of Func(Of T1, Boolean))) As GeneraSql(Of T)

            Return WhereBase(Of T1)(w1, TipiWhere.OR)

        End Function

        Public Function RemoveWhereAll() As GeneraSql(Of T)

            wh = String.Empty

            Return Me

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

#Region " GROUP BY "

        Public Function GroupBy(Col As Expression(Of Func(Of T, String)),
                                Optional AliasName As String = Nothing) As GeneraSql(Of T)

            Return GroupBy(Of T)(Col, AliasName)

        End Function

        Public Function GroupBy(Of TG As Class)(Col As Expression(Of Func(Of TG, String)),
                                                Optional AliasName As String = Nothing) As GeneraSql(Of T)

            Dim gbSql As New GroupBySQL(Of TG)(Col)

            gb += If(String.IsNullOrEmpty(gb), "", ",") + gbSql.GetSQL()

            gbSel += If(String.IsNullOrEmpty(gbSel), "", ",") + String.Format("{0}", gbSql.GetSQL(AliasName))

            If gbFrom.IndexOf(gbSql.tableName) = -1 Then gbFrom += If(String.IsNullOrEmpty(gbFrom), "", ",") + String.Format("{0}", gbSql.tableName)

            Return Me

        End Function

        Public Function RemoveGroupByAll() As GeneraSql(Of T)
            gb = String.Empty
            gbFrom = String.Empty
            Return Me
        End Function

#End Region

#Region " HAVING "

        Public Function Having(w1 As Expression(Of Func(Of T, Boolean)), AggregateFunction As AggregateFunction) As GeneraSql(Of T)

            Return HavingBase(Of T)(w1, AggregateFunction)

        End Function

        Public Function HavingAND(w1 As Expression(Of Func(Of T, Boolean)), AggregateFUnction As AggregateFunction) As GeneraSql(Of T)

            Return HavingBase(Of T)(w1, AggregateFUnction, TipiWhere.AND)

        End Function

        Public Function HavingOR(w1 As Expression(Of Func(Of T, Boolean)), AggregateFUnction As AggregateFunction) As GeneraSql(Of T)

            Return HavingBase(Of T)(w1, AggregateFUnction, TipiWhere.OR)

        End Function

        Public Function Having(Of T1 As Class)(w1 As Expression(Of Func(Of T1, Boolean)), AggregateFUnction As AggregateFunction) As GeneraSql(Of T)

            Return HavingBase(Of T1)(w1, AggregateFUnction)

        End Function

        Public Function HavingAND(Of T1 As Class)(w1 As Expression(Of Func(Of T1, Boolean)), AggregateFUnction As AggregateFunction) As GeneraSql(Of T)

            Return HavingBase(Of T1)(w1, AggregateFUnction, TipiWhere.AND)

        End Function

        Public Function HavingOR(Of T1 As Class)(w1 As Expression(Of Func(Of T1, Boolean)), AggregateFUnction As AggregateFunction) As GeneraSql(Of T)

            Return HavingBase(Of T1)(w1, AggregateFUnction, TipiWhere.OR)

        End Function

        Public Function RemoveHavingAll() As GeneraSql(Of T)

            hv = String.Empty

            Return Me

        End Function

        Private Function HavingBase(Of T1 As Class)(w1 As Expression(Of Func(Of T1, Boolean)), AggregateFUnction As AggregateFunction, Optional tipo As TipiWhere = Nothing) As GeneraSql(Of T)

            Dim h As New HavingSQL(Of T1)(w1, AggregateFUnction)

            Select Case tipo

                Case TipiWhere.NOTHING
                    hv = h.Clause

                Case TipiWhere.AND, TipiWhere.OR
                    If String.IsNullOrEmpty(hv) Then
                        Throw New Exception("Manca operatore AND o OR!")
                    Else
                        hv = String.Format("({0} {1} {2})", hv, tipo, h.Clause)
                    End If

                Case TipiWhere.NOT
                    hv = String.Format("({0} {1} {2})", hv, tipo, h.Clause)

            End Select

            If Not String.IsNullOrEmpty(h.ParamName) Then params.Add(h.ParamName, h.ParamValue)

            Return Me

        End Function

#End Region

#Region " ORDER BY "

        Public Function OrderBy(sortExp As Expression(Of Func(Of T, String)), Optional ordine As TipiOrderBy = TipiOrderBy.Default) As GeneraSql(Of T)

            Return OrderBy(Of T)(sortExp, ordine)

        End Function

        Public Function OrderBy(Of TS As Class)(sortExp As Expression(Of Func(Of TS, String)), Optional ordine As TipiOrderBy = TipiOrderBy.Default) As GeneraSql(Of T)

            Dim s As New SortSQL(Of TS)(sortExp, ordine)

            sh += If(String.IsNullOrEmpty(sh), "", ",") + s.GetSQL()

            Return Me

        End Function

        Public Function OrderBy(sortClause As String) As GeneraSql(Of T)

            sh += If(String.IsNullOrEmpty(sh), "", ",") + String.Format("{0}", sortClause)

            Return Me

        End Function

        Public Function RemoveOrderByAll() As GeneraSql(Of T)

            sh = String.Empty

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
            hv = String.Empty
            sh = String.Empty
            ph = String.Empty
            gb = String.Empty
            gbFrom = String.Empty
        End Sub

        Private Function GetSqlBase(sqlCount As Boolean) As String

            If sqlCount Then
                sel = "COUNT(*)"
                sh = String.Empty
            Else
                If String.IsNullOrEmpty(sel) Then sel = String.Format("[{0}].*", tableName)
            End If

            If String.IsNullOrEmpty(join) Then join = If(String.IsNullOrEmpty(gb), tableName, gbFrom)
            If Not String.IsNullOrEmpty(gb) Then gb = If(gb.Contains("GROUP BY"), gb.Trim, "GROUP BY " + gb)
            If Not String.IsNullOrEmpty(wh) Then wh = If(wh.Contains("WHERE"), wh, "WHERE " + wh)
            If Not String.IsNullOrEmpty(sh) Then sh = If(sh.Contains("ORDER BY"), sh, "ORDER BY " + sh)
            If Not String.IsNullOrEmpty(hv) Then hv = If(hv.Contains("HAVING"), hv, "HAVING " + hv)

            Dim ret As String = String.Format("SELECT {0} FROM {1} {2} {3} {4} {5} {6}", sel.Trim, join.Trim, wh.Trim, gb.Trim, hv.Trim, sh.Trim, ph.Trim)

            Return ret.Trim + ";"

        End Function

        Public Function GetSql() As String

            Return GetSqlBase(False)

        End Function

        Public Function GetSqlCount() As String
            Return GetSqlBase(True)
        End Function

        Public Function GetParams() As DynamicParameters
            Return params
        End Function

        'Public Function Clone() As GeneraSql(Of T)

        '    Dim copia As GeneraSql(Of T) = Activator.CreateInstance(Me.GetType)

        '    For Each p As PropertyInfo In Me.GetType.GetProperties
        '        If p.CanWrite Then p.SetValue(copia, p.GetValue(Me))
        '    Next

        '    Return copia

        'End Function

#End Region

    End Class

End Namespace