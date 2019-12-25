Imports System.Reflection

Namespace Database.Infrastrutture

    Public Class SelectSQL(Of T As Class)

        Private _tableName As String

        Public Sub New()

            _tableName = TableNameModel(Of T).Get

        End Sub

        Public Function GetSql()

            Dim prop As New PropertyGet(Of T)

            Dim sel As String = String.Empty
            Dim virgola As String = String.Empty

            ' ... rinomina i campi chiave esterna secondo la codifica <nomeTabella><nomeCampo>
            If prop.fkList.Count = 0 And prop.mustMappedList.Count = 0 Then
                sel += String.Format("{1} [{0}].*", _tableName, virgola)
                virgola = ","
            Else
                For Each p As PropertyInfo In prop.propsAll
                    ' ... exclude readonly fields
                    If Not p.CanWrite Then Continue For
                    ' .. field name
                    Dim name As String = p.Name
                    If prop.fkList.FirstOrDefault(Function(x) String.Compare(x.FKColumnName, name, True) = 0) IsNot Nothing Then
                        ' .. foreign key
                        name = String.Format("{1} AS {0}{1}", _tableName, name)
                    ElseIf prop.mustMappedList.FirstOrDefault(Function(x) String.Compare(x, p.Name, True) = 0) IsNot Nothing Then
                        ' .. rimapped field
                        name = String.Format("{1} AS {0}{1}", _tableName, name)
                    End If
                    ' .. selection field
                    sel += String.Format("{2} [{0}].{1}", _tableName, name, virgola)
                    virgola = ","
                Next
            End If

            Return sel

        End Function

    End Class

    Public Class SelectSQL(Of T1 As Class, T2 As Class)

        Public Function GetSql()

            Dim sel1 As String = (New SelectSQL(Of T1)).GetSql
            Dim sel2 As String = (New SelectSQL(Of T2)).GetSql
            Dim virgola As String = If(Not String.IsNullOrEmpty(sel1) And Not String.IsNullOrEmpty(sel2), ",", String.Empty)

            Return sel1 + virgola + sel2

        End Function

    End Class

    Public Class SelectSQL(Of T1 As Class, T2 As Class, T3 As Class)

        Public Function GetSql()

            Dim sel1 As String = (New SelectSQL(Of T1, T2)).GetSql
            Dim sel2 As String = (New SelectSQL(Of T3)).GetSql
            Dim virgola As String = If(Not String.IsNullOrEmpty(sel1) And Not String.IsNullOrEmpty(sel2), ",", String.Empty)

            Return sel1 + virgola + sel2

        End Function

    End Class

    Public Class SelectSQL(Of T1 As Class, T2 As Class, T3 As Class, T4 As Class)

        Public Function GetSql()

            Dim sel1 As String = (New SelectSQL(Of T1, T2)).GetSql
            Dim sel2 As String = (New SelectSQL(Of T3, T4)).GetSql
            Dim virgola As String = If(Not String.IsNullOrEmpty(sel1) And Not String.IsNullOrEmpty(sel2), ",", String.Empty)

            Return sel1 + virgola + sel2

        End Function

    End Class

    Public Class SelectSQL(Of T1 As Class, T2 As Class, T3 As Class, T4 As Class, T5 As Class)

        Public Function GetSql()

            Dim sel1 As String = (New SelectSQL(Of T1, T2, T3)).GetSql
            Dim sel2 As String = (New SelectSQL(Of T4, T5)).GetSql
            Dim virgola As String = If(Not String.IsNullOrEmpty(sel1) And Not String.IsNullOrEmpty(sel2), ",", String.Empty)

            Return sel1 + virgola + sel2

        End Function

    End Class

End Namespace