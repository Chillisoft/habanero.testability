using System;
using System.Collections.Generic;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using Habanero.Smooth;

// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace Habanero.Testability.Tests.Base
{
    public class BOTestFactoryFakeBO : BOTestFactory<FakeBO>
    {
    }
    public class FakeBO : BusinessObject
    {

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
            set
            {
                base.SetPropertyValue("NonCompulsoryString", value);
            }
        }
    } 
    public class FakeBOWithAllPropsMapped : BusinessObject
    {
        public virtual string NonCompulsoryString
        {
            get
            {
                return (string)base.GetPropertyValue("NonCompulsoryString");
            }
            set
            {
                base.SetPropertyValue("NonCompulsoryString", value);
            }
        }
        public virtual Guid? FakeBOWithAllPropsMappedID
        {
            get
            {
                return (Guid?)base.GetPropertyValue("FakeBOWithAllPropsMappedID");
            }
            set
            {
                base.SetPropertyValue("FakeBOWithAllPropsMappedID", value);
            }
        }
    }
    public class FakeBOWithNoSetter : BusinessObject
    {

        public virtual string NonCompulsoryString
        {
            get
            {
                return (string)base.GetPropertyValue("NonCompulsoryString");
            }
        }
    }
    public class FakeBOWithNoSetterGetterIncorrectlyMapped : BusinessObject
    {

        public virtual string NonCompulsoryString
        {
            get
            {
                return (string)base.GetPropertyValue("SomeOtherProp");
            }
        }
    }
    public class FakeBOWithNoGetterSetterIncorrectlyMapped : BusinessObject
    {

        public virtual string NonCompulsoryString
        {
            set
            {
                base.SetPropertyValue("SomeOtherProp", value);
            }
        }
    }
    public class FakeBOWithNoGetter : BusinessObject
    {

        public virtual string NonCompulsoryString
        {
            set
            {
                base.SetPropertyValue("NonCompulsoryString", value);
            }
        }

    }
    public class FakeBOWithIncorrectMappings : BusinessObject
    {
        public virtual string GetterNotMapped
        {
            get
            {
                return (string)base.GetPropertyValue("SomeOtherProp");
            }
            set
            {
                base.SetPropertyValue("GetterNotMapped", value);
            }
        }
        public virtual string SetterNotMapped
        {
            get
            {
                return (string)base.GetPropertyValue("SetterNotMapped");
            }
            set
            {
                base.SetPropertyValue("SomeOtherProp", value);
            }
        }
        public virtual string PropertyMappedCorrectly
        {
            get
            {
                return (string)base.GetPropertyValue("PropertyMappedCorrectly");
            }
            set
            {
                base.SetPropertyValue("PropertyMappedCorrectly", value);
            }
        }
        public virtual string GetterAndSetterMappedToIncorrectBOProp
        {
            get
            {
                return (string)base.GetPropertyValue("SomeOtherProp");
            }
            set
            {
                base.SetPropertyValue("SomeOtherProp", value);
            }
        }
    }
    public class FakeBoWithOnePropIncorrectlyMapped : FakeBOWithAllPropsMapped
    {
        public virtual string GetterNotMapped
        {
            get
            {
                return (string)base.GetPropertyValue("SomeOtherProp");
            }
            set
            {
                base.SetPropertyValue("GetterNotMapped", value);
            }
        }
    }

    #region Relationships

    public class FakeBoWithSingleRel : BusinessObject
    {
        public virtual FakeBOWithNothing SingleRelMapped
        {
            get { return Relationships.GetRelatedObject<FakeBOWithNothing>("SingleRelMapped"); }
            set { Relationships.SetRelatedObject("SingleRelMapped", value); }
        }
        [AutoMapManyToOne("RevRel")]
        public virtual FakeBOWithNothing SingleRelGetterNotMapped
        {
            get { return Relationships.GetRelatedObject<FakeBOWithNothing>("SingleRelMapped"); }
            set { Relationships.SetRelatedObject("SingleRelGetterNotMapped", value); }
        }
        [AutoMapManyToOne("RevRel2")]
        public virtual FakeBOWithNothing SingleRelSetterNotMapped
        {
            get { return Relationships.GetRelatedObject<FakeBOWithNothing>("SingleRelSetterNotMapped"); }
            set { Relationships.SetRelatedObject("SingleRelMapped", value); }
        }
        [AutoMapManyToOne("RevRel3")]
        public virtual FakeBOWithNothing SingleRelGetterAndSetterNotMapped
        {
            get { return Relationships.GetRelatedObject<FakeBOWithNothing>("SingleRelMapped"); }
            set { Relationships.SetRelatedObject("SingleRelMapped", value); }
        }
        [AutoMapManyToOne("RevRel4")]
        public virtual FakeBOWithNothing SingleRelNoGetterSetterIncorrect
        {
            set { Relationships.SetRelatedObject("SingleRelMapped", value); }
        }
        [AutoMapManyToOne("RevRel5")]
        public virtual FakeBOWithNothing SingleRelNoSetterGetterIncorrect
        {
            get { return Relationships.GetRelatedObject<FakeBOWithNothing>("SingleRelMapped"); }
        }
        [AutoMapManyToOne("RevRel6")]
        public virtual FakeBOWithNothing SingleRelSetterMappedToNonExistentRelDef
        {
            get { return Relationships.GetRelatedObject<FakeBOWithNothing>("SingleRelSetterMappedToNonExistentRelDef"); }
            set { Relationships.SetRelatedObject("NonExistentRelDef", value); }
        }
    }
    public class FakeBoWithSingleRelGetterNotMapped : BusinessObject
    {
        public virtual FakeBOWithNothing SingleRelMapped
        {
            get { return Relationships.GetRelatedObject<FakeBOWithNothing>("SingleRelMapped"); }
            set { Relationships.SetRelatedObject("SingleRelMapped", value); }
        }
        [AutoMapManyToOne("RevRel")]
        public virtual FakeBOWithNothing SingleRelGetterNotMapped
        {
            get { return Relationships.GetRelatedObject<FakeBOWithNothing>("SingleRelMapped"); }
            set { Relationships.SetRelatedObject("SingleRelGetterNotMapped", value); }
        }
    }
    public class FakeBoWithInvalidMultipleRelAndSingleRels : BusinessObject
    {
        public virtual FakeBOWithNothing SingleRelMapped
        {
            get { return Relationships.GetRelatedObject<FakeBOWithNothing>("SingleRelMapped"); }
            set { Relationships.SetRelatedObject("SingleRelMapped", value); }
        }
        [AutoMapManyToOne("RevRel1")]
        public virtual FakeBOWithNothing SingleRelGetterNotMapped
        {
            get { return Relationships.GetRelatedObject<FakeBOWithNothing>("SingleRelGetterNotMapped"); }
            set { Relationships.SetRelatedObject("SingleRelGetterNotMapped", value); }
        }
        [AutoMapOneToMany("RevRel2")]
        public virtual BusinessObjectCollection<FakeBOWithNothing> InvalidMultipeRel
        {
            get { return Relationships.GetRelatedCollection<FakeBOWithNothing>("Unmatched"); }
        }
    }
    public class FakeBoWithSingleRelNoSetter : BusinessObject
    {
        public virtual FakeBOWithNothing SingleRelMapped
        {
            get { return Relationships.GetRelatedObject<FakeBOWithNothing>("SingleRelMapped"); }
        }
    }
    public class FakeBoWithSingleRelNoGetter : BusinessObject
    {
        public virtual FakeBOWithNothing SingleRelMapped
        {
            set { Relationships.SetRelatedObject("SingleRelMapped", value); }
        }
    }
    public class FakeBOWithNothing : BusinessObject
    { }

    #endregion

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


    public enum FakeEnum
    {
        SomeNum,
        AnotherNum
    }

    public class PropDefFake : PropDef
    {
        public PropDefFake()
            : base(RandomValueGen.GetRandomString(), typeof(int), PropReadWriteRule.ReadWrite, null)
        {
        }
        public PropDefFake(string propName)
            : base(propName, typeof(string), PropReadWriteRule.ReadWrite, null)
        {
        }
    }

    public class BOFakeWithDefault : BusinessObject
    {
        protected override IClassDef ConstructClassDef()
        {
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            return base.GetType().MapClass();
        }
        [AutoMapDefault("SomeValue")]
        public virtual string DefaultProp
        {
            get
            {
                return (string)base.GetPropertyValue("DefaultProp");
            }
        }

        public virtual string NonDefaultProp
        {
            get
            {
                return (string)base.GetPropertyValue("NonDefaultProp");
            }
        }
    }
    public class BOFakeWithCompulsory : BusinessObject
    {
        protected override IClassDef ConstructClassDef()
        {
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            return base.GetType().MapClass();
        }
        [AutoMapCompulsory]
        public virtual string CompulsoryProp
        {
            get
            {
                return (string)base.GetPropertyValue("CompulsoryProp");
            }
        }

        public virtual string NonCompulsoryProp
        {
            get
            {
                return (string)base.GetPropertyValue("NonCompulsoryProp");
            }
        }
    }

    public class FakeBOWithReadWriteRuleProp : BusinessObject
    {
        [AutoMapReadWriteRule(PropReadWriteRule.ReadOnly)]
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public DateTime ReadWriteRuleReadOnly { get; set; }

        [AutoMapReadWriteRule(PropReadWriteRule.ReadWrite)]
        public DateTime ReadWriteRuleReadWrite { get; set; }

        public String ReadWriteRuleDefault { get; set; }
    }
    
    public class FakeBOWithUniqueConstraint : BusinessObject
    {
        [AutoMapUniqueConstraint("UC1")]
        public String UCProp { get; set; }

        [AutoMapUniqueConstraint("UC2")]
        public String ComplexUCProp1 { get; set; }
        [AutoMapUniqueConstraint("UC2")]
        public String ComplexUCProp2 { get; set; }

        public String NonUCProp { get; set; }
    }
    
    public class RelatedFakeBo : BusinessObject
    {

    }
    public class SuperClassFakeBO: BusinessObject
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
        public RelatedFakeBo SomeRelationship
        {
            get
            {
                return base.Relationships.GetRelatedObject<RelatedFakeBo>("SomeRelationship");
            }
        }
    }
    public class SubClassFakeBO : SuperClassFakeBO
    {

    }

    [AutoMapIgnore]
    public class Unmapped : BusinessObject
    {


        public virtual string SomeProp
        {
            get
            {
                return (string)base.GetPropertyValue("SomeProp");
            }
        }
    }

    public class FakeBOWithReflectiveProp: BusinessObject
    {

        public string ReflectiveProp
        {
            get; set;
        }
    }
    public class FakeBOWithRelationship : BusinessObject
    {


        public RelatedFakeBo SingleRelationship
        {
            get
            {
                return base.Relationships.GetRelatedObject<RelatedFakeBo>("SingleRelationship");
            }
            set
            {
                base.Relationships.SetRelatedObject("SingleRelationship", value);
            }
        }

        public virtual string SomeProp
        {
            get
            {
                return (string)base.GetPropertyValue("SomeProp");
            }
            set
            {
                base.SetPropertyValue("SomeProp", value);
            }
        }

        public virtual bool? BoolProp
        {
            get
            {
                return (bool?)base.GetPropertyValue("BoolProp");
            }
            set
            {
                base.SetPropertyValue("BoolProp", value);
            }
        }
        [AutoMapDefault("true")]
        public virtual bool? BoolPropWithDefault
        {
            get
            {
                return (bool?)base.GetPropertyValue("BoolPropWithDefault");
            }
            set
            {
                base.SetPropertyValue("BoolPropWithDefault", value);
            }
        }
    }
    public class FakeBOWithManyRelationship:BusinessObject
    {
        public virtual BusinessObjectCollection<RelatedFakeBo> RelatedFakeBos
        {
            get
            {
                return Relationships.GetRelatedCollection<RelatedFakeBo>("RelatedFakeBos");
            }
        }        
        public virtual BusinessObjectCollection<RelatedFakeBo> OtherRelatedFakeBos
        {
            get
            {
                return Relationships.GetRelatedCollection<RelatedFakeBo>("OtherRelatedFakeBos");
            }
        }
    }    
    [AutoMapIgnore]
    public class FakeBOCompositeKeyAndManyRel:BusinessObject
    {
        public static IClassDef LoadDefaultClassDef()
        {
            XmlClassLoader itsLoader = new XmlClassLoader(new DtdLoader(), new DefClassFactory());
            IClassDef itsClassDef = itsLoader.LoadClass(@"
			  <class name=""FakeBOCompositeKeyAndManyRel"" assembly=""Habanero.Testability.Tests.Base"">
			    <property name=""Name"" compulsory=""true""/>
			    <property name=""SomeProp""/>
			    <property name=""OrganisationID"" compulsory=""true"" />
			    <primaryKey isObjectID=""false"">
			      <prop name=""OrganisationID"" />
			      <prop name=""Name"" />
			    </primaryKey>
			    <relationship name=""RelatedFakeBos"" type=""multiple"" relatedClass=""FakeBOCompositeKeySingleRel"" relatedAssembly=""Habanero.Testability.Tests.Base"">
			      <relatedProperty property=""OrganisationID"" relatedProperty=""OrganisationID"" />
			      <relatedProperty property=""Name"" relatedProperty=""Name"" />
			    </relationship>
			  </class>
			");
            ClassDef.ClassDefs.Add(itsClassDef);
            return itsClassDef;
        }
        public virtual BusinessObjectCollection<FakeBOCompositeKeySingleRel> RelatedFakeBos
        {
            get
            {
                return Relationships.GetRelatedCollection<FakeBOCompositeKeySingleRel>("RelatedFakeBos");
            }
        }        
    }
    [AutoMapIgnore]
    public class FakeBOCompositeKeySingleRel:BusinessObject
    {
        public static IClassDef LoadDefaultClassDef()
        {
            XmlClassLoader itsLoader = new XmlClassLoader(new DtdLoader(), new DefClassFactory());
            IClassDef itsClassDef = itsLoader.LoadClass(@"
			  <class name=""FakeBOCompositeKeySingleRel"" assembly=""Habanero.Testability.Tests.Base"">
			    <property name=""Name"" compulsory=""true""/>
			    <property name=""OrganisationID"" compulsory=""true"" />
			    <property name=""AnotherPKProp"" compulsory=""true"" />
			    <property name=""AnotherProp"" />
			    <primaryKey isObjectID=""false"">
			      <prop name=""OrganisationID"" />
			      <prop name=""Name"" />
			      <prop name=""AnotherPKProp"" />
			    </primaryKey>
			    <relationship name=""FakeBO"" type=""single"" relatedClass=""FakeBOCompositeKeyAndManyRel"" relatedAssembly=""Habanero.Testability.Tests.Base"">
			      <relatedProperty property=""OrganisationID"" relatedProperty=""OrganisationID"" />
			      <relatedProperty property=""Name"" relatedProperty=""Name"" />
			    </relationship>
			  </class>
			");
            ClassDef.ClassDefs.Add(itsClassDef);
            return itsClassDef;
        }       
    }



    public class FakeLListBO : BusinessObject
    {
        protected override IClassDef ConstructClassDef()
        {
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            return base.GetType().MapClass();
        }


        [AutoMapCompulsory]
        public virtual string Description
        {
            get
            {
                return (string)base.GetPropertyValue("Description");
            }
        }
    } 
    // ReSharper restore UnusedMember.Global
    // ReSharper restore ClassNeverInstantiated.Global
    // ReSharper restore UnusedAutoPropertyAccessor.Global

}
