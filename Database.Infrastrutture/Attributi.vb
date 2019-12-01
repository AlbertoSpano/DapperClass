Namespace Database.Infrastrutture
    Namespace Attributi

        Public Class PrimaryKey
            Inherits Attribute

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


    End Namespace

End Namespace
