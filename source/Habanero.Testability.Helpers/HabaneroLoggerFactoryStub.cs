using System;
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
    /// <summary>
    /// Constructs Loggers that log to the Console.
    /// </summary>
    public class HabaneroConsoleLoggerFactory : IHabaneroLoggerFactory
    {
        #region Implementation of IHabaneroLoggerFactory

        public IHabaneroLogger GetLogger(string contextName)
        {
            return new HabaneroConsoleLogger(contextName);
        }

        #endregion
    }
    /// <summary>
    /// Logs any messaged to the console
    /// </summary>
    public class HabaneroConsoleLogger : IHabaneroLogger
    {
        private readonly string _contextName;

        public HabaneroConsoleLogger(string contextName)
        {
            _contextName = contextName;
        }

        #region Implementation of IHabaneroLogger

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Log(string message, LogCategory logCategory)
        {
            Console.WriteLine(logCategory + " : " + message);
        }

        public string ContextName
        {
            get { return _contextName; }
        }

        #endregion
    }
}