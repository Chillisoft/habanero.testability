namespace Habanero.Testability.Tests
{
    using AutoMappingHabanero;
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
            ClassDefCol classDefCol = typeof(FakeBO).MapClasses();
            ClassDef.ClassDefs.Add(classDefCol);
        }

        [Test]
        public void Test_Register_WhenCustomBOTestFactory_WithFactoryType_ShouldReturnWhenResolved()
        {
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            registry.Register<FakeBO>(typeof(BOTestFactoryFakeBO));
            Assert.IsInstanceOf<BOTestFactoryFakeBO>(registry.Resolve<FakeBO>());
        }

        [Test]
        public void Test_Register_WhenFactoryTypeNull_ShouldThrowError()
        {
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            Type factoryType = null;
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
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
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
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            registry.Register<FakeBO, BOTestFactoryFakeBO>();
            Assert.IsInstanceOf<BOTestFactoryFakeBO>(registry.Resolve<FakeBO>());
        }

        [Test]
        public void Test_RegisterInstance_ShouldRegister()
        {
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            BOTestFactoryFakeBO boTestFactoryFakeBO = new BOTestFactoryFakeBO();
            registry.Register<FakeBO>(boTestFactoryFakeBO);
            BOTestFactory boTestFactory = registry.Resolve<FakeBO>();
            Assert.AreSame(boTestFactoryFakeBO, boTestFactory);
        }

        [Test]
        public void Test_RegisterTwice_ShouldStoreSecond()
        {
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            registry.Register<FakeBO, BOTestFactoryFakeBO>();
            Assert.IsNotNull(registry.Resolve<FakeBO>());
            registry.Register<FakeBO, BOTestFactory<FakeBO>>();
            Assert.IsNotInstanceOf<BOTestFactoryFakeBO>(registry.Resolve<FakeBO>());
        }

        [Test]
        public void Test_RegisterTwice_WhenInstanceThenInstance_ShouldStoreSecond()
        {
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            BOTestFactoryFakeBO firstInstance = new BOTestFactoryFakeBO();
            registry.Register<FakeBO>(firstInstance);
            Assert.IsNotNull(registry.Resolve<FakeBO>());
            BOTestFactoryFakeBO secondFactory = new BOTestFactoryFakeBO();
            registry.Register<FakeBO>(secondFactory);
            BOTestFactory boTestFactory = registry.Resolve<FakeBO>();
            Assert.AreSame(secondFactory, boTestFactory);
        }

        [Test]
        public void Test_RegisterTwice_WhenInstanceThenType_ShouldStoreSecond()
        {
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            BOTestFactoryFakeBO origFactory = new BOTestFactoryFakeBO();
            registry.Register<FakeBO>(origFactory);
            Assert.IsNotNull(registry.Resolve<FakeBO>());
            registry.Register<FakeBO, BOTestFactory<FakeBO>>();
            BOTestFactory boTestFactory = registry.Resolve<FakeBO>();
            Assert.AreNotSame(origFactory, boTestFactory);
        }

        [Test]
        public void Test_RegisterTwice_WhenTypeThenInstance_ShouldStoreSecond()
        {
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            registry.Register<FakeBO, BOTestFactory<FakeBO>>();
            Assert.IsNotNull(registry.Resolve<FakeBO>());
            BOTestFactoryFakeBO secondFactory = new BOTestFactoryFakeBO();
            registry.Register<FakeBO>(secondFactory);
            BOTestFactory boTestFactory = registry.Resolve<FakeBO>();
            Assert.AreSame(secondFactory, boTestFactory);
        }

        [Test]
        public void Test_Registry_ShouldReturnSingletonRegistry()
        {
            Assert.IsNotNull(BOTestFactoryRegistry.Registry);
        }

        [Test]
        public void Test_Registry_WhenCallTwice_ShouldReturnSameSingletonRegistry()
        {
            BOTestFactoryRegistry origRegistry = BOTestFactoryRegistry.Registry;
            BOTestFactoryRegistry registry = BOTestFactoryRegistry.Registry;
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
            BOTestFactory boTestFactory = new BOTestFactoryRegistry().Resolve(typeof(FakeBO));
            Assert.IsInstanceOf<BOTestFactoryFakeBO>(boTestFactory);
            Assert.IsInstanceOf<BOTestFactory<FakeBO>>(boTestFactory);
        }

        [Test]
        public void Test_Resolve_WithType_WhenNoFactoryRegistered_AndNoSpecialisedFactory_ShouldRetGenericFactory()
        {
            BOTestFactoryRegistry registry = new BOTestFactoryRegistry();
            var boTestFactory = registry.Resolve(typeof(RelatedFakeBo));
            Assert.IsInstanceOf<BOTestFactory<RelatedFakeBo>>(boTestFactory);
        }

        [Test]
        public void Test_Resolve_WithTypeNull_ShouldThrowError()
        {
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

