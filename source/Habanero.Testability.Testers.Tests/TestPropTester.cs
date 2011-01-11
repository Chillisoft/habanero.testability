using System;
using Habanero.Base;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Testability.Testers.Tests
{
    [TestFixture]
    public class TestPropTester
    {
        [Test]
        public void Test_CreateBOPropTester()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            BOPropTester tester = new BOPropTester(MockRepository.GenerateMock<IBOProp>());
            //---------------Test Result -----------------------
            Assert.IsNotNull(tester.BOProp);
        }

        [Test]
        public void Test_Construct_WithNullpropDef_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                new BOPropTester(null);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("boProp", ex.ParamName);
            }
        }

        [Ignore("REASON")] //TODO Brett 29 Mar 2010: Ignored Test - REASON
        [Test]
        public void Test_IsMappedCorrectly_WhenIsMappedShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Not Yet Implemented");
        }
/*
        [Test]
        public void Test_ShouldBeCompulsory_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var propDef = GetMockBOProp();
            propDef.Compulsory = true;
            BOPropTester tester = new BOPropTester(propDef);
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
            var propDef = GetMockBOProp();
            propDef.PropertyName = GetRandomString();
            var className = GetRandomString();
            propDef.Stub(def => def.ClassName).Return(className);
            propDef.Compulsory = false;
            BOPropTester tester = new BOPropTester(propDef);
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
                string expected = string.Format("The PropDef for '{0}' for class '{1}' should be compulsory", propDef.PropertyName, className);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveRule_WhenHas_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var propDef = new PropDefFake();
            propDef.AddPropRule(MockRepository.GenerateMock<IPropRule>());
            BOPropTester tester = new BOPropTester(propDef);
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
            var propDef = GetMockBOProp();
            propDef.PropertyName = GetRandomString();
            var className = GetRandomString();
            propDef.Stub(def => def.ClassName).Return(className);
            propDef.Compulsory = false;
            BOPropTester tester = new BOPropTester(propDef);
            //---------------Assert Precondition----------------
            Assert.IsFalse(propDef.Compulsory);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldHaveRule();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The PropDef for '{0}' for class '{1}' should have Rules set", propDef.PropertyName, className);
                StringAssert.Contains(expected, ex.Message);
            }
        }*/
        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }

        private static IBOProp GetMockBOProp()
        {
            return MockRepository.GenerateStub<IBOProp>();
        }
    }
}