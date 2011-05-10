using Habanero.Base;
using System;

namespace Habanero.Testability.CF
{
    public class ValidValueGeneratorGuid : ValidValueGenerator
    {
        public ValidValueGeneratorGuid(IPropDef propDef) : base(propDef)
        {
        }

        public override object GenerateValidValue()
        {
            return Guid.NewGuid();
        }
    }
}

