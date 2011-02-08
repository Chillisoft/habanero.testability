using System;
using Habanero.Base.Logging;

namespace Habanero.Testability.Helpers
{
    public class HabaneroLoggerFactoryStub : IHabaneroLoggerFactory
    {
        private readonly HabaneroLoggerStub _habaneroLoggerStub = new HabaneroLoggerStub();

        public IHabaneroLogger GetLogger(string contextName)
        {
            return _habaneroLoggerStub;
        }

        public IHabaneroLogger GetLogger(Type type)
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

        public void Log(Exception exception)
        {
            
        }

        public void Log(string message, Exception exception)
        {
            
        }

        public void Log(string message, Exception exception, LogCategory logCategory)
        {
            
        }

        public string ContextName
        {
            get { return "HabaneroLoggerStub (logger for testing only)"; }
        }
    }
}