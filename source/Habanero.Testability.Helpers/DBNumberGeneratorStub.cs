using Habanero.Base;

namespace Habanero.Testability.Helpers
{
    ///<summary>
    /// Used to stub out the IDBNumber generator for visual testing with an in memory database as well as
    /// for Unit and integration testing.
    ///</summary>
    public class DBNumberGeneratorStub : IDBNumberGenerator
    {
        private readonly TransactionalStub _updateTransaction = new TransactionalStub();

        static DBNumberGeneratorStub()
        {
            CurrentNumber = 0;
        }

        ///<summary>
        /// The current value being used by the Number Gen
        ///</summary>
        public static int CurrentNumber { get; set; }

        public virtual int GetNextNumberInt()
        {
            return ++CurrentNumber;
        }

        public virtual ITransactional GetUpdateTransaction()
        {
            return _updateTransaction;
        }
    }
}