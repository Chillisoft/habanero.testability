using Habanero.Base;

namespace Habanero.Testability.Helpers
{
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
}