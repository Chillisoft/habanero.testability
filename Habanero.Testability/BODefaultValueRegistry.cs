using System;
using System.Collections.Generic;
using System.Linq;
using Habanero.Base;
using Habanero.Base.Exceptions;

namespace Habanero.Testability
{
    /// <summary>
    /// This is a registry of the BOTestValues for various Properties and Single Relationships of a BusinessObject.
    /// This was creates so as to allow the Developer to <see cref="BOTestFactory{T}.SetValueFor{TReturn}"/> and 
    /// such that when the Object is created with the <see cref="BOTestFactory.CreateBusinessObject"/> etc or updated with
    /// <see cref="BOTestFactory.UpdateCompulsoryProperties"/> this will be the value used.
    /// </summary>
    public class BODefaultValueRegistry
    {
        private readonly Dictionary<string, object> _defaultValuesDictionary = new Dictionary<string, object>();
        private static BODefaultValueRegistry _boDefaultValueRegistry;

        /// <summary>
        /// Registers the Default value to be used for the Method
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="defaultValue"></param>
        public virtual void Register(string methodName, object defaultValue)
        {
            ClearPreviousInstances(methodName);
            this._defaultValuesDictionary.Add(methodName, defaultValue);  
        }

        /// <summary>
        /// Resolves the registered <see cref="BOTestFactory"/> if one is registered.
        /// Else tries to find a Sub Class of the Generic <see cref="BOTestFactory{TBO}"/> 
        ///   in the App Domain and returns an instance of it.
        /// else constructs the Generic <see cref="BOTestFactory{TBO}"/>
        /// </summary>
        /// <returns></returns>
        public virtual object Resolve(string methodName)
        {
            return this._defaultValuesDictionary.ContainsKey(methodName) 
                    ? this._defaultValuesDictionary[methodName] 
                    : null;
        }

        /// <summary>
        /// Returns True if a value is registered with the name <paramref name="methodName"/>
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public bool IsRegistered(string methodName)
        {
            return this._defaultValuesDictionary.ContainsKey(methodName);
        }
        private void ClearPreviousInstances(string methodName)
        {
            if (this._defaultValuesDictionary.ContainsKey(methodName))
            {
                this._defaultValuesDictionary.Remove(methodName);
            }
        }
        /// <summary>
        /// Returns the Singleton Registry Instance
        /// </summary>
        public static BODefaultValueRegistry Instance
        {
            get
            {
                if (_boDefaultValueRegistry == null) return (_boDefaultValueRegistry = new BODefaultValueRegistry());
                return _boDefaultValueRegistry;
            }
            set
            {
                _boDefaultValueRegistry = value;
            }
        }
    }
}