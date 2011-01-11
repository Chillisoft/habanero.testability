namespace Habanero.Testability.Tests
{
    using AutoMappingHabanero;
    using Habanero.Base;
    using Habanero.Base.Exceptions;
    using Habanero.BO;
    using Habanero.BO.ClassDefinition;
    using Habanero.BO.Rules;
    using Habanero.Testability;
    using Habanero.Util;
    using NUnit.Framework;
    using Rhino.Mocks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    [TestFixture]
    public class TestBOTestFactory_Generic
    {
        private static PropRuleDate CreatePropRuleDateTime(DateTime min, DateTime max)
        {
            return TestUtilsFactory.CreatePropRuleDateTime(min, max);
        }

        private static PropRuleString CreatePropRuleString(int minLength, int maxLength)
        {
            return TestUtilsFactory.CreatePropRuleString(minLength, maxLength);
        }

        private static IEnumerable<IBusinessObjectRule> GetBusinessObjectRules(IBusinessObject bo)
        {
            return (ReflectionUtilities.GetPrivateMethodInfo(bo.GetType(), "GetBusinessObjectRules").Invoke(bo, new object[0]) as IList<IBusinessObjectRule>);
        }

        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }

        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }

        [TestFixtureSetUp]
        public void SetFixtureUp()
        {
        }

        [SetUp]
        public void Setup()
        {
            BOTestFactoryRegistry.Registry = null;
            BORegistry.DataAccessor = GetDataAccessorInMemory();
            ClassDef.ClassDefs.Clear();
            AllClassesAutoMapper.ClassDefCol.Clear();
            ClassDefCol classDefCol = typeof(FakeBO).MapClasses();
            ClassDef.ClassDefs.Add(classDefCol);
        }

        [Test]
        public void Test_ConstructWithBO_ShouldSetBusinessObject()
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            BOTestFactory<FakeBOWithRules> boTestFactory = new BOTestFactory<FakeBOWithRules>(bo);
            Assert.IsNotNull(boTestFactory.BusinessObject);
        }

        [Test]
        public void Test_ConstructWithBO_WhenBONull_ShouldSetBusinessObjectToNull()
        {
            FakeBOWithRules bo = null;
            Assert.IsNull(bo);
            BOTestFactory<FakeBOWithRules> boTestFactory = new BOTestFactory<FakeBOWithRules>(bo);
            Assert.IsNull(boTestFactory.BusinessObject);
        }
        [Test]
        public void Test_CreateBusinessObject()
        {
            //---------------Set up test pack-------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var businessObject = factory.CreateDefaultBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject );
        }


        [Test]
        public void Test_CreateValidBusinessObject_ShouldReturnBOWithCompulsoryPropsPopulated()
        {
            //---------------Set up test pack-------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var businessObject = factory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject);
            Assert.IsNotNull(businessObject.CompulsoryString);
        }

        [Test]
        public void Test_CreateValidBusinessObject_ShouldReturnBOWithNonCompulsoryPropsNotPopulated()
        {
            //---------------Set up test pack-------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var businessObject = factory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNull(businessObject.NonCompulsoryString);
        }

        [Test]
        public void Test_CreateValidBusinessObject_ShouldReturnBOWithCompulsoryRelationshpPopulated()
        {
            //---------------Set up test pack-------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var businessObject = factory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            var relatedFakeBo = businessObject.CompulsoryRelationship;
            Assert.IsNotNull(relatedFakeBo);
            Assert.IsTrue(relatedFakeBo.Status.IsValid());
        }

        [Test]
        public void Test_CreateValidBusinessObject_ShouldUserRegisteredFactoryWhenRequired()
        {
            //---------------Set up test pack-------------------
            BOTestFactoryRegistry boTestFactoryRegistry = BOTestFactoryRegistry.Registry;
            var fakeBOTestFactory = MockRepository.GenerateMock<BOTestFactory<RelatedFakeBo>>();
            boTestFactoryRegistry.Register<RelatedFakeBo>(fakeBOTestFactory);

            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            Assert.AreSame(fakeBOTestFactory, boTestFactoryRegistry.Resolve<RelatedFakeBo>());
            fakeBOTestFactory.AssertWasNotCalled(testFactory => testFactory.CreateValidBusinessObject(Arg<Type>.Is.Anything));
            //---------------Execute Test ----------------------
            factory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            fakeBOTestFactory.AssertWasCalled(testFactory => testFactory.CreateValidBusinessObject(Arg<Type>.Is.Anything));
        }
                [Test]
        public void Test_CreateValidBusinessObject_ShouldReturnBOWithNonCompulsoryRelationshipPopulated()
        {
            //---------------Set up test pack-------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var businessObject = factory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNull(businessObject.NonCompulsoryRelationship);
        }

        [Test]
        public void Test_SetRelationship_WhenNull_ShouldSetValue()
        {
            //---------------Set up test pack-------------------
            FakeBO bo = new BOTestFactory().CreateValidBusinessObject<FakeBO>();
            ISingleRelationship relationship = bo.Relationships["NonCompulsoryRelationship"] as ISingleRelationship;

            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
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
            FakeBO bo = new BOTestFactory().CreateValidBusinessObject<FakeBO>();
            ISingleRelationship relationship = bo.Relationships["NonCompulsoryRelationship"] as ISingleRelationship;

            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
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
        public void Test_SetPropValue_WhenString_ShouldSetToString()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {PropertyType = typeof (string)};
            IBOProp prop = new BOProp(def);
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            Assert.IsInstanceOf(typeof(string), prop.Value);
        }
        [Test]
        public void Test_SetPropValue_WhenStringAndHasValue_ShouldNotChange()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {PropertyType = typeof (string)};
            var origValue = GetRandomString();
            IBOProp prop = new BOProp(def) {Value = origValue};
            //---------------Assert Precondition----------------
            Assert.AreEqual(origValue, prop.Value);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.AreEqual(origValue, prop.Value);
        }

        [Test]
        public void Test_SetPropValue_WhenStringAndMaxLength_ShouldSetToValidValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake { PropertyType = typeof(string) };
            const int minLength = 3;
            const int maxLength = 7;
            def.AddPropRule(CreatePropRuleString(minLength, maxLength));
            IBOProp prop = new BOProp(def) ;
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(typeof(string), prop.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleString>().ToList());
            PropRuleString propRule = def.PropRules.OfType<PropRuleString>().First();
            Assert.AreEqual(minLength, propRule.MinLength);
            Assert.AreEqual(maxLength, propRule.MaxLength);
            //---------------Execute Test ----------------------
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsTrue(prop.IsValid, prop.InvalidReason);
            Assert.IsNotNull(prop.Value);
            Assert.GreaterOrEqual(prop.Value.ToString().Length, minLength);
            Assert.LessOrEqual(prop.Value.ToString().Length, maxLength);
        }

        [Test]
        public void Test_SetPropValue_WhenGuid_ShouldSetToGuid()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {PropertyType = typeof (Guid)};
            IBOProp prop = new BOProp(def);
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            Assert.IsInstanceOf(typeof(Guid), prop.Value);
        }

        [Test]
        public void Test_SetPropValue_WhenGuidValueAlreadySet_ShouldNotChangeValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {PropertyType = typeof (Guid)};
            var origValue = Guid.NewGuid();
            IBOProp prop = new BOProp(def) {Value = origValue};
            //---------------Assert Precondition----------------
            Assert.IsNotNull(prop.Value);
            Assert.AreEqual(origValue, prop.Value);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.AreEqual(origValue, prop.Value);
        }

        [Test]
        public void Test_SetPropValue_WhenInt_ShouldSetToInt()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake { PropertyType = typeof(int) };
            IBOProp prop = new BOProp(def);
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(typeof(int), prop.PropertyType);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            Assert.IsInstanceOf(typeof(int), prop.Value);
        }

        [Test]
        public void Test_SetPropValue_WhenBool_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var propertyType = typeof(bool);
            IPropDef def = new PropDefFake { PropertyType = propertyType };
            IBOProp prop = new BOProp(def);
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(propertyType, prop.PropertyType);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            Assert.IsInstanceOf(typeof(bool), prop.Value);
        }

        [Test]
        public void Test_SetPropValue_WhenDecimal_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var propertyType = typeof(decimal);
            IPropDef def = new PropDefFake { PropertyType = propertyType };
            IBOProp prop = new BOProp(def);
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(propertyType, prop.PropertyType);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            Assert.IsInstanceOf(typeof(decimal), prop.Value);
        }

        [Test]
        public void Test_SetPropValue_WhenDateTime_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var propertyType = typeof(DateTime);
            IPropDef def = new PropDefFake { PropertyType = propertyType };
            IBOProp prop = new BOProp(def);
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(propertyType, prop.PropertyType);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            Assert.IsInstanceOf(typeof(DateTime), prop.Value);
        }

        [Test]
        public void Test_SetPropValue_WhenDateTimeAndMaxValue_ShouldSetToValidValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake { PropertyType = typeof(DateTime) };
            DateTime min = DateTime.MinValue.AddDays(5555);
            DateTime max = DateTime.MinValue.AddDays(5565);
            def.AddPropRule(CreatePropRuleDateTime(min, max));
            IBOProp prop = new BOProp(def);
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(typeof(DateTime), prop.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1).AddMilliseconds(-1), propRule.MaxValue);
            //---------------Execute Test ----------------------
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsTrue(prop.IsValid, prop.InvalidReason);
            Assert.IsNotNull(prop.Value);
            Assert.GreaterOrEqual((DateTime)prop.Value, min);
            Assert.LessOrEqual((DateTime)prop.Value, max);
        }

        [Test]
        public void Test_SetPropValue_WhenEnum_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var propertyType = typeof(FakeEnum);
            IPropDef def = new PropDefFake { PropertyType = propertyType };
            IBOProp prop = new BOProp(def);
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(propertyType, prop.PropertyType);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            Assert.IsInstanceOf(typeof(FakeEnum), prop.Value);
        }

        [Test]
        public void Test_SetPropValue_WhenLookupList_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var propertyType = typeof(string);
            IPropDef def = new PropDefFake { PropertyType = propertyType };
            IBOProp prop = new BOProp(def);
            var dictionary = new Dictionary<string, string> {{"fda", "fdafasd"}, {"fdadffasdf", "gjhj"}};
            def.LookupList = new SimpleLookupList(dictionary);
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(propertyType, prop.PropertyType);
            Assert.IsNotNull(def.LookupList);
            Assert.AreEqual(2, def.LookupList.GetLookupList().Count);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            StringAssert.StartsWith("fda", prop.Value.ToString());
        }

        [Test]
        public void Test_UpdateCompulsoryProps_ShouldUpdateCompulsoryPropsAndRelationships()
        {
            //---------------Set up test pack-------------------
            FakeBO bo = new FakeBO();
            //---------------Assert Precondition----------------
            Assert.IsNull(bo.CompulsoryString);
            Assert.IsNull(bo.NonCompulsoryString);
            Assert.IsNull(bo.CompulsoryRelationship);
            Assert.IsNull(bo.NonCompulsoryRelationship);
            //---------------Execute Test ----------------------
            BOTestFactory factory = new BOTestFactory();
            factory.UpdateCompulsoryProperties(bo);
            //---------------Test Result -----------------------
            Assert.IsNotNull(bo.CompulsoryString);
            Assert.IsNull(bo.NonCompulsoryString);
            Assert.IsNotNull(bo.CompulsoryRelationship);
            Assert.IsNull(bo.NonCompulsoryRelationship);
        }
        [Test]
        public void Test_UpdateCompulsoryProps_WhenAlreadySet_ShouldNotUpdate()
        {
            //---------------Set up test pack-------------------
            FakeBO bo = new FakeBO();
            BOTestFactory factory = new BOTestFactory();
            factory.UpdateCompulsoryProperties(bo);
            var origionalString = bo.CompulsoryString;
            var origionalRel = bo.CompulsoryRelationship;
            //---------------Assert Precondition----------------
            Assert.IsNotNull(origionalString);
            Assert.IsNull(bo.NonCompulsoryString);
            Assert.IsNotNull(origionalRel);
            Assert.IsNull(bo.NonCompulsoryRelationship);
            //---------------Execute Test ----------------------
            factory.UpdateCompulsoryProperties(bo);
            //---------------Test Result -----------------------
            Assert.AreSame(origionalString, bo.CompulsoryString);
            Assert.AreSame(origionalRel, bo.CompulsoryRelationship);
        }
        [TestCase(ComparisonOperator.LessThanOrEqual), TestCase(ComparisonOperator.LessThan)]
        public void Test_FixInterPropRules_WhenDecimal_WhenInvalid_ShouldUpdateToValidValues(ComparisonOperator op)
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, op, expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = (decimal?) factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
            decimal? acquisitionCost = bo.AcquisitionCost;
            bo.ExpectedScrapValue = acquisitionCost.GetValueOrDefault() - 1;
            //---------------Assert Precondition----------------
            Assert.Greater(bo.AcquisitionCost, bo.ExpectedScrapValue);
            Assert.IsFalse(rule.IsValid(bo));
            Assert.IsFalse(bo.Status.IsValid());
            //---------------Execute Test ----------------------
            factory.FixInvalidInterPropRules(bo);
            //---------------Test Result -----------------------
            Assert.Less(bo.AcquisitionCost, bo.ExpectedScrapValue);
        }

        [Test]
        public void Test_FixInterPropRules_WhenDecimal_WhenInvalid_WhenEqualThan_ShouldUpdateToValidValues()
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, ComparisonOperator.EqualTo, expectedScrapValueProp);
            bo.AddBusinessRule(rule);

            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = (decimal?) factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
            decimal? acquisitionCost = bo.AcquisitionCost;
            bo.ExpectedScrapValue = acquisitionCost.GetValueOrDefault() + 1;
            //---------------Assert Precondition----------------
            Assert.Less(bo.AcquisitionCost, bo.ExpectedScrapValue);
            Assert.IsFalse(rule.IsValid(bo));
            Assert.IsFalse(bo.Status.IsValid());
            //---------------Execute Test ----------------------
            factory.FixInvalidInterPropRules(bo);
            //---------------Test Result -----------------------
            Assert.AreEqual(bo.AcquisitionCost, bo.ExpectedScrapValue);
        }

        [TestCase(ComparisonOperator.GreaterThanOrEqual), TestCase(ComparisonOperator.GreaterThan)]
        public void Test_FixInterPropRules_WhenDecimal_WhenInvalid_WhenGreaterThan_ShouldUpdateToValidValues(ComparisonOperator op)
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, op, expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = (decimal?) factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
            decimal? acquisitionCost = bo.AcquisitionCost;
            bo.ExpectedScrapValue = acquisitionCost.GetValueOrDefault() + 1;
            //---------------Assert Precondition----------------
            Assert.Less(bo.AcquisitionCost, bo.ExpectedScrapValue);
            Assert.IsFalse(rule.IsValid(bo));
            Assert.IsFalse(bo.Status.IsValid());
            //---------------Execute Test ----------------------
            factory.FixInvalidInterPropRules(bo);
            //---------------Test Result -----------------------
            Assert.Greater(bo.AcquisitionCost, bo.ExpectedScrapValue);
        }

        [TestCase(ComparisonOperator.LessThanOrEqual), TestCase(ComparisonOperator.LessThan)]
        public void Test_FixInterPropRules_WhenInvalidInterPropRules_ShouldUpdateToValidValues(ComparisonOperator op)
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef economicLifeProp = bo.Props["EconomicLife"].PropDef;
            IPropDef engineeringLifeProp = bo.Props["EngineeringLife"].PropDef;
            InterPropRule rule = new InterPropRule(economicLifeProp, op, engineeringLifeProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.EconomicLife = (int?) factory.GetValidPropValue(bo2 => bo2.EconomicLife);
            bo.EngineeringLife = bo.EconomicLife - 1;
            //---------------Assert Precondition----------------
            Assert.Greater(bo.EconomicLife, bo.EngineeringLife);
            Assert.IsFalse(rule.IsValid(bo));
            Assert.IsFalse(bo.Status.IsValid());
            //---------------Execute Test ----------------------
            factory.FixInvalidInterPropRules(bo);
            //---------------Test Result -----------------------
            Assert.Less(bo.EconomicLife, bo.EngineeringLife);
        }

        [Test]
        public void Test_FixInterPropRules_WhenInvalidInterPropRules_WhenEqualThan_ShouldUpdateToValidValues()
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef economicLifeProp = bo.Props["EconomicLife"].PropDef;
            IPropDef engineeringLifeProp = bo.Props["EngineeringLife"].PropDef;
            InterPropRule rule = new InterPropRule(economicLifeProp, ComparisonOperator.EqualTo, engineeringLifeProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.EconomicLife = (int?) factory.GetValidPropValue(bo2 => bo2.EconomicLife);
            bo.EngineeringLife = bo.EconomicLife + 1;
            //---------------Assert Precondition----------------
            Assert.Less(bo.EconomicLife, bo.EngineeringLife);
            Assert.IsFalse(rule.IsValid(bo));
            Assert.IsFalse(bo.Status.IsValid());
            //---------------Execute Test ----------------------
            factory.FixInvalidInterPropRules(bo);
            //---------------Test Result -----------------------
            Assert.AreEqual(bo.EconomicLife, bo.EngineeringLife);
        }

        [TestCase(ComparisonOperator.GreaterThanOrEqual), TestCase(ComparisonOperator.GreaterThan)]
        public void Test_FixInterPropRules_WhenInvalidInterPropRules_WhenGreaterThan_ShouldUpdateToValidValues(ComparisonOperator op)
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef economicLifeProp = bo.Props["EconomicLife"].PropDef;
            IPropDef engineeringLifeProp = bo.Props["EngineeringLife"].PropDef;
            InterPropRule rule = new InterPropRule(economicLifeProp, op, engineeringLifeProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.EconomicLife = (int?) factory.GetValidPropValue(bo2 => bo2.EconomicLife);
            bo.EngineeringLife = bo.EconomicLife + 1;
            //---------------Assert Precondition----------------
            Assert.Less(bo.EconomicLife, bo.EngineeringLife);
            Assert.IsFalse(rule.IsValid(bo));
            Assert.IsFalse(bo.Status.IsValid());
            //---------------Execute Test ----------------------
            factory.FixInvalidInterPropRules(bo);
            //---------------Test Result -----------------------
            Assert.Greater(bo.EconomicLife, bo.EngineeringLife);
        }

        [Test]
        public void Test_FixInterPropRules_WhenValid_ShouldNotChange_ShouldUpdateToValidValues()
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef economicLifeProp = bo.Props["EconomicLife"].PropDef;
            IPropDef engineeringLifeProp = bo.Props["EngineeringLife"].PropDef;
            InterPropRule rule = new InterPropRule(economicLifeProp, ComparisonOperator.LessThan, engineeringLifeProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.EconomicLife = (int?) factory.GetValidPropValue(bo2 => bo2.EconomicLife);
            int? origEconomicLife = bo.EconomicLife;
            bo.EngineeringLife = origEconomicLife + 1;
            int? origEngineeringLife = bo.EngineeringLife;
            //---------------Assert Precondition----------------
            Assert.Less(origEconomicLife, origEngineeringLife);
            Assert.IsTrue(rule.IsValid(bo));
            Assert.IsTrue(bo.Status.IsValid());
            //---------------Execute Test ----------------------
            factory.FixInvalidInterPropRules(bo);
            //---------------Test Result -----------------------
            Assert.AreEqual(origEconomicLife, bo.EconomicLife);
            Assert.AreEqual(origEngineeringLife, bo.EngineeringLife);
        }

        [Test]
        public void Test_GetValidPropValue_WhenBONull_ShouldRetValidValues()
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IBOProp boProp = bo.Props["ExpectedScrapValue"];
            decimal minValue = RandomValueGen.GetRandomDecimal();
            decimal maxValue = minValue + 5M;
            boProp.PropDef.AddPropRule(new PropRuleDecimal(GetRandomString(), GetRandomString(), minValue, maxValue));
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            decimal validPropValue = (decimal) factory.GetValidPropValue(null, bobj => bobj.ExpectedScrapValue);
            //---------------Test Result -----------------------
            Assert.Greater(validPropValue, minValue);
            Assert.Less(validPropValue, maxValue);
        }

        [Test]
        public void Test_GetValidPropValue_WhenCreateFactoryWithBusinessObject_ShouldTakeInterPropRulesIntoAccount()
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, ComparisonOperator.EqualTo, expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>(bo);
            bo.AcquisitionCost = (decimal?) factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
            decimal? acquisitionCost = bo.AcquisitionCost;
            bo.ExpectedScrapValue = acquisitionCost.GetValueOrDefault() + 1;
            //---------------Assert Precondition----------------
            Assert.Less(bo.AcquisitionCost, bo.ExpectedScrapValue);
            Assert.IsFalse(rule.IsValid(bo));
            Assert.IsFalse(bo.Status.IsValid());
            Assert.IsNotNull(factory.BusinessObject);
            //---------------Execute Test ----------------------
            object validExpectedScrapValue = factory.GetValidPropValue(bobj => bobj.ExpectedScrapValue);
            //---------------Test Result -----------------------
            Assert.AreEqual(bo.AcquisitionCost, validExpectedScrapValue);
        }

        [TestCase(ComparisonOperator.LessThanOrEqual), TestCase(ComparisonOperator.LessThan)]
        public void Test_GetValidPropValue_WhenInterPropRules_WhenDecimal_WhenInvalid_ShouldRetValidValues(ComparisonOperator op)
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, op, expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = (decimal?) factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
            decimal? acquisitionCost = bo.AcquisitionCost;
            bo.ExpectedScrapValue = acquisitionCost.GetValueOrDefault() - 1;
            //---------------Assert Precondition----------------
            Assert.Greater(bo.AcquisitionCost, bo.ExpectedScrapValue);
            Assert.IsFalse(rule.IsValid(bo));
            Assert.IsFalse(bo.Status.IsValid());
            //---------------Execute Test ----------------------
            factory.FixInvalidInterPropRules(bo);
            //---------------Test Result -----------------------
            Assert.Less(bo.AcquisitionCost, bo.ExpectedScrapValue);
        }

        [TestCase(ComparisonOperator.GreaterThan), TestCase(ComparisonOperator.GreaterThanOrEqual)]
        public void Test_GetValidPropValue_WhenInterPropRules_WhenDecimal_WhenInvalid_WhenGreaterThan_ShouldRetValidValues(ComparisonOperator op)
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, op, expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = (decimal?) factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
            decimal? acquisitionCost = bo.AcquisitionCost;
            bo.ExpectedScrapValue = acquisitionCost.GetValueOrDefault() + 1;
            //---------------Assert Precondition----------------
            Assert.Less(bo.AcquisitionCost, bo.ExpectedScrapValue);
            Assert.IsFalse(rule.IsValid(bo));
            Assert.IsFalse(bo.Status.IsValid());
            //---------------Execute Test ----------------------
            factory.FixInvalidInterPropRules(bo);
            //---------------Test Result -----------------------
            Assert.Greater(bo.AcquisitionCost, bo.ExpectedScrapValue);
        }

        [Test]
        public void Test_GetValidPropValue_WhenInterPropRulesLeftProp_WhenDecimal_WhenInvalid_WhenEqualThan_ShouldRetValidValues()
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, ComparisonOperator.EqualTo, expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = (decimal?) factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
            decimal? acquisitionCost = bo.AcquisitionCost;
            bo.ExpectedScrapValue = acquisitionCost.GetValueOrDefault() + 1;
            //---------------Assert Precondition----------------
            Assert.Less(bo.AcquisitionCost, bo.ExpectedScrapValue);
            Assert.IsFalse(rule.IsValid(bo));
            Assert.IsFalse(bo.Status.IsValid());
            //---------------Execute Test ----------------------
            object validPropValue = factory.GetValidPropValue(bo, bobj => bobj.AcquisitionCost);
            //---------------Test Result -----------------------
            Assert.AreEqual(validPropValue, bo.ExpectedScrapValue);
        }

        [Test]
        public void Test_GetValidPropValue_WhenInterPropRulesRigtProp_WhenDecimal_WhenInvalid_WhenEqual_ShouldRetValidValues()
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, ComparisonOperator.EqualTo, expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = (decimal?) factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
            decimal? acquisitionCost = bo.AcquisitionCost;
            bo.ExpectedScrapValue = acquisitionCost.GetValueOrDefault() + 1;
            //---------------Assert Precondition----------------
            Assert.Less(bo.AcquisitionCost, bo.ExpectedScrapValue);
            Assert.IsFalse(rule.IsValid(bo));
            Assert.IsFalse(bo.Status.IsValid());
            //---------------Execute Test ----------------------
            object validExpectedScrapValue = factory.GetValidPropValue(bo, bobj => bobj.ExpectedScrapValue);
            //---------------Test Result -----------------------
            Assert.AreEqual(bo.AcquisitionCost, validExpectedScrapValue);
        }

        [Test]
        public void Test_GetValidPropValue_WhenInvalidLambda_ShouldRetValidValue()
        {
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Execute Test ----------------------
            try
            {
                factory.GetValidPropValue(bo => bo.Status.IsValid());
                Assert.Fail("Expected to throw an ArgumentException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentException ex)
            {
                StringAssert.Contains("Not a member access", ex.Message);
            }
        }

        [Test]
        public void Test_GetValidPropValue_WhenPropRuleAndInterPropRule_PropRuleMoreRestrictive_ShouldReturnValidValue()
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IBOProp expectedScrapValueProp = bo.Props["ExpectedScrapValue"];
            PropRuleDecimal propRuleDecimal = new PropRuleDecimal(GetRandomString(), GetRandomString(), 79M, 81M);

            expectedScrapValueProp.PropDef.AddPropRule(propRuleDecimal);
            IPropDef acquisitionCostPropDef = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValuePropDef = expectedScrapValueProp.PropDef;
            InterPropRule interPropRule = new InterPropRule(acquisitionCostPropDef, ComparisonOperator.GreaterThan, expectedScrapValuePropDef);
            bo.AddBusinessRule(interPropRule);
            bo.AcquisitionCost = decimal.MaxValue - 5;
            bo.ExpectedScrapValue = bo.AcquisitionCost + 1;
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>(bo);
            string message = "";
            //---------------Assert Precondition----------------
            Assert.IsFalse(propRuleDecimal.IsPropValueValid("fd", bo.ExpectedScrapValue, ref message));
            Assert.IsFalse(interPropRule.IsValid(bo));

            //---------------Execute Test ----------------------
            decimal validPropValue = (decimal) factory.GetValidPropValue(bobj => bobj.ExpectedScrapValue);

            //---------------Test Result -----------------------
            Assert.Greater(validPropValue, 79M);
            Assert.Less(validPropValue, 81M);
        }

        [Test]
        public void Test_GetValidPropValue_WhenRightProp_WhenDecimal_WhenNoInterPropRule_ShouldRetValidValues()
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IBOProp boProp = bo.Props["ExpectedScrapValue"];
            decimal minValue = RandomValueGen.GetRandomDecimal();
            decimal maxValue = minValue + 2M;
            boProp.PropDef.AddPropRule(new PropRuleDecimal(GetRandomString(), GetRandomString(), minValue, maxValue));
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = (decimal?) factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
            bo.ExpectedScrapValue = bo.AcquisitionCost + 1;
            //---------------Assert Precondition----------------
            Assert.Less(bo.AcquisitionCost, bo.ExpectedScrapValue);
            Assert.IsFalse(bo.Status.IsValid());
            Assert.AreEqual(0, GetBusinessObjectRules(bo).Count());
            Assert.AreEqual(1, boProp.PropDef.PropRules.Count);
            //---------------Execute Test ----------------------
            decimal validPropValue = (decimal) factory.GetValidPropValue(bo, bobj => bobj.ExpectedScrapValue);

            //---------------Test Result -----------------------
            Assert.Greater(validPropValue, minValue);
            Assert.Less(validPropValue, maxValue);
        }

        [Test]
        public void Test_GetValidPropValue_WhenStringAndMaxLengthWhenPropLambda_ShouldRetValidValue()
        {
            IClassDef classDef = ClassDef.ClassDefs[typeof(FakeBO)];
            IPropDef propDef = classDef.PropDefcol.FirstOrDefault(propDef1 => propDef1.PropertyName == "CompulsoryString");
            Assert.IsNotNull(propDef);
            propDef.AddPropRule(CreatePropRuleString(3, 7));
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            Assert.AreSame(typeof(string), propDef.PropertyType);
            Assert.IsNotEmpty(propDef.PropRules.OfType<PropRuleString>().ToList());
            PropRuleString propRule = propDef.PropRules.OfType<PropRuleString>().First();
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, propRule.MinLength);
            Assert.AreEqual(7, propRule.MaxLength);
            //---------------Execute Test ----------------------
            object validPropValue = factory.GetValidPropValue(bo => bo.CompulsoryString);
            //---------------Test Result -----------------------
            Assert.IsNotNull(validPropValue);
            Assert.GreaterOrEqual(validPropValue.ToString().Length, 3);
            Assert.LessOrEqual(validPropValue.ToString().Length, 7);
            string errMessage = "";
            Assert.IsTrue(propDef.IsValueValid(validPropValue, ref errMessage));
        }

        [Test]
        public void Test_GetValidPropValue_WhenStringAndMaxLengthWhenPropName_ShouldRetValidValue()
        {
            IClassDef classDef = ClassDef.ClassDefs[typeof(FakeBO)];
            IPropDef def = classDef.PropDefcol.FirstOrDefault(propDef => propDef.PropertyName == "CompulsoryString");
            Assert.IsNotNull(def);
            def.AddPropRule(CreatePropRuleString(3, 7));
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(string), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleString>().ToList());
            PropRuleString propRule = def.PropRules.OfType<PropRuleString>().First();
            Assert.AreEqual(3, propRule.MinLength);
            Assert.AreEqual(7, propRule.MaxLength);
            //---------------Execute Test ----------------------
            object validPropValue = factory.GetValidPropValue("CompulsoryString");
            //---------------Test Result -----------------------
            Assert.IsNotNull(validPropValue);
            Assert.GreaterOrEqual(validPropValue.ToString().Length, 3);
            Assert.LessOrEqual(validPropValue.ToString().Length, 7);
            string errMessage = "";
            Assert.IsTrue(def.IsValueValid(validPropValue, ref errMessage));
        }

        [Test]
        public void Test_GetValidRelationshipValue_WhenClassNotInClassDefs_ShouldRaiseError()
        {
            Type type = typeof(Unmapped);
            BOTestFactory<Unmapped> factory = new BOTestFactory<Unmapped>();
            Assert.IsFalse(ClassDef.ClassDefs.Contains(type));
            //---------------Execute Test ----------------------
            try
            {
                factory.GetValidRelationshipValue("SomeProp");
                Assert.Fail("Expected to throw an HabaneroDeveloperException");
            }
            //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains(string.Format("The ClassDef for '{0}' does not have any classDefs Loaded", type), ex.Message);
                StringAssert.Contains("This class is designed ot be used in Testing so it is likely that your classdefs are not being loaded as part of your testing process.", ex.DeveloperMessage);
            }
        }

        [Test]
        public void Test_GetValidRelationshipValue_WhenCreateFactoryWithBusinessObject_WhenLambda_ShouldRetRelatedObject()
        {
            FakeBO bo = new FakeBO();
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>(bo);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(factory.BusinessObject);
            //---------------Execute Test ----------------------
            object validCompulsoryRelationship = factory.GetValidRelationshipValue(bobj => bobj.CompulsoryRelationship);
            //---------------Test Result -----------------------
            Assert.IsNotNull(validCompulsoryRelationship);
            Assert.IsInstanceOf<RelatedFakeBo>(validCompulsoryRelationship);
        }

        [Test]
        public void Test_GetValidRelationshipValue_WhenCreateFactoryWithBusinessObject_WhenUseNameString_ShouldRetRelatedObject()
        {
            FakeBO bo = new FakeBO();
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>(bo);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(factory.BusinessObject);
            //---------------Execute Test ----------------------
            object validCompulsoryRelationship = factory.GetValidRelationshipValue("CompulsoryRelationship");
            //---------------Test Result -----------------------
            Assert.IsNotNull(validCompulsoryRelationship);
            Assert.IsInstanceOf<RelatedFakeBo>(validCompulsoryRelationship);
        }

        [Test]
        public void Test_GetValidRelationshipValue_WhenNoPropDefForClassDef_ShouldRaiseError()
        {
            Type type = typeof(FakeBO);
            FakeBO fakeBo = new FakeBO();
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>(fakeBo);
            //---------------Assert Precondition----------------
            Assert.IsTrue(ClassDef.ClassDefs.Contains(type));
            //---------------Execute Test ----------------------
            try
            {
                factory.GetValidRelationshipValue("InvalidProp");
                Assert.Fail("Expected to throw an HabaneroDeveloperException");
            }
            //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains(string.Format("The relationship '{0}' for the ClassDef for '{1}' is not defined", "InvalidProp", type), ex.Message);
                StringAssert.Contains("This class is designed to be used in Testing so it is likely that your classdefs are not being loaded as part of your testing process.", ex.DeveloperMessage);
            }
        }

        [Test]
        public void Test_SetBusinesssObject_ShouldSetBusinessObject()
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            BOTestFactory<FakeBOWithRules> boTestFactory = new BOTestFactory<FakeBOWithRules>();
            //---------------Assert Precondition----------------
            Assert.IsNull(boTestFactory.BusinessObject);
            //---------------Execute Test ----------------------
            boTestFactory.BusinessObject = bo;
            //---------------Test Result -----------------------
            Assert.AreSame(bo, boTestFactory.BusinessObject);
        }

    }
}

