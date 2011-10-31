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
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Habanero.Base;
using Habanero.BO;

namespace Habanero.Testability.Helpers
{
    /// <summary>
    /// This is a Fake BO which is used for testing.
    /// </summary>
    [Serializable]
    public class FakeBusinessObjectCollection<TBusinessObject>
        : BusinessObjectCollection<TBusinessObject>
        where TBusinessObject : class, IBusinessObject, new()
    {
        #region StronglyTypedComparer

        #endregion//StronglyTypedComparer

        /*
        private IClassDef _boClassDef;

        private ISelectQuery _selectQuery;*/

        /// <summary>
        /// Default constructor. 
        /// The <see cref="IClassDef"/> will be implied from <typeparamref name="TBusinessObject"/> and the Current Database Connection will be used.
        /// </summary>
        public FakeBusinessObjectCollection()
            : this(null, null)
        {
        }

        /// <summary>
        /// Use this constructor if you will only know <typeparamref name="TBusinessObject"/> at run time - <see cref="BusinessObject"/> will be the generic type
        /// and the objects in the collection will be determined from the <see cref="IClassDef"/> passed in.
        /// </summary>
        /// <param name="classDef">The <see cref="IClassDef"/> of the objects to be contained in this collection</param>
        public FakeBusinessObjectCollection(IClassDef classDef)
            : this(classDef, null)
        {
        }

        /// <summary>
        /// Constructor to initialize a new collection with a
        /// class definition provided by an existing business object
        /// </summary>
        /// <param name="bo">The business object whose class definition
        /// is used to initialize the collection</param>
        [Obsolete(
            "Please initialize with a ClassDef instead.  This option will be removed in later versions of Habanero."
            )]
        public FakeBusinessObjectCollection(TBusinessObject bo)
            : this(null, bo)
        {
        }


        // ReSharper disable UnusedParameter.Local
        private FakeBusinessObjectCollection(IClassDef classDef, TBusinessObject sampleBo)
        {
        }
        // ReSharper restore UnusedParameter.Local

        /// <summary>
        /// Reconstitutes the collection from a stream that was serialized.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected FakeBusinessObjectCollection(SerializationInfo info, StreamingContext context)
        {
        }


        // ReSharper disable VirtualMemberNeverOverriden.Global
        /// <summary>
        /// Adds a business object to the collection
        /// </summary>
        /// <param name="bo">The business object to add</param>
        /// <exception cref="ArgumentNullException"><c>bo</c> is null.</exception>
        public override void Add(TBusinessObject bo)
        {
            this._boCol.Add(bo);
        }

        /// <summary>
        /// Refreshes the business objects in the collection
        /// </summary>
        [ReflectionPermission(SecurityAction.Demand)]
        public override void Refresh()
        {
        }

        #region Load Methods

        public override void LoadWithLimit(string searchCriteria, string orderByClause, int limit)
        {
            Refresh();
        }


        public override void LoadWithLimit(Criteria searchExpression, string orderByClause, int limit)
        {
            Refresh();
        }

        public override void LoadWithLimit(Criteria searchCriteria, IOrderCriteria orderByClause,
                                           int firstRecordToLoad, int numberOfRecordsToLoad,
                                           out int totalNoOfRecords)
        {
            this.SelectQuery.Criteria = searchCriteria;
            this.SelectQuery.OrderCriteria = orderByClause;
            this.SelectQuery.FirstRecordToLoad = firstRecordToLoad;
            this.SelectQuery.Limit = numberOfRecordsToLoad;
            Refresh();
            totalNoOfRecords = TotalCountAvailableForPaging;
        }

        public override void LoadWithLimit(string searchCriteria, string orderByClause,
                                           int firstRecordToLoad, int numberOfRecordsToLoad,
                                           out int totalNoOfRecords)
        {
            Refresh();
            totalNoOfRecords = 0;
        }

        #endregion

        /// <summary>
        /// Clears the collection
        /// </summary>
        public override void Clear()
        {
            _boCol.Clear();
        }

        public override bool Remove(TBusinessObject bo)
        {
            this._boCol.Remove(bo);
            return true;
        }

        public override TBusinessObject Find(Guid key)
        {
            return this._boCol.FirstOrDefault(businessObject => businessObject.ID.GetAsGuid() == key);
        }

        public override void Sort(string propertyName, bool isBoProperty, bool isAscending)
        {
        }


        /*        /// <summary>
                /// Sorts the Collection by the Order Criteria Set up during the Loading of this collection.
                /// For <see cref="RelatedBusinessObjectCollection{TBusinessObject}"/>'s this will be the 
                /// Order Criteria set up in the orderBy in the <see cref="IClassDef"/> for 
                /// the <typeparamref name="TBusinessObject"/> type (i.e. in the <c>ClassDef.xml</c>).
                /// </summary>
                public void Sort()
                {
                    if (this.SelectQuery.OrderCriteria.Fields.Count > 0)
                    {
                        _boCol.Sort(new StronglyTypedComperer<TBusinessObject>(this.SelectQuery.OrderCriteria));
                    }
                }*/


        /// <summary>
        /// Returns a list containing all the objects sorted by the property
        /// name and in the order specified
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="isAscending">True for ascending, false for descending
        /// </param>
        /// <returns>Returns a sorted list</returns>
        public override List<TBusinessObject> GetSortedList(string propertyName, bool isAscending)
        {
            return this._boCol;
        }


        /// <summary>
        /// Returns the business object collection as an <see cref="IList{T}"/>.
        /// </summary>
        /// <returns>Returns an <see cref="IList{T}"/> object</returns>
        public override List<TBusinessObject> GetList()
        {
            return _boCol;
        }

        /// <summary>
        /// Commits to the database all the business objects that are either
        /// new or have been altered since the last committal
        /// </summary>
        public override void SaveAll()
        {
        }


        /// <summary>
        /// Restores all the business objects to their last persisted state, that
        /// is their state and values at the time they were last saved to the database
        /// </summary>
        public override void CancelEdits()
        {
            foreach (TBusinessObject bo in _boCol)
            {
                bo.CancelEdits();
            }
        }

        #region IBusinessObjectCollection Members

        #endregion

        public override TBusinessObject CreateBusinessObject()
        {
            return default(TBusinessObject);
        }

        ///<summary>
        /// Marks the business object as MarkedForDeletion and places the object
        ///</summary>
        ///<param name="businessObject"></param>
        public override void MarkForDelete(TBusinessObject businessObject)
        {
            if (businessObject.Status.IsNew)
            {
                //Remove object from collection and created col and set state as permanently deleted
            }
            businessObject.MarkForDelete();
        }
    }
}
