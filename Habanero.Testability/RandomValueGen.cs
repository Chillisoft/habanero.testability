﻿using System;
using System.Collections.Generic;
using Habanero.Base;
using Habanero.BO;
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
    public static class RandomValueGen
    {
        //Seee discussion here
        // http://stackoverflow.com/questions/1344221/how-can-i-generate-random-8-character-alphanumeric-strings-in-c
        /* -----------------Generate pure alpha       --------------------------
         var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var result = new string(
                    Enumerable.Repeat(chars, 8)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray());*/
        /*-----------------         OR ------------------------------
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[8];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
                stringChars[i] = chars[random.Next(chars.Length)];
        }

        var finalString = new String(stringChars);*/




        /*-----------------         OR ------------------------------
         using System.Security.Cryptography;
using System.Text;

namespace UniqueKey
{
    public class KeyGenerator
    {
        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            return result.ToString();
        }
    }
}

         */

        /*--------------------------------And now for matching a Regex--------------------------------------
         * namespace ConsoleApplication2
{
    using System;
    using System.Text.RegularExpressions;

    class Program
    {
        static void Main(string[] args)
        {
            Random adomRng = new Random();
            string rndString = string.Empty;
            char c;

            for (int i = 0; i < 8; i++)
            {
                while (!Regex.IsMatch((c=Convert.ToChar(adomRng.Next(48,128))).ToString(), "[A-Za-z0-9]"));
                rndString += c;
            }

            Console.WriteLine(rndString + Environment.NewLine);
        }
    }
}
*/



        //Random rand = new Random(DateTime.Now.ToString().GetHashCode());
        private static readonly Random _randomGenerator = new Random(DateTime.Now.ToString().GetHashCode());
        private static Random Random
        {
            get
            {
                return (_randomGenerator);
            }
        }
        private static DateTime GetDate(string dateString, DateTime initialDate)
        {
            object value;
            bool dateValueParsedOk = new BOPropDateTimeDataMapper().TryParsePropValue(dateString, out value);
            DateTime dateTime = initialDate;
            if (dateValueParsedOk)
            {
                if (value is DateTime)
                {
                    return (DateTime) value;
                }
                if (value is IResolvableToValue)
                {
                    dateTime = (DateTime) ((IResolvableToValue) value).ResolveToValue();
                }
            }
            return dateTime;
        }

        public static bool GetRandomBoolean()
        {
            return (GetRandomInt(0x186a0) > 0xc350);
        }

        public static DateTime GetRandomDate()
        {
            DateTime minDate = DateTime.MinValue;
            DateTime maxDate = DateTime.MaxValue;
            return GetRandomDate(minDate, maxDate);
        }

        public static DateTime GetRandomDate(string max)
        {
            return GetRandomDate(DateTime.MinValue.ToString("yyyy/MM/dd"), max);
        }

        public static DateTime GetRandomDate(DateTime minDate, DateTime maxDate)
        {
            if (maxDate < minDate) maxDate = minDate;
            var timeSpan = maxDate - minDate;
            
            long range = timeSpan.Days;
            if (maxDate == DateTime.MaxValue && minDate < DateTime.MaxValue.AddDays(-1)) range -= 1;
            if (range <= 0) range = 1;
            int intRange;
            if (range > int.MaxValue)
            {
                intRange = int.MaxValue - 1;
            }
            else
            {
                intRange = (int) range;
            }
            int randomInt = GetRandomInt(1, intRange);
            return minDate.AddDays(randomInt);
        }

        public static DateTime GetRandomDate(string min, string max)
        {
            DateTime minDate = GetDate(min, DateTime.MinValue);
            DateTime maxDate = GetDate(max, DateTime.MaxValue);
            return GetRandomDate(minDate, maxDate);
        }

        public static decimal GetRandomDecimal()
        {
            return GetRandomInt();
        }

        public static decimal GetRandomDecimal(decimal minValue, decimal maxValue)
        {
            if (((minValue < 0M) && (maxValue > 0M)) && ((decimal.MaxValue - maxValue) < (-1M*minValue)))
            {
                minValue = 0M;
            }
            decimal range = maxValue - minValue;
            decimal truncatedRange = Math.Truncate(range);
            if (truncatedRange > int.MaxValue)
            {
                truncatedRange = int.MaxValue - 1;
            }
            if (truncatedRange <= 0M)
            {
                truncatedRange = 1M;
            }
            int randomAddition = GetRandomInt(1, Convert.ToInt32(truncatedRange));
            return (minValue + randomAddition);
        }

        public static double GetRandomDouble()
        {
            return GetRandomInt();
        }

        public static double GetRandomDouble(double minValue, double maxValue)
        {
            if (((minValue < 0.0) && (maxValue > 0.0)) && ((double.MaxValue - maxValue) < (-1.0*minValue)))
            {
                minValue = 0.0;
            }
            double range = maxValue - minValue;
            double truncatedRange = Math.Truncate(range);
            if (truncatedRange > int.MaxValue)
            {
                truncatedRange = int.MaxValue - 1;
            }
            if (truncatedRange <= 0.0)
            {
                truncatedRange = 1.0;
            }
            int randomAddition = GetRandomInt(1, Convert.ToInt32(truncatedRange));
            return (minValue + randomAddition);
        }

        public static TEnum GetRandomEnum<TEnum>() where TEnum : struct
        {
            return GetRandomEnum<TEnum>(null);
        }

        public static TEnum GetRandomEnum<TEnum>(TEnum? excluded) where TEnum : struct
        {
            Type enumType = typeof (TEnum);
            TEnum value = (TEnum) GetRandomEnum(enumType);
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

        public static Guid GetRandomGuid()
        {
            return Guid.NewGuid();
        }

        public static int GetRandomInt()
        {
            return GetRandomInt(0x186a0);
        }

        public static int GetRandomInt(int max)
        {
            return Random.Next(-2147483648, max);
        }

        public static int GetRandomInt(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static long GetRandomLong()
        {
            return GetRandomLong(long.MaxValue);
        }

        public static long GetRandomLong(long max)
        {
            return GetRandomLong(long.MinValue, max);
        }

        public static long GetRandomLong(long min, long max)
        {
            
            if(min > max) min = max;
            if (min > int.MinValue && max < int.MaxValue) return GetRandomInt((int)min, (int) max);
            do
            {
                int firstPart = Random.Next();
                int secondPart = Random.Next();
                long randomLong = (long) firstPart << 32 + secondPart;
                if (randomLong >= min && randomLong <= max) return randomLong;
            } while (true);
        }


        public static object GetRandomLookupListValue(Dictionary<string, string> lookupList)
        {
            if (lookupList.Count == 0)
            {
                return null;
            }
            string[] values = new string[lookupList.Count];
            lookupList.Values.CopyTo(values, 0);
            return ((values.Length == 1) ? values[0] : values[GetRandomInt(0, values.Length - 1)]);
        }

        public static string GetRandomString()
        {
            return ("A" + Guid.NewGuid().ToString().Replace("-", ""));
        }

        public static string GetRandomString(int maxLength)
        {
            string randomString = GetRandomString();
            if (maxLength > randomString.Length)
            {
                maxLength = randomString.Length;
            }
            return randomString.Substring(0, maxLength);
        }

        public static string GetRandomString(int minLength, int maxLength)
        {
            if (maxLength <= 0) maxLength = int.MaxValue;
            if (minLength < 0) minLength = 0;
            string randomString = GetRandomString(maxLength);
            if (randomString.Length < minLength)
            {
                randomString = randomString.PadRight(minLength, 'A');
            }
            return randomString;
        }


        /// <summary>
        /// Returns the most restrictive Maximum Value based on the Prop Rule and the
        /// overriding Max Value for the Type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propRule"></param>
        /// <param name="overridingMaxValue"></param>
        /// <returns></returns>
        public static T GetMaxValue<T>(IPropRuleComparable<T> propRule, T? overridingMaxValue)
            where T : struct, IComparable<T>
        {
            var absoluteMin = (T) GetAbsoluteMax<T>();
            T propRuleMaxValue = (propRule == null) ? absoluteMin : propRule.MaxValue;
            T internalMaxValue = overridingMaxValue.HasValue ? overridingMaxValue.GetValueOrDefault() : absoluteMin;
            return internalMaxValue.CompareTo(propRuleMaxValue) > 0 ? propRuleMaxValue : internalMaxValue;
        }
        /// <summary>
        /// Returns the most restrictive Minimum Value based on the Prop Rule and the
        /// overriding Min Value for the Type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propRule"></param>
        /// <param name="overridingMinValue"></param>
        /// <returns></returns>
        public static T GetMinValue<T>(IPropRuleComparable<T> propRule, T? overridingMinValue)
            where T : struct, IComparable<T>
        {
            var absoluteMin = (T) GetAbsoluteMin<T>();
            T propRuleMinValue = (propRule == null) ? absoluteMin : propRule.MinValue;
            T internalMinValue = overridingMinValue.HasValue ? overridingMinValue.GetValueOrDefault() : absoluteMin;
            return internalMinValue.CompareTo(propRuleMinValue) < 0 ? propRuleMinValue : internalMinValue;
        }

        /// <summary>
        /// Returns the absolute minimum for the dataTypes.
        /// This only supports types that have MinValue, MaxValue e.g. single, Double, Decimaal 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object GetAbsoluteMin<T>() // where T : struct, IComparable<T>
        {
            Type type = typeof (T);
            return GetAbsoluteMin(type);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetAbsoluteMin(Type type)
        {
            if (type == typeof (int)) return int.MinValue;
            if (type == typeof (decimal)) return decimal.MinValue;
            if (type == typeof (double)) return double.MinValue;
            if (type == typeof (Single)) return Single.MinValue;
            if (type == typeof (long)) return long.MinValue;
            if (type == typeof (DateTime)) return DateTime.MinValue;

            return int.MinValue;
        }

        /// <summary>
        /// Returns the absolute minimum for the dataTypes.
        /// This only supports types that have MinValue, MaxValue e.g. single, Double, Decimaal 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object GetAbsoluteMax<T>() // where T : struct, IComparable<T>
        {
            Type type = typeof (T);
            return GetAbsoluteMax(type);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetAbsoluteMax(Type type)
        {
            if (type == typeof (int)) return int.MaxValue;
            if (type == typeof (decimal)) return decimal.MaxValue;
            if (type == typeof (double)) return double.MaxValue;
            if (type == typeof (Single)) return Single.MaxValue;
            if (type == typeof (long)) return long.MaxValue;
            if (type == typeof (DateTime)) return DateTime.MaxValue;

            return int.MaxValue;
        }
    }
}