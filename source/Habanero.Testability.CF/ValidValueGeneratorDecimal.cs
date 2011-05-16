using Habanero.Base;
using Habanero.BO;

namespace Habanero.Testability
{
    public class ValidValueGeneratorDecimal : ValidValueGenerator, IValidValueGeneratorNumeric
    {
        public ValidValueGeneratorDecimal(IPropDef propDef) : base(propDef)
        {
        }

        public override object GenerateValidValue()
        {
            PropRuleDecimal propRule = base.GetPropRule<PropRuleDecimal>();
            return ((propRule == null) ? RandomValueGen.GetRandomDecimal() : RandomValueGen.GetRandomDecimal(propRule.MinValue, propRule.MaxValue));
        }

        private decimal GenerateValidValue(decimal? overridingMinValue, decimal? overridingMaxValue)
        {
            PropRuleDecimal propRule = base.GetPropRule<PropRuleDecimal>();
            decimal minValue = GetMinValue(propRule, overridingMinValue);
            decimal maxValue = GetMaxValue(propRule, overridingMaxValue);
            return RandomValueGen.GetRandomDecimal(minValue, maxValue);
        }

        public object GenerateValidValueGreaterThan(object minValue)
        {
            return this.GenerateValidValue((decimal?) minValue, null);
        }

        public object GenerateValidValueLessThan(object maxValue)
        {
            return this.GenerateValidValue(null, (decimal?) maxValue);
        }
        
        private static decimal GetMaxValue(IPropRuleComparable<decimal> propRule, decimal? overridingMaxValue)
        {
            return RandomValueGen.GetMaxValue(propRule, overridingMaxValue);
        }

        private static decimal GetMinValue(IPropRuleComparable<decimal> propRule, decimal? overridingMinValue)
        {
            return RandomValueGen.GetMinValue(propRule, overridingMinValue);
        }
    }
}

