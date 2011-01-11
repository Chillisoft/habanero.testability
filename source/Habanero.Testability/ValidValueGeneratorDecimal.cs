namespace Habanero.Testability
{
    using Habanero.Base;
    using Habanero.BO;
    using System;

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
     /*   private static decimal GetMaxValue(PropRuleDecimal propRule, decimal? overridingMaxValue)
        {
            decimal propRuleMaxValue = (propRule == null) ? 79228162514264337593543950335M : propRule.MaxValue;
            if (!overridingMaxValue.HasValue)
            {
                overridingMaxValue = 79228162514264337593543950335M;
            }
            decimal? acquisitionCost = overridingMaxValue;
            decimal CS$0$0003 = propRuleMaxValue;
            if ((acquisitionCost.GetValueOrDefault() > CS$0$0003) && acquisitionCost.HasValue)
            {
                overridingMaxValue = new decimal?(propRuleMaxValue);
            }
            return overridingMaxValue.Value;
        }

        private static decimal GetMinValue(PropRuleDecimal propRule, decimal? overridingMinValue)
        {
            decimal propRuleMinValue = (propRule == null) ? decimal.MinValue  : propRule.MinValue;
            if (!overridingMinValue.HasValue)
            {
                overridingMinValue = decimal.MinValue ;
            }
            decimal? acquisitionCost = overridingMinValue;
            decimal CS$0$0003 = propRuleMinValue;
            if ((acquisitionCost.GetValueOrDefault() < CS$0$0003) && CS$0$0002.HasValue)
            {
                overridingMinValue = new decimal?(propRuleMinValue);
            }
            return overridingMinValue.Value;
        }*/
    }
}

