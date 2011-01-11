namespace Habanero.Testability
{
    using Habanero.Base;
    using Habanero.BO;
    using System;

    public class ValidValueGeneratorDouble : ValidValueGenerator, IValidValueGeneratorNumeric
    {
        public ValidValueGeneratorDouble(IPropDef propDef) : base(propDef)
        {
        }

        public override object GenerateValidValue()
        {
            PropRuleDouble propRule = base.GetPropRule<PropRuleDouble>();
            return ((propRule == null) ? RandomValueGen.GetRandomDouble() : RandomValueGen.GetRandomDouble(propRule.MinValue, propRule.MaxValue));
        }

        private double GenerateValidValue(double? overridingMinValue, double? overridingMaxValue)
        {
            PropRuleDouble propRule = base.GetPropRule<PropRuleDouble>();
            double minValue = GetMinValue(propRule, overridingMinValue);
            double maxValue = GetMaxValue(propRule, overridingMaxValue);
            return RandomValueGen.GetRandomDouble(minValue, maxValue);
        }

        public object GenerateValidValueGreaterThan(object minValue)
        {
            return this.GenerateValidValue((double?) minValue, null);
        }

        public object GenerateValidValueLessThan(object maxValue)
        {
            return this.GenerateValidValue(null, (double?) maxValue);
        }

        private static double GetMaxValue(IPropRuleComparable<double> propRule, double? overridingMaxValue)
        {
            return RandomValueGen.GetMaxValue(propRule, overridingMaxValue);
        }

        private static double GetMinValue(IPropRuleComparable<double> propRule, double? overridingMinValue)
        {
            return RandomValueGen.GetMinValue(propRule, overridingMinValue);
        }
    }
}

