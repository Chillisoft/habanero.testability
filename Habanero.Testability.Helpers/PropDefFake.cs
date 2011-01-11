using Habanero.Base;

namespace Habanero.Testability.Tests
{
    using Habanero.BO.ClassDefinition;

    internal class PropDefFake : PropDef
    {
        public PropDefFake() : base(RandomValueGen.GetRandomString(), typeof(int), PropReadWriteRule.ReadWrite, null)
        {
        }
    }
}

