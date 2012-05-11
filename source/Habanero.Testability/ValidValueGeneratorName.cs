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
using System.Collections.Generic;
using Habanero.Base;
using Habanero.Base.Exceptions;

namespace Habanero.Testability
{
    public class ValidValueGeneratorName : ValidValueGenerator
    {
        private readonly IPropDef _propDef;
        private static Dictionary<IPropDef, int>  _nextNameDictionaryRef = new Dictionary<IPropDef, int>();

        public ValidValueGeneratorName(IPropDef propDef, IList<string> names)
            : base(propDef)
        {
            if (names == null) throw new ArgumentNullException("names");
            _propDef = propDef;
            CheckPropertyTypeIsString(propDef);

            NameList = names;
        }

        private static void CheckPropertyTypeIsString(IPropDef propDef)
        {
            if (propDef.PropertyType != typeof (string))
            {
                throw new HabaneroArgumentException(
                    "You cannot use a ValidValueGeneratorName for generating values for a property that is not a string. For Prop : '" +
                    propDef.ClassName + "_" + propDef.PropertyName + "'");
            }
        }

        public IList<string> NameList { get; private set; }

        public override object GenerateValidValue()
        {
            if (NameList.Count == 0)
            {
                //Generates a random string conforming to the rules on the PropDef
                return new ValidValueGeneratorString(_propDef).GenerateValidValue();
            }

            var nextNameReference = GetNextNameReference(_propDef);
            return NameList[nextNameReference];
        }

        private int GetNextNameReference(IPropDef propDef)
        {
            if(!_nextNameDictionaryRef.ContainsKey(propDef)) _nextNameDictionaryRef.Add(propDef, 0);
            var nextNameReference = _nextNameDictionaryRef[propDef];
            nextNameReference++;
            if (nextNameReference >= NameList.Count) nextNameReference = 0;
            _nextNameDictionaryRef[propDef] = nextNameReference;
            return nextNameReference;
        }
    }
}