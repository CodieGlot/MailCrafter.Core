using System.Linq.Expressions;

namespace MailCrafter.Utils.Extensions;
public static class ExpressionExtensions
{
    public static string GetMemberName<T, TField>(this Expression<Func<T, TField>> fieldSelector)
    {
        if (fieldSelector.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }
        else if (fieldSelector.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression unaryMemberExpression)
        {
            // Handle cases where the expression has a conversion (e.g., for value types)
            return unaryMemberExpression.Member.Name;
        }

        throw new ArgumentException("The field selector expression is not valid.");
    }
}