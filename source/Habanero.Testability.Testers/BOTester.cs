using System;
using System.Linq.Expressions;
using Habanero.Base;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.Testability.Testers
{
    /// <summary>
    /// This is a specialised Tester for testing Habanero.<see cref="IPropDef"/> 
    /// This tester provides methods for testing the basic attributes of a <see cref="IPropDef"/>
    /// such as ShouldBeCompulsory.
    /// </summary>
    public class BOTester
    {
        private readonly BOTestFactory _boTestFactory;
        public IBusinessObject BusinessObject { get; private set; }

        public BOTester(IBusinessObject businessObject)
        {
            if (businessObject == null) throw new ArgumentNullException("businessObject");
            _boTestFactory = new BOTestFactory(businessObject.GetType());
            BusinessObject = businessObject;
        }

        public void ShouldHaveDefault(string propName)
        {
            IPropDef propDef = GetClassDef().GetPropDef(propName, false);
            propDef.ShouldHaveDefault();
        }

        public void ShouldHaveDefault(string propName, string expectedDefaultValueString)
        {
            IPropDef propDef = GetClassDef().GetPropDef(propName, false);
            propDef.ShouldHaveDefault(expectedDefaultValueString);
        }
        public void ShouldNotHaveDefault(string propName)
        {
            IPropDef propDef = GetClassDef().GetPropDef(propName, false);
            propDef.ShouldNotHaveDefault();
        }

        public void ShouldNotHaveDefault(string propName, string expectedDefaultValueString)
        {
            IPropDef propDef = GetClassDef().GetPropDef(propName, false);
            propDef.ShouldNotHaveDefault(expectedDefaultValueString);
        }

        public void ShouldBeCompulsory(string propName)
        {
            IPropDef propDef = GetClassDef().GetPropDef(propName, false);
            propDef.ShouldBeCompulsory();
        }

        public void ShouldNotBeCompulsory(string propName)
        {
            IPropDef propDef = GetClassDef().GetPropDef(propName, false);
            propDef.ShouldNotBeCompulsory();
        }
        /*
        public void ShouldHaveRule()
        {
            var expectedMessage = string.Format("The PropDef for '{0}' for class '{1}' should have Rules set", this.PropDef.PropertyName, this.PropDef.ClassName);
            ths.PropDef.PropRules.ShouldNotBeEmpty(expectedMessage);
        }*/

        public void ShouldHaveReadWriteRule(string propName, PropReadWriteRule expectedReadWriteRule)
        {
            IPropDef propDef = GetClassDef().GetPropDef(propName, false);
            propDef.ShouldHaveReadWriteRule(expectedReadWriteRule);
        }

        public void ShouldHaveAllPropsMapped()
        {
            var classDef = this.BusinessObject.ClassDef;
            foreach (var propDef in classDef.PropDefcol)
            {
                PropertyShouldBeMapped(propDef.PropertyName);
            }
        }


        public void PropertyShouldBeMapped(string propertyName)
        {
            string className = GetClassName();
            
            var propertyInfo = ReflectionUtilities.GetPropertyInfo(this.BusinessObject.GetType(), propertyName);
            if(propertyInfo == null) return;
            if (propertyInfo.CanWrite)
            {
                if (propertyInfo.CanRead)
                {
                    AssertGetterAndSetterSameMapping(propertyName);
                }
                AssertSetterMappedToCorrectBOProp(propertyName);
            }
            if(propertyInfo.CanRead)
            {
                AssertGetterMappedToCorrectBOProp(propertyName, className);
            }
        }

        private string GetClassName()
        {
            return GetClassDef().ClassName;
        }

        private IClassDef GetClassDef()
        {
            return this.BusinessObject.ClassDef;
        }

        private void AssertGetterMappedToCorrectBOProp(string propertyName, string className)
        {
            var validPropValue = _boTestFactory.GetValidPropValue(this.BusinessObject, propertyName);
            var baseErrroMessage = string.Format("The Getter for the Property '{0}' for class '{1}'", propertyName, className);
            this.BusinessObject.SetPropertyValue(propertyName, validPropValue);
            var errorMessage = baseErrroMessage +
                               " is not mapped to the correct BOProp. Check the Property in your code";
            Assert.AreEqual(validPropValue,
                            ReflectionUtilities.GetPropertyValue(this.BusinessObject, propertyName),
                            errorMessage);
        }

        private void AssertSetterMappedToCorrectBOProp(string propertyName)
        {
            object validPropValue = _boTestFactory.GetValidPropValue(this.BusinessObject, propertyName);
            var baseErrroMessage = string.Format("The Setter for the Property '{0}' for class '{1}'", propertyName, this.BusinessObject.ClassDef.ClassName);
            ReflectionUtilities.SetPropertyValue(this.BusinessObject, propertyName, validPropValue);
            string errorMessage = baseErrroMessage +
                                  " is mapped to the incorrect BOProp. Check the Property in your code";
            Assert.AreEqual(validPropValue, this.BusinessObject.GetPropertyValue(propertyName), errorMessage);
        }

        private void AssertGetterAndSetterSameMapping(string propertyName)
        {
            string className = this.BusinessObject.ClassDef.ClassName;
            var validPropValue = _boTestFactory.GetValidPropValue(this.BusinessObject, propertyName);
            var baseErrroMessage = string.Format("The Getter And Setter for the Property '{0}' for class '{1}'", propertyName, className);
            ReflectionUtilities.SetPropertyValue(this.BusinessObject, propertyName, validPropValue);
            var errorMessage = baseErrroMessage +
                               " are not both mapped to the same BOProp. Check the Property in your code";
            Assert.AreEqual(validPropValue,
                            ReflectionUtilities.GetPropertyValue(this.BusinessObject, propertyName),
                            errorMessage);
        }
    }

    public class BOTester<T> : BOTester where T: class, IBusinessObject
    {
        public BOTester() : base(new BOTestFactory<T>().CreateDefaultBusinessObject())
        {
        }
        public void PropertyShouldBeMapped(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            PropertyShouldBeMapped(propertyName);
        }
        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> does not have a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        public void ShouldHaveDefault(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldHaveDefault(propertyName);
        }
        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> has a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        public void ShouldNotHaveDefault(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldNotHaveDefault(propertyName);
        }
        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> does not have a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        /// <param name="defaultValue">The default value for this property.</param>
        public void ShouldHaveDefault(Expression<Func<T, object>> propExpression, string defaultValue)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldHaveDefault(propertyName, defaultValue);
        }
        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> has a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        public void ShouldNotHaveDefault(Expression<Func<T, object>> propExpression, string expectedDefaultValue)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldNotHaveDefault(propertyName, expectedDefaultValue);
        }
        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> does not have a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        public void ShouldBeCompulsory(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldBeCompulsory(propertyName);
        }
        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> does not have a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        public void ShouldNotBeCompulsory(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldNotBeCompulsory(propertyName);
        }

        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> does not have a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        /// <param name="expectedReadWriteRule">The expected ReadWriteRule</param>
        public void ShouldHaveReadWriteRule(Expression<Func<T, object>> propExpression, PropReadWriteRule expectedReadWriteRule)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldHaveReadWriteRule(propertyName, expectedReadWriteRule);
        }
    }
}