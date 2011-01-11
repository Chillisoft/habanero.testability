namespace Habanero.Testability.Tests
{
    using AutoMappingHabanero;
    using Habanero.Base;
    using Habanero.Base.Exceptions;
    using Habanero.BO;
    using Habanero.BO.ClassDefinition;
    using Habanero.Testability;
    using NUnit.Framework;
    using System;
    using System.Linq;

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
            ClassDefCol classDefCol = typeof(FakeBO).MapClasses(type => type.Name != "Unmapped");
            ClassDef.ClassDefs.Add(classDefCol);
        }

        [Test]
        public void Test_CreateBusinessObject()
        {
            IBusinessObject businessObject = new BOTestFactory().CreateDefaultBusinessObject(typeof(FakeBO));
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject);
        }

        [Test]
        public void Test_CreateValidBusinessObject_ShouldReturnBOWithCompulsoryPropsPopulated()
        {
            BOTestFactory factory = new BOTestFactory();
            FakeBO businessObject = factory.CreateValidBusinessObject(typeof(FakeBO)) as FakeBO;
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject);
            Assert.IsNotNull(businessObject.CompulsoryString);
        }

        [Test]
        public void Test_CreateValidBusinessObject_ShouldReturnBOWithCompulsoryRelationshipPopulated()
        {
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            Assert.IsNotNull(factory.CreateValidBusinessObject().CompulsoryRelationship);
        }

        [Test]
        public void Test_CreateValidBusinessObject_ShouldReturnBOWithCompulsoryRelationshpsPopulated()
        {
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            Assert.IsNotNull(factory.CreateValidBusinessObject().CompulsoryRelationship);
        }

        [Test]
        public void Test_CreateValidBusinessObject_ShouldReturnBOWithNonCompulsoryPropsNotPopulated()
        {
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            Assert.IsNull(factory.CreateValidBusinessObject().NonCompulsoryString);
        }

        [Test]
        public void Test_GetValidPropValue_WhenClassNotInClassDefs_ShouldRaiseError()
        {
            Type type = typeof(Unmapped);
            BOTestFactory factory = new BOTestFactory();
            Assert.IsFalse(ClassDef.ClassDefs.Contains(type));
            try
            {
                factory.GetValidPropValue(type, "SomeProp");
                Assert.Fail("Expected to throw an HabaneroDeveloperException");
            }
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains(string.Format("The ClassDef for '{0}' does not have any classDefs Loaded", type), ex.Message);
                StringAssert.Contains("This class is designed ot be used in Testing so it is likely that your classdefs are not being loaded as part of your testing process.", ex.DeveloperMessage);
            }
        }

        [Test]
        public void Test_GetValidPropValue_WhenDateTimeAndMaxValue_WhenPropDef_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.MinValue.AddDays(5555.0);
            DateTime max = DateTime.MinValue.AddDays(5565.0);
            def.AddPropRule(TestUtilsFactory.CreatePropRuleDateTime(min, max));
            BOTestFactory factory = new BOTestFactory();
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList<PropRuleDate>());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First<PropRuleDate>();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max, propRule.MaxValue.Date);
            object validPropValue = factory.GetValidPropValue(def);
            Assert.GreaterOrEqual((DateTime) validPropValue, min);
            Assert.LessOrEqual((DateTime) validPropValue, max);
            string errMessage = "";
            Assert.IsTrue(def.IsValueValid(validPropValue, ref errMessage), errMessage);
        }

        [Test]
        public void Test_GetValidPropValue_WhenDateTimeAndMaxValue_WhenPropName_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.MinValue.AddDays(5555.0);
            DateTime max = DateTime.MinValue.AddDays(5565.0);
            def.AddPropRule(TestUtilsFactory.CreatePropRuleDateTime(min, max));
            BOTestFactory factory = new BOTestFactory();
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList<PropRuleDate>());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First<PropRuleDate>();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max, propRule.MaxValue.Date);
            object validPropValue = factory.GetValidPropValue(def);
            Assert.GreaterOrEqual((DateTime) validPropValue, min);
            Assert.LessOrEqual((DateTime) validPropValue, max);
            string errMessage = "";
            Assert.IsTrue(def.IsValueValid(validPropValue, ref errMessage), errMessage);
        }

        [Test]
        public void Test_GetValidPropValue_WhenNoPropDefForClassDef_ShouldRaiseError()
        {
            Type type = typeof(FakeBO);
            BOTestFactory factory = new BOTestFactory();
            Assert.IsTrue(ClassDef.ClassDefs.Contains(type));
            try
            {
                factory.GetValidPropValue(type, "InvalidProp");
                Assert.Fail("Expected to throw an HabaneroDeveloperException");
            }
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains(string.Format("The property '{0}' for the ClassDef for '{1}' is not defined", "InvalidProp", type), ex.Message);
                StringAssert.Contains("This class is designed to be used in Testing so it is likely that your classdefs are not being loaded as part of your testing process.", ex.DeveloperMessage);
            }
        }

        [Test]
        public void Test_GetValidPropValue_WhenStringAndMaxLength_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(string)
            };
            def.AddPropRule(CreatePropRuleString(3, 7));
            IBOProp prop = new BOProp(def);
            BOTestFactory factory = new BOTestFactory();
            Assert.AreSame(typeof(string), prop.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleString>().ToList<PropRuleString>());
            PropRuleString propRule = def.PropRules.OfType<PropRuleString>().First<PropRuleString>();
            Assert.AreEqual(3, propRule.MinLength);
            Assert.AreEqual(7, propRule.MaxLength);
            object validPropValue = factory.GetValidPropValue(prop);
            Assert.IsNotNull(validPropValue);
            Assert.GreaterOrEqual(validPropValue.ToString().Length, 3);
            Assert.LessOrEqual(validPropValue.ToString().Length, 7);
        }

        [Test]
        public void Test_GetValidPropValue_WhenStringAndMaxLengthWhenPropDef_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(string)
            };
            def.AddPropRule(CreatePropRuleString(3, 7));
            BOTestFactory factory = new BOTestFactory();
            Assert.AreSame(typeof(string), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleString>().ToList<PropRuleString>());
            PropRuleString propRule = def.PropRules.OfType<PropRuleString>().First<PropRuleString>();
            Assert.AreEqual(3, propRule.MinLength);
            Assert.AreEqual(7, propRule.MaxLength);
            object validPropValue = factory.GetValidPropValue(def);
            Assert.IsNotNull(validPropValue);
            Assert.GreaterOrEqual(validPropValue.ToString().Length, 3);
            Assert.LessOrEqual(validPropValue.ToString().Length, 7);
            string errMessage = "";
            Assert.IsTrue(def.IsValueValid(validPropValue, ref errMessage));
        }

        [Test]
        public void Test_GetValidPropValue_WhenStringAndMaxLengthWhenPropName_ShouldRetValidValue()
        {
            IClassDef classDef = typeof(FakeBO).MapClass();
            ClassDef.ClassDefs.Add(classDef);
            IPropDef def = classDef.PropDefcol.FirstOrDefault<IPropDef>(propDef => propDef.PropertyName == "CompulsoryString");
            Assert.IsNotNull(def);
            def.AddPropRule(CreatePropRuleString(3, 7));
            BOTestFactory factory = new BOTestFactory();
            Assert.AreSame(typeof(string), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleString>().ToList<PropRuleString>());
            PropRuleString propRule = def.PropRules.OfType<PropRuleString>().First<PropRuleString>();
            Assert.AreEqual(3, propRule.MinLength);
            Assert.AreEqual(7, propRule.MaxLength);
            object validPropValue = factory.GetValidPropValue(typeof(FakeBO), "CompulsoryString");
            Assert.IsNotNull(validPropValue);
            Assert.GreaterOrEqual(validPropValue.ToString().Length, 3);
            Assert.LessOrEqual(validPropValue.ToString().Length, 7);
            string errMessage = "";
            Assert.IsTrue(def.IsValueValid(validPropValue, ref errMessage));
        }

        [Test]
        public void Test_GetValidRelationshipValue_ShouldRetValidValue()
        {
            ISingleRelationship relationship = new BOTestFactory().CreateValidBusinessObject<FakeBO>().Relationships["NonCompulsoryRelationship"] as ISingleRelationship;
            BOTestFactory factory = new BOTestFactory();
            Assert.IsNotNull(relationship);
            Assert.IsNull(relationship.GetRelatedObject());
            IBusinessObject validRelationshipValue = factory.GetValidRelationshipValue(relationship.RelationshipDef);
            Assert.IsNotNull(validRelationshipValue);
            Assert.AreSame(validRelationshipValue.ClassDef, relationship.RelatedObjectClassDef);
        }
    }
}

