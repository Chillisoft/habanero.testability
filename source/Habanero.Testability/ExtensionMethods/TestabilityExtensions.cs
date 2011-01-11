using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Habanero.Base;

namespace Habanero.Testability
{
    public static class TestabilityExtensions
    {
        public static BOTestFactory<T> WithValue<T, TPropType>(this BOTestFactory<T> factory, Expression<Func<T, TPropType>> expression, TPropType value) where T : class, IBusinessObject
        {
            factory.SetValueFor(expression, value);
            return factory;
        }
        public static BOTestFactory<T> WithValueFor<T, TPropType>(this BOTestFactory<T> factory, Expression<Func<T, TPropType>> expression) where T : class, IBusinessObject
        {
            factory.SetValueFor(expression);
            return factory;
        }
    }
}
