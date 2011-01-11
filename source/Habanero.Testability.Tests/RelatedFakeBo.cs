namespace Habanero.Testability.Tests
{
    using AutoMappingHabanero;
    using Habanero.Base;
    using Habanero.BO;
    using Habanero.BO.ClassDefinition;

    public class RelatedFakeBo : BusinessObject
    {
        protected override IClassDef ConstructClassDef()
        {
            AllClassesAutoMapper.ClassDefCol = ClassDef.ClassDefs;
            return base.GetType().MapClass();
        }
    }
}

