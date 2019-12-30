Namespace Database.Infrastrutture

    Public Enum TipiWhere
        [NOTHING]
        [AND]
        [OR]
        [NOT]
    End Enum

    Public Enum AggregateFunction
        [NOTHING]
        AVG
        COUNT
        MAX
        MIN
        SUM
    End Enum

    Public Enum TipiJoin
        INNER
        LEFT
        RIGHT
    End Enum

    Public Enum TipiOrderBy
        [Default]
        DESC
    End Enum

End Namespace