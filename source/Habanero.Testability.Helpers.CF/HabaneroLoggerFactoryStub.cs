using System;
using Habanero.Base;
using Habanero.Base.Logging;

namespace Habanero.Testability.Helpers
{
    /// <summary>
    /// This is a stub implementation of a HabaneroLoggerFactor.
    /// GetLogger returns <see cref="HabaneroLoggerStub"/>
    /// Basically just returns a logger that stubs out all methods.
    /// </summary>
    public class HabaneroLoggerFactoryStub : IHabaneroLoggerFactory
    {
        private readonly HabaneroLoggerStub _habaneroLoggerStub = new HabaneroLoggerStub();

        ///<summary>
        ///</summary>
        ///<param name="contextName"></param>
        ///<returns></returns>
        public IHabaneroLogger GetLogger(string contextName)
        {
            return _habaneroLoggerStub;
        }

        ///<summary>
        ///</summary>
        ///<param name="type"></param>
        ///<returns></returns>
        public IHabaneroLogger GetLogger(Type type)
        {
            return _habaneroLoggerStub;
        }
    }

    ///<summary>
    /// Basically just a logger that stubs out all methods.
    ///</summary>
    public class HabaneroLoggerStub : IHabaneroLogger
    {
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