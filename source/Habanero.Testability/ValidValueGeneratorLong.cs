using System;
using Habanero.Base;
using Habanero.BO;

namespace Habanero.Testability
{
    /// <summary>
    /// Created a valid long where the long can be limited to a valid range
    /// the valid range can result from either the propdef max min
    /// or by overriding this.
    /// For more details on overriding the max and min See <see cref="IValidValueGeneratorNumeric"/>
    /// </summary>
    public class ValidValueGeneratorLong : ValidValueGenerator, IValidValueGeneratorNumeric
    {
        /// <summary>
        /// Construct for this propdef
        /// </summary>
        /// <param name="propDef"></param>
        public ValidValueGeneratorLong(IPropDef propDef) : base(propDef)
        {
        }

        public override object GenerateValidValue()
        {
            return this.GenerateValidValue(null, null);
        }

        private long GenerateValidValue(long? overridingMinValue, long? overridingMaxValue)
        {
            PropRuleLong propRule = base.GetPropRule<PropRuleLong>();
            long minValue = GetMinValue(propRule, overridingMinValue);
            long maxValue = GetMaxValue(propRule, overridingMaxValue);
            return RandomValueGen.GetRandomLong(minValue, maxValue);
        }

        public object GenerateValidValueGreaterThan(object minValue)
        {
            return this.GenerateValidValue(ConvertToLong(minValue), null);
        }

        public object GenerateValidValueLessThan(object maxValue)
        {
            return this.GenerateValidValue(null, ConvertToLong(maxValue));
        }

        private static long? ConvertToLong(object value)
        {
            return value == null? (long?)null: Convert.ToInt64(value);
        }

        private static long GetMaxValue(IPropRuleComparable<long> propRule, long? overridingMaxValue)
        {
            return RandomValueGen.GetMaxValue(propRule, overridingMaxValue);
        }

        private static long GetMinValue(IPropRuleComparable<long> propRule, long? overridingMinValue)
        {
            return RandomValueGen.GetMinValue(propRule, overridingMinValue);
        }
    }
}