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
            ILookupList lookupList = base.PropDef.LookupList;
            object generateValidValue = GetLookupListValue(lookupList);
            if (generateValidValue == null && lookupList is BusinessObjectLookupList)
            {
                BusinessObjectLookupList boLList = (BusinessObjectLookupList) lookupList;
                var boTestFactory = BOTestFactoryRegistry.Instance.Resolve(boLList.BoType);
                IBusinessObject businessObject = boTestFactory.CreateSavedBusinessObject();
                generateValidValue = businessObject.ID.ToString();
            }
            object value;
            
            var tryParsePropValue = this.PropDef.TryParsePropValue(generateValidValue, out value);
            return tryParsePropValue? value: generateValidValue;
        }

        private static object GetLookupListValue(ILookupList lookupList)
        {
            return (((lookupList == null) || (lookupList is NullLookupList)) ? null : RandomValueGen.GetRandomLookupListValue(lookupList.GetLookupList()));
        }
    }
}

