#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
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
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Habanero.Base;

namespace Habanero.Testability
{
    public static class TestabilityExtensions
    {
        public static BOTestFactory<T> WithValue<T, TPropType>(this BOTestFactory<T> factory, Expression<Func<T, TPropType>> expression, TPropType value) where T : class, IBusinessObject
        {
            factory.SetValueFor(expression, value);
            return factory;
        }
        public static BOTestFactory<T> WithValueFor<T, TPropType>(this BOTestFactory<T> factory, Expression<Func<T, TPropType>> expression) where T : class, IBusinessObject
        {
            factory.SetValueFor(expression);
            return factory;
        }
        public static BOTestFactory<T> WithValue<T, TPropType>(this BOTestFactory<T> factory, string propertyName, TPropType value) where T : class, IBusinessObject
        {
			factory.SetValueFor(propertyName, value);
            return factory;
        }
        public static BOTestFactory<T> WithValueFor<T, TPropType>(this BOTestFactory<T> factory, string propertyName) where T : class, IBusinessObject
        {
			factory.SetValueFor(propertyName);
            return factory;
        }
    }
}
