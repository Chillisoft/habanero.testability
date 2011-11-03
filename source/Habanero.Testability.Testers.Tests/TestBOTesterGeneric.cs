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
using Habanero.Smooth.ReflectionWrappers;
using Habanero.Testability.Tests.Base;
using Habanero.Util;
using NUnit.Framework;
using Rhino.Mocks;

namespace Habanero.Testability.Testers.Tests
{
// ReSharper disable InconsistentNaming
    /// <summary>
    /// For success these tests tests that no error was thrown since any NUnit Assert Failure raises the AssertionException.
    /// </summary>
    [TestFixture]
    public class TestBOTesterGeneric : TestBOTester
    {
        protected override BOTester CreateTester<T>()
        {
            return new BOTester<T>();
        }

        private BOTester<T> CreateGenericTester<T>() where T : class, IBusinessObject
        {
            return (BOTester<T>) CreateTester<T>();
        }

        [Test]
        public void Test_GetPropTester_WithLambda_WhenHasPropDef_ShouldReturnPropTesterForPropDef()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<FakeBOWithAllPropsMapped>();
            const string propName = "NonCompulsoryString";
            var boTester = CreateGenericTester<FakeBOWithAllPropsMapped>();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(boTester);
            //---------------Execute Test ----------------------
            var propTester = boTester.GetPropTester(bo1 => bo1.NonCompulsoryString);
            //---------------Test Result -----------------------
            Assert.IsNotNull(propTester);
            Assert.AreEqual(propName, propTester.PropertyName);
        }
        [Test]
        public void Test_GetSingleRelationshipTester_WithLambda_WhenHasRelDef_ShouldReturnTesterForRelDef()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<FakeBoWithSingleRel>();
            const string relationshipName = "SingleRelMapped";
            var boTester = CreateGenericTester<FakeBoWithSingleRel>();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(boTester);
            //---------------Execute Test ----------------------
            var tester = boTester.GetSingleRelationshipTester(bo1 => bo1.SingleRelMapped);
            //---------------Test Result -----------------------
            Assert.IsNotNull(tester);
            Assert.IsInstanceOf<SingleRelDefTester>(tester);
            Assert.AreEqual(relationshipName, tester.RelationshipName);
        }
        [Test]
        public void Test_GetMultipleRelationshipTester_WithLambda_WhenHasRelDef_ShouldReturnTesterForRelDef()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<FakeBOWithManyRelationship>();
            SetupClassDef<RelatedFakeBo>();
            const string relationshipName = "RelatedFakeBos";
            var boTester = CreateGenericTester<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(boTester);
            //---------------Execute Test ----------------------
            var tester = boTester.GetMultipleRelationshipTester(bo1 => bo1.RelatedFakeBos);
            //---------------Test Result -----------------------
            Assert.IsNotNull(tester);
            Assert.IsInstanceOf<MultipleRelDefTester>(tester);
            Assert.AreEqual(relationshipName, tester.RelationshipName);
        }

        [Test]
        public void Test_ShouldHavePropertyMapped_WithLambda_WhenGetterNotMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            const string propertyName = "GetterNotMapped";
            SetupClassDefWithAddProperty<FakeBOWithIncorrectMappings>();
            var newValue = GetRandomString();
            BOTester<FakeBOWithIncorrectMappings> boTester = CreateGenericTester<FakeBOWithIncorrectMappings>();
            FakeBOWithIncorrectMappings bo = (FakeBOWithIncorrectMappings)boTester.BusinessObject;
            bo.GetterNotMapped = newValue;
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(newValue, bo.GetterNotMapped);
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldHavePropertyMapped(bo1 => bo1.GetterNotMapped);
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

        #region Relationship

        [Test]
        public void Test_ShouldRelationshipBeMapped_WithLambda_WhenSingleRelationship_WhenMapped_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRel>();
            var boTester = CreateGenericTester<FakeBoWithSingleRel>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTester.ShouldHaveSingleRelationshipMapped(rel => rel.SingleRelMapped);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }


        [Test]
        public void Test_ShouldHaveSingleRelationshipMapped_WithLambda_WhenGetterNotMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRel>();
            const string relationshipName = "SingleRelGetterNotMapped";
            var newValue = new FakeBOWithNothing();
            var boTester = CreateGenericTester<FakeBoWithSingleRel>();
            FakeBoWithSingleRel bo = (FakeBoWithSingleRel)boTester.BusinessObject;
            bo.SingleRelGetterNotMapped = newValue;
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(newValue, bo.SingleRelGetterNotMapped);
            //---------------Execute Test -----------------------
            try
            {
                boTester.ShouldHaveSingleRelationshipMapped(rel => rel.SingleRelGetterNotMapped);
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
        public void Test_ShouldHaveSingleRelationshipMapped_WithLambda_WhenSetterNotMapped_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            const string propertyName = "SingleRelSetterNotMapped";
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRel>();
            var boTester = CreateGenericTester<FakeBoWithSingleRel>();
            FakeBoWithSingleRel bo = (FakeBoWithSingleRel)boTester.BusinessObject;
            var newValue = new FakeBOWithNothing();
            bo.SingleRelSetterNotMapped = newValue;
            //---------------Assert Precondition----------------
            Assert.AreNotEqual(newValue, bo.SingleRelSetterNotMapped);
            //---------------Test Result -----------------------
            try
            {
                boTester.ShouldHaveSingleRelationshipMapped(rel => rel.SingleRelSetterNotMapped);
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
        public void Test_ShouldHaveSingleRelationshipMapped_WithLambda_WhenGetterAndSetterMappedToIncorrectprop_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            CreateClassDefs<FakeBOWithNothing, FakeBoWithSingleRel>();
            const string propName = "SingleRelGetterAndSetterNotMapped";
            var boTester = CreateGenericTester<FakeBoWithSingleRel>();
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
                boTester.ShouldHaveSingleRelationshipMapped(rel => rel.SingleRelGetterAndSetterNotMapped);
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

        #endregion

        [Test]
        public void Test_ShouldHaveDefault_WithLambda_WhenNotHasDefault_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDefWithAddProperty<BOFakeWithDefault>();
            const string propName = "NonDefaultProp";
            var boTester = CreateGenericTester<BOFakeWithDefault>();
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

        /// <summary>
        /// This tests that no error was thrown since any NUnit Assert Failure raises the AssertionException.
        /// </summary>
        [Test]
        public void Test_ShouldNotHaveDefault_WithLambda_WhenNotHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithDefault>();
            BOTester<BOFakeWithDefault> boTester = CreateGenericTester<BOFakeWithDefault>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTester.ShouldNotHaveDefault(bo => bo.NonDefaultProp);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldNotHaveDefault_WithLambda_WhenHasDefault_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDefWithAddProperty<BOFakeWithDefault>();
            const string propName = "DefaultProp";
            BOTester<BOFakeWithDefault> boTester = CreateGenericTester<BOFakeWithDefault>();
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
        public void Test_ShouldHaveDefault_WithLambda_WhenHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithDefault>();
            BOTester<BOFakeWithDefault> boTester = CreateGenericTester<BOFakeWithDefault>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTester.ShouldHaveDefault(bo => bo.DefaultProp);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }


        [Test]
        public void Test_ShouldHaveDefault_WithLambda_WithSpecifiedValue_WhenNotHasDefault_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDefWithAddProperty<BOFakeWithDefault>();
            const string propName = "NonDefaultProp";
            IPropDef propDef = classDef.GetPropDef(propName);
            const string defaultValueString = "SomeOtherValue";
            BOTester<BOFakeWithDefault> boTester = CreateGenericTester<BOFakeWithDefault>();
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
        public void Test_ShouldNotHaveDefault_WithLambda_WithSpecifiedValue_WhenNotHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDef<BOFakeWithDefault>();
            BOTester<BOFakeWithDefault> boTester = CreateGenericTester<BOFakeWithDefault>();
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
        public void Test_ShouldNotHaveDefault_WithLambda_WithSpecifiedValue_WhenHasDefault_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDefWithAddProperty<BOFakeWithDefault>();
            const string propName = "DefaultProp";
            BOTester<BOFakeWithDefault> boTester = CreateGenericTester<BOFakeWithDefault>();
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
        public void Test_ShouldHaveDefault_WithLambda_WithSpecifiedValue_WhenHasDefault_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDef<BOFakeWithDefault>();
            BOTester<BOFakeWithDefault> boTester = CreateGenericTester<BOFakeWithDefault>();
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
        public void Test_ShouldBeCompulsory_WithLambda_WhenCompulsory_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithCompulsory>();
            BOTester<BOFakeWithCompulsory> boTester = CreateGenericTester<BOFakeWithCompulsory>();
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
        public void Test_ShouldBeCompulsory_WithLambda_WhenNotCompulsory_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithCompulsory>();
            BOTester<BOFakeWithCompulsory> boTester = CreateGenericTester<BOFakeWithCompulsory>();
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
        public void Test_ShouldNotBeCompulsory_WithLambda_WhenNonCompulsory_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithCompulsory>();
            BOTester<BOFakeWithCompulsory> boTester = CreateGenericTester<BOFakeWithCompulsory>();
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
        public void Test_ShouldNotBeCompulsory_WithLambda_WhenCompulsory_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            SetupClassDef<BOFakeWithCompulsory>();
            BOTester<BOFakeWithCompulsory> boTester = CreateGenericTester<BOFakeWithCompulsory>();
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
        public void Test_ShouldHaveReadWriteRule_WithLambda_ReadOnly_WhenIs_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDef<FakeBOWithReadWriteRuleProp>();
            BOTester<FakeBOWithReadWriteRuleProp> boTester = CreateGenericTester<FakeBOWithReadWriteRuleProp>();
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
        public void Test_ShouldHaveReadWriteRule_WithLambda_WriteNew_WhenIsNot_ShouldAssertFalse()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = SetupClassDef<FakeBOWithReadWriteRuleProp>();
            BOTester<FakeBOWithReadWriteRuleProp> boTester = CreateGenericTester<FakeBOWithReadWriteRuleProp>();
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


        #region UniqueConstraint

        [Test]
        public void Test_ShouldBeUniqueConstraint_WithLambda_WhenSingleProp_WhenHas_ShouldAssertTrue()
        {
            //---------------Set up test pack-------------------
            const string propName = "UCProp";
            IClassDef classDef = SetupClassDef<FakeBOWithUniqueConstraint>();
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<FakeBOWithUniqueConstraint, object>(bo => bo.UCProp);

            var boTester = CreateGenericTester<FakeBOWithUniqueConstraint>() as BOTester<FakeBOWithUniqueConstraint>;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(boTester);
            propertyInfo.AssertHasUniqueConstraintAttribute();
            Assert.IsNotNull(classDef.GetPropDef(propName), "Property should be defined");
            //---------------Execute Test ----------------------
            boTester.ShouldBeUniqueConstraint(bo => bo.UCProp);
            //---------------Test Result -----------------------
            Assert.IsTrue(true, "If it has got here then passed");
        }

        [Test]
        public void Test_ShouldBeUniqueConstraint_WithLambda_WhenClassHasNoUC_ShouldRaiseAssertionException()
        {
            //---------------Set up test pack-------------------
            const string propName = "CompulsoryString";
            IClassDef classDef = SetupClassDef<FakeBO>();
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<FakeBO, object>(bo => bo.CompulsoryString);
            var boTester = CreateGenericTester<FakeBO>() as BOTester<FakeBO>;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(boTester);
            propertyInfo.AssertNotHasUniqueConstraintAttribute();
            Assert.IsNotNull(classDef.GetPropDef(propName), "Property should be defined");
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldBeUniqueConstraint(bo => bo.CompulsoryString);
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
        public void Test_ShouldBeUniqueConstraint_WithLambda_WhenClassHasUC_WhenPropNotOnUC_ShouldRaiseAssertionException()
        {
            //---------------Set up test pack-------------------
            const string propName = "NonUCProp";
            IClassDef classDef = SetupClassDef<FakeBOWithUniqueConstraint>();
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<FakeBOWithUniqueConstraint, object>(bo => bo.NonUCProp);
            var boTester = CreateGenericTester<FakeBOWithUniqueConstraint>() as BOTester<FakeBOWithUniqueConstraint>;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(boTester);
            propertyInfo.AssertNotHasUniqueConstraintAttribute();
            Assert.IsNotNull(classDef.GetPropDef(propName), "Property should be defined");
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldBeUniqueConstraint(bo => bo.NonUCProp);
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
        public void Test_ShouldBeUniqueConstraint_WithLambda_WhenClassHasCompositeUC_WhenPropNotOnCompositeUC_ShouldRaiseAssertionException()
        {
            //---------------Set up test pack-------------------
            const string propName = "ComplexUCProp1";
            IClassDef classDef = SetupClassDef<FakeBOWithUniqueConstraint>();
            var propertyInfo = ReflectionUtilities.GetPropertyInfo<FakeBOWithUniqueConstraint, object>(bo => bo.ComplexUCProp1);
            var boTester = CreateGenericTester<FakeBOWithUniqueConstraint>() as BOTester<FakeBOWithUniqueConstraint>;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(boTester);
            propertyInfo.AssertHasUniqueConstraintAttribute();
            Assert.IsNotNull(classDef.GetPropDef(propName), "Property should be defined");
            Assert.AreEqual(2, classDef.KeysCol["UC2"].Count);
            //---------------Execute Test ----------------------
            try
            {
                boTester.ShouldBeUniqueConstraint(bo => bo.ComplexUCProp1);
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
        public void Test_ShouldBeUniqueConstraint_WithLambda_WithMultiProps_WhenHasCompositeUC()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Not Yet Implemented");
        }
        #endregion




    }
}