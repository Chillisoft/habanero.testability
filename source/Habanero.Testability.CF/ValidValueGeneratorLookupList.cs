using Habanero.Base;
using Habanero.BO;

namespace Habanero.Testability
{
    public class ValidValueGeneratorLookupList : ValidValueGenerator
    {
        public ValidValueGeneratorLookupList(ISingleValueDef propDef)
            : base(propDef)
        {
        }

        public override object GenerateValidValue()
        {
            var lookupList = base.SingleValueDef.LookupList;
            var generateValidValue = GetLookupListValue(lookupList);
            if (generateValidValue == null && lookupList is BusinessObjectLookupList)
            {
                BusinessObjectLookupList boLList = (BusinessObjectLookupList) lookupList;
                var boTestFactory = BOTestFactoryRegistry.Instance.Resolve(boLList.BoType);
                IBusinessObject businessObject = boTestFactory.CreateSavedBusinessObject();
                generateValidValue = businessObject.ID.GetAsValue();
            }
            object value;
            IPropDef propDef = this.SingleValueDef as IPropDef;
            if(propDef == null) return generateValidValue;
            var tryParsePropValue = propDef.TryParsePropValue(generateValidValue, out value);
            return tryParsePropValue? value: generateValidValue;
        }

        private static object GetLookupListValue(ILookupList lookupList)
        {
            return (((lookupList == null) || (lookupList is NullLookupList)) ? null : RandomValueGen.GetRandomLookupListValue(lookupList.GetLookupList()));
        }
    }
}