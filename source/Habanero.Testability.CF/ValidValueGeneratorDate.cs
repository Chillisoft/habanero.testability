using Habanero.Base;
using Habanero.BO;
using System;
using Habanero.BO.Rules;

namespace Habanero.Testability.CF
{
    /// <summary>
    /// Generates a valid value for PropDef of type DateTime.
    /// </summary>
    public class ValidValueGeneratorDate : ValidValueGenerator, IValidValueGeneratorNumeric
    {
        /// <summary>
        /// Construct a Valid Value Generator with a PropDef
        /// </summary>
        /// <param name="propDef"></param>
        public ValidValueGeneratorDate(IPropDef propDef) : base(propDef)
        {
        }
        /// <summary>
        /// Generates a valid value taking into account only the <see cref="IPropRule"/>s. I.e. any <see cref="InterPropRule"/>s 
        /// will not be taken into account. The <see cref="IValidValueGeneratorNumeric"/>'s methods are used
        /// by the BOTestFactory to create valid values taking into account InterPropRules
        /// </summary>
        /// <returns></returns>
        public override object GenerateValidValue()
        {
            PropRuleDate propRule = base.GetPropRule<PropRuleDate>();
            return ((propRule == null) ? RandomValueGen.GetRandomDate() : RandomValueGen.GetRandomDate(propRule.MinValue, propRule.MaxValue));
        }

        private DateTime GenerateValidValue(DateTime? overridingMinValue, DateTime? overridingMaxValue)
        {
            PropRuleDate propRule = base.GetPropRule<PropRuleDate>();
            DateTime intMinValue = GetMinValue(propRule, overridingMinValue);
            DateTime intMaxValue = GetMaxValue(propRule, overridingMaxValue);
            return RandomValueGen.GetRandomDate(intMinValue, intMaxValue);
        }

        /// <summary>
        /// Generates a Valid Value taking <see cref="IPropRule"/> and minValue into 
        /// account.
        /// </summary>
        /// <param name="minValue"></param>
        /// <returns></returns>
        public object GenerateValidValueGreaterThan(object minValue)
        {
            return this.GenerateValidValue((DateTime?) minValue, null);
        }

        /// <summary>
        /// Generates a Valid Value taking <see cref="IPropRule"/> and maxValue into 
        /// account.
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public object GenerateValidValueLessThan(object maxValue)
        {
            return this.GenerateValidValue(null, (DateTime?) maxValue);
        }

        private static DateTime GetMaxValue(IPropRuleComparable<DateTime> propRule, DateTime? overridingMaxValue)
        {
            return RandomValueGen.GetMaxValue(propRule, overridingMaxValue);
        }

        private static DateTime GetMinValue(IPropRuleComparable<DateTime> propRule, DateTime? overridingMinValue)
        {
            return RandomValueGen.GetMinValue(propRule, overridingMinValue);
        }
    }
}

