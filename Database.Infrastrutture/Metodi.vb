Imports System.Data.OleDb
Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace Database.Infrastrutture

    Public Module Metodi

        <Extension()>
        Public Function GetAttributeFrom(Of T As Attribute)(ByVal instanceType As Type, ByVal propertyName As String) As T
            Dim attrType As Type = GetType(T)
            Dim prop As System.Reflection.PropertyInfo = instanceType.GetProperty(propertyName)
            Dim ret As T = CType(prop.GetCustomAttributes(attrType, False).FirstOrDefault(), T)
            Return ret
        End Function

    End Module

End Namespace


