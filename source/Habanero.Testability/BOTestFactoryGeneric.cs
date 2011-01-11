using System.Collections.Generic;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
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
    /// A Valid Property Value can also be generated for any particular Prop using one of the overloads of <see cref="GetValidPropValue{TReturn}(T, System.Linq.Expressions.Expression{System.Func{T,TReturn}})"/>,
    /// <see cref="GetValidPropValue(string)"/> or any of the methods from the base type <see cref="BOTestFactory"/>
    /// A Valid Relationship can be generated for any particular relationship using <see cref="GetValidRelationshipValue{TReturn}"/>.<br/>
    /// of <see cref="GetValidRelationshipValue(string)"/><br/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BOTestFactory<T> : BOTestFactory where T : class, IBusinessObject
    {
        //For testing I consider 3 to be many i.e. there may be special cases where the 
        // test can handle one or even two objects but most algorithms that work on 3
        // items will work on n items.
        private const int MANY = 3;

        /// <summary>
        /// The default constructor for the Factory the Busienss object can be set later
        /// by using <see cref="BusinessObject"/> or <see cref="CreateBusinessObject"/>.
        /// </summary>
        public BOTestFactory() : base(typeof (T))
        {
        }

        /// <summary>
        /// Constructs with a business objec.t
        /// </summary>
        /// <param name="bo"></param>
        public BOTestFactory(T bo) : base(typeof (T))
        {
            this.BusinessObject = bo;
        }

        private new T CreateBusinessObject()
        {
            this.BusinessObject = base.BOFactory.CreateBusinessObject<T>();
            return this.BusinessObject;
        }

        //Creates a business object with only its default values set.
        public virtual T CreateDefaultBusinessObject()
        {
            return this.CreateBusinessObject();
        }

        /// <summary>
        /// Creates a valid value for the property identified by the lambda expression <paramref name="propExpression"/>.
        /// </summary>
        /// <param name="propExpression"></param>
        /// <returns></returns>
        public TReturn GetValidPropValue<TReturn>(Expression<Func<T, TReturn>> propExpression)
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

        private static PropertyInfo GetPropertyInfo<TModel, TReturn>(Expression<Func<TModel, TReturn>> expression)
        {
            return ReflectionUtilities.GetPropertyInfo(expression);
        }

        private static string GetPropertyName<TReturn>(Expression<Func<T, TReturn>> propExpression)
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
        public TReturn GetValidPropValue<TReturn>(T bo, Expression<Func<T, TReturn>> propExpression)
        {
            string propName = GetPropertyName(propExpression);
            object value = (bo == null) ? this.GetValidPropValue(propName) : this.GetValidPropValue(bo, propName);
            try
            {
                return (TReturn) value;
            }
            catch (InvalidCastException ex)
            {
                string errorMessage = string.Format("The value '{0}' could not be cast to type '{1}'", value,
                                                    typeof (TReturn));
                throw new InvalidCastException(errorMessage, ex);
            }
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
        public IBusinessObject GetValidRelationshipValue<TReturn>(Expression<Func<T, TReturn>> propExpression)
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
            IRelationshipDef relationshipDef = GetRelationshipDef(typeof (T), relationshipName, true);
            return this.GetValidRelationshipValue(relationshipDef);
        }

        /// <summary>
        /// Get and set the <see cref="IBusinessObject"/> object that this generic Factory is generic
        /// factory is generating values for.
        /// This property is set directly or via the constructor or via <see cref="CreateDefaultBusinessObject"/>
        /// or via <see cref="CreateValidBusinessObject"/>
        /// </summary>
        public T BusinessObject { get; set; }

        /// <summary>
        /// Sets the <paramref name="setPropValue"/> for the method Idenfied by the <paramref name="propertyExpression"/>.
        /// This ensures that when the <see cref="CreateValidBusinessObject"/> or <see cref="BOTestFactory.UpdateCompulsoryProperties"/>
        /// or <see cref="GetValidPropValue"/> and the <see cref="GetValidRelationshipValue"/>
        /// for this Property this value is always used
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <param name="setPropValue"></param>
        public void SetValueFor<TReturn>(Expression<Func<T, TReturn>> propertyExpression, TReturn setPropValue)
        {
            string propertyName = GetPropertyName(propertyExpression);
            _defaultValueRegistry.Register(propertyName, setPropValue);
        }

        public void SetValueFor<TReturn>(Expression<Func<T, TReturn>> propertyExpression)
        {
            var propDef = GetPropDef(propertyExpression, false);
            if (propDef != null)
            {
                var validValueGenerator = _validValueGenRegistry.Resolve(propDef);
                _validValueGenRegistry.Register(propDef, validValueGenerator.GetType());
            }
            else
            {
                var propertyName = GetPropertyName(propertyExpression);
                //If this is not a relationshipdef then will get Error from here.
                var validRelatedValue = GetValidRelationshipValue(propertyExpression);
                _defaultValueRegistry.Register(propertyName, validRelatedValue);
            }
        }

        private static IRelationshipDef GetRelationshipDef<TReturn>(Expression<Func<T, TReturn>> propertyExpression, bool raiseErrIfNotExists)
        {
            string propertyName = GetPropertyName(propertyExpression);
            return GetRelationshipDef(typeof(T), propertyName, raiseErrIfNotExists);
        }
        private static IPropDef GetPropDef<TReturn>(Expression<Func<T, TReturn>> propertyExpression, bool raiseErrIfNotExists )
        {
            string propertyName = GetPropertyName(propertyExpression);
            return GetPropDef(typeof(T), propertyName, raiseErrIfNotExists);
        }

        public void SetValidValueGenerator<TReturn>(Expression<Func<T, TReturn>> propertyExpression, Type validValueGeneratorType)
        {
            IPropDef propDef = GetPropDef(propertyExpression, true);
            _validValueGenRegistry.Register(propDef, validValueGeneratorType);
        }
        public BOTestFactory<T> WithOne<TBusinessObject>(
            Expression<Func<T, BusinessObjectCollection<TBusinessObject>>> relationshipExpression)
            where TBusinessObject : class, IBusinessObject, new()
        {
            return WithMany(relationshipExpression, 1);
        }

        public BOTestFactory<T> WithTwo<TBusinessObject>(
            Expression<Func<T, BusinessObjectCollection<TBusinessObject>>> relationshipExpression)
            where TBusinessObject : class, IBusinessObject, new()
        {
            return WithMany(relationshipExpression, 2);
        }

        public BOTestFactory<T> WithMany<TBusinessObject>(
            Expression<Func<T, BusinessObjectCollection<TBusinessObject>>> relationshipExpression)
            where TBusinessObject : class, IBusinessObject, new()
        {
            return WithMany(relationshipExpression, MANY);
        }

        public BOTestFactory<T> WithMany<TBusinessObject>(
            Expression<Func<T, BusinessObjectCollection<TBusinessObject>>> relationshipExpression,
            int expectedNoOfCreatedChildObjects)
            where TBusinessObject : class, IBusinessObject, new()
        {
            string propertyName = GetPropertyName(relationshipExpression);
            IList<TBusinessObject> bos = new List<TBusinessObject>();
            var boTBusinessObjectFactory = BOTestFactoryRegistry.Instance.Resolve<TBusinessObject>();
            for (int i = 0; i < expectedNoOfCreatedChildObjects; i++)
            {
                TBusinessObject relatedBO = boTBusinessObjectFactory.CreateValidBusinessObject();
                bos.Add(relatedBO);
            }
            _defaultValueRegistry.Register(propertyName, bos);
            return this;
        }

        public BOTestFactory<T> WithOutSingleRelationships()
        {
            SetCompulsorySingleRelationships = false;
            return this;
        }

        public new virtual T CreateValidBusinessObject()
        {
            return (this.BusinessObject = (T) base.CreateValidBusinessObject());
        }

        public new virtual T CreateSavedBusinessObject()
        {
            T bo = this.CreateValidBusinessObject();
            bo.Save();
            return bo;
        }
        public virtual IList<T> CreateManySavedBusinessObject()
        {
            return CreateManySavedBusinessObject(MANY);
        }

        public virtual IList<T> CreateManySavedBusinessObject(int noToCreate)
        {
            IList<T> col = new List<T>();
            for (int i = 0; i < noToCreate; i++)
            {
                T bo = this.CreateValidBusinessObject();
                bo.Save();
                col.Add(bo);
            }
            return col;
        }


    }
}