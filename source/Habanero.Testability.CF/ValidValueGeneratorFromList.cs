using System.Collections.Generic;
using Habanero.Base;
using Habanero.Base.Exceptions;

namespace Habanero.Testability
{
    /// <summary>
    /// This will return an Item of {T} based on a randomly selected item
    /// from an available List of Items.
    /// If the list of available items is not set or is set to null via the constructor,
    ///  then the List of available items will be loaded using the LoadItems method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ValidValueGeneratorFromList<T> : ValidValueGenerator
    {
        //Dictionary of the next list pointer index for each SingleValueDef.
        private static readonly Dictionary<ISingleValueDef, int> _nextItemDictionaryRef = new Dictionary<ISingleValueDef, int>();

        public ValidValueGeneratorFromList(ISingleValueDef singleValueDef, IList<T> availableItems)
            : base(singleValueDef)
        {
            CheckPropertyTypeIsCorrect(singleValueDef);
            SetAvailableItems(availableItems);
        }

        public ValidValueGeneratorFromList(ISingleValueDef singleValueDef) : this(singleValueDef, null)
        {
        }

        public virtual IList<T> AvailableItemsList { get; protected set; }

        private void SetAvailableItems(IList<T> availableItems)
        {
            if (availableItems == null)
            {
                AvailableItemsList = LoadItems();
                return;
            }
            AvailableItemsList = availableItems;
        }

        protected abstract IList<T> LoadItems();


        public override object GenerateValidValue()
        {
            if (AvailableItemsList.Count == 0)
            {
                return CreateBO();
            }

            var nextNameReference = GetNextNameReference(this.SingleValueDef);
            return AvailableItemsList[nextNameReference];
        }

        protected abstract T CreateBO();


        protected virtual int GetNextNameReference(ISingleValueDef propDef)
        {
            if (!_nextItemDictionaryRef.ContainsKey(propDef)) _nextItemDictionaryRef.Add(propDef, 0);
            var nextNameReference = _nextItemDictionaryRef[propDef];
            nextNameReference++;
            if (nextNameReference >= AvailableItemsList.Count) nextNameReference = 0;
            _nextItemDictionaryRef[propDef] = nextNameReference;
            return nextNameReference;
        }

        protected void CheckPropertyTypeIsCorrect(ISingleValueDef valueDef)
        {
            if (!typeof (T).IsAssignableFrom(valueDef.PropertyType))
            {
                var message =
                    string.Format(
                        "You cannot use a ValidValueGeneratorFromList for generating values of type \'{0}\' since the Property is of type \'{1} : \'",
                        typeof (T), valueDef.PropertyType);
                throw new HabaneroArgumentException(message +
                                                    valueDef.ClassName + "_" + valueDef.PropertyName + "'");
            }
        }
    }
}