using System;
using Habanero.Base;
using Habanero.Base.Logging;

namespace Habanero.Testability.Helpers
{
    /// <summary>
    /// Constructs Loggers that log to the Console.
    /// </summary>
    public class HabaneroConsoleLoggerFactory : IHabaneroLoggerFactory
    {
        #region Implementation of IHabaneroLoggerFactory

        ///<summary>
        ///</summary>
        ///<param name="contextName"></param>
        ///<returns></returns>
        public IHabaneroLogger GetLogger(string contextName)
        {
            return new HabaneroConsoleLogger(contextName);
        }

        ///<summary>
        ///</summary>
        ///<param name="type"></param>
        ///<returns></returns>
        public IHabaneroLogger GetLogger(Type type)
        {
            return new HabaneroConsoleLogger(type == null? "": type.FullName);
        }

        #endregion
    }
}