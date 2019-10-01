Namespace Database.Infrastrutture

    Public Interface IPaging

        Property CurrentRowsCount As Integer
        Property CurrentPagesCount As Integer

        Sub PageCount(pageSize As Integer)

    End Interface

End Namespace