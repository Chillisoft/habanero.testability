using System;
using Habanero.Base;
using NUnit.Framework;

namespace Habanero.Testability.Testers
{
    /// <summary>
    /// This is a specialised Tester for testing Habanero.<see cref="IPropDef"/> 
    /// This tester provides methods for testing the basic attributes of a <see cref="IPropDef"/>
    /// such as ShouldBeCompulsory.
    /// </summary>
    public class BOPropTester
    {
        public IBOProp BOProp { get; private set; }

        public BOPropTester(IBOProp boProp)
        {
            if (boProp == null) throw new ArgumentNullException("boProp");
            BOProp = boProp;
        }
/*
        public void ShouldBeCompulsory()
        {
            var expectedMessage = string.Format("The PropDef for '{0}' for class '{1}' should be compulsory", this.PropDef.PropertyName, this.PropDef.ClassName);
            Assert.IsTrue(this.PropDef.Compulsory, expectedMessage);
        }

        public void ShouldHaveRule()
        {
            var expectedMessage = string.Format("The PropDef for '{0}' for class '{1}' should have Rules set", this.PropDef.PropertyName, this.PropDef.ClassName);
            this.PropDef.PropRules.ShouldNotBeEmpty(expectedMessage);
        }*/
    }
}