#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Testability.Testers.Tests
{
    /// <summary>
    /// For success these tests tests that no error was thrown since any NUnit Assert Failure raises the AssertionException.
    /// </summary>
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestPropDefTester
    {
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            BORegistry.DataAccessor = new DataAccessorInMemory();
        }

        [Test]
        public void Test_CreatePropDefTester_ShouldSetPropDef()
        {
            //---------------Set up test pack-------------------

            var expectedPropDef = MockRepository.GenerateStub<IPropDef>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            PropDefTester tester = new PropDefTester(expectedPropDef);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedPropDef, tester.PropDef);
        }
        [Test]
        public void Test_ValueDef_ReturnsPropDef()
        {
            //---------------Set up test pack-------------------
            var expectedPropDef = MockRepository.GenerateStub<IPropDef>();
            PropDefTester tester = new PropDefTester(expectedPropDef);

            //---------------Assert Precondition----------------
            Assert.AreSame(expectedPropDef, tester.PropDef);
            //---------------Execute Test ----------------------
            var singleValueDef = tester.SingleValueDef;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedPropDef, singleValueDef);
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
        public void Test_PropertyName_ShouldReturnPropDefsPropName()
        {
            //---------------Set up test pack-------------------
            var propDef = MockRepository.GenerateStub<IPropDef>();
            propDef.PropertyName = GetRandomString();
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.IsNotNullOrEmpty(propDef.PropertyName);
            //---------------Execute Test ----------------------
            var returnedPropertyName = tester.PropertyName;
            //---------------Test Result -----------------------
            Assert.IsNotNull(tester.PropDef);
            Assert.AreEqual(propDef.PropertyName, returnedPropertyName);
        }
        /// <summary>
        /// This tests that no error was thrown since any NUnit Assert Failure raises the AssertionException.
        /// </summary>
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
            string className;
            IPropDef propDef = GetPropDef(out className);
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
        public void Test_ShouldHavePropRuleDate_WhenHas_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var propDef = new PropDefFake();
            propDef.AddPropRule(GetPropRuleDate());
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, propDef.PropRules.Count);
            Assert.IsInstanceOf<PropRuleDate>(propDef.PropRules[0]);
            //---------------Execute Test ----------------------
            tester.ShouldHaveRule<PropRuleDate>();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHavPropRuleDate_WhenNotHas_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = new PropDefFake();
            propDef.AddPropRule(MockRepository.GenerateMock<IPropRule>());
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, propDef.PropRules.Count);
            Assert.IsNotInstanceOf<PropRuleDate>(propDef.PropRules[0]);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldHaveRule<PropRuleDate>();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Property '{0}' for class '{1}' should have a rule of type ",
                                                propDef.PropertyName, propDef.ClassName);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHavePropRuleDate_WhenSpecifyMinAndMaxDate_WhenRuleMatches_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var propDef = new PropDefFake();
            var minDate = DateTime.Today;
            var maxDate = DateTime.Today.AddDays(3);
            propDef.AddPropRule(GetPropRuleDate(minDate, maxDate));
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, propDef.PropRules.Count);
            Assert.IsInstanceOf<PropRuleDate>(propDef.PropRules[0]);
            //---------------Execute Test ----------------------

            var expectedMaxDate = maxDate.AddDays(1).AddMilliseconds(-1);//Rule automatically moves it to the last millisecond of the Day.
            tester.ShouldHaveRule<PropRuleDate, DateTime>(minDate, expectedMaxDate);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }


        [Test]
        public void Test_ShouldHavePropRuleDate_WhenSpecifyMinAndMaxDate_WhenMinDoesNotMatch_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = new PropDefFake();
            var minDate = DateTime.Today;
            DateTime? maxDate = null;
            propDef.AddPropRule(GetPropRuleDate(minDate, maxDate));
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, propDef.PropRules.Count);
            Assert.IsInstanceOf<PropRuleDate>(propDef.PropRules[0]);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldHaveRule<PropRuleDate, DateTime>(minDate.AddDays(-1), maxDate);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Property '{0}' for class '{1}'",
                                                propDef.PropertyName, propDef.ClassName);
                StringAssert.Contains(expected, ex.Message);
                StringAssert.Contains("MinValue Should Be ", ex.Message);

            }
        }

        [Test]
        public void Test_ShouldHavePropRuleDate_WhenSpecifyMinAndMaxDate_WhenMaxDoesNotMatch_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = new PropDefFake();
            DateTime? minDate = null;
            var maxDate = DateTime.Today.AddDays(3);
            propDef.AddPropRule(GetPropRuleDate(minDate, maxDate));
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, propDef.PropRules.Count);
            Assert.IsInstanceOf<PropRuleDate>(propDef.PropRules[0]);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldHaveRule<PropRuleDate, DateTime>(minDate, maxDate.AddDays(-1));
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Property '{0}' for class '{1}'",
                                                propDef.PropertyName, propDef.ClassName);
                StringAssert.Contains(expected, ex.Message);
                StringAssert.Contains("MaxValue Should Be ", ex.Message);

            }
        }

        
        [Test]
        public void Test_ShouldHavePropRuleDate_WhenSpecifyMinAndMaxString_WhenRuleMatches_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var propDef = new PropDefFake();
            String minDate = "Today";
            String maxDate = "Tomorrow";
            propDef.AddPropRule(GetPropRuleDate(minDate, maxDate));
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, propDef.PropRules.Count);
            Assert.IsInstanceOf<PropRuleDate>(propDef.PropRules[0]);
            //---------------Execute Test ----------------------

            var expectedMaxDate = maxDate;
            tester.ShouldHaveRuleDate(minDate, expectedMaxDate);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }


        [Test]
        public void Test_ShouldHavePropRuleDate_WhenSpecifyMinAndMaxString_WhenMinDoesNotMatch_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = new PropDefFake();
            var minDate = "Today";
            string maxDate = null;
            propDef.AddPropRule(GetPropRuleDate(minDate, maxDate));
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, propDef.PropRules.Count);
            Assert.IsInstanceOf<PropRuleDate>(propDef.PropRules[0]);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldHaveRuleDate("Tomorrow", maxDate);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Property '{0}' for class '{1}'",
                                                propDef.PropertyName, propDef.ClassName);
                StringAssert.Contains(expected, ex.Message);
                StringAssert.Contains("MinValue Should Be ", ex.Message);

            }
        }

        [Test]
        public void Test_ShouldHavePropRuleDate_WhenSpecifyMinAndMaxString_WhenMaxDoesNotMatch_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = new PropDefFake();
            String minDate = "";
            var maxDate = "Tomorrow";
            propDef.AddPropRule(GetPropRuleDate(minDate, maxDate));
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, propDef.PropRules.Count);
            Assert.IsInstanceOf<PropRuleDate>(propDef.PropRules[0]);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldHaveRuleDate(minDate, "Today");
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Property '{0}' for class '{1}'",
                                                propDef.PropertyName, propDef.ClassName);
                StringAssert.Contains(expected, ex.Message);
                StringAssert.Contains("MaxValue Should Be ", ex.Message);

            }
        }

        [Test]
        public void ShouldHaveRule_WithMinandMax_WhenHas_WhenMinAndMaxMatchRule_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = new PropDefFake();
            const int minInt = 22;
            const int maxInt = 999;
            propDef.AddPropRule(GetPropRuleInt(minInt, maxInt));
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, propDef.PropRules.Count);
            Assert.IsInstanceOf<PropRuleInteger>(propDef.PropRules[0]) ;
            var propRule = propDef.PropRules[0] as IPropRuleComparable<int>;
            Assert.IsNotNull(propRule);
            Assert.AreEqual(minInt, propRule.MinValue);
            Assert.AreEqual(maxInt, propRule.MaxValue);
            //---------------Execute Test ----------------------
            tester.ShouldHaveRule<PropRuleInteger, int>(minInt, maxInt);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void ShouldHaveRule_WithMinAndMax_WhenMinNotMatch_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = new PropDefFake();
            const int actualMinInt = 22;
            const int expectedMinInt = 25;
            const int maxInt = 999;
            propDef.AddPropRule(GetPropRuleInt(actualMinInt, maxInt));
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, propDef.PropRules.Count);
            var propRule = propDef.PropRules[0] as IPropRuleComparable<int>;
            Assert.IsNotNull(propRule);
            Assert.AreEqual(actualMinInt, propRule.MinValue);
            Assert.AreEqual(maxInt, propRule.MaxValue);
            //---------------Execute Test ----------------------
            try
            {
                
                tester.ShouldHaveRule<PropRuleInteger, int>(expectedMinInt, maxInt);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Property '{0}' for class '{1}'",
                                                propDef.PropertyName, propDef.ClassName);
                StringAssert.Contains(expected, ex.Message);
                StringAssert.Contains("MinValue Should Be '" + expectedMinInt, ex.Message);

            }
        }

        [Test]
        public void ShouldHaveRule_WithMinAndMax_WhenMaxNotMatch_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = new PropDefFake();
            const int actualMaxInt = 22;
            const int expectedMaxInt = 25;
            const int minInt = 1;
            propDef.AddPropRule(GetPropRuleInt(minInt, actualMaxInt));
            PropDefTester tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, propDef.PropRules.Count);
            var propRule = propDef.PropRules[0] as IPropRuleComparable<int>;
            Assert.IsNotNull(propRule);
            Assert.AreEqual(actualMaxInt, propRule.MaxValue);
            Assert.AreEqual(minInt, propRule.MinValue);
            //---------------Execute Test ----------------------
            try
            {

                tester.ShouldHaveRule<PropRuleInteger, int>(minInt, expectedMaxInt);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                var expected = string.Format("The Property '{0}' for class '{1}'",
                                                propDef.PropertyName, propDef.ClassName);
                StringAssert.Contains(expected, ex.Message);
                StringAssert.Contains("MaxValue Should Be '" + expectedMaxInt, ex.Message);

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
        [Test]
        public void Test_ShouldHaveReadWriteRule_WriteNotNew_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IPropDef propDef = GetPropDef();
            propDef.ReadWriteRule = PropReadWriteRule.ReadOnly;
            const PropReadWriteRule expectedReadWriteRule = PropReadWriteRule.WriteNotNew;
            var tester = new PropDefTester(propDef);
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
        public void Test_ShouldHaveReadWriteRule_WriteNotNew_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var propDef = GetPropDef<FakeBOWithReadWriteRuleProp>("ReadWriteRuleWriteNotNew");
            propDef.ReadWriteRule = PropReadWriteRule.WriteNotNew;
            var tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(PropReadWriteRule.WriteNotNew, propDef.ReadWriteRule);
            //---------------Execute Test ----------------------
            tester.ShouldHaveReadWriteRule(PropReadWriteRule.WriteNotNew);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

      
        [Test]
        public void Test_ShouldHavePropertyMapped_WhenWriteNotNew_WhenIsCorrectlyMapped_ShouldReturnTrue_FixBug1288()
        {
            //---------------Set up test pack-------------------
            var propDef = GetPropDef<FakeBOWithReadWriteRuleProp>("ReadWriteRuleWriteNotNew");
            var tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(PropReadWriteRule.WriteNotNew, propDef.ReadWriteRule);
            //---------------Execute Test ----------------------
            tester.ShouldHavePropertyMapped();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHavePropertyMapped_WhenReadWrite_WhenIsCorrectlyMapped_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var propDef = GetPropDef<FakeBOWithReadWriteRuleProp>("ReadWriteRuleReadWrite");
            var tester = new PropDefTester(propDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(PropReadWriteRule.ReadWrite, propDef.ReadWriteRule);
            //---------------Execute Test ----------------------
            tester.ShouldHavePropertyMapped();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        private static IPropDef GetPropDef<T>(string propertyName)
        {
            var classDef = SetupClassDef<T>();
            return classDef.GetPropDef(propertyName);
        }


        protected static IClassDef SetupClassDef<T>()
        {
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            var classDef = typeof(T).MapClass();
            ClassDef.ClassDefs.Add(classDef);
            return classDef;
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

        private static PropRuleDate GetPropRuleDate()
        {
            var propRuleDate = new PropRuleDate("", "")
            {
                Parameters =
                    new Dictionary<string, object> { { "min", "Today" }, { "max", "Tomorrow" } }
            };
            return propRuleDate;
        }

        private static IPropRule GetPropRuleDate(DateTime? minDate, DateTime? maxDate)
        {
            var propRuleDate = new PropRuleDate("", "")
            {
                Parameters =
                    new Dictionary<string, object> { { "min", minDate }, { "max", maxDate } }
            };
            return propRuleDate;
        }
        private static IPropRule GetPropRuleInt(int ? minInt, int? maxInt)
        {
            var propRuleInt = new PropRuleInteger("", "")
            {
                Parameters =
                    new Dictionary<string, object> { { "min", minInt }, { "max", maxInt } }
            };
            return propRuleInt;
        }
        private static IPropRule GetPropRuleDate(string minDate, string maxDate)
        {
            var propRuleDate = new PropRuleDate("", "")
            {
                Parameters =
                    new Dictionary<string, object> { { "min", minDate }, { "max", maxDate } }
            };
            return propRuleDate;
        }
    }
    // ReSharper restore InconsistentNaming
}
