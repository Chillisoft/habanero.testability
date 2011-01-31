using System;
using Habanero.Base;

namespace Habanero.Testability.Helpers
{
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