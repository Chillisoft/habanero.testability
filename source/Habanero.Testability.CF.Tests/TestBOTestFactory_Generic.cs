using System.Collections;
using Habanero.Smooth;
using Habanero.Testability;
using Habanero.Testability.Helpers;
using Habanero.Testability.Tests.Base;

namespace Habanero.Testability.Tests
{
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

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestBOTestFactory_Generic
    {
        private const int MANY = 3;

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
            return
                (ReflectionUtilities.GetPrivateMethodInfo(bo.GetType(), "GetBusinessObjectRules").Invoke(bo,
                                                                                                         new object[0])
                 as IList<IBusinessObjectRule>);
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
            GlobalRegistry.LoggerFactory = new HabaneroLoggerFactoryStub();
            BOTestFactoryRegistry.Instance = null;
            BORegistry.DataAccessor = GetDataAccessorInMemory();
            ClassDef.ClassDefs.Clear();
            AllClassesAutoMapper.ClassDefCol.Clear();
            ClassDefCol classDefCol = typeof (FakeBO).MapClasses();
            ClassDef.ClassDefs.Add(classDefCol);
            ClassDef.ClassDefs.Add(FakeBOCompositeKeyAndManyRel.LoadDefaultClassDef());
            ClassDef.ClassDefs.Add(FakeBOCompositeKeySingleRel.LoadDefaultClassDef());
        }

        [Test]
        public void Test_ConstructWithBO_ShouldSetBusinessObject()
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBOWithRules> boTestFactory = new BOTestFactory<FakeBOWithRules>(bo);
            //---------------Test Result -----------------------
            Assert.IsNotNull(boTestFactory.BusinessObject);
        }

        [Test]
        public void Test_ConstructWithBO_WhenBONull_ShouldSetBusinessObjectToNull()
        {
            FakeBOWithRules bo = null;
            //---------------Assert Precondition----------------
            Assert.IsNull(bo);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBOWithRules> boTestFactory = new BOTestFactory<FakeBOWithRules>(bo);
            //---------------Test Result -----------------------
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
            Assert.IsInstanceOf<FakeBO>(businessObject);
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

        /*No AssertWasNotCalled in CF
                [Test]
                public void Test_CreateValidBusinessObject_ShouldUseRegisteredFactoryWhenRequired()
                {
                    //---------------Set up test pack-------------------
                    BOTestFactoryRegistry boTestFactoryRegistry = BOTestFactoryRegistry.Instance;
                    var fakeBOTestFactory = MockRepository.GenerateMock<BOTestFactory<RelatedFakeBo>>();
                    boTestFactoryRegistry.Register<RelatedFakeBo>(fakeBOTestFactory);

                    BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
                    //---------------Assert Precondition----------------
                    Assert.AreSame(fakeBOTestFactory, boTestFactoryRegistry.Resolve<RelatedFakeBo>());
                    fakeBOTestFactory.AssertWasNotCalled(
                        testFactory => ((BOTestFactory) testFactory).CreateSavedBusinessObject());
                    //---------------Execute Test ----------------------
                    factory.CreateValidBusinessObject();
                    //---------------Test Result -----------------------
                    fakeBOTestFactory.AssertWasCalled(testFactory => ((BOTestFactory) testFactory).CreateValidBusinessObject());
                }*/

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
            Assert.IsInstanceOf(typeof (string), prop.Value);
        }

/*//TODO brett 10 May 2011: CF
        [Test]
        public void Test_SetPropValueToValidValue_WhenHasValue_AndValueRegistered_ShouldSetToRegisteredValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {PropertyType = typeof (string), PropertyName = "NonCompulsoryString"};
            IBOProp prop = new BOProp(def);
            var expectedValue = RandomValueGen.GetRandomString();
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            //---------------Execute Test ----------------------
            var factory = new BOTestFactory<FakeBO2>();
            factory.SetValueFor(bo => bo.NonCompulsoryString, expectedValue);
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            Assert.IsInstanceOf(typeof (string), prop.Value);
            Assert.AreEqual(expectedValue, prop.Value);
        }*/

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
            IPropDef def = new PropDefFake {PropertyType = typeof (string)};
            const int minLength = 3;
            const int maxLength = 7;
            def.AddPropRule(CreatePropRuleString(minLength, maxLength));
            IBOProp prop = new BOProp(def);
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(typeof (string), prop.PropertyType);
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
            Assert.IsInstanceOf(typeof (Guid), prop.Value);
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
            IPropDef def = new PropDefFake {PropertyType = typeof (int)};
            IBOProp prop = new BOProp(def);
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(typeof (int), prop.PropertyType);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            Assert.IsInstanceOf(typeof (int), prop.Value);
        }

        [Test]
        public void Test_SetPropValue_WhenBool_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var propertyType = typeof (bool);
            IPropDef def = new PropDefFake {PropertyType = propertyType};
            IBOProp prop = new BOProp(def);
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(propertyType, prop.PropertyType);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            Assert.IsInstanceOf(typeof (bool), prop.Value);
        }

        [Test]
        public void Test_SetPropValue_WhenDecimal_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var propertyType = typeof (decimal);
            IPropDef def = new PropDefFake {PropertyType = propertyType};
            IBOProp prop = new BOProp(def);
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(propertyType, prop.PropertyType);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            Assert.IsInstanceOf(typeof (decimal), prop.Value);
        }

        [Test]
        public void Test_SetPropValue_WhenDateTime_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var propertyType = typeof (DateTime);
            IPropDef def = new PropDefFake {PropertyType = propertyType};
            IBOProp prop = new BOProp(def);
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(propertyType, prop.PropertyType);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            Assert.IsInstanceOf(typeof (DateTime), prop.Value);
        }

        [Test]
        public void Test_SetPropValue_WhenDateTimeAndMaxValue_ShouldSetToValidValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {PropertyType = typeof (DateTime)};
            DateTime min = DateTime.MinValue.AddDays(5555);
            DateTime max = DateTime.MinValue.AddDays(5565);
            def.AddPropRule(CreatePropRuleDateTime(min, max));
            IBOProp prop = new BOProp(def);
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(typeof (DateTime), prop.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1).AddMilliseconds(-1), propRule.MaxValue);
            //---------------Execute Test ----------------------
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsTrue(prop.IsValid, prop.InvalidReason);
            Assert.IsNotNull(prop.Value);
            Assert.GreaterOrEqual((DateTime) prop.Value, min);
            Assert.LessOrEqual((DateTime) prop.Value, max);
        }

        [Test]
        public void Test_SetPropValue_WhenEnum_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var propertyType = typeof (FakeEnum);
            IPropDef def = new PropDefFake {PropertyType = propertyType};
            IBOProp prop = new BOProp(def);
            //---------------Assert Precondition----------------
            Assert.IsNull(prop.Value);
            Assert.AreSame(propertyType, prop.PropertyType);
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetPropValueToValidValue(prop);
            //---------------Test Result -----------------------
            Assert.IsNotNull(prop.Value);
            Assert.IsInstanceOf(typeof (FakeEnum), prop.Value);
        }

        [Test]
        public void Test_SetPropValue_WhenLookupList_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var propertyType = typeof (string);
            IPropDef def = new PropDefFake {PropertyType = propertyType};
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
            BOTestFactory factory = new BOTestFactory(typeof (FakeBO));
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
            BOTestFactory factory = new BOTestFactory(typeof (FakeBO));
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

/*No linq.Expression in CF
        [TestCase(ComparisonOperator.LessThanOrEqual), TestCase(ComparisonOperator.LessThan)]
        public void Test_FixInterPropRules_WhenDecimal_WhenInvalid_ShouldUpdateToValidValues(ComparisonOperator op)
        {
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, op, expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
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
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, ComparisonOperator.EqualTo,
                                                   expectedScrapValueProp);
            bo.AddBusinessRule(rule);

            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
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
        public void Test_FixInterPropRules_WhenDecimal_WhenInvalid_WhenGreaterThan_ShouldUpdateToValidValues(
            ComparisonOperator op)
        {
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, op, expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
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
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef economicLifeProp = bo.Props["EconomicLife"].PropDef;
            IPropDef engineeringLifeProp = bo.Props["EngineeringLife"].PropDef;
            InterPropRule rule = new InterPropRule(economicLifeProp, op, engineeringLifeProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.EconomicLife = factory.GetValidPropValue(bo2 => bo2.EconomicLife);
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
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef economicLifeProp = bo.Props["EconomicLife"].PropDef;
            IPropDef engineeringLifeProp = bo.Props["EngineeringLife"].PropDef;
            InterPropRule rule = new InterPropRule(economicLifeProp, ComparisonOperator.EqualTo, engineeringLifeProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.EconomicLife = factory.GetValidPropValue(bo2 => bo2.EconomicLife);
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
        public void Test_FixInterPropRules_WhenInvalidInterPropRules_WhenGreaterThan_ShouldUpdateToValidValues(
            ComparisonOperator op)
        {
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef economicLifeProp = bo.Props["EconomicLife"].PropDef;
            IPropDef engineeringLifeProp = bo.Props["EngineeringLife"].PropDef;
            InterPropRule rule = new InterPropRule(economicLifeProp, op, engineeringLifeProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.EconomicLife = factory.GetValidPropValue(bo2 => bo2.EconomicLife);
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
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef economicLifeProp = bo.Props["EconomicLife"].PropDef;
            IPropDef engineeringLifeProp = bo.Props["EngineeringLife"].PropDef;
            InterPropRule rule = new InterPropRule(economicLifeProp, ComparisonOperator.LessThan, engineeringLifeProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.EconomicLife = factory.GetValidPropValue(bo2 => bo2.EconomicLife);
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
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IBOProp boProp = bo.Props["ExpectedScrapValue"];
            decimal minValue = RandomValueGen.GetRandomDecimal();
            decimal maxValue = minValue + 5M;
            boProp.PropDef.AddPropRule(new PropRuleDecimal(GetRandomString(), GetRandomString(), minValue, maxValue));
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            decimal? validPropValue = factory.GetValidPropValue(null, bobj => bobj.ExpectedScrapValue);
            //---------------Test Result -----------------------
            Assert.Greater(validPropValue, minValue);
            Assert.Less(validPropValue, maxValue);
        }

        [Test]
        public void Test_GetValidPropValue_WhenCreateFactoryWithBusinessObject_ShouldTakeInterPropRulesIntoAccount()
        {
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, ComparisonOperator.EqualTo,
                                                   expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>(bo);
            bo.AcquisitionCost = factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
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
        public void Test_GetValidPropValue_WhenInterPropRules_WhenDecimal_WhenInvalid_ShouldRetValidValues(
            ComparisonOperator op)
        {
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, op, expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
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
        public void
            Test_GetValidPropValue_WhenInterPropRules_WhenDecimal_WhenInvalid_WhenGreaterThan_ShouldRetValidValues(
            ComparisonOperator op)
        {
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, op, expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
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
        public void
            Test_GetValidPropValue_WhenInterPropRulesLeftProp_WhenDecimal_WhenInvalid_WhenEqualThan_ShouldRetValidValues
            ()
        {
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, ComparisonOperator.EqualTo,
                                                   expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
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
        public void
            Test_GetValidPropValue_WhenInterPropRulesRigtProp_WhenDecimal_WhenInvalid_WhenEqual_ShouldRetValidValues()
        {
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IPropDef acquisitionCostProp = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValueProp = bo.Props["ExpectedScrapValue"].PropDef;
            InterPropRule rule = new InterPropRule(acquisitionCostProp, ComparisonOperator.EqualTo,
                                                   expectedScrapValueProp);
            bo.AddBusinessRule(rule);
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
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
            //---------------Set up test pack-------------------
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
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IBOProp expectedScrapValueProp = bo.Props["ExpectedScrapValue"];
            PropRuleDecimal propRuleDecimal = new PropRuleDecimal(GetRandomString(), GetRandomString(), 79M, 81M);

            expectedScrapValueProp.PropDef.AddPropRule(propRuleDecimal);
            IPropDef acquisitionCostPropDef = bo.Props["AcquisitionCost"].PropDef;
            IPropDef expectedScrapValuePropDef = expectedScrapValueProp.PropDef;
            InterPropRule interPropRule = new InterPropRule(acquisitionCostPropDef, ComparisonOperator.GreaterThan,
                                                            expectedScrapValuePropDef);
            bo.AddBusinessRule(interPropRule);
            bo.AcquisitionCost = decimal.MaxValue - 5;
            bo.ExpectedScrapValue = bo.AcquisitionCost + 1;
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>(bo);
            string message = "";
            //---------------Assert Precondition----------------
            Assert.IsFalse(propRuleDecimal.IsPropValueValid("fd", bo.ExpectedScrapValue, ref message));
            Assert.IsFalse(interPropRule.IsValid(bo));

            //---------------Execute Test ----------------------
            decimal? validPropValue = factory.GetValidPropValue(bobj => bobj.ExpectedScrapValue);
            //---------------Test Result -----------------------
            Assert.Greater(validPropValue, 79M);
            Assert.Less(validPropValue, 81M);
        }

        [Test]
        public void Test_GetValidPropValue_WhenRightProp_WhenDecimal_WhenNoInterPropRule_ShouldRetValidValues()
        {
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            IBOProp boProp = bo.Props["ExpectedScrapValue"];
            decimal minValue = RandomValueGen.GetRandomDecimal();
            decimal maxValue = minValue + 2M;
            boProp.PropDef.AddPropRule(new PropRuleDecimal(GetRandomString(), GetRandomString(), minValue, maxValue));
            BOTestFactory<FakeBOWithRules> factory = new BOTestFactory<FakeBOWithRules>();
            bo.AcquisitionCost = factory.GetValidPropValue(bo2 => bo2.AcquisitionCost);
            bo.ExpectedScrapValue = bo.AcquisitionCost + 1;
            //---------------Assert Precondition----------------
            Assert.Less(bo.AcquisitionCost, bo.ExpectedScrapValue);
            Assert.IsFalse(bo.Status.IsValid());
            Assert.AreEqual(0, GetBusinessObjectRules(bo).Count());
            Assert.AreEqual(1, boProp.PropDef.PropRules.Count);
            //---------------Execute Test ----------------------
            decimal? validPropValue = factory.GetValidPropValue(bo, bobj => bobj.ExpectedScrapValue);
            //---------------Test Result -----------------------
            Assert.Greater(validPropValue, minValue);
            Assert.Less(validPropValue, maxValue);
        }

        [Test]
        public void Test_GetValidPropValue_WhenStringAndMaxLengthWhenPropLambda_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = ClassDef.ClassDefs[typeof (FakeBO)];
            IPropDef propDef =
                classDef.PropDefcol.FirstOrDefault(propDef1 => propDef1.PropertyName == "CompulsoryString");
            Assert.IsNotNull(propDef);
            propDef.AddPropRule(CreatePropRuleString(3, 7));
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            Assert.AreSame(typeof (string), propDef.PropertyType);
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
        }*/

        [Test]
        public void Test_GetValidPropValue_WhenStringAndMaxLengthWhenPropName_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            IClassDef classDef = ClassDef.ClassDefs[typeof (FakeBO)];
            IPropDef def = classDef.PropDefcol.FirstOrDefault(propDef => propDef.PropertyName == "CompulsoryString");
            Assert.IsNotNull(def);
            def.AddPropRule(CreatePropRuleString(3, 7));
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof (string), def.PropertyType);
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
            //---------------Set up test pack-------------------
            Type type = typeof (Unmapped);
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
                StringAssert.Contains(string.Format("The ClassDef for '{0}' does not have any classDefs Loaded", type),
                                      ex.Message);
                StringAssert.Contains(
                    "The BOTestFactory class is designed to be used in Testing so it is likely that your classdefs are not being loaded as part of your testing process",
                    ex.DeveloperMessage);
            }
        }

/*No linq.Expression in CF
        [Test]
        public void Test_GetValidRelationshipValue_WhenCreateFactoryWithBusinessObject_WhenLambda_ShouldRetRelatedObject

            ()
        {
            //---------------Set up test pack-------------------
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
        */

        [Test]
        public void
            Test_GetValidRelationshipValue_WhenCreateFactoryWithBusinessObject_WhenUseNameString_ShouldRetRelatedObject()
        {
            //---------------Set up test pack-------------------
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
            //---------------Set up test pack-------------------
            Type type = typeof (FakeBO);
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
                StringAssert.Contains(
                    string.Format("The relationship '{0}' for the ClassDef for '{1}' is not defined", "InvalidProp",
                                  type), ex.Message);
                StringAssert.Contains(
                    "The BOTestFactory class is designed to be used in Testing so it is likely that your classdefs are not being loaded as part of your testing process.",
                    ex.DeveloperMessage);
            }
        }

        [Test]
        public void Test_SetBusinesssObject_ShouldSetBusinessObject()
        {
            //---------------Set up test pack-------------------
            FakeBOWithRules bo = new FakeBOWithRules();
            BOTestFactory<FakeBOWithRules> boTestFactory = new BOTestFactory<FakeBOWithRules>();
            //---------------Assert Precondition----------------
            Assert.IsNull(boTestFactory.BusinessObject);
            //---------------Execute Test ----------------------
            boTestFactory.BusinessObject = bo;
            //---------------Test Result -----------------------
            Assert.AreSame(bo, boTestFactory.BusinessObject);
        }

        [Test]
        public void Test_SetValueFor_WhenRelationshipName_ShouldCreateWithRelationshipSet()
        {
            //---------------Set up test pack-------------------
            RelatedFakeBo relatedBO = new RelatedFakeBo();
            var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.SetValueFor("SingleRelationship", relatedBO);
            var boWithRelationship = boWithRelFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreSame(relatedBO, boWithRelationship.SingleRelationship);
        }

        [Test]
        public void Test_SetValueFor_WithRelationship_ShouldCreateWithRelationshipSet()
        {
            //---------------Set up test pack-------------------
            RelatedFakeBo relatedBO = new RelatedFakeBo();
            var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.SetValueFor("SingleRelationship", relatedBO);
            var boWithRelationship = boWithRelFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreSame(relatedBO, boWithRelationship.SingleRelationship);
        }

        [Test]
        public void Test_SetValueFor_WithProperty_ShouldCreateWithPropertySet()
        {
            //---------------Set up test pack-------------------
            const string expectedPropValue = "SomeValue";
            var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.SetValueFor("SomeProp", expectedPropValue);
            var boWithRelationship = boWithRelFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreSame(expectedPropValue, boWithRelationship.SomeProp);
        }

        [Test]
        public void Test_WithMany_ManyRelationship_ShouldCreateEntryInDefaultValueRegistry()
        {
            //---------------Set up test pack-------------------
            const int expectedNoOfCreatedChildObjects = 3;
            var boWithRelFactory = new GenericBOTestFactorySpy<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.WithMany<RelatedFakeBo>("RelatedFakeBos");
            //---------------Test Result -----------------------
            var boDefaultValueRegistry = boWithRelFactory.GetBODefaultValueRegistry();
            IList lists = (IList) boDefaultValueRegistry.Resolve("RelatedFakeBos");
            Assert.AreEqual(expectedNoOfCreatedChildObjects, lists.Count);
        }

        [Test]
        public void Test_WithMany_ShouldCreate3RelatedObjects()
        {
            //---------------Set up test pack-------------------
            //For testing I consider 3 to be many i.e. there may be special cases where the 
            // test can handle one or even two objects but most algorithms that work on 3
            // items will work on n items.
            const int expectedNoOfCreatedChildObjects = 3;
            var boWithRelFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.WithMany<RelatedFakeBo>("RelatedFakeBos");
            var boWithRelationship = boWithRelFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedNoOfCreatedChildObjects, boWithRelationship.RelatedFakeBos.Count);
        }

        [Test]
        public void Test_WithMany_ShouldReturnFactory()
        {
            //---------------Set up test pack-------------------
            var expectedFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var returnedFactory = expectedFactory.WithMany<RelatedFakeBo>("RelatedFakeBos");
            //---------------Test Result -----------------------
            Assert.AreSame(expectedFactory, returnedFactory);
        }

        [Test]
        public void Test_WithMany_WhenSpecifiedNo_ShouldCreateSpecifiedRelatedObjects()
        {
            //---------------Set up test pack-------------------
            const int expectedNoOfCreatedChildObjects = 5;
            var boWithRelFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.WithMany<RelatedFakeBo>("RelatedFakeBos", expectedNoOfCreatedChildObjects);
            var boWithRelationship = boWithRelFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedNoOfCreatedChildObjects, boWithRelationship.RelatedFakeBos.Count);
        }

        [Test]
        public void Test_WithMany_WhenOtherRel_ShouldCreateSpecifiedRelatedObjects()
        {
            //---------------Set up test pack-------------------
            const int expectedNoOfCreatedChildObjects = 4;
            var boWithRelFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.WithMany<RelatedFakeBo>("OtherRelatedFakeBos", expectedNoOfCreatedChildObjects);
            var boWithRelationship = boWithRelFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedNoOfCreatedChildObjects, boWithRelationship.OtherRelatedFakeBos.Count);
        }

        [Test]
        public void Test_WithMany_WithSpecifiedNo_ShouldReturnFactory()
        {
            //---------------Set up test pack-------------------
            var expectedFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var returnedFactory = expectedFactory.WithMany<RelatedFakeBo>("RelatedFakeBos", 6);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedFactory, returnedFactory);
        }

        [Test]
        public void Test_WithOne_WhenOtherRel_WhenCompositeKey_ShouldCreateSpecifiedRelatedObjects_WithFKSet()
        {
            //---------------Set up test pack-------------------
            var boWithRelFactory = new BOTestFactory<FakeBOCompositeKeyAndManyRel>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.WithOne<FakeBOCompositeKeySingleRel>("RelatedFakeBos");
            var boWithRelationship = boWithRelFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, boWithRelationship.RelatedFakeBos.Count);
            var relatedFakeBo = boWithRelationship.RelatedFakeBos[0];
            Assert.AreEqual(boWithRelationship.GetPropertyValue("OrganisationID"),
                            relatedFakeBo.GetPropertyValue("OrganisationID"));
            Assert.AreEqual(boWithRelationship.GetPropertyValue("Name"), relatedFakeBo.GetPropertyValue("Name"));
        }

        [Test]
        public void Test_WithOne_ShouldCreateOneRelatedObjects()
        {
            //---------------Set up test pack-------------------
            //For testing I consider 3 to be many i.e. there may be special cases where the 
            // test can handle one or even two objects but most algorithms that work on 3
            // items will work on n items.
            const int expectedNoOfCreatedChildObjects = 1;
            var boWithRelFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.WithOne<RelatedFakeBo>("RelatedFakeBos");
            var boWithRelationship = boWithRelFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedNoOfCreatedChildObjects, boWithRelationship.RelatedFakeBos.Count);
        }

        [Test]
        public void Test_WithOne_WithSpecifiedNo_ShouldReturnFactory()
        {
            //---------------Set up test pack-------------------
            var expectedFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var returnedFactory = expectedFactory.WithOne<RelatedFakeBo>("RelatedFakeBos");
            //---------------Test Result -----------------------
            Assert.AreSame(expectedFactory, returnedFactory);
        }

        [Test]
        public void Test_WithTwo_ShouldCreateOneRelatedObjects()
        {
            //---------------Set up test pack-------------------
            //For testing I consider 3 to be many i.e. there may be special cases where the 
            // test can handle one or even two objects but most algorithms that work on 3
            // items will work on n items.
            const int expectedNoOfCreatedChildObjects = 2;
            var boWithRelFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.WithTwo<RelatedFakeBo>("RelatedFakeBos");
            var boWithRelationship = boWithRelFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedNoOfCreatedChildObjects, boWithRelationship.RelatedFakeBos.Count);
        }

        [Test]
        public void Test_WithTwo_WithSpecifiedNo_ShouldReturnFactory()
        {
            //---------------Set up test pack-------------------
            var expectedFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var returnedFactory = expectedFactory.WithTwo<RelatedFakeBo>("RelatedFakeBos");
            //---------------Test Result -----------------------
            Assert.AreSame(expectedFactory, returnedFactory);
        }

        [Test]
        public void Test_WithOutSingleRelationships_ShouldNotSetCompulsorySingleRels()
        {
            //---------------Set up test pack-------------------
            var boWithRelFactory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            var initialBO = boWithRelFactory.CreateValidBusinessObject();
            Assert.IsNotNull(initialBO.CompulsoryRelationship);
            //---------------Execute Test ----------------------
            boWithRelFactory.WithOutSingleRelationships();
            var boWithRelationship = boWithRelFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNull(boWithRelationship.CompulsoryRelationship);
        }

        [Test]
        public void Test_WithOutSingleRelationships_ShouldReturnFactory()
        {
            //---------------Set up test pack-------------------
            var expectedFactory = new BOTestFactory<FakeBO>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            BOTestFactory<FakeBO> returnedFactory = expectedFactory.WithOutSingleRelationships();
            //---------------Test Result -----------------------
            Assert.AreSame(expectedFactory, returnedFactory);
        }

        /*No linq.Expression in CF
                [Test]
                public void Test_SetValueFor_WithLambda_GetPropertyValue_ShouldReturnSetValue()
                {
                    //---------------Set up test pack-------------------
                    const string expectedPropValue = "SomeValue";
                    var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
                    //---------------Assert Precondition----------------
                    //---------------Execute Test ----------------------
                    Expression<Func<FakeBOWithRelationship, string>> propertyExpression = alert => alert.SomeProp;
                    boWithRelFactory.SetValueFor(propertyExpression, expectedPropValue);
                    var actualValue = boWithRelFactory.GetValidPropValue(propertyExpression);
                    //---------------Test Result -----------------------
                    Assert.AreSame(expectedPropValue, actualValue);
                }
        */

        [Test]
        public void Test_SetValueFor_WithName_GetPropertyValue_ShouldReturnSetValue()
        {
            //---------------Set up test pack-------------------
            const string expectedPropValue = "SomeValue";
            const string propertyName = "SomeProp";
            var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.SetValueFor(propertyName, expectedPropValue);
            var actualValue = boWithRelFactory.GetValidPropValue(propertyName);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedPropValue, actualValue);
        }

        [Test]
        public void Test_SetValueFor_WithName_WhenRelationship_ShouldReturnSetValue()
        {
            //---------------Set up test pack-------------------
            var expectedRelationshipValue = new RelatedFakeBo();

            const string relationshipName = "SingleRelationship";
            var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boWithRelFactory.SetValueFor(relationshipName, expectedRelationshipValue);
            var actualValue = boWithRelFactory.GetValidRelationshipValue(relationshipName);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedRelationshipValue, actualValue);
        }

        /*No linq.Expression in CF
                        [Test]
                        public void Test_SetValueFor_WithLambda_WhenRelationship_ShouldReturnSetValue()
                        {
                            //---------------Set up test pack-------------------
                            var expectedRelationshipValue = new RelatedFakeBo();

                            const string relationshipName = "SingleRelationship";
                            var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
                            Expression<Func<FakeBOWithRelationship, RelatedFakeBo>> propertyExpression =
                                alert => alert.SingleRelationship;
                            //---------------Assert Precondition----------------
                            //---------------Execute Test ----------------------
                            boWithRelFactory.SetValueFor(propertyExpression, expectedRelationshipValue);
                            var actualValue = boWithRelFactory.GetValidRelationshipValue(relationshipName);
                            //---------------Test Result -----------------------
                            Assert.AreSame(expectedRelationshipValue, actualValue);
                        }

                        [Test]
                        public void Test_SetValueFor_WithLambda_GetPropertyValue_WithFalse_ShouldReturnFalse()
                        {
                            //---------------Set up test pack-------------------
                            var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
                            //---------------Assert Precondition----------------
                            //---------------Execute Test ----------------------
                            Expression<Func<FakeBOWithRelationship, bool?>> propertyExpression = alert => alert.BoolProp;
                            boWithRelFactory.SetValueFor(propertyExpression, false);
                            bool? actualValue = boWithRelFactory.GetValidPropValue(propertyExpression);
                            //---------------Test Result -----------------------
                            Assert.IsNotNull(actualValue);
                            Assert.IsFalse((bool) actualValue);
                        }

                        [Test]
                        public void Test_SetValueFor_WithLambda_GetPropertyValue_WithTrue_ShouldReturnTrue()
                        {
                            //---------------Set up test pack-------------------
                            var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
                            //---------------Assert Precondition----------------
                            //---------------Execute Test ----------------------
                            Expression<Func<FakeBOWithRelationship, bool?>> propertyExpression = alert => alert.BoolProp;
                            boWithRelFactory.SetValueFor(propertyExpression, true);
                            bool? actualValue = boWithRelFactory.GetValidPropValue(propertyExpression);
                            //---------------Test Result -----------------------
                            Assert.IsNotNull(actualValue);
                            Assert.IsTrue((bool) actualValue);
                        }

                        [Test]
                        public void Test_SetValueFor_ToFalse_WithLambda_WhenHasDefaultTrue_ShouldReturnFalse()
                        {
                            //---------------Set up test pack-------------------
                            var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
                            //---------------Assert Precondition----------------
                            Assert.IsTrue(boWithRelFactory.CreateDefaultBusinessObject().BoolPropWithDefault.GetValueOrDefault());
                            //---------------Execute Test ----------------------
                            Expression<Func<FakeBOWithRelationship, bool?>> propertyExpression = alert => alert.BoolPropWithDefault;
                            boWithRelFactory.SetValueFor(propertyExpression, false);
                            bool? actualValue = boWithRelFactory.GetValidPropValue(propertyExpression);
                            //---------------Test Result -----------------------
                            Assert.IsNotNull(actualValue);
                            Assert.IsFalse((bool) actualValue);
                        }

                        [Test]
                        public void Test_CreateValidBusinessObject_WhenSetValueForToFalse_WhenHasDefaultTrue_ShouldSetPropToFalse()
                        {
                            //---------------Set up test pack-------------------
                            var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
                            //---------------Assert Precondition----------------
                            Assert.IsTrue(boWithRelFactory.CreateDefaultBusinessObject().BoolPropWithDefault.GetValueOrDefault());
                            //---------------Execute Test ----------------------
                            boWithRelFactory.SetValueFor(alert => alert.BoolPropWithDefault, false);
                            var bo = boWithRelFactory.CreateValidBusinessObject();
                            //---------------Test Result -----------------------
                            Assert.IsNotNull(bo);
                            Assert.IsNotNull(bo.BoolPropWithDefault);
                            Assert.IsFalse(bo.BoolPropWithDefault.GetValueOrDefault());
                        }
        
                        [Ignore("This should be easy to implement")]
                        //TODO Brett 14 May 2010: Ignored Test - This should be easy to implement
                        [Test]
                        public void
                            Test_CreateValidBusinessObject_WhenSetValueFor_WhenPropNotForABOPropOrRelationship_ShouldSetValueForViaPropInfo
                            ()
                        {
                            //---------------Set up test pack-------------------
                            var boWithReflectiveProp = new BOTestFactory<FakeBOWithReflectiveProp>();
                            //---------------Assert Precondition----------------
                            Assert.IsNullOrEmpty(boWithReflectiveProp.CreateValidBusinessObject().ReflectiveProp);
                            //---------------Execute Test ----------------------
                            var randomString = RandomValueGen.GetRandomString();
                            boWithReflectiveProp.SetValueFor(alert => alert.ReflectiveProp, randomString);
                            var bo = boWithReflectiveProp.CreateValidBusinessObject();
                            //---------------Test Result -----------------------
                            Assert.IsNotNull(bo);
                            Assert.IsNotNullOrEmpty(bo.ReflectiveProp);
                            Assert.AreEqual(randomString, bo.ReflectiveProp);
                        }
                */

        [Test]
        public void Test_CreateSavedBusinessObject_ShouldReturnSavedValidBO()
        {
            //---------------Set up test pack-------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Execute Test ----------------------
            FakeBO validBusinessObject = factory.CreateSavedBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNotNull(validBusinessObject);
            Assert.IsTrue(validBusinessObject.Status.IsValid());
            Assert.IsFalse(validBusinessObject.Status.IsNew);
        }

        [Test]
        public void Test_CreateSavedBusinessObject_ShouldNotSetNonCompulsoryPropValue()
        {
            //---------------Set up test pack-------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Execute Test ----------------------
            FakeBO validBusinessObject = factory.CreateSavedBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNullOrEmpty(validBusinessObject.NonCompulsoryString);
        }

        /*No linq.Expression in CF
                
        */

        /// <summary>
        /// <see cref="Test_CreateSavedBusinessObject_ShouldNotSetNonCompulsoryPropValue"/>
        /// </summary>
        [Test]
        public void Test_CreateSavedBusinessObject_WhenSetValueForNonCompulsoryProp_ShouldSetNonCompulsoryPropValue()
        {
            //---------------Set up test pack-------------------
            var factory = new BOTestFactory<FakeBO2>();
            factory.SetValueFor("NonCompulsoryString");
            //---------------Execute Test ----------------------
            var validBusinessObject = factory.CreateSavedBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNotNullOrEmpty(validBusinessObject.NonCompulsoryString);
        }

        /// <summary>
        /// <see cref="Test_CreateSavedBusinessObject_WhenSetValueForNonCompulsoryRel_ShouldSetNonCompulsoryRelationship"/>
        /// </summary>
        [Test]
        public void Test_CreateSavedBusinessObject_ShouldNotSetNonCompulsoryRelationship()
        {
            //---------------Set up test pack-------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            //---------------Execute Test ----------------------
            FakeBO validBusinessObject = factory.CreateSavedBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNull(validBusinessObject.NonCompulsoryRelationship);
        }

        /// <summary>
        /// <see cref="Test_CreateSavedBusinessObject_ShouldNotSetNonCompulsoryRelationship"/>
        /// </summary>
        [Test]
        public void Test_CreateSavedBusinessObject_WhenSetValueForNonCompulsoryRel_ShouldSetNonCompulsoryRelationship()
        {
            //---------------Set up test pack-------------------
            BOTestFactory<FakeBO> factory = new BOTestFactory<FakeBO>();
            factory.SetValueFor("NonCompulsoryRelationship");
            //---------------Execute Test ----------------------
            FakeBO validBusinessObject = factory.CreateSavedBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNotNull(validBusinessObject.NonCompulsoryRelationship);
        }

        [Test]
        public void Test_UpdateCompulsoryProps_WhenSetValueFor_ShouldSetValuesToRegisteredValue()
        {
            //---------------Set up test pack-------------------
            var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
            var bo = boWithRelFactory.CreateDefaultBusinessObject();
            var expectedRelationshipValue = new RelatedFakeBo();
            //---------------Assert Precondition----------------
            Assert.IsTrue(bo.BoolPropWithDefault.GetValueOrDefault());
            //---------------Execute Test ----------------------
            boWithRelFactory.SetValueFor("BoolPropWithDefault", false);
            boWithRelFactory.SetValueFor("SingleRelationship", expectedRelationshipValue);
            boWithRelFactory.UpdateCompulsoryProperties(bo);
            //---------------Test Result -----------------------
            Assert.IsFalse(bo.BoolPropWithDefault.GetValueOrDefault());
            Assert.AreSame(expectedRelationshipValue, bo.SingleRelationship);
        }

        [Test]
        public void Test_CreateManySavedBusinessObjects_ShouldCreate3()
        {
            //---------------Set up test pack-------------------
            var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var bos = boWithRelFactory.CreateManySavedBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreEqual(3, bos.Count, "For the puroposes of testability Many is considered tobe 3");
        }

        [Test]
        public void Test_CreateManySavedBusinessObjects_WithSpecifiedNo_ShouldCreateSpecifiedNo()
        {
            //---------------Set up test pack-------------------
            var boWithRelFactory = new BOTestFactory<FakeBOWithRelationship>();
            const int noToCreate = 5;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var bos = boWithRelFactory.CreateManySavedBusinessObject(noToCreate);
            //---------------Test Result -----------------------
            Assert.AreEqual(noToCreate, bos.Count, "For the puroposes of testability Many is considered tobe 3");
            Assert.IsFalse(bos[0].Status.IsNew);
            Assert.IsFalse(bos[0].Status.IsDirty);
        }

/*No linq.Expression in CF

        [Test]
        public void Test_GetValidValueGenerator_WhenSetValidValueGenerator_ShouldRetCustomGen()
        {
            //---------------Set up test pack-------------------
            var type = typeof (FakeBO2);
            ClassDef.ClassDefs.Add(type.MapClass());
            var boTestFactory = new BOTestFactory<FakeBO2>();
            boTestFactory.SetValidValueGenerator(bo => bo.NonCompulsoryString, typeof (ValidValueGeneratorGuid));

            IClassDef classDef = ClassDef.ClassDefs[type];

            IPropDef def = classDef.GetPropDef("NonCompulsoryString", false);

            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            ValidValueGenerator generator = boTestFactory.GetValidValueGenerator(def);
            //---------------Test Result -----------------------
            Assert.IsNotNull(generator);
            Assert.IsInstanceOf<ValidValueGeneratorGuid>(generator);
        }

        [Test]
        public void Test_GetValidValueGenerator_WhenSetValidValueGenerator_ShouldRetCustomGen_WithParameter()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBO2);
            ClassDef.ClassDefs.Add(type.MapClass());
            var boTestFactory = new BOTestFactory<FakeBO2>();
            var parameter = RandomValueGen.GetRandomString();
            boTestFactory.SetValidValueGenerator(bo => bo.NonCompulsoryString, typeof(ValidValueGeneratorWithParameterWithFake), parameter);

            IClassDef classDef = ClassDef.ClassDefs[type];

            IPropDef def = classDef.GetPropDef("NonCompulsoryString", false);

            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            ValidValueGenerator generator = boTestFactory.GetValidValueGenerator(def);
            //---------------Test Result -----------------------
            Assert.IsNotNull(generator);
            Assert.IsInstanceOf<ValidValueGeneratorWithParameterWithFake>(generator);
        }

        [Test]
        public void
            Test_GetValidValueGenerator_WhenSetValidValueGenerator_WhenCallFromDiffFactoryInstance_ShouldRetCustomGenerator
            ()
        {
            //---------------Set up test pack-------------------
            var type = typeof (FakeBO2);
            ClassDef.ClassDefs.Add(type.MapClass());
            var boTestFactory = new BOTestFactory<FakeBO2>();
            boTestFactory.SetValidValueGenerator(bo => bo.NonCompulsoryString, typeof (ValidValueGeneratorGuid));

            IClassDef classDef = ClassDef.ClassDefs[type];

            IPropDef def = classDef.GetPropDef("NonCompulsoryString", false);

            //---------------Assert Precondition----------------    
            Assert.IsInstanceOf<ValidValueGeneratorGuid>(boTestFactory.GetValidValueGenerator(def));
            //---------------Execute Test ----------------------
            ValidValueGenerator generator = new BOTestFactory<FakeBO2>().GetValidValueGenerator(def);
            //---------------Test Result -----------------------
            Assert.IsNotNull(generator);
            Assert.IsInstanceOf<ValidValueGeneratorGuid>(generator);
        }

        [Test]
        public void
            TestAccept_GetValidValue_WhenSetValidValueGenerator_WhenCallFromDiffFactoryInstance_ShouldRetValueFromCustomGenerator
            ()
        {
            //---------------Set up test pack-------------------
            var type = typeof (FakeBO2);
            ClassDef.ClassDefs.Add(type.MapClass());
            var boTestFactory = new BOTestFactory<FakeBO2>();
            boTestFactory.SetValidValueGenerator(bo => bo.NonCompulsoryString,
                                                 typeof (ValidValueGeneratorIncrementalInt));

            IClassDef classDef = ClassDef.ClassDefs[type];

            IPropDef def = classDef.GetPropDef("NonCompulsoryString", false);

            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<ValidValueGeneratorIncrementalInt>(boTestFactory.GetValidValueGenerator(def));
            //---------------Execute Test ----------------------
            int firstValue = (int) boTestFactory.GetValidValueGenerator(def).GenerateValidValue();
            var secondValue = new BOTestFactory<FakeBO2>().GetValidValueGenerator(def).GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreEqual(firstValue + 1, secondValue);
        }
        
        [Test]
        public void
            TestAccept_CreateValidBusinessObject_WhenSetValidValueGenerator_WhenHasDefault_ShouldUseValueFromCustomGenerator
            ()
        {
            //---------------Set up test pack-------------------
            var type = typeof (FakeBOWithAllPropsMapped);
            ClassDef.ClassDefs.Add(type.MapClass());
            var boTestFactory = new BOTestFactory<FakeBOWithAllPropsMapped>();
            IClassDef classDef = ClassDef.ClassDefs[type];
            IPropDef def = classDef.GetPropDef("NonCompulsoryString", false);
            const string defaultValue = "1105";
            def.DefaultValue = defaultValue;
            boTestFactory.SetValidValueGenerator(bo => bo.NonCompulsoryString,
                                                 typeof (ValidValueGeneratorIncrementalInt));

            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<ValidValueGeneratorIncrementalInt>(boTestFactory.GetValidValueGenerator(def));
            //---------------Execute Test ----------------------
            var createdBO = boTestFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreNotEqual(defaultValue, createdBO.NonCompulsoryString);
        }
        
        [Test]
        public void Test_GetValidPropValue_WhenSetValidValueGenerator_ShouldUseSetValidValueGenerator()
        {
            //---------------Set up test pack-------------------
            var type = typeof (FakeBOWithAllPropsMapped);
            ClassDef.ClassDefs.Add(type.MapClass());
            var boTestFactory = new BOTestFactory<FakeBOWithAllPropsMapped>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTestFactory.SetValidValueGenerator(bo => bo.NonCompulsoryString, typeof (ValidValueGeneratorNameFake));
            var validPropValue = boTestFactory.GetValidPropValue(bo => bo.NonCompulsoryString);
            //---------------Test Result -----------------------
            StringAssert.StartsWith("Name", validPropValue);
        }

        [Test]
        public void Test_GetValidPropValue_WhenSetValidValueGenerator_ShouldUseSetValidValueGenerator_WithParameter()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOWithAllPropsMapped);
            ClassDef.ClassDefs.Add(type.MapClass());
            var boTestFactory = new BOTestFactory<FakeBOWithAllPropsMapped>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var parameter = RandomValueGen.GetRandomString();
            boTestFactory.SetValidValueGenerator(bo => bo.NonCompulsoryString, typeof(ValidValueGeneratorWithParameterWithFake), parameter);
            var validPropValue = boTestFactory.GetValidPropValue(bo => bo.NonCompulsoryString);
            //---------------Test Result -----------------------
            Assert.AreEqual("value" + parameter, validPropValue);
        }

        [Test]
        public void Test_CreateValidBusinessObject_WhenSetValidValueGenerator_ShouldUseSetValidValueGenerator()
        {
            //---------------Set up test pack-------------------
            var type = typeof (FakeBOWithAllPropsMapped);
            ClassDef.ClassDefs.Add(type.MapClass());
            var boTestFactory = new BOTestFactory<FakeBOWithAllPropsMapped>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTestFactory.SetValidValueGenerator(bo => bo.NonCompulsoryString, typeof (ValidValueGeneratorNameFake));
            var validBO = boTestFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            StringAssert.StartsWith("Name", validBO.NonCompulsoryString);
        }

        [Test]
        public void Test_CreateValidBusinessObject_WhenSetValidValueGenerator_ShouldUseSetValidValueGenerator_WithParameter()
        {
            //---------------Set up test pack-------------------
            var type = typeof(FakeBOWithAllPropsMapped);
            ClassDef.ClassDefs.Add(type.MapClass());
            var boTestFactory = new BOTestFactory<FakeBOWithAllPropsMapped>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var parameter = RandomValueGen.GetRandomString();
            boTestFactory.SetValidValueGenerator(bo => bo.NonCompulsoryString, typeof(ValidValueGeneratorWithParameterWithFake), parameter);
            var validBO = boTestFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreEqual("value" + parameter, validBO.NonCompulsoryString);
        }
       
        [Test]
        public void Test_CreateValidBusinessObject_WhenWithManyChildren_ShouldCreateManyChildrenforParent_FIXBUG1077()
        {
            //---------------Set up test pack-------------------
            var boTestFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var fakeParent = boTestFactory
                .WithMany(company => company.OtherRelatedFakeBos)
                .CreateValidBusinessObject();
            //---------------Test Result -----------------------

            Assert.AreEqual(MANY, fakeParent.OtherRelatedFakeBos.Count, "Many Children should have been created");
        }
        [Test]
        public void Test_CreateValidBusinessObjectAndSave_WhenWithManyChildren_ShouldCreateManyChildrenforParent_FIXBUG1077()
        {
            //---------------Set up test pack-------------------
            var boTestFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var fakeParent = boTestFactory
                .WithMany(company => company.OtherRelatedFakeBos)
                .CreateValidBusinessObject();
            fakeParent.Save();
            //---------------Test Result -----------------------

            Assert.AreEqual(MANY, fakeParent.OtherRelatedFakeBos.Count, "Many Children should have been created");
        }
        [Test]
        public void Test_CreateValidBusinessObject_Twice_WhenWithManyChildren_ShouldCreateManyChildrenforBothParents_FIXBUG1077()
        {
            //---------------Set up test pack-------------------
            var boTestFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            boTestFactory = boTestFactory.WithMany(company => company.OtherRelatedFakeBos);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var fakeParent1 = boTestFactory.CreateValidBusinessObject();
            var fakeParent2 = boTestFactory.CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreEqual(MANY, fakeParent1.OtherRelatedFakeBos.Count, "Many Children should have been created");
            Assert.AreEqual(MANY, fakeParent2.OtherRelatedFakeBos.Count, "Many Children should have been created");
        }

        [Ignore("Working on this bug")] //TODO Brett 02 Nov 2010: Ignored Test - REASON
        [Test]
        public void Test_CreateValidBusinessObjectAndSave_Twice_WhenWithManyChildren_ShouldCreateManyChildrenforBothParents_FIXBUG1077()
        {
            //---------------Set up test pack-------------------
            var boTestFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            boTestFactory = boTestFactory.WithMany(company => company.OtherRelatedFakeBos);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var fakeParent1 = boTestFactory.CreateValidBusinessObject();
            fakeParent1.Save();
            var fakeParent2 = boTestFactory.CreateValidBusinessObject();
            fakeParent2.Save();
            //---------------Test Result -----------------------
            Assert.AreEqual(MANY, fakeParent2.OtherRelatedFakeBos.Count, "FakeParent2: Many Children should have been created");
            Assert.AreEqual(MANY, fakeParent1.OtherRelatedFakeBos.Count, "FakeParent1: Many Children should have been created");
        }

        [Test]
        public void Test_CreateManyValidBusinesObjects_With1_WhenWithManyChildren_ShouldCreateManyChildrenforEachParent_FIXBUG1077()
        {
            //---------------Set up test pack-------------------
            var boTestFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var fakeParents = boTestFactory
                .WithMany(fakeParent1 => fakeParent1.OtherRelatedFakeBos)
                .CreateManySavedBusinessObject(1);
            //---------------Test Result -----------------------
            Assert.AreEqual(1, fakeParents.Count, "Many companies should have been created");
            var fakeParent = fakeParents[0];
            Assert.AreEqual(MANY, fakeParent.OtherRelatedFakeBos.Count, "Many Children should have been created");
        }

        [Ignore("Working on this bug")] //TODO Brett 02 Nov 2010: Ignored Test - REASON
        [Test]
        public void Test_CreateManyValidBusinesObjects_WhenWithManyChildren_ShouldCreateManyChildrenforEachParent_FIXBUG1077()
        {
            //---------------Set up test pack-------------------
            var boTestFactory = new BOTestFactory<FakeBOWithManyRelationship>();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var fakeParents = boTestFactory
                .WithMany(fakeParent1 => fakeParent1.OtherRelatedFakeBos)
                .CreateManySavedBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreEqual(MANY, fakeParents.Count, "Many companies should have been created");
            AssertAllParentsHaveManyChildren(fakeParents);
        }
        */

        [Test]
        public void Test_WithValuesForAllProps_ShouldConstructBoWithValuesForNonCompulsoryProps()
        {
            //---------------Set up test pack-------------------
            var boTestFactory = new BOTestFactory<FakeBOWithAllPropsMapped>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var fakeBO = boTestFactory
                .WithValueForAllProps()
                .CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNotNullOrEmpty(fakeBO.NonCompulsoryString, "Should have set value");
            Assert.IsNotNullOrEmpty(fakeBO.NonCompulsoryString2, "Should have set value");
        }

        /*No linq.Expression in CF
        [Test]
        public void Test_WithValuesForAllProps_WhenValueIsRegisterd_ShouldConstructWithRegisteredValue()
        {
            //---------------Set up test pack-------------------
            var boTestFactory = new BOTestFactory<FakeBOWithAllPropsMapped>();
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var expectedNonCompulsoryStringValue = GetRandomString();
            var fakeBO = boTestFactory
                .WithValueForAllProps()
                .WithValue(bo => bo.NonCompulsoryString, expectedNonCompulsoryStringValue )
                .CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNotNullOrEmpty(fakeBO.NonCompulsoryString2, "Should have set value");
        }
        [Test]
        public void Test_WithValuesForAllProps_WhenPropHasDefaultValue_ShouldConstructWithDefaultValue()
        {
            //---------------Set up test pack-------------------
            var boTestFactory = new BOTestFactory<FakeBOWithAllPropsMapped>();
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var expectedNonCompulsoryStringValue = GetRandomString();
            var fakeBO = boTestFactory
                .WithValueForAllProps()
                .WithValue(bo => bo.NonCompulsoryString, expectedNonCompulsoryStringValue )
                .CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreEqual("SomeDefaultValue", fakeBO.NonCompulsoryDefaultProp, "Should be set to Registerd Value");
            Assert.AreEqual(expectedNonCompulsoryStringValue, fakeBO.NonCompulsoryString, "Should have set value");
            Assert.IsNotNullOrEmpty(fakeBO.NonCompulsoryString2, "Should have set value");
        }
        [Test]
        public void Test_WithValuesForAllProps_WhenHasValueGeneratorRegistered_ShouldConstructWithValueFromRegisteredValueGen()
        {
            //---------------Set up test pack-------------------
            var boTestFactory = new BOTestFactory<FakeBOWithAllPropsMapped>();
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            boTestFactory.SetValidValueGenerator(bo => bo.NonCompulsoryString, typeof (ValidValueGeneratorFake));
            var fakeBO = boTestFactory
                .WithValueForAllProps()
                .CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.AreEqual("ValueFromValueValueGeneratorFake", fakeBO.NonCompulsoryString, "Should have set value");
            Assert.IsNotNullOrEmpty(fakeBO.NonCompulsoryString2, "Should have set value");
        }
         * */

        [Test]
        public void Test_WithValuesForAllProps_PropIsWriteNotNew_ShouldNotSetValueForProp()
        {
            //---------------Set up test pack-------------------
            var boTestFactory = new BOTestFactory<FakeBOWithAllPropsMapped>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var fakeBO = boTestFactory
                .WithValueForAllProps()
                .CreateValidBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNull(fakeBO.WriteNotNewProp, "Should not have value set");
        }

        private void AssertAllParentsHaveManyChildren(IList<FakeBOWithManyRelationship> fakeParents)
        {
            foreach (var fakeParent in fakeParents)
            {
                Assert.AreEqual(MANY, fakeParent.OtherRelatedFakeBos.Count, "Many Children should have been created");
            }
        }
    }

    // ReSharper restore InconsistentNaming
    public class ValidValueGeneratorNameFake : ValidValueGeneratorName
    {
        public ValidValueGeneratorNameFake(IPropDef propDef) : base(propDef, new List<string> {"Name1", "Name2"})
        {
        }
    }

    public class ValidValueGeneratorFake : ValidValueGenerator
    {
        public ValidValueGeneratorFake(ISingleValueDef singleValueDef) : base(singleValueDef)
        {
        }

        public override object GenerateValidValue()
        {
            return "ValueFromValueValueGeneratorFake";
        }
    }

    public class ValidValueGeneratorWithParameterWithFake : ValidValueGenerator
    {
        public string ExtraParameter { get; set; }

        public ValidValueGeneratorWithParameterWithFake(ISingleValueDef singleValueDef, string extraParameter)
            : base(singleValueDef)
        {
            ExtraParameter = extraParameter;
        }

        public override object GenerateValidValue()
        {
            if (ExtraParameter == null) return "value";
            return "value" + ExtraParameter;
        }
    }

    public class GenericBOTestFactorySpy<T> : BOTestFactory<T> where T : class, IBusinessObject
    {
        public BODefaultValueRegistry GetBODefaultValueRegistry()
        {
            return _defaultValueRegistry;
        }
    }
}