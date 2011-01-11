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
            DateTime randomDate = RandomValueGen.GetRandomDate();
            Assert.GreaterOrEqual(randomDate, DateTime.MinValue);
            Assert.LessOrEqual(randomDate, DateTime.MaxValue);
            Assert.AreNotEqual(DateTime.Today, randomDate.Date);
        }

        [Test]
        public void Test_GetRandomDate_WhenInvalidMaxDateString_ShouldRetDateBetweenMinDateAndMaxDate()
        {
            DateTime minDate = DateTime.Today.AddDays(50.0);
            DateTime maxDate = DateTime.MaxValue;
            DateTime randomDate = RandomValueGen.GetRandomDate(minDate.ToShortDateString(), "InvalidString");
            Assert.GreaterOrEqual(randomDate, minDate);
            Assert.LessOrEqual(randomDate, maxDate);
            Assert.AreNotEqual(DateTime.Today, randomDate.Date);
        }

        [Test]
        public void Test_GetRandomDate_WhenInvalidMinDateString_ShouldRetDateBetweenMinDateAndMaxDate()
        {
            DateTime minDate = DateTime.MinValue;
            DateTime maxDate = DateTime.MinValue.AddDays(70.0);
            string maxDateString = maxDate.ToLongDateString();
            DateTime randomDate = RandomValueGen.GetRandomDate("Invalid", maxDateString);
            Assert.GreaterOrEqual(randomDate, minDate);
            Assert.LessOrEqual(randomDate, maxDate);
        }

        [Test]
        public void Test_GetRandomDate_WhenMaxAndMinDate_ShouldNotRetSameDateTwice()
        {
            DateTime minDate = DateTime.MinValue.AddDays(50.0);
            DateTime maxDate = DateTime.MaxValue.AddDays(-70.0);

            DateTime randomDate = RandomValueGen.GetRandomDate(minDate, maxDate);
            DateTime randomDate2 = RandomValueGen.GetRandomDate(minDate, maxDate);
            
            Assert.AreNotEqual(randomDate, randomDate2);
        }

        [Test]
        public void Test_GetRandomDate_WhenMaxAndMinDate_ShouldRetDateBetweenMinAndMax()
        {
            DateTime minDate = DateTime.MinValue.AddDays(50.0);
            DateTime maxDate = DateTime.MaxValue.AddDays(-70.0);
            DateTime randomDate = RandomValueGen.GetRandomDate(minDate, maxDate);
            Assert.GreaterOrEqual(randomDate, minDate);
            Assert.LessOrEqual(randomDate, maxDate);
        }

        [Test]
        public void Test_GetRandomDate_WhenMaxAndMinDateString_ShouldRetDateBetweenMinAndMax()
        {
            DateTime minDate = DateTime.Today.AddDays(50.0);
            DateTime maxDate = DateTime.MaxValue.AddDays(-70.0);
            string minDateString = minDate.ToShortDateString();
            string maxDateString = maxDate.ToLongDateString();
            DateTime randomDate = RandomValueGen.GetRandomDate(minDateString, maxDateString);
            Assert.GreaterOrEqual(randomDate, minDate);
            Assert.LessOrEqual(randomDate, maxDate);
            Assert.AreNotEqual(DateTime.Today, randomDate.Date);
        }

        [Test]
        public void Test_GetRandomDate_WhenMaxStringOnly_ShouldRetDateBetweenMinDateAndMax()
        {
            DateTime maxDate = DateTime.Today.AddDays(-70.0);
            DateTime randomDate = RandomValueGen.GetRandomDate(maxDate.ToShortDateString());
            Assert.GreaterOrEqual(randomDate, DateTime.MinValue);
            Assert.LessOrEqual(randomDate, maxDate);
        }

        [Test]
        public void Test_GetRandomDate_WhenMaxToday_ShouldRetDateBetweenMinDateToday()
        {
            DateTime minDate = DateTime.MinValue.AddDays(70.0);
            DateTime maxDate = DateTime.Today;
            DateTime randomDate = RandomValueGen.GetRandomDate(minDate.ToLongDateString(), "Today");
            Assert.GreaterOrEqual(randomDate, minDate);
            Assert.LessOrEqual(randomDate, maxDate);
        }

        [Test]
        public void Test_GetRandomDate_WhenMinToday_ShouldRetDateBetweenTodayAndMaxDate()
        {
            DateTime minDate = DateTime.Today;
            DateTime maxDate = DateTime.MaxValue.AddDays(-70.0);
            string maxDateString = maxDate.ToLongDateString();
            DateTime randomDate = RandomValueGen.GetRandomDate("Today", maxDateString);
            Assert.GreaterOrEqual(randomDate, minDate);
            Assert.LessOrEqual(randomDate, maxDate);
            Assert.AreNotEqual(RandomValueGen.GetRandomDate("Today", maxDateString), randomDate, "Should not produce same date twice");
        }

        [Test]
        public void Test_GetRandomDecimal_ShouldReturnDecimal()
        {
            decimal randomDecimal = RandomValueGen.GetRandomDecimal();
            Assert.IsNotNull(randomDecimal);
            Assert.AreNotEqual(RandomValueGen.GetRandomDecimal(), randomDecimal, "Should not ret same value twice");
        }

        [Test]
        public void Test_GetRandomDecimal_WhenMinGTMax_ShouldReturnMinValue()
        {
            decimal randomDecimal = RandomValueGen.GetRandomDecimal(39614081257132168796771975167M, 0M);
            Assert.IsNotNull(randomDecimal);
            Assert.Greater(randomDecimal, 39614081257132168796771975167M);
        }

        [Test]
        public void Test_GetRandomDecimalWhenMinAndMax_ShouldReturnDecimalWithinRange()
        {
            decimal randomDecimal = RandomValueGen.GetRandomDecimal(22M, 33M);
            Assert.IsNotNull(randomDecimal);
            Assert.Greater(randomDecimal, 22M);
            Assert.LessOrEqual(randomDecimal, 33M);
        }

        [Test]
        public void Test_GetRandomDecimalWhenMinValueAndMax_ShouldReturnDecimalWithinRange()
        {
            decimal randomDecimal = RandomValueGen.GetRandomDecimal(decimal.MinValue, 39614081257132168796771975167M);
            Assert.IsNotNull(randomDecimal);
            Assert.Greater(randomDecimal, decimal.MinValue);
            Assert.LessOrEqual(randomDecimal, 39614081257132168796771975167M);
        }

        [Test]
        public void Test_GetRandomDecimalWhenMinValueAndMaxValue_ShouldReturnDecimalWithinRange()
        {
            decimal randomDecimal = RandomValueGen.GetRandomDecimal(decimal.MinValue, decimal.MaxValue);
            Assert.IsNotNull(randomDecimal);
            Assert.Greater(randomDecimal, decimal.MinValue);
            Assert.LessOrEqual(randomDecimal, decimal.MaxValue);
        }

        [Test]
        public void Test_GetRandomDouble_ShouldReturnDouble()
        {
            double randomDouble = RandomValueGen.GetRandomDouble();
            Assert.IsNotNull(randomDouble);
            Assert.AreNotEqual(RandomValueGen.GetRandomDouble(), randomDouble, "Should not ret same value twice");
        }

        [Test]
        public void Test_GetRandomDoubleWhenMinAndMax_ShouldReturnDoubleWithinRange()
        {
            double randomDouble = RandomValueGen.GetRandomDouble(22.0, 24.0);
            Assert.IsNotNull(randomDouble);
            Assert.Greater(randomDouble, 22.0);
            Assert.LessOrEqual(randomDouble, 24.0);
        }

        [Test]
        public void Test_GetRandomString_ShouldReturnGuidPrefixedWithAAndDashRemoved()
        {
            string randomString = RandomValueGen.GetRandomString();
            StringAssert.DoesNotContain("-", randomString);
            StringAssert.DoesNotContain("{", randomString);
            StringAssert.StartsWith("A", randomString);
        }

        [Test]
        public void Test_GetRandomString_WhenMaxLengthGTGuidLength_ShouldReturnGuid()
        {
            int lengthOfGuidString = GetRandomString().Length;
            string randomString = RandomValueGen.GetRandomString(0x37);
            StringAssert.DoesNotContain("-", randomString);
            StringAssert.DoesNotContain("{", randomString);
            StringAssert.StartsWith("A", randomString);
            Assert.AreEqual(lengthOfGuidString, randomString.Length);
            Assert.AreNotEqual(0x37, randomString.Length);
        }

        [Test]
        public void Test_GetRandomString_WhenMinLengthGTGuidStringLength_ShouldReturnStringOfMinLengthPaddedWithAs()
        {
            int lengthOfGuidString = GetRandomString().Length;
            Assert.Greater(0x37, lengthOfGuidString);
            string randomString = RandomValueGen.GetRandomString(0x37, 0x41);
            Assert.AreEqual(0x37, randomString.Length);
        }

        [Test]
        public void Test_GetRandomString_WhenMinLengthGTMaxLength_ShouldReturnStringOfMinLength()
        {
            const int minLenght = 0x16;
            const int maxLength = 11;
            Assert.Greater(minLenght, maxLength);
            string randomString = RandomValueGen.GetRandomString(minLenght, maxLength);
            Assert.AreEqual(minLenght, randomString.Length);
            Assert.AreNotEqual(RandomValueGen.GetRandomString(minLenght, maxLength), randomString);
        }

        [Test]
        public void Test_GetRandomString_WhenMinLengthLTGuidStringLength_ShouldReturnGuidTrimmedToMaxLength()
        {
            int lengthOfGuidString = GetRandomString().Length;
            string randomString = RandomValueGen.GetRandomString(12, 0x37);
            StringAssert.DoesNotContain("-", randomString);
            StringAssert.DoesNotContain("{", randomString);
            StringAssert.StartsWith("A", randomString);

            var actualLength = randomString.Length;

            Assert.AreEqual(lengthOfGuidString, actualLength);
            Assert.AreNotEqual(0x37, actualLength);
        }

        [Test]
        public void Test_GetRandomStringWithMaxLength_ShouldReturnGuidTrimmedToMaxLength()
        {
            string randomString = RandomValueGen.GetRandomString(12);
            StringAssert.DoesNotContain("-", randomString);
            StringAssert.DoesNotContain("{", randomString);
            StringAssert.StartsWith("A", randomString);

            var actualLength = randomString.Length;

            Assert.AreEqual(12, actualLength);
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
    }
}

