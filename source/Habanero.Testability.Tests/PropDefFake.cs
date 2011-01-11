namespace Habanero.Testability.Tests
{
    using Habanero.BO.ClassDefinition;
    using System;

    internal class PropDefFake : PropDef
    {
        public PropDefFake() : base(RandomValueGen.GetRandomString(), typeof(int), PropReadWriteRule.ReadWrite, null)
        {
        }
    }
}

