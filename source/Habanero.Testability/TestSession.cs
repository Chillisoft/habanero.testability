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
using Habanero.Base;

namespace Habanero.Testability
{
    ///<summary>
    ///</summary>
    public class TestSession
    {
        /*
         These are our ideas on what the TestSession should do.
          
          
         
                    var testSession = new TestSession();
                    testSession.RegisterBuilder<Sample, BOTestFactory<Sample>>();
          
        
                    testSession
                        .ConfigureBuilder<Sample>()
                            .WithValue(sample => sample.DateSubmitted, DateTime.Today.AddDays(-5))
                            .WithValueFor(sample3 => sample3.SampleNumber)
                            .SetValidValueGenerator(typeof (string), typeof (MyStringValidValGen))
                            .WithValidvalueGenerator(smp => smp.SampleNumber, typeof (SampleNumberGenerator))
                        .GetBuilder();

                    testSession.GetBuilder<Sample>()
                        .WithValue(sample => sample.DateSubmitted, DateTime.Today.AddDays(+9555))
                        .WithValueFor(sample3 => sample3.SampleNumber)
                        .CreateSavedBusinessObject();

                    testSession.GetBuilder<Sample>(); //should return a builder configured in the same way as the first one.
         

                    testSession.WithBuilder<ContactPerson>()
                            .With
                    
                    testSession.GetBuilder<Sample>().CreateSavedBusinessObject();
                    testSession.GetBuilder<Sample>():

                    testSession.SetBO<Sample>(sample1);
                    testSession.CreateBO<SampleTest>();
         
         
         
         
         
         */

        /*        private BOTestFactoryRegistry testFactoryRegistry = BOTestFactoryRegistry.Instance;
                public T CreateObject<T>() where T: class, IBusinessObject
                {
                    return testFactoryRegistry.Resolve<T>().CreateValidBusinessObject();
                }

                public void Register<T>(T bo) where T : class, IBusinessObject
                {
                    var instanceReturningBOTestFactory = new InstanceReturningBOTestFactory<T>(bo);
                    testFactoryRegistry.Register<T>(instanceReturningBOTestFactory);
                }*/

 /*       
        /// <summary>
        /// Register a Valid Value Generator to be used for generating values for a specified PropDef.
        /// </summary>
        /// <param name="propDef">The property definition for which this generator type is being assigned</param>
        /// <param name="validValuGenType">The type of generator which will be instantiated when a valid value is needed</param>
        /// <param name="parameter">An additional parameter to pass to the constructor of the generator</param>
        public virtual void Register(ISingleValueDef propDef, Type validValuGenType, object parameter = null)
        {
        }*/

/*        
        /// <summary>
        /// Resolves the registered <see cref="ValidValueGenerator"/> for the PropDef if one is registered.
        /// Else tries to find a <see cref="ValidValueGenerator"/> for the specified PropDefs Property Type 
        /// using the <see cref="ValidValueGeneratorRegistry"/>
        /// </summary>
        /// <returns></returns>
        public virtual ValidValueGenerator Resolve(ISingleValueDef propDef)
        {
        }*/


    }
}