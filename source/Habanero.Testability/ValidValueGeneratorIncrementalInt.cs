using System.Collections.Generic;
using Habanero.Base;
using Habanero.BO;

namespace Habanero.Testability
{
    public class ValidValueGeneratorIncrementalInt : ValidValueGenerator, IValidValueGeneratorNumeric
    {
        private static Dictionary<ISingleValueDef, int> _latestValue = new Dictionary<ISingleValueDef, int>();
        private int _nextValue;
        public ValidValueGeneratorIncrementalInt(IPropDef propDef) : base(propDef)
        {
           if(!_latestValue.ContainsKey(propDef))
           {
               _latestValue.Add(propDef, 0);
           }
            _latestValue.TryGetValue(propDef, out _nextValue);
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
            
            _latestValue.Remove(base.SingleValueDef);
            if (_nextValue < intMinValue) _nextValue = intMinValue;
            if (_nextValue > intMaxValue) _nextValue = intMinValue;
            var returnValue = _nextValue;
            _latestValue.Add(base.SingleValueDef, ++_nextValue);
            return returnValue;
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