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
using Habanero.BO.ClassDefinition;
using NUnit.Framework;

namespace Habanero.Testability.Testers
{
    /// <summary>
    /// This is a specialised Tester for testing Habanero.<see cref="ISingleRelationshipDef"/> 
    /// This tester provides methods for testing the basic attributes of a <see cref="ISingleRelationshipDef"/>
    /// such as ShouldHaveRelationshipType and ShouldBeOneToOne{}.
    /// If any of these Asserts fail then an <see cref="AssertionException"/>. is thrown.
    /// Else the Assert executes without an Exception
    /// </summary>
    public class SingleRelDefTester: SingleValueTester
    {
        public ISingleRelationshipDef SingleRelationshipDef { get; private set; }

        public SingleRelDefTester(ISingleRelationshipDef singleRelationshipDef)
        {
            if (singleRelationshipDef == null) throw new ArgumentNullException("singleRelationshipDef");
            SingleRelationshipDef = singleRelationshipDef;
        }

        public override ISingleValueDef SingleValueDef
        {
            get { return this.SingleRelationshipDef; }
        }

        protected override string BaseMessage
        {
            get { return string.Format("The Relationship '{0}' for class '{1}'", RelationshipName, ClassName); }
        }

        public void ShouldBeOneToOne()
        {
            Assert.IsTrue(this.SingleRelationshipDef.IsOneToOne, BaseMessage + " should be OneToOne");
        }

        public void ShouldNotBeOneToOne()
        {
            Assert.IsFalse(this.SingleRelationshipDef.IsOneToOne, BaseMessage + " should not be OneToOne");
        }
        public void ShouldBeManyToOne()
        {
            Assert.IsTrue(this.SingleRelationshipDef.IsManyToOne, BaseMessage + " should be ManyToOne");
        }

        public void ShouldNotBeManyToOne()
        {
            Assert.IsFalse(this.SingleRelationshipDef.IsManyToOne, BaseMessage + " should not be ManyToOne");
        }
        public void ShouldHaveRelationshipType(RelationshipType expectedRelationshipType)
        {

            string errMessage = BaseMessage +  string.Format(
                "should have a RelationshipType '{0}' but is '{1}'", expectedRelationshipType, this.SingleRelationshipDef.RelationshipType);
            Assert.AreEqual(expectedRelationshipType, this.SingleRelationshipDef.RelationshipType, errMessage);
        }

        protected override string ClassName
        {
            get { return this.SingleRelationshipDef.OwningClassName; }
        }
        /// <summary>
        /// The Name of the SingleRelationshipDef that is being tested.
        /// </summary>
        public string RelationshipName
        {
            get { return this.SingleRelationshipDef.RelationshipName; }
        }
/*
        public void ShouldHaveReadWriteRule(PropReadWriteRule expectedReadWriteRule)
        {

            string errMessage = string.Format(
                "The Relationship '{0}' for class '{1}' should have a ReadWriteRule '{2}' but is '{3}'", RelationshipName,
                ClassName, expectedReadWriteRule, this.SingleRelationshipDef.ReadWriteRule);
            Assert.AreEqual(expectedReadWriteRule, this.SingleRelationshipDef.ReadWriteRule, errMessage);
        }*/
    }
}