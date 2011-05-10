using Habanero.Testability.CF;
using Habanero.Testability.CF.Tests.Base;

namespace Habanero.Testability.Tests
{
    using Habanero.Base;
    using Habanero.BO;
    using Habanero.Testability;
    using NUnit.Framework;
    using System;
    using System.Linq;

    [TestFixture]
    public class TestValidValueGeneratorDecimal
    {
        private static PropRuleDecimal CreatePropRuleDecimal(decimal min, decimal max)
        {
            return new PropRuleDecimal(RandomValueGen.GetRandomString(), RandomValueGen.GetRandomString(), min, max);
        }

        [Test]
        public void Test_GenerateValue_WhenDecimal_ShouldSet()
        {
            Type propertyType = typeof(decimal);
            IPropDef def = new PropDefFake {
                PropertyType = propertyType
            };
            ValidValueGenerator valueGenerator = new ValidValueGeneratorDecimal(def);
            object value = valueGenerator.GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(decimal), value);
            Assert.AreNotEqual(valueGenerator.GenerateValidValue(), value);
        }

        [Test]
        public void Test_GenerateValue_WhenDecimapAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(decimal)
            };
            def.AddPropRule(CreatePropRuleDecimal(3.01M, 7.0004M));
            ValidValueGenerator generator = new ValidValueGeneratorDecimal(def);
            Assert.AreSame(typeof(decimal), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDecimal>().ToList());
            PropRuleDecimal propRule = def.PropRules.OfType<PropRuleDecimal>().First();
            Assert.AreEqual(3.01M, propRule.MinValue);
            Assert.AreEqual(7.0004M, propRule.MaxValue);
            decimal value = (decimal) generator.GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3.01M);
            Assert.LessOrEqual(value, 7.0004M);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDecimalAndNoRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(decimal)
            };
            ValidValueGeneratorDecimal generator = new ValidValueGeneratorDecimal(def);
            Assert.AreSame(typeof(decimal), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleDecimal>().ToList());
            decimal value = (decimal) generator.GenerateValidValueGreaterThan(decimal.MaxValue - 5);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, decimal.MaxValue - 5);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDecimalAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(decimal)
            };
            def.AddPropRule(CreatePropRuleDecimal(3M, decimal.MaxValue - 7));
            ValidValueGeneratorDecimal generator = new ValidValueGeneratorDecimal(def);
            Assert.AreSame(typeof(decimal), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDecimal>().ToList());
            PropRuleDecimal propRule = def.PropRules.OfType<PropRuleDecimal>().First();
            Assert.AreEqual(3M, propRule.MinValue);
            Assert.AreEqual(decimal.MaxValue - 7, propRule.MaxValue);
            decimal value = (decimal) generator.GenerateValidValueGreaterThan(null);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3M);
            Assert.LessOrEqual(value, decimal.MaxValue - 7);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDecimalAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(decimal)
            };
            def.AddPropRule(CreatePropRuleDecimal(3M, decimal.MaxValue - 7));
            ValidValueGeneratorDecimal generator = new ValidValueGeneratorDecimal(def);
            Assert.AreSame(typeof(decimal), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDecimal>().ToList());
            PropRuleDecimal propRule = def.PropRules.OfType<PropRuleDecimal>().First();
            Assert.AreEqual(3M, propRule.MinValue);
            Assert.AreEqual(decimal.MaxValue - 7, propRule.MaxValue);
            decimal value = (decimal) generator.GenerateValidValueGreaterThan(decimal.MaxValue - 10);
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value, decimal.MaxValue - 7);
            Assert.GreaterOrEqual(value, decimal.MaxValue - 10);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDecimalAndRule_WhenRuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(decimal)
            };
            def.AddPropRule(CreatePropRuleDecimal(3M, 7M));
            ValidValueGeneratorDecimal generator = new ValidValueGeneratorDecimal(def);
            Assert.AreSame(typeof(decimal), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDecimal>().ToList());
            PropRuleDecimal propRule = def.PropRules.OfType<PropRuleDecimal>().First();
            Assert.AreEqual(3M, propRule.MinValue);
            Assert.AreEqual(7M, propRule.MaxValue);
            decimal value = (decimal) generator.GenerateValidValueGreaterThan(decimal.MinValue + 5);
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value, 7M);
            Assert.GreaterOrEqual(value, decimal.MinValue + 5);
            Assert.GreaterOrEqual(value, 3M);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDecimalAndNoRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(decimal)
            };
            ValidValueGeneratorDecimal generator = new ValidValueGeneratorDecimal(def);
            Assert.AreSame(typeof(decimal), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleDecimal>().ToList());
            decimal value = (decimal) generator.GenerateValidValueLessThan(decimal.MinValue + 5);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, decimal.MinValue );
            Assert.LessOrEqual(value, decimal.MinValue + 5);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDecimalAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(decimal)
            };
            def.AddPropRule(CreatePropRuleDecimal(3M, decimal.MaxValue - 7));
            ValidValueGeneratorDecimal generator = new ValidValueGeneratorDecimal(def);
            Assert.AreSame(typeof(decimal), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDecimal>().ToList());
            PropRuleDecimal propRule = def.PropRules.OfType<PropRuleDecimal>().First();
            Assert.AreEqual(3M, propRule.MinValue);
            Assert.AreEqual(decimal.MaxValue - 7, propRule.MaxValue);
            decimal value = (decimal) generator.GenerateValidValueLessThan(null);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3M);
            Assert.LessOrEqual(value, decimal.MaxValue - 7);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDecimalAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(decimal)
            };
            def.AddPropRule(CreatePropRuleDecimal(3M, decimal.MaxValue - 7));
            ValidValueGeneratorDecimal generator = new ValidValueGeneratorDecimal(def);
            Assert.AreSame(typeof(decimal), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDecimal>().ToList());
            PropRuleDecimal propRule = def.PropRules.OfType<PropRuleDecimal>().First();
            Assert.AreEqual(3M, propRule.MinValue);
            Assert.AreEqual(decimal.MaxValue - 7, propRule.MaxValue);
            decimal value = (decimal) generator.GenerateValidValueLessThan(5M);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3M);
            Assert.LessOrEqual(value, 5M);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDecimalAndRule_WhenRuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(decimal)
            };
            def.AddPropRule(CreatePropRuleDecimal(3M, 7M));
            ValidValueGeneratorDecimal generator = new ValidValueGeneratorDecimal(def);
            Assert.AreSame(typeof(decimal), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDecimal>().ToList());
            PropRuleDecimal propRule = def.PropRules.OfType<PropRuleDecimal>().First();
            Assert.AreEqual(3M, propRule.MinValue);
            Assert.AreEqual(7M, propRule.MaxValue);
            decimal value = (decimal) generator.GenerateValidValueLessThan(decimal.MaxValue - 5);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3M);
            Assert.LessOrEqual(value, decimal.MaxValue - 5);
            Assert.LessOrEqual(value, 7M);
        }
    }
}

