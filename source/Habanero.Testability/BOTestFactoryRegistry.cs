using System;
using System.Collections.Generic;
using System.Linq;
using Habanero.Base;
using Habanero.Base.Exceptions;

namespace Habanero.Testability
{
    /// <summary>
    /// This is a registry of the BOTestFactories for various Business Object types.
    /// This was creates so as to allow the Developer to override the Generic <see cref="BOTestFactory{TBO}"/>.<br/>
    /// E.g. if the Business object has specific rules or things that must be set to ensure that it is a valid 
    /// saveable business object. This specialised <see cref="BOTestFactory{TBO}"/> must then be 
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
        private readonly Dictionary<Type, Type> _boTestFactoryTypes = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, BOTestFactory> _boTestFactoryInstances = new Dictionary<Type, BOTestFactory>();
        private static BOTestFactoryRegistry _boTestFactoryRegistry;

        private void ClearPreviousInstances(Type boType)
        {
            if (this._boTestFactoryTypes.ContainsKey(boType))
            {
                this._boTestFactoryTypes.Remove(boType);
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
        public virtual void Register<TBO, TBOTestFactory>() where TBO: class, IBusinessObject where TBOTestFactory: BOTestFactory<TBO>
        {
            this.Register(typeof(TBO), typeof(TBOTestFactory));
        }

        /// <summary>
        /// Registers an instance of <see cref="BOTestFactory"/>
        /// </summary>
        /// <typeparam name="TBO"></typeparam>
        /// <param name="boTestFactory"></param>
        public virtual void Register<TBO>(BOTestFactory boTestFactory)
        {
            Type boType = typeof(TBO);
            this.ClearPreviousInstances(boType);
            this._boTestFactoryInstances.Add(boType, boTestFactory);
        }
		/// <summary>
        /// Registeres a specific type of <see cref="BOTestFactory"/>
        /// </summary>
        /// <typeparam name="TBO"></typeparam>
        ///
        public virtual void Register<TBO>(Type type) where TBO : class, IBusinessObject
        {
            this.Register(typeof(TBO), type);
        }

        private void Register(Type boType, Type factoryType)
        {
            ValidateFactoryType(boType, factoryType);
            this.ClearPreviousInstances(boType);
            this._boTestFactoryTypes.Add(boType, factoryType);
        }
        /// <summary>
        /// Resolves the registered <see cref="BOTestFactory"/> if one is registered else resolves the 
        /// Generid <see cref="BOTestFactory{TBO}"/>
        /// </summary>
        /// <typeparam name="TBO"></typeparam>
        /// <returns></returns>
        public virtual BOTestFactory<TBO> Resolve<TBO>() where TBO : class, IBusinessObject
        {
            Type typeOfBO = typeof(TBO);
            return (BOTestFactory <TBO>) this.Resolve(typeOfBO);
        }
        /// <summary>
        /// Resolves the registered <see cref="BOTestFactory"/> if one is registered.
        /// Else tries to find a Sub Class of the Generic <see cref="BOTestFactory{TBO}"/> 
        ///   in the App Domain and returns an instance of it.
        /// else constructs the Generic <see cref="BOTestFactory{TBO}"/>
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
            if (this._boTestFactoryTypes.ContainsKey(typeOfBO))
            {
                boTestFactoryType = this._boTestFactoryTypes[typeOfBO];
                return (BOTestFactory) Activator.CreateInstance(boTestFactoryType);
            }
            boTestFactoryType = typeof(BOTestFactory<>).MakeGenericType(new[] { typeOfBO });
            Type factoryType = boTestFactoryType;
            //Excludes Types created via RhinoMocks.
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
        /// Returns the Singleton Registry Instance
        /// </summary>
        public static BOTestFactoryRegistry Instance
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
        public virtual void ClearAll()
        {
            this._boTestFactoryInstances.Clear();
            this._boTestFactoryTypes.Clear();
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

