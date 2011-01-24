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