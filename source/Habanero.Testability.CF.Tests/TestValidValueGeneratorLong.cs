using System.Linq;
using Habanero.Base;
using Habanero.BO;
using Habanero.Testability.CF;
using Habanero.Testability.CF.Tests.Base;
using NUnit.Framework;

namespace Habanero.Testability.Tests
{
    [TestFixture]
    public class TestValidValueGeneratorLong
    {
        private static PropRuleLong CreatePropRuleLong(int min, int max)
        {
            return new PropRuleLong(RandomValueGen.GetRandomString(), RandomValueGen.GetRandomString(), min, max);
        }

        [Test]
        public void Test_GenerateValue_WhenInt_ShouldRetInt()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(long)
                                           };
            ValidValueGenerator valueGenerator = new ValidValueGeneratorLong(def);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            object value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(long), value);
            Assert.AreNotEqual(valueGenerator.GenerateValidValue(), value);
        }

        [Test]
        public void Test_GenerateValue_WhenIntAndRule_ShouldRetValidValue()
        {            
            //---------------Set up test pack-------------------

            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(long)
                                           };
            def.AddPropRule(CreatePropRuleLong(3, 7));
            ValidValueGenerator generator = new ValidValueGeneratorLong(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(long), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleLong>().ToList());
            PropRuleLong propRule = def.PropRules.OfType<PropRuleLong>().First();
            Assert.AreEqual(3, propRule.MinValue);
            Assert.AreEqual(7, propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (long)generator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3);
            Assert.LessOrEqual(value, 7);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenIntAndNoRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(long)
                                           };
            ValidValueGeneratorLong generator = new ValidValueGeneratorLong(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(long), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleLong>().ToList());
            //---------------Execute Test ----------------------
            var value = (long)generator.GenerateValidValueGreaterThan(int.MaxValue - 5);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, int.MaxValue - 5);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenIntAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(long)
                                           };
            const int minValue = 3;
            const int maxValue = 20;
            def.AddPropRule(CreatePropRuleLong(minValue, maxValue));
            ValidValueGeneratorLong generator = new ValidValueGeneratorLong(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(long), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleLong>().ToList());
            PropRuleLong propRule = def.PropRules.OfType<PropRuleLong>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (long)generator.GenerateValidValueGreaterThan(null);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, minValue);
            Assert.LessOrEqual(value,maxValue);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenIntAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(long)
                                           };
            const int maxValue = int.MaxValue - 77;
            const int minValue = 3;
            const int greaterThanValue = int.MaxValue - 88;
            def.AddPropRule(CreatePropRuleLong(minValue,maxValue));
            ValidValueGeneratorLong generator = new ValidValueGeneratorLong(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(long), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleLong>().ToList());
            PropRuleLong propRule = def.PropRules.OfType<PropRuleLong>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (long)generator.GenerateValidValueGreaterThan(greaterThanValue);
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
                                               PropertyType = typeof(long)
                                           };
            const int minValue = 3;
            const int maxValue = 8;
            def.AddPropRule(CreatePropRuleLong(minValue, maxValue));
            const int greaterThanValue = int.MinValue + 10;
            ValidValueGeneratorLong generator = new ValidValueGeneratorLong(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(long), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleLong>().ToList());
            PropRuleLong propRule = def.PropRules.OfType<PropRuleLong>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (long)generator.GenerateValidValueGreaterThan(greaterThanValue);
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
                                               PropertyType = typeof(long)
                                           };
            ValidValueGeneratorLong generator = new ValidValueGeneratorLong(def);
            const long lessThanValue = long.MinValue + 10;
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(long), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleLong>().ToList());
            //---------------Execute Test ----------------------
            var value = (long)generator.GenerateValidValueLessThan(lessThanValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, long.MinValue);
            Assert.LessOrEqual(value, lessThanValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenIntAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(long)
                                           };
            const int maxValue = int.MaxValue - 77;
            const int minValue = 3;
            def.AddPropRule(CreatePropRuleLong(minValue,maxValue));
            ValidValueGeneratorLong generator = new ValidValueGeneratorLong(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(long), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleLong>().ToList());
            PropRuleLong propRule = def.PropRules.OfType<PropRuleLong>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            long value = (long)generator.GenerateValidValueLessThan(null);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, minValue);
            Assert.LessOrEqual(value, maxValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenIntAndRule_RuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                                               PropertyType = typeof(long)
                                           };
            const int maxValue = int.MaxValue - 77;
            const int minValue = 3;
            const int lessThanValue = 5;
            def.AddPropRule(CreatePropRuleLong(minValue,maxValue));
            ValidValueGeneratorLong generator = new ValidValueGeneratorLong(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(long), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleLong>().ToList());
            PropRuleLong propRule = def.PropRules.OfType<PropRuleLong>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            long value = (long)generator.GenerateValidValueLessThan(lessThanValue);
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
                                               PropertyType = typeof(long)
                                           };
            const int minValue = 3;
            const int maxValue = 5;
            const int lessThanValue = int.MaxValue - 77;

            def.AddPropRule(CreatePropRuleLong(minValue, maxValue));
            ValidValueGeneratorLong generator = new ValidValueGeneratorLong(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(long), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleLong>().ToList());
            PropRuleLong propRule = def.PropRules.OfType<PropRuleLong>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            long value = (long)generator.GenerateValidValueLessThan(lessThanValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, minValue);
            Assert.LessOrEqual(value, lessThanValue);
            Assert.LessOrEqual(value, maxValue);
        }
    }
}