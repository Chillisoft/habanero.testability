using System;

using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Testability.Tests.Base;
using Habanero.Util;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Testability.Testers.Tests
{
    [TestFixture]
    public class TestBOTester
    {
        [SetUp]
        public void SetUpTest()
        {
            ClassDef.ClassDefs.Clear();
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
                string expected = string.Format("The Getter And Setter for the Property '{0}' for class '{1}' are not both mapped to the same BOProp. Check the Property in your code", "GetterNotMapped", "FakeBoWithOnePropIncorrectlyMapped");
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
                string expected = string.Format("The Getter for the Property '{0}' for class '{1}'", "NonCompulsoryString", "FakeBOWithNoSetterGetterIncorrectlyMapped");
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
            AddPropDef(classDef, propName);
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
                string expected = string.Format("The Setter for the Property '{0}' for class '{1}'", propName, "FakeBOWithNoGetterSetterIncorrectlyMapped");
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
        public void Test_PropertyShouldBeMapped_WhenIsMapped_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<FakeBOWithIncorrectMappings>();
            var boTester = CreateTester<FakeBOWithIncorrectMappings>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTester.PropertyShouldBeMapped("PropertyMappedCorrectly");
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }
        [Test]
        public void Test_PropertyShouldBeMapped_WhenGetterNotMapped_ShouldAssertFalse()
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
                boTester.PropertyShouldBeMapped(propertyName);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Getter And Setter for the Property '{0}' for class '{1}' are not both mapped to the same BOProp. Check the Property in your code", propertyName, "FakeBOWithIncorrectMappings");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_PropertyShouldBeMapped_WhenSetterNotMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            const string propertyName = "SetterNotMapped";
            SetupClassDefWithAddProperty<FakeBOWithIncorrectMappings>();
            var boTester = CreateTester<FakeBOWithIncorrectMappings>();
            FakeBOWithIncorrectMappings bo = (FakeBOWithIncorrectMappings)boTester.BusinessObject;
            var newValue = GetRandomString();
            bo.SetterNotMapped = newValue;
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(newValue, bo.SetterNotMapped);
            //---------------Test Result -----------------------
            try
            {
                boTester.PropertyShouldBeMapped(propertyName);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Getter And Setter for the Property '{0}' for class '{1}' are not both mapped to the same BOProp. Check the Property in your code", propertyName, "FakeBOWithIncorrectMappings");
                StringAssert.Contains(expected, ex.Message);
            }
        }
        [Test]
        public void Test_PropertyShouldBeMapped_WhenGetterAndSetterMappedToIncorrectprop_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDefWithAddProperty<FakeBOWithIncorrectMappings>();
            const string propName = "GetterAndSetterMappedToIncorrectBOProp";
            var boTester = CreateTester<FakeBOWithIncorrectMappings>();
            FakeBOWithIncorrectMappings bo = (FakeBOWithIncorrectMappings)boTester.BusinessObject;
            var newValue = GetRandomString();
            bo.GetterAndSetterMappedToIncorrectBOProp = newValue;
            var boProp = bo.Props[propName];
            //---------------Assert Precondition----------------
            Assert.AreEqual(newValue, bo.GetterAndSetterMappedToIncorrectBOProp);
            Assert.AreNotEqual(boProp.Value, newValue);
            //---------------Test Result -----------------------
            try
            {
                boTester.PropertyShouldBeMapped(propName);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Setter for the Property '{0}' for class '{1}' is mapped to the incorrect BOProp. Check the Property in your code", propName, "FakeBOWithIncorrectMappings");
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
            BOFakeWithDefault bo = (BOFakeWithDefault)boTester.BusinessObject;
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
                string expected = string.Format("The Property '{0}' for class '{1}' should have a default but does not", propName, "BOFakeWithDefault");
                StringAssert.Contains(expected, ex.Message);
            }
        }
//        boTester.ShouldHaveDefault(propName, "Today");
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
                string expected = string.Format("The Property '{0}' for class '{1}' should not have a default but does", propName, "BOFakeWithDefault");
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
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<BOFakeWithCompulsory, object>(bo => bo.NonCompulsoryProp);
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
                string expected = string.Format("The Property '{0}' for class '{1}' should be compulsory", propName, "BOFakeWithCompulsory");
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
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<BOFakeWithCompulsory, object>(bo => bo.NonCompulsoryProp);
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
                string expected = string.Format("The Property '{0}' for class '{1}' should not be compulsory", propName, "BOFakeWithCompulsory");
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
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<FakeBOWithReadWriteRuleProp, object>(bo => bo.ReadWriteRuleReadOnly);
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
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<FakeBOWithReadWriteRuleProp, object>(bo => bo.ReadWriteRuleReadOnly);
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
                    "The Property '{0}' for class '{1}' should have a ReadWriteRule '{2}' but is '{3}'", propDef.PropertyName,
                    propDef.ClassName, expectedReadWriteRule, propDef.ReadWriteRule);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        protected static IClassDef SetupClassDefWithAddProperty<T>()
        {
            IClassDef classDef = SetupClassDef<T>();
            AddPropDef(classDef, "SomeOtherProp");
            return classDef;
        }

        protected static IClassDef SetupClassDef<T>()
        {
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            var classDef = typeof(T).MapClass();
            ClassDef.ClassDefs.Add(classDef);
            return classDef;
        }

        private static void AddPropDef(IClassDef classDef, string propName)
        {
            classDef.PropDefcol.Add(new PropDefFake(propName));
        }

        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }
    }
}