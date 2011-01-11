using System;
using System.Collections.Generic;
using System.Linq;
using AutoMappingHabanero;
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
    public class BOTestFactoryRegistry
    {
        private readonly Dictionary<Type, Type> _boTestFactories = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, BOTestFactory> _boTestFactoryInstances = new Dictionary<Type, BOTestFactory>();
        private static BOTestFactoryRegistry _boTestFactoryRegistry;

        private void ClearPreviousInstances(Type boType)
        {
            if (this._boTestFactories.ContainsKey(boType))
            {
                this._boTestFactories.Remove(boType);
            }
            if (this._boTestFactoryInstances.ContainsKey(boType))
            {
                this._boTestFactoryInstances.Remove(boType);
            }
        }

        /// <summary>
        /// Registeres a specific type <typeparamref name="TBOTestFactory"/> of <see cref="BOTestFactory"/> for the specified 
        /// <see cref="IBusinessObject"/> <typeparamref name="TBO"/>
        /// </summary>
        /// <typeparam name="TBO"></typeparam>
        /// <typeparam name="TBOTestFactory"></typeparam>
        public virtual void Register<TBO, TBOTestFactory>() where TBO: IBusinessObject where TBOTestFactory: BOTestFactory<TBO>
        {
            this.Register(typeof(TBO), typeof(TBOTestFactory));
        }

        /// <summary>
        /// Registers an instance of <see cref="BOTestFactory"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="boTestFactoryFakeBO"></param>
        public virtual void Register<T>(BOTestFactory boTestFactoryFakeBO)
        {
            Type boType = typeof(T);
            this.ClearPreviousInstances(boType);
            this._boTestFactoryInstances.Add(boType, boTestFactoryFakeBO);
        }
		/// <summary>
        /// Registeres a specific type of <see cref="BOTestFactory"/>
        /// </summary>
        /// <typeparam name="TBO"></typeparam>
        ///
        public virtual void Register<TBO>(Type type) where TBO: IBusinessObject
        {
            this.Register(typeof(TBO), type);
        }

        private void Register(Type boType, Type factoryType)
        {
            ValidateFactoryType(boType, factoryType);
            this.ClearPreviousInstances(boType);
            this._boTestFactories.Add(boType, factoryType);
        }
        /// <summary>
        /// Resolves the registered <see cref="BOTestFactory"/> if one is registered else resolves the 
        /// Generid <see cref="BOTestFactory{TBO}"/>
        /// </summary>
        /// <typeparam name="TBO"></typeparam>
        /// <returns></returns>
        public virtual BOTestFactory Resolve<TBO>()
        {
            Type typeOfBO = typeof(TBO);
            return this.Resolve(typeOfBO);
        }
        /// <summary>
        /// Resolves the registered <see cref="BOTestFactory"/> if one is registered else resolves the 
        /// Generid <see cref="BOTestFactory{TBO}"/>
        /// </summary>
        /// <returns></returns>
        public virtual BOTestFactory Resolve(Type typeOfBO)
        {
            Type boTestFactoryType;
            if (typeOfBO == null)
            {
                throw new ArgumentNullException("typeOfBO");
            }
            if (this._boTestFactoryInstances.ContainsKey(typeOfBO))
            {
                BOTestFactory boTestFactoryInstance = this._boTestFactoryInstances[typeOfBO];
                if (boTestFactoryInstance != null)
                {
                    return boTestFactoryInstance;
                }
            }
            if (this._boTestFactories.ContainsKey(typeOfBO))
            {
                boTestFactoryType = this._boTestFactories[typeOfBO];
                return (BOTestFactory) Activator.CreateInstance(boTestFactoryType);
            }
            boTestFactoryType = typeof(BOTestFactory<>).MakeGenericType(new[] { typeOfBO });
            Type factoryType = boTestFactoryType;
            AppDomainTypeSource typeSource = new AppDomainTypeSource(type => !type.Name.Contains("Proxy") );
            Type firstSubType = typeSource.GetTypes()
                    .Where(factoryType.IsAssignableFrom)
                    .FirstOrDefault();
            if (firstSubType != null)
            {
                boTestFactoryType = firstSubType;
            }
            return (BOTestFactory) Activator.CreateInstance(boTestFactoryType);
        }

        private static void ValidateFactoryType(Type boType, Type factoryType)
        {
            if (factoryType == null)
            {
                throw new HabaneroApplicationException(string.Format("A BOTestFactory is being Registered for '{0}' but the BOTestFactory is Null", boType.Name));
            }
            if (!typeof(BOTestFactory).IsAssignableFrom(factoryType))
            {
                throw new HabaneroApplicationException(string.Format("A BOTestFactory is being Registered for '{0}' but the BOTestFactory is not of Type BOTestFactory", boType.Name));
            }
        }
        /// <summary>
        /// Returns the Singleton Registry
        /// </summary>
        public static BOTestFactoryRegistry Registry
        {
            get
            {
                if (_boTestFactoryRegistry == null) return (_boTestFactoryRegistry = new BOTestFactoryRegistry());
                return _boTestFactoryRegistry;
            }
            set
            {
                _boTestFactoryRegistry = value;
            }
        }
    }

    /// <summary>
    /// Gets the Types for the Currently Loaded App Domain
    /// </summary>
    internal class AppDomainTypeSource
    {
        public AppDomainTypeSource(Func<Type, bool> where)
        {
            this.Where = where;
        }

        public IEnumerable<Type> GetTypes()
        {
            return ((this.Where == null) ? TypesImplementing() : TypesImplementing().Where(this.Where));
        }

        private static IEnumerable<Type> TypesImplementing()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly1 => assembly1.GetTypes())
                .Where(type1 => !type1.IsInterface && !type1.IsAbstract);
        }

        private Func<Type, bool> Where { get; set; }
    }

}

