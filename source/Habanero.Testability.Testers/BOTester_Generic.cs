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
using System.Linq.Expressions;
using Habanero.Base;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.Testability.Testers
{
    /// <summary>
    /// This is a specialised Tester for testing Habanero.<see cref="IPropDef"/> 
    /// This tester provides methods for testing the basic attributes of a <see cref="IPropDef"/>
    /// such as ShouldBeCompulsory.
    /// </summary>
    public class BOTester<T> : BOTester where T: class, IBusinessObject
    {
        ///<summary>
        /// Constructs a tester.
        ///</summary>
        public BOTester() : base(new BOTestFactory<T>().CreateDefaultBusinessObject())
        {
        }
        /// <summary>
        /// Tests that the Mapping within the Property Gettter and Setter
        /// are mapped correctly i.e. the string "ProspectTypeName" in 
        /// the Getter and the string "ProspectTypeName" in the setter are 
        /// both the same and they are mapped to a valid property Def in the ClassDef.
        /// <code>
        ///  public virtual string ProspectTypeName
        ///  {
        ///      get { return ((string)(base.GetPropertyValue("ProspectTypeName"))); }
        ///      set { base.SetPropertyValue("ProspectTypeName", value); }
        ///  }
        /// </code>
        /// This is done by setting the Property via reflection (if it has a set)
        /// and setting it via reflection (if it has a get) and comparing these values
        /// to the values for the BOProp to ensure that all mappings are correct.
        /// </summary>
        /// <param name="propExpression"></param>
        public void ShouldHavePropertyMapped(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldHavePropertyMapped(propertyName);
        }

        /// <summary>
        /// Tests that the Mapping within the Property Gettter and Setter
        /// of a relationship are mapped correctly i.e. the string "SingleRelGetterNotMapped" in 
        /// the Getter and the string "SingleRelationship" in the setter are 
        /// both the same and they are mapped to a valid <see cref="IRelationshipDef"/> in the ClassDef
        /// <see cref="IClassDef"/>.
        /// <code>
        ///   public virtual FakeBO SingleRelationship
        ///   {
        ///       get { return Relationships.GetRelatedObject{FakeBO}("SingleRelationship"); }
        ///       set { Relationships.SetRelatedObject("SingleRelationship", value); }
        ///   }
        /// </code>
        /// This is done by setting the Property via reflection (if it has a set)
        /// and setting it via reflection (if it has a get) and comparing these values
        /// to the values for the BOProp to ensure that all mappings are correct.
        /// </summary>
        /// <param name="singleRelExpression"></param>
        public void ShouldHaveSingleRelationshipMapped(Expression<Func<T, IBusinessObject>> singleRelExpression)
        {
            var relationshipName = ReflectionUtilities.GetPropertyName(singleRelExpression);
            ShouldHaveSingleRelationshipMapped(relationshipName);
        }

        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> does not have a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        public void ShouldHaveDefault(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldHaveDefault(propertyName);
        }
        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> has a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        public void ShouldNotHaveDefault(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldNotHaveDefault(propertyName);
        }
        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> does not have a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        /// <param name="defaultValue">The default value for this property.</param>
        public void ShouldHaveDefault(Expression<Func<T, object>> propExpression, string defaultValue)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldHaveDefault(propertyName, defaultValue);
        }

        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> has a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        /// <param name="expectedDefaultValue"></param>
        public void ShouldNotHaveDefault(Expression<Func<T, object>> propExpression, string expectedDefaultValue)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldNotHaveDefault(propertyName, expectedDefaultValue);
        }
        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> does not have a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        public void ShouldBeCompulsory(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldBeCompulsory(propertyName);
        }
        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> does not have a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        public void ShouldNotBeCompulsory(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldNotBeCompulsory(propertyName);
        }

        /// <summary>
        /// Raises <see cref="AssertionException"/> if the prop identified by
        /// <paramref name="propExpression"/> does not have a default value set.
        /// Does nothing otherwise
        /// </summary>
        /// <param name="propExpression"></param>
        /// <param name="expectedReadWriteRule">The expected ReadWriteRule</param>
        public void ShouldHaveReadWriteRule(Expression<Func<T, object>> propExpression, PropReadWriteRule expectedReadWriteRule)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldHaveReadWriteRule(propertyName, expectedReadWriteRule);
        }
        /// <summary>
        /// Asserts that the property identified by <paramref name="propExpression"/> is a Unique Constraint
        /// </summary>
        /// <param name="propExpression"></param>
        public void ShouldBeUniqueConstraint(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            ShouldBeUniqueConstraint(propertyName);
        }
        /// <summary>
        /// Returns a <see cref="PropDefTester"/> for the property specified by <paramref name="propExpression"/>
        /// </summary>
        /// <param name="propExpression"></param>
        /// <returns></returns>
        public PropDefTester GetPropTester(Expression<Func<T, object>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);

            return base.GetPropTester(propertyName);
        }
        /// <summary>
        /// Returns a <see cref="SingleRelDefTester"/> for the property (Relationship) specified by <paramref name="propExpression"/>
        /// </summary>
        /// <param name="propExpression"></param>
        /// <returns></returns>
        public SingleRelDefTester GetSingleRelationshipTester(Expression<Func<T, IBusinessObject>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);

            return base.GetSingleRelationshipTester(propertyName);
        }
        /// <summary>
        /// Returns a <see cref="MultipleRelDefTester"/> for the property (relationship) specified by <paramref name="propExpression"/>
        /// </summary>
        /// <param name="propExpression"></param>
        /// <returns></returns>
        public MultipleRelDefTester GetMultipleRelationshipTester(Expression<Func<T, IBusinessObjectCollection>> propExpression)
        {
            var propertyName = ReflectionUtilities.GetPropertyName(propExpression);
            var relDef = GetClassDef().GetRelationship(propertyName);
            return new MultipleRelDefTester(relDef);
        }

    }
}