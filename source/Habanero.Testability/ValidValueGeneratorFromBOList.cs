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
using System;
using System.Collections.Generic;
using Habanero.Base;
using Habanero.Base.Exceptions;
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