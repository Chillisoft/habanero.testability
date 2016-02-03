using NUnit.Framework;

namespace Habanero.Testability.Testers.Tests
{
    public class NUnit3AssertsPolyFill
    {
        public static void IsNotNullOrEmpty(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new AssertionException("Expected data, but got null or empty value");
        }

        public static void IsNullOrEmpty(string value)
        {
            if (!string.IsNullOrEmpty(value)) throw new AssertionException("Expected null or empty value, but got data instead");
        }
    }
}