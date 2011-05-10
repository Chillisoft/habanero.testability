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