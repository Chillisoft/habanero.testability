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
using System.Collections;
using System.ComponentModel;
using System.IO;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace Habanero.Testability.Tests
{
    [TestFixture]
    public class TestValidValueGeneratorTextFile
    {
        private const string BadFileName = "nonexistentfile.txt";
        private const string GoodFileName = "temptextfile.txt";

        [SetUp]
        public void Setup()
        {
            ValidValueGeneratorTextFile.CachedSampleValues.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            RemoveFileIfExists(GoodFileName);
        }

        [Test]
        public void Test_Construct_WithFileName_StoresFileName()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var valueGenerator = new ValidValueGeneratorTextFile(GetPropDef(), GoodFileName);
            //---------------Test Result -----------------------
            Assert.AreSame(GoodFileName, valueGenerator.FileName);
            Assert.IsNotNull(ValidValueGeneratorTextFile.CachedSampleValues);
            Assert.AreEqual(0, ValidValueGeneratorTextFile.CachedSampleValues.Count);
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
            Assert.AreSame(propDef, valueGenerator.SingleValueDef);
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
                StringAssert.Contains("singleValueDef", ex.ParamName);
            }
        }

        [Test]
        public void Test_GenerateValue_WhenFileNotExists_ShouldThrowException()
        {
            //---------------Set up test pack-------------------
            var valueGenerator = new ValidValueGeneratorTextFile(GetPropDef(), BadFileName);
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

        [Test]
        public void Test_GenerateValue_WhenFileIsEmpty_ShouldThrowException()
        {
            //---------------Set up test pack-------------------
            CreateFile(GoodFileName);
            var valueGenerator = new ValidValueGeneratorTextFile(GetPropDef(), GoodFileName);
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
                    "The test data file " + GoodFileName +
                    " does not contain any data to use for random data generation.",
                    ex.Message);
            }
        }

        [Test]
        public void Test_GenerateValue_OneValue_ShouldSet()
        {
            //---------------Set up test pack-------------------
            const string fileName = GoodFileName;
            var items = GetRandomItems(1);
            CreateFile(fileName, items);
            var valueGenerator = CreateValueGenerator(fileName);
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, items.Count);
            //---------------Execute Test ----------------------
            object value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(string), value);
        }

        [Test]
        public void Test_GenerateValue_HundredValues_ShouldUseOneAndLaterGetDifferentValue()
        {
            //---------------Set up test pack-------------------
            const string fileName = GoodFileName;
            const int numberOfItems = 100;
            var items = GetRandomItems(numberOfItems);
            CreateFile(fileName, items);
            var valueGenerator = CreateValueGenerator(fileName);
            //---------------Assert Precondition----------------
            object firstValue = valueGenerator.GenerateValidValue();
            Assert.AreEqual(numberOfItems, items.Count);
            Assert.IsNotNull(firstValue);
            //---------------Execute Test ----------------------
            object nextValue = firstValue;
            int counter = 0;
            while (nextValue.ToString() == firstValue.ToString())
            {
                nextValue = valueGenerator.GenerateValidValue();
                counter++;
                if (counter > 50) //unlikely to have the first item 50 times in a row!
                {
                    Assert.Fail("In a file with many different strings, several attempts to get a next random value produced the same as the original.");
                }
            }
            //---------------Test Result -----------------------
            Assert.Contains(nextValue, items);
        }

        [Test]
        public void Test_GenerateValue_SecondCallShouldNotReloadFile_OneInstance()
        {
            //---------------Set up test pack-------------------
            const string fileName = GoodFileName;
            const int numberOfItems1 = 10;
            const int numberOfItems2 = 11;
            var items = GetRandomItems(numberOfItems1);
            CreateFile(fileName, items);
            var valueGenerator = CreateValueGenerator(fileName);
            //---------------Assert Precondition----------------
            valueGenerator.GenerateValidValue();
            Assert.AreEqual(numberOfItems1, valueGenerator.SampleValues.Count);
            //---------------Execute Test ----------------------
            RemoveFileIfExists(fileName);
            items = GetRandomItems(numberOfItems2);
            CreateFile(fileName, items);
            valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreEqual(numberOfItems1, valueGenerator.SampleValues.Count, "The generator should be caching the sample values read from the text file.");
        }

        [Test]
        public void Test_GenerateValue_SecondCallShouldNotReloadFile_TwoInstances()
        {
            //---------------Set up test pack-------------------
            const string fileName = GoodFileName;
            const int numberOfItems1 = 10;
            const int numberOfItems2 = 11;
            var items = GetRandomItems(numberOfItems1);
            CreateFile(fileName, items);
            var valueGenerator = CreateValueGenerator(fileName);
            //---------------Assert Precondition----------------
            valueGenerator.GenerateValidValue();
            Assert.AreEqual(numberOfItems1, valueGenerator.SampleValues.Count);
            Assert.AreEqual(1, ValidValueGeneratorTextFile.CachedSampleValues.Count);
            //---------------Execute Test ----------------------
            RemoveFileIfExists(fileName);
            items = GetRandomItems(numberOfItems2);
            CreateFile(fileName, items);
            valueGenerator = CreateValueGenerator(fileName);
            valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreEqual(numberOfItems1, valueGenerator.SampleValues.Count, "The generator should be caching the sample values read from the text file.");
            Assert.AreEqual(1, ValidValueGeneratorTextFile.CachedSampleValues.Count);
        }

        [Test]
        public void Test_GenerateValue_ShouldConvertToCorrectType_Int32()
        {
            //---------------Set up test pack-------------------
            const string fileName = GoodFileName;
            var items = new List<string> {"1", "2"};
            CreateFile(fileName, items);
            var propDef = GetPropDef();
            propDef.PropertyType = typeof (Int32);
            var valueGenerator = CreateValueGenerator(fileName, propDef);
            //---------------Assert Precondition----------------
            
            //---------------Execute Test ----------------------
            var value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf(typeof(Int32), value);
        }

        [Ignore("Does not work See DoubleConverter_ShouldConvertStringToDouble below")] //TODO Brett 01 May 2013: Ignored Test - Does not work
        [Test]
        public void Test_GenerateValue_ShouldConvertToCorrectType_Double()
        {
            //---------------Set up test pack-------------------
            const string fileName = GoodFileName;
            var items = new List<string> { "1.1", "2.1" };
            CreateFile(fileName, items);
            var propDef = GetPropDef();
            propDef.PropertyType = typeof(double);
            var valueGenerator = CreateValueGenerator(fileName, propDef);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf(typeof(double), value);
        }
        [Ignore("Does not work")] //TODO Brett 01 May 2013: Ignored Test - Does not work
        [Test]
        public void DoubleConverter_ShouldConvertStringToDouble()
        {
            //---------------Set up test pack-------------------
            var typeConverter = TypeDescriptor.GetConverter(typeof(double));
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var convertedDouble = typeConverter.ConvertTo("1.1", typeof (double));
            //---------------Test Result -----------------------
            Assert.AreEqual(1.1, convertedDouble);
            Assert.Fail("Test Not Yet Implemented");
        }
        [Test]
        public void Test_GenerateValue_ShouldConvertToCorrectType_CustomType_ConvertsToString() //ie. type has no type converter
        {
            //---------------Set up test pack-------------------
            const string fileName = GoodFileName;
            var items = new List<string> { "1.1", "2.1" };
            CreateFile(fileName, items);
            var propDef = GetPropDef();
            propDef.PropertyType = typeof(DummyWithNoTypeConverter);
            var valueGenerator = CreateValueGenerator(fileName, propDef);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var value = valueGenerator.GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsInstanceOf(typeof(string), value);
        }

        private static IPropDef GetPropDef()
        {
            return new PropDefFake
            {
                PropertyType = typeof(string)
            };
        }

        private static ValidValueGeneratorTextFile CreateValueGenerator(string fileName, IPropDef propDef = null)
        {
            if (propDef == null)
            {
                propDef = new PropDefFake
                              {
                                  PropertyType = typeof (string)
                              };
            }
            return new ValidValueGeneratorTextFile(propDef, fileName);
        }

        private static void CreateFile(string fileName, List<string> items = null)
        {
            RemoveFileIfExists(fileName);
            StreamWriter streamWriter = File.CreateText(fileName);
            if (items != null)
            {
                foreach (var item in items)
                {
                    streamWriter.WriteLine(item);
                }
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

        private static List<string> GetRandomItems(int numberOfItems)
        {
            var items = new List<string>();
            for (var i = 0; i < numberOfItems; i++)
            {
                items.Add(RandomValueGen.GetRandomString());
            }
            return items;
        }

        private class ValidValueGeneratorTextFileSpy : ValidValueGeneratorTextFile
        {
            public ValidValueGeneratorTextFileSpy(IPropDef propDef, string fileName)
                : base(propDef, fileName)
            {
            }
/*
            public ISingleValueDef GetPropDef()
            {
                return this.SingleValueDef;
            }*/
        }

        private class DummyWithNoTypeConverter
        {
            
        }
    }
}