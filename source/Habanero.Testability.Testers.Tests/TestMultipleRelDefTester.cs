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
    [TestFixture]
    public class TestMultipleRelDefTester
    {
        [Test]
// ReSharper disable InconsistentNaming
        public void Test_CreateMultipleRelDefTester()
        {
            //---------------Set up test pack-------------------
            var relationshipDef = MockRepository.GenerateStub<IRelationshipDef>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var tester = new MultipleRelDefTester(relationshipDef);
            //---------------Test Result -----------------------
            Assert.AreSame(relationshipDef, tester.MultipleRelationshipDef);
        }
        [Test]
        public void Test_MultipleRelationshipDef_ReturnsMultipleRelationshipDef()
        {
            //---------------Set up test pack-------------------
            var relationshipDef = MockRepository.GenerateStub<IRelationshipDef>();
            var tester = new MultipleRelDefTester(relationshipDef);

            //---------------Assert Precondition----------------
            Assert.AreSame(relationshipDef, tester.MultipleRelationshipDef);
            //---------------Execute Test ----------------------
            var multipleRelationshipDef = tester.MultipleRelationshipDef;
            //---------------Test Result -----------------------
            Assert.AreSame(relationshipDef, multipleRelationshipDef);
        }
        [Test]
        public void Test_Construct_WithNullMultipleRelationshipDef_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                new MultipleRelDefTester(null);
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("relationshipDef", ex.ParamName);
            }
        }
        [Test]
        public void Test_RelationshipName_ShouldReturnMultipleRelationshipDefsRelationshipName()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = CreateMultipleRelationshipDef();
            singleRelationshipDef.Stub(def => def.RelationshipName).Return(GetRandomString());
            var tester = new MultipleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            NUnit3AssertsPolyFill.IsNotNullOrEmpty(singleRelationshipDef.RelationshipName);
            //---------------Execute Test ----------------------
            var returnedRelationshipName = tester.RelationshipName;
            //---------------Test Result -----------------------
            Assert.IsNotNull(tester.MultipleRelationshipDef);
            Assert.AreEqual(singleRelationshipDef.RelationshipName, returnedRelationshipName);
        }

        #region DeleteParentAction

        [Test]
        public void Test_ShouldHaveDeleteParentAction_Prevent_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = CreateMultipleRelationshipDef(DeleteParentAction.Prevent);
            var tester = new MultipleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.AreEqual(DeleteParentAction.Prevent, singleRelationshipDef.DeleteParentAction);
            //---------------Execute Test ----------------------
            tester.ShouldHaveDeleteParentAction(DeleteParentAction.Prevent);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldHaveDeleteParentAction_Prevent_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = CreateMultipleRelationshipDef(DeleteParentAction.DeleteRelated);
            const DeleteParentAction expectedDeleteParentAction = DeleteParentAction.Prevent;
            var tester = new MultipleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(expectedDeleteParentAction, singleRelationshipDef.DeleteParentAction);
            //---------------Test Result -----------------------
            try
            {
                tester.ShouldHaveDeleteParentAction(expectedDeleteParentAction);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                var expected = string.Format(
                    "should have a DeleteParentAction '{0}' but is '{1}'",
                    expectedDeleteParentAction, singleRelationshipDef.DeleteParentAction);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        [Test]
        public void Test_ShouldHaveDeleteParentAction_DereferenceRelated_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = CreateMultipleRelationshipDef(DeleteParentAction.DoNothing);

            const DeleteParentAction expectedDeleteParentAction = DeleteParentAction.DereferenceRelated;
            var tester = new MultipleRelDefTester(singleRelationshipDef);
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(expectedDeleteParentAction, singleRelationshipDef.DeleteParentAction);
            //---------------Test Result -----------------------
            try
            {
                tester.ShouldHaveDeleteParentAction(expectedDeleteParentAction);
                Assert.Fail("Expected to throw an AssertionException");
            }
            //---------------Test Result -----------------------
            catch (AssertionException ex)
            {
                var expected = string.Format(
                    "should have a DeleteParentAction '{0}' but is '{1}'", expectedDeleteParentAction, singleRelationshipDef.DeleteParentAction);
                StringAssert.Contains(expected, ex.Message);
            }
        }

        #endregion
/*//TODO brett 24 Jan 2011: Implement these tests for MultipRelDefTester
        
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
            classDef.ShouldHaveMultipleRelationshipDef(relName);
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
            classDef.ShouldHaveMultipleRelationshipDef(relName);
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


        #region RelationshipType

        [Test]
        public void Test_ShouldHaveRelationshipType_Aggregation_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            var singleRelationshipDef = CreateMultipleRelationshipDef();
            singleRelationshipDef.RelationshipType = RelationshipType.Aggregation;
            MultipleRelDefTester tester = new MultipleRelDefTester(singleRelationshipDef);
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
            var singleRelationshipDef = CreateMultipleRelationshipDef();
            singleRelationshipDef.RelationshipType = RelationshipType.Association;
            const RelationshipType expectedRelationshipType = RelationshipType.Aggregation;
            MultipleRelDefTester tester = new MultipleRelDefTester(singleRelationshipDef);
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
            var singleRelationshipDef = CreateMultipleRelationshipDef();
            singleRelationshipDef.RelationshipType = RelationshipType.Aggregation;
            const RelationshipType expectedRelationshipType = RelationshipType.Association;
            MultipleRelDefTester tester = new MultipleRelDefTester(singleRelationshipDef);
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

        #endregion*/


/*
       
        

        

        

        */

/* The ReadWriteRule needs to be implemented in some manner for SingleRelationships

        [Test]
        public void Test_ShouldHaveReadWriteRule_ReadOnly_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IRelationshipDef singleRelationshipDef = CreateMultipleRelationshipDef();
            singleRelationshipDef.ReadWriteRule = PropReadWriteRule.ReadOnly;
            MultipleRelDefTester tester = new MultipleRelDefTester(singleRelationshipDef);
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
            IRelationshipDef singleRelationshipDef = CreateMultipleRelationshipDef();
            singleRelationshipDef.ReadWriteRule = PropReadWriteRule.WriteNew;
            const PropReadWriteRule expectedReadWriteRule = PropReadWriteRule.ReadOnly;
            MultipleRelDefTester tester = new MultipleRelDefTester(singleRelationshipDef);
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
            IRelationshipDef singleRelationshipDef = CreateMultipleRelationshipDef();
            singleRelationshipDef.ReadWriteRule = PropReadWriteRule.ReadOnly;
            const PropReadWriteRule expectedReadWriteRule = PropReadWriteRule.ReadWrite;
            MultipleRelDefTester tester = new MultipleRelDefTester(singleRelationshipDef);
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
        private static IRelationshipDef GetMultipleRelationshipDef()
        {
            string className;
            return GetMultipleRelationshipDef(out className);
        }*/

/*        private static IRelationshipDef GetMultipleRelationshipDef(out string className)
        {
            var singleRelationshipDef = GetMockMultipleRelationshipDef();
            singleRelationshipDef.RelationshipName = GetRandomString();
            className = GetRandomString();
            singleRelationshipDef.Stub(def => def.ClassName).Return(className);
            return singleRelationshipDef;
        }*/


        private static IRelationshipDef CreateMultipleRelationshipDef(DeleteParentAction action)
        {
            var multipleRelationshipDef = MockRepository.GenerateStub<IRelationshipDef>();
            multipleRelationshipDef.Stub(def => def.DeleteParentAction).Return(action);
            return multipleRelationshipDef;
        }

        private static IRelationshipDef CreateMultipleRelationshipDef()
        {
            return MockRepository.GenerateStub<IRelationshipDef>();
        }
        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }

    }
}