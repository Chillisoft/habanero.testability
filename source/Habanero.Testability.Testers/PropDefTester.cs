using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Testability.Helpers;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.Testability.Testers
{
    /// <summary>
    /// This is a specialised Tester for testing Habanero.<see cref="IPropDef"/> 
    /// This tester provides methods for testing the basic attributes of a <see cref="IPropDef"/>
    /// such as ShouldBeCompulsory.
    /// If any of these Asserts fail then an <see cref="AssertionException"/>. is thrown.
    /// Else the Assert executes without an Exception
    /// </summary>
    public class PropDefTester : SingleValueTester
    {
        /// <summary>
        /// the Property Def that is being tested.
        /// </summary>
        public IPropDef PropDef { get; private set; }

        ///<summary>
        ///</summary>
        ///<param name="propDef"></param>
        ///<exception cref="ArgumentNullException"></exception>
        public PropDefTester(IPropDef propDef)
        {
            if (propDef == null) throw new ArgumentNullException("propDef");
            PropDef = propDef;
        }

        /// <summary>
        /// Should have a rule this Assert does not specify what rule just that it has a Rule
        /// </summary>
        public void ShouldHaveRule()
        {
            var errMessage = BaseMessage + " should have Rules set";
            this.PropDef.PropRules.ShouldNotBeEmpty(errMessage);
        }

        /// <summary>
        /// Should have a rule of the specified type e.g. <see cref="PropRuleDate"/>.
        /// Note_ this Test does not test that the min, max values etc are correct for the
        /// rule merely that there is a rule of that type.
        /// </summary>
        public void ShouldHaveRule<TRuleType>() where TRuleType: IPropRule
        {
            var ruleType = typeof(TRuleType);
            var errMessage = BaseMessage +" should have a rule of type '" + ruleType + "'";
            var matchingRule = GetRuleOfType<TRuleType>();
            matchingRule.ShouldNotBeNull(errMessage);
        }

        private IPropRule GetRuleOfType<TRuleType>() where TRuleType : IPropRule
        {
            return this.PropDef.PropRules.FirstOrDefault(rule => rule.IsOfType<TRuleType>());
        }

        /// <summary>
        /// Should have a rule of the specified type e.g. <see cref="PropRuleDate"/>.
        /// This is necessary for the case when you have your rule defined as string e.g. minDate == "Today"
        /// </summary>
        public void ShouldHaveRuleDate(string minDate, string maxDate)
        {
            this.ShouldHaveRule<PropRuleDate>();

            var message = BaseMessage + " the PropRule of type '" + typeof(PropRuleDate);
            var propRule = GetRuleOfType<PropRuleDate>() as PropRuleDate;

            if (!string.IsNullOrEmpty(minDate) && propRule != null)
            {
                var expectedMinDate = DateTimeUtilities.ParseToDate(minDate);
                Assert.AreEqual(expectedMinDate, propRule.MinValue, message + "' MinValue Should Be '" + minDate + "'");
            }
            if (!string.IsNullOrEmpty(maxDate) && propRule != null)
            {
                var expectedMaxDate = GetLastMillisecondOfDay(maxDate);
                Assert.AreEqual(expectedMaxDate, propRule.MaxValue, message + "' MaxValue Should Be '" + minDate + "'");
            }
        }

        private static DateTime GetLastMillisecondOfDay(string max)
        {
            return DateTimeUtilities.ParseToDate(max).AddDays(1).AddMilliseconds(-1);
        }

        /// <summary>
        /// Should have a rule of the specified type e.g. <see cref="PropRuleDate"/>.
        /// </summary>
        public void ShouldHaveRule<TRuleType, T>(T? min, T? max) where TRuleType : IPropRuleComparable<T>, IPropRule where T : struct
        {
            this.ShouldHaveRule<TRuleType>();

            var message = BaseMessage + " the PropRule of type '" + typeof(TRuleType);
            var propRule = GetRuleOfType<TRuleType>() as IPropRuleComparable<T>;
            if (min != null && propRule != null) Assert.AreEqual(min, propRule.MinValue, message + "' MinValue Should Be '" + min + "'");
            if (max != null && propRule != null) Assert.AreEqual(max, propRule.MaxValue, message + "' MaxValue Should Be '" + min + "'");
        }

        /// <summary>
        /// Should have a default value this Assert does not specify what the default value should be 
        /// but merely assserts that there is one.
        /// </summary>
        public void ShouldHaveDefault()
        {
            string message = BaseMessage + " should have a default but does not";
            Assert.IsNotNull(this.PropDef.DefaultValueString, message);
        }
        /// <summary>
        /// Asserts that this <see cref="PropDef"/> have a default matching the <paramref name="expectedDefaultValueString"/>
        /// </summary>
        /// <param name="expectedDefaultValueString"></param>
        public void ShouldHaveDefault(string expectedDefaultValueString)
        {
            this.ShouldHaveDefault();
            string actualDefaultValueString = this.PropDef.DefaultValueString;
            string errMessage = BaseMessage + string.Format(
                " should have a default of '{0}' but has a default value of '{1}'", expectedDefaultValueString, actualDefaultValueString);
            Assert.AreEqual(expectedDefaultValueString, actualDefaultValueString, errMessage);
        }
        /// <summary>
        /// Asserts that this <see cref="PropDef"/> does not have a default/>
        /// </summary>
        public void ShouldNotHaveDefault()
        {
            string message = BaseMessage + " should not have a default but does";
            Assert.IsNull(this.PropDef.DefaultValueString, message);
        }
        /// <summary>
        /// Asserts that this <see cref="PropDef"/> does not have a default matching the <paramref name="expectedDefaultValueString"/>
        /// </summary>
        /// <param name="expectedDefaultValueString"></param>
        public void ShouldNotHaveDefault(string expectedDefaultValueString)
        {
            this.ShouldNotHaveDefault();
            string actualDefaultValueString = this.PropDef.DefaultValueString;
            string errMessage = BaseMessage + string.Format(
                " should not have a default of '{0}' but has a default value of '{1}'", expectedDefaultValueString, actualDefaultValueString);
            Assert.AreNotEqual(expectedDefaultValueString, actualDefaultValueString, errMessage);
        }
        /// <summary>
        /// Asserts that this <see cref="PropDef"/> has read write rules matching the <paramref name="expectedReadWriteRule"/>
        /// </summary>
        /// <param name="expectedReadWriteRule"></param>
        public void ShouldHaveReadWriteRule(PropReadWriteRule expectedReadWriteRule)
        {
            string errMessage = BaseMessage + string.Format(
                " should have a ReadWriteRule '{0}' but is '{1}'", expectedReadWriteRule, this.PropDef.ReadWriteRule);
            Assert.AreEqual(expectedReadWriteRule, this.PropDef.ReadWriteRule, errMessage);
        }

        #region Overrides of SingleValueTester

        public override ISingleValueDef SingleValueDef
        {
            get { return this.PropDef; }
        }

        #endregion
    }

    ///<summary>
    /// Extension methods for easily testing an <see cref="IPropDef"/>
    ///</summary>
    public static class PropDefExtensions
    {
        /// <summary>
        /// Checks if the PropRule is of the correct Type
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static bool IsOfType<TRuleType>(this IPropRule rule) where TRuleType: IPropRule
        {
            return rule.GetType() == typeof(TRuleType);
        }

        /// <summary>
        /// Asserts that the property Def is set up as compulsory
        /// </summary>
        /// <param name="propDef"></param>
        public static void ShouldBeCompulsory(this IPropDef propDef)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldBeCompulsory();
        }
        /// <summary>
        /// Asserts that the propDef is not set up as compulsory
        /// </summary>
        /// <param name="propDef"></param>
        public static void ShouldNotBeCompulsory(this IPropDef propDef)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldNotBeCompulsory();
        }
        /// <summary>
        /// Asserts that the <see cref="PropDef"/> does not have a default
        /// </summary>
        /// <param name="propDef"></param>
        public static void ShouldNotHaveDefault(this IPropDef propDef)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldNotHaveDefault();
        }
        /// <summary>
        /// Asserts that the <see cref="PropDef"/> has a default
        /// </summary>
        /// <param name="propDef"></param>
        public static void ShouldHaveDefault(this IPropDef propDef)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldHaveDefault();
        }

        /// <summary>
        /// Asserts that the <see cref="PropDef"/> has a default matching the <paramref name="expectedDefaultValueString"/>
        /// </summary>
        /// <param name="propDef"></param>
        /// <param name="expectedDefaultValueString"></param>
        public static void ShouldHaveDefault(this IPropDef propDef, string expectedDefaultValueString)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldHaveDefault(expectedDefaultValueString);
        }
        /// <summary>
        /// Asserts that the <see cref="PropDef"/> does not have a default matching the <paramref name="expectedDefaultValueString"/>
        /// </summary>
        /// <param name="propDef"></param>
        /// <param name="expectedDefaultValueString"></param>
        public static void ShouldNotHaveDefault(this IPropDef propDef, string expectedDefaultValueString)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldNotHaveDefault(expectedDefaultValueString);
        }
        /// <summary>
        /// Asserts that the <see cref="PropDef"/> does not have a default matching the <paramref name="expectedReadWriteRule"/>
        /// </summary>
        /// <param name="propDef"></param>
        /// <param name="expectedReadWriteRule"></param>
        public static void ShouldHaveReadWriteRule(this IPropDef propDef, PropReadWriteRule expectedReadWriteRule)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldHaveReadWriteRule(expectedReadWriteRule);
        }
    }
}