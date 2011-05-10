using System;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Testability.CF;

namespace Habanero.Testability.Helpers
{
    public class SingleRelDefFake : SingleRelationshipDef
    {
        public SingleRelDefFake()
            : this(RandomValueGen.GetRandomString())
        {
        }
        public SingleRelDefFake(string relName)
            : this(relName, typeof(FakeBOForRelDef))
        {
        }
        public SingleRelDefFake(string relName, Type relatedType)
            : base(relName, relatedType, new RelKeyDef(), false, Base.DeleteParentAction.Prevent)
        {
        }
    }
    public class FakeBOForRelDef : BusinessObject
    { }
}