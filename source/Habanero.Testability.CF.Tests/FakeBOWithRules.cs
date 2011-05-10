namespace Habanero.Testability.Tests
{
    using Habanero.Base;
    using Habanero.BO;
    using System;
    using System.Collections.Generic;

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
                return (decimal?) base.GetPropertyValue("AcquisitionCost");
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
                return (DateTime?) base.GetPropertyValue("AcquisitionDate");
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
                return (DateTime?) base.GetPropertyValue("DisposalDate");
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
                return (int?) base.GetPropertyValue("EconomicLife");
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
                return (int?) base.GetPropertyValue("EngineeringLife");
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
                return (decimal?) base.GetPropertyValue("ExpectedScrapValue");
            }
            set
            {
                base.SetPropertyValue("ExpectedScrapValue", value);
            }
        }
    }
}

