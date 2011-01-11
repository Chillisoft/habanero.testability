namespace Habanero.Testability
{
    using Habanero.Base;
    using System;

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

