namespace Habanero.Testability
{
    using Habanero.Base;
    using Habanero.BO;
    using System;

    public class ValidValueGeneratorLookupList : ValidValueGenerator
    {
        public ValidValueGeneratorLookupList(IPropDef propDef) : base(propDef)
        {
        }

        public override object GenerateValidValue()
        {
            return (((base.PropDef.LookupList == null) || (base.PropDef.LookupList is NullLookupList)) ? null : RandomValueGen.GetRandomLookupListValue(base.PropDef.LookupList.GetLookupList()));
        }
    }
}

