using System;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability;
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