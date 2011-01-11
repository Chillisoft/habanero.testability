using System;
using System.Collections;
using System.IO;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;

//TODO ERIC:
// - finish random generation of items from textfile
// - build sample data files (eg. first names, countries, etc)
// - load data once into memory to speed up tests
// - investigate pattern language for generating specific data


namespace Habanero.Testability.Tests
{
    public class ValidValueGeneratorTextFile : ValidValueGenerator
    {
        public ValidValueGeneratorTextFile(IPropDef propDef) : this(propDef, null)
        {
        }
        public ValidValueGeneratorTextFile(IPropDef propDef, string fileName) : base(propDef)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = propDef.PropertyName + ".txt";
            }
            FileName = fileName;
        }

        public string FileName { get; private set; }

        public override object GenerateValidValue()
        {
            var items = ReadItemsFromFile();
            if (items.Count == 0)
            {
                throw new HabaneroApplicationException("The test data file " + FileName + " does not contain any data to use for random data generation.");
            }
            return items[RandomValueGen.GetRandomInt(items.Count - 1)];
        }

        private ArrayList ReadItemsFromFile()
        {
            StreamReader streamReader = File.OpenText(FileName);
            var items = new ArrayList();
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                if (line == null) break;
                items.Add(line);
            }
            streamReader.Close();
            return items;
        }
    }

    [TestFixture]
    public class TestValidValueGeneratorTextFile
    {
        private string _badFileName = "nonexistentfile.txt";
        private const string _goodFileName = "temptextfile.txt";

        [TearDown]
        public void TearDown()
        {
            RemoveFileIfExists(_goodFileName);
        }

        [Test]
        public void Test_Construct_WithFileName_StoresFileName()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var valueGenerator = new ValidValueGeneratorTextFile(GetPropDef(), _goodFileName);
            //---------------Test Result -----------------------
            Assert.AreSame(_goodFileName, valueGenerator.FileName);
        }

        [Test]
        public void Test_Construct_WithNullFileName_ShouldDefaultFileName()
        {
            //---------------Set up test pack-------------------
            const string fileName = null;

            //---------------Assert Precondition----------------
            Assert.IsNull(fileName);
            //---------------Execute Test ----------------------
            IPropDef propDef = GetPropDef();
            var valueGenerator = new ValidValueGeneratorTextFile(propDef, fileName);
            //---------------Test Result -----------------------
            Assert.AreEqual(propDef.PropertyName + ".txt", valueGenerator.FileName);
        }

        [Test]
        public void Test_Construct_WithEmptyFileName_ShouldDefaultFileName()
        {
            //---------------Set up test pack-------------------
            const string fileName = null;

            //---------------Assert Precondition----------------
            Assert.IsNull(fileName);
            //---------------Execute Test ----------------------
            IPropDef propDef = GetPropDef();
            var valueGenerator = new ValidValueGeneratorTextFile(propDef, "");
            //---------------Test Result -----------------------
            Assert.AreEqual(propDef.PropertyName + ".txt", valueGenerator.FileName);
        }

        [Test]
        public void Test_Construct_WithNoFileName_ShouldDefaultFileName()
        {
            //---------------Set up test pack-------------------
            const string fileName = null;

            //---------------Assert Precondition----------------
            Assert.IsNull(fileName);
            //---------------Execute Test ----------------------
            IPropDef propDef = GetPropDef();
            var valueGenerator = new ValidValueGeneratorTextFile(propDef);
            //---------------Test Result -----------------------
            Assert.AreEqual(propDef.PropertyName + ".txt", valueGenerator.FileName);
        }

        [Test]
        public void Test_Construct_WithPropDef_ShouldSetPropDef()
        {
            //---------------Set up test pack-------------------
            var propDef = GetPropDef();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var valueGenerator = new ValidValueGeneratorTextFileSpy(propDef, "");
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
                new ValidValueGeneratorTextFileSpy(propDefIsNull, "");
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("propDef", ex.ParamName);
            }
        }

        //[Test]
        //public void Test_Construct_WithPropDefNotString_ShouldRaiseError()
        //{
        //    //---------------Set up test pack-------------------
        //    var propDef = (IPropDef)new PropDefFake
        //    {
        //        PropertyType = typeof(bool)
        //    };
        //    //---------------Assert Precondition----------------
        //    Assert.AreSame(typeof(bool), propDef.PropertyType);
        //    //---------------Execute Test ----------------------
        //    try
        //    {
        //        new ValidValueGeneratorName(propDef, GetEmptyList());
        //        Assert.Fail("Expected to throw an HabaneroArgumentException");
        //    }
        //    //---------------Test Result -----------------------
        //    catch (HabaneroArgumentException ex)
        //    {
        //        StringAssert.Contains(
        //            "You cannot use a ValidValueGeneratorName for generating values for a property that is not a string.",
        //            ex.Message);
        //    }
        //}

        [Test, Ignore("TODO ERIC - work in progress")]
        public void Test_GenerateValue_WhenFileNotExists_ShouldThrowException()
        {
            //---------------Set up test pack-------------------
            var valueGenerator = new ValidValueGeneratorTextFile(GetPropDef(), _badFileName);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                var value = valueGenerator.GenerateValidValue();
                Assert.Fail("Expected to throw a FileNotFoundException");
            }
            //---------------Test Result -----------------------
            catch (FileNotFoundException ex)
            {
                StringAssert.Contains(
                    "Could not find file",
                    ex.Message);
            }
        }

        [Test, Ignore("TODO ERIC - work in progress")]
        public void Test_GenerateValue_WhenFileIsEmpty_ShouldThrowException()
        {
            //---------------Set up test pack-------------------
            CreateFile(_goodFileName);
            _badFileName = "nonexistentfile.txt";
            var valueGenerator = new ValidValueGeneratorTextFile(GetPropDef(), _goodFileName);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                var value = valueGenerator.GenerateValidValue();
                Assert.Fail("Expected to throw a HabaneroApplicationException");
            }
            //---------------Test Result -----------------------
            catch (HabaneroApplicationException ex)
            {
                StringAssert.Contains(
                    "The test data file " + _goodFileName +
                    " does not contain any data to use for random data generation.",
                    ex.Message);
            }
        }

        [Test,Ignore("TODO ERIC - work in progress")]
        public void Test_GenerateValue_OneValue_ShouldSet()
        {
            //---------------Set up test pack-------------------
            const string fileName = _goodFileName;
            var items = GetRandomItems(1);
            CreateFile(fileName, items);
            ValidValueGenerator valueGenerator;
            Type propertyTypeString = CreateValueGenerator(fileName, out valueGenerator);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, items.Count);
            //---------------Execute Test ----------------------
            object value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(propertyTypeString, value);
        }

        [Test, Ignore("TODO ERIC - work in progress")]
        public void Test_GenerateValue_TenValues_ShouldUseOne()
        {
            //---------------Set up test pack-------------------
            const string fileName = _goodFileName;
            var numberOfItems = 10;
            var items = GetRandomItems(numberOfItems);
            CreateFile(fileName, items);
            ValidValueGenerator valueGenerator;
            Type propertyTypeString = CreateValueGenerator(fileName, out valueGenerator);
            //---------------Assert Precondition----------------
            Assert.AreEqual(numberOfItems, items.Count);
            //---------------Execute Test ----------------------
            object value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(propertyTypeString, value);
            Assert.Contains(value, items);
        }

        //[Test]
        //public void Test_GenerateValue_WhenCalledSecondTime_ShouldGenDifferentName()
        //{
        //    //---------------Set up test pack-------------------
        //    ValidValueGenerator valueGenerator = CreateValidValueGenerator();
        //    //---------------Assert Precondition----------------
        //    //---------------Execute Test ----------------------
        //    object value1 = valueGenerator.GenerateValidValue();
        //    object value2 = valueGenerator.GenerateValidValue();
        //    //---------------Test Result -----------------------
        //    Assert.AreNotEqual(value2, value1);
        //}

        //[Test]
        //public void
        //    Test_GenerateValue_WhenNumberOfCallsExceedsTheNumberOfItemsInTheList_ShouldStartGeneratingFromTheBeginning()
        //{
        //    //---------------Set up test pack-------------------
        //    ValidValueGenerator valueGenerator = CreateValidValueGenerator();
        //    //---------------Assert Precondition----------------
        //    //---------------Execute Test ----------------------
        //    object value1 = valueGenerator.GenerateValidValue();
        //    object value2 = valueGenerator.GenerateValidValue();
        //    object value3 = valueGenerator.GenerateValidValue();
        //    //---------------Test Result -----------------------
        //    Assert.AreNotEqual(value2, value1);
        //    Assert.AreEqual(value3, value1);
        //}

        //[Test]
        //public void Test_GenerateValue_WhenUseDifferentGeneratorInstances_ShouldGenerateDifferentValues()
        //{
        //    //---------------Set up test pack-------------------
        //    ValidValueGenerator valueGenerator1 = CreateValidValueGenerator();

        //    ValidValueGenerator valueGenerator2 = CreateValidValueGenerator();

        //    //---------------Assert Precondition----------------

        //    //---------------Execute Test ----------------------
        //    object valueFromGenerator1 = valueGenerator1.GenerateValidValue();
        //    object valueFromGenerator2 = valueGenerator2.GenerateValidValue();
        //    //---------------Test Result -----------------------
        //    Assert.AreNotEqual(valueFromGenerator2, valueFromGenerator1);
        //}

        private static IPropDef GetPropDef()
        {
            return new PropDefFake
            {
                PropertyType = typeof(string)
            };
        }

        private static Type CreateValueGenerator(string fileName, out ValidValueGenerator valueGenerator)
        {
            var propertyTypeString = typeof(string);
            IPropDef def = new PropDefFake
            {
                PropertyType = propertyTypeString
            };
            valueGenerator = new ValidValueGeneratorTextFile(def, fileName);
            return propertyTypeString;
        }

        //private static ValidValueGenerator CreateValidValueGenerator()
        //{
        //    IList<string> companyNames = GetCompanyNames();
        //    return CreateNameValidValueGenerator(GetPropDef(), companyNames);
        //}

        //private static IList<string> GetCompanyNames()
        //{
        //    return new List<string> { "Hulamin", "Chilisoft" };
        //}

        //private static ValidValueGeneratorName CreateNameValidValueGenerator(IPropDef def, IList<string> companyNames)
        //{
        //    return new ValidValueGeneratorName(def, companyNames);
        //}

        //private static List<string> GetEmptyList()
        //{
        //    return new List<string>();
        //}

        private static void CreateFile(string fileName)
        {
            CreateFile(fileName, null);
        }
        private static void CreateFile(string fileName, ArrayList items)
        {
            RemoveFileIfExists(fileName);
            StreamWriter streamWriter = File.CreateText(fileName);
            if (items == null) return;
            foreach (var item in items)
            {
                streamWriter.WriteLine(item);
            }
            streamWriter.Close();
        }

        private static void RemoveFileIfExists(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        private static ArrayList GetRandomItems(int numberOfItems)
        {
            var items = new ArrayList();
            for (var i = 0; i < numberOfItems; i++)
            {
                items.Add(RandomValueGen.GetRandomString());
            }
            return items;
        }

        public class ValidValueGeneratorTextFileSpy : ValidValueGeneratorTextFile
        {
            public ValidValueGeneratorTextFileSpy(IPropDef propDef, string fileName)
                : base(propDef, fileName)
            {
            }

            public IPropDef GetPropDef()
            {
                return this.PropDef;
            }
        }
    }
}