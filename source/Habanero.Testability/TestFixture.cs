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
using System.Text;
using Habanero.Base;

namespace Habanero.Testability
{
    public class TestFixture
    {
        private BOTestFactoryRegistry testFactoryRegistry = BOTestFactoryRegistry.Instance;
        public T CreateObject<T>() where T: class, IBusinessObject
        {
            return testFactoryRegistry.Resolve<T>().CreateValidBusinessObject();
        }

        public void Register<T>(T bo) where T : class, IBusinessObject
        {
            var instanceReturningBOTestFactory = new InstanceReturningBOTestFactory<T>(bo);
            testFactoryRegistry.Register<T>(instanceReturningBOTestFactory);
        }
    }
    public class InstanceReturningBOTestFactory<T>:BOTestFactory<T> where T : class, IBusinessObject
    {
        private readonly T _boToAlwaysReturn;

        public InstanceReturningBOTestFactory(T  boToAlwaysReturn)
        {
            _boToAlwaysReturn = boToAlwaysReturn;
        }
        protected override IBusinessObject CreateBusinessObject()
        {
            return _boToAlwaysReturn;
        }

        public override T CreateValidBusinessObject()
        {
            return _boToAlwaysReturn;
        }

        public override void UpdateCompulsoryProperties(IBusinessObject businessObject)
        {
           //Do nothing you are returning an instance as is
        }

        public override T CreateSavedBusinessObject()
        {
            return _boToAlwaysReturn;
        }

        public override T CreateDefaultBusinessObject()
        {
            return _boToAlwaysReturn;
        }
    }
}
