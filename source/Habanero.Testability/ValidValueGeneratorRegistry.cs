using System;
using System.Collections.Generic;
using System.Linq;
using Habanero.Base;
using Habanero.Base.Exceptions;

namespace Habanero.Testability
{
    /// <summary>
    /// This is a registry of the Valid Value Generators to be used for Generating Various Data Types e.g. String, Guid.
    /// This was creates so as to allow the Developer to override the <see cref="ValidValueGenerator"/> for a specified type.<br/>
    /// E.g. If you want to generate all strings in the entire project based on some other algorithm (Currently using A Guid) then
    /// you can replace it by registering. Also if you have additional types that you want to support as a default e.g. 
    /// If you create a value type such as Address that a number of properties in various business objects in your project can use
    /// then you can register the ValidValueGenerator for Address.
    /// As Testability supports additional types e.g. TimeSpan Valid Value Generators can also be added for these.
    /// </summary>
    public class ValidValueGeneratorRegistry
    {
        private readonly Dictionary<Type, Type> _validValueGenTypes = new Dictionary<Type, Type>();
        private static ValidValueGeneratorRegistry _boValidValueRegistry;
        /// <summary>
        /// Construct ValidValueGenRegistry
        /// </summary>
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
            this.Register<long, ValidValueGeneratorLong>();
        }

        private void ClearPreviousInstances(Type boType)
        {
            if (this._validValueGenTypes.ContainsKey(boType))
            {
                this._validValueGenTypes.Remove(boType);
            }
        }

        /// <summary>
        /// Registeres a specific type <typeparamref name="TValidValuGenType"/> of <see cref="ValidValueGenerator"/> for the specified 
        /// Type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValidValuGenType"></typeparam>
        public virtual void Register<T, TValidValuGenType>()
            where TValidValuGenType : ValidValueGenerator
        {
            this.Register(typeof(T), typeof(TValidValuGenType));
        }

        /// <summary>
        /// Registeres a specific type <pararef name="type"/> of <see cref="ValidValueGenerator"/> for the 
        /// Type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        ///
        public virtual void Register<T>(Type type)
        {
            this.Register(typeof(T), type);
        }

        private void Register(Type type, Type validValuGenType)
        {
            ValidateGeneratorType(validValuGenType, type.Name);
            this.ClearPreviousInstances(type);
            this._validValueGenTypes.Add(type, validValuGenType);
        }



        /// <summary>
        /// Resolves the registered <see cref="ValidValueGenerator"/> if one is registered.
        /// Else returns null./>
        /// </summary>
        /// <returns></returns>
        public virtual ValidValueGenerator Resolve(ISingleValueDef propDef)
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

        private static readonly object _padlock = new object();
        /// <summary>
        /// Returns the Singleton Registry Instance
        /// </summary>
        public static ValidValueGeneratorRegistry Instance
        {
            get
            {
                lock (_padlock)
                {
                    return _boValidValueRegistry ?? (_boValidValueRegistry = new ValidValueGeneratorRegistry());
                }
            }
        }
    }
}