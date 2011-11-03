#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
#endregion
using System.Collections.Generic;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;

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
                        Messages.You_cannot_use_a_ValidValueGeneratorFromList_for_generating_values_of_typeT,
                        typeof (T), valueDef.PropertyType);
                throw new HabaneroArgumentException(message +
                                                    valueDef.ClassName + "_" + valueDef.PropertyName + "'");
            }
        }
    }
}