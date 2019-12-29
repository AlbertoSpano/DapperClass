Imports System.Linq.Expressions
Imports System.Reflection

Namespace Database.Infrastrutture

    Public Module ReflectionHelper

        Public Function GetMemberName(ByVal lambda As LambdaExpression) As ExpressionParams

            Dim expr As Expression = lambda
            Dim ret As New ExpressionParams

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
                        ret.MethodName = member.Method.Name
                        ret.FieldName = CType(member.Arguments(0), MemberExpression).Member.Name
                        Return ret

                    Case ExpressionType.MemberAccess
                        ret.FieldName = CType(expr, MemberExpression).Member.Name
                        Return ret

                    Case Else
                        Return Nothing
                End Select

            End While

        End Function

        Public Function GetBoolExpression(ByVal lambda As LambdaExpression) As ExpressionParams

            Dim expr As Expression = lambda
            Dim cExpr As ConstantExpression = Nothing
            Dim bExpr As BinaryExpression = Nothing

            Dim ret As New ExpressionParams With {.FieldName = Nothing, .Operation = Nothing, .Value = Nothing}

            While True

                Select Case expr.NodeType

                    Case ExpressionType.Lambda
                        expr = (CType(expr, LambdaExpression)).Body

                    Case ExpressionType.Convert
                        expr = (CType(expr, UnaryExpression)).Operand

                    Case ExpressionType.ConvertChecked
                        expr = (CType(expr, UnaryExpression)).Operand

                    Case ExpressionType.Constant
                        cExpr = CType(bExpr.Right, ConstantExpression)
                        ret.Value = cExpr.Value

                    Case ExpressionType.Call
                        Dim member As MethodCallExpression = CType(expr, MethodCallExpression)
                        ret.MethodName = member.Method.Name
                        ret.FieldName = CType(member.Arguments(0), MemberExpression).Member.Name
                        Return ret

                    Case ExpressionType.Equal, ExpressionType.GreaterThan, ExpressionType.GreaterThanOrEqual, ExpressionType.LessThan, ExpressionType.LessThanOrEqual, ExpressionType.NotEqual, ExpressionType.And, ExpressionType.AndAlso, ExpressionType.Or, ExpressionType.OrElse
                        ' .. operator
                        ret.Operation = GetOperation(expr.NodeType)
                        ' .. value
                        bExpr = CType(expr, BinaryExpression)
                        If TypeOf bExpr.Right Is ConstantExpression Then
                            cExpr = CType(bExpr.Right, ConstantExpression)
                            ret.Value = cExpr.Value
                            'ElseIf TypeOf bExpr.Right Is UnaryExpression Then
                            '    expr = CType(bExpr.Right, UnaryExpression)
                        ElseIf TypeOf bExpr.Right Is MemberExpression Then
                            Dim s As MemberExpression = CType(bExpr.Right, System.Linq.Expressions.MemberExpression)
                            ret.VariableName = s.Member.Name.Replace("$VB$Local_", "")
                            Dim objectMember = Expression.Convert(s, GetType(Object))
                            Dim getterLambda = Expression.Lambda(Of Func(Of Object))(objectMember)
                            Dim getter = getterLambda.Compile()
                            ret.Value = getter()
                        End If
                        ' .. field
                        expr = bExpr.Left

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

    Public Class ExpressionParams
        Public Property VariableName As String
        Public Property MethodName As String
        Public Property FieldName As String
        Public Property Operation As String
        Public Property Value As Object
    End Class

End Namespace
