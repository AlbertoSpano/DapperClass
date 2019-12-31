Imports System.Reflection

Namespace Database.Infrastrutture

    Public Class PropertyInfoEx

        Public Property PropertyInfo As PropertyInfo
        Public Property IsForegnKey As Boolean
        Public Property IsPrimaryKey As Boolean
        Public Property IsMustMapped As Boolean
        Public ReadOnly Property CanWrite As Boolean
            Get
                Return PropertyInfo.CanWrite
            End Get
        End Property
        Public ReadOnly Property CanRead As Boolean
            Get
                Return PropertyInfo.CanRead
            End Get
        End Property
        Public ReadOnly Property Name As String
            Get
                Return PropertyInfo.Name
            End Get
        End Property
    End Class

End Namespace
