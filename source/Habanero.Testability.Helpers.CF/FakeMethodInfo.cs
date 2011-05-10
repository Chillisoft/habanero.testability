using System;
using System.Globalization;
using System.Reflection;
using Rhino.Mocks;

namespace Habanero.Testability.Helpers
{
    public class FakeMethodInfo : MethodInfo
    {
        private readonly Type _declaringType;
        private readonly string _methodName;
        private readonly Type _propType;
        private Type _reflectedType;

        public FakeMethodInfo(string methodName)
        {
            _methodName = methodName;
        }
        public FakeMethodInfo(Type declaringType)
        {
            _declaringType = declaringType;
        }
        public FakeMethodInfo()
        {
            _declaringType = MockRepository.GenerateMock<Type>();
            _propType = MockRepository.GenerateMock<Type>();
            _methodName = RandomStringGen.GetRandomString();
        }
        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetParameters()
        {
            throw new NotImplementedException();
        }


        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetBaseDefinition()
        {
            throw new NotImplementedException();
        }

        public override Type ReturnType
        {
            get { throw new NotImplementedException(); }
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get { throw new NotImplementedException(); }
        }

        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override Type DeclaringType
        {
            get { throw new NotImplementedException(); }
        }

        public override Type ReflectedType
        {
            get { throw new NotImplementedException(); }
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get { throw new NotImplementedException(); }
        }

        public override MethodAttributes Attributes
        {
            get { return new MethodAttributes(); }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

    }
}