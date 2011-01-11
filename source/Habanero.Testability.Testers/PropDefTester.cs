using System;
using System.Collections.Generic;
using System.Text;
using Habanero.Base;
using NUnit.Framework;

namespace Habanero.Testability.Testers
{
    /// <summary>
    /// This is a specialised Tester for testing Habanero.<see cref="IPropDef"/> 
    /// This tester provides methods for testing the basic attributes of a <see cref="IPropDef"/>
    /// such as ShouldBeCompulsory.
    /// </summary>
    public class PropDefTester : SingleValueTester
    {
        public IPropDef PropDef { get; private set; }

        public PropDefTester(IPropDef propDef)
        {
            if (propDef == null) throw new ArgumentNullException("propDef");
            PropDef = propDef;
        }

        public void ShouldHaveRule()
        {
            var expectedMessage = BaseMessage + " should have Rules set";
            this.PropDef.PropRules.ShouldNotBeEmpty(expectedMessage);
        }

        public void ShouldHaveDefault()
        {
            string message = BaseMessage + " should have a default but does not";
            Assert.IsNotNull(this.PropDef.DefaultValueString, message);
        }

        public void ShouldHaveDefault(string expectedDefaultValueString)
        {
            this.ShouldHaveDefault();
            string actualDefaultValueString = this.PropDef.DefaultValueString;
            string errMessage = BaseMessage + string.Format(
                " should have a default of '{0}' but has a default value of '{1}'", expectedDefaultValueString, actualDefaultValueString);
            Assert.AreEqual(expectedDefaultValueString, actualDefaultValueString, errMessage);
        }

        public void ShouldNotHaveDefault()
        {
            string message = BaseMessage + " should not have a default but does";
            Assert.IsNull(this.PropDef.DefaultValueString, message);
        }

        public void ShouldNotHaveDefault(string expectedDefaultValueString)
        {
            this.ShouldNotHaveDefault();
            string actualDefaultValueString = this.PropDef.DefaultValueString;
            string errMessage = BaseMessage + string.Format(
                " should not have a default of '{0}' but has a default value of '{1}'", expectedDefaultValueString, actualDefaultValueString);
            Assert.AreNotEqual(expectedDefaultValueString, actualDefaultValueString, errMessage);
        }

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

    public static class PropDefExtensions
    {
        public static void ShouldBeCompulsory(this IPropDef propDef)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldBeCompulsory();
        }

        public static void ShouldNotBeCompulsory(this IPropDef propDef)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldNotBeCompulsory();
        }

        public static void ShouldNotHaveDefault(this IPropDef propDef)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldNotHaveDefault();
        }

        public static void ShouldHaveDefault(this IPropDef propDef)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldHaveDefault();
        }

        public static void ShouldHaveDefault(this IPropDef propDef, string expectedDefaultValueString)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldHaveDefault(expectedDefaultValueString);
        }

        public static void ShouldNotHaveDefault(this IPropDef propDef, string expectedDefaultValueString)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldNotHaveDefault(expectedDefaultValueString);
        }

        public static void ShouldHaveReadWriteRule(this IPropDef propDef, PropReadWriteRule expectedReadWriteRule)
        {
            PropDefTester tester = new PropDefTester(propDef);
            tester.ShouldHaveReadWriteRule(expectedReadWriteRule);
        }
    }
}