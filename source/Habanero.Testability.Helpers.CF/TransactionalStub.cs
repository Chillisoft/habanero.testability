using Habanero.Base;

namespace Habanero.Testability.Helpers
{
    internal class TransactionalStub : ITransactional
    {
        private static int i = 0;
        public string TransactionID()
        {
            i++;
            return i.ToString();
        }

        public void UpdateStateAsCommitted()
        {

        }

        public void UpdateAsRolledBack()
        {

        }
    }
}