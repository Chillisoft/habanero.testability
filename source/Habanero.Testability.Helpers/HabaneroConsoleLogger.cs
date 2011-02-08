using System;
using Habanero.Base;
using Habanero.Base.Logging;

namespace Habanero.Testability.Helpers
{
    /// <summary>
    /// Logs any messaged to the console
    /// </summary>
    public class HabaneroConsoleLogger : IHabaneroLogger
    {
        private readonly string _contextName;

        ///<summary>
        ///</summary>
        ///<param name="contextName"></param>
        public HabaneroConsoleLogger(string contextName)
        {
            _contextName = contextName;
        }

        #region Implementation of IHabaneroLogger

        ///<summary>
        ///</summary>
        ///<param name="message"></param>
        public void Log(string message)
        {
            Console.WriteLine(ContextName + " : " + message);
        }

        ///<summary>
        ///</summary>
        ///<param name="message"></param>
        ///<param name="logCategory"></param>
        public void Log(string message, LogCategory logCategory)
        {
            Console.WriteLine(ContextName + " : " + logCategory + " : " + message);
        }

        public void Log(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Log(string message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Log(string message, Exception exception, LogCategory logCategory)
        {
            throw new NotImplementedException();
        }

        ///<summary>
        ///</summary>
        public string ContextName
        {
            get { return _contextName; }
        }

        #endregion
    }
}