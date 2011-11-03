﻿#region Licensing Header
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
using Habanero.Smooth;
using Habanero.Testability.Tests.Base;

namespace Habanero.Testability.Tests
{
    using Habanero.Base;
    using Habanero.Base.Exceptions;
    using Habanero.BO;
    using Habanero.BO.ClassDefinition;
    using Habanero.Testability;
    using NUnit.Framework;
    using System;
    using System.Linq;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestBOTestFactory_NonGeneric
    {
        private static PropRuleString CreatePropRuleString(int minLength, int maxLength)
        {
            return TestUtilsFactory.CreatePropRuleString(minLength, maxLength);
        }

        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }

        [TestFixtureSetUp]
        public void SetFixtureUp()
        {
            BORegistry.DataAccessor = GetDataAccessorInMemory();
            AllClassesAutoMapper.ClassDefCol.Clear();
            var classDefCol = typeof(FakeBO).MapClasses(type => type.Name != "Unmapped");
            ClassDef.ClassDefs.Add(classDefCol);
        }

        [SetUp]
        public void Setup()
        {
            BOPropValueGeneratorRegistry.Instance.ClearAll();
            BOTestFactoryRegistry.Instance.ClearAll();
        }
/*        [Test]
        public void Test_CreateBusinessObject()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            IBusinessObject businessObject = new BOTestFactory().CreateDefaultBusinessObject(typeof(FakeBO));
            //---------------Test Result -----------------------
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject);
        }*/

        [Test]
        public void Test_CreateValidBusinessObject_ShouldReturnBOWithCompulsoryPropsPopulated()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            var factory = new BOTestFactory(typeof(FakeBO));
            //---------------Execute Test ----------------------
            var businessObject = factory.CreateValidBusinessObject() as FakeBO;
            //---------------Test Result -----------------------
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject);
            Assert.IsNotNull(businessObject.CompulsoryString);
        }

        [Test]
        public void Test_CreateValidBusinessObject_ShouldReturnBOWithCompulsoryRelationshipPopulated()
        {
            //---------------Set up test pack-------------------
            var factory = CreateBOTestFactory();
            //---------------Execute Test ----------------------
            var businessObject = (FakeBO) factory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            var relatedBO = businessObject.CompulsoryRelationship;
            Assert.IsNotNull(relatedBO);
        }

        private static BOTestFactory CreateBOTestFactory()
        {
            return new BOTestFactory(typeof(FakeBO));
        }

        [Test]
        public void Test_CreateValidBusinessObject_ShouldReturnBOWithCompulsoryRelationshpsPopulated()
        {
            //---------------Set up test pack-------------------
            var factory = CreateBOTestFactory();
            //---------------Execute Test ----------------------
            var businessObject = (FakeBO)factory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            var relatedBO = businessObject.CompulsoryRelationship;
            Assert.IsNotNull(relatedBO);
        }
        [Test]
        public void Test_CreateValidBusinessObject_ShouldReturnBOWithRelatedBONotSaved()
        {
            //---------------Set up test pack-------------------
            var factory = CreateBOTestFactory();
            //---------------Execute Test ----------------------
            var businessObject = (FakeBO)factory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            var relatedBO = businessObject.CompulsoryRelationship;
            Assert.IsNotNull(relatedBO);
            Assert.IsTrue(relatedBO.Status.IsNew);
        }

        [Test]
        public void Test_CreateValidBusinessObject_ShouldReturnBOWithNonCompulsoryPropsNotPopulated()
        {
            //---------------Set up test pack-------------------
            var factory = CreateBOTestFactory();
            //---------------Execute Test ----------------------
            var validBusinessObject = (FakeBO)  factory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            var nonCompulsoryString = validBusinessObject.NonCompulsoryString;
            Assert.IsNull(nonCompulsoryString);
        }

        [Test]
        public void Test_CreateSavedBusinessObject_ShouldReturnSavedValidBO()
        {
            //---------------Set up test pack-------------------
            var factory = CreateBOTestFactory();
            //---------------Execute Test ----------------------
            var validBusinessObject = factory.CreateSavedBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNotNull(validBusinessObject);
            Assert.IsTrue(validBusinessObject.Status.IsValid());
            Assert.IsFalse(validBusinessObject.Status.IsNew);
        }
        [Test]
        public void Test_GetValidPropValue_WhenClassNotInClassDefs_ShouldRaiseError()
        {
            //---------------Assert Precondition----------------
            Type type = typeof(Unmapped);
            var factory = new BOTestFactory(typeof(FakeBO));
            Assert.IsFalse(ClassDef.ClassDefs.Contains(type));
            //---------------Execute Test ----------------------
            try
            {
                factory.GetValidPropValue(type, "SomeProp");
                Assert.Fail("Expected to throw an HabaneroDeveloperException");
            }
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains(string.Format("The ClassDef for '{0}' does not have any classDefs Loaded", type), ex.Message);
                StringAssert.Contains("The BOTestFactory class is designed to be used in Testing so it is likely that your classdefs are not being loaded as part of your testing process", ex.DeveloperMessage);
            }
        }

        [Test]
        public void Test_GetValidPropValue_WhenDateTimeAndMaxValue_WhenPropDef_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            var min = RandomValueGen.GetAbsoluteMin<DateTime>().AddDays(5555.0);
            var max = RandomValueGen.GetAbsoluteMin<DateTime>().AddDays(5565.0);
            def.AddPropRule(TestUtilsFactory.CreatePropRuleDateTime(min, max));
            var factory = new BOTestFactory(typeof(FakeBO));
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            var propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max, propRule.MaxValue.Date);
            //---------------Execute Test ----------------------
            var validPropValue = factory.GetValidPropValue(def);
            //---------------Test Result -----------------------
            Assert.GreaterOrEqual((DateTime) validPropValue, min);
            Assert.LessOrEqual((DateTime) validPropValue, max);
            string errMessage = "";
            Assert.IsTrue(def.IsValueValid(validPropValue, ref errMessage), errMessage);
        }

        [Test]
        public void Test_GetValidPropValue_WhenDateTimeAndMaxValue_WhenPropName_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = RandomValueGen.GetAbsoluteMin<DateTime>().AddDays(5555.0);
            DateTime max = RandomValueGen.GetAbsoluteMin<DateTime>().AddDays(5565.0);
            def.AddPropRule(TestUtilsFactory.CreatePropRuleDateTime(min, max));
            var factory = new BOTestFactory(typeof(FakeBO));
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            var propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max, propRule.MaxValue.Date);
            //---------------Execute Test ----------------------
            var validPropValue = factory.GetValidPropValue(def);
            //---------------Test Result -----------------------
            Assert.GreaterOrEqual((DateTime) validPropValue, min);
            Assert.LessOrEqual((DateTime) validPropValue, max);
            string errMessage = "";
            Assert.IsTrue(def.IsValueValid(validPropValue, ref errMessage), errMessage);
        }

        [Test]
        public void Test_GetValidPropValue_WhenNoPropDefForClassDef_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            Type type = typeof(FakeBO);
            var factory = new BOTestFactory(typeof(FakeBO));
            //---------------Assert Precondition----------------
            Assert.IsTrue(ClassDef.ClassDefs.Contains(type));
            //---------------Execute Test ----------------------
            try
            {
                factory.GetValidPropValue(type, "InvalidProp");
                Assert.Fail("Expected to throw an HabaneroDeveloperException");
            }
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains(string.Format("The property '{0}' for the ClassDef for '{1}' is not defined", "InvalidProp", type), ex.Message);
                StringAssert.Contains("The BOTestFactory class is designed to be used in Testing so it is likely that your classdefs are not being loaded as part of your testing process.", ex.DeveloperMessage);
            }
        }

        [Test]
        public void Test_GetValidPropValue_WhenStringAndMaxLength_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {
                PropertyType = typeof(string)
            };
            def.AddPropRule(CreatePropRuleString(3, 7));
            IBOProp prop = new BOProp(def);
            var factory = new BOTestFactory(typeof(FakeBO));
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(string), prop.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleString>().ToList());
            var propRule = def.PropRules.OfType<PropRuleString>().First();
            Assert.AreEqual(3, propRule.MinLength);
            Assert.AreEqual(7, propRule.MaxLength);
            //---------------Execute Test ----------------------
            var validPropValue = factory.GetValidPropValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(validPropValue);
            Assert.GreaterOrEqual(validPropValue.ToString().Length, 3);
            Assert.LessOrEqual(validPropValue.ToString().Length, 7);
        }

        [Test]
        public void Test_GetValidPropValue_WhenStringAndMaxLengthWhenPropDef_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {
                PropertyType = typeof(string)
            };
            def.AddPropRule(CreatePropRuleString(3, 7));
            var factory = new BOTestFactory(typeof(FakeBO));
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(string), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleString>().ToList());
            var propRule = def.PropRules.OfType<PropRuleString>().First();
            Assert.AreEqual(3, propRule.MinLength);
            Assert.AreEqual(7, propRule.MaxLength);
            //---------------Execute Test ----------------------
            var validPropValue = factory.GetValidPropValue(def);
            //---------------Test Result -----------------------
            Assert.IsNotNull(validPropValue);
            Assert.GreaterOrEqual(validPropValue.ToString().Length, 3);
            Assert.LessOrEqual(validPropValue.ToString().Length, 7);
            string errMessage = "";
            Assert.IsTrue(def.IsValueValid(validPropValue, ref errMessage));
        }

        [Test]
        public void Test_GetValidPropValue_WhenStringAndMaxLengthWhenPropName_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            var classDef = ClassDef.Get<FakeBO>();
            var def = classDef.PropDefcol.FirstOrDefault(propDef => propDef.PropertyName == "CompulsoryString");
            //---------------Assert Precondition----------------
            Assert.IsNotNull(def);
            def.AddPropRule(CreatePropRuleString(3, 7));
            var factory = new BOTestFactory(typeof(FakeBO));
            Assert.AreSame(typeof(string), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleString>().ToList());
            var propRule = def.PropRules.OfType<PropRuleString>().First();
            Assert.AreEqual(3, propRule.MinLength);
            Assert.AreEqual(7, propRule.MaxLength);
            //---------------Execute Test ----------------------
            var validPropValue = factory.GetValidPropValue(typeof(FakeBO), "CompulsoryString").ToString();
            //---------------Test Result -----------------------
            Assert.IsNotNull(validPropValue);
            Assert.GreaterOrEqual(validPropValue.Length, 3);
            Assert.LessOrEqual(validPropValue.Length, 7);
            string errMessage = "";
            Assert.IsTrue(def.IsValueValid(validPropValue, ref errMessage));
        }

        #region GetValidValue

        [Test]
        public void Test_GetValidValue_WhenIsProp_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            var classDef = ClassDef.Get<FakeBO>();
            var factory = new BOTestFactory(typeof(FakeBO));
            IPropDef def = classDef.PropDefcol.FirstOrDefault(propDef => propDef.PropertyName == "CompulsoryString");
            //---------------Assert Precondition----------------
            Assert.IsNotNull(def);
            def.AddPropRule(CreatePropRuleString(3, 7));
            //---------------Execute Test ----------------------
            var validPropValue = factory.GetValidValue("CompulsoryString");
            //---------------Test Result -----------------------
            Assert.IsNotNull(validPropValue);
            string validPropStringValue = validPropValue.ToString();
            Assert.GreaterOrEqual(validPropStringValue.Length, 3);
            Assert.LessOrEqual(validPropStringValue.Length, 7);
            string errMessage = "";
            Assert.IsTrue(def.IsValueValid(validPropStringValue, ref errMessage));
        }

        [Test]
        public void Test_GetValidValue_WhenIsRelationship_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            const string relName = "NonCompulsoryRelationship";
            Type boType = typeof(FakeBO);
            var relationship = new BOTestFactory(boType).CreateValidBusinessObject().Relationships[relName] as ISingleRelationship;
            var factory = new BOTestFactory(boType);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(relationship);
            Assert.IsNull(relationship.GetRelatedObject());
            //---------------Execute Test ----------------------
            object validValue = factory.GetValidValue(relName);
            //---------------Test Result -----------------------
            Assert.IsNotNull(validValue);
            Assert.IsInstanceOf<IBusinessObject>(validValue);
            var validRelationshipValue = validValue as IBusinessObject;
            Assert.IsNotNull(validRelationshipValue);
            Assert.AreSame(validRelationshipValue.ClassDef, relationship.RelatedObjectClassDef);
        }

        [Test]
        public void Test_GetValidValue_WhenNotRelOrProp_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var factory = new BOTestFactory(typeof(FakeBO));
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                factory.GetValidValue( "InvalidProp");
                Assert.Fail("Expected to throw an HabaneroApplicationException");
            }
                //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                const string message = "is not defined as either a SingleRelationshipDef or a PropertyDef";
                StringAssert.Contains(message, ex.Message);
            }
        }
        [Test]
        public void Test_GetValidValue_WithBO_WhenIsProp_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            var classDef = ClassDef.Get<FakeBO>();
            var factory = new BOTestFactory(typeof(FakeBO));
            IPropDef def = classDef.PropDefcol.FirstOrDefault(propDef => propDef.PropertyName == "CompulsoryString");
            //---------------Assert Precondition----------------
            Assert.IsNotNull(def);
            def.AddPropRule(CreatePropRuleString(3, 7));
            //---------------Execute Test ----------------------
            var validPropValue = factory.GetValidValue(new FakeBO(), "CompulsoryString");
            //---------------Test Result -----------------------
            Assert.IsNotNull(validPropValue);
            string validPropStringValue = validPropValue.ToString();
            Assert.GreaterOrEqual(validPropStringValue.Length, 3);
            Assert.LessOrEqual(validPropStringValue.Length, 7);
            string errMessage = "";
            Assert.IsTrue(def.IsValueValid(validPropStringValue, ref errMessage));
        }

        [Test]
        public void Test_GetValidValue_WithBO_WhenIsRelationship_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            const string relName = "NonCompulsoryRelationship";
            Type boType = typeof(FakeBO);
            var relationship = new BOTestFactory(boType).CreateValidBusinessObject().Relationships[relName] as ISingleRelationship;
            var factory = new BOTestFactory(boType);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(relationship);
            Assert.IsNull(relationship.GetRelatedObject());
            //---------------Execute Test ----------------------
            object validValue = factory.GetValidValue(new FakeBO(),relName);
            //---------------Test Result -----------------------
            Assert.IsNotNull(validValue);
            Assert.IsInstanceOf<IBusinessObject>(validValue);
            var validRelationshipValue = validValue as IBusinessObject;
            Assert.IsNotNull(validRelationshipValue);
            Assert.AreSame(validRelationshipValue.ClassDef, relationship.RelatedObjectClassDef);
        }

        [Test]
        public void Test_GetValidValue_WithBO_WhenNotRelOrProp_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var factory = new BOTestFactory(typeof(FakeBO));
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                factory.GetValidValue(new FakeBO(), "InvalidProp");
                Assert.Fail("Expected to throw an HabaneroApplicationException");
            }
                //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                string message = "The property 'InvalidProp' for the ClassDef";
                StringAssert.Contains(message, ex.Message);
                message = "is not defined";
                StringAssert.Contains(message, ex.Message);
            }
        }

        #endregion
        
        [Test]
        public void Test_GetValidRelationshipValue_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            var relationship = new BOTestFactory(typeof(FakeBO)).CreateValidBusinessObject().Relationships["NonCompulsoryRelationship"] as ISingleRelationship;
            var factory = new BOTestFactory(typeof(FakeBO));
            //---------------Assert Precondition----------------
            Assert.IsNotNull(relationship);
            Assert.IsNull(relationship.GetRelatedObject());
            //---------------Execute Test ----------------------
            var validRelationshipValue = factory.GetValidRelationshipValue(relationship.RelationshipDef as ISingleValueDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(validRelationshipValue);
            Assert.AreSame(validRelationshipValue.ClassDef, relationship.RelatedObjectClassDef);
        }

        [Test]
        public void Test_GetValidRelationshipValue_ShouldCreateRelatedBO()
        {
            //---------------Set up test pack-------------------
            var boTestFactory = new BOTestFactory(typeof(FakeBO));
            var businessObject = (FakeBO) boTestFactory.CreateValidBusinessObject();
            var relationship = businessObject.Relationships["NonCompulsoryRelationship"] as ISingleRelationship;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(relationship);
            Assert.IsNull(relationship.GetRelatedObject());
            //---------------Execute Test ----------------------
            IRelationshipDef relationshipDef = relationship.RelationshipDef;
            IBusinessObject validRelationshipValue = boTestFactory.GetValidRelationshipValue(relationshipDef as ISingleValueDef);
            //---------------Test Result -----------------------
            Assert.IsNotNull(validRelationshipValue);
            Assert.IsTrue(validRelationshipValue.Status.IsNew);
        }

        [Test]
        public void Test_GetPropDef_WhenSubClass_ReturnsParentsPropDef()
        {
            //---------------Set up test pack-------------------
            Type boType = typeof (SubClassFakeBO);
            var classDef = ClassDef.ClassDefs[boType];
            const string propertyName = "SomeProp";
            var factory = new BOTestFactorySpy(boType);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(classDef);
            Assert.IsFalse(classDef.PropDefcol.Contains(propertyName));
            //---------------Execute Test ----------------------
            var propDef = BOTestFactorySpy.CallGetPropDef(classDef, propertyName);
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef);
            Assert.AreEqual(propertyName, propDef.PropertyName);
        }
        [Test]
        public void Test_GetRelDef_WhenSubClass_ReturnsParentsRelDef()
        {
            //---------------Set up test pack-------------------
            var boType = typeof (SubClassFakeBO);
            var classDef = ClassDef.ClassDefs[boType];
            const string relName = "SomeRelationship";
            var factory = new BOTestFactorySpy(boType);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(classDef);
            Assert.IsFalse(classDef.PropDefcol.Contains(relName));
            //---------------Execute Test ----------------------
            var relationshipDef = BOTestFactorySpy.CallGetRelationshipDef(boType, relName);
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationshipDef);
            Assert.AreEqual(relName, relationshipDef.RelationshipName);
        }

        [Test]
        public void Test_SetDefaultValue_WhenRelationship_ShouldCreateWithValueSet()
        {
            //---------------Set up test pack-------------------
            var relatedBO = new RelatedFakeBo();
            var boWithRelFactory = new BOTestFactory(typeof(FakeBOWithRelationship));
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.SetValueFor("SingleRelationship", relatedBO);
            var boWithRelationship = boWithRelFactory.CreateValidBusinessObject() as FakeBOWithRelationship;
            //---------------Test Result -----------------------
            Assert.IsNotNull(boWithRelationship);
            Assert.AreSame(relatedBO, boWithRelationship.SingleRelationship);
        }
        [Test]
        public void Test_SetDefaultValue_WhenProperty_ShouldReturnSetValue()
        {
            //---------------Set up test pack-------------------
            const string expectedPropValue = "SomeValue";
            const string propName = "SomeProp";
            var boWithRelFactory = new BOTestFactory(typeof(FakeBO));
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.SetValueFor(propName, expectedPropValue);
            var actualValue = boWithRelFactory.GetValidPropValue(typeof(FakeBOWithRelationship), propName);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedPropValue, actualValue);
        }

        [Test]
        public void Test_SetRelationship_WhenNull_ShouldSetValue()
        {
            //---------------Set up test pack-------------------
            var bo = new BOTestFactory(typeof(FakeBO)).CreateValidBusinessObject();
            var relationship = bo.Relationships["NonCompulsoryRelationship"] as ISingleRelationship;

            var factory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(relationship);
            Assert.IsNull(relationship.GetRelatedObject());
            //---------------Execute Test ----------------------
            factory.SetRelationshipToValidValue(relationship);
            //---------------Test Result -----------------------
            Assert.IsNotNull(relationship.GetRelatedObject());
        }
        [Test]
        public void Test_SetRelationship_WhenHasValue_ShouldNotChange()
        {
            //---------------Set up test pack-------------------
            var bo = new BOTestFactory(typeof(FakeBO)).CreateValidBusinessObject();
            var relationship = bo.Relationships["NonCompulsoryRelationship"] as ISingleRelationship;

            var factory = new BOTestFactory<FakeBO>();
            factory.SetRelationshipToValidValue(relationship);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(relationship);
            var origionalBO = relationship.GetRelatedObject();
            Assert.IsNotNull(relationship.GetRelatedObject());
            //---------------Execute Test ----------------------
            factory.SetRelationshipToValidValue(relationship);
            var actualRelatedObject = relationship.GetRelatedObject();
            //---------------Test Result -----------------------
            Assert.IsNotNull(actualRelatedObject);
            Assert.AreSame(origionalBO, actualRelatedObject);
        }
        [Test]
        public void Test_SetRelationship_WhenHasValue_AndValueRegistered_ShouldSetToRegisteredValue()
        {
            //---------------Set up test pack-------------------
            var bo = new BOTestFactory(typeof(FakeBO)).CreateValidBusinessObject();
            var relationship = bo.Relationships["NonCompulsoryRelationship"] as ISingleRelationship;

            var factory = new BOTestFactory<FakeBO>();
            factory.SetRelationshipToValidValue(relationship);
            var expectedRelationshipValue = new RelatedFakeBo();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(relationship);
            var origionalBO = relationship.GetRelatedObject();
            Assert.IsNotNull(relationship.GetRelatedObject());
            //---------------Execute Test ----------------------
            factory.SetValueFor(fakeBO => fakeBO.NonCompulsoryRelationship, expectedRelationshipValue);
            factory.SetRelationshipToValidValue(relationship);
            var actualRelatedObject = relationship.GetRelatedObject();
            //---------------Test Result -----------------------
            Assert.IsNotNull(actualRelatedObject);
            Assert.AreSame(expectedRelationshipValue, actualRelatedObject);
        }
    }
    //This is not really a spy but is used purely so that I can test private/protected methods.
    // without making them public.not sure what to call it.
    public class BOTestFactorySpy: BOTestFactory
    {
        public BOTestFactorySpy(Type boType) : base(boType)
        {
        }

        public static ISingleValueDef CallGetPropDef(IClassDef classDef, string propName)
        {
            return GetPropDef(classDef, propName, true);
        }
        public static IRelationshipDef CallGetRelationshipDef(Type classType, string propName)
        {
            return GetRelationshipDef(classType, propName, true);
        }
        public BODefaultValueRegistry GetBODefaultValueRegistry()
        {
            return _defaultValueRegistry;
        }
    }
}

