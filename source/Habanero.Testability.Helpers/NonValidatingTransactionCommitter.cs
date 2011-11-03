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
using System.Text;
using Habanero.Base;
using Habanero.BO;

namespace Habanero.Testability.Helpers
{
    /// <summary>
    /// A Transaction Committer for a <see cref="DataStoreInMemory"/> that does not Validate the BusinessObject before persisting it
    /// </summary>
    public class NonValidatingTransactionCommitter : TransactionCommitterInMemory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataStoreInMemory"></param>
        public NonValidatingTransactionCommitter(DataStoreInMemory dataStoreInMemory)
            : base(dataStoreInMemory)
        {
        }

        /// <summary>
        /// Used to decorate a businessObject in a TransactionalBusinessObject. To be overridden in the concrete 
        /// implementation of a TransactionCommitter depending on the type of transaction you need.
        /// </summary>
        /// <param name="businessObject">The business object to decorate</param>
        /// <returns>
        /// A decorated Business object (TransactionalBusinessObject)
        /// </returns>
        protected override TransactionalBusinessObject CreateTransactionalBusinessObject(IBusinessObject businessObject)
        {
            return new NonValidatingTransactionalBusinessObject(businessObject);
        }
    }
    /// <summary>
    /// This is a <see cref="DataAccessorInMemory"/> that does not validate the businessObjects.
    /// This is very usefull for testing in cases where you need to persist an object that 
    /// is in a state that would break some validation rules.
    /// E.g. Expired samples but you cannot create the expired samples in the normal way since the rule is ExpiryDate > Today.
    /// </summary>
    public class NonValidatingDataAccessorInMemory : DataAccessorInMemory
    {
        /// <summary>
        /// 
        /// </summary>
        public NonValidatingDataAccessorInMemory()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataStore"></param>
        public NonValidatingDataAccessorInMemory(DataStoreInMemory dataStore)
            : base(dataStore)
        {
        }

        /// <summary>
        /// Creates a TransactionCommitter for you to use to persist BusinessObjects. A new TransactionCommitter is required
        /// each time an object or set of objects is persisted.
        /// </summary>
        /// <returns>
        /// </returns>
        public override ITransactionCommitter CreateTransactionCommitter()
        {
            return new NonValidatingTransactionCommitter(_dataStore);
        }
    }
    /// <summary>
    /// Wraps a BO in a <see cref="TransactionalBusinessObject"/> but does not call validate the BO when IsValid is called.
    /// </summary>
    public class NonValidatingTransactionalBusinessObject : TransactionalBusinessObject
    {
        protected internal NonValidatingTransactionalBusinessObject(IBusinessObject businessObject)
            : base(businessObject)
        {
        }

        /// <summary>
        ///             Indicates whether all of the property values are valid
        /// </summary>
        /// <param name="invalidReason">A string to modify with a reason
        ///             for any invalid values</param>
        /// <returns>
        /// Returns true if all are valid
        /// </returns>
        protected override bool IsValid(out string invalidReason)
        {
            invalidReason = null;
            return true;
        }

        /// <summary>
        /// returns true if there is already an object in the database with the same primary identifier (primary key)
        /// or with the same alternate identifier (alternate key)
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns>
        /// </returns>
        protected override bool HasDuplicateIdentifier(out string errMsg)
        {
            errMsg = null;
            return false;
        }

        /// <summary>
        /// Checks the underlying business object for any concurrency control errors before trying to commit to 
        /// the datasource
        /// </summary>
        protected override void CheckForConcurrencyErrors()
        {

        }
    }
}
