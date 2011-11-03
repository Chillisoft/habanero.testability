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
using Habanero.Base;
using Habanero.BO;
using Rhino.Mocks;

namespace Habanero.Testability.Helpers
{
    /// <summary>
    /// DataAccessor that returns a Mock TransactionCommitter.
    /// The Mock TransactionCommitter can then be used for testing whether commit was called etc.
    /// Also if you are using Mocked out BusinessObjects then you can still test saving etc.
    /// </summary>
    public class DataAccessorInMemoryWithMockCommitter : DataAccessorInMemory
    {
        public DataAccessorInMemoryWithMockCommitter()
        {
            TransactionCommitter = MockRepository.GenerateStub<ITransactionCommitter>();
        }

        public ITransactionCommitter TransactionCommitter { get; private set; }

        public override ITransactionCommitter CreateTransactionCommitter()
        {

            return TransactionCommitter;
        }
    }    /// <summary>
    /// DataAccessor that returns a Mock TransactionCommitter.
    /// The Mock TransactionCommitter can then be used for testing whether commit was called etc.
    /// Also if you are using Mocked out BusinessObjects then you can still test saving etc.
    /// </summary>
    public class DataAccessorWithMockCommitter : IDataAccessor
    {
        public DataAccessorWithMockCommitter()
        {
            TransactionCommitter = MockRepository.GenerateStub<ITransactionCommitter>();
            BusinessObjectLoader = MockRepository.GenerateStub<IBusinessObjectLoader>();
        }

        public ITransactionCommitter TransactionCommitter { get; private set; }

        public ITransactionCommitter CreateTransactionCommitter()
        {

            return TransactionCommitter;
        }

        public IBusinessObjectLoader BusinessObjectLoader { get; private set; }
    }
}