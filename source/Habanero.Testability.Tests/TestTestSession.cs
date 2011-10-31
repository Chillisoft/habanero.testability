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
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;

namespace Habanero.Testability.Tests
{
    [TestFixture]
    public class TestTestSession
    {

        [TestFixtureSetUp]
        public void SetFixtureUp()
        {
            BOTestFactoryRegistry.Instance = null;
            
            ClassDef.ClassDefs.Clear();
            AllClassesAutoMapper.ClassDefCol.Clear();
            ClassDefCol classDefCol = typeof(FakeBO).MapClasses();
            ClassDef.ClassDefs.Add(classDefCol);
        }

        [SetUp]
        public void Setup()
        {
            BORegistry.DataAccessor = GetDataAccessorInMemory();
        }

        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }

/*        [Test]
        public void Test_Register()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            //---------------Test Result -----------------------
            Assert.Fail("Test Not Yet Implemented");
        }*/
    }
}