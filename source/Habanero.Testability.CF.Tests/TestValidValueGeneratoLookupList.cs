using System;
using System.Collections.Generic;
using System.Linq;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Smooth;
using Habanero.Testability;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;

namespace Habanero.Testability.Tests
{
    [TestFixture]
    public class TestValidValueGeneratoLookupList
    {

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            BORegistry.DataAccessor = new DataAccessorInMemory();
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            ClassDef.ClassDefs.Add(typeof(FakeLListBO).MapClass());
            ClassDef.ClassDefs.Add(FakeLListBOWithIntID.BuildAutoMappedClassDef());
        }

        [Test]
        public void Test_GenerateValidValue_WhenPropTypeString_ShouldRetItemInList()
        {
            //---------------Set up test pack-------------------
            Type propertyType = typeof(string);
            IPropDef def = new PropDefFake
            {
                PropertyType = propertyType
            };
            var dictionary = new Dictionary<string, string>
                                        {
                                            {"fda", "fdafasd"},
                                            {"fdadffasdf", "gjhj"}
                                        };
            def.LookupList = new SimpleLookupList(dictionary);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(def.LookupList);
            Assert.AreEqual(2, def.LookupList.GetLookupList().Count);
            //---------------Execute Test ----------------------
            object value = new ValidValueGeneratorLookupList(def).GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(typeof(string), value);
            StringAssert.StartsWith("fda", value.ToString());
        }

        [Test]
        public void Test_GenerateValidValue_WhenPropTypeGuid_ShouldReturnItemInListAsGuid()
        {
            //---------------Set up test pack-------------------
            Type guidPropType = typeof(Guid);
            IPropDef def = new PropDefFake
            {
                PropertyType = guidPropType
            };
            RandomValueGen.GetRandomGuid().ToString();
            var dictionary = new Dictionary<string, string>
                                        {
                                            {"fdafasd", RandomValueGen.GetRandomGuid().ToString()}
                                        };
            def.LookupList = new SimpleLookupList(dictionary);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(def.LookupList);
            Assert.AreEqual(1, def.LookupList.GetLookupList().Count);
            Assert.AreSame(guidPropType, def.PropertyType);
            //---------------Execute Test ----------------------
            object value = new ValidValueGeneratorLookupList(def).GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(value);
            Assert.IsInstanceOf(guidPropType, value, value + " should be of type guid");
        }
        [Test]
        public void Test_GenerateValidValue_WhenLookupListNull_ShouldRetNull()
        {
            //---------------Set up test pack-------------------
            Type propertyType = typeof(string);
            IPropDef def = new PropDefFake
            {
                PropertyType = propertyType
            };
            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<NullLookupList>(def.LookupList);
            //---------------Execute Test ----------------------
            object generateValidValue = new ValidValueGeneratorLookupList(def).GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.IsNull(generateValidValue);
        }

        [Test]
        public void Test_GenerateValidValue_WhenBOLList_AndNoItemsInList_ShouldCreateAnItem()
        {
            //---------------Set up test pack-------------------

            Type propertyType = typeof(Guid);
            IPropDef def = new PropDefFake
            {
                PropertyType = propertyType
            };
            def.LookupList = new BusinessObjectLookupList(typeof(FakeLListBO));
            def.LookupList.TimeOut = 0;
            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<BusinessObjectLookupList>(def.LookupList);
            Assert.AreEqual(0, def.LookupList.GetLookupList().Count);
            //---------------Execute Test ----------------------
            object generateValidValue = new ValidValueGeneratorLookupList(def).GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, def.LookupList.GetLookupList().Count);
            Assert.IsNotNull(generateValidValue);
            Assert.IsInstanceOf(propertyType, generateValidValue, generateValidValue + " should be of type guid");
        }

        [Test]
        public void Test_GenerateValidValue_WhenBOLList_AndNoItemsInList_AndIntPK_ShouldReturnCreatedItemValue_BUGFIX_1612()
        {
            //---------------Set up test pack-------------------

            Type propertyType = typeof(int);
            IPropDef def = new PropDefFake
            {
                PropertyType = propertyType
            };
            def.LookupList = new BusinessObjectLookupList(typeof(FakeLListBOWithIntID));
            def.LookupList.TimeOut = 0;
            //---------------Assert Precondition----------------
            Assert.IsInstanceOf<BusinessObjectLookupList>(def.LookupList);
            Assert.AreEqual(0, def.LookupList.GetLookupList().Count);
            //---------------Execute Test ----------------------
            object generateValidValue = new ValidValueGeneratorLookupList(def).GenerateValidValue();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, def.LookupList.GetLookupList().Count);
            Assert.IsNotNull(generateValidValue);
            Assert.IsInstanceOf(propertyType, generateValidValue, generateValidValue + " should be of type guid");
        }
    }
}