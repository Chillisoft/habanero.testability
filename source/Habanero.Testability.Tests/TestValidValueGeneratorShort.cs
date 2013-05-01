using System;
using System.Linq;
using Habanero.BO;
using Habanero.Base;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;
// ReSharper disable InconsistentNaming
namespace Habanero.Testability.Tests
{
    [TestFixture]
    public class TestValidValueGeneratorShort
    {
        private static PropRuleShort CreatePropRuleShort(short min, short max)
        {
            return new PropRuleShort(RandomValueGen.GetRandomString(), RandomValueGen.GetRandomString(), min, max);
        }

        [Test]
        public void Test_GenerateValue_WhenShort_ShouldRetShort()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(short)
                                           };
            var valueGenerator = CreateValidValueGenerator(def);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(short), value);
            Assert.AreNotEqual(valueGenerator.GenerateValidValue(), value);
        }

        [Test]
        public void Test_GenerateValue_WhenShortAndRule_ShouldRetValidValue()
        {            
            //---------------Set up test pack-------------------

            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(short)
                                           };
            def.AddPropRule(CreatePropRuleShort(3, 7));
            var generator = CreateValidValueGenerator(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(short), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleShort>().ToList());
            var propRule = def.PropRules.OfType<PropRuleShort>().First();
            Assert.AreEqual(3, propRule.MinValue);
            Assert.AreEqual(7, propRule.MaxValue);
            var value = (short)generator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3);
            Assert.LessOrEqual(value, 7);
        }

        private static ValidValueGeneratorShort CreateValidValueGenerator(IPropDef def)
        {
            return new ValidValueGeneratorShort(def);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenShortAndNoRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(short)
                                           };
            var generator = CreateValidValueGenerator(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(short), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleShort>().ToList());
            //---------------Execute Test ----------------------
            var value = (short)generator.GenerateValidValueGreaterThan(Convert.ToInt16(short.MaxValue - 5));
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, short.MaxValue - 5);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenShortAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(short)
                                           };
            const short minValue = 3;
            const short maxValue = 20;
            def.AddPropRule(CreatePropRuleShort(minValue, maxValue));
            var generator = CreateValidValueGenerator(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(short), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleShort>().ToList());
            PropRuleShort propRule = def.PropRules.OfType<PropRuleShort>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (short)generator.GenerateValidValueGreaterThan(null);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, minValue);
            Assert.LessOrEqual(value,maxValue);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenShortAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(short)
                                           };
            const short maxValue = short.MaxValue - 77;
            const short minValue = 3;
            const short greaterThanValue = short.MaxValue - 88;
            def.AddPropRule(CreatePropRuleShort(minValue,maxValue));
            var generator = CreateValidValueGenerator(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(short), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleShort>().ToList());
            PropRuleShort propRule = def.PropRules.OfType<PropRuleShort>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (short)generator.GenerateValidValueGreaterThan(greaterThanValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value,maxValue);
            Assert.GreaterOrEqual(value, minValue);
            Assert.GreaterOrEqual(value, greaterThanValue);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenShortAndRule_WhenRuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(short)
                                           };
            const short minValue = 3;
            const short maxValue = 8;
            def.AddPropRule(CreatePropRuleShort(minValue, maxValue));
            const short greaterThanValue = short.MinValue + 10;
            var generator = CreateValidValueGenerator(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(short), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleShort>().ToList());
            PropRuleShort propRule = def.PropRules.OfType<PropRuleShort>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (short)generator.GenerateValidValueGreaterThan(greaterThanValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value, maxValue);
            Assert.GreaterOrEqual(value, minValue);
            Assert.GreaterOrEqual(value, greaterThanValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenShortAndNoRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(short)
                                           };
            var generator = CreateValidValueGenerator(def);
            const short lessThanValue = short.MinValue + 10;
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(short), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleShort>().ToList());
            //---------------Execute Test ----------------------
            var value = (short)generator.GenerateValidValueLessThan(lessThanValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, short.MinValue);
            Assert.LessOrEqual(value, lessThanValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenShortAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(short)
                                           };
            const short maxValue = short.MaxValue - 77;
            const short minValue = 3;
            def.AddPropRule(CreatePropRuleShort(minValue,maxValue));
            var generator = CreateValidValueGenerator(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(short), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleShort>().ToList());
            PropRuleShort propRule = def.PropRules.OfType<PropRuleShort>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (short)generator.GenerateValidValueLessThan(null);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, minValue);
            Assert.LessOrEqual(value,maxValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenShortAndRule_RuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(short)
                                           };
            const short maxValue = short.MaxValue - 77;
            const short minValue = 3;
            const short lessThanValue = 5;
            def.AddPropRule(CreatePropRuleShort(minValue,maxValue));
            var generator = CreateValidValueGenerator(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(short), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleShort>().ToList());
            PropRuleShort propRule = def.PropRules.OfType<PropRuleShort>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (short)generator.GenerateValidValueLessThan(lessThanValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, minValue);
            Assert.LessOrEqual(value, lessThanValue);
            Assert.LessOrEqual(value,maxValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenShortAndRule_WhenRuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(short)
                                           };
            const short minValue = 3;
            const short maxValue = 5;
            const short lessThanValue = short.MaxValue - 77;

            def.AddPropRule(CreatePropRuleShort(minValue, maxValue));
            var generator = CreateValidValueGenerator(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(short), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleShort>().ToList());
            var propRule = def.PropRules.OfType<PropRuleShort>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (short)generator.GenerateValidValueLessThan(lessThanValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, minValue);
            Assert.LessOrEqual(value, lessThanValue);
            Assert.LessOrEqual(value, maxValue);
        }
    }
}