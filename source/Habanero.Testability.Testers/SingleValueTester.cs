using System;
using System.Linq;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.Testability.Testers
{
    /// <summary>
    /// This is a specialised Tester for testing Habanero.<see cref="ISingleValueDef"/> 
    /// This tester provides methods for testing the basic attributes of a <see cref="ISingleValueDef"/>
    /// such as ShouldBeIsCompulsory.
    /// If any of these Asserts fail then an <see cref="AssertionException"/>. is thrown.
    /// Else the Assert executes without an Exception
    /// </summary>
    public abstract class SingleValueTester
    {
        private BOTestFactory _boTestFactory;
        private IBusinessObject _businessObject;
        /// <summary>
        /// The <see cref="ISingleValueDef"/> that is being tested by this Tester
        /// </summary>
        public abstract ISingleValueDef SingleValueDef { get; }

        ///<summary>
        /// The <see cref="ISingleValueDef"/> i.e. <see cref="ISingleRelationshipDef"/> or <see cref="IPropDef"/> should be defined as compulsory
        ///</summary>
        public void ShouldBeCompulsory()
        {
            var errMessage = BaseMessage + " should be compulsory";
            Assert.IsTrue(this.SingleValueDef.Compulsory, errMessage);
        }
        ///<summary>
        /// The <see cref="ISingleValueDef"/> i.e. <see cref="ISingleRelationshipDef"/> or <see cref="IPropDef"/> should be defined as compulsory
        ///</summary>
        public void ShouldNotBeCompulsory()
        {
            var errMessage = BaseMessage + " should not be compulsory";
            Assert.IsFalse(this.SingleValueDef.Compulsory, errMessage);
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
        /// 
        /// Or in the case of a single relationship
        /// Should be mapped to a valid <see cref="ISingleRelationshipDef"/> in the ClassDef
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
        public void ShouldHavePropertyMapped()
        {
            try
            {
                if (IsPropWriteNotNew())
                {
                    //You cannot set WriteNotNew values if the Business Object is new so we have to ensure we have a saved one.
                    this._businessObject = GetTestFactory().CreateSavedBusinessObject();
                }
                var className = this.ClassName;
                var propertyInfo = GetPropertyInfo(PropertyName);
                AssertPropInfoNotNull(propertyInfo, className, PropertyName);
                //if(this.SingleValueDef.)
                if (propertyInfo.CanWrite)
                {
                    if (propertyInfo.CanRead)
                    {
                        AssertGetterAndSetterSameMapping();
                    }
                    AssertSetterMappedToCorrectBOProp();
                }
                if (propertyInfo.CanRead)
                {
                    AssertGetterMappedToCorrectBOProp(className);
                }
            }
            finally
            {
                _businessObject = null;
            }
        }

        private bool IsPropWriteNotNew()
        {
            return this.SingleValueDef.ReadWriteRule == PropReadWriteRule.WriteNotNew;
        }

        private BOTestFactory GetTestFactory()
        {
            if (_boTestFactory == null)
            {
                _boTestFactory = BOTestFactoryRegistry.Instance.Resolve(ClassType);
            }
            return _boTestFactory;
        }

        private void AssertGetterMappedToCorrectBOProp(string className)
        {
            var expectedValue = GetTestFactory().GetValidValue(PropertyName);
            //this.BusinessObject.SetPropertyValue(propertyName, validPropValue);
            SetValueFromBO(PropertyName, expectedValue);
            var valueViaReflection = GetValueViaReflection(PropertyName);

            Assert.AreEqual(expectedValue, valueViaReflection, GetGetterMappedErrorMessage(PropertyName, className));
        }

        private void AssertSetterMappedToCorrectBOProp()
        {
            object expectedValue = GetTestFactory().GetValidValue( PropertyName);
            SetValueViaReflection(PropertyName, expectedValue);
            var valueFromBO = GetValueFromBO(PropertyName);
            Assert.AreEqual(expectedValue, valueFromBO, GetSetterMappedErrorMessage(PropertyName));
        }

        private void AssertGetterAndSetterSameMapping()
        {
            var expectedValue = GetTestFactory().GetValidValue(PropertyName);
            SetValueViaReflection(PropertyName, expectedValue);
            var valueViaReflection = GetValueViaReflection(PropertyName);
            Assert.AreEqual(expectedValue, valueViaReflection, GetGetterAndSetterErrorMessage(PropertyName));
        }

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
            try
            {
                ReflectionUtilities.SetPropertyValue(this.BusinessObject, propertyName, validPropValue);
            }
            catch (Exception ex)
            {
                Assert.Fail(GetSetterMappedErrorMessage(propertyName) + Environment.NewLine
                        + " Setting the Property via reflection failed" + Environment.NewLine
                        + "Reflection Error : " + ex.Message);
            }
        }

        protected IBusinessObject BusinessObject
        {
            get
            {
                if(_businessObject == null)
                {
                    _businessObject = GetTestFactory().CreateDefaultBusinessObject();
                }
                return _businessObject;
            }
        }

        private static ISingleRelationship GetSingleRelationship(IBusinessObject bo, string relationshipName)
        {
            return bo.Relationships.FirstOrDefault(rel => rel.RelationshipName == relationshipName) as ISingleRelationship;
        }
        private static IBusinessObject GetRelationshipValue(IBusinessObject bo, string relationshipName)
        {
            var relationship = GetSingleRelationship(bo, relationshipName);
            return relationship == null ? null : relationship.GetRelatedObject();
        }
        private bool HasPropDef(string propertyName)
        {
            var propDef = ClassDef.GetPropDef(propertyName, false);
            return propDef != null;
        }
        private object GetValueFromBO(string propertyName)
        {
            if (HasPropDef(propertyName)) return this.BusinessObject.GetPropertyValue(propertyName);
            return GetRelationshipValue(this.BusinessObject, propertyName);
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

        private static void SetRelationshipValue(IBusinessObject bo, string relationshipName, IBusinessObject validValue)
        {
            var relationship = GetSingleRelationship(bo, relationshipName);
            if (relationship == null) return;
            relationship.SetRelatedObject(validValue);
        }

        private static void AssertPropInfoNotNull(PropertyInfo propertyInfo, string className, string propertyName)
        {
            Assert.IsNotNull(propertyInfo, "The Property '{0}' does not exist on the class '{1}'. Please check your code.", propertyName, className);
        }

        protected virtual string BaseMessage
        {
            get { return string.Format("The Property '{0}' for class '{1}'", PropertyName, ClassName); }
        }

        protected virtual string ClassName
        {
            get { return this.SingleValueDef.ClassName; }
        }
        /// <summary>
        /// The Property name of the BOProp or SingleRelationship that is being tested.
        /// </summary>
        public virtual string PropertyName
        {
            get { return this.SingleValueDef.PropertyName; }
        }

        protected virtual Type ClassType
        {
            get { return this.ClassDef.ClassType; }
        }

        protected virtual IClassDef ClassDef
        {
            get { return this.SingleValueDef.ClassDef; }
        }

        protected virtual PropertyInfo GetPropertyInfo(string propName)
        {
            return ReflectionUtilities.GetPropertyInfo(this.ClassDef.ClassType, propName);
        }
    }
}