Namespace Database.Infrastrutture
    Namespace Attributi

        Public Class NotMapped
            Inherits Attribute

        End Class

        Public Class PrimaryKey
            Inherits Attribute

            Private _fieldName As String

            Public Sub New()
            End Sub

            Public Sub New(ByVal fieldName As String)
                Me._fieldName = fieldName
            End Sub

            Public Overridable ReadOnly Property FieldName As String
                Get
                    Return _fieldName
                End Get
            End Property

        End Class

        <AttributeUsage(AttributeTargets.Class)>
        Public Class TableNameAttribute
            Inherits Attribute

            Private _name As String

            Public Sub New(ByVal name As String)
                Me._name = name
            End Sub

            Public Overridable ReadOnly Property Name As String
                Get
                    Return _name
                End Get
            End Property

        End Class

        <AttributeUsage(AttributeTargets.Property)>
        Public Class ForeignKeyAttribute
            Inherits Attribute

            Private _pkColumnName As String
            Private _pkTableName As String

            Public Sub New(ByVal pkTableName As String, pkColumnName As String)
                Me._pkTableName = pkTableName
                Me._pkColumnName = pkColumnName
            End Sub

            Public Overridable ReadOnly Property PKTableName As String
                Get
                    Return _pkTableName
                End Get
            End Property

            Public Overridable ReadOnly Property PKColumnName As String
                Get
                    Return _pkColumnName
                End Get
            End Property
        End Class

        <AttributeUsage(AttributeTargets.Property)>
        Public Class MustMappedAttribute
            Inherits Attribute

        End Class


    End Namespace

End Namespace
