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
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            object value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(int), value);
            Assert.AreNotEqual(valueGenerator.GenerateValidValue(), value);
        }

        [Test]
        public void Test_GenerateValue_WhenIntAndRule_ShouldRetValidValue()
        {            
            //---------------Set up test pack-------------------

            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            def.AddPropRule(CreatePropRuleInt(3, 7));
            ValidValueGenerator generator = new ValidValueGeneratorInt(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First();
            Assert.AreEqual(3, propRule.MinValue);
            Assert.AreEqual(7, propRule.MaxValue);
            int value = (int)generator.GenerateValidValue();
            //---------------Test Result -----------------------
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
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleInteger>().ToList());
            //---------------Execute Test ----------------------
            int value = (int)generator.GenerateValidValueGreaterThan(int.MaxValue - 5);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, int.MaxValue - 5);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenIntAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            const int minValue = 3;
            const int maxValue = 20;
            def.AddPropRule(CreatePropRuleInt(minValue, maxValue));
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            int value = (int)generator.GenerateValidValueGreaterThan(null);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, minValue);
            Assert.LessOrEqual(value,maxValue);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenIntAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            const int maxValue = int.MaxValue - 77;
            const int minValue = 3;
            const int greaterThanValue = int.MaxValue - 88;
            def.AddPropRule(CreatePropRuleInt(minValue,maxValue));
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            int value = (int)generator.GenerateValidValueGreaterThan(greaterThanValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value,maxValue);
            Assert.GreaterOrEqual(value, minValue);
            Assert.GreaterOrEqual(value, greaterThanValue);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenIntAndRule_WhenRuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            const int minValue = 3;
            const int maxValue = 8;
            def.AddPropRule(CreatePropRuleInt(minValue, maxValue));
            const int greaterThanValue = int.MinValue + 10;
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            int value = (int)generator.GenerateValidValueGreaterThan(greaterThanValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value, maxValue);
            Assert.GreaterOrEqual(value, minValue);
            Assert.GreaterOrEqual(value, greaterThanValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenIntAndNoRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            const int lessThanValue = int.MinValue + 10;
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleInteger>().ToList());
            //---------------Execute Test ----------------------
            int value = (int)generator.GenerateValidValueLessThan(lessThanValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, int.MinValue);
            Assert.LessOrEqual(value, lessThanValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenIntAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            const int maxValue = int.MaxValue - 77;
            const int minValue = 3;
            def.AddPropRule(CreatePropRuleInt(minValue,maxValue));
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            int value = (int)generator.GenerateValidValueLessThan(null);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, minValue);
            Assert.LessOrEqual(value,maxValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenIntAndRule_RuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            const int maxValue = int.MaxValue - 77;
            const int minValue = 3;
            const int lessThanValue = 5;
            def.AddPropRule(CreatePropRuleInt(minValue,maxValue));
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            int value = (int)generator.GenerateValidValueLessThan(lessThanValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, minValue);
            Assert.LessOrEqual(value, lessThanValue);
            Assert.LessOrEqual(value,maxValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenIntAndRule_WhenRuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            const int minValue = 3;
            const int maxValue = 5;
            const int lessThanValue = int.MaxValue - 77;

            def.AddPropRule(CreatePropRuleInt(minValue, maxValue));
            ValidValueGeneratorInt generator = new ValidValueGeneratorInt(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            int value = (int)generator.GenerateValidValueLessThan(lessThanValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, minValue);
            Assert.LessOrEqual(value, lessThanValue);
            Assert.LessOrEqual(value, maxValue);
        }
    }
}

