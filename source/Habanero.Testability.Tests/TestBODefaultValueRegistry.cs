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
using Habanero.Testability.Tests.Base;
using NUnit.Framework;

namespace Habanero.Testability.Tests
{
    [TestFixture]
    public class TestBODefaultValueRegistry
    {
        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }

        [TestFixtureSetUp]
        public void SetFixtureUp()
        {
            BORegistry.DataAccessor = GetDataAccessorInMemory();
        }

        [Test]
        public void Test_Register_WithValue_ResolveShouldReturn()
        {
            //---------------Set up test pack-------------------
            const string methodName = "fdsafasdfasd";
            const string expectedDefaultValue = "fdfasdfasd";
            BODefaultValueRegistry registry = new BODefaultValueRegistry();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            registry.Register(methodName, expectedDefaultValue);
            object defaultValue = registry.Resolve(methodName);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedDefaultValue, defaultValue);
        }

        [Test]
        public void Test_Register_Twice_WithValue_ResolveShouldReturnSecondValue()
        {
            //---------------Set up test pack-------------------
            const string methodName = "fdsafasdfasd";
            const string expectedDefaultValue = "fdfasdfasd";
            BODefaultValueRegistry registry = new BODefaultValueRegistry();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            registry.Register(methodName, "InitialValue");
            registry.Register(methodName, expectedDefaultValue);
            object defaultValue = registry.Resolve(methodName);
            //---------------Test Result -----------------------
            Assert.AreSame(expectedDefaultValue, defaultValue);
        }
        [Test]
        public void Test_Register_WithNoValue_ResolveShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            const string methodName = "fdsafasdfasd";
            const string expectedDefaultValue = null;
            BODefaultValueRegistry registry = new BODefaultValueRegistry();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            registry.Register(methodName, expectedDefaultValue);
            object defaultValue = registry.Resolve(methodName);
            //---------------Test Result -----------------------
            Assert.IsNull(defaultValue);
            Assert.AreSame(expectedDefaultValue, defaultValue);
        }
        [Test]
        public void Test_IsRegister_WhenRegistered_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            const string methodName = "fdsafasdfasd";
            const string expectedDefaultValue = "fdfasdfasd";
            BODefaultValueRegistry registry = new BODefaultValueRegistry();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            registry.Register(methodName, expectedDefaultValue);
            bool isRegistered = registry.IsRegistered(methodName);
            //---------------Test Result -----------------------
            Assert.IsTrue(isRegistered);
        }
        [Test]
        public void Test_IsRegister_WhenNotRegistered_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            const string methodName = "fdsafasdfasd";
            BODefaultValueRegistry registry = new BODefaultValueRegistry();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            bool isRegistered = registry.IsRegistered(methodName);
            //---------------Test Result -----------------------
            Assert.IsFalse(isRegistered);
        }

        [Test]
        public void Test_Registry_ShouldReturnSingletonRegistry()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            BODefaultValueRegistry registry = BODefaultValueRegistry.Instance;
            //---------------Test Result -----------------------
            Assert.IsNotNull(registry);
        }

        [Test]
        public void Test_Registry_WhenCallTwice_ShouldReturnSameSingletonRegistry()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            BODefaultValueRegistry origRegistry = BODefaultValueRegistry.Instance;
            BODefaultValueRegistry registry = BODefaultValueRegistry.Instance;
            //---------------Test Result -----------------------
            Assert.AreSame(origRegistry, registry);
        }

        [Test]
        public void Test_SetRegistry_WithNull_ShouldReturnNewSingleton()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            BODefaultValueRegistry.Instance = null;
            BODefaultValueRegistry boDefaultValueRegistry = BODefaultValueRegistry.Instance;
            //---------------Test Result -----------------------
            Assert.IsNotNull(boDefaultValueRegistry);
        }
    }
}