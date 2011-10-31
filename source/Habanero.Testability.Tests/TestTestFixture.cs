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
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;

namespace Habanero.Testability.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestTestFixture
    {
        public TestTestFixture()
        {
        }

        [TestFixtureSetUp]
        public void SetFixtureUp()
        {
            BOTestFactoryRegistry.Instance = null;
            
            ClassDef.ClassDefs.Clear();
            AllClassesAutoMapper.ClassDefCol.Clear();
            ClassDefCol classDefCol = typeof(FakeBO).MapClasses();
            ClassDef.ClassDefs.Add(classDefCol);
        }
        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }
        [SetUp]
        public void Setup()
        {
            BORegistry.DataAccessor = GetDataAccessorInMemory();
        }
        [Test]
        public void Test_CreateObject_ShouldCreateUsingTestFactory()
        {
            //---------------Set up test pack-------------------
            TestFixture fixture = new TestFixture();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var fakeBO = fixture.CreateObject<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsNotNull(fakeBO);
        }

        [Test]
        public void Test_RegisterInstance_CreateShouldUseThisInstance()
        {
            //---------------Set up test pack-------------------
            TestFixture fixture = new TestFixture();
            var expectedFakeBO = new FakeBO();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            fixture.Register(expectedFakeBO);
            var fakeBO = fixture.CreateObject<FakeBO>();
            //---------------Test Result -----------------------
            Assert.AreSame(expectedFakeBO, fakeBO);
        }
    }
}