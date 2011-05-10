using Habanero.Base;
using System;

namespace Habanero.Testability.CF
{
    /// <summary>
    /// The IBOFactory is an interface for creating a Standard Business Object.
    /// This is used so as to prevent the random construction of business objects in code
    /// all over the project as well as to allow the easy mocking/Stubbing or faking of business
    /// object creation when requried for testing.
    /// </summary>
    public interface IBOFactory
    {
        /// <summary>
        /// Creates a business object of Type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T CreateBusinessObject<T>() where T: IBusinessObject;
        /// <summary>
        /// Creates of business object of type <paramref name="type"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IBusinessObject CreateBusinessObject(Type type);
    }
}

