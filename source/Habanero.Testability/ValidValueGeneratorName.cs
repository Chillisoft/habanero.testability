using System;
using System.Collections.Generic;
using Habanero.Base;
using Habanero.Base.Exceptions;

namespace Habanero.Testability
{
    public class ValidValueGeneratorName : ValidValueGenerator
    {
        private static int _nextNameReference;

        public ValidValueGeneratorName(IPropDef propDef, IList<string> names)
            : base(propDef)
        {
            if (names == null) throw new ArgumentNullException("names");
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
                return new ValidValueGeneratorString(this.PropDef).GenerateValidValue();
            }

            if (_nextNameReference >= NameList.Count) _nextNameReference = 0;
            return NameList[_nextNameReference++];
        }
    }
}