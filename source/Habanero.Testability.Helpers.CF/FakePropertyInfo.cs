// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2010 Chillisoft Solutions
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
using System;
using System.Globalization;
using System.Reflection;
using Rhino.Mocks;

namespace Habanero.Testability.Helpers
{
    /// <summary>
    /// Fake PropertyInfo for testing reflection code e.g. binding, Smooth, testability.
    /// </summary>
    public class FakePropertyInfo : PropertyInfo
    {
        private readonly Type _declaringType;
        private readonly string _propName;
        private readonly Type _propType;
        private Type _reflectedType;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propType"></param>
        /// <param name="declaringType"></param>
        public FakePropertyInfo(string propName, Type propType, Type declaringType)
        {
            _propName = propName;
            _propType = propType;
            _declaringType = declaringType;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propType"></param>
        public FakePropertyInfo(string propName, Type propType)
            : this(propName, propType, GetMockType())
        {

        }
        /// <summary>
        /// /
        /// </summary>
        /// <param name="declaringType"></param>
        public FakePropertyInfo(Type declaringType)
            : this(GetRandomString(), GetMockType(), declaringType)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public FakePropertyInfo()
            : this(GetRandomString(), GetMockType(), GetMockType())
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        public FakePropertyInfo(string propName)
            : this(propName, GetMockType(), GetMockType())
        {

        }

        private static Type GetMockType()
        {
            return MockRepository.GenerateMock<Type>();
        }

        private static string GetRandomString()
        {
            return RandomValueGen.GetRandomString();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            return new FakeMethodInfo();
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            return new FakeMethodInfo();
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { return _propName; }
        }

        /// <summary>
        /// Gets the class that declares this member.
        /// </summary>
        /// <returns>
        /// The Type object for the class that declares this member.
        /// </returns>
        public override Type DeclaringType
        {
            get { return _declaringType; }
        }

        public override Type ReflectedType
        {
            get { return _reflectedType; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reflectedType"></param>
        public void SetReflectedType(Type reflectedType)
        {
            _reflectedType = reflectedType;
        }
        
        public override Type PropertyType
        {
            get { return _propType; }
        }

        public override PropertyAttributes Attributes
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
    }
/*
    internal class RandomStringGen
    {

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
    }*/
}