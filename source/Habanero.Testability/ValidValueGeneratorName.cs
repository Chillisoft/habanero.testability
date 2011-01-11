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