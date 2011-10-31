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

namespace Habanero.Testability.Tests
{
// ReSharper disable InconsistentNaming
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
            DateTime min = RandomValueGen.GetAbsoluteMin<DateTime>().AddDays(5555.0);
            DateTime max = RandomValueGen.GetAbsoluteMin<DateTime>().AddDays(5565.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGenerator generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max, propRule.MaxValue.Date);
            //---------------Execute Test ----------------------
            var value = (DateTime)generator.GenerateValidValue();
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
            var generator = new ValidValueGeneratorDate(def);
            var overridingMinValue = RandomValueGen.GetAbsoluteMax<DateTime>().AddDays(-9.0);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            //---------------Execute Test ----------------------
            var value = (DateTime)generator.GenerateValidValueGreaterThan(overridingMinValue);
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
            DateTime max = RandomValueGen.GetAbsoluteMax<DateTime>().AddDays(-5.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            var generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            var propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (DateTime)generator.GenerateValidValueGreaterThan(null);
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
            DateTime overridingMinValue = RandomValueGen.GetAbsoluteMin<DateTime>().AddDays(9.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            var generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            PropRuleDate propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (DateTime)generator.GenerateValidValueGreaterThan(overridingMinValue);
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
            var min = DateTime.Now;
            var max = RandomValueGen.GetAbsoluteMax<DateTime>().AddDays(-5.0);
            var overridingMinValue = max.AddDays(-9.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            var generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            var propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (DateTime)generator.GenerateValidValueGreaterThan(overridingMinValue);
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
            var generator = new ValidValueGeneratorDate(def);
            DateTime overridingMaxValue = expectedAbsoluteMin.AddDays(7.0);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            //---------------Execute Test ----------------------
            var value = (DateTime)generator.GenerateValidValueLessThan(overridingMaxValue);
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
            DateTime max = RandomValueGen.GetAbsoluteMax<DateTime>().AddDays(-5.0);
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
            DateTime max = RandomValueGen.GetAbsoluteMax<DateTime>().AddDays(-5.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            var generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            var propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            var overridingMaxValue = DateTime.Now.AddDays(5.0);
            //---------------Execute Test ----------------------
            var value = (DateTime)generator.GenerateValidValueLessThan(overridingMaxValue);
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
            DateTime overridingMaxValue = RandomValueGen.GetAbsoluteMax<DateTime>().AddDays(-5.0);
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

