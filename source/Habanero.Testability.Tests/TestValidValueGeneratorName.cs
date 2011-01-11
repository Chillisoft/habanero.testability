using System;
using System.Collections.Generic;
using System.Linq;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;

namespace Habanero.Testability.Tests
{
    [TestFixture]
    public class TestValidValueGeneratorName
    {
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
                StringAssert.Contains("propDef", ex.ParamName);
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
            ValidValueGenerator valueGenerator = CreateNameValidValueGenerator(def, GetCompanyNames());
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            object value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(propertyTypeString, value);
        }

        [Test]
        public void Test_GenerateValue_WhenCalledSecondTime_ShouldGenDifferentName()
        {
            //---------------Set up test pack-------------------
            ValidValueGenerator valueGenerator = CreateValidValueGenerator();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            object value1 = valueGenerator.GenerateValidValue();
            object value2 = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreNotEqual(value2, value1);
        }

        [Test]
        public void
            Test_GenerateValue_WhenNumberOfCallsExceedsTheNumberOfItemsInTheList_ShouldStartGeneratingFromTheBeginning()
        {
            //---------------Set up test pack-------------------
            ValidValueGenerator valueGenerator = CreateValidValueGenerator();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            object value1 = valueGenerator.GenerateValidValue();
            object value2 = valueGenerator.GenerateValidValue();
            object value3 = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreNotEqual(value2, value1);
            Assert.AreEqual(value3, value1);
        }

        [Test]
        public void Test_GenerateValue_WhenUseDifferentGeneratorInstances_ShouldGenerateDifferentValues()
        {
            //---------------Set up test pack-------------------
            ValidValueGenerator valueGenerator1 = CreateValidValueGenerator();

            ValidValueGenerator valueGenerator2 = CreateValidValueGenerator();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            object valueFromGenerator1 = valueGenerator1.GenerateValidValue();
            object valueFromGenerator2 = valueGenerator2.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreNotEqual(valueFromGenerator2, valueFromGenerator1);
        }

        private static IPropDef GetPropDef()
        {
            return new PropDefFake
                       {
                           PropertyType = typeof (string)
                       };
        }

        private static ValidValueGenerator CreateValidValueGenerator()
        {
            IList<string> companyNames = GetCompanyNames();
            return CreateNameValidValueGenerator(GetPropDef(), companyNames);
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
    }

    public class ValidValueGeneratorNameSpy : ValidValueGeneratorName
    {
        public ValidValueGeneratorNameSpy(IPropDef propDef, List<string> list) : base(propDef, list)
        {
        }

        public IPropDef GetPropDef()
        {
            return this.PropDef;
        }
    }
}