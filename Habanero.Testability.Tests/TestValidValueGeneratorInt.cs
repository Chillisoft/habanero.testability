namespace Habanero.Testability.Tests
{
    using Habanero.Base;
    using Habanero.BO;
    using Habanero.Testability;
    using NUnit.Framework;
    using System;
    using System.Linq;

    [TestFixture]
    public class TestValidValueGeneratorInt
    {
        private static PropRuleInteger CreatePropRuleInt(int min, int max)
        {
            return new PropRuleInteger(RandomValueGen.GetRandomString(), RandomValueGen.GetRandomString(), min, max);
        }

        [Test]
        public void Test_GenerateValue_WhenInt_ShouldRetInt()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            ValidValueGenerator valueGenerator = new ValidValueGeneratorInt(def);
            object value = valueGenerator.GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(int), value);
            Assert.AreNotEqual(valueGenerator.GenerateValidValue(), value);
        }

        [Test]
        public void Test_GenerateValue_WhenIntAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            def.AddPropRule(CreatePropRuleInt(3, 7));
            ValidValueGenerator generator = new ValidValueGeneratorInt(def);
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList<PropRuleInteger>());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First<PropRuleInteger>();
            Assert.AreEqual(3, propRule.MinValue);
            Assert.AreEqual(7, propRule.MaxValue);
            int value = (int) generator.GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3);
            Assert.LessOrEqual(value, 7);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenIntAndNoRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleInteger>().ToList<PropRuleInteger>());
            int value = (int) generator.GenerateValidValueGreaterThan(0x7ffffff6);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 0x7ffffff6);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenIntAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            def.AddPropRule(CreatePropRuleInt(3, 0x7ffffffa));
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList<PropRuleInteger>());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First<PropRuleInteger>();
            Assert.AreEqual(3, propRule.MinValue);
            Assert.AreEqual(0x7ffffffa, propRule.MaxValue);
            int value = (int) generator.GenerateValidValueGreaterThan(null);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3);
            Assert.LessOrEqual(value, 0x7ffffffa);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenIntAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            def.AddPropRule(CreatePropRuleInt(3, 0x7ffffffa));
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList<PropRuleInteger>());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First<PropRuleInteger>();
            Assert.AreEqual(3, propRule.MinValue);
            Assert.AreEqual(0x7ffffffa, propRule.MaxValue);
            int value = (int) generator.GenerateValidValueGreaterThan(0x7ffffff1);
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value, 0x7ffffffa);
            Assert.GreaterOrEqual(value, 3);
            Assert.GreaterOrEqual(value, 0x7ffffff1);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenIntAndRule_WhenRuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            def.AddPropRule(CreatePropRuleInt(3, 8));
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList<PropRuleInteger>());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First<PropRuleInteger>();
            Assert.AreEqual(3, propRule.MinValue);
            Assert.AreEqual(8, propRule.MaxValue);
            int value = (int) generator.GenerateValidValueGreaterThan(-2147483639);
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value, 8);
            Assert.GreaterOrEqual(value, 3);
            Assert.GreaterOrEqual(value, -2147483639);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenIntAndNoRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleInteger>().ToList<PropRuleInteger>());
            int value = (int) generator.GenerateValidValueLessThan(-2147483641);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, -2147483648);
            Assert.LessOrEqual(value, -2147483641);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenIntAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            def.AddPropRule(CreatePropRuleInt(3, 0x7ffffffa));
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList<PropRuleInteger>());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First<PropRuleInteger>();
            Assert.AreEqual(3, propRule.MinValue);
            Assert.AreEqual(0x7ffffffa, propRule.MaxValue);
            int value = (int) generator.GenerateValidValueLessThan(null);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3);
            Assert.LessOrEqual(value, 0x7ffffffa);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenIntAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            def.AddPropRule(CreatePropRuleInt(3, 0x7ffffffa));
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList<PropRuleInteger>());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First<PropRuleInteger>();
            Assert.AreEqual(3, propRule.MinValue);
            Assert.AreEqual(0x7ffffffa, propRule.MaxValue);
            int value = (int) generator.GenerateValidValueLessThan(5);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3);
            Assert.LessOrEqual(value, 5);
            Assert.LessOrEqual(value, 0x7ffffffa);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenIntAndRule_WhenRuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            def.AddPropRule(CreatePropRuleInt(3, 5));
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList<PropRuleInteger>());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First<PropRuleInteger>();
            Assert.AreEqual(3, propRule.MinValue);
            Assert.AreEqual(5, propRule.MaxValue);
            int value = (int) generator.GenerateValidValueLessThan(0x7ffffff5);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3);
            Assert.LessOrEqual(value, 0x7ffffff5);
            Assert.LessOrEqual(value, 5);
        }
    }
}

