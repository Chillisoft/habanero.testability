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
namespace Habanero.Testability
{
    using Habanero.Base;
    using Habanero.BO;
    using System;

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

