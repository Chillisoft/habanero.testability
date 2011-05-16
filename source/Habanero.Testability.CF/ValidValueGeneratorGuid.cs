using Habanero.Base;
using System;

namespace Habanero.Testability
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

