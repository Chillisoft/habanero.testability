﻿#region Licensing Header
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
using Habanero.Smooth;
using Habanero.Testability.Helpers;
using Habanero.Testability.Tests.Base;

namespace Habanero.Testability.Tests
{
    using Habanero.Base;
    using Habanero.Base.Exceptions;
    using Habanero.BO;
    using Habanero.BO.ClassDefinition;
    using Habanero.Testability;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class TestBOTestFactoryRegistry
    {
        public static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }
        [TestFixtureSetUp]
        public void SetFixtureUp()
        {
            BORegistry.DataAccessor = GetDataAccessorInMemory();
            GlobalRegistry.LoggerFactory = new HabaneroLoggerFactoryStub();
            ClassDefCol classDefCol = typeof(FakeBO).MapClasses();
            ClassDef.ClassDefs.Add(classDefCol);
        }

        [Test]
        public void Test_Register_WhenCustomBOTestFactory_WithFactoryType_ShouldReturnWhenResolved()
        {
            //---------------Set up test pack-------------------
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            registry.Register<FakeBO>(typeof(BOTestFactoryFakeBO));
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            BOTestFactory boTestFactory = registry.Resolve<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<BOTestFactoryFakeBO>(boTestFactory);
        }

        [Test]
        public void Test_Register_WhenFactoryTypeNull_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            //---------------Assert Precondition----------------
            Type factoryType = null;
            //---------------Execute Test ----------------------
            try
            {
                registry.Register<FakeBO>(factoryType);
                Assert.Fail("Expected to throw an HabaneroApplicationException");
            }
            catch (HabaneroApplicationException ex)
            {
                StringAssert.Contains("A BOTestFactory is being Registered for '", ex.Message);
                StringAssert.Contains("but the BOTestFactory is Null", ex.Message);
            }
        }

        [Test]
        public void Test_Register_WhenNotFactoryType_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                registry.Register<FakeBO>(typeof(RelatedFakeBo));
                Assert.Fail("Expected to throw an HabaneroApplicationException");
            }
            catch (HabaneroApplicationException ex)
            {
                StringAssert.Contains("A BOTestFactory is being Registered for '", ex.Message);
                StringAssert.Contains("is not of Type BOTestFactory", ex.Message);
            }
        }

        [Test]
        public void Test_RegisterCustomBOTestFactory_ShouldReturnWhenResolved()
        {
            //---------------Set up test pack-------------------
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            //---------------Assert Precondition----------------
            registry.Register<FakeBO, BOTestFactoryFakeBO>();
            //---------------Execute Test ----------------------
            Assert.IsInstanceOf<BOTestFactoryFakeBO>(registry.Resolve<FakeBO>());
        }

        [Test]
        public void Test_RegisterInstance_ShouldRegister()
        {
            //---------------Set up test pack-------------------
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            BOTestFactoryFakeBO boTestFactoryFakeBO = new BOTestFactoryFakeBO();
            //---------------Assert Precondition----------------
            registry.Register<FakeBO>(boTestFactoryFakeBO);
            //---------------Execute Test ----------------------
            BOTestFactory boTestFactory = registry.Resolve<FakeBO>();
            Assert.AreSame(boTestFactoryFakeBO, boTestFactory);
        }

        [Test]
        public void Test_RegisterTwice_ShouldStoreSecond()
        {
            //---------------Set up test pack-------------------
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            registry.Register<FakeBO, BOTestFactoryFakeBO>();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(registry.Resolve<FakeBO>());
            //---------------Execute Test ----------------------
            registry.Register<FakeBO, BOTestFactory<FakeBO>>();
            Assert.IsNotInstanceOf<BOTestFactoryFakeBO>(registry.Resolve<FakeBO>());
        }

        [Test]
        public void Test_RegisterTwice_WhenInstanceThenInstance_ShouldStoreSecond()
        {
            //---------------Set up test pack-------------------
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            BOTestFactoryFakeBO firstInstance = new BOTestFactoryFakeBO();
            registry.Register<FakeBO>(firstInstance);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(registry.Resolve<FakeBO>());
            //---------------Execute Test ----------------------
            BOTestFactoryFakeBO secondFactory = new BOTestFactoryFakeBO();
            registry.Register<FakeBO>(secondFactory);
            BOTestFactory boTestFactory = registry.Resolve<FakeBO>();
            Assert.AreSame(secondFactory, boTestFactory);
        }

        [Test]
        public void Test_RegisterTwice_WhenInstanceThenType_ShouldStoreSecond()
        {
            //---------------Set up test pack-------------------
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            BOTestFactoryFakeBO origFactory = new BOTestFactoryFakeBO();
            registry.Register<FakeBO>(origFactory);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(registry.Resolve<FakeBO>());
            //---------------Execute Test ----------------------
            registry.Register<FakeBO, BOTestFactory<FakeBO>>();
            BOTestFactory boTestFactory = registry.Resolve<FakeBO>();
            Assert.AreNotSame(origFactory, boTestFactory);
        }

        [Test]
        public void Test_RegisterTwice_WhenTypeThenInstance_ShouldStoreSecond()
        {
            //---------------Set up test pack-------------------
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            registry.Register<FakeBO, BOTestFactory<FakeBO>>();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(registry.Resolve<FakeBO>());
            BOTestFactoryFakeBO secondFactory = new BOTestFactoryFakeBO();
            //---------------Execute Test ----------------------
            registry.Register<FakeBO>(secondFactory);
            BOTestFactory boTestFactory = registry.Resolve<FakeBO>();
            Assert.AreSame(secondFactory, boTestFactory);
        }

        [Test]
        public void Test_Registry_ShouldReturnSingletonRegistry()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            BOTestFactoryRegistry registry = BOTestFactoryRegistry.Instance;
            Assert.IsNotNull(registry);
        }

        [Test]
        public void Test_Registry_WhenCallTwice_ShouldReturnSameSingletonRegistry()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            BOTestFactoryRegistry origRegistry = BOTestFactoryRegistry.Instance;
            BOTestFactoryRegistry registry = BOTestFactoryRegistry.Instance;

            Assert.AreSame(origRegistry, registry);
        }
        [Test]
        public void Test_Resolve_WhenNoFactoryRegistered_ShouldRetGenericFactory()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            BOTestFactory boTestFactory = new BOTestFactoryRegistry().Resolve<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<BOTestFactoryFakeBO>(boTestFactory);
            Assert.IsInstanceOf<BOTestFactory<FakeBO>>(boTestFactory);
        }

        [Test]
        public void Test_Resolve_WithType_WhenNoFactoryRegistered_AndHasSpecialisedFactory_ShouldRetSpecialisedFactory()
        {
            //---------------Set up test pack-------------------
            var boTestFactoryRegistry = new BOTestFactoryRegistry();
            //---------------Execute Test ----------------------
            BOTestFactory boTestFactory = boTestFactoryRegistry.Resolve(typeof(FakeBO));
            Assert.IsInstanceOf<BOTestFactoryFakeBO>(boTestFactory);
            Assert.IsInstanceOf<BOTestFactory<FakeBO>>(boTestFactory);
        }

        [Test]
        public void Test_Resolve_WithType_WhenNoFactoryRegistered_AndNoSpecialisedFactory_ShouldRetGenericFactory()
        {
            //---------------Set up test pack-------------------
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            //---------------Execute Test ----------------------
            var boTestFactory = registry.Resolve(typeof(RelatedFakeBo));
            Assert.IsInstanceOf<BOTestFactory<RelatedFakeBo>>(boTestFactory);
        }

        [Test]
        public void Test_Resolve_WithTypeNull_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            try
            {
                registry.Resolve(null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("typeOfBO", ex.ParamName);
            }
        }
    }

}

