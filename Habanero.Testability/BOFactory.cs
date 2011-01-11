namespace Habanero.Testability
{
    using Habanero.Base;
    using Habanero.Base.Exceptions;
    using System;

    public class BOFactory : IBOFactory
    {
        public virtual T CreateBusinessObject<T>() where T: IBusinessObject
        {
            return (T) this.CreateBusinessObject(typeof(T));
        }

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

