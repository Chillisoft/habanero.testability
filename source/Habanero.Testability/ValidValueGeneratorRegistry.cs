using System;
using System.Collections.Generic;
using System.Linq;
using Habanero.Base;
using Habanero.Base.Exceptions;

namespace Habanero.Testability
{
    /// <summary>
    /// This is a registry of the BOTestFactories for various Business Object types.
    /// This was creates so as to allow the Developer to override the Generic <see cref="BOTestFactory{T}"/>.<br/>
    /// E.g. if the Business object has specific rules or things that must be set to ensure that it is a valid 
    /// saveable business object. This specialised <see cref="BOTestFactory{T}"/> must then be 
    /// registered with this Registery.<br/>
    /// When any other TestBusiness Object has a compulsory relationship involving the Business Object with the Specialised
    /// BOTestFactory the specialised factory will be used instead of the generalised Test factory.
    /// This allows a situation such as Creating a valid Asset wich has a compulsory relationship to AssetType.
    /// For a valid Asset to be created a valid Asset Type is required. If the Asset type has a specialised
    /// BOTestFactoryAssetType then the asset must use this specialised factory else it will not be constructed in a valid.
    /// manner.
    /// </summary>
    public class ValidValueGeneratorRegistry
    {
        private readonly Dictionary<Type, Type> _validValueGenTypes = new Dictionary<Type, Type>();
        private static readonly ValidValueGeneratorRegistry _boValidValueRegistry = new ValidValueGeneratorRegistry();

        public ValidValueGeneratorRegistry()
        {
            RegisterDefaultGenerators();
        }

        private void RegisterDefaultGenerators()
        {
            this.Register<string, ValidValueGeneratorString>();
            this.Register<Guid, ValidValueGeneratorGuid>();
            this.Register<int, ValidValueGeneratorInt>();
            this.Register<bool, ValidValueGeneratorBool>();
            this.Register<decimal, ValidValueGeneratorDecimal>();
            this.Register<DateTime, ValidValueGeneratorDate>();
            this.Register<double, ValidValueGeneratorDouble>();
            // Removed for this version compiled against Habanero 2.4
            //this.Register<long, ValidValueGeneratorLong>();
        }

        private void ClearPreviousInstances(Type boType)
        {
            if (this._validValueGenTypes.ContainsKey(boType))
            {
                this._validValueGenTypes.Remove(boType);
            }
        }

        /// <summary>
        /// Registeres a specific type <typeparamref name="TValidValuGenType"/> of <see cref="BOTestFactory"/> for the specified 
        /// <see cref="IBusinessObject"/> <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValidValuGenType"></typeparam>
        public virtual void Register<T, TValidValuGenType>()
            where TValidValuGenType : ValidValueGenerator
        {
            this.Register(typeof(T), typeof(TValidValuGenType));
        }
        /// <summary>
        /// Registeres a specific type of <see cref="BOTestFactory"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        ///
        public virtual void Register<T>(Type type)
        {
            this.Register(typeof(T), type);
        }

        private void Register(Type type, Type validValuGenType)
        {
            ValidateGeneratorType(type, validValuGenType);
            this.ClearPreviousInstances(type);
            this._validValueGenTypes.Add(type, validValuGenType);
        }

        /// <summary>
        /// Resolves the registered <see cref="BOTestFactory"/> if one is registered.
        /// Else tries to find a Sub Class of the Generic <see cref="BOTestFactory{TBO}"/> 
        ///   in the App Domain and returns an instance of it.
        /// else constructs the Generic <see cref="BOTestFactory{TBO}"/>
        /// </summary>
        /// <returns></returns>
        public virtual ValidValueGenerator Resolve(IPropDef propDef)
        {
            if (propDef == null)
            {
                throw new ArgumentNullException("propDef");
            }
            var valueType = propDef.PropertyType;

            if (this._validValueGenTypes.ContainsKey(valueType))
            {
                Type validValueGenType = this._validValueGenTypes[valueType];
                return (ValidValueGenerator)Activator.CreateInstance(validValueGenType, propDef);
            }
            return null;
        }

        private static void ValidateGeneratorType(Type boType, Type factoryType)
        {
            if (factoryType == null)
            {
                throw new HabaneroApplicationException(string.Format("A ValidValueGenerator is being Registered for '{0}' but the ValidValueGenerator is Null", boType.Name));
            }
            if (!typeof(ValidValueGenerator).IsAssignableFrom(factoryType))
            {
                throw new HabaneroApplicationException(string.Format("A ValidValueGenerator is being Registered for '{0}' but the ValidValueGenerator is not of Type ValidValueGenerator", boType.Name));
            }
        }
        /// <summary>
        /// Returns the Singleton Registry Instance
        /// </summary>
        public static ValidValueGeneratorRegistry Instance
        {
            get
            {
                return _boValidValueRegistry;
            }
        }
    }
}