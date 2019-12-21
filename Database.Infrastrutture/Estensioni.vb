Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace Database.Infrastrutture

    Module Estensioni

        <Extension()>
        Public Function GetName(Of T As Class)(ByVal exp As Expression(Of Func(Of T, String))) As String

            Return ReflectionHelper.GetMemberName(exp)

        End Function

        <Extension()>
        Public Function GetAttributeFrom(Of T As Attribute)(ByVal instanceType As Type, ByVal propertyName As String) As T
            Dim attrType As Type = GetType(T)
            Dim prop As System.Reflection.PropertyInfo = instanceType.GetProperty(propertyName)
            Dim ret As T = CType(prop.GetCustomAttributes(attrType, False).FirstOrDefault(), T)
            Return ret
        End Function


    End Module

End Namespace
