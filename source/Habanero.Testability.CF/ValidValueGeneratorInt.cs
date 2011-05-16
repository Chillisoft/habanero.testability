using Habanero.Base;
using Habanero.BO;

namespace Habanero.Testability
{
    public class ValidValueGeneratorInt : ValidValueGenerator, IValidValueGeneratorNumeric
    {
        public ValidValueGeneratorInt(IPropDef propDef) : base(propDef)
        {
        }

        public override object GenerateValidValue()
        {
            return this.GenerateValidValue(null, null);
        }

        private int GenerateValidValue(int? overridingMinValue, int? overridingMaxValue)
        {
            PropRuleInteger propRule = base.GetPropRule<PropRuleInteger>();
            int intMinValue = GetMinValue(propRule, overridingMinValue);
            int intMaxValue = GetMaxValue(propRule, overridingMaxValue);
            return RandomValueGen.GetRandomInt(intMinValue, intMaxValue);
        }

        public object GenerateValidValueGreaterThan(object minValue)
        {
            return this.GenerateValidValue((int?) minValue, null);
        }

        public object GenerateValidValueLessThan(object maxValue)
        {
            return this.GenerateValidValue(null, (int?) maxValue);
        }

        private static int GetMaxValue(IPropRuleComparable<int> propRule, int? overridingMaxValue)
        {
            return RandomValueGen.GetMaxValue(propRule, overridingMaxValue);
        }

        private static int GetMinValue(IPropRuleComparable<int> propRule, int? overridingMinValue)
        {
            return RandomValueGen.GetMinValue(propRule, overridingMinValue);
        }
    }
}

