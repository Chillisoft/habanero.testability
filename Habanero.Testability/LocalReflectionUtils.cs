using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Habanero.Testability
{
    public class LocalReflectionUtils
    {
        // ReSharper restore PossibleNullReferenceException
        /// <summary>
        /// Returns the Property Name of the property used in the Lambda expression of type
        /// bo -> bo.MyProperty. This function will return 'MyProperty'.
        /// </summary>
        /// <typeparam name="TModel">The object whose Property Name is being returned</typeparam>
        /// <typeparam name="TReturn">The Return type of the Lambda Expression</typeparam>
        /// <param name="propExpression">The Lambda expression</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"> Exception if the lamda is not a lambda for a property</exception>
        public static string GetPropertyName<TModel, TReturn>(Expression<Func<TModel, TReturn>> propExpression)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propExpression);
            return propertyInfo.Name;
        }

        /// <summary>
        /// Returns the <see cref="PropertyInfo"/> of the property used in the Lambda expression of type
        /// bo -> bo.MyProperty. This function will return the PropertyInfo for MyProperty.
        /// </summary>
        /// <typeparam name="TModel">The object whose PropertyInfo is being returned</typeparam>
        /// <typeparam name="TReturn">The Return type of the Lambda expression</typeparam>
        /// <param name="propExpression">The Lambda expression</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"> Exception if the lamda is not a lambda for a property</exception>
        public static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> propExpression)
        {
            var memberExpression = GetMemberExpression(propExpression);
            return (PropertyInfo)memberExpression.Member;
        }

        private static MemberExpression GetMemberExpression<TModel, T>(Expression<Func<TModel, T>> expression)
        {
            return GetMemberExpression(expression, true);
        }
        private static MemberExpression GetMemberExpression<TModel, T>(Expression<Func<TModel, T>> expression, bool enforceCheck)
        {
            MemberExpression memberExpression = null;
            switch (expression.Body.NodeType)
            {
                case ExpressionType.Convert:
                    var body = (UnaryExpression)expression.Body;
                    memberExpression = body.Operand as MemberExpression;
                    break;
                case ExpressionType.MemberAccess:
                    memberExpression = expression.Body as MemberExpression;
                    break;
            }

            if (enforceCheck && memberExpression == null)
            {
                throw new ArgumentException(expression.ToString() + " - Not a member accessor", "expression");
            }

            return memberExpression;
        }
    }
}
