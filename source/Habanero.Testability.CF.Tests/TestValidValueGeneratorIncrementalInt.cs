using Habanero.Testability;
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
    public class TestValidValueGeneratorIncrementalInt
    {
        // ReSharper disable InconsistentNaming
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
            ValidValueGenerator valueGenerator = new ValidValueGeneratorIncrementalInt(def);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            object value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(int), value);
            Assert.AreNotEqual(valueGenerator.GenerateValidValue(), value);
        }

        [Test]
        public void Test_GenerateValue_WhenInt_WhenNoRule_WhenFirstTime_ShouldRetZero()
        {
            IPropDef def = new PropDefFake
            {
                PropertyType = typeof(int)
            };
            ValidValueGenerator valueGenerator = new ValidValueGeneratorIncrementalInt(def);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            object value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(int), value);
            Assert.AreEqual(0, value);
        }

        [Test]
        public void Test_GenerateValue_WhenSecondTime_WhenConstructedAgain_WhenNoRule_ShouldRetOne()
        {
            IPropDef def = new PropDefFake
            {
                PropertyType = typeof(int)
            };
            ValidValueGenerator valueGenerator = new ValidValueGeneratorIncrementalInt(def);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            object firstValue = valueGenerator.GenerateValidValue();
            object secondValue = new ValidValueGeneratorIncrementalInt(def).GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(secondValue);
            Assert.IsInstanceOf(typeof(int), secondValue);
            Assert.AreEqual(1, secondValue);
            Assert.AreNotEqual(firstValue, secondValue);
        }

        [Test]
        public void Test_GenerateValue_WhenSecondTime_WhenNoRule_ShouldRetOne()
        {
            IPropDef def = new PropDefFake
            {
                PropertyType = typeof(int)
            };
            ValidValueGenerator valueGenerator = new ValidValueGeneratorIncrementalInt(def);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            object firstValue = valueGenerator.GenerateValidValue();
            object secondValue = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(secondValue);
            Assert.IsInstanceOf(typeof(int), secondValue);
            Assert.AreEqual(1, secondValue);
            Assert.AreNotEqual(firstValue, secondValue);
        }

        [Test]
        public void Test_GenerateValue_WhenSecondTime_WhenNoRule_WhenDiffPropDef_ShouldRetZero()
        {
            //Each Prop Def should have its own incremental value. Otherwise would get continued reseting with
            // each having a different Set of rules (i.e Min and Max).
            IPropDef def1 = new PropDefFake
            {
                PropertyType = typeof(int)
            };
            IPropDef def2 = new PropDefFake
            {
                PropertyType = typeof(int)
            };
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            new ValidValueGeneratorIncrementalInt(def1).GenerateValidValue();
            object secondValue = new ValidValueGeneratorIncrementalInt(def2).GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf(typeof(int), secondValue);
            Assert.AreEqual(0, secondValue);
        }

        [Test]
        public void Test_GenerateValue_WhenIntAndRule_ShouldRetValidValue()
        {            
            //---------------Set up test pack-------------------

            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            const int minValue = 3;
            def.AddPropRule(CreatePropRuleInt(minValue, 7));
            ValidValueGenerator generator = new ValidValueGeneratorIncrementalInt(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(7, propRule.MaxValue);
            //---------------Execute Test ----------------------
            int value = (int)generator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreEqual(minValue, value);
        }
        [Test]
        public void Test_GenerateValue_WhenIntAndRule_AndNextValGTMaxVal_ShouldRestartAtMin()
        {            
            //---------------Set up test pack-------------------

            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            const int minValue = 3;
            const int maxValue = 4;
            def.AddPropRule(CreatePropRuleInt(minValue, maxValue));
            ValidValueGenerator generator = new ValidValueGeneratorIncrementalInt(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(int), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleInteger>().ToList());
            PropRuleInteger propRule = def.PropRules.OfType<PropRuleInteger>().First();
            Assert.AreEqual(minValue, propRule.MinValue);
            Assert.AreEqual(maxValue, propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value1 = (int)generator.GenerateValidValue();
            var value2 = (int)generator.GenerateValidValue();
            int value3 = (int)generator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreEqual(minValue, value1);
            Assert.AreEqual(maxValue, value2);
            Assert.AreEqual(minValue, value3);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenIntAndNoRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(int)
            };
            ValidValueGeneratorIncrementalInt generator = new ValidValueGeneratorIncrementalInt(def);
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
            ValidValueGeneratorIncrementalInt generator = new ValidValueGeneratorIncrementalInt(def);
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
            ValidValueGeneratorIncrementalInt generator = new ValidValueGeneratorIncrementalInt(def);
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
            ValidValueGeneratorIncrementalInt generator = new ValidValueGeneratorIncrementalInt(def);
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
            ValidValueGeneratorIncrementalInt generator = new ValidValueGeneratorIncrementalInt(def);
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
            ValidValueGeneratorIncrementalInt generator = new ValidValueGeneratorIncrementalInt(def);
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
            ValidValueGeneratorIncrementalInt generator = new ValidValueGeneratorIncrementalInt(def);
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
            ValidValueGeneratorIncrementalInt generator = new ValidValueGeneratorIncrementalInt(def);
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

