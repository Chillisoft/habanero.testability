using Habanero.Base;
using Habanero.Util;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Habanero.Testability
{
    /// <summary>
    /// The BOTestFactory is a factory used to construct a Business Object for testing.
    /// The Constructed Business object can be constructed a a valid (i.e. saveable Business object)
    /// <see cref="CreateValidBusinessObject"/> a Default Busienss object <see cref="CreateDefaultBusinessObject"/>.<br/>
    /// A Valid Property Value can also be generated for any particular Prop using one of the overloads of <see cref="GetValidPropValue(System.Linq.Expressions.Expression{System.Func{T,object}})"/>,
    /// <see cref="GetValidPropValue(string)"/> or any of the methods from the base type <see cref="BOTestFactory"/>
    /// A Valid Relationship can be generated for any particular relationship using <see cref="GetValidRelationshipValue(System.Linq.Expressions.Expression{System.Func{T,object}})"/>.<br/>
    /// of <see cref="GetValidRelationshipValue(string)"/><br/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BOTestFactory<T> : BOTestFactory where T : IBusinessObject
    {
        /// <summary>
        /// The default constructor for the Factory the Busienss object can be set later
        /// by using <see cref="BusinessObject"/> or <see cref="CreateBusinessObject"/>.
        /// </summary>
        public BOTestFactory()
        {
        }
        /// <summary>
        /// Constructs with a business objec.t
        /// </summary>
        /// <param name="bo"></param>
        public BOTestFactory(T bo)
        {
            this.BusinessObject = bo;
        }

        private T CreateBusinessObject()
        {
            this.BusinessObject = base.BOFactory.CreateBusinessObject<T>();
            return this.BusinessObject;
        }
        //Creates a business object with only its default values set.
        public virtual T CreateDefaultBusinessObject()
        {
            return this.CreateBusinessObject();
        }

        public virtual T CreateValidBusinessObject()
        {
            return (this.BusinessObject = (T) this.CreateValidBusinessObject(typeof (T)));
        }
        /// <summary>
        /// Creates a valid value for the property identified by the lambda expression <paramref name="propExpression"/>.
        /// </summary>
        /// <param name="propExpression"></param>
        /// <returns></returns>
        public object GetValidPropValue(Expression<Func<T, object>> propExpression)
        {
            return this.GetValidPropValue(this.BusinessObject, propExpression);
        }
        /// <summary>
        /// Creates a valid property for the property identified by <paramref name="propName"/>
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        public object GetValidPropValue(string propName)
        {
            if (this.BusinessObject == null) return base.GetValidPropValue(typeof (T), propName);
            return base.GetValidPropValue(this.BusinessObject, propName);
        }

        private static PropertyInfo GetPropertyInfo<TModel>(Expression<Func<TModel, object>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }

        private static string GetPropertyName(Expression<Func<T, object>> propExpression)
        {
            return GetPropertyInfo(propExpression).Name;
        }
        /// <summary>
        /// Returns a valid value for the busienss object <paramref name="bo"/>'s property
        /// identified by <pararef name="propExpression"/>.
        /// </summary>
        /// <param name="bo"></param>
        /// <param name="propExpression"></param>
        /// <returns></returns>
        public object GetValidPropValue(T bo, Expression<Func<T, object>> propExpression)
        {
            string propName = GetPropertyName(propExpression);
            return ((bo == null) ? this.GetValidPropValue(propName) : this.GetValidPropValue(bo, propName));
        }

        private object GetValidPropValue(T bo, string propName)
        {
            return base.GetValidPropValue(bo, propName);
        }
        /// <summary>
        /// Returns a valid relationship for the <see cref="BusinessObject"/>'s
        /// property identified by <paramref name="propExpression"/>
        /// </summary>
        /// <param name="propExpression"></param>
        /// <returns></returns>
        public IBusinessObject GetValidRelationshipValue(Expression<Func<T, object>> propExpression)
        {
            string relationshipName = GetPropertyName(propExpression);
            return this.GetValidRelationshipValue(relationshipName);
        }
        /// <summary>
        /// Returns a valid Relationship for the <see cref="BusinessObject"/>'s
        /// relationship identified by the <paramref name="relationshipName"/>
        /// </summary>
        /// <param name="relationshipName"></param>
        /// <returns></returns>
        public IBusinessObject GetValidRelationshipValue(string relationshipName)
        {
            IRelationshipDef relationshipDef = GetRelationshipDef(typeof (T), relationshipName);
            return this.GetValidRelationshipValue(relationshipDef);
        }
        /// <summary>
        /// Get and set the <see cref="IBusinessObject"/> object that this generic Factory is generic
        /// factory is generating values for.
        /// This property is set directly or via the constructor or via <see cref="CreateDefaultBusinessObject"/>
        /// or via <see cref="CreateValidBusinessObject"/>
        /// </summary>
        public T BusinessObject { get; set; }
    }
}