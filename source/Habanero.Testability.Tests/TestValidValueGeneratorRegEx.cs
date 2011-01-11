using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.Testability.Tests.Base;
using NUnit.Framework;
using System.Collections.Generic;

namespace Habanero.Testability.Tests
{
    [TestFixture]
    public class TestValidValueGeneratorRegEx
    {
        [Test]
        public void Test_Construct_IsCorrectInstance()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var regExPhrase = RandomValueGen.GetRandomString();
            var singleValueDef = GetPropDef();
            var valueGenerator = new ValidValueGeneratorRegEx(singleValueDef, regExPhrase);
            //---------------Test Result -----------------------
            Assert.IsInstanceOf(typeof(ValidValueGenerator), valueGenerator);
            Assert.AreSame(singleValueDef, valueGenerator.SingleValueDef);
        }

        [Test]
        public void Test_Construct_WithRegEx_StoresRegEx()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var regExPhrase = RandomValueGen.GetRandomString();
            var valueGenerator = new ValidValueGeneratorRegEx(GetPropDef(), regExPhrase);
            //---------------Test Result -----------------------
            Assert.AreSame(regExPhrase, valueGenerator.RegExPhrase);
        }

        [Test]
        public void Test_Construct_WithNullRegEx_ShouldThrowException()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
            try
            {
                new ValidValueGeneratorRegEx(GetPropDef(), null);
                Assert.Fail("expected ArgumentNullException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentNullException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("regExPhrase", ex.ParamName);
            }
        }

        [Test]
        public void Test_Construct_WithEmptyRegEx_ShouldThrowException()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            //---------------Test Result -----------------------
            try
            {
                new ValidValueGeneratorRegEx(GetPropDef(), "");
                Assert.Fail("expected ArgumentException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentException ex)
            {
                StringAssert.Contains("Must supply a non-empty regular expression", ex.Message);
                StringAssert.Contains("regExPhrase", ex.ParamName);
            }
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
                new ValidValueGeneratorRegEx(propDefIsNull, RandomValueGen.GetRandomString());
                Assert.Fail("expected ArgumentException");
            }
            //---------------Test Result -----------------------
            catch (ArgumentException ex)
            {
                StringAssert.Contains("Value cannot be null", ex.Message);
                StringAssert.Contains("singleValueDef", ex.ParamName);
            }
        }

        [Test, Ignore("ERIC - to get this test running, you need to place Rex.exe in the working directory")]
        public void Test_GenerateValue_OneValue_ShouldSet()
        {
            //---------------Set up test pack-------------------
            const string regExPhrase = "[ab]{5}";
            var valueGenerator = CreateValueGenerator(regExPhrase);
            //---------------Assert Precondition----------------
            Assert.IsTrue(Regex.IsMatch("abaab", regExPhrase));
            Assert.IsFalse(Regex.IsMatch("abcba", regExPhrase));
            //---------------Execute Test ----------------------
            //object previousValue = null;
            for (var i = 0; i < 5; i++)
            {
                var value = valueGenerator.GenerateValidValue();
                //---------------Test Result -----------------------
                Assert.IsNotNull(value);
                Assert.IsInstanceOf(typeof(string), value);
                Assert.IsTrue(Regex.IsMatch(value.ToString(), regExPhrase), "The value generated does not match the regular expression, value: " + value);
                //ERIC - the test below actually does fail occasionally.
                //Assert.AreNotEqual(previousValue, value, "Expect that a new generated value would be different to a previous one.  In theory this test could fail occasionally, but it's unlikely.");
                //previousValue = value;
            }
        }

        private static IPropDef GetPropDef()
        {
            return new PropDefFake
            {
                PropertyType = typeof(string)
            };
        }

        private static ValidValueGeneratorRegEx CreateValueGenerator(string regExPhrase, ISingleValueDef singleValueDef = null)
        {
            if (singleValueDef == null)
            {
                singleValueDef = new PropDefFake
                {
                    PropertyType = typeof(string)
                };
            }
            return new ValidValueGeneratorRegEx(singleValueDef, regExPhrase);
        }
    }
}