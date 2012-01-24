using System;
using System.Collections.Generic;
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
    public class TestSingleValueTester
    {
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            BORegistry.DataAccessor = new DataAccessorInMemory();
        }

        [Test]
        public void Test_CreateSingleValueDefTester_ShouldSetSingleValueDef()
        {
            //---------------Set up test pack-------------------

            var expectedSingleValueDef = MockRepository.GenerateStub<ISingleValueDef>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var tester = new SingleValueTesterTestDouble(expectedSingleValueDef);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedSingleValueDef, tester.SingleValueDef);
        }

        [Test]
        public void Test_ValueDef_ReturnsSingleValueDef()
        {
            //---------------Set up test pack-------------------
            var expectedSingleValueDef = MockRepository.GenerateStub<ISingleValueDef>();
            var tester = new SingleValueTesterTestDouble(expectedSingleValueDef);

            //---------------Assert Precondition----------------
            Assert.AreSame(expectedSingleValueDef, tester.SingleValueDef);
            //---------------Execute Test ----------------------
            var singleValueDef = tester.SingleValueDef;
            //---------------Test Result -----------------------
            Assert.AreSame(expectedSingleValueDef, singleValueDef);
        }

        [Test]
        public void Test_PropertyName_ShouldReturnSingleValueDefsPropName()
        {
            //---------------Set up test pack-------------------
            var SingleValueDef = MockRepository.GenerateStub<ISingleValueDef>();
            SingleValueDef.PropertyName = GetRandomString();
            var tester = new SingleValueTesterTestDouble(SingleValueDef);
            //---------------Assert Precondition----------------
            Assert.IsNotNullOrEmpty(SingleValueDef.PropertyName);
            //---------------Execute Test ----------------------
            var returnedPropertyName = tester.PropertyName;
            //---------------Test Result -----------------------
            Assert.IsNotNull(tester.SingleValueDef);
            Assert.AreEqual(SingleValueDef.PropertyName, returnedPropertyName);
        }
        /// <summary>
        /// This tests that no error was thrown since any NUnit Assert Failure raises the AssertionException.
        /// </summary>
        [Test]
        public void Test_ShouldBeCompulsory_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var SingleValueDef = GetMockSingleValueDef();
            SingleValueDef.Compulsory = true;
            var tester = new SingleValueTesterTestDouble(SingleValueDef);
            //---------------Assert Precondition----------------
            Assert.IsTrue(SingleValueDef.Compulsory);
            //---------------Execute Test ----------------------
            tester.ShouldBeCompulsory();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldBeCompulsory_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var SingleValueDef = GetMockSingleValueDef();
            SingleValueDef.PropertyName = GetRandomString();
            var className = GetRandomString();
            SingleValueDef.Stub(def => def.ClassName).Return(className);
            SingleValueDef.Compulsory = false;
            var tester = new SingleValueTesterTestDouble(SingleValueDef);
            //---------------Assert Precondition----------------
            Assert.IsFalse(SingleValueDef.Compulsory);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldBeCompulsory();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                var expected = string.Format("The Property '{0}' for class '{1}' should be compulsory",
                                                SingleValueDef.PropertyName, className);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldNotBeCompulsory_WhenNot_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var SingleValueDef = GetMockSingleValueDef();
            SingleValueDef.Compulsory = false;
            var tester = new SingleValueTesterTestDouble(SingleValueDef);
            //---------------Assert Precondition----------------
            Assert.IsFalse(SingleValueDef.Compulsory);
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
            var SingleValueDef = GetSingleValueDef(out className);
            SingleValueDef.Compulsory = true;
            var tester = new SingleValueTesterTestDouble(SingleValueDef);
            //---------------Assert Precondition----------------
            Assert.IsTrue(SingleValueDef.Compulsory);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldNotBeCompulsory();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                var expected = string.Format("The Property '{0}' for class '{1}' should not be compulsory",
                                                SingleValueDef.PropertyName, className);
                StringAssert.Contains(expected, ex.Message);
            }
        }


        [Test]
        public void Test_ShouldHaveReadWriteRule_ReadOnly_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var SingleValueDef = GetSingleValueDef();
            SingleValueDef.ReadWriteRule = PropReadWriteRule.ReadOnly;
            var tester = new SingleValueTesterTestDouble(SingleValueDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(PropReadWriteRule.ReadOnly, SingleValueDef.ReadWriteRule);
            //---------------Execute Test ----------------------
            tester.ShouldHaveReadWriteRule(PropReadWriteRule.ReadOnly);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveReadWriteRule_ReadOnly_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var SingleValueDef = GetSingleValueDef();
            SingleValueDef.ReadWriteRule = PropReadWriteRule.WriteNew;
            const PropReadWriteRule expectedReadWriteRule = PropReadWriteRule.ReadOnly;
            var tester = new SingleValueTesterTestDouble(SingleValueDef);
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(expectedReadWriteRule, SingleValueDef.ReadWriteRule);
            //---------------Test Result -----------------------
            try
            {
                tester.ShouldHaveReadWriteRule(expectedReadWriteRule);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                var expected = string.Format(
                    "The Property '{0}' for class '{1}' should have a ReadWriteRule '{2}' but is '{3}'", SingleValueDef.PropertyName,
                    SingleValueDef.ClassName, expectedReadWriteRule, SingleValueDef.ReadWriteRule);
                StringAssert.Contains(expected, ex.Message);
            }
        }
        [Test]
        public void Test_ShouldHaveReadWriteRule_ReadWrite_WhenIsNot_ShouldAssertFalse()

        {
            //---------------Set up test pack-------------------
            var SingleValueDef = GetSingleValueDef();
            SingleValueDef.ReadWriteRule = PropReadWriteRule.ReadOnly;
            const PropReadWriteRule expectedReadWriteRule = PropReadWriteRule.ReadWrite;
            var tester = new SingleValueTesterTestDouble(SingleValueDef);
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(expectedReadWriteRule, SingleValueDef.ReadWriteRule);
            //---------------Test Result -----------------------
            try
            {
                tester.ShouldHaveReadWriteRule(expectedReadWriteRule);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                var expected = string.Format(
                    "The Property '{0}' for class '{1}' should have a ReadWriteRule '{2}' but is '{3}'", SingleValueDef.PropertyName,
                    SingleValueDef.ClassName, expectedReadWriteRule, SingleValueDef.ReadWriteRule);
                StringAssert.Contains(expected, ex.Message);
            }
        }
        [Test]
        public void Test_ShouldHaveReadWriteRule_WriteNotNew_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var SingleValueDef = GetSingleValueDef();
            SingleValueDef.ReadWriteRule = PropReadWriteRule.ReadOnly;
            const PropReadWriteRule expectedReadWriteRule = PropReadWriteRule.WriteNotNew;
            var tester = new SingleValueTesterTestDouble(SingleValueDef);
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(expectedReadWriteRule, SingleValueDef.ReadWriteRule);
            //---------------Test Result -----------------------
            try
            {
                tester.ShouldHaveReadWriteRule(expectedReadWriteRule);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                var expected = string.Format(
                    "The Property '{0}' for class '{1}' should have a ReadWriteRule '{2}' but is '{3}'", SingleValueDef.PropertyName,
                    SingleValueDef.ClassName, expectedReadWriteRule, SingleValueDef.ReadWriteRule);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveReadWriteRule_WriteNotNew_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var SingleValueDef = GetSingleValueDef<FakeBOWithReadWriteRuleProp>("ReadWriteRuleWriteNotNew");
            SingleValueDef.ReadWriteRule = PropReadWriteRule.WriteNotNew;
            var tester = new SingleValueTesterTestDouble(SingleValueDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(PropReadWriteRule.WriteNotNew, SingleValueDef.ReadWriteRule);
            //---------------Execute Test ----------------------
            tester.ShouldHaveReadWriteRule(PropReadWriteRule.WriteNotNew);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

      
        [Test]
        public void Test_ShouldHavePropertyMapped_WhenWriteNotNew_WhenIsCorrectlyMapped_ShouldReturnTrue_FixBug1288()
        {
            //---------------Set up test pack-------------------
            var SingleValueDef = GetSingleValueDef<FakeBOWithReadWriteRuleProp>("ReadWriteRuleWriteNotNew");
            var tester = new SingleValueTesterTestDouble(SingleValueDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(PropReadWriteRule.WriteNotNew, SingleValueDef.ReadWriteRule);
            //---------------Execute Test ----------------------
            tester.ShouldHavePropertyMapped();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHavePropertyMapped_WhenReadWrite_WhenIsCorrectlyMapped_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var SingleValueDef = GetSingleValueDef<FakeBOWithReadWriteRuleProp>("ReadWriteRuleReadWrite");
            var tester = new SingleValueTesterTestDouble(SingleValueDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(PropReadWriteRule.ReadWrite, SingleValueDef.ReadWriteRule);
            //---------------Execute Test ----------------------
            tester.ShouldHavePropertyMapped();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        private static ISingleValueDef GetSingleValueDef<T>(string propertyName)
        {
            var classDef = SetupClassDef<T>();
            var propDef = classDef.GetPropDef(propertyName);
            if (propDef == null) return classDef.GetRelationship(propertyName) as SingleRelationshipDef;
            return propDef;
        }


        protected static IClassDef SetupClassDef<T>()
        {
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            var classDef = typeof(T).MapClass();
            ClassDef.ClassDefs.Add(classDef);
            return classDef;
        }

        private static ISingleValueDef GetSingleValueDef()
        {
            string className;
            return GetSingleValueDef(out className);
        }

        private static ISingleValueDef GetSingleValueDef(out string className)
        {
            var SingleValueDef = GetMockSingleValueDef();
            SingleValueDef.PropertyName = GetRandomString();
            className = GetRandomString();
            SingleValueDef.Stub(def => def.ClassName).Return(className);
            return SingleValueDef;
        }

        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }

        private static ISingleValueDef GetMockSingleValueDef()
        {
            return MockRepository.GenerateStub<ISingleValueDef>();
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

        public class SingleValueTesterTestDouble : SingleValueTester
        {
            private readonly ISingleValueDef _singleValueDef;

            public SingleValueTesterTestDouble(ISingleValueDef singleValueDef)
            {
                _singleValueDef = singleValueDef;
            }

            public override ISingleValueDef SingleValueDef
            {
                get { return _singleValueDef; }
            }
        }
    }
}