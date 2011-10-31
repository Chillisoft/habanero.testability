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
using System.Collections.Generic;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability.Helpers;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;

namespace Habanero.Testability.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestValidValueGeneratorFromList
    {
        [TestFixtureSetUp]
        public void SetFixtureUp()
        {
            BORegistry.DataAccessor = new DataAccessorInMemory();
            AllClassesAutoMapper.ClassDefCol.Clear();
            ClassDefCol classDefCol = typeof(FakeBO).MapClasses(type => type.Name != "Unmapped");
            ClassDef.ClassDefs.Add(classDefCol);
            BOTestFactoryRegistry.Instance.ClearAll();/*
            BOTestFactoryRegistry.Instance.Register<FakeBO2>(typeof(FakeBO2TestFactory));*/
        }
        [SetUp]
        public void SetUp()
        {
            BORegistry.DataAccessor = new DataAccessorInMemory();
        }
        private static BusinessObjectCollection<FakeBO2> GetList()
        {
            return new BusinessObjectCollection<FakeBO2> { new FakeBO2(), new FakeBO2() };
        }

        private static FakeBOWSingleRelToFakeBO2 CreateNewRandomFakeBO()
        {
            var boTestFactory = GetFactory<FakeBOWSingleRelToFakeBO2>();
            boTestFactory.SetValidValueGenerator(bo => bo.FakeBO2, typeof(ValidValueGeneratorFromBOList<FakeBO2>));
            return boTestFactory.CreateSavedBusinessObject();
        }

        private static BOTestFactory<T> GetFactory<T>() where T : class, IBusinessObject
        {
            return BOTestFactoryRegistry.Instance.Resolve<T>();
        }

        private static ISingleValueDef GetRelDef<T>()
        {
            return new PropDefFake
            {
                PropertyType = typeof(T)
            };
        }

        private static ValidValueGeneratorFromBOList<FakeBO2> CreateValidValueGenerator(ISingleValueDef relDef)
        {
            var list = GetList();
            return CreateValidValueGenerator(relDef, list);
        }


        private static ValidValueGeneratorFromBOList<T> CreateValidValueGenerator<T>(ISingleValueDef def, BusinessObjectCollection<T> list) where T : class, IBusinessObject, new()
        {
            return new ValidValueGeneratorFromBOList<T>(def, list);
        }

        private static BusinessObjectCollection<T> GetEmptyList<T>() where T : class, IBusinessObject, new()
        {
            return new BusinessObjectCollection<T>();
        }

        private static BusinessObjectCollection<FakeBO2> LoadBOs()
        {
            var availableItemsList = new BusinessObjectCollection<FakeBO2>();
            availableItemsList.LoadAll();
            return availableItemsList;
        }

        private static List<FakeBO2> Create3SavedBOs()
        {
            return new List<FakeBO2> {CreateSavedBO(), CreateSavedBO(), CreateSavedBO()};
        }

        private static FakeBO2 CreateSavedBO()
        {
            return new FakeBO2().Save() as FakeBO2;
        }

        private void AssertPropValueIsFromList<T>(IBusinessObject randomFakeBO, string propName, IList<T> itemsList) where T : class, IBusinessObject, new()
        {
            var relationship = randomFakeBO.Relationships[propName] as SingleRelationship<T>;
            Assert.IsNotNull(relationship, "The single relationship '" + propName + "' must exist on the businessobject");
            var propertyValue = relationship.GetRelatedObject();
            /*           BOPropertyMapper propertyMapper = new BOPropertyMapper(propName) { BusinessObject = randomFakeBO };
                       T propertyValue = (T) propertyMapper.GetPropertyValue();*/
            var message = string.Format("The property '{0}' value '{1}' should be from the list of available Items",
                                                   propName, propertyValue);
            Assert.IsTrue(itemsList.Contains(propertyValue), message);

            //Assert.IsFalse(parsed, message);
        }

        [Test]
        public void Test_Construct_WithList_ShouldSetTheList()
        {
            //---------------Set up test pack-------------------
            var availableItems = GetList();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var valueGenerator = new ValidValueGeneratorFromBOList<FakeBO2>(GetRelDef<FakeBO2>(), availableItems);
            //---------------Test Result -----------------------
            Assert.AreSame(availableItems, valueGenerator.AvailableItemsList);
        }

        [Test]
        public void Test_Construct_WithNullLIst_LoadListFromDataStore()
        {
            //---------------Set up test pack-------------------
            var bo1 = CreateSavedBO();
            var bo2 = CreateSavedBO();
            //---------------Assert Precondition----------------
            
            //---------------Execute Test ----------------------

            var validValueGeneratorFromLoadedList = new ValidValueGeneratorFromBOList<FakeBO2>(GetRelDef<FakeBO2>(), null);
            //---------------Test Result -----------------------
            var availableItemsList = validValueGeneratorFromLoadedList.AvailableItemsList;
            Assert.IsNotNull(availableItemsList);
            Assert.IsTrue(availableItemsList.Contains(bo1));
            Assert.IsTrue(availableItemsList.Contains(bo2));

        }

        [Test]
        public void Test_Construct_WithoutAList_LoadListFromDataStore()
        {
            //---------------Set up test pack-------------------
            new FakeBO2().Save();
            new FakeBO2().Save();
            //---------------Assert Precondition----------------
            
            //---------------Execute Test ----------------------

            var validValueGeneratorFromLoadedList = new ValidValueGeneratorFromBOList<FakeBO2>(GetRelDef<FakeBO2>());
            //---------------Test Result -----------------------
            var availableItemsList = validValueGeneratorFromLoadedList.AvailableItemsList;
            Assert.IsNotNull(availableItemsList);

        }

        [Test]
        public void Test_Construct_WithValueDef_ShouldSetValueDef()
        {
            //---------------Set up test pack-------------------

            var relDef = GetRelDef<FakeBO2>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var valueGenerator = new ValidValueGeneratorFromBOList<FakeBO2>(relDef, GetEmptyList<FakeBO2>());
            //---------------Test Result -----------------------
            Assert.AreSame(relDef, valueGenerator.SingleValueDef);
        }


        [Test]
        public void Test_Construct_WithValueDefNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            ISingleValueDef relDefIsNull = null;
            //---------------Assert Precondition----------------
            Assert.IsNull(relDefIsNull);
            //---------------Execute Test ----------------------
            try
            {
                new ValidValueGeneratorFromBOList<FakeBO2>(relDefIsNull, GetEmptyList<FakeBO2>());
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("singleValueDef", ex.ParamName);
            }
        }

        [Test]
        public void Test_Construct_WithValueDefNotFakeBO2_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var relDef = GetRelDef<FakeBO>();
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof(FakeBO), relDef.PropertyType);
            //---------------Execute Test ----------------------
            try
            {
                new ValidValueGeneratorFromBOList<FakeBO2>(relDef, GetEmptyList<FakeBO2>());
                Assert.Fail("Expected to throw an HabaneroArgumentException");
            }
            //---------------Test Result -----------------------
            catch (HabaneroArgumentException ex)
            {
                StringAssert.Contains("You cannot use a ValidValueGeneratorFromList for generating values", ex.Message);
                StringAssert.Contains("since the Property is of type ", ex.Message);
            }
        }

        [Test]
        public void Test_GenerateValue_WhenListEmpty_ShouldGenerateRandomValue()
        {
            //---------------Set up test pack-------------------
            var valueGenerator = CreateValidValueGenerator(GetRelDef<FakeBO2>(), GetEmptyList<FakeBO2>());
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, LoadBOs().Count);
            Assert.AreEqual(0, valueGenerator.AvailableItemsList.Count);
            //---------------Execute Test ----------------------
            object value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf<FakeBO2>(value);
        }

        [Test]
        public void Test_GenerateValue_WhenCalledSecondTime_ShouldGenDifferentName()
        {
            //---------------Set up test pack-------------------
            Create3SavedBOs();
            var valueGenerator = CreateValidValueGenerator(GetRelDef<FakeBO2>());
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var value1 = valueGenerator.GenerateValidValue();
            var value2 = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreNotEqual(value2, value1);
        }
        [Test]
        public void Test_GenerateValue_WhenSavedItems_ShouldReturnSavedItems()
        {
            //---------------Set up test pack-------------------
            var savedItems = Create3SavedBOs();
            var valueGenerator = new ValidValueGeneratorFromBOList<FakeBO2>(GetRelDef<FakeBO2>(), null);
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, LoadBOs().Count);
            Assert.AreEqual(3, valueGenerator.AvailableItemsList.Count);
            //---------------Execute Test ----------------------
            var value1 = valueGenerator.GenerateValidValue() as FakeBO2;
            var value2 = valueGenerator.GenerateValidValue() as FakeBO2;
            //---------------Test Result -----------------------
            savedItems.ShouldContain(value2);
            savedItems.ShouldContain(value1);
        }

        [Test]
        public void
            Test_GenerateValue_WhenNumberOfCallsExceedsTheNumberOfItemsInTheList_ShouldStartGeneratingFromTheBeginning()
        {
            //---------------Set up test pack-------------------
            var fakeBo2s = GetList();
            var valueGenerator = CreateValidValueGenerator(GetRelDef<FakeBO2>(), fakeBo2s);
            //---------------Assert Precondition----------------
            Assert.AreEqual(2, fakeBo2s.Count);
            //---------------Execute Test ----------------------
            var value1 = valueGenerator.GenerateValidValue();
            valueGenerator.GenerateValidValue();
            var value3 = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreEqual(value3, value1);
        }
        [Test]
        public void Test_GenerateValue_WhenUseDifferentGeneratorInstances_ButSameValueDef_ShouldGenerateDifferentValues()
        {
            //---------------Set up test pack-------------------
            var relDef = GetRelDef<FakeBO2>();
            Create3SavedBOs();
            var valueGenerator1 = new ValidValueGeneratorFromBOList<FakeBO2>(relDef, null);
            var valueGenerator2 = new ValidValueGeneratorFromBOList<FakeBO2>(relDef, null);
            //---------------Assert Precondition----------------
            Assert.AreSame(relDef, valueGenerator1.SingleValueDef);
            Assert.AreSame(relDef, valueGenerator1.SingleValueDef);
            Assert.AreNotSame(valueGenerator1, valueGenerator2);
            //---------------Execute Test ----------------------
            var valueFromGenerator1 = valueGenerator1.GenerateValidValue();
            var valueFromGenerator2 = valueGenerator2.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreNotEqual(valueFromGenerator2, valueFromGenerator1);
        }

        /// <summary>
        /// If different prop defs are being used the they should each have their own incrementer.
        /// This is essential when we have large trees of objects being created all using the ValidValueGeneratorFromBOList.
        /// In this cases we want to not want to loop through the list too fast else we could get duplicate data
        /// e.g. where we have Unique Constraints.
        /// </summary>
        [Test]
        public void Test_GenerateValue_WhenUseDifferentValueDefs_ShouldNotGenerateSequentialValues_FixBug1267()
        {
            //---------------Set up test pack-------------------
            var fakeBo2s = GetList();
            var relDef1 = GetRelDef<FakeBO2>();
            var relDef2 = GetRelDef<FakeBO2>();
            var valueGenerator1 = new ValidValueGeneratorFromBOList<FakeBO2>(relDef1, fakeBo2s);
            var valueGenerator2 = new ValidValueGeneratorFromBOList<FakeBO2>(relDef2, fakeBo2s);
            //---------------Assert Precondition----------------
            Assert.AreNotSame(valueGenerator1.SingleValueDef, valueGenerator2.SingleValueDef);
            //---------------Execute Test ----------------------
            var valueFromGenerator1 = valueGenerator1.GenerateValidValue();
            var valueFromGenerator2 = valueGenerator2.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreEqual(valueFromGenerator1, valueFromGenerator2, "Should both return the first value since generating for different prop defs");
        }


        [Test]
        public void Test_CreateNewRandomFakeBO_WhenRegisterGenerator_ShouldWithCreateDataFromList()
        {
            //---------------Set up test pack-------------------
            var create3SavedBOs = Create3SavedBOs();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var boTestFactory = GetFactory<FakeBOWSingleRelToFakeBO2>();
            boTestFactory.SetValidValueGenerator(bo => bo.FakeBO2, typeof(ValidValueGeneratorFromBOList<FakeBO2>));
            var randomBO = boTestFactory.CreateSavedBusinessObject();
            //---------------Test Result -----------------------
            Assert.IsNotNull(randomBO.FakeBO2, "The BO should be generated with a value for its FakeBO2 prop");
            AssertPropValueIsFromList(randomBO, "FakeBO2", create3SavedBOs);
        }


        [Test]
        public void Test_CreateNewRandomFakeBO_FirstTime_ShouldWithCreateFakeBO2FromList()
        {
            //---------------Set up test pack-------------------
            var create3SavedBOs = Create3SavedBOs();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomBO = CreateNewRandomFakeBO();
            //---------------Test Result -----------------------
            AssertPropValueIsFromList(randomBO, "FakeBO2", create3SavedBOs);
        }

        [Test]
        public void Test_CreateFakeBO_SecondTime_ShouldCreateFakeBO2FromList()
        {
            //---------------Set up test pack-------------------
            var create3SavedBOs = Create3SavedBOs();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            CreateNewRandomFakeBO();
            var randomBO2 = CreateNewRandomFakeBO();
            //---------------Test Result -----------------------
            AssertPropValueIsFromList(randomBO2, "FakeBO2", create3SavedBOs);
        }

        /// <summary>
        /// This test is purely to prove that the AssertPropValueIsFromList is correct.
        /// </summary>
        [Test]
        public void Test_CreateFakeBO_CheckAssertShouldCreateBOFromList()
        {
            //---------------Set up test pack-------------------
            var create3SavedBOs = Create3SavedBOs();
            //---------------Assert Precondition----------------
            Assert.Greater(create3SavedBOs.Count, 2);
            //---------------Execute Test ----------------------
            var randomBO = CreateNewRandomFakeBO();
            randomBO.FakeBO2 = create3SavedBOs[1];
            //---------------Test Result -----------------------
            AssertPropValueIsFromList(randomBO, "FakeBO2", create3SavedBOs);
        }
    }
    // ReSharper restore InconsistentNaming
}