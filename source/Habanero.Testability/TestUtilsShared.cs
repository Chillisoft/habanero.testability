﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Habanero.Base;
using Habanero.BO.ClassDefinition;

namespace Habanero.Testability
{
    /// <summary>
    /// Provides a standard set of utilities to be used by tests for BusinessObjects in this
    /// application.  In addition, each tested BO has a TestUtils class used to create
    /// random test packs.
    ///
    /// You can safely add additional utilities to this particular file since it is
    /// only written once.  If you are adding utilities for a specific BO only, rather
    /// add them to the TestUtils class for that BO (usually title TestUtilsCar if Car is the BO).
    /// </summary>
    public class TestUtilsShared
    {
        private static Random _randomGenerator;

        protected static Random Random
        {
            get { return _randomGenerator ?? (_randomGenerator = new Random()); }
        }

        public static int GetRandomInt()
        {
            return GetRandomInt(100000);
        }

        public static int GetRandomInt(int max)
        {
            return Random.Next(0, max);
        }

        public static int GetRandomInt(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static string GetRandomString()
        {
            return "A" + Guid.NewGuid().ToString().Replace("-", "");
        }

        public static string GetRandomString(int maxLength)
        {
            var randomString = GetRandomString();
            if (maxLength > randomString.Length) maxLength = randomString.Length;
            
            return randomString.Substring(0, maxLength);
            
        }

        public static string GetRandomString(int minLength, int maxLength)
        {
            var randomString = GetRandomString(maxLength);
            if (randomString.Length < minLength)
                randomString = randomString.PadRight(minLength, 'A');
            return randomString;
        }

        public static bool GetRandomBoolean()
        {
            return (GetRandomInt(100000) > 50000);
        }

        public static DateTime GetRandomDate()
        {
            var minDate = DateTime.MinValue;
            var maxDate = DateTime.MaxValue;

            return GetRandomDate(minDate, maxDate);
        }

        public static DateTime GetRandomDate(DateTime minDate, DateTime maxDate)
        {
            int range = (maxDate - minDate).Days;
            var randomInt = GetRandomInt(range);
            return minDate.AddDays(randomInt);
        }
        public static DateTime GetRandomDate(string min, string max)
        {
            var minDate = GetDate(min, DateTime.MinValue);
            var maxDate = GetDate(max, DateTime.MaxValue);
            return GetRandomDate(minDate, maxDate);
        }

        private static DateTime GetDate(string dateString, DateTime initialDate)
        {
            BOPropDateTimeDataMapper mapper = new BOPropDateTimeDataMapper();
            object value;
            var dateValueParsedOk = mapper.TryParsePropValue(dateString, out value);
            var dateTime = initialDate;
            if (dateValueParsedOk)
            {
                if (value is DateTime)
                {
                    dateTime = (DateTime) value;
                }
                else if (value is IResolvableToValue)
                {
                    dateTime = (DateTime)((IResolvableToValue)value).ResolveToValue();
                }
            }
            return dateTime;
        }

        public static DateTime GetRandomDate(string max)
        {
            string start = DateTime.MinValue.ToString("yyyy/MM/dd");
            return GetRandomDate(start, max);
        }

        /// <summary>
        /// Takes a lookup list generated by Habanero and randomly selects a value
        /// from the list
        /// </summary>
        public static object GetRandomLookupListValue(Dictionary<string, string> lookupList)
        {
            string[] values = new string[lookupList.Count];
            lookupList.Values.CopyTo(values, 0);
            return values[GetRandomInt(0, values.Length - 1)];
        }

        public static TEnum GetRandomEnum<TEnum>()
            where TEnum : struct
        {
            return GetRandomEnum<TEnum>(null);
        }

        public static TEnum GetRandomEnum<TEnum>(TEnum? excluded)
            where TEnum : struct
        {
            var enumType = typeof(TEnum);
            TEnum value = (TEnum)GetRandomEnum(enumType);
            if (excluded.HasValue && excluded.Value.Equals(value))
            {
                return GetRandomEnum(excluded);
            }
            return value;
        }

        public static object GetRandomEnum(Type enumType)
        {
            Array values = Enum.GetValues(enumType);
            int randomIndex = GetRandomInt(0, values.Length);
            return values.GetValue(randomIndex);
        }

        public static decimal GetRandomDecimal()
        {
            return GetRandomInt();
        }
    }
}
