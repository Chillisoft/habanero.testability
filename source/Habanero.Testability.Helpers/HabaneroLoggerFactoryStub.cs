using Habanero.Base;

namespace Habanero.Testability.Helpers
{
    public class HabaneroLoggerFactoryStub : IHabaneroLoggerFactory
    {
        private readonly HabaneroLoggerStub _habaneroLoggerStub = new HabaneroLoggerStub();

        public IHabaneroLogger GetLogger(string contextName)
        {
            return _habaneroLoggerStub;
        }
    }

    public class HabaneroLoggerStub : IHabaneroLogger
    {
        public void Log(string message)
        {

        }

        public void Log(string message, LogCategory logCategory)
        {

        }

        public string ContextName
        {
            get { return "HabaneroLoggerStub (logger for testing only)"; }
        }
    }
}