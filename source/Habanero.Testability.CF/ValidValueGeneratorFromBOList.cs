using System.Collections.Generic;
using Habanero.Base;
using Habanero.BO;

namespace Habanero.Testability
{
    /// <summary>
    /// This will return an BusinessObject based on a randomly selected <see cref="IBusinessObject"/> 
    /// from an available List of BusinessObjects.
    /// If the list of available items is not set or is set to null via the constructor,
    ///  then the List of available items will be loaded from the DataStore.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValidValueGeneratorFromBOList<T> : ValidValueGeneratorFromList<T> where T : class, IBusinessObject, new()
    {
        public ValidValueGeneratorFromBOList(ISingleValueDef singleValueDef, IList<T> availableItems)
            : base(singleValueDef, availableItems)
        {
        }

        public ValidValueGeneratorFromBOList(ISingleValueDef singleValueDef) : base(singleValueDef)
        {
        }
        protected override IList<T> LoadItems()
        {
            var availableItemsList = new BusinessObjectCollection<T>();
            availableItemsList.LoadAll();
            return availableItemsList;
        }

        protected override T CreateBO()
        {
            return BOTestFactoryRegistry.Instance.Resolve<T>().CreateSavedBusinessObject();
        }
    }
}