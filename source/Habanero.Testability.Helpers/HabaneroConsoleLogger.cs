#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
#endregion
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

        public bool IsLogging(LogCategory logCategory)
        {
        	return true;
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