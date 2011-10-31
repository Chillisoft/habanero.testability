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
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability.Tests.Base;
using Rhino.Mocks;

namespace Habanero.Testability.Tests
{
    using Habanero.Base;
    using Habanero.Base.Exceptions;
    using Habanero.Testability;
    using NUnit.Framework;
    using System;

    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestBOFactory
    {
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            BORegistry.DataAccessor = new DataAccessorInMemory();
            AllClassesAutoMapper.ClassDefCol.Clear();
            ClassDefCol classDefCol = typeof(FakeBO).MapClasses(type => type.Name != "Unmapped");
            ClassDef.ClassDefs.Add(classDefCol);
        }
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
            var factory = new BOFactory();
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
        public void Test_GetValidValueGenerator_WhenDouble_ShouldRetDoubleGenerator()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake 
                    {
                        PropertyType = typeof(double)
                    };
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            ValidValueGenerator generator = new BOTestFactory<FakeBOWithRules>().GetValidValueGenerator(def);
            //---------------Test Result -----------------------
            Assert.IsNotNull(generator);
            Assert.IsInstanceOf<ValidValueGeneratorDouble>(generator);
        }
        [Test]
        public void Test_GetValidValueGenerator_WhenEnum_ShouldRetEnumGenerator()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake 
                    {
                        PropertyType = typeof(FakeEnum)
                    };
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            ValidValueGenerator generator = new BOTestFactory<FakeBOWithRules>().GetValidValueGenerator(def);
            //---------------Test Result -----------------------
            Assert.IsNotNull(generator);
            Assert.IsInstanceOf<ValidValueGeneratorEnum>(generator);
        }
        [Test]
        public void Test_GetValidValueGenerator_WhenHasLookup_ShouldRetLookupGenerator()
        {
            //---------------Set up test pack-------------------
            IPropDef def = new PropDefFake 
                    {
                        PropertyType = typeof(string),
                        LookupList = MockRepository.GenerateStub<IBusinessObjectLookupList>()
                    };
            //---------------Assert Precondition----------------
            Assert.IsTrue(def.HasLookupList());
            //---------------Execute Test ----------------------
            ValidValueGenerator generator = new BOTestFactory<FakeBOWithRules>().GetValidValueGenerator(def);
            //---------------Test Result -----------------------
            Assert.IsNotNull(generator);
            Assert.IsInstanceOf<ValidValueGeneratorLookupList>(generator);
        }
       

    }
}

