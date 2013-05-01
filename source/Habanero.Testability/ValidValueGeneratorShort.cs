using Habanero.BO;
using Habanero.Base;

namespace Habanero.Testability
{
    /// <summary>
    /// Generates a Valid Value for an Integer (int or Int32) based on the min or max value in the PropDef
    /// </summary>
    public class ValidValueGeneratorShort : ValidValueGenerator, IValidValueGeneratorNumeric
    {
        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="propDef"></param>
        public ValidValueGeneratorShort(IPropDef propDef) : base(propDef)
        {
        }

        public override object GenerateValidValue()
        {
            return this.GenerateValidValue(null, null);
        }

        private short GenerateValidValue(short? overridingMinValue, short? overridingMaxValue)
        {
            var propRule = base.GetPropRule<PropRuleShort>();
            var intMinValue = GetMinValue(propRule, overridingMinValue);
            var intMaxValue = GetMaxValue(propRule, overridingMaxValue);
            return RandomValueGen.GetRandomShort(intMinValue, intMaxValue);
        }

        public object GenerateValidValueGreaterThan(object minValue)
        {
            return this.GenerateValidValue((short?)minValue, null);
        }

        public object GenerateValidValueLessThan(object maxValue)
        {
            return this.GenerateValidValue(null, (short?)maxValue);
        }

        private static short GetMaxValue(IPropRuleComparable<short> propRule, short? overridingMaxValue)
        {
            return RandomValueGen.GetMaxValue(propRule, overridingMaxValue);
        }

        private static short GetMinValue(IPropRuleComparable<short> propRule, short? overridingMinValue)
        {
            return RandomValueGen.GetMinValue(propRule, overridingMinValue);
        }
    }
}