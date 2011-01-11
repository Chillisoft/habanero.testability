using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMappingHabanero;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;

namespace Habanero.Testability.Tests
{

    internal class BOTestFactoryFakeBO : BOTestFactory<FakeBO>
    {
    }
    public class FakeBO : BusinessObject
    {
        protected override IClassDef ConstructClassDef()
        {
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            return base.GetType().MapClass();
        }

        [AutoMapCompulsory]
        public RelatedFakeBo CompulsoryRelationship
        {
            get
            {
                return base.Relationships.GetRelatedObject<RelatedFakeBo>("CompulsoryRelationship");
            }
        }

        [AutoMapCompulsory]
        public virtual string CompulsoryString
        {
            get
            {
                return (string)base.GetPropertyValue("CompulsoryString");
            }
        }

        public RelatedFakeBo NonCompulsoryRelationship
        {
            get
            {
                return base.Relationships.GetRelatedObject<RelatedFakeBo>("NonCompulsoryRelationship");
            }
        }

        public virtual string NonCompulsoryString
        {
            get
            {
                return (string)base.GetPropertyValue("NonCompulsoryString");
            }
        }
    }
    public class FakeBOWithRules : BusinessObject
    {
        private readonly List<IBusinessObjectRule> _myRuleList = new List<IBusinessObjectRule>();

        public void AddBusinessRule(IBusinessObjectRule businessObjectRuleStub)
        {
            this._myRuleList.Add(businessObjectRuleStub);
        }

        protected override void LoadBusinessObjectRules(IList<IBusinessObjectRule> boRules)
        {
            base.LoadBusinessObjectRules(boRules);
            if (this._myRuleList != null)
            {
                foreach (IBusinessObjectRule rule in this._myRuleList)
                {
                    boRules.Add(rule);
                }
            }
        }

        public virtual decimal? AcquisitionCost
        {
            get
            {
                return (decimal?)base.GetPropertyValue("AcquisitionCost");
            }
            set
            {
                base.SetPropertyValue("AcquisitionCost", value);
            }
        }

        public virtual DateTime? AcquisitionDate
        {
            get
            {
                return (DateTime?)base.GetPropertyValue("AcquisitionDate");
            }
            set
            {
                base.SetPropertyValue("AcquisitionDate", value);
            }
        }

        public virtual DateTime? DisposalDate
        {
            get
            {
                return (DateTime?)base.GetPropertyValue("DisposalDate");
            }
            set
            {
                base.SetPropertyValue("DisposalDate", value);
            }
        }

        public virtual int? EconomicLife
        {
            get
            {
                return (int?)base.GetPropertyValue("EconomicLife");
            }
            set
            {
                base.SetPropertyValue("EconomicLife", value);
            }
        }

        public virtual int? EngineeringLife
        {
            get
            {
                return (int?)base.GetPropertyValue("EngineeringLife");
            }
            set
            {
                base.SetPropertyValue("EngineeringLife", value);
            }
        }

        public virtual decimal? ExpectedScrapValue
        {
            get
            {
                return (decimal?)base.GetPropertyValue("ExpectedScrapValue");
            }
            set
            {
                base.SetPropertyValue("ExpectedScrapValue", value);
            }
        }

    }



    internal enum FakeEnum
    {
        SomeNum,
        AnotherNum
    }

    internal class PropDefFake : PropDef
    {
        public PropDefFake()
            : base(RandomValueGen.GetRandomString(), typeof(int), PropReadWriteRule.ReadWrite, null)
        {
        }
    }

    public class RelatedFakeBo : BusinessObject
    {
        protected override IClassDef ConstructClassDef()
        {
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            return base.GetType().MapClass();
        }
    }
    [AutoMapIgnore]
    public class Unmapped : BusinessObject
    {
        protected override IClassDef ConstructClassDef()
        {
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            return base.GetType().MapClass();
        }

        public virtual string SomeProp
        {
            get
            {
                return (string)base.GetPropertyValue("SomeProp");
            }
        }
    }
}
