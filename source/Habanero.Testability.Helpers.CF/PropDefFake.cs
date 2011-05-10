using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Testability.CF;

namespace Habanero.Testability.Helpers
{
    internal class PropDefFake : PropDef
    {
        public PropDefFake() : base(RandomValueGen.GetRandomString(), typeof(int), PropReadWriteRule.ReadWrite, null)
        {
        }
    }
 
}

