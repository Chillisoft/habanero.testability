using System;
using System.Collections.Generic;
using System.Linq;
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
    public class PropDefTester
    {
        public IPropDef PropDef { get; private set; }

        public PropDefTester(IPropDef propDef)
        {
            if (propDef == null) throw new ArgumentNullException("propDef");
            PropDef = propDef;
        }

        public void ShouldBeCompulsory()
        {
            var expectedMessage = string.Format("The Property '{0}' for class '{1}' should be compulsory",
                                                PropertyName, ClassName);
            Assert.IsTrue(this.PropDef.Compulsory, expectedMessage);
        }

        public void ShouldNotBeCompulsory()
        {
            var expectedMessage = string.Format("The Property '{0}' for class '{1}' should not be compulsory",
                                                PropertyName, ClassName);
            Assert.IsFalse(this.PropDef.Compulsory, expectedMessage);
        }

        public void ShouldHaveRule()
        {
            var expectedMessage = string.Format("The Property '{0}' for class '{1}' should have Rules set",
                                                PropertyName, ClassName);
            this.PropDef.PropRules.ShouldNotBeEmpty(expectedMessage);
        }

        public void ShouldHaveDefault()
        {
            string message = string.Format("The Property '{0}' for class '{1}' should have a default but does not", PropertyName, ClassName);
            Assert.IsNotNull(this.PropDef.DefaultValueString, message);
        }

        public void ShouldHaveDefault(string expectedDefaultValueString)
        {
            this.ShouldHaveDefault();
            string actualDefaultValueString = this.PropDef.DefaultValueString;
            string errMessage = string.Format(
                "The Property '{0}' for class '{1}' should have a default of '{2}' but has a default value of '{3}'", PropertyName,
                ClassName, expectedDefaultValueString, actualDefaultValueString);
            Assert.AreEqual(expectedDefaultValueString, actualDefaultValueString, errMessage);
        }

        public void ShouldNotHaveDefault()
        {
            string message = string.Format("The Property '{0}' for class '{1}' should not have a default but does", PropertyName, ClassName);
            Assert.IsNull(this.PropDef.DefaultValueString, message);
        }

        public void ShouldNotHaveDefault(string expectedDefaultValueString)
        {
            this.ShouldNotHaveDefault();
            string actualDefaultValueString = this.PropDef.DefaultValueString;
            string errMessage = string.Format(
                "The Property '{0}' for class '{1}' should not have a default of '{2}' but has a default value of '{3}'", PropertyName,
                ClassName, expectedDefaultValueString, actualDefaultValueString);
            Assert.AreNotEqual(expectedDefaultValueString, actualDefaultValueString, errMessage);
        }

        private string ClassName
        {
            get { return this.PropDef.ClassName; }
        }

        private string PropertyName
        {
            get { return this.PropDef.PropertyName; }
        }

        public void ShouldHaveReadWriteRule(PropReadWriteRule expectedReadWriteRule)
        {

            string errMessage = string.Format(
                "The Property '{0}' for class '{1}' should have a ReadWriteRule '{2}' but is '{3}'", PropertyName,
                ClassName, expectedReadWriteRule, this.PropDef.ReadWriteRule);
            Assert.AreEqual(expectedReadWriteRule, this.PropDef.ReadWriteRule, errMessage);
        }
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