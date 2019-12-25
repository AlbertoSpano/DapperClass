Imports System.Linq.Expressions
Imports System.Reflection

Namespace Database.Infrastrutture

    Public Module ReflectionHelper

        Public Function GetMemberName(ByVal lambda As LambdaExpression) As String

            Dim expr As Expression = lambda

            While True

                Select Case expr.NodeType

                    Case ExpressionType.Lambda
                        expr = (CType(expr, LambdaExpression)).Body

                    Case ExpressionType.Convert
                        expr = (CType(expr, UnaryExpression)).Operand

                    Case ExpressionType.ConvertChecked
                        expr = (CType(expr, UnaryExpression)).Operand

                    Case ExpressionType.Call
                        Dim member As MethodCallExpression = CType(expr, MethodCallExpression)
                        Dim methodName As String = member.Method.Name
                        Dim campo As String = CType(member.Arguments(0), MemberExpression).Member.Name
                        Return String.Format("{0}([{1}])", methodName, campo)

                    Case ExpressionType.MemberAccess
                        Return CType(expr, MemberExpression).Member.Name

                    Case Else
                        Return Nothing
                End Select

            End While

        End Function

        Public Function GetBoolExpression(ByVal lambda As LambdaExpression) As Espressione

            Dim expr As Expression = lambda
            Dim cExpr As ConstantExpression = Nothing
            Dim bExpr As BinaryExpression = Nothing
            Dim baseProperty As PropertyInfo = Nothing
            Dim [property] As PropertyInfo = Nothing

            Dim ret As New Espressione With {.FieldName = Nothing, .Operation = Nothing, .Value = Nothing}

            While True

                Select Case expr.NodeType

                    Case ExpressionType.Lambda
                        expr = (CType(expr, LambdaExpression)).Body

                    Case ExpressionType.Convert
                        expr = (CType(expr, UnaryExpression)).Operand

                    Case ExpressionType.ConvertChecked
                        expr = (CType(expr, UnaryExpression)).Operand

                    Case ExpressionType.Equal, ExpressionType.GreaterThan, ExpressionType.GreaterThanOrEqual, ExpressionType.LessThan, ExpressionType.LessThanOrEqual, ExpressionType.NotEqual, ExpressionType.And, ExpressionType.AndAlso, ExpressionType.Or, ExpressionType.OrElse
                        ' .. operatore
                        ret.Operation = GetOperation(expr.NodeType)
                        ' .. valore
                        bExpr = CType(expr, BinaryExpression)
                        If TypeOf bExpr.Right Is ConstantExpression Then
                            cExpr = CType(bExpr.Right, ConstantExpression)
                            ret.Value = cExpr.Value
                        ElseIf TypeOf bExpr.Right Is UnaryExpression Then
                            expr = CType(bExpr.Right, UnaryExpression)
                        Else
                            Dim s As MemberExpression = CType(bExpr.Right, System.Linq.Expressions.MemberExpression)
                            Dim objectMember = Expression.Convert(s, GetType(Object))
                            Dim getterLambda = Expression.Lambda(Of Func(Of Object))(objectMember)
                            Dim getter = getterLambda.Compile()
                            ret.Value = getter()
                        End If
                        ' .. field
                        If TypeOf bExpr.Left Is UnaryExpression Then
                            expr = CType(bExpr.Left, UnaryExpression)
                        Else
                            expr = CType(bExpr.Left, MemberExpression)
                        End If

                    Case ExpressionType.MemberAccess
                        ret.FieldName = CType(expr, MemberExpression).Member.Name
                        Return ret

                    Case Else
                        Return ret

                End Select

            End While

            Return ret

        End Function

        Private Function GetOperation(nodeType As ExpressionType) As String

            Select Case nodeType
                Case ExpressionType.Equal
                    Return "="
                Case ExpressionType.NotEqual
                    Return "<>"
                Case ExpressionType.GreaterThan
                    Return ">"
                Case ExpressionType.GreaterThanOrEqual
                    Return ">="
                Case ExpressionType.LessThanOrEqual
                    Return "<="
                Case ExpressionType.LessThan
                    Return "<"
                Case ExpressionType.And
                    Return "AND"
                Case ExpressionType.AndAlso
                    Return "AND"
                Case ExpressionType.Or
                    Return "OR"
                Case ExpressionType.OrElse
                    Return "OR"
                Case Else
                    Return Nothing
            End Select
        End Function

    End Module

    Public Class Espressione
        Public Property FieldName As String
        Public Property Operation As String
        Public Property Value As Object
    End Class

End Namespace
