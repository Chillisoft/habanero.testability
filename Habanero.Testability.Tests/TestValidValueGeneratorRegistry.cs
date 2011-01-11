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
    [TestFixture]
    public class TestValidValueGeneratorRegistry
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
        public void Test_Register_WhenCustomValidValueGenerator_WithFactoryType_ShouldReturnWhenResolved()
        {
            //---------------Set up test pack-------------------
            ValidValueGeneratorRegistry registry = new ValidValueGeneratorRegistry();
            registry.Register<string>(typeof(ValidValueGeneratorString));
            PropDefFake propDef = new PropDefFake();
            propDef.PropertyType = typeof(string);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(string), propDef.PropertyType);
            //---------------Execute Test ----------------------
            ValidValueGenerator boTestFactory = registry.Resolve(propDef);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<ValidValueGeneratorString>(boTestFactory);
        }
        [Test]
        public void Test_Register_WhenStdValidValueGenerator_WithValueGen_ShouldReturnWhenResolved()
        {
            //---------------Set up test pack-------------------
            ValidValueGeneratorRegistry registry = new ValidValueGeneratorRegistry();
            registry.Register<string>(typeof(ValidValueGeneratorString));
            PropDefFake propDef = new PropDefFake();
            propDef.PropertyType = typeof(string);
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(string), propDef.PropertyType);
            //---------------Execute Test ----------------------
            ValidValueGenerator boTestFactory = registry.Resolve(propDef);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<ValidValueGeneratorString>(boTestFactory);
        }

        [Test]
        public void Test_Register_WhenFactoryTypeNull_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            ValidValueGeneratorRegistry registry = new ValidValueGeneratorRegistry();
            //---------------Assert Precondition----------------
            Type generatorType = null;
            //---------------Execute Test ----------------------
            try
            {
                registry.Register<string>(generatorType);
                Assert.Fail("Expected to throw an HabaneroApplicationException");
            }
            catch (HabaneroApplicationException ex)
            {
                StringAssert.Contains("A ValidValueGenerator is being Registered for '", ex.Message);
                StringAssert.Contains("but the ValidValueGenerator is Null", ex.Message);
            }
        }

        [Test]
        public void Test_Register_WhenNotFactoryType_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            ValidValueGeneratorRegistry registry = new ValidValueGeneratorRegistry();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                registry.Register<string>(typeof(RelatedFakeBo));
                Assert.Fail("Expected to throw an HabaneroApplicationException");
            }
            catch (HabaneroApplicationException ex)
            {
                StringAssert.Contains("A ValidValueGenerator is being Registered for '", ex.Message);
                StringAssert.Contains("is not of Type ValidValueGenerator", ex.Message);
            }
        }


        [Test]
        public void Test_RegisterCustomBOTestFactory_ShouldReturnWhenResolved()
        {
            //---------------Set up test pack-------------------
            ValidValueGeneratorRegistry registry = new ValidValueGeneratorRegistry();
            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<ValidValueGeneratorString>(registry.Resolve(GetStringPropDef()));
            //---------------Execute Test ----------------------
            registry.Register<string, ValidValueGeneratorInt>();
            ValidValueGenerator validValueGenerator = registry.Resolve(GetStringPropDef());
            //---------------Test Result -----------------------
            Assert.IsInstanceOf<ValidValueGeneratorInt>(validValueGenerator);
        }
        [Test]
        public void Test_RegisterTwice_ShouldStoreSecond()
        {
            //---------------Set up test pack-------------------
            ValidValueGeneratorRegistry registry = new ValidValueGeneratorRegistry();
            registry.Register<string, ValidValueGeneratorString>();
            //---------------Assert Precondition----------------
            Assert.IsNotNull(registry.Resolve(GetStringPropDef()));
            //---------------Execute Test ----------------------
            registry.Register<string, ValidValueGeneratorDecimal>();
            ValidValueGenerator validValueGenerator = registry.Resolve(GetStringPropDef());
            //---------------Test Result -----------------------
            Assert.IsNotInstanceOf<ValidValueGeneratorString>(validValueGenerator);
            Assert.IsInstanceOf<ValidValueGeneratorDecimal>(validValueGenerator);
        }
        private static PropDefFake GetStringPropDef()
        {
            return new PropDefFake {PropertyType = typeof(string)};
        }


        [Test]
        public void Test_Resolve_WithTypeNull_ShouldThrowError()
        {
            //---------------Set up test pack-------------------
            ValidValueGeneratorRegistry registry = new ValidValueGeneratorRegistry();
            try
            {
                registry.Resolve(null);
                Assert.Fail("expected ArgumentNullException");
            }
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("propDef", ex.ParamName);
            }
        }

        [Test]
        public void Test_Registry_ShouldReturnSingletonRegistry()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            ValidValueGeneratorRegistry registry = ValidValueGeneratorRegistry.Instance;
            //---------------Execute Test ----------------------
            Assert.IsNotNull(registry);
        }

        [Test]
        public void Test_Registry_WhenCallTwice_ShouldReturnSameSingletonRegistry()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            ValidValueGeneratorRegistry origRegistry = ValidValueGeneratorRegistry.Instance;
            ValidValueGeneratorRegistry registry = ValidValueGeneratorRegistry.Instance;
            //---------------Execute Test ----------------------
            Assert.AreSame(origRegistry, registry);
        }


    }
}