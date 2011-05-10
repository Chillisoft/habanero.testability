using Habanero.Base;
using Habanero.Base.Exceptions;
using System;

namespace Habanero.Testability.CF
{
    /// <summary>
    /// A generalised factory for creating any habanero business object.
    /// </summary>
    public class BOFactory : IBOFactory
    {
        /// <summary>
        /// Creates a business object of Type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T CreateBusinessObject<T>() where T: IBusinessObject
        {
            return (T) this.CreateBusinessObject(typeof(T));
        }
        /// <summary>
        /// Creates the specified business object of tppe <paramref name="type"/>
        /// </summary>
        /// <param name="type">The type of business object being created.</param>
        /// <returns>the created BO</returns>
        public IBusinessObject CreateBusinessObject(Type type)
        {
            if (!typeof(IBusinessObject).IsAssignableFrom(type))
            {
                throw new HabaneroDeveloperException("The BOFactory.CreateBusinessObject was called with Type that does not implement IBusinessObject");
            }
            return (Activator.CreateInstance(type) as IBusinessObject);
        }
    }
}

