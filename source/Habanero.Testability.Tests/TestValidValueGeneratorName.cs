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
using System.Linq;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;

namespace Habanero.Testability.Tests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class TestValidValueGeneratorName
    {
        [TestFixtureSetUp]
        public void SetFixtureUp()
        {
            BORegistry.DataAccessor = new DataAccessorInMemory();
            AllClassesAutoMapper.ClassDefCol.Clear();
            ClassDefCol classDefCol = typeof (FakeBO).MapClasses(type => type.Name != "Unmapped");
            ClassDef.ClassDefs.Add(classDefCol);
            BOTestFactoryRegistry.Instance.ClearAll();
            BOTestFactoryRegistry.Instance.Register<FakeBO2>(typeof (FakeBO2TestFactory));
        }

        [Test]
        public void Test_Construct_WithList_ShouldSetTheList()
        {
            //---------------Set up test pack-------------------
            IList<string> companyNames = GetCompanyNames();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var valueGenerator = new ValidValueGeneratorName(GetPropDef(), companyNames);
            //---------------Test Result -----------------------
            Assert.AreSame(companyNames, valueGenerator.NameList);
        }

        [Test]
        public void Test_Construct_WithNullLIst_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            IList<string> namesIsNull = null;

            //---------------Assert Precondition----------------
            Assert.IsNull(namesIsNull);
            //---------------Execute Test ----------------------
            try
            {
                new ValidValueGeneratorName(GetPropDef(), namesIsNull);
                Assert.Fail("expected ArgumentNullException");
            }
                //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("names", ex.ParamName);
            }
        }

        [Test]
        public void Test_Construct_WithPropDef_ShouldSetPropDef()
        {
            //---------------Set up test pack-------------------
            var propDef = GetPropDef();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var valueGenerator = new ValidValueGeneratorNameSpy(propDef, GetEmptyList());
            //---------------Test Result -----------------------
            Assert.AreSame(propDef, valueGenerator.GetPropDef());
        }

        [Test]
        public void Test_Construct_WithPropDefNull_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            IPropDef propDefIsNull = null;
            //---------------Assert Precondition----------------
            Assert.IsNull(propDefIsNull);
            //---------------Execute Test ----------------------
            try
            {
                new ValidValueGeneratorNameSpy(propDefIsNull, GetEmptyList());
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
        public void Test_Construct_WithPropDefNotString_ShouldRaiseError()
        {
            //---------------Set up test pack-------------------
            var propDef = (IPropDef) new PropDefFake
                                         {
                                             PropertyType = typeof (bool)
                                         };
            //---------------Assert Precondition----------------
            Assert.AreSame(typeof (bool), propDef.PropertyType);
            //---------------Execute Test ----------------------
            try
            {
                new ValidValueGeneratorName(propDef, GetEmptyList());
                Assert.Fail("Expected to throw an HabaneroArgumentException");
            }
                //---------------Test Result -----------------------
            catch (HabaneroArgumentException ex)
            {
                StringAssert.Contains(
                    "You cannot use a ValidValueGeneratorName for generating values for a property that is not a string.",
                    ex.Message);
            }
        }

        [Test]
        public void Test_GenerateValue_WhenListEmpty_ShouldGenerateRandomString()
        {
            //---------------Set up test pack-------------------

            ValidValueGenerator valueGenerator = CreateNameValidValueGenerator(GetPropDef(), GetEmptyList());
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            object value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
        }

        [Test]
        public void Test_GenerateValue_WhenString_ShouldSet()
        {
            //---------------Set up test pack-------------------
            Type propertyTypeString = typeof (string);
            IPropDef def = new PropDefFake
                               {
                                   PropertyType = propertyTypeString
                               };
            var valueGenerator = CreateNameValidValueGenerator(def, GetCompanyNames());
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(propertyTypeString, value);
        }

        [Test]
        public void Test_GenerateValue_WhenCalledSecondTime_ShouldGenDifferentName()
        {
            //---------------Set up test pack-------------------
            var valueGenerator = CreateValidValueGenerator(GetPropDef());
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var value1 = valueGenerator.GenerateValidValue();
            var value2 = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreNotEqual(value2, value1);
        }

        [Test]
        public void
            Test_GenerateValue_WhenNumberOfCallsExceedsTheNumberOfItemsInTheList_ShouldStartGeneratingFromTheBeginning()
        {
            //---------------Set up test pack-------------------
            var valueGenerator = CreateValidValueGenerator(GetPropDef());
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var value1 = valueGenerator.GenerateValidValue();
            var value2 = valueGenerator.GenerateValidValue();
            var value3 = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreNotEqual(value2, value1);
            Assert.AreEqual(value3, value1);
        }

        [Test]
        public void Test_GenerateValue_WhenUseDifferentGeneratorInstances_ButSamePropDef_ShouldGenerateDifferentValues()
        {
            //---------------Set up test pack-------------------
            var propDef = GetPropDef();
            var valueGenerator1 = CreateValidValueGenerator(propDef);
            var valueGenerator2 = CreateValidValueGenerator(propDef);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var valueFromGenerator1 = valueGenerator1.GenerateValidValue();
            var valueFromGenerator2 = valueGenerator2.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreNotEqual(valueFromGenerator2, valueFromGenerator1);
        }

        /// <summary>
        /// If different prop defs are being used the they should each have their own incrementer.
        /// This is essential when we have large trees of objects being created all using the NameGenerator.
        /// In this cases we want do not want to loop through the list too fast else we could get duplicate data
        /// e.g. where we have Unique Constraints.
        /// </summary>
        [Test]
        public void Test_GenerateValue_WhenUseDifferentPropDefs_ShouldNotGenerateSequentialValues_FixBug1267()
        {
            //---------------Set up test pack-------------------
            var propDef1 = GetPropDef();
            var propDef2 = GetPropDef();
            var valueGenerator1 = CreateValidValueGenerator(propDef1);
            var valueGenerator2 = CreateValidValueGenerator(propDef2);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var valueFromGenerator1 = valueGenerator1.GenerateValidValue();
            var valueFromGenerator2 = valueGenerator2.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreEqual(valueFromGenerator2, valueFromGenerator1, "Should be equal");
        }

        [Test]
        public void Test_CreateNewRandomFakeBO_WhenRegisterFirstNameGenerator_ShouldWithCreateSensibleFirstName()
        {
            //---------------Set up test pack-------------------
            var propDef = GetPropDef();
            var firstNameList = new FirstNameValidValueGenerator(propDef).NameList;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var boTestFactory = GetFactory<FakeBO>();
            boTestFactory.SetValidValueGenerator(bo => bo.NonCompulsoryString, typeof (FirstNameValidValueGenerator));
            var randomFakeBO = boTestFactory
                .CreateSavedBusinessObject();
            //---------------Test Result -----------------------
            AssertPropValueIsFromList(randomFakeBO, "NonCompulsoryString", firstNameList);
        }

        [Test]
        public void Test_CreateNewRandomFakeBO_FirstTime_ShouldWithCreateSensibleFirstName()
        {
            //---------------Set up test pack-------------------
            var propDef = GetPropDef();
            var firstNameList = new FirstNameValidValueGenerator(propDef).NameList;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomFakeBO = CreateNewRandomFakeBO();
            //---------------Test Result -----------------------
            AssertPropValueIsFromList(randomFakeBO, "NonCompulsoryString", firstNameList);
        }

        [Test]
        public void Test_CreateFakeBO_SecondTime_ShouldCreateWithSensibleFirstName()
        {
            //---------------Set up test pack-------------------
            var propDef = GetPropDef();
            var firstNameList = new FirstNameValidValueGenerator(propDef).NameList;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomFakeBO = CreateNewRandomFakeBO();
            //---------------Test Result -----------------------
            AssertPropValueIsFromList(randomFakeBO, "NonCompulsoryString", firstNameList);
        }

        /// <summary>
        /// This test is purely to prove that the AssertPropValueIsFromList is correct.
        /// </summary>
        [Test]
        public void Test_CreateFakeBO_CheckAssertShouldCreateWithSensibleName()
        {
            //---------------Set up test pack-------------------
            var propDef = GetPropDef();
            var firstNameList = new FirstNameValidValueGenerator(propDef).NameList;
            //---------------Assert Precondition----------------
            Assert.Greater(firstNameList.Count, 3);
            //---------------Execute Test ----------------------
            var randomFakeBO = CreateNewRandomFakeBO();
            randomFakeBO.NonCompulsoryString = "Tom";
            //---------------Test Result -----------------------
            AssertPropValueIsFromList(randomFakeBO, "NonCompulsoryString", firstNameList);
        }

        private void AssertPropValueIsFromList(IBusinessObject randomFakeBO, string propName, IList<string> nameList)
        {
            BOPropertyMapper propertyMapper = new BOPropertyMapper(propName) {BusinessObject = randomFakeBO};
            string propertyValue = propertyMapper.GetPropertyValue() as string;
            var message = string.Format("The property '{0}' value '{1}' should be from the list of available names",
                                        propName, propertyValue);
            Assert.IsTrue(nameList.Contains(propertyValue), message);

            //Assert.IsFalse(parsed, message);
        }

        private static FakeBO2 CreateNewRandomFakeBO()
        {
            var boTestFactory = GetFactory<FakeBO2>();
            return boTestFactory.CreateSavedBusinessObject();
        }

        private static BOTestFactory<T> GetFactory<T>() where T : class, IBusinessObject
        {
            return BOTestFactoryRegistry.Instance.Resolve<T>();
        }

        private static IPropDef GetPropDef()
        {
            return new PropDefFake
                       {
                           PropertyType = typeof (string)
                       };
        }

        private static ValidValueGenerator CreateValidValueGenerator(IPropDef propDef)
        {
            IList<string> companyNames = GetCompanyNames();
            return CreateNameValidValueGenerator(propDef, companyNames);
        }

        private static IList<string> GetCompanyNames()
        {
            return new List<string> {"Hulamin", "Chilisoft"};
        }

        private static ValidValueGeneratorName CreateNameValidValueGenerator(IPropDef def, IList<string> companyNames)
        {
            return new ValidValueGeneratorName(def, companyNames);
        }

        private static List<string> GetEmptyList()
        {
            return new List<string>();
        }

        private class FakeBO2TestFactory : BOTestFactory<FakeBO2>
        {
            public FakeBO2TestFactory()
            {
                SetupValidValueGenerators();
            }

            public FakeBO2TestFactory(FakeBO2 bo)
                : base(bo)
            {
                SetupValidValueGenerators();
            }

            private void SetupValidValueGenerators()
            {
                this.SetValidValueGenerator(source => source.NonCompulsoryString, typeof (FirstNameValidValueGenerator));
            }
        }
    }


    public class FirstNameValidValueGenerator : ValidValueGeneratorName
    {
        private static readonly List<string> _names = new List<string>
                                                          {
                                                              "Tom",
                                                              "Jack",
                                                              "Harry",
                                                              "Leslie",
                                                              "Mitch",
                                                              "Gary"
                                                          };

        public FirstNameValidValueGenerator(IPropDef propDef)
            : base(propDef, _names)
        {
        }
    }

    public class ValidValueGeneratorNameSpy : ValidValueGeneratorName
    {
        public ValidValueGeneratorNameSpy(IPropDef propDef, List<string> list) : base(propDef, list)
        {
        }

        public ISingleValueDef GetPropDef()
        {
            return this.SingleValueDef;
        }
    }

    // ReSharper restore InconsistentNaming
}