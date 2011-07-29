using Habanero.Testability.Tests.Base;

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
            //---------------Set up test pack-------------------
            Type propertyType = typeof(DateTime);
            IPropDef def = new PropDefFake {
                PropertyType = propertyType
            };
            ValidValueGenerator valueGenerator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            object value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(DateTime), value);
            Assert.AreNotEqual(valueGenerator.GenerateValidValue(), value);
        }

        [Test]
        public void Test_GenerateValue_WhenDateTimeAndRule_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.MinValue.AddDays(5555.0);
            DateTime max = DateTime.MinValue.AddDays(5565.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGenerator generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max, propRule.MaxValue.Date);
            //---------------Execute Test ----------------------
            DateTime value = (DateTime)generator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, min);
            Assert.LessOrEqual(value, max);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDateTimeAndNoRule_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            DateTime overridingMinValue = DateTime.MaxValue.AddDays(-9.0);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            //---------------Execute Test ----------------------
            DateTime value = (DateTime)generator.GenerateValidValueGreaterThan(overridingMinValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, overridingMinValue);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDateTimeAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.Now;
            DateTime max = DateTime.MaxValue.AddDays(-5.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            //---------------Execute Test ----------------------
            DateTime value = (DateTime)generator.GenerateValidValueGreaterThan(null);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, min);
            Assert.LessOrEqual(value, max);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDateTimeAndRule_RuleMoreRestrictive_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.Now;
            DateTime max = DateTime.Now.AddDays(5.0);
            DateTime overridingMinValue = DateTime.MinValue.AddDays(9.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            //---------------Execute Test ----------------------
            DateTime value = (DateTime)generator.GenerateValidValueGreaterThan(overridingMinValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value, max);
            Assert.GreaterOrEqual(value, overridingMinValue);
            Assert.GreaterOrEqual(value, min);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDateTimeAndRule_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.Now;
            DateTime max = DateTime.MaxValue.AddDays(-5.0);
            DateTime overridingMinValue = max.AddDays(-9.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            //---------------Execute Test ----------------------
            DateTime value = (DateTime)generator.GenerateValidValueGreaterThan(overridingMinValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value, max);
            Assert.GreaterOrEqual(value, overridingMinValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDateTimeAndNoRule_ShouldRetValidValue()
        {
            // Redmine #1745
            // Changed from using DateTime.MinValue as Sql Server can't handle this
            // Sql Server min date is 1/1/1753

            //---------------Set up test pack-------------------
            var expectedAbsoluteMin = new DateTime(1753, 1, 1);
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            DateTime overridingMaxValue = expectedAbsoluteMin.AddDays(7.0);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            //---------------Execute Test ----------------------
            DateTime value = (DateTime)generator.GenerateValidValueLessThan(overridingMaxValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, expectedAbsoluteMin);
            Assert.LessOrEqual(value, overridingMaxValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDateTimeAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.Now;
            DateTime max = DateTime.MaxValue.AddDays(-5.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            //---------------Execute Test ----------------------
            DateTime value = (DateTime)generator.GenerateValidValueLessThan(null);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, min);
            Assert.LessOrEqual(value, max);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDateTimeAndRule_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.Now;
            DateTime max = DateTime.MaxValue.AddDays(-5.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            DateTime overridingMaxValue = DateTime.Now.AddDays(5.0);
            //---------------Execute Test ----------------------
            DateTime value = (DateTime)generator.GenerateValidValueLessThan(overridingMaxValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, min);
            Assert.LessOrEqual(value, overridingMaxValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDateTimeAndRule_WhenRuleMoreRestrictive_ShouldRetValidValue()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake {
                PropertyType = typeof(DateTime)
            };
            DateTime min = DateTime.Now;
            DateTime max = DateTime.Today.AddDays(5.0);
            DateTime overridingMaxValue = DateTime.MaxValue.AddDays(-5.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGeneratorDate generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            //---------------Execute Test ----------------------
            DateTime value = (DateTime)generator.GenerateValidValueLessThan(overridingMaxValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, min);
            Assert.LessOrEqual(value, max);
            Assert.LessOrEqual(value, overridingMaxValue);
        }
    }
}

