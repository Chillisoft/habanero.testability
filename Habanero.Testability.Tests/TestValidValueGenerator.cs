namespace Habanero.Testability.Tests
{
    using Habanero.Base;
    using Habanero.BO;
    using Habanero.Testability;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class TestValidValueGenerator
    {
        private static PropRuleDate CreatePropRuleDate(DateTime min, DateTime max)
        {
            return new PropRuleDate(RandomValueGen.GetRandomString(), RandomValueGen.GetRandomString(), min, max);
        }

        private static PropRuleDecimal CreatePropRuleDecimal(decimal min, decimal max)
        {
            return new PropRuleDecimal(RandomValueGen.GetRandomString(), RandomValueGen.GetRandomString(), min, max);
        }
/*
        private static PropRuleInteger CreatePropRuleInt(int min, int max)
        {
            return new PropRuleInteger(RandomValueGen.GetRandomString(), RandomValueGen.GetRandomString(), min, max);
        }*/

        private static PropRuleString CreatePropRuleString(int minLength, int maxLength)
        {
            return new PropRuleString(RandomValueGen.GetRandomString(), RandomValueGen.GetRandomString(), minLength, maxLength, "");
        }
/*
        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }*/

        [Test]
        public void Test_GenerateValue_WhenBool_ShouldGetBool()
        {
            Type propertyType = typeof(bool);
            IPropDef def = new PropDefFake {
                PropertyType = propertyType
            };
            object value = new ValidValueGeneratorBool(def).GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(bool), value);
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
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            DateTime value = (DateTime) generator.GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, min);
            Assert.LessOrEqual(value, max);
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
        public void Test_GenerateValue_WhenEnum_ShouldSet()
        {
            Type propertyType = typeof(FakeEnum);
            IPropDef def = new PropDefFake {
                PropertyType = propertyType
            };
            object value = new ValidValueGeneratorEnum(def).GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(FakeEnum), value);
        }

        [Test]
        public void Test_GenerateValue_WhenGuid_ShouldRetGuid()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(Guid)
            };
            ValidValueGenerator valueGenerator = new ValidValueGeneratorGuid(def);
            object value = valueGenerator.GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(Guid), value);
            Assert.AreNotEqual(valueGenerator.GenerateValidValue(), value);
        }

        [Test]
        public void Test_GenerateValue_WhenLookupList_ShouldSet()
        {
            Type propertyType = typeof(string);
            IPropDef def = new PropDefFake {
                PropertyType = propertyType
            };
            var dictionary = new Dictionary<string, string>
                                        {
                                            {"fda", "fdafasd"},
                                            {"fdadffasdf", "gjhj"}
                                        };
            def.LookupList = new SimpleLookupList(dictionary);
            Assert.IsNotNull(def.LookupList);
            Assert.AreEqual(2, def.LookupList.GetLookupList().Count);

            object value = new ValidValueGeneratorLookupList(def).GenerateValidValue();

            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(string), value);
            StringAssert.StartsWith("fda", value.ToString());
        }

        [Test]
        public void Test_GenerateValue_WhenString_ShouldRetToString()
        {
            IPropDef def = new PropDefFake {
                Compulsory = true,
                PropertyType = typeof(string)
            };
            ValidValueGeneratorString valueGenerator = new ValidValueGeneratorString(def);
            object value = valueGenerator.GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.IsNotNullOrEmpty(value.ToString());
            Assert.IsInstanceOf(typeof(string), value);
            Assert.AreNotEqual(valueGenerator.GenerateValidValue(), value);
        }

        [Test]
        public void Test_GenerateValue_WhenStringAndMaxLength_ShouldRetToValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(string)
            };
            def.AddPropRule(CreatePropRuleString(3, 7));
            ValidValueGenerator generator = new ValidValueGeneratorString(def);
            Assert.AreSame(typeof(string), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleString>().ToList());
            PropRuleString propRule = def.PropRules.OfType<PropRuleString>().First();
            Assert.AreEqual(3, propRule.MinLength);
            Assert.AreEqual(7, propRule.MaxLength);
            object value = generator.GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value.ToString().Length, 3);
            Assert.LessOrEqual(value.ToString().Length, 7);
        }

        [Test]
        public void Test_GenerateValueLList_WhenLookupListNull_ShouldRetNull()
        {
            Type propertyType = typeof(string);
            IPropDef def = new PropDefFake {
                PropertyType = propertyType
            };
            Assert.IsInstanceOf<NullLookupList>(def.LookupList);
            Assert.IsNull(new ValidValueGeneratorLookupList(def).GenerateValidValue());
        }
    }
}

