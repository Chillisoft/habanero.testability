using System;
using System.Collections.Generic;
using Habanero.Base;
using Habanero.Base.Exceptions;

namespace Habanero.Testability
{
    /// <summary>
    /// This is a registry of the ValidValueGenerators Registered for a specific PropertyDefinition <see cref="ISingleValueDef"/>
    /// of a particular Property for a specified Business Object type.
    /// This was creates so as to allow the Developer to override the Generic <see cref="ValidValueGenerator"/>.<br/>
    /// E.g. if the Business object Property has specific rules or things that must be set to ensure that it is a valid 
    /// saveable business object. Then you can register a specialised Valid Value Generator that will generate the appropriate
    ///  value for that Business Object Property.<br/>
    /// This is also used behind the scenes when you want to generate a business object with a specified non compulsory property
    /// in a particular case. Then a Valid Value Generator is registered for that <see cref="ISingleValueDef"/> and the non compulsory prop
    /// will have a value set. For more details see <see cref="BOTestFactory"/>
    /// </summary>
    public class BOPropValueGeneratorRegistry
    {
        private readonly Dictionary<ISingleValueDef, Type> _validValueGenTypesPerPropDef = new Dictionary<ISingleValueDef, Type>();
        private static BOPropValueGeneratorRegistry _boValueGeneratorRegistry;

        private static string GetClassType(ISingleValueDef propDef)
        {
            return propDef.ClassDef == null ? "" : propDef.ClassDef.ClassNameFull;
        }

        private void ClearPreviousInstances(ISingleValueDef propDef)
        {
            if (this._validValueGenTypesPerPropDef.ContainsKey(propDef))
            {
                this._validValueGenTypesPerPropDef.Remove(propDef);
            }
        }

        /// <summary>
        /// Register a Valid Value Generator to be used for generating values for a specified PropDef.
        /// </summary>
        /// <param name="propDef"></param>
        /// <param name="validValuGenType"></param>
        public virtual void Register(ISingleValueDef propDef, Type validValuGenType)
        {
            string boTypeName = GetClassType(propDef);
            ValidateGeneratorType(validValuGenType, boTypeName);
            this.ClearPreviousInstances(propDef);
            _validValueGenTypesPerPropDef.Add(propDef, validValuGenType);
        }

        /// <summary>
        /// Resolves the registered <see cref="ValidValueGenerator"/> for the PropDef if one is registered.
        /// Else tries to find a <see cref="ValidValueGenerator"/> for the specified PropDefs Property Type 
        /// using the <see cref="ValidValueGeneratorRegistry"/>
        /// </summary>
        /// <returns></returns>
        public virtual ValidValueGenerator Resolve(ISingleValueDef propDef)
        {
            if (propDef == null)
            {
                throw new ArgumentNullException("propDef");
            }
            if(_validValueGenTypesPerPropDef.ContainsKey(propDef))
            {
                Type validValueGenType = this._validValueGenTypesPerPropDef[propDef];
                return (ValidValueGenerator)Activator.CreateInstance(validValueGenType, propDef);
            }

            return ValidValueGeneratorRegistry.Instance.Resolve(propDef);
        }
        /// <summary>
        /// Returns True if a value Gen is registered with the propDef <paramref name="propDef"/>
        /// </summary>
        /// <param name="propDef"></param>
        /// <returns></returns>
        public bool IsRegistered(ISingleValueDef propDef)
        {
            return this._validValueGenTypesPerPropDef.ContainsKey(propDef);
        }
        private static void ValidateGeneratorType(Type factoryType, string typeName)
        {
            if (factoryType == null)
            {
                throw new HabaneroApplicationException(string.Format("A ValidValueGenerator is being Registered for '{0}' but the ValidValueGenerator is Null", typeName));
            }
            if (!typeof(ValidValueGenerator).IsAssignableFrom(factoryType))
            {
                throw new HabaneroApplicationException(string.Format("A ValidValueGenerator is being Registered for '{0}' but the ValidValueGenerator is not of Type ValidValueGenerator", typeName));
            }
        }
        /// <summary>
        /// Returns the Singleton Registry Instance
        /// </summary>
        public static BOPropValueGeneratorRegistry Instance
        {
            get
            {
                if (_boValueGeneratorRegistry == null) _boValueGeneratorRegistry = new BOPropValueGeneratorRegistry();
                return _boValueGeneratorRegistry;
            }
            set
            {
                _boValueGeneratorRegistry = value;
            }
        }


        public void ClearAll()
        {
            _validValueGenTypesPerPropDef.Clear();
        }
    }
}