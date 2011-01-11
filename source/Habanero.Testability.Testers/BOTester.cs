using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Testability.Helpers;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.Testability.Testers
{
    /// <summary>
    /// This is a specialised Tester for testing Habanero.<see cref="IBusinessObject"/> 
    /// This tester provides methods for testing the basic aspects of properties of
    /// a business object see <see cref="IPropDef"/>
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

        /// <summary>
        /// For each property defined in the ClassDefs for this Business
        /// Object should have all Properties Mapped.<br/>
        /// 
        /// See <see cref="ShouldHavePropertyMapped"/> for more details on how this 
        /// mapping testing is done.
        /// </summary>
        public void ShouldHaveAllPropsMapped()
        {
            var propDefsWithPropInfos = GetClassDef().PropDefcol.Where(HasPropInfo);
            foreach (var propDef in propDefsWithPropInfos)
            {
                ShouldHavePropertyMapped(propDef.PropertyName);
            }
        }

        public void ShouldHaveAllSingleRelationshipsMapped()
        {
            var singleRelDefsWithPropInfos = GetClassDef().RelationshipDefCol.Where(HasPropInfo).OfType<SingleRelationshipDef>();
            foreach (var relationshipDef in singleRelDefsWithPropInfos)
            {
                ShouldHaveSingleRelationshipMapped(relationshipDef.RelationshipName);
            }
        }

        private bool HasPropInfo(IPropDef propDef)
        {
            var propName = propDef.PropertyName;
            return HasPropInfo(propName);
        }
        private bool HasPropInfo(IRelationshipDef relationshipDef)
        {
            var propName = relationshipDef.RelationshipName;
            return HasPropInfo(propName);
        }

        private bool HasPropInfo(string propName)
        {
            var propertyInfo = GetPropertyInfo(propName);
            return propertyInfo != null;
        }

        /// <summary>
        /// Tests that the Mapping within the Property Gettter and Setter
        /// are mapped correctly i.e. the string "ProspectTypeName" in 
        /// the Getter and the string "ProspectTypeName" in the setter are 
        /// both the same and they are mapped to a valid property Def in the ClassDef.
        /// <code>
        ///  public virtual string ProspectTypeName
        ///  {
        ///      get { return ((string)(base.GetPropertyValue("ProspectTypeName"))); }
        ///      set { base.SetPropertyValue("ProspectTypeName", value); }
        ///  }
        /// </code>
        /// This is done by setting the Property via reflection (if it has a set)
        /// and setting it via reflection (if it has a get) and comparing these values
        /// to the values for the BOProp to ensure that all mappings are correct.
        /// </summary>
        /// <param name="propertyName"></param>
        public void ShouldHavePropertyMapped(string propertyName)
        {

            var propertyInfo = GetPropertyInfo(propertyName);
            AssertPropInfoNotNull(propertyInfo, GetClassName(), propertyName);

            //This should be moved to use a Factory Method that Creates the Appropriate SingleValueTester.
            GetTester(propertyName).ShouldHavePropertyMapped();
/*
            string className = GetClassName();
            
            var propertyInfo = GetPropertyInfo(propertyName);
            AssertPropInfoNotNull(propertyInfo, className, propertyName);
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
            }*/
        }

        private SingleValueTester GetTester(string propertyName)
        {
            if (HasPropDef(propertyName)) { return GetPropTester(propertyName);}
            if (HasRelDef(propertyName)) { return GetSingleRelationshipTester(propertyName); }
            return null;
        }

        /// <summary>
        /// Tests that the Mapping within the Property Gettter and Setter
        /// of a relationship are mapped correctly i.e. the string "SingleRelGetterNotMapped" in 
        /// the Getter and the string "SingleRelationship" in the setter are 
        /// both the same and they are mapped to a valid <see cref="IRelationshipDef"/> in the ClassDef
        /// <see cref="IClassDef"/>.
        /// <code>
        ///   public virtual FakeBO SingleRelationship
        ///   {
        ///       get { return Relationships.GetRelatedObject{FakeBO}("SingleRelationship"); }
        ///       set { Relationships.SetRelatedObject("SingleRelationship", value); }
        ///   }
        /// </code>
        /// This is done by setting the Property via reflection (if it has a set)
        /// and setting it via reflection (if it has a get) and comparing these values
        /// to the values for the BOProp to ensure that all mappings are correct.
        /// </summary>
        /// <param name="relationshipName"></param>
        public void ShouldHaveSingleRelationshipMapped(string relationshipName)
        {
            ShouldHavePropertyMapped(relationshipName);
        }
        private static void AssertPropInfoNotNull(PropertyInfo propertyInfo, string className, string propertyName)
        {
            Assert.IsNotNull(propertyInfo, "The Property '{0}' does not exist on the class '{1}'. Please check your code.", propertyName, className);
        }
/*
        private void AssertGetterMappedToCorrectBOProp(string propertyName, string className)
        {
            var expectedValue = _boTestFactory.GetValidValue(this.BusinessObject, propertyName);
            //this.BusinessObject.SetPropertyValue(propertyName, validPropValue);
            SetValueFromBO(propertyName, expectedValue);
            var valueViaReflection = GetValueViaReflection(propertyName);

            Assert.AreEqual(expectedValue, valueViaReflection, GetGetterMappedErrorMessage(propertyName, className));
        }

        private void AssertSetterMappedToCorrectBOProp(string propertyName)
        {
            object expectedValue = _boTestFactory.GetValidValue(this.BusinessObject, propertyName);
            SetValueViaReflection(propertyName, expectedValue);
            var valueFromBO = GetValueFromBO(propertyName);
            Assert.AreEqual(expectedValue, valueFromBO, GetSetterMappedErrorMessage(propertyName));
        }

        private void AssertGetterAndSetterSameMapping(string propertyName)
        {
            var expectedValue = _boTestFactory.GetValidValue(this.BusinessObject, propertyName);
            SetValueViaReflection(propertyName, expectedValue);
            var valueViaReflection = GetValueViaReflection(propertyName);
            Assert.AreEqual(expectedValue, valueViaReflection, GetGetterAndSetterErrorMessage(propertyName));
        }*/

        private string GetClassName()
        {
            return GetClassDef().ClassName;
        }

        private IClassDef GetClassDef()
        {
            return this.BusinessObject.ClassDef;
        }
/*

        private static string GetGetterMappedErrorMessage(string propertyName, string className)
        {
            var baseErrroMessage = string.Format("The Getter for the Property '{0}' for class '{1}'", propertyName, className);
            return baseErrroMessage +
                   " is not mapped to the correct BOProp. Check the Property in your code";
        }

        private string GetSetterMappedErrorMessage(string propertyName)
        {
            var baseErrroMessage = string.Format("The Setter for the Property '{0}' for class '{1}'", propertyName, this.BusinessObject.ClassDef.ClassName);
            return baseErrroMessage +
                   " is mapped to the incorrect BOProp. Check the Property in your code";
        }

        private string GetGetterAndSetterErrorMessage(string propertyName)
        {
            string className = this.BusinessObject.ClassDef.ClassName;
            var baseErrroMessage = string.Format("The Getter And Setter for the Property '{0}' for class '{1}'", propertyName, className);
            return baseErrroMessage +
                   " are not both mapped to the same BOProp. Check the Property in your code";
        }

        private void SetValueViaReflection(string propertyName, object validPropValue)
        {
            ReflectionUtilities.SetPropertyValue(this.BusinessObject, propertyName, validPropValue);
        }

        private object GetValueViaReflection(string propertyName)
        {
            return ReflectionUtilities.GetPropertyValue(this.BusinessObject, propertyName);
        }

        private void SetValueFromBO(string propertyName, object validValue)
        {
            if (HasPropDef(propertyName)) this.BusinessObject.SetPropertyValue(propertyName, validValue);
            SetRelationshipValue(this.BusinessObject, propertyName, validValue as IBusinessObject);
        }
*/
/*
        private static void SetRelationshipValue(IBusinessObject bo, string relationshipName, IBusinessObject validValue)
        {
            var relationship = GetSingleRelationship(bo, relationshipName);
            if(relationship == null) return;
            relationship.SetRelatedObject(validValue);
        }*/
/*
        private static ISingleRelationship GetSingleRelationship(IBusinessObject bo, string relationshipName)
        {
            return bo.Relationships.FirstOrDefault(rel => rel.RelationshipName == relationshipName) as ISingleRelationship;
        }*/
/*

        private object GetValueFromBO(string propertyName)
        { 
            if(HasPropDef(propertyName)) return this.BusinessObject.GetPropertyValue(propertyName);
            return GetRelationshipValue(this.BusinessObject, propertyName);
        }
*/

/*

        private static IBusinessObject GetRelationshipValue(IBusinessObject bo, string relationshipName)
        {
            var relationship = GetSingleRelationship(bo, relationshipName);
            return relationship == null ? null : relationship.GetRelatedObject();
        }
*/

        private bool HasPropDef(string propertyName)
        {
            IPropDef propDef = GetClassDef().GetPropDef(propertyName, false);
            return propDef != null;
        }
        private bool HasRelDef(string propertyName)
        {
            IRelationshipDef relationshipDef = GetClassDef().GetRelationship(propertyName);
            return relationshipDef != null;
        }

        /// <summary>
        /// Returns a <see cref="PropDefTester"/> for the property specified by propName
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        public PropDefTester GetPropTester(string propName)
        {
            IPropDef propDef = GetClassDef().GetPropDef(propName, false);
            return new PropDefTester(propDef);
        }
        /// <summary>
        /// Returns a <see cref="PropDefTester"/> for the property specified by propName
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        public SingleRelDefTester GetSingleRelationshipTester(string propName)
        {
            var relDef = GetClassDef().GetRelationship(propName) as ISingleRelationshipDef;
            return new SingleRelDefTester(relDef);
        }

        public void ShouldBeUniqueConstraint(string propName)
        {
            AssertIsPartOfUniqueConstraint(propName);
        }

        protected void AssertIsPartOfUniqueConstraint(string propName)
        {
            var classDef = this.BusinessObject.ClassDef;
            string message = string.Format(" '{0}' is not part of a Unique Constraint since there are no Unique Constraints Defined on the Class '{1}'"
                , propName, classDef.ClassName);

            classDef.KeysCol.ShouldNotBeEmpty(message);
            IEnumerable<IKeyDef> simpleUniqueConstraints = classDef.KeysCol.Where(def => def.Count == 1);
            foreach (IKeyDef keyDef in simpleUniqueConstraints)
            {
                foreach (IPropDef propDef in keyDef)
                {
                    if (propDef.PropertyName == propName) return;
                }
            }
            Assert.Fail("'{0}' is not part of any Unique Constraint");
        }


        private PropertyInfo GetPropertyInfo(string propName)
        {
            return ReflectionUtilities.GetPropertyInfo(this.BusinessObject.GetType(), propName);
        }
    }
    /// <summary>
    /// This is a specialised Tester for testing Habanero.<see cref="IPropDef"/> 
    /// This tester provides methods for testing the basic attributes of a <see cref="IPropDef"/>
    /// such as ShouldBeCompulsory.
    /// </summary>
    public class BOTester<T> : BOTester where T: class, IBusinessObject
    {
        public BOTester() : base(new BOTestFactory<T>().CreateDefaultBusinessObject())
        {
        }
        /// <summary>
        /// Tests that the Mapping within the Property Gettter and Setter
        /// are mapped correctly i.e. the string "ProspectTypeName" in 
        /// the Getter and the string "ProspectTypeName" in the setter are 
        /// both the same and they are mapped to a valid property Def in the ClassDef.
        /// <code>
        ///  public virtual string ProspectTypeName
        ///  {
        ///      get { return ((string)(base.GetPropertyValue("ProspectTypeName"))); }
        ///      set { base.SetPropertyValue("ProspectTypeName", value); }
        ///  }
        /// </code>
        /// This is done by setting the Property via reflection (if it has a set)
        /// and setting it via reflection (if it has a get) and comparing these values
        /// to the values for the BOProp to ensure that all mappings are correct.
        /// </summary>
        /// <param name="propExpression"></param>
        public void ShouldHavePropertyMapped(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldHavePropertyMapped(propertyName);
        }

        /// <summary>
        /// Tests that the Mapping within the Property Gettter and Setter
        /// of a relationship are mapped correctly i.e. the string "SingleRelGetterNotMapped" in 
        /// the Getter and the string "SingleRelationship" in the setter are 
        /// both the same and they are mapped to a valid <see cref="IRelationshipDef"/> in the ClassDef
        /// <see cref="IClassDef"/>.
        /// <code>
        ///   public virtual FakeBO SingleRelationship
        ///   {
        ///       get { return Relationships.GetRelatedObject{FakeBO}("SingleRelationship"); }
        ///       set { Relationships.SetRelatedObject("SingleRelationship", value); }
        ///   }
        /// </code>
        /// This is done by setting the Property via reflection (if it has a set)
        /// and setting it via reflection (if it has a get) and comparing these values
        /// to the values for the BOProp to ensure that all mappings are correct.
        /// </summary>
        /// <param name="singleRelExpression"></param>
        public void ShouldHaveSingleRelationshipMapped(Expression<Func<T, IBusinessObject>> singleRelExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(singleRelExpression);
            ShouldHaveSingleRelationshipMapped(propertyName);
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
        /// <param name="expectedDefaultValue"></param>
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
        public void ShouldBeUniqueConstraint(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldBeUniqueConstraint(propertyName);
        }
        /// <summary>
        /// Returns a <see cref="PropDefTester"/> for the property specified by <paramref name="propExpression"/>
        /// </summary>
        /// <param name="propExpression"></param>
        /// <returns></returns>
        public PropDefTester GetPropTester(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);

            return base.GetPropTester(propertyName);
        }
        /// <summary>
        /// Returns a <see cref="PropDefTester"/> for the property specified by <paramref name="propExpression"/>
        /// </summary>
        /// <param name="propExpression"></param>
        /// <returns></returns>
        public SingleRelDefTester GetSingleRelationshipTester(Expression<Func<T, IBusinessObject>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);

            return base.GetSingleRelationshipTester(propertyName);
        }
    }
}