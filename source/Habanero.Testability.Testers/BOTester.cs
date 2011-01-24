using System;
using System.Collections.Generic;
using System.Linq;
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
    /// If any of these Asserts fail then an <see cref="AssertionException"/>. is thrown.
    /// Else the Assert executes without an Exception
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
        /// <summary>
        /// Asserts that the property identified by propName has the expectedReadWriteRule
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="expectedReadWriteRule"></param>
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
        /// <summary>
        /// Asserts that all single relationships are mapped correctly
        /// </summary>
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

            GetTester(propertyName).ShouldHavePropertyMapped();
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

        protected IClassDef GetClassDef()
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
        /// <summary>
        /// Asserts that the property identified by <paramref name="propName"/> is a Unique Constraint
        /// </summary>
        /// <param name="propName"></param>
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
}