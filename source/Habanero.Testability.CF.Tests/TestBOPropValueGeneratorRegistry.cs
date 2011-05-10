using System;
using System.Collections.Generic;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.Testability.CF;
using Habanero.Testability.Helpers;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;

namespace Habanero.Testability.Tests
{
    [TestFixture]
    public class TestBOPropValueGeneratorRegistry
    {
        // ReSharper disable InconsistentNaming
        private static DataAccessorInMemory GetDataAccessorInMemory()
        {
            return new DataAccessorInMemory();
        }

        [TestFixtureSetUp]
        public void SetFixtureUp()
        {
            BORegistry.DataAccessor = GetDataAccessorInMemory();
            GlobalRegistry.LoggerFactory = new HabaneroLoggerFactoryStub();
        }
        [TearDown]
        public void TearDown()
        {
            ValidValueGeneratorRegistry.Instance.Register<string, ValidValueGeneratorString>();
        }

        [Test]
        public void Test_RegisterCustomBOTestFactory_ForAPropDef_ShouldReturnWhenResolved()
        {
            //---------------Set up test pack-------------------
            BOPropValueGeneratorRegistry registry = new BOPropValueGeneratorRegistry();
            var stringPropDef = GetStringPropDef();
            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<ValidValueGeneratorString>(registry.Resolve(stringPropDef));
            //---------------Execute Test ----------------------
            registry.Register(stringPropDef, typeof(ValidValueGeneratorIncrementalInt));
            ValidValueGenerator validValueGenerator = registry.Resolve(stringPropDef);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<ValidValueGeneratorIncrementalInt>(validValueGenerator);
        }

        [Test]
        public void Test_RegisterTwice_ForAPropDef_ShouldStoreSecond()
        {
            //---------------Set up test pack-------------------
            BOPropValueGeneratorRegistry registry = new BOPropValueGeneratorRegistry();
            var stringPropDef = GetStringPropDef();
            registry.Register(stringPropDef, typeof(ValidValueGeneratorEnum));
            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<ValidValueGeneratorEnum>(registry.Resolve(stringPropDef));
            //---------------Execute Test ----------------------
            registry.Register(stringPropDef, typeof(ValidValueGeneratorDecimal));
            ValidValueGenerator validValueGenerator = registry.Resolve(stringPropDef);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<ValidValueGeneratorDecimal>(validValueGenerator);
        }

        [Test]
        public void Test_RegisterCustomGenForTypeAndForPropDef_ShouldReturnPropDefRegistration()
        {
            //---------------Set up test pack-------------------
            BOPropValueGeneratorRegistry registry = new BOPropValueGeneratorRegistry();
            var stringPropDef = GetStringPropDef();
            registry.Register(stringPropDef, typeof(ValidValueGeneratorInt));
            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<ValidValueGeneratorInt>(registry.Resolve(stringPropDef));
            //---------------Execute Test ----------------------
            registry.Register(stringPropDef, typeof(ValidValueGeneratorDecimal));
            ValidValueGenerator validValueGenerator = registry.Resolve(stringPropDef);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<ValidValueGeneratorDecimal>(validValueGenerator);
        }

        [Test]
        public void Test_ResolveForDifPropDefShouldReturnDefaultValidValGen()
        {
            //---------------Set up test pack-------------------
            BOPropValueGeneratorRegistry registry = new BOPropValueGeneratorRegistry();
            ValidValueGeneratorRegistry.Instance.Register<string, ValidValueGeneratorInt>();
            var stringPropDef1 = GetStringPropDef();
            var stringPropDef2 = GetStringPropDef();
            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<ValidValueGeneratorInt>(registry.Resolve(stringPropDef1));
            Assert.IsInstanceOf<ValidValueGeneratorInt>(registry.Resolve(stringPropDef2));
            //---------------Execute Test ----------------------
            registry.Register(stringPropDef1, typeof(ValidValueGeneratorDecimal));
            ValidValueGenerator validValueGenerator1 = registry.Resolve(stringPropDef1);
            ValidValueGenerator validValueGenerator2 = registry.Resolve(stringPropDef2);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<ValidValueGeneratorDecimal>(validValueGenerator1);
            Assert.IsInstanceOf<ValidValueGeneratorInt>(validValueGenerator2);
        }

        [Test]
        public void Test_Resolve_WhenNoPropDefRegistered_ShouldReturnForType()
        {
            //---------------Set up test pack-------------------
            BOPropValueGeneratorRegistry registry = new BOPropValueGeneratorRegistry();
            
            var stringPropDef1 = GetStringPropDef();
            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<ValidValueGeneratorString>(registry.Resolve(stringPropDef1));
            //---------------Execute Test ----------------------
            ValidValueGeneratorRegistry.Instance.Register<string, ValidValueGeneratorInt>();
            ValidValueGenerator validValueGenerator1 = registry.Resolve(stringPropDef1);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<ValidValueGeneratorInt>(validValueGenerator1);
        }

        [Test]
        public void Test_RegisterCustomBOTestFactory_ForAPropDef_WithInvalidGeneratorType_ShouldRaiseErr()
        {
            //---------------Set up test pack-------------------
            BOPropValueGeneratorRegistry registry = new BOPropValueGeneratorRegistry();
            var stringPropDef = GetStringPropDef();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                registry.Register(stringPropDef, typeof(FakeEnum));
                Assert.Fail("Expected to throw an HabaneroApplicationException");
            }
            //---------------Test Result -----------------------
            catch (HabaneroApplicationException ex)
            {
                StringAssert.Contains("A ValidValueGenerator is being Registered for", ex.Message);
                StringAssert.Contains("but the ValidValueGenerator is not of Type ValidValueGenerator", ex.Message);
            }
        }


        [Test]
        public void Test_RegisterCustomBOTestFactory_AdditionalParameter_ExceptionIfSuitableConstructorNotFound()
        {
            //---------------Set up test pack-------------------
            var randomParameter = RandomValueGen.GetRandomString();
            var registry = new BOPropValueGeneratorRegistry();
            var stringPropDef = GetStringPropDef();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            registry.Register(stringPropDef, typeof(GeneratorStub), randomParameter);
            try
            {
                var validValueGenerator = registry.Resolve(stringPropDef);
                //---------------Test Result -----------------------
                Assert.Fail("Expected to throw an ArgumentException");
            }
            catch (ArgumentException ex)
            {
                StringAssert.Contains("An extra parameter was provided for a valid value generator type (GeneratorStub), but no suitable constructor was found with a second parameter.", ex.Message);
            }
        }

        [Test]
        public void Test_IsRegister_WhenRegistered_ShouldReturnTrue()
        {
            //---------------Set up test pack-------------------
            var stringPropDef = GetStringPropDef();
 
            BOPropValueGeneratorRegistry registry = new BOPropValueGeneratorRegistry();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            registry.Register(stringPropDef, typeof(ValidValueGeneratorLong));
            bool isRegistered = registry.IsRegistered(stringPropDef);
            //---------------Test Result -----------------------
            Assert.IsTrue(isRegistered);
        }
        [Test]
        public void Test_IsRegister_WhenNotRegistered_ShouldReturnFalse()
        {
            //---------------Set up test pack-------------------
            var stringPropDef = GetStringPropDef();
            BOPropValueGeneratorRegistry registry = new BOPropValueGeneratorRegistry();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            bool isRegistered = registry.IsRegistered(stringPropDef);
            //---------------Test Result -----------------------
            Assert.IsFalse(isRegistered);
        }


        [Test]
        public void Test_Registry_ShouldReturnSingletonRegistry()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            BOPropValueGeneratorRegistry registry = BOPropValueGeneratorRegistry.Instance;
            //---------------Test Result -----------------------
            Assert.IsNotNull(registry);
        }

        [Test]
        public void Test_Registry_WhenCallTwice_ShouldReturnSameSingletonRegistry()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            BOPropValueGeneratorRegistry origRegistry = BOPropValueGeneratorRegistry.Instance;
            BOPropValueGeneratorRegistry registry = BOPropValueGeneratorRegistry.Instance;
            //---------------Test Result -----------------------
            Assert.AreSame(origRegistry, registry);
        }

        [Test]
        public void Test_SetRegistry_WithNull_ShouldReturnNewSingleton()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            BOPropValueGeneratorRegistry.Instance = null;
            BOPropValueGeneratorRegistry boDefaultValueRegistry = BOPropValueGeneratorRegistry.Instance;
            //---------------Test Result -----------------------
            Assert.IsNotNull(boDefaultValueRegistry);
        }

        private static PropDefFake GetStringPropDef()
        {
            return new PropDefFake { PropertyType = typeof(string) };
        }

        private class GeneratorStub : ValidValueGenerator
        {
            public GeneratorStub(IPropDef propDef) : base(propDef)
            {
            }

            public override object GenerateValidValue()
            {
                throw new NotImplementedException();
            }
        }
    }
}