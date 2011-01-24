using System;
using Habanero.Base;
using NUnit.Framework;

namespace Habanero.Testability.Testers
{
    /// <summary>
    /// This is a specialised Tester for testing Habanero.<see cref="IRelationshipDef"/> for a multiple relationship
    /// This tester provides methods for testing the basic attributes of a <see cref="IRelationshipDef"/>
    /// such as ShouldHaveRelationshipType and ShouldHaveDeleteAction{}.
    /// If any of these Asserts fail then an <see cref="AssertionException"/>. is thrown.
    /// Else the Assert executes without an Exception
    /// </summary>
    public class MultipleRelDefTester
    {
        /// <summary>
        /// Constructs the tester with the relationship under test.
        /// </summary>
        /// <param name="relationshipDef"></param>
        public MultipleRelDefTester(IRelationshipDef relationshipDef)
        {
            if (relationshipDef == null) throw new ArgumentNullException("relationshipDef");
            MultipleRelationshipDef = relationshipDef;
        }
        /// <summary>
        /// The name of the Relationship being tested
        /// </summary>
        public string RelationshipName
        {
            get {
                return MultipleRelationshipDef.RelationshipName;
            }
        }

        protected virtual string ClassName
        {
            get { return this.MultipleRelationshipDef.OwningClassName; }
        }
        protected virtual string BaseMessage
        {
            get { return string.Format("The Relationship '{0}' for class '{1}'", RelationshipName, ClassName); }
        }
        /// <summary>
        /// The Relationship that is being tested.
        /// </summary>
        public IRelationshipDef MultipleRelationshipDef { get; private set; }
        /// <summary>
        /// Asserts that the Delete Action (<see cref="DeleteParentAction"/> is set correctly for the relationship.
        /// </summary>
        /// <param name="expectedDeleteAction"></param>
        public void ShouldHaveDeleteParentAction(DeleteParentAction expectedDeleteAction)
        {
            var actualDeleteAction = this.MultipleRelationshipDef.DeleteParentAction;
            var errMessage = BaseMessage 
                    + string.Format("should have a DeleteParentAction '{0}' but is '{1}'"
                    , expectedDeleteAction, actualDeleteAction);
            Assert.AreEqual(expectedDeleteAction, actualDeleteAction, errMessage);
        }

        /*        public void ShouldHaveRelationshipType(RelationshipType expectedRelationshipType)
                {

                    string errMessage = BaseMessage + string.Format(
                        "should have a RelationshipType '{0}' but is '{1}'", expectedRelationshipType, this.SingleRelationshipDef.RelationshipType);
                    Assert.AreEqual(expectedRelationshipType, this.SingleRelationshipDef.RelationshipType, errMessage);
                }*/
    }
}