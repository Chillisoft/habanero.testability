namespace Habanero.Testability.Tests
{
    using Habanero.Base;
    using Habanero.BO;
    using Habanero.Testability;
    using NUnit.Framework;
    using System;
    using System.Linq;

    [TestFixture]
    public class TestValidValueGeneratorDate
    {
        private static PropRuleDate CreatePropRuleDate(DateTime min, DateTime max)
        {
            return new PropRuleDate(RandomValueGen.GetRandomString(), RandomValueGen.GetRandomString(), min, max);
        }

        [Test]
        public void Test_GenerateValue_WhenDateTime_ShouldSet()
        {
            Type propertyType = typeof(DateTime);
            IPropDef def = new PropDefFake {
                PropertyType = propertyType
            };
            ValidValueGenerator valueGenerator = new ValidValueGeneratorDate(def);
            object value = valueGenerator.GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(DateTime), value);
            Assert.AreNotEqual(valueGenerator.GenerateValidValue(), value);
        }

        [Test]
        public void Test_GenerateValue_WhenDateTimeAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.MinValue.AddDays(5555.0);
            DateTime max = DateTime.MinValue.AddDays(5565.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGenerator generator = new ValidValueGeneratorDate(def);
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList<PropRuleDate>());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First<PropRuleDate>();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max, propRule.MaxValue.Date);
            DateTime value = (DateTime) generator.GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, min);
            Assert.LessOrEqual(value, max);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDateTimeAndNoRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            DateTime overridingMinValue = DateTime.MaxValue.AddDays(-9.0);
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleDate>().ToList<PropRuleDate>());
            DateTime value = (DateTime) generator.GenerateValidValueGreaterThan(overridingMinValue);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, overridingMinValue);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDateTimeAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.Now;
            DateTime max = DateTime.MaxValue.AddDays(-5.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList<PropRuleDate>());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First<PropRuleDate>();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            DateTime value = (DateTime) generator.GenerateValidValueGreaterThan(null);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, min);
            Assert.LessOrEqual(value, max);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDateTimeAndRule_RuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.Now;
            DateTime max = DateTime.Now.AddDays(5.0);
            DateTime overridingMinValue = DateTime.MinValue.AddDays(9.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList<PropRuleDate>());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First<PropRuleDate>();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            DateTime value = (DateTime) generator.GenerateValidValueGreaterThan(overridingMinValue);
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value, max);
            Assert.GreaterOrEqual(value, overridingMinValue);
            Assert.GreaterOrEqual(value, min);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDateTimeAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.Now;
            DateTime max = DateTime.MaxValue.AddDays(-5.0);
            DateTime overridingMinValue = max.AddDays(-9.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList<PropRuleDate>());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First<PropRuleDate>();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            DateTime value = (DateTime) generator.GenerateValidValueGreaterThan(overridingMinValue);
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value, max);
            Assert.GreaterOrEqual(value, overridingMinValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDateTimeAndNoRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            DateTime overridingMaxValue = DateTime.MinValue.AddDays(7.0);
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleDate>().ToList<PropRuleDate>());
            DateTime value = (DateTime) generator.GenerateValidValueLessThan(overridingMaxValue);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, DateTime.MinValue);
            Assert.LessOrEqual(value, overridingMaxValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDateTimeAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.Now;
            DateTime max = DateTime.MaxValue.AddDays(-5.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList<PropRuleDate>());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First<PropRuleDate>();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            DateTime value = (DateTime) generator.GenerateValidValueLessThan(null);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, min);
            Assert.LessOrEqual(value, max);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDateTimeAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.Now;
            DateTime max = DateTime.MaxValue.AddDays(-5.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList<PropRuleDate>());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First<PropRuleDate>();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            DateTime overridingMaxValue = DateTime.Now.AddDays(5.0);
            DateTime value = (DateTime) generator.GenerateValidValueLessThan(overridingMaxValue);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, min);
            Assert.LessOrEqual(value, overridingMaxValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDateTimeAndRule_WhenRuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.Now;
            DateTime max = DateTime.Today.AddDays(5.0);
            DateTime overridingMaxValue = DateTime.MaxValue.AddDays(-5.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList<PropRuleDate>());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First<PropRuleDate>();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            DateTime value = (DateTime) generator.GenerateValidValueLessThan(overridingMaxValue);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, min);
            Assert.LessOrEqual(value, max);
            Assert.LessOrEqual(value, overridingMaxValue);
        }
    }
}

