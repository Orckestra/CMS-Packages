using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Composite.Search.SimplePageSearch
{
    internal static class StaticReflection
    {
        public static MethodInfo GetGenericMethodInfo(Expression<Action<object>> expression)
        {
            Verify.ArgumentNotNull(expression, nameof(expression));

            return GetMethodInfo(expression.Body).GetGenericMethodDefinition();
        }

        public static MethodInfo GetGenericMethodInfo(Expression<Func<object>> expression)
        {
            Verify.ArgumentNotNull(expression, nameof(expression));

            return GetMethodInfo(expression.Body).GetGenericMethodDefinition();
        }

        public static MethodInfo GetMethodInfo<T, S>(Expression<Func<T, S>> expression)
        {
            return GetMethodInfo(expression.Body as MethodCallExpression);
        }

        public static MethodInfo GetMethodInfo<T>(Expression<Func<T>> expression)
        {
            return GetMethodInfo(expression.Body as MethodCallExpression);
        }

        public static MethodInfo GetMethodInfo(Expression expression)
        {
            Verify.ArgumentNotNull(expression, nameof(expression));

            if (expression is UnaryExpression unaryExpression
                && unaryExpression.NodeType == ExpressionType.Convert)
            {
                expression = unaryExpression.Operand;
            }

            Verify.ArgumentCondition(expression is MethodCallExpression, nameof(expression),
                $"Expression body should be of type '{nameof(MethodCallExpression)}'");

            return (expression as MethodCallExpression).Method;
        }
    }
}
