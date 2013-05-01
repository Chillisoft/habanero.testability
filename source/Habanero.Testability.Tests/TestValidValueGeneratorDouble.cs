#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
#endregion
using Habanero.Testability.Tests.Base;
// ReSharper disable InconsistentNaming

namespace Habanero.Testability.Tests
{
    using Habanero.Base;
    using Habanero.BO;
    using Habanero.Testability;
    using NUnit.Framework;
    using System;
    using System.Linq;

    [TestFixture]
    public class TestValidValueGeneratorDouble
    {
        private static PropRuleDouble CreatePropRuleDouble(double min, double max)
        {
            return new PropRuleDouble(RandomValueGen.GetRandomString(), RandomValueGen.GetRandomString(), min, max);
        }

        [Test]
        public void Test_GenerateValue_WhenDecimapAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(double)
            };
            def.AddPropRule(CreatePropRuleDouble(3.01, 7.0004));
            ValidValueGenerator generator = new ValidValueGeneratorDouble(def);
            Assert.AreSame(typeof(double), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDouble>().ToList<PropRuleDouble>());
            var propRule = def.PropRules.OfType<PropRuleDouble>().First<PropRuleDouble>();
            Assert.AreEqual(3.01, propRule.MinValue);
            Assert.AreEqual(7.0004, propRule.MaxValue);
            var value = (double) generator.GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3.01);
            Assert.LessOrEqual(value, 7.0004);
        }

        [Test]
        public void Test_GenerateValue_WhenDouble_ShouldSet()
        {
            Type propertyType = typeof(double);
            IPropDef def = new PropDefFake {
                PropertyType = propertyType
            };
            ValidValueGenerator valueGenerator = new ValidValueGeneratorDouble(def);
            object value = valueGenerator.GenerateValidValue();
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(double), value);
            Assert.AreNotEqual(valueGenerator.GenerateValidValue(), value);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDoubleAndNoRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(double)
            };
            var generator = new ValidValueGeneratorDouble(def);
            Assert.AreSame(typeof(double), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleDouble>().ToList<PropRuleDouble>());
            var value = (double) generator.GenerateValidValueGreaterThan(1.7976931348623157E+308);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, double.MaxValue);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDoubleAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(double)
            };
            def.AddPropRule(CreatePropRuleDouble(3.0, double.MaxValue));
            var generator = new ValidValueGeneratorDouble(def);
            Assert.AreSame(typeof(double), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDouble>().ToList<PropRuleDouble>());
            var propRule = def.PropRules.OfType<PropRuleDouble>().First<PropRuleDouble>();
            Assert.AreEqual(3.0, propRule.MinValue);
            Assert.AreEqual(1.7976931348623157E+308, propRule.MaxValue);
            var value = (double) generator.GenerateValidValueGreaterThan(null);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3.0);
            Assert.LessOrEqual(value, double.MaxValue);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDoubleAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(double)
            };
            def.AddPropRule(CreatePropRuleDouble(3.0, double.MaxValue));
            var generator = new ValidValueGeneratorDouble(def);
            Assert.AreSame(typeof(double), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDouble>().ToList<PropRuleDouble>());
            var propRule = def.PropRules.OfType<PropRuleDouble>().First<PropRuleDouble>();
            Assert.AreEqual(3.0, propRule.MinValue);
            Assert.AreEqual(1.7976931348623157E+308, propRule.MaxValue);
            var value = (double) generator.GenerateValidValueGreaterThan(1.7976931348623157E+308);
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value, double.MaxValue);
            Assert.GreaterOrEqual(value, double.MaxValue);
        }

        [Test]
        public void Test_GenerateValueGreaterThan_WhenDoubleAndRule_WhenRuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(double)
            };
            def.AddPropRule(CreatePropRuleDouble(3.0, 7.0));
            var generator = new ValidValueGeneratorDouble(def);
            Assert.AreSame(typeof(double), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDouble>().ToList<PropRuleDouble>());
            var propRule = def.PropRules.OfType<PropRuleDouble>().First<PropRuleDouble>();
            Assert.AreEqual(3.0, propRule.MinValue);
            Assert.AreEqual(7.0, propRule.MaxValue);
            var value = (double) generator.GenerateValidValueGreaterThan(-1.7976931348623157E+308);
            Assert.IsNotNull(value);
            Assert.LessOrEqual(value, 7.0);
            Assert.GreaterOrEqual(value, double.MinValue);
            Assert.GreaterOrEqual(value, 3.0);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDoubleAndNoRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(double)
            };
            var generator = new ValidValueGeneratorDouble(def);
            Assert.AreSame(typeof(double), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleDouble>().ToList<PropRuleDouble>());
            var value = (double) generator.GenerateValidValueLessThan(-1.7976931348623157E+308);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, double.MinValue);
            Assert.LessOrEqual(value, double.MinValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDoubleAndRule_LtNull_ShouldRetValidValueUsingRules()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(double)
            };
            def.AddPropRule(CreatePropRuleDouble(3.0, double.MaxValue));
            var generator = new ValidValueGeneratorDouble(def);
            Assert.AreSame(typeof(double), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDouble>().ToList<PropRuleDouble>());
            var propRule = def.PropRules.OfType<PropRuleDouble>().First<PropRuleDouble>();
            Assert.AreEqual(3.0, propRule.MinValue);
            Assert.AreEqual(1.7976931348623157E+308, propRule.MaxValue);
            var value = (double) generator.GenerateValidValueLessThan(null);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3.0);
            Assert.LessOrEqual(value, double.MaxValue);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDoubleAndRule_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(double)
            };
            def.AddPropRule(CreatePropRuleDouble(3.0, double.MaxValue));
            var generator = new ValidValueGeneratorDouble(def);
            Assert.AreSame(typeof(double), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDouble>().ToList<PropRuleDouble>());
            var propRule = def.PropRules.OfType<PropRuleDouble>().First<PropRuleDouble>();
            Assert.AreEqual(3.0, propRule.MinValue);
            Assert.AreEqual(1.7976931348623157E+308, propRule.MaxValue);
            var value = (double) generator.GenerateValidValueLessThan(5.0);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3.0);
            Assert.LessOrEqual(value, 5.0);
        }

        [Test]
        public void Test_GenerateValueLessThan_WhenDoubleAndRule_WhenRuleMoreRestrictive_ShouldRetValidValue()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(double)
            };
            def.AddPropRule(CreatePropRuleDouble(3.0, 7.0));
            var generator = new ValidValueGeneratorDouble(def);
            Assert.AreSame(typeof(double), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDouble>().ToList<PropRuleDouble>());
            var propRule = def.PropRules.OfType<PropRuleDouble>().First<PropRuleDouble>();
            Assert.AreEqual(3.0, propRule.MinValue);
            Assert.AreEqual(7.0, propRule.MaxValue);
            var value = (double) generator.GenerateValidValueLessThan(1.7976931348623157E+308);
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, 3.0);
            Assert.LessOrEqual(value, double.MaxValue);
            Assert.LessOrEqual(value, 7.0);
        }
    }
}

