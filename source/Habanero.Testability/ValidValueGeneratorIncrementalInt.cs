#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
#endregion
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