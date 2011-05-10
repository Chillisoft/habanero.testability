using Habanero.Testability.CF;

namespace Habanero.Testability.Tests
{
    using Habanero.BO;
    using Habanero.Testability;
    using System;

    public static class TestUtilsFactory
    {
        public static PropRuleDate CreatePropRuleDateTime(DateTime min, DateTime max)
        {
            return new PropRuleDate(RandomValueGen.GetRandomString(), RandomValueGen.GetRandomString(), min, max);
        }

        public static PropRuleString CreatePropRuleString(int minLength, int maxLength)
        {
            return new PropRuleString(RandomValueGen.GetRandomString(), RandomValueGen.GetRandomString(), minLength, maxLength, "");
        }
    }
}

