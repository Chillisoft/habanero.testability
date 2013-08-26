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
using System.Data.SqlTypes;

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
        private long SOME_LARGE_NUMBER = 1000000;

        private static PropRuleDate CreatePropRuleDate(DateTime min, DateTime max)
        {
            return new PropRuleDate(RandomValueGen.GetRandomString(), RandomValueGen.GetRandomString(), min, max);
        }

        [Test]
        public void Test_GenerateValue_WhenDateTime_ShouldSet()
        {
            //---------------Set up test pack-------------------
            var propertyType = typeof(DateTime);
            IPropDef def = new PropDefFake {
                PropertyType = propertyType
            };
            ValidValueGenerator valueGenerator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var value = valueGenerator.GenerateValidValue();
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
            var min = RandomValueGen.GetAbsoluteMin<DateTime>().AddDays(5555.0);
            var max = RandomValueGen.GetAbsoluteMin<DateTime>().AddDays(5565.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            ValidValueGenerator generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            var propRule = def.PropRules.OfType<PropRuleDate>().First();
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
            var min = DateTime.Now;
            var max = RandomValueGen.GetAbsoluteMax<DateTime>().AddDays(-5.0);
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
            var min = DateTime.Now;
            var max = DateTime.Now.AddDays(5.0);
            var overridingMinValue = RandomValueGen.GetAbsoluteMin<DateTime>().AddDays(9.0);
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
            var overridingMaxValue = expectedAbsoluteMin.AddDays(7.0);
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
            var min = DateTime.Now;
            var max = RandomValueGen.GetAbsoluteMax<DateTime>().AddDays(-5.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            var generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            var propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (DateTime)generator.GenerateValidValueLessThan(null);
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
            var min = DateTime.Now;
            var max = RandomValueGen.GetAbsoluteMax<DateTime>().AddDays(-5.0);
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
            var min = DateTime.Now;
            var max = DateTime.Today.AddDays(5.0);
            var overridingMaxValue = RandomValueGen.GetAbsoluteMax<DateTime>().AddDays(-5.0);
            def.AddPropRule(CreatePropRuleDate(min, max));
            var generator = new ValidValueGeneratorDate(def);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(DateTime), def.PropertyType);
            Assert.IsNotEmpty(def.PropRules.OfType<PropRuleDate>().ToList());
            var propRule = def.PropRules.OfType<PropRuleDate>().First();
            Assert.AreEqual(min, propRule.MinValue);
            Assert.AreEqual(max.AddDays(1.0).AddMilliseconds(-1.0), propRule.MaxValue);
            //---------------Execute Test ----------------------
            var value = (DateTime)generator.GenerateValidValueLessThan(overridingMaxValue);
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.GreaterOrEqual(value, min);
            Assert.LessOrEqual(value, max);
            Assert.LessOrEqual(value, overridingMaxValue);
        }

        [Test]
        public void GetRandomDate_GivenNoArguments_ShouldNeverReturnValueLessThanSQLServerSmallDateTimeMin()
        {
            //---------------Set up test pack-------------------
            var smallDateTimeMin = new DateTime(1900, 1, 1);
            //---------------Assert Precondition----------------
            for (var i = 0; i < SOME_LARGE_NUMBER; i++)
            {
                Assert.That(RandomValueGen.GetRandomDate(), Is.GreaterThanOrEqualTo(smallDateTimeMin));
            }

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
        }

        [Test]
        public void GetRandomDate_GivenMinValueLessThanSQLServerSmallDateTimeMin_WillReturnValueDownToGivenMin()
        {
            //---------------Set up test pack-------------------
            var myMin = new DateTime(1500, 1, 1);
            var myMax = new DateTime(1600, 1, 1);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            for (var i = 0; i < SOME_LARGE_NUMBER; i++)
            {
                Assert.That(RandomValueGen.GetRandomDate(myMin, myMax), Is.GreaterThanOrEqualTo(myMin));
            }

            //---------------Test Result -----------------------
        }

        [Test]
        public void GetRandomDate_GivenNoArguments_ShouldNeverReturnValueGreaterThanSQLServerSmallDateTimeMax()
        {
            //---------------Set up test pack-------------------
            var smallDateTimeMax = new DateTime(2079, 6, 6);
            
            //---------------Assert Precondition----------------
            for (var i = 0; i < SOME_LARGE_NUMBER; i++)
            {
                Assert.That(RandomValueGen.GetRandomDate(), Is.LessThanOrEqualTo(smallDateTimeMax));
            }
            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
        }

        [Test]
        public void GetRandomDate_GivenMaxValueGreaterThanSQLServerSmalLDateTimeMax_WillReturnValueUpToGivenMax()
        {
            //---------------Set up test pack-------------------
            var myMax = new DateTime(9000, 1, 1);   // age of aquarius?
            var myMin = new DateTime(8000, 1, 1);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            for (var i = 0; i < SOME_LARGE_NUMBER; i++)
            {
                Assert.That(RandomValueGen.GetRandomDate(myMin, myMax), Is.LessThanOrEqualTo(myMax));
            }

            //---------------Test Result -----------------------
        }

        [Test]
        public void GetMinimumSqlSmallDateTimeValue_Returns_19000101_AsPerSpec()
        {
            // see http://technet.microsoft.com/en-us/library/ms182418.aspx
            //---------------Set up test pack-------------------
            var expected = new DateTime(1900, 1, 1);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.AreEqual(expected, RandomValueGen.GetMinimumSqlSmallDateTimeValue());

            //---------------Test Result -----------------------
        }

        [Test]
        public void GetMaximumSqlSmallDateTimeValue_Returns_20790606_AsPerSpec()
        {
            // see http://technet.microsoft.com/en-us/library/ms182418.aspx
            //---------------Set up test pack-------------------
            var expected = new DateTime(2079, 6, 6);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.AreEqual(expected, RandomValueGen.GetMaximumSqlSmallDateTimeValue());

            //---------------Test Result -----------------------
        }

        [Test]
        public void GetRandomDate_WhenGivenMaxDateAsStringWhichIsGreaterThanSqlSmallDateTimeMinValue_DoesNotReturnDateLessThanSqlSmallDateTimeMinValue()
        {
            //---------------Set up test pack-------------------
            var myMax = new DateTime(1980, 1, 1);
            //---------------Assert Precondition----------------
            var minimumSqlSmallDateTimeValue = RandomValueGen.GetMinimumSqlSmallDateTimeValue();
            Assert.That(myMax, Is.GreaterThan(minimumSqlSmallDateTimeValue));


            //---------------Execute Test ----------------------
            var myDateString = myMax.ToString("yyyy/MM/dd");
            for (var i = 0; i < SOME_LARGE_NUMBER; i++)
            {
                Assert.That(RandomValueGen.GetRandomDate(myDateString), Is.GreaterThan(minimumSqlSmallDateTimeValue));
            }

            //---------------Test Result -----------------------
        }

        [Test]
        public void GetRandomDate_WhenGivenMaxDateAsStringWhichIsLessThanSqlSmalLDateTimeMinValue_WillReturnValuesLessThanThatDateTime()
        {
            //---------------Set up test pack-------------------
            var sqlmin = RandomValueGen.GetMinimumSqlSmallDateTimeValue();
            var myMin = sqlmin.AddDays(-1);
            var myMinString = myMin.ToString("yyyy/MM/dd");
            //---------------Assert Precondition----------------
            
            //---------------Execute Test ----------------------
            Assert.That(RandomValueGen.GetRandomDate(myMinString), Is.LessThan(RandomValueGen.GetMinimumSqlSmallDateTimeValue()));

            //---------------Test Result -----------------------
        }

    }
}

