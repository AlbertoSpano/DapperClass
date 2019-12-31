Namespace Database.Infrastrutture

    Public Class DapperSplitOn

        Public Shared Function [Get](Of T As Class)() As String
            Return (New PropertyGet(Of T)).propId.Name
        End Function

        Public Shared Function [Get](Of T1 As Class, T2 As Class)() As String
            Return [Get](Of T1)() + "," + [Get](Of T2)()
        End Function

        Public Shared Function [Get](Of T1 As Class, T2 As Class, T3 As Class)() As String
            Return [Get](Of T1, T2)() + "," + [Get](Of T3)()
        End Function

        Public Shared Function [Get](Of T1 As Class, T2 As Class, T3 As Class, T4 As Class)() As String
            Return [Get](Of T1, T2, T3)() + "," + [Get](Of T4)()
        End Function

        Public Shared Function [Get](Of T1 As Class, T2 As Class, T3 As Class, T4 As Class, T5 As Class)() As String
            Return [Get](Of T1, T2, T3, T4)() + "," + [Get](Of T5)()
        End Function

        Public Shared Function [Get](Of T1 As Class, T2 As Class, T3 As Class, T4 As Class, T5 As Class, T6 As Class)() As String
            Return [Get](Of T1, T2, T3, T4, T5)() + "," + [Get](Of T6)()
        End Function

    End Class

End Namespace