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
using Habanero.Base;

namespace Habanero.Testability.Helpers
{
    ///<summary>
    /// Used to stub out the IDBNumber generator for visual testing with an in memory database as well as
    /// for Unit and integration testing.
    ///</summary>
    public class DBNumberGeneratorStub : IDBNumberGenerator
    {
        private readonly TransactionalStub _updateTransaction = new TransactionalStub();

        static DBNumberGeneratorStub()
        {
            CurrentNumber = 0;
        }

        ///<summary>
        /// The current value being used by the Number Gen
        ///</summary>
        public static int CurrentNumber { get; set; }

        public virtual int GetNextNumberInt()
        {
            return ++CurrentNumber;
        }

        public virtual ITransactional GetUpdateTransaction()
        {
            return _updateTransaction;
        }
    }
}