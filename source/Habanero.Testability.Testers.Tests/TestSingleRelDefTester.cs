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
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability.Helpers;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Testability.Testers.Tests
{
    
    /// <summary>
    /// For success these tests tests that no error was thrown since any NUnit Assert Failure raises the AssertionException.
    /// </summary>
    [TestFixture]
    public class TestSingleRelDefTester
    {
        [Test]
// ReSharper disable InconsistentNaming
        public void Test_CreateSingleRelDefTester()
        {
            //---------------Set up test pack-------------------
            var relationshipDef = MockRepository.GenerateStub<ISingleRelationshipDef>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var tester = new SingleRelDefTester(relationshipDef);
            //---------------Test Result -----------------------
            Assert.AreSame(relationshipDef, tester.SingleRelationshipDef);
        }

        [Test]
        public void Test_SingleValueDef_ReturnsSingleRelationshipDef()
        {
            //---------------Set up test pack-------------------
            var relationshipDef = MockRepository.GenerateStub<ISingleRelationshipDef>();
            var tester = new SingleRelDefTester(relationshipDef);

            //---------------Assert Precondition----------------
            Assert.AreSame(relationshipDef, tester.SingleRelationshipDef);
            //---------------Execute Test ----------------------
            var singleValueDef = tester.SingleValueDef;
            //---------------Test Result -----------------------
            Assert.AreSame(relationshipDef, singleValueDef);
        }

        [Test]
        public void Test_Construct_WithNullsingleRelationshipDef_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                new SingleRelDefTester(null);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("singleRelationshipDef", ex.ParamName);
            }
        }
        [Test]
        public void Test_RelationshipName_ShouldReturnSingleRelationshipDefsRelationshipName()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = CreateSingleRelationshipDef();
            singleRelationshipDef.Stub(def => def.RelationshipName).Return(GetRandomString());
            var tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.IsNotNullOrEmpty(singleRelationshipDef.RelationshipName);
            //---------------Execute Test ----------------------
            var returnedRelationshipName = tester.RelationshipName;
            //---------------Test Result -----------------------
            Assert.IsNotNull(tester.SingleRelationshipDef);
            Assert.AreEqual(singleRelationshipDef.RelationshipName, returnedRelationshipName);
        }


        #region IsCompulsory
        
        /// <summary>
        /// This tests that no error was thrown since any NUnit Assert Failure raises the AssertionException.
        /// </summary>
        [Test]
        public void Test_ShouldBeCompulsory_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = GetMockSingleRelationshipDef();
            singleRelationshipDef.SetIsCompulsory();
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.IsTrue(singleRelationshipDef.Compulsory);
            //---------------Execute Test ----------------------
            tester.ShouldBeCompulsory();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldBeCompulsory_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = GetMockSingleRelationshipDef();
            singleRelationshipDef.SetRelationshipName();
            singleRelationshipDef.SetOwningClassName();
            singleRelationshipDef.SetIsNotCompulsory();

            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.IsFalse(singleRelationshipDef.IsCompulsory);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldBeCompulsory();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Relationship '{0}' for class '{1}' should be compulsory",
                                                singleRelationshipDef.RelationshipName, singleRelationshipDef.OwningClassName);
                StringAssert.Contains(expected, ex.Message);
            }
        }
        [Test]
        public void Test_ShouldNotBeCompulsory_WhenNot_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = GetMockSingleRelationshipDef();
            singleRelationshipDef.SetIsNotCompulsory();
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.IsFalse(singleRelationshipDef.IsCompulsory);
            //---------------Execute Test ----------------------
            tester.ShouldNotBeCompulsory();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }
        [Test]
        public void Test_ShouldNotBeCompulsory_WhenIs_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = GetMockSingleRelationshipDef();
            singleRelationshipDef.SetRelationshipName();
            singleRelationshipDef.SetOwningClassName();
            singleRelationshipDef.SetIsCompulsory();
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.IsTrue(singleRelationshipDef.Compulsory);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldNotBeCompulsory();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Relationship '{0}' for class '{1}' should not be compulsory",
                                                singleRelationshipDef.RelationshipName, singleRelationshipDef.OwningClassName);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        #endregion

        #region ShouldHavePropertyCorrectlyMapped

        [Test]
        public void Test_ShouldHavePropertyMapped_WhenPropWithNoSetter_GetterNotCorrectlyMapped_ShouldAssertFalse()
        {
            BORegistry.DataAccessor = new DataAccessorInMemory();
            //---------------Set up test pack-------------------
            const string relName = "SingleRelNoSetterGetterIncorrect";
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRel>();
            var classDef = ClassDef.Get<FakeBoWithSingleRel>();
            var boTester = CreateTester<FakeBoWithSingleRel>();
            var singleRelDefTester = boTester.GetSingleRelationshipTester(rel => rel.SingleRelNoSetterGetterIncorrect);
            //---------------Assert Precondition----------------
            classDef.ShouldHaveSingleRelationshipDef(relName);
            classDef.ShouldHavePropertyInfo(relName);
            //---------------Execute Test ----------------------
            try
            {
                singleRelDefTester.ShouldHavePropertyMapped();
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
        [Test]
        public void Test_ShouldHavePropertyMapped_WhenPropSetterMappedToInvalidName_ShouldAssertFalse()
        {
            BORegistry.DataAccessor = new DataAccessorInMemory();
            //---------------Set up test pack-------------------
            const string relName = "SingleRelSetterMappedToNonExistentRelDef";
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRel>();
            var classDef = ClassDef.Get<FakeBoWithSingleRel>();
            var boTester = CreateTester<FakeBoWithSingleRel>();
            var singleRelDefTester = boTester.GetSingleRelationshipTester(rel => rel.SingleRelSetterMappedToNonExistentRelDef);
            //---------------Assert Precondition----------------
            classDef.ShouldHaveSingleRelationshipDef(relName);
            classDef.ShouldHavePropertyInfo(relName);
            //---------------Execute Test ----------------------
            try
            {
                singleRelDefTester.ShouldHavePropertyMapped();
                Assert.Fail("Expected to throw an AssertionException ");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format("The Setter for the Property '{0}' for class '{1}'",
                                                relName, "FakeBoWithSingleRel");
                StringAssert.Contains(expected, ex.Message);
                StringAssert.Contains("Setting the Property via reflection failed", ex.Message);
            }
        }

        private static BOTester<T> CreateTester<T>() where T : class, IBusinessObject
        {
            return new BOTester<T>(); ;
        }
        protected static void CreateClassDefs<T1, T2>()
        {
            CustomTypeSource typeSource = new CustomTypeSource();
            typeSource.Add<T1>();
            typeSource.Add<T2>();
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            new AllClassesAutoMapper(typeSource).Map();
        }

        #endregion


        #region IsOneToOne

        [Test]
        public void Test_ShouldBeOneToOne_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = GetMockSingleRelationshipDef();
            singleRelationshipDef.SetIsOneToOne();
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.IsTrue(singleRelationshipDef.IsOneToOne);
            //---------------Execute Test ----------------------
            tester.ShouldBeOneToOne();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldBeOneToOne_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = GetMockSingleRelationshipDef();
            singleRelationshipDef.SetRelationshipName();
            singleRelationshipDef.SetOwningClassName();
            singleRelationshipDef.SetIsNotOneToOne();

            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.IsFalse(singleRelationshipDef.IsOneToOne);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldBeOneToOne();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                const string expected = "should be OneToOne";
                StringAssert.Contains(expected, ex.Message);
            }
        }
        [Test]
        public void Test_ShouldNotBeOneToOne_WhenNot_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = GetMockSingleRelationshipDef();
            singleRelationshipDef.SetIsNotOneToOne();
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.IsFalse(singleRelationshipDef.IsOneToOne);
            //---------------Execute Test ----------------------
            tester.ShouldNotBeOneToOne();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }
        [Test]
        public void Test_ShouldNotBeOneToOne_WhenIs_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = GetMockSingleRelationshipDef();
            singleRelationshipDef.SetRelationshipName();
            singleRelationshipDef.SetOwningClassName();
            singleRelationshipDef.SetIsOneToOne();
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.IsTrue(singleRelationshipDef.IsOneToOne);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldNotBeOneToOne();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                const string expected = "should not be OneToOne";
                StringAssert.Contains(expected, ex.Message);
            }
        }

        #endregion


        #region IsManyToOne

        [Test]
        public void Test_ShouldBeManyToOne_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = GetMockSingleRelationshipDef();
            singleRelationshipDef.SetIsManyToOne();
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.IsTrue(singleRelationshipDef.IsManyToOne);
            //---------------Execute Test ----------------------
            tester.ShouldBeManyToOne();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldBeManyToOne_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = GetMockSingleRelationshipDef();
            singleRelationshipDef.SetIsNotManyToOne();

            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.IsFalse(singleRelationshipDef.IsManyToOne);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldBeManyToOne();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                const string expected = "should be ManyToOne";
                StringAssert.Contains(expected, ex.Message);
            }
        }
        [Test]
        public void Test_ShouldNotBeManyToOne_WhenNot_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = GetMockSingleRelationshipDef();
            singleRelationshipDef.SetIsNotManyToOne();
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.IsFalse(singleRelationshipDef.IsManyToOne);
            //---------------Execute Test ----------------------
            tester.ShouldNotBeManyToOne();
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }
        [Test]
        public void Test_ShouldNotBeManyToOne_WhenIs_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = GetMockSingleRelationshipDef();
            singleRelationshipDef.SetIsManyToOne();
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.IsTrue(singleRelationshipDef.IsManyToOne);
            //---------------Execute Test ----------------------
            try
            {
                tester.ShouldNotBeManyToOne();
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                const string expected = "should not be ManyToOne";
                StringAssert.Contains(expected, ex.Message);
            }
        }

        #endregion

        #region RelationshipType

        [Test]
        public void Test_ShouldHaveRelationshipType_Aggregation_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = CreateSingleRelationshipDef();
            singleRelationshipDef.RelationshipType = RelationshipType.Aggregation;
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(RelationshipType.Aggregation, singleRelationshipDef.RelationshipType);
            //---------------Execute Test ----------------------
            tester.ShouldHaveRelationshipType(RelationshipType.Aggregation);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveRelationshipType_Aggregation_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = CreateSingleRelationshipDef();
            singleRelationshipDef.RelationshipType = RelationshipType.Association;
            const RelationshipType expectedRelationshipType = RelationshipType.Aggregation;
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(expectedRelationshipType, singleRelationshipDef.RelationshipType);
            //---------------Test Result -----------------------
            try
            {
                tester.ShouldHaveRelationshipType(expectedRelationshipType);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format(
                    "should have a RelationshipType '{0}' but is '{1}'", 
                    expectedRelationshipType, singleRelationshipDef.RelationshipType);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveRelationshipType_Association_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = CreateSingleRelationshipDef();
            singleRelationshipDef.RelationshipType = RelationshipType.Aggregation;
            const RelationshipType expectedRelationshipType = RelationshipType.Association;
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(expectedRelationshipType, singleRelationshipDef.RelationshipType);
            //---------------Test Result -----------------------
            try
            {
                tester.ShouldHaveRelationshipType(expectedRelationshipType);
                Assert.Fail("Expected to throw an AssertionException");
            }
                //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                string expected = string.Format(
                    "should have a RelationshipType '{0}' but is '{1}'", expectedRelationshipType, singleRelationshipDef.RelationshipType);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        #endregion


/*
       
        

        

        

        */

/* The ReadWriteRule needs to be implemented in some manner for SingleRelationships

        [Test]
        public void Test_ShouldHaveReadWriteRule_ReadOnly_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IRelationshipDef singleRelationshipDef = CreateSingleRelationshipDef();
            singleRelationshipDef.ReadWriteRule = PropReadWriteRule.ReadOnly;
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(PropReadWriteRule.ReadOnly, singleRelationshipDef.ReadWriteRule);
            //---------------Execute Test ----------------------
            tester.ShouldHaveReadWriteRule(PropReadWriteRule.ReadOnly);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveReadWriteRule_ReadOnly_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IRelationshipDef singleRelationshipDef = CreateSingleRelationshipDef();
            singleRelationshipDef.ReadWriteRule = PropReadWriteRule.WriteNew;
            const PropReadWriteRule expectedReadWriteRule = PropReadWriteRule.ReadOnly;
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(expectedReadWriteRule, singleRelationshipDef.ReadWriteRule);
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
                    "The Relationship '{0}' for class '{1}' should have a ReadWriteRule '{2}' but is '{3}'", singleRelationshipDef.RelationshipName,
                    singleRelationshipDef.ClassName, expectedReadWriteRule, singleRelationshipDef.ReadWriteRule);
                StringAssert.Contains(expected, ex.Message);
            }
        }
        [Test]
        public void Test_ShouldHaveReadWriteRule_ReadWrite_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IRelationshipDef singleRelationshipDef = CreateSingleRelationshipDef();
            singleRelationshipDef.ReadWriteRule = PropReadWriteRule.ReadOnly;
            const PropReadWriteRule expectedReadWriteRule = PropReadWriteRule.ReadWrite;
            SingleRelDefTester tester = new SingleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(expectedReadWriteRule, singleRelationshipDef.ReadWriteRule);
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
                    "The Relationship '{0}' for class '{1}' should have a ReadWriteRule '{2}' but is '{3}'", singleRelationshipDef.RelationshipName,
                    singleRelationshipDef.ClassName, expectedReadWriteRule, singleRelationshipDef.ReadWriteRule);
                StringAssert.Contains(expected, ex.Message);
            }
        }*/

/*
        private static IRelationshipDef GetSingleRelationshipDef()
        {
            string className;
            return GetSingleRelationshipDef(out className);
        }*/

/*        private static IRelationshipDef GetSingleRelationshipDef(out string className)
        {
            var singleRelationshipDef = GetMockSingleRelationshipDef();
            singleRelationshipDef.RelationshipName = GetRandomString();
            className = GetRandomString();
            singleRelationshipDef.Stub(def => def.ClassName).Return(className);
            return singleRelationshipDef;
        }*/


        private static ISingleRelationshipDef CreateSingleRelationshipDef()
        {
            return MockRepository.GenerateStub<ISingleRelationshipDef>();
        }
        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }

        private static ISingleRelationshipDef GetMockSingleRelationshipDef()
        {
            return MockRepository.GenerateStub<ISingleRelationshipDef>();
        }
    }

    internal static class RelationshipDefStubExtensionMethods
    {

        internal static void SetOwningClassName(this IRelationshipDef singleRelationshipDef)
        {
            singleRelationshipDef.Stub(def => def.OwningClassName).Return(GetRandomString());
        }

        internal static void SetRelationshipName(this IRelationshipDef singleRelationshipDef)
        {
            singleRelationshipDef.Stub(def => def.RelationshipName).Return(GetRandomString());
        }
        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }

        internal static void SetIsCompulsory(this ISingleRelationshipDef singleRelationshipDef)
        {
            singleRelationshipDef.Compulsory = true;
        }
        internal static void SetIsNotCompulsory(this ISingleRelationshipDef singleRelationshipDef)
        {
            singleRelationshipDef.Compulsory = false;
        }
        internal static void SetIsOneToOne(this IRelationshipDef singleRelationshipDef)
        {
            singleRelationshipDef.Stub(def => def.IsOneToOne).Return(true);
        }
        internal static void SetIsNotOneToOne(this IRelationshipDef singleRelationshipDef)
        {
            singleRelationshipDef.Stub(def => def.IsOneToOne).Return(false);
        }
        internal static void SetIsManyToOne(this IRelationshipDef singleRelationshipDef)
        {
            singleRelationshipDef.Stub(def => def.IsManyToOne).Return(true);
        }
        internal static void SetIsNotManyToOne(this IRelationshipDef singleRelationshipDef)
        {
            singleRelationshipDef.Stub(def => def.IsManyToOne).Return(false);
        }
    }
}