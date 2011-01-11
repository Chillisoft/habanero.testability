using System;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Testability.Tests.Base;
using Habanero.Util;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Testability.Testers.Tests
{
    [TestFixture]
    public class TestBOTesterGeneric : TestBOTester
    {
        [Test]
        public void Test_PropertyShouldBeMapped_WithLambda_WhenGetterNotMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            const string propertyName = "GetterNotMapped";
            SetupClassDefWithAddProperty<FakeBOWithIncorrectMappings>();
            var newValue = GetRandomString();
            BOTester<FakeBOWithIncorrectMappings> boTester = (BOTester<FakeBOWithIncorrectMappings>)CreateTester<FakeBOWithIncorrectMappings>();
            FakeBOWithIncorrectMappings bo = (FakeBOWithIncorrectMappings)boTester.BusinessObject;
            bo.GetterNotMapped = newValue;
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(newValue, bo.GetterNotMapped);
            //---------------Test Result -----------------------
            try
            {
                boTester.PropertyShouldBeMapped(bo1 => bo1.GetterNotMapped);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Getter And Setter for the Property '{0}' for class '{1}'"
                        + " are not both mapped to the same BOProp. Check the Property in your code"
                        , propertyName, "FakeBOWithIncorrectMappings");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_WithLambda_ShouldHaveDefault_WhenNotHasDefault_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDefWithAddProperty<BOFakeWithDefault>();
            const string propName = "NonDefaultProp";
            BOTester<BOFakeWithDefault> boTester = (BOTester<BOFakeWithDefault>)CreateTester<BOFakeWithDefault>();
            //---------------Assert Precondition----------------
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldHaveDefault(bo => bo.NonDefaultProp);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Property '{0}' for class '{1}' should have a default but does not", propName, "BOFakeWithDefault");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_WithLambda_ShouldNotHaveDefault_WhenNotHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithDefault>();
            BOTester<BOFakeWithDefault> boTester = (BOTester<BOFakeWithDefault>)CreateTester<BOFakeWithDefault>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTester.ShouldNotHaveDefault(bo => bo.NonDefaultProp);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }
        [Test]
        public void Test_WithLambda_ShouldNotHaveDefault_WhenHasDefault_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDefWithAddProperty<BOFakeWithDefault>();
            const string propName = "DefaultProp";
            BOTester<BOFakeWithDefault> boTester = (BOTester<BOFakeWithDefault>)CreateTester<BOFakeWithDefault>();
            //---------------Assert Precondition----------------
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldNotHaveDefault(bo => bo.DefaultProp);
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
        public void Test_WithLambda_ShouldHaveDefault_WhenHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithDefault>();
            BOTester<BOFakeWithDefault> boTester = (BOTester<BOFakeWithDefault>)CreateTester<BOFakeWithDefault>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTester.ShouldHaveDefault(bo => bo.DefaultProp);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }


        [Test]
        public void Test_WithLambda_ShouldHaveDefault_WithSpecifiedValue_WhenNotHasDefault_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDefWithAddProperty<BOFakeWithDefault>();
            const string propName = "NonDefaultProp";
            IPropDef propDef = classDef.GetPropDef(propName);
            const string defaultValueString = "SomeOtherValue";
            BOTester<BOFakeWithDefault> boTester = (BOTester<BOFakeWithDefault>)CreateTester<BOFakeWithDefault>();
            //---------------Assert Precondition----------------
            Assert.IsNullOrEmpty(propDef.DefaultValueString);
            Assert.AreNotEqual(defaultValueString, propDef.DefaultValueString);
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldHaveDefault(bo => bo.NonDefaultProp, defaultValueString);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Property '{0}' for class '{1}' should have a default but does not", propName, "BOFakeWithDefault");
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_WithLambda_ShouldNotHaveDefault_WithSpecifiedValue_WhenNotHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDef<BOFakeWithDefault>();
            BOTester<BOFakeWithDefault> boTester = (BOTester<BOFakeWithDefault>)CreateTester<BOFakeWithDefault>();
            const string propName = "NonDefaultProp";
            const string defaultValueString = "SomeValue";
            IPropDef propDef = classDef.GetPropDef(propName);
            //---------------Assert Precondition----------------
            Assert.IsNullOrEmpty(propDef.DefaultValueString);
            //---------------Execute Test ----------------------
            boTester.ShouldNotHaveDefault(bo => bo.NonDefaultProp, defaultValueString);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }
        [Test]
        public void Test_WithLambda_ShouldNotHaveDefault_WithSpecifiedValue_WhenHasDefault_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDefWithAddProperty<BOFakeWithDefault>();
            const string propName = "DefaultProp";
            BOTester<BOFakeWithDefault> boTester = (BOTester<BOFakeWithDefault>)CreateTester<BOFakeWithDefault>();
            const string defaultValueString = "SomeOtherValue"; 
            //---------------Assert Precondition----------------
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldNotHaveDefault(bo => bo.DefaultProp, defaultValueString);
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
        public void Test_WithLambda_ShouldHaveDefault_WithSpecifiedValue_WhenHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDef<BOFakeWithDefault>();
            BOTester<BOFakeWithDefault> boTester = (BOTester<BOFakeWithDefault>)CreateTester<BOFakeWithDefault>();
            const string propName = "DefaultProp";
            const string defaultValueString = "SomeValue";
            IPropDef propDef = classDef.GetPropDef(propName);
            //---------------Assert Precondition----------------
            Assert.IsNotNullOrEmpty(propDef.DefaultValueString);
            Assert.AreEqual(defaultValueString, propDef.DefaultValueString);
            //---------------Execute Test ----------------------
            boTester.ShouldHaveDefault(bo => bo.DefaultProp, defaultValueString);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_WithLambda_ShouldBeCompulsory_WhenCompulsory_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithCompulsory>();
            BOTester<BOFakeWithCompulsory> boTester = (BOTester<BOFakeWithCompulsory>)CreateTester<BOFakeWithCompulsory>();
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<BOFakeWithCompulsory, object>(bo => bo.CompulsoryProp);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(propertyWrapper.HasAttribute<AutoMapCompulsoryAttribute>());
            //---------------Execute Test ----------------------
            boTester.ShouldBeCompulsory(bo => bo.CompulsoryProp);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_WithLambda_ShouldBeCompulsory_WhenNotCompulsory_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithCompulsory>();
            BOTester<BOFakeWithCompulsory> boTester = (BOTester<BOFakeWithCompulsory>)CreateTester<BOFakeWithCompulsory>();
            const string propName = "NonCompulsoryProp";
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<BOFakeWithCompulsory, object>(bo => bo.NonCompulsoryProp);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsFalse(propertyWrapper.HasAttribute<AutoMapCompulsoryAttribute>());
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldBeCompulsory(bo => bo.NonCompulsoryProp);
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
        public void Test_WithLambda_ShouldNotBeCompulsory_WhenNonCompulsory_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithCompulsory>();
            BOTester<BOFakeWithCompulsory> boTester = (BOTester<BOFakeWithCompulsory>)CreateTester<BOFakeWithCompulsory>();
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<BOFakeWithCompulsory, object>(bo => bo.NonCompulsoryProp);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsFalse(propertyWrapper.HasAttribute<AutoMapCompulsoryAttribute>());
            //---------------Execute Test ----------------------
            boTester.ShouldNotBeCompulsory(bo => bo.NonCompulsoryProp);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }
        [Test]
        public void Test_WithLambda_ShouldNotBeCompulsory_WhenCompulsory_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithCompulsory>();
            BOTester<BOFakeWithCompulsory> boTester = (BOTester<BOFakeWithCompulsory>)CreateTester<BOFakeWithCompulsory>();
            const string propName = "CompulsoryProp";
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<BOFakeWithCompulsory, object>(bo => bo.CompulsoryProp);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(propertyWrapper.HasAttribute<AutoMapCompulsoryAttribute>());
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldNotBeCompulsory(bo => bo.CompulsoryProp);
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
        public void Test_WithLambda_ShouldHaveReadWriteRule_ReadOnly_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDef<FakeBOWithReadWriteRuleProp>();
            BOTester<FakeBOWithReadWriteRuleProp> boTester = (BOTester<FakeBOWithReadWriteRuleProp>)CreateTester<FakeBOWithReadWriteRuleProp>();
            const string propName = "ReadWriteRuleReadOnly";
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<FakeBOWithReadWriteRuleProp, object>(bo => bo.ReadWriteRuleReadOnly);
            var propertyWrapper = propertyInfo.ToPropertyWrapper();
            //---------------Assert Precondition----------------
            Assert.IsTrue(propertyWrapper.HasAttribute<AutoMapReadWriteRuleAttribute>());
            Assert.IsNotNull(classDef.GetPropDef(propName));
            //---------------Execute Test ----------------------
            boTester.ShouldHaveReadWriteRule(bo => bo.ReadWriteRuleReadOnly, PropReadWriteRule.ReadOnly);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_WithLambda_ShouldHaveReadWriteRule_WriteNew_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDef<FakeBOWithReadWriteRuleProp>();
            BOTester<FakeBOWithReadWriteRuleProp> boTester = (BOTester<FakeBOWithReadWriteRuleProp>)CreateTester<FakeBOWithReadWriteRuleProp>();
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
                boTester.ShouldHaveReadWriteRule(bo => bo.ReadWriteRuleReadOnly, expectedReadWriteRule);
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

        protected override BOTester CreateTester<T>()
        {
            return new BOTester<T>();
        }

        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }
    }
}