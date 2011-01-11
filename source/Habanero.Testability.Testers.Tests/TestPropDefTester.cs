using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Habanero.Base;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Testability.Testers.Tests
{
    [TestFixture]
    public class TestPropDefTester
    {
        [Test]
        public void Test_CreatePropDefTester()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            PropDefTester tester = new PropDefTester(MockRepository.GenerateMock<IPropDef>());
            //---------------Test Result -----------------------
            Assert.IsNotNull(tester.PropDef);
        }

        [Test]
        public void Test_Construct_WithNullpropDef_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                new PropDefTester(null);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("propDef", ex.ParamName);
            }
        }

        [Test]
        public void Test_ShouldBeCompulsory_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var propDef = GetMockPropDef();
            propDef.Compulsory = true;
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.IsTrue(propDef.Compulsory);
            //---------------Execute Test ----------------------
            tester.ShouldBeCompulsory();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldBeCompulsory_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var propDef = GetMockPropDef();
            propDef.PropertyName = GetRandomString();
            var className = GetRandomString();
            propDef.Stub(def => def.ClassName).Return(className);
            propDef.Compulsory = false;
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.IsFalse(propDef.Compulsory);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldBeCompulsory();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Property '{0}' for class '{1}' should be compulsory",
                                                propDef.PropertyName, className);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldNotBeCompulsory_WhenNot_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var propDef = GetMockPropDef();
            propDef.Compulsory = false;
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.IsFalse(propDef.Compulsory);
            //---------------Execute Test ----------------------
            tester.ShouldNotBeCompulsory();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldNotBeCompulsory_WhenIs_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var propDef = GetMockPropDef();
            propDef.PropertyName = GetRandomString();
            var className = GetRandomString();
            propDef.Stub(def => def.ClassName).Return(className);
            propDef.Compulsory = true;
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.IsTrue(propDef.Compulsory);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldNotBeCompulsory();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Property '{0}' for class '{1}' should not be compulsory",
                                                propDef.PropertyName, className);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveRule_WhenHas_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var propDef = new PropDefFake();
            propDef.AddPropRule(MockRepository.GenerateMock<IPropRule>());
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, propDef.PropRules.Count);
            //---------------Execute Test ----------------------
            tester.ShouldHaveRule();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveRule_WhenNotHas_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = GetPropDef();
            propDef.Compulsory = false;
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.IsFalse(propDef.Compulsory);
            Assert.IsNotNullOrEmpty(propDef.PropertyName);
            Assert.IsNotNullOrEmpty(propDef.ClassName);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldHaveRule();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Property '{0}' for class '{1}' should have Rules set",
                                                propDef.PropertyName, propDef.ClassName);
                StringAssert.Contains(expected, ex.Message);
            }
        }


        [Test]
        public void Test_ShouldHaveDefault_WhenHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = GetPropDef();
            propDef.DefaultValueString = "fdafasdfasd";
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propDef.DefaultValueString);
            //---------------Execute Test ----------------------
            tester.ShouldHaveDefault();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveDefault_WhenNotHasDefault_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = GetPropDef();
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.IsNull(propDef.DefaultValueString);
            Assert.IsNotNullOrEmpty(propDef.PropertyName);
            Assert.IsNotNullOrEmpty(propDef.ClassName);
            //---------------Test Result -----------------------
            try
            {
                tester.ShouldHaveDefault();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format(
                    "The Property '{0}' for class '{1}' should have a default but does not", propDef.PropertyName,
                    propDef.ClassName);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        //        boTester.ShouldHaveDefault(propName, "Today");
        [Test]
        public void Test_ShouldHaveDefault_WithSpecifiedValue_WhenHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = GetPropDef();
            const string defaultValueString = "Today";
            propDef.DefaultValueString = defaultValueString;
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(propDef.DefaultValueString);
            //---------------Execute Test ----------------------
            tester.ShouldHaveDefault(defaultValueString);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveDefault_WithSpecifiedValue_WhenNotHasDefault_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = GetPropDef();
            const string defaultValueString = "Today";
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.IsNull(propDef.DefaultValueString);
            Assert.IsNotNullOrEmpty(propDef.PropertyName);
            Assert.IsNotNullOrEmpty(propDef.ClassName);
            //---------------Test Result -----------------------
            try
            {
                tester.ShouldHaveDefault(defaultValueString);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format(
                    "The Property '{0}' for class '{1}' should have a default but does not", propDef.PropertyName,
                    propDef.ClassName);
                StringAssert.Contains(expected, ex.Message);
            }
        }
        [Test]
        public void Test_ShouldHaveDefault_WithSpecifiedValue_WhenValueNotEqual_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = GetPropDef();
            const string defaultValueString = "aToday";
            propDef.DefaultValueString = "Today";
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.IsNotNullOrEmpty(propDef.PropertyName);
            Assert.IsNotNullOrEmpty(propDef.ClassName);
            Assert.AreNotEqual(defaultValueString, propDef.DefaultValueString);
            //---------------Test Result -----------------------
            try
            {
                tester.ShouldHaveDefault(defaultValueString);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format(
                    "The Property '{0}' for class '{1}' should have a default of '{2}' but has a default value of '{3}'",
                    propDef.PropertyName,
                    propDef.ClassName, defaultValueString, propDef.DefaultValueString);
                StringAssert.Contains(expected, ex.Message);
            }
        }
        [Test]
        public void Test_ShouldHaveDefault_ExtensionMethod_WithSpecifiedValue_WhenValueNotEqual_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = GetPropDef();
            const string defaultValueString = "aToday";
            propDef.DefaultValueString = "Today";
            
            //---------------Assert Precondition----------------
            Assert.IsNotNullOrEmpty(propDef.PropertyName);
            Assert.IsNotNullOrEmpty(propDef.ClassName);
            Assert.AreNotEqual(defaultValueString, propDef.DefaultValueString);
            //---------------Test Result -----------------------
            try
            {
                propDef.ShouldHaveDefault(defaultValueString);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format(
                    "The Property '{0}' for class '{1}' should have a default of '{2}' but has a default value of '{3}'",
                    propDef.PropertyName,
                    propDef.ClassName, defaultValueString, propDef.DefaultValueString);
                StringAssert.Contains(expected, ex.Message);
            }
        }
        [Test]
        public void Test_ShouldNotHaveDefault_WhenNotHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = GetPropDef();
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            tester.ShouldNotHaveDefault();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldNotHaveDefault_WhenHasDefault_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = GetPropDef();
            propDef.DefaultValueString = "Fdafasdfasdfasd";
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.IsNotNullOrEmpty(propDef.DefaultValueString);
            //---------------Test Result -----------------------
            try
            {
                tester.ShouldNotHaveDefault();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format(
                    "The Property '{0}' for class '{1}' should not have a default but does", propDef.PropertyName,
                    propDef.ClassName);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveReadWriteRule_ReadOnly_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = GetPropDef();
            propDef.ReadWriteRule = PropReadWriteRule.ReadOnly;
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(PropReadWriteRule.ReadOnly, propDef.ReadWriteRule);
            //---------------Execute Test ----------------------
            tester.ShouldHaveReadWriteRule(PropReadWriteRule.ReadOnly);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveReadWriteRule_ReadOnly_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = GetPropDef();
            propDef.ReadWriteRule = PropReadWriteRule.WriteNew;
            const PropReadWriteRule expectedReadWriteRule = PropReadWriteRule.ReadOnly;
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(expectedReadWriteRule, propDef.ReadWriteRule);
            //---------------Test Result -----------------------
            try
            {
                tester.ShouldHaveReadWriteRule(expectedReadWriteRule);
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
        [Test]
        public void Test_ShouldHaveReadWriteRule_ReadWrite_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = GetPropDef();
            propDef.ReadWriteRule = PropReadWriteRule.ReadOnly;
            const PropReadWriteRule expectedReadWriteRule = PropReadWriteRule.ReadWrite;
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(expectedReadWriteRule, propDef.ReadWriteRule);
            //---------------Test Result -----------------------
            try
            {
                tester.ShouldHaveReadWriteRule(expectedReadWriteRule);
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


        private static IPropDef GetPropDef()
        {
            string className;
            return GetPropDef(out className);
        }

        private static IPropDef GetPropDef(out string className)
        {
            var propDef = GetMockPropDef();
            propDef.PropertyName = GetRandomString();
            className = GetRandomString();
            propDef.Stub(def => def.ClassName).Return(className);
            return propDef;
        }
        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }

        private static IPropDef GetMockPropDef()
        {
            return MockRepository.GenerateStub<IPropDef>();
        }
    }
}
