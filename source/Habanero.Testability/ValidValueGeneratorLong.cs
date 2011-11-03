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
using System;
using Habanero.Base;
using Habanero.BO;

namespace Habanero.Testability
{
    public class ValidValueGeneratorLong : ValidValueGenerator, IValidValueGeneratorNumeric
    {
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

        private long? ConvertToLong(object value)
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