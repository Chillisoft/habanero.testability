using Habanero.Testability.Tests.Base;

namespace Habanero.Testability.Tests
{
    using Habanero.Base;
    using Habanero.Base.Exceptions;
    using Habanero.Testability;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class TestBOFactory
    {
        [Test]
        public void Test_CreateBusinessObjectGeneric()
        {
            //---------------Set up test pack-------------------
            BOFactory factory = new BOFactory();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var businessObject = factory.CreateBusinessObject<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject);
        }
        [Test]
        public void Test_CreateBusinessObjectGenericViaInterface()
        {
            //---------------Set up test pack-------------------
            IBOFactory factory = new BOFactory();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var businessObject = factory.CreateBusinessObject<FakeBO>();
            //---------------Test Result -----------------------
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject);
        }
        [Test]
        public void Test_CreateBusinessObject()
        {
            //---------------Set up test pack-------------------
            BOFactory factory = new BOFactory();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var businessObject = factory.CreateBusinessObject(typeof(FakeBO));
            //---------------Test Result -----------------------
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject);
        }
        [Test]
        public void Test_CreateBusinessObjectViaInterface()
        {
            //---------------Set up test pack-------------------
            IBOFactory factory = new BOFactory();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var businessObject = factory.CreateBusinessObject(typeof(FakeBO));
            //---------------Test Result -----------------------
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject);
        }
        [Test]
        public void Test_CreateBusinessObject_WhenTypeNotIBO_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            IBOFactory factory = new BOFactory();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                factory.CreateBusinessObject(typeof(int));
                Assert.Fail("Expected to throw an HabaneroDeveloperException");
            }
            //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains("The BOFactory.CreateBusinessObject was called with Type that does not implement IBusinessObject", ex.Message);
            }
        }

        [Test]
        public void Test_GetValidValueGenerator_WhenDoubld_ShouldRetDoubleGenerator()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake { PropertyType = typeof(Double) };

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var generator = BOTestFactory.GetValidValueGenerator(def);
            //---------------Test Result -----------------------
            Assert.IsNotNull(generator);
            Assert.IsInstanceOf<ValidValueGeneratorDouble>(generator);
        }

        [Test]
        public void Test_GetValidValueGenerator_WhenDouble_ShouldRetDoubleGenerator()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake 
                    {
                        PropertyType = typeof(double)
                    };
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            ValidValueGenerator generator = BOTestFactory.GetValidValueGenerator(def);
            //---------------Test Result -----------------------
            Assert.IsNotNull(generator);
            Assert.IsInstanceOf<ValidValueGeneratorDouble>(generator);
        }
    }
}

