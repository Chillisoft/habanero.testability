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
        public void Test_CreateBusinessObject()
        {
            IBusinessObject businessObject = new BOFactory().CreateBusinessObject(typeof(FakeBO));
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject);
        }

        [Test]
        public void Test_CreateBusinessObject_WhenTypeNotIBO_ShouldRaiseError()
        {
            IBOFactory factory = new BOFactory();
            try
            {
                factory.CreateBusinessObject(typeof(int));
                Assert.Fail("Expected to throw an HabaneroDeveloperException");
            }
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains("The BOFactory.CreateBusinessObject was called with Type that does not implement IBusinessObject", ex.Message);
            }
        }

        [Test]
        public void Test_CreateBusinessObjectGeneric()
        {
            FakeBO businessObject = new BOFactory().CreateBusinessObject<FakeBO>();
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject);
        }

        [Test]
        public void Test_CreateBusinessObjectGenericViaInterface()
        {
            IBOFactory factory = new BOFactory();
            FakeBO businessObject = factory.CreateBusinessObject<FakeBO>();
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject);
        }

        [Test]
        public void Test_CreateBusinessObjectViaInterface()
        {
            IBOFactory factory = new BOFactory();
            IBusinessObject businessObject = factory.CreateBusinessObject(typeof(FakeBO));
            Assert.IsNotNull(businessObject);
            Assert.IsInstanceOf<FakeBO>(businessObject);
        }

        [Test]
        public void Test_GetValidValueGenerator_WhenDouble_ShouldRetDoubleGenerator()
        {
            IPropDef def = new PropDefFake {
                PropertyType = typeof(double)
            };
            ValidValueGenerator generator = BOTestFactory.GetValidValueGenerator(def);
            Assert.IsNotNull(generator);
            Assert.IsInstanceOf<ValidValueGeneratorDouble>(generator);
        }
    }
}

