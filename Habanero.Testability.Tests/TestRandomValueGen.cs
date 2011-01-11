namespace Habanero.Testability.Tests
{
    using Habanero.Testability;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class TestRandomValueGen
    {
        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }

        [Test]
        public void Test_GetRandomDate_NoMaxOrMin_ShouldRetDateBetweenMinDateAndMaxDate()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var randomDate = RandomValueGen.GetRandomDate();
            //---------------Test Result -----------------------
            Assert.GreaterOrEqual(randomDate, DateTime.MinValue);
            Assert.LessOrEqual(randomDate, DateTime.MaxValue);
            Assert.AreNotEqual(DateTime.Today, randomDate.Date);
        }
        [Test]
        public void Test_GetRandomDate_WhenMaxAndMinDate_ShouldRetDateBetweenMinAndMax()
        {
            //---------------Set up test pack-------------------
            var minDate = DateTime.MinValue.AddDays(50);
            var maxDate = DateTime.MaxValue.AddDays(-70);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var randomDate = RandomValueGen.GetRandomDate(minDate, maxDate);
            //---------------Test Result -----------------------
            Assert.GreaterOrEqual(randomDate, minDate);
            Assert.LessOrEqual(randomDate, maxDate);
        }

        [Test]
        public void Test_GetRandomDate_WhenMaxLTMinDate_ShouldRetMinDate()
        {
            //---------------Set up test pack-------------------
            var minDate = DateTime.MinValue.AddDays(50);
            var maxDate = minDate.AddDays(-10);
            //---------------Assert Precondition----------------
            Assert.Less(maxDate, minDate);
            //---------------Execute Test ----------------------
            var randomDate = RandomValueGen.GetRandomDate(minDate, maxDate);
            //---------------Test Result -----------------------
            Assert.AreEqual(minDate.AddDays(1), randomDate);
        }

        [Test]
        public void Test_GetRandomDate_WhenMaxAndMinDate_ShouldNotRetSameDateTwice()
        {
            //---------------Set up test pack-------------------
            var minDate = DateTime.MinValue.AddDays(50);
            var maxDate = DateTime.MaxValue.AddDays(-70);
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            var randomDate = RandomValueGen.GetRandomDate(minDate, maxDate);
            var randomDate2 = RandomValueGen.GetRandomDate(minDate, maxDate);
            //---------------Test Result -----------------------
            Assert.AreNotEqual(randomDate, randomDate2);
        }
        [Test]
        public void Test_GetRandomDate_WhenMaxAndMinDateString_ShouldRetDateBetweenMinAndMax()
        {
            //---------------Set up test pack-------------------
            var minDate = DateTime.Today.AddDays(50);
            var maxDate = DateTime.MaxValue.AddDays(-70);
            var minDateString = minDate.ToShortDateString();
            var maxDateString = maxDate.ToLongDateString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------

            var randomDate = RandomValueGen.GetRandomDate(minDateString, maxDateString);
            //---------------Test Result -----------------------
            Assert.GreaterOrEqual(randomDate, minDate);
            Assert.LessOrEqual(randomDate, maxDate);
            Assert.AreNotEqual(DateTime.Today, randomDate.Date);
        }
        [Test]
        public void Test_GetRandomDate_WhenInvalidMaxDateString_ShouldRetDateBetweenMinDateAndMaxDate()
        {
            //---------------Set up test pack-------------------
            var minDate = DateTime.Today.AddDays(50);
            var maxDate = DateTime.MaxValue;
            var minDateString = minDate.ToShortDateString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------

            var randomDate = RandomValueGen.GetRandomDate(minDateString, "InvalidString");
            //---------------Test Result -----------------------
            Assert.GreaterOrEqual(randomDate, minDate);
            Assert.LessOrEqual(randomDate, maxDate);
            Assert.AreNotEqual(DateTime.Today, randomDate.Date);
        }
        [Test]
        public void Test_GetRandomDate_WhenInvalidMinDateString_ShouldRetDateBetweenMinDateAndMaxDate()
        {
            //---------------Set up test pack-------------------
            var minDate = DateTime.MinValue;
            var maxDate = DateTime.MinValue.AddDays(70);
            var maxDateString = maxDate.ToLongDateString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------

            var randomDate = RandomValueGen.GetRandomDate("Invalid", maxDateString);
            //---------------Test Result -----------------------
            Assert.GreaterOrEqual(randomDate, minDate);
            Assert.LessOrEqual(randomDate, maxDate);
        }
        [Test]
        public void Test_GetRandomDate_WhenMinToday_ShouldRetDateBetweenTodayAndMaxDate()
        {
            //---------------Set up test pack-------------------
            var minDate = DateTime.Today;
            var maxDate = DateTime.MaxValue.AddDays(-70);
            var maxDateString = maxDate.ToLongDateString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------

            var randomDate = RandomValueGen.GetRandomDate("Today", maxDateString);
            //---------------Test Result -----------------------
            Assert.GreaterOrEqual(randomDate, minDate);
            Assert.LessOrEqual(randomDate, maxDate);
            Assert.AreNotEqual(RandomValueGen.GetRandomDate("Today", maxDateString), randomDate, "Should not produce same date twice");
        }
        [Test]
        public void Test_GetRandomDate_WhenMaxToday_ShouldRetDateBetweenMinDateToday()
        {
            //---------------Set up test pack-------------------
            var minDate = DateTime.MinValue.AddDays(70);
            var maxDate = DateTime.Today;
            var minDateString = minDate.ToLongDateString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------

            var randomDate = RandomValueGen.GetRandomDate(minDateString, "Today");
            //---------------Test Result -----------------------
            Assert.GreaterOrEqual(randomDate, minDate);
            Assert.LessOrEqual(randomDate, maxDate);
        }
        [Test]
        public void Test_GetRandomDate_WhenMaxStringOnly_ShouldRetDateBetweenMinDateAndMax()
        {
            //---------------Set up test pack-------------------
            var maxDate = DateTime.Today.AddDays(-70);
            var maxDateString = maxDate.ToShortDateString();
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------

            var randomDate = RandomValueGen.GetRandomDate(maxDateString);
            //---------------Test Result -----------------------
            Assert.GreaterOrEqual(randomDate, DateTime.MinValue);
            Assert.LessOrEqual(randomDate, maxDate);
        }

        [Test]
        public void Test_GetRandomString_ShouldReturnGuidPrefixedWithAAndDashRemoved()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomString = RandomValueGen.GetRandomString();
            //---------------Test Result -----------------------
            StringAssert.DoesNotContain("-", randomString);
            StringAssert.DoesNotContain("{", randomString);
            StringAssert.StartsWith("A", randomString);
        }
        [Test]
        public void Test_GetRandomStringWithMaxLength_ShouldReturnGuidTrimmedToMaxLength()
        {
            //---------------Set up test pack-------------------
            const int maxLength = 12;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomString = RandomValueGen.GetRandomString(maxLength);
            //---------------Test Result -----------------------
            StringAssert.DoesNotContain("-", randomString);
            StringAssert.DoesNotContain("{", randomString);
            StringAssert.StartsWith("A", randomString);
            Assert.AreEqual(maxLength, randomString.Length);
        }
        [Test]
        public void Test_GetRandomString_WhenMaxLengthGTGuidLength_ShouldReturnGuid()
        {
            //---------------Set up test pack-------------------
            const int maxLength = 55;
            int lengthOfGuidString = GetRandomString().Length;

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomString = RandomValueGen.GetRandomString(maxLength);
            //---------------Test Result -----------------------
            StringAssert.DoesNotContain("-", randomString);
            StringAssert.DoesNotContain("{", randomString);
            StringAssert.StartsWith("A", randomString);
            Assert.AreEqual(lengthOfGuidString, randomString.Length);
            Assert.AreNotEqual(maxLength, randomString.Length);
        }
        [Test]
        public void Test_GetRandomString_WhenMinLengthLTGuidStringLength_ShouldReturnGuidTrimmedToMaxLength()
        {
            //---------------Set up test pack-------------------
            const int maxLength = 55;
            const int minLength = 12;
            int lengthOfGuidString = GetRandomString().Length;

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomString = RandomValueGen.GetRandomString(minLength, maxLength);
            //---------------Test Result -----------------------
            StringAssert.DoesNotContain("-", randomString);
            StringAssert.DoesNotContain("{", randomString);
            StringAssert.StartsWith("A", randomString);
            Assert.AreEqual(lengthOfGuidString, randomString.Length);
            Assert.AreNotEqual(maxLength, randomString.Length);
        }
        [Test]
        public void Test_GetRandomString_WhenMinLengthGTGuidStringLength_ShouldReturnStringOfMinLengthPaddedWithAs()
        {
            //---------------Set up test pack-------------------
            const int maxLength = 65;
            const int minLength = 55;
            int lengthOfGuidString = GetRandomString().Length;

            //---------------Assert Precondition----------------
            Assert.Greater(minLength, lengthOfGuidString);
            //---------------Execute Test ----------------------
            var randomString = RandomValueGen.GetRandomString(minLength, maxLength);
            //---------------Test Result -----------------------
            Assert.AreEqual(minLength, randomString.Length);
        }
        [Test]
        public void Test_GetRandomString_WhenMinLengthGTMaxLength_ShouldReturnStringOfMinLength()
        {
            //---------------Set up test pack-------------------
            const int maxLength = 11;
            const int minLength = 22;

            //---------------Assert Precondition----------------
            Assert.Greater(minLength, maxLength);
            //---------------Execute Test ----------------------
            var randomString = RandomValueGen.GetRandomString(minLength, maxLength);
            //---------------Test Result -----------------------
            Assert.AreEqual(minLength, randomString.Length);
            Assert.AreNotEqual(RandomValueGen.GetRandomString(minLength, maxLength), randomString);
        }
        [Test]
        public void Test_GetRandomString_WhenMinLengthValidAndMaxLengthNeg_ShouldReturnStringGreaterThanMaxLength()
        {
            //---------------Set up test pack-------------------
            const int maxLength = -1;
            const int minLength = 22;

            //---------------Assert Precondition----------------
            Assert.Greater(minLength, maxLength);
            //---------------Execute Test ----------------------
            var randomString = RandomValueGen.GetRandomString(minLength, maxLength);
            //---------------Test Result -----------------------
            Assert.GreaterOrEqual(randomString.Length, minLength);
            Assert.AreNotEqual(RandomValueGen.GetRandomString(minLength, maxLength), randomString);
        }

        [Test]
        public void Test_GetRandomDecimal_ShouldReturnDecimal()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomDecimal = RandomValueGen.GetRandomDecimal();
            //---------------Test Result -----------------------G
            Assert.IsNotNull(randomDecimal);
            Assert.AreNotEqual(RandomValueGen.GetRandomDecimal(), randomDecimal, "Should not ret same value twice");

        }
        [Test]
        public void Test_GetRandomDecimalWhenMinAndMax_ShouldReturnDecimalWithinRange()
        {
            //---------------Set up test pack-------------------
            const decimal maxValue = 33;
            const decimal minValue = 22;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomDecimal = RandomValueGen.GetRandomDecimal(minValue, maxValue);
            //---------------Test Result -----------------------G
            Assert.IsNotNull(randomDecimal);
            Assert.Greater(randomDecimal, minValue);
            Assert.LessOrEqual(randomDecimal, maxValue);
        }
        [Test]
        public void Test_GetRandomDecimalWhenMinValueAndMaxValue_ShouldReturnDecimalWithinRange()
        {
            //---------------Set up test pack-------------------
            const decimal maxValue = decimal.MaxValue;
            const decimal minValue = decimal.MinValue;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomDecimal = RandomValueGen.GetRandomDecimal(minValue, maxValue);
            //---------------Test Result -----------------------G
            Assert.IsNotNull(randomDecimal);
            Assert.Greater(randomDecimal, minValue);
            Assert.LessOrEqual(randomDecimal, maxValue);
        }
        [Test]
        public void Test_GetRandomDecimalWhenMinValueAndMax_ShouldReturnDecimalWithinRange()
        {
            //---------------Set up test pack-------------------
            const decimal maxValue = decimal.MaxValue - decimal.MaxValue / 2;
            const decimal minValue = decimal.MinValue;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomDecimal = RandomValueGen.GetRandomDecimal(minValue, maxValue);
            //---------------Test Result -----------------------G
            Assert.IsNotNull(randomDecimal);
            Assert.Greater(randomDecimal, minValue);
            Assert.LessOrEqual(randomDecimal, maxValue);
        }
        [Test]
        public void Test_GetRandomDecimal_WhenMinGTMax_ShouldReturnMinValue()
        {
            //---------------Set up test pack-------------------
            const decimal maxValue = 0;
            const decimal minValue = decimal.MaxValue - decimal.MaxValue / 2;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomDecimal = RandomValueGen.GetRandomDecimal(minValue, maxValue);
            //---------------Test Result -----------------------G
            Assert.IsNotNull(randomDecimal);
            Assert.Greater(randomDecimal, minValue);
        }
        [Test]
        public void Test_GetRandomDouble_ShouldReturnDouble()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomDouble = RandomValueGen.GetRandomDouble();
            //---------------Test Result -----------------------G
            Assert.IsNotNull(randomDouble);
            Assert.AreNotEqual(RandomValueGen.GetRandomDouble(), randomDouble, "Should not ret same value twice");

        }
        [Test]
        public void Test_GetRandomDoubleWhenMinAndMax_ShouldReturnDoubleWithinRange()
        {
            //---------------Set up test pack-------------------
            const double maxValue = 24;
            const double minValue = 22;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomDouble = RandomValueGen.GetRandomDouble(minValue, maxValue);
            //---------------Test Result -----------------------G
            Assert.IsNotNull(randomDouble);
            Assert.Greater(randomDouble, minValue);
            Assert.LessOrEqual(randomDouble, maxValue);
        }


        [TestCase(typeof(int), int.MinValue)]
        [TestCase(typeof(double), double.MinValue)]
        [TestCase(typeof(Single), Single.MinValue)]
        [TestCase(typeof(long), long.MinValue)]
        public void Test_GetAbsoluteMin_ReturnsTheAppropriateMinForTheType(Type type, object expecteMin)
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var absoluteMin = RandomValueGen.GetAbsoluteMin(type);
            //---------------Test Result -----------------------
            Assert.AreEqual(expecteMin, absoluteMin);
        }
        [TestCase(typeof(int), int.MaxValue)]
        [TestCase(typeof(double), double.MaxValue)]
        [TestCase(typeof(Single), Single.MaxValue)]
        [TestCase(typeof(long), long.MaxValue)]
        public void Test_GetAbsoluteMax_ReturnsTheAppropriateMaxForTheType(Type type, object expecteMax)
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var absoluteMax = RandomValueGen.GetAbsoluteMax(type);
            //---------------Test Result -----------------------
            Assert.AreEqual(expecteMax, absoluteMax);
        }
        [Test]
        public void Test_GetAbsoluteMax_WhenDate_ReturnsTheAppropriateMaxDate()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var absoluteMax = RandomValueGen.GetAbsoluteMax<DateTime>();
            //---------------Test Result -----------------------
            Assert.AreEqual(DateTime.MaxValue, absoluteMax);
        }
        [Test]
        public void Test_GetAbsoluteMin_WhenDate_ReturnsTheAppropriateMinDate()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var absoluteMin = RandomValueGen.GetAbsoluteMin<DateTime>();
            //---------------Test Result -----------------------
            Assert.AreEqual(DateTime.MinValue, absoluteMin);
        }
        [Test]
        public void Test_GetAbsoluteMax_WhenDecimal_ReturnsTheAppropriateMaxDecimal()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var absoluteMax = RandomValueGen.GetAbsoluteMax<decimal>();
            //---------------Test Result -----------------------
            Assert.AreEqual(decimal.MaxValue, absoluteMax);
        }
        [Test]
        public void Test_GetAbsoluteMin_WhenDecimal_ReturnsTheAppropriateMinDecimal()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var absoluteMin = RandomValueGen.GetAbsoluteMin<decimal>();
            //---------------Test Result -----------------------
            Assert.AreEqual(decimal.MinValue, absoluteMin);
        }

        [Test]
        public void Test_WithNoMaxAndMin_ShouldReturnRandomLong()
        {
            //---------------Set up test pack-------------------
            
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var randomLong = RandomValueGen.GetRandomLong();
            //---------------Test Result -----------------------
            Assert.Greater(randomLong, (long) RandomValueGen.GetAbsoluteMin<long>(), "fddasf");
        }
    }
}

