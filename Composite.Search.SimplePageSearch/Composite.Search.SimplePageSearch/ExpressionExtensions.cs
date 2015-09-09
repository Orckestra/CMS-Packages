using System.Linq.Expressions;

namespace Composite.Search.SimplePageSearch
{
    static class ExpressionExtensions
    {
        public static Expression AndAlso(this Expression left, Expression right)
        {
            if (left == null) return right;

            return Expression.AndAlso(left, right);
        }

        public static Expression OrElse(this Expression left, Expression right)
        {
            if (left == null) return right;

            return Expression.OrElse(left, right);
        }
    }
}
