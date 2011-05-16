using Habanero.Base;

namespace Habanero.Testability.ExtensionMethods
{
    public static class TestabilityExtensions
    {
/*No linq.Expression in CF  
  public static BOTestFactory<T> WithValue<T, TPropType>(this BOTestFactory<T> factory, Expression<Func<T, TPropType>> expression, TPropType value) where T : class, IBusinessObject
        {
            factory.SetValueFor(expression, value);
            return factory;
        }
        public static BOTestFactory<T> WithValueFor<T, TPropType>(this BOTestFactory<T> factory, Expression<Func<T, TPropType>> expression) where T : class, IBusinessObject
        {
            factory.SetValueFor(expression);
            return factory;
        }*/

        public static BOTestFactory<T> WithValue<T, TPropType>(this BOTestFactory<T> factory, string propName, TPropType value) where T : class, IBusinessObject
        {
            factory.SetValueFor(propName, value);
            return factory;
        }
        public static BOTestFactory<T> WithValueFor<T, TPropType>(this BOTestFactory<T> factory, string propName) where T : class, IBusinessObject
        {
            factory.SetValueFor(propName);
            return factory;
        }
    }
}
