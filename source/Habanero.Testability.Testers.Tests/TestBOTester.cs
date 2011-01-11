using System;
using System.Reflection;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Testability.Helpers;
using Habanero.Testability.Tests.Base;
using Habanero.Util;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Testability.Testers.Tests
{ 
     // ReSharper disable InconsistentNaming 
    /// <summary>
    /// For success these tests tests that no error was thrown since any NUnit Assert Failure raises the AssertionException.
    /// </summary>
    [TestFixture]
    public class TestBOTester
    {
        [SetUp]
        public void SetUpTest()
        {
            ClassDef.ClassDefs.Clear();
            BORegistry.DataAccessor = new DataAccessorInMemory();
        }

        protected virtual BOTester CreateTester<T>() where T : class, IBusinessObject
        {
            T bo = new BOTestFactory<T>().CreateDefaultBusinessObject();
            return new BOTester(bo);
        }

        [Test]
        public void Test_CreateBOPropTester()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            BOTester tester = new BOTester(MockRepository.GenerateMock<IBusinessObject>());
            //---------------Test Result -----------------------
            Assert.IsNotNull(tester.BusinessObject);
        }

        [Test]
        public void Test_Construct_WithNullpropDef_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                new BOTester(null);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("businessObject", ex.ParamName);
            }
        }

        [Test]
        public void Test_GetPropTester_WhenHasPropDef_ShouldReturnPropTesterForPropDef()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<FakeBOWithAllPropsMapped>();
            const string propName = "NonCompulsoryString";
            var boTester = CreateTester<FakeBOWithAllPropsMapped>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var propTester = boTester.GetPropTester(propName);
            //---------------Test Result -----------------------
            Assert.IsNotNull(propTester);
            Assert.AreEqual(propName, propTester.PropertyName);
        }
        [Test]
        public void Test_GetSingleRelationshipTester_WhenHasRelationshipDef_ShouldReturnRelTesterForRelDef()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<FakeBoWithSingleRel>();
            const string relationshipName = "SingleRelMapped";

            var boTester = CreateTester<FakeBoWithSingleRel>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var singleRelationshipTester = boTester.GetSingleRelationshipTester(relationshipName);
            //---------------Test Result -----------------------
            Assert.IsNotNull(singleRelationshipTester);
            Assert.AreEqual(relationshipName, singleRelationshipTester.RelationshipName);
        }
        
        /// <summary>
        /// This tests that no error was thrown since any NUnit Assert Failure raises the AssertionException.
        /// </summary>
        [Test]
        public void Test_ShouldHaveAllPropsMapped_WhenIsMapped_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<FakeBOWithAllPropsMapped>();
            var boTester = CreateTester<FakeBOWithAllPropsMapped>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTester.ShouldHaveAllPropsMapped();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveAllPropsMapped_WhenNotIsMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDefWithAddProperty<FakeBoWithOnePropIncorrectlyMapped>();
            var boTester = CreateTester<FakeBoWithOnePropIncorrectlyMapped>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldHaveAllPropsMapped();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected =
                    string.Format(
                        "The Getter And Setter for the Property '{0}' for class '{1}' are not both mapped to the same BOProp. Check the Property in your code",
                        "GetterNotMapped", "FakeBoWithOnePropIncorrectlyMapped");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveAllPropsMapped_WhenPropWithNoSetter_GetterCorrectlyMapped_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<FakeBOWithNoSetter>();
            var boTester = CreateTester<FakeBOWithNoSetter>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boTester.ShouldHaveAllPropsMapped();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveAllPropsMapped_WhenPropWithNoSetter_GetterNotCorrectlyMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDefWithAddProperty<FakeBOWithNoSetterGetterIncorrectlyMapped>();
            var boTester = CreateTester<FakeBOWithNoSetterGetterIncorrectlyMapped>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldHaveAllPropsMapped();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Getter for the Property '{0}' for class '{1}'",
                                                "NonCompulsoryString", "FakeBOWithNoSetterGetterIncorrectlyMapped");
                StringAssert.Contains(expected, ex.Message);
                StringAssert.Contains("is not mapped to the correct BOProp. Check the Property in your code", ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveAllPropsMapped_WhenPropWithNoGetter_SetterNotCorrectlyMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var classDef = SetupClassDefWithAddProperty<FakeBOWithNoGetterSetterIncorrectlyMapped>();
            const string propName = "NonCompulsoryString";
            classDef.AddPropDef(propName);
            var boTester = CreateTester<FakeBOWithNoGetterSetterIncorrectlyMapped>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldHaveAllPropsMapped();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Setter for the Property '{0}' for class '{1}'", propName,
                                                "FakeBOWithNoGetterSetterIncorrectlyMapped");
                StringAssert.Contains(expected, ex.Message);
                StringAssert.Contains("is mapped to the incorrect BOProp. Check the Property in your code", ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveAllPropsMapped_WhenPropWithNoGetter_SetterCorrectlyMapped_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<FakeBOWithNoGetter>();
            var boTester = CreateTester<FakeBOWithNoGetter>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boTester.ShouldHaveAllPropsMapped();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveAllPropsMapped_WhenPropWithNoSetterOrGetter_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDefWithAddProperty<FakeBOWithNoSetter>();
            var boTester = CreateTester<FakeBOWithNoSetter>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boTester.ShouldHaveAllPropsMapped();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHavePropertyMapped_WhenIsMapped_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<FakeBOWithIncorrectMappings>();
            var boTester = CreateTester<FakeBOWithIncorrectMappings>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTester.ShouldHavePropertyMapped("PropertyMappedCorrectly");
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        /// <summary>
        /// If is perfectly legitimate to have a property that is defined in the class def but which does not exist on the 
        /// the Business object e.g. you may not want the foreign key prop on yr BO but it is needed in the ClassDef since it
        /// is needed by the relationship. But if you are testing an individual Property then it is 
        /// assumed that you are expecting it to fail if there are no property get; and set;
        /// <see cref="BOTester.ShouldHaveAllPropsMapped"/>
        /// </summary>
        [Test]
        public void Test_ShouldHavePropertyMapped_WhenPropNotDefinedOnClass_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            const string propertyName = "InvalidPropName";
            SetupClassDefWithAddProperty<FakeBOWithIncorrectMappings>();
            var boTester = CreateTester<FakeBOWithIncorrectMappings>();
            //---------------Assert Precondition----------------
            PropertyInfo propertyInfo = GetPropertyInfo<FakeBOWithIncorrectMappings>(propertyName);
            Assert.IsNull(propertyInfo);
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldHavePropertyMapped(propertyName);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected =
                    string.Format(
                        "The Property '{0}' does not exist on the class '{1}'",
                        propertyName, "FakeBOWithIncorrectMappings");
                StringAssert.Contains(expected, ex.Message);
            }
        }
        [Test]
        public void Test_ShouldHavePropertyMapped_WhenGetterNotMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            const string propertyName = "GetterNotMapped";
            SetupClassDefWithAddProperty<FakeBOWithIncorrectMappings>();
            var newValue = GetRandomString();
            var boTester = CreateTester<FakeBOWithIncorrectMappings>();
            FakeBOWithIncorrectMappings bo = (FakeBOWithIncorrectMappings) boTester.BusinessObject;
            bo.GetterNotMapped = newValue;
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(newValue, bo.GetterNotMapped);
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldHavePropertyMapped(propertyName);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected =
                    string.Format(
                        "The Getter And Setter for the Property '{0}' for class '{1}' are not both mapped to the same BOProp. Check the Property in your code",
                        propertyName, "FakeBOWithIncorrectMappings");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHavePropertyMapped_WhenSetterNotMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            const string propertyName = "SetterNotMapped";
            SetupClassDefWithAddProperty<FakeBOWithIncorrectMappings>();
            var boTester = CreateTester<FakeBOWithIncorrectMappings>();
            FakeBOWithIncorrectMappings bo = (FakeBOWithIncorrectMappings) boTester.BusinessObject;
            var newValue = GetRandomString();
            bo.SetterNotMapped = newValue;
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(newValue, bo.SetterNotMapped);
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldHavePropertyMapped(propertyName);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected =
                    string.Format(
                        "The Getter And Setter for the Property '{0}' for class '{1}' are not both mapped to the same BOProp. Check the Property in your code",
                        propertyName, "FakeBOWithIncorrectMappings");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHavePropertyMapped_WhenGetterAndSetterMappedToIncorrectprop_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDefWithAddProperty<FakeBOWithIncorrectMappings>();
            const string propName = "GetterAndSetterMappedToIncorrectBOProp";
            var boTester = CreateTester<FakeBOWithIncorrectMappings>();
            FakeBOWithIncorrectMappings bo = (FakeBOWithIncorrectMappings) boTester.BusinessObject;
            var newValue = GetRandomString();
            bo.GetterAndSetterMappedToIncorrectBOProp = newValue;
            var boProp = bo.Props[propName];
            //---------------Assert Precondition----------------
            Assert.AreEqual(newValue, bo.GetterAndSetterMappedToIncorrectBOProp);
            Assert.AreNotEqual(boProp.Value, newValue);
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldHavePropertyMapped(propName);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected =
                    string.Format(
                        "The Setter for the Property '{0}' for class '{1}' is mapped to the incorrect BOProp. Check the Property in your code",
                        propName, "FakeBOWithIncorrectMappings");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveDefault_WhenNotHasDefault_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDefWithAddProperty<BOFakeWithDefault>();
            const string propName = "NonDefaultProp";
            var boTester = CreateTester<BOFakeWithDefault>();
            BOFakeWithDefault bo = (BOFakeWithDefault) boTester.BusinessObject;
            var newValue = GetRandomString();
            var boProp = bo.Props[propName];
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(boProp.Value, newValue);
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldHaveDefault(propName);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format(
                    "The Property '{0}' for class '{1}' should have a default but does not", propName,
                    "BOFakeWithDefault");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldNotHaveDefault_WhenNotHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithDefault>();
            var boTester = CreateTester<BOFakeWithDefault>();
            const string propName = "NonDefaultProp";

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTester.ShouldNotHaveDefault(propName);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldNotHaveDefault_WhenHasDefault_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDefWithAddProperty<BOFakeWithDefault>();
            const string propName = "DefaultProp";
            var boTester = CreateTester<BOFakeWithDefault>();
            //---------------Assert Precondition----------------
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldNotHaveDefault(propName);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format(
                    "The Property '{0}' for class '{1}' should not have a default but does", propName,
                    "BOFakeWithDefault");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveDefault_WhenHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithDefault>();
            var boTester = CreateTester<BOFakeWithDefault>();
            const string propName = "DefaultProp";
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTester.ShouldHaveDefault(propName);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveDefault_WithSpecifiedValue_WhenValueNotEqual_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDef<BOFakeWithDefault>();
            var boTester = CreateTester<BOFakeWithDefault>();
            const string defaultValueString = "SomeOtherValue";
            const string propName = "DefaultProp";
            //---------------Assert Precondition----------------
            IPropDef propDef = classDef.GetPropDef(propName);
            Assert.IsNotNullOrEmpty(propDef.DefaultValueString);
            Assert.AreNotEqual(defaultValueString, propDef.DefaultValueString);
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldHaveDefault(propName, defaultValueString);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format(
                    "The Property '{0}' for class '{1}' should have a default of '{2}' but has a default value of '{3}'",
                    propName,
                    propDef.ClassName, defaultValueString, propDef.DefaultValueString);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveDefault_WithSpecifiedValue_WhenHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDef<BOFakeWithDefault>();
            var boTester = CreateTester<BOFakeWithDefault>();
            const string propName = "DefaultProp";
            const string defaultValueString = "SomeValue";
            //---------------Assert Precondition----------------
            IPropDef propDef = classDef.GetPropDef(propName);
            Assert.IsNotNullOrEmpty(propDef.DefaultValueString);
            Assert.AreEqual(defaultValueString, propDef.DefaultValueString);
            //---------------Execute Test ----------------------
            boTester.ShouldHaveDefault(propName, defaultValueString);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldBeCompulsory_WhenCompulsory_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithCompulsory>();
            var boTester = CreateTester<BOFakeWithCompulsory>();
            const string propName = "CompulsoryProp";
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<BOFakeWithCompulsory, object>(bo => bo.CompulsoryProp);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(propertyWrapper.HasAttribute<AutoMapCompulsoryAttribute>());
            //---------------Execute Test ----------------------
            boTester.ShouldBeCompulsory(propName);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldBeCompulsory_WhenNotCompulsory_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithCompulsory>();
            var boTester = CreateTester<BOFakeWithCompulsory>();
            const string propName = "NonCompulsoryProp";
            var propertyInfo =
                ReflectionUtilities.GetPropertyInfo<BOFakeWithCompulsory, object>(bo => bo.NonCompulsoryProp);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsFalse(propertyWrapper.HasAttribute<AutoMapCompulsoryAttribute>());
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldBeCompulsory(propName);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Property '{0}' for class '{1}' should be compulsory", propName,
                                                "BOFakeWithCompulsory");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldNotBeCompulsory_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithCompulsory>();
            var boTester = CreateTester<BOFakeWithCompulsory>();
            const string propName = "NonCompulsoryProp";
            var propertyInfo =
                ReflectionUtilities.GetPropertyInfo<BOFakeWithCompulsory, object>(bo => bo.NonCompulsoryProp);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsFalse(propertyWrapper.HasAttribute<AutoMapCompulsoryAttribute>());
            //---------------Execute Test ----------------------
            boTester.ShouldNotBeCompulsory(propName);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldNotBeCompulsory_WhenCompulsory_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDef<BOFakeWithCompulsory>();
            var boTester = CreateTester<BOFakeWithCompulsory>();
            const string propName = "CompulsoryProp";
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<BOFakeWithCompulsory, object>(bo => bo.CompulsoryProp);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(propertyWrapper.HasAttribute<AutoMapCompulsoryAttribute>());
            Assert.IsNotNull(classDef.GetPropDef(propName));
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldNotBeCompulsory(propName);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Property '{0}' for class '{1}' should not be compulsory", propName,
                                                "BOFakeWithCompulsory");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveReadWriteRule_ReadOnly_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDef<FakeBOWithReadWriteRuleProp>();
            var boTester = CreateTester<FakeBOWithReadWriteRuleProp>();
            const string propName = "ReadWriteRuleReadOnly";
            var propertyInfo =
                ReflectionUtilities.GetPropertyInfo<FakeBOWithReadWriteRuleProp, object>(bo => bo.ReadWriteRuleReadOnly);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(propertyWrapper.HasAttribute<AutoMapReadWriteRuleAttribute>());
            Assert.IsNotNull(classDef.GetPropDef(propName));
            //---------------Execute Test ----------------------
            boTester.ShouldHaveReadWriteRule(propName, PropReadWriteRule.ReadOnly);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveReadWriteRule_ReadOnly_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDef<FakeBOWithReadWriteRuleProp>();
            var boTester = CreateTester<FakeBOWithReadWriteRuleProp>();
            const string propName = "ReadWriteRuleReadOnly";
            var propertyInfo =
                ReflectionUtilities.GetPropertyInfo<FakeBOWithReadWriteRuleProp, object>(bo => bo.ReadWriteRuleReadOnly);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            const PropReadWriteRule expectedReadWriteRule = PropReadWriteRule.WriteNew;
            //---------------Assert Precondition----------------
            Assert.IsTrue(propertyWrapper.HasAttribute<AutoMapReadWriteRuleAttribute>());
            IPropDef propDef = classDef.GetPropDef(propName);
            Assert.IsNotNull(propDef);
            Assert.AreNotEqual(expectedReadWriteRule, propDef.ReadWriteRule);
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldHaveReadWriteRule(propName, expectedReadWriteRule);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format(
                    "The Property '{0}' for class '{1}' should have a ReadWriteRule '{2}' but is '{3}'",
                    propDef.PropertyName,
                    propDef.ClassName, expectedReadWriteRule, propDef.ReadWriteRule);
                StringAssert.Contains(expected, ex.Message);
            }
        }





        #region UniqueConstraint

        [Test]
        public void Test_ShouldBeUniqueConstraint_WhenSingleProp_WhenHas_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            const string propName = "UCProp";
            IClassDef classDef = SetupClassDef<FakeBOWithUniqueConstraint>();
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<FakeBOWithUniqueConstraint, object>(bo => bo.UCProp);
            var boTester = CreateTester<FakeBOWithUniqueConstraint>();
            //---------------Assert Precondition----------------
            propertyInfo.AssertHasUniqueConstraintAttribute();
            Assert.IsNotNull(classDef.GetPropDef(propName), "Property should be defined");
            //---------------Execute Test ----------------------
            boTester.ShouldBeUniqueConstraint(propName);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldBeUniqueConstraint_WhenClassHasNoUC_ShouldRaiseAssertionException()
        {
            //---------------Set up test pack-------------------
            const string propName = "CompulsoryString";
            IClassDef classDef = SetupClassDef<FakeBO>();
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<FakeBO, object>(bo => bo.CompulsoryString);
            var boTester = CreateTester<FakeBO>();
            //---------------Assert Precondition----------------
            propertyInfo.AssertNotHasUniqueConstraintAttribute();
            Assert.IsNotNull(classDef.GetPropDef(propName), "Property should be defined");
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldBeUniqueConstraint(propName);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                const string expected = "no Unique Constraints Defined";
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldBeUniqueConstraint_WhenClassHasUC_WhenPropNotOnUC_ShouldRaiseAssertionException()
        {
            //---------------Set up test pack-------------------
            const string propName = "NonUCProp";
            IClassDef classDef = SetupClassDef<FakeBOWithUniqueConstraint>();
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<FakeBOWithUniqueConstraint, object>(bo => bo.NonUCProp);
            var boTester = CreateTester<FakeBOWithUniqueConstraint>();
            //---------------Assert Precondition----------------
            propertyInfo.AssertNotHasUniqueConstraintAttribute();
            Assert.IsNotNull(classDef.GetPropDef(propName), "Property should be defined");
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldBeUniqueConstraint(propName);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                const string expected = "is not part of any Unique Constraint";
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldBeUniqueConstraint_WhenClassHasCompositeUC_WhenPropNotOnCompositeUC_ShouldRaiseAssertionException()
        {
            //---------------Set up test pack-------------------
            const string propName = "ComplexUCProp1";
            IClassDef classDef = SetupClassDef<FakeBOWithUniqueConstraint>();
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<FakeBOWithUniqueConstraint, object>(bo => bo.ComplexUCProp1);
            var boTester = CreateTester<FakeBOWithUniqueConstraint>();
            //---------------Assert Precondition----------------
            propertyInfo.AssertHasUniqueConstraintAttribute();
            Assert.IsNotNull(classDef.GetPropDef(propName), "Property should be defined");
            Assert.AreEqual(2, classDef.KeysCol["UC2"].Count);
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldBeUniqueConstraint(propName);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                //TODO brett 01 Oct 2010: const string expected = "is part of a complex Unique Constraint 'UC2' and not a simple UC";
                const string expected = "is not part of any Unique Constraint";
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Ignore("To Implement tests for Composite")] //TODO Brett 01 Oct 2010: Ignored Test - To Implement tests for Composite
        [Test]
        public void Test_ShouldBeUniqueConstraint_WithMultiProps_WhenHasCompositeUC()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Not Yet Implemented");
        }
        #endregion

        #region Relationship

        [Test]
        public void Test_ShouldRelationshipBeMapped_WhenSingleRelationship_WhenMapped_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRel>();
            var boTester = CreateTester<FakeBoWithSingleRel>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTester.ShouldHaveSingleRelationshipMapped("SingleRelMapped");
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveSingleRelationshipMapped_WhenRelationshipIsNotDeclaredOnTheClass_ShouldFail()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRel>();
            const string relationshipName = "InvalidRelName";
            var boTester = CreateTester<FakeBoWithSingleRel>();
            //---------------Assert Precondition----------------
            var propertyInfo = GetPropertyInfo <FakeBoWithSingleRel>(relationshipName);
            Assert.IsNull(propertyInfo);
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldHaveSingleRelationshipMapped(relationshipName);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected =
                    string.Format(
                        "The Property '{0}' does not exist on the class '{1}'",
                        relationshipName, "FakeBoWithSingleRel");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveSingleRelationshipMapped_WhenGetterNotMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRel>();
            const string relationshipName = "SingleRelGetterNotMapped";
            var newValue = new FakeBOWithNothing();
            var boTester = CreateTester<FakeBoWithSingleRel>();
            FakeBoWithSingleRel bo = (FakeBoWithSingleRel)boTester.BusinessObject;
            bo.SingleRelGetterNotMapped = newValue;
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(newValue, bo.SingleRelGetterNotMapped);
            //---------------Execute Test -----------------------
            try
            {
                boTester.ShouldHaveSingleRelationshipMapped(relationshipName);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected =
                    string.Format(
                        "The Getter And Setter for the Property '{0}' for class '{1}' are not both mapped to the same BOProp. Check the Property in your code",
                        relationshipName, "FakeBoWithSingleRel");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveSingleRelationshipMapped_WhenSetterNotMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            const string propertyName = "SingleRelSetterNotMapped";
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRel>();
            var boTester = CreateTester<FakeBoWithSingleRel>();
            FakeBoWithSingleRel bo = (FakeBoWithSingleRel)boTester.BusinessObject;
            var newValue = new FakeBOWithNothing();
            bo.SingleRelSetterNotMapped = newValue;
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(newValue, bo.SingleRelSetterNotMapped);
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldHaveSingleRelationshipMapped(propertyName);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected =
                    string.Format(
                        "The Getter And Setter for the Property '{0}' for class '{1}' are not both mapped to the same BOProp. Check the Property in your code",
                        propertyName, "FakeBoWithSingleRel");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveSingleRelationshipMapped_WhenGetterAndSetterMappedToIncorrectprop_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRel>();
            const string propName = "SingleRelGetterAndSetterNotMapped";
            var boTester = CreateTester<FakeBoWithSingleRel>();
            FakeBoWithSingleRel bo = (FakeBoWithSingleRel)boTester.BusinessObject;
            var newValue = new FakeBOWithNothing();
            bo.SingleRelGetterAndSetterNotMapped = newValue;
            var singleRelationship = bo.Relationships[propName] as ISingleRelationship;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(singleRelationship);
            Assert.AreEqual(newValue, bo.SingleRelGetterAndSetterNotMapped);
            Assert.AreNotEqual(singleRelationship.GetRelatedObject(), newValue);
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldHaveSingleRelationshipMapped(propName);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected =
                    string.Format(
                        "The Setter for the Property '{0}' for class '{1}' is mapped to the incorrect BOProp. Check the Property in your code",
                        propName, "FakeBoWithSingleRel");
                StringAssert.Contains(expected, ex.Message);
            }
        }


        [Test]
        public void Test_ShouldHaveSingleRelationshipMapped_WhenPropWithNoSetter_GetterCorrectlyMapped_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRelNoSetter>();
            var boTester = CreateTester<FakeBoWithSingleRelNoSetter>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boTester.ShouldHaveSingleRelationshipMapped("SingleRelMapped");
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveSingleRelationshipMapped_WhenPropWithNoSetterOrGetter_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRelNoSetter>();
            var boTester = CreateTester<FakeBoWithSingleRelNoSetter>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boTester.ShouldHaveSingleRelationshipMapped("SingleRelMapped");
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveSingleRelationshipMapped_WhenPropWithNoGetter_SetterNotCorrectlyMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRel>();
            const string propName = "SingleRelNoGetterSetterIncorrect";
            var classDef = ClassDef.Get<FakeBoWithSingleRel>();
            classDef.AddSingleRelDef(propName, typeof(FakeBOWithNothing));//This is necessary since smooth does not automap a relationship that does not 
            // have a Getter.
            var boTester = CreateTester<FakeBoWithSingleRel>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldHaveSingleRelationshipMapped(propName);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Setter for the Property '{0}' for class '{1}'", propName,
                                                "FakeBoWithSingleRel");
                StringAssert.Contains(expected, ex.Message);
                StringAssert.Contains("is mapped to the incorrect BOProp. Check the Property in your code", ex.Message);
            }
        }


        [Test]
        public void Test_ShouldHaveSingleRelationshipMapped_WhenPropWithNoSetter_GetterNotCorrectlyMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            const string relName = "SingleRelNoSetterGetterIncorrect";
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRel>();
            var classDef = ClassDef.Get<FakeBoWithSingleRel>();
            var boTester = CreateTester<FakeBoWithSingleRel>();
            //---------------Assert Precondition----------------
            classDef.ShouldHaveSingleRelationshipDef(relName);
            classDef.ShouldHavePropertyInfo(relName);
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldHaveSingleRelationshipMapped(relName);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Getter for the Property '{0}' for class '{1}'",
                                                relName, "FakeBoWithSingleRel");
                StringAssert.Contains(expected, ex.Message);
                StringAssert.Contains("is not mapped to the correct BOProp. Check the Property in your code", ex.Message);
            }
        }


        /// <summary>
        /// Even though this Single Relationship has no property Getters or Setters for the SingleRelMapped
        /// Relationship defined in the ClassDef this should still pass.
        /// It is perfectly legitimate to have a Relationship defined in the ClassDef but
        /// not have a Getter and Setter for it in the Code.
        /// </summary>
        [Test]
        public void Test_ShouldHaveAllSingleRelationshipsMapped_WhenPropWithNoGetter_SetterCorrectlyMapped_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBOWithNoGetter>();
            var classDef = ClassDef.Get<FakeBOWithNoGetter>();
            classDef.AddSingleRelDef("SingleRelMapped");
            var boTester = CreateTester<FakeBOWithNoGetter>();
            //---------------Assert Precondition----------------
            classDef.ShouldHaveSingleRelationshipDef("SingleRelMapped");
            classDef.ShouldNotHavePropertyInfo("SingleRelMapped");
            //---------------Execute Test ----------------------
            boTester.ShouldHaveAllSingleRelationshipsMapped();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }
        /// <summary>
        /// The Should All Single RelationshipsBeMatched should exclude Multiple Relationships.
        /// </summary>
        [Test]
        public void Test_ShouldHaveAllSingleRelationshipsMapped_WhenHasMultipleRel_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBoWithInvalidMultipleRelAndSingleRels>();
            var classDef = ClassDef.Get<FakeBoWithInvalidMultipleRelAndSingleRels>();
            var boTester = CreateTester<FakeBoWithInvalidMultipleRelAndSingleRels>();
            //---------------Assert Precondition----------------
            classDef.ShouldHaveSingleRelationshipDef("SingleRelMapped");
            classDef.ShouldHaveMultipleRelationshipDef("InvalidMultipeRel");
            classDef.ShouldHavePropertyInfo("InvalidMultipeRel");
            //---------------Execute Test ----------------------
            boTester.ShouldHaveAllSingleRelationshipsMapped();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveAllRelsMapped_WhenIsMapped_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRelNoGetter>();
            var boTester = CreateTester<FakeBoWithSingleRelNoGetter>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTester.ShouldHaveAllSingleRelationshipsMapped();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveAllRelsMapped_WhenGetterNotIsMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRelGetterNotMapped>();
            var boTester = CreateTester<FakeBoWithSingleRelGetterNotMapped>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldHaveAllSingleRelationshipsMapped();
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected =
                    string.Format(
                        "The Getter And Setter for the Property '{0}' for class '{1}' are not both mapped to the same BOProp. Check the Property in your code",
                        "SingleRelGetterNotMapped", "FakeBoWithSingleRelGetterNotMapped");
                StringAssert.Contains(expected, ex.Message);
            }
        }


        #endregion

        protected static void CreateClassDefs<T1, T2>()
        {
            CustomTypeSource typeSource = new CustomTypeSource();
            typeSource.Add<T1>();
            typeSource.Add<T2>();
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            new AllClassesAutoMapper(typeSource).Map();
        }

        protected static PropertyInfo GetPropertyInfo<T>(string relationshipName)
        {
            return ReflectionUtilities.GetPropertyInfo(typeof(T), relationshipName);
        }

        protected static IClassDef SetupClassDefWithAddProperty<T>()
        {
            IClassDef classDef = SetupClassDef<T>();
            classDef.AddPropDef("SomeOtherProp");
            return classDef;
        }

        protected static IClassDef SetupClassDef<T>()
        {
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            var classDef = typeof (T).MapClass();
            ClassDef.ClassDefs.Add(classDef);
            return classDef;
        }


        protected static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }
    }
    public static class ClassDefTesterExtensions
    {

        public static void AddPropDef(this IClassDef classDef, string propName)
        {
            classDef.PropDefcol.Add(new PropDefFake(propName));
        }
        public static void AddSingleRelDef(this IClassDef classDef, string propName)
        {
            classDef.RelationshipDefCol.Add(new SingleRelDefFake(propName));
        }
        public static void AddSingleRelDef(this IClassDef classDef, string propName, Type relatedType)
        {
            var singleRelDefFake = new SingleRelDefFake(propName, relatedType) {ClassDef = classDef};
            classDef.RelationshipDefCol.Add(singleRelDefFake);
        }

        public static void AssertHasUniqueConstraintAttribute(this PropertyInfo propertyInfo)
        {
            Assert.IsTrue(propertyInfo.ToPropertyWrapper().HasAttribute<AutoMapUniqueConstraintAttribute>(), "Should have UCAttribute");
        }
        public static void AssertNotHasUniqueConstraintAttribute(this PropertyInfo propertyInfo)
        {
            Assert.IsFalse(propertyInfo.ToPropertyWrapper().HasAttribute<AutoMapUniqueConstraintAttribute>(), "Should not have UCAttribute");
        }


        public static void ShouldHaveRelationship(this IClassDef classDef, string relationshipName)
        {
            var message = string.Format("Should have relationship with name '{0}'", relationshipName);
            Assert.IsNotNull(classDef.GetRelationship(relationshipName), message);
        }
        public static void ShouldHaveSingleRelationshipDef(this IClassDef classDef, string relationshipName)
        {
            classDef.ShouldHaveRelationship(relationshipName);
            var relationshipDef = classDef.GetRelationship(relationshipName);
            Assert.IsInstanceOf<SingleRelationshipDef>(relationshipDef);
        }
        public static void ShouldHaveMultipleRelationshipDef(this IClassDef classDef, string relationshipName)
        {
            classDef.ShouldHaveRelationship(relationshipName);
            var relationshipDef = classDef.GetRelationship(relationshipName);
            Assert.IsInstanceOf<MultipleRelationshipDef>(relationshipDef);
        }

        public static void ShouldHavePropertyInfo(this IClassDef classDef, string propertyName)
        {
            var message = string.Format("The Class '{0} should have a PropertyInfo for the property name '{1}'", classDef.ClassNameFull, propertyName);
            Assert.IsNotNull(classDef.GetPropertyInfo(propertyName), message);
        }
        public static void ShouldNotHavePropertyInfo(this IClassDef classDef, string propertyName)
        {
            var message = string.Format("The Class '{0} should not have a PropertyInfo for the property name '{1}'", classDef.ClassNameFull, propertyName);
            Assert.IsNull(classDef.GetPropertyInfo(propertyName), message);
        }

        private static PropertyInfo GetPropertyInfo(this IClassDef classDef, string relationshipName)
        {
            return ReflectionUtilities.GetPropertyInfo(classDef.ClassType, relationshipName);
        }
    }
}