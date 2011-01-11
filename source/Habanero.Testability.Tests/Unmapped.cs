namespace Habanero.Testability.Tests
{
    using AutoMappingHabanero;
    using Habanero.Base;
    using Habanero.BO;
    using Habanero.BO.ClassDefinition;
    using System;

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
                return (string) base.GetPropertyValue("SomeProp");
            }
        }
    }
}

