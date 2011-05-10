using Habanero.Base;
using Habanero.BO;
using Habanero.BO.Rules;

namespace Habanero.Testability.CF
{
    public class ValidValueGeneratorString : ValidValueGenerator
    {
        public ValidValueGeneratorString(IPropDef propDef) : base(propDef)
        {
        }

        /// <summary>
        /// Generates a valid value taking into account only the <see cref="IPropRule"/>s. I.e. any <see cref="InterPropRule"/>s 
        /// will not be taken into account. The <see cref="IValidValueGeneratorNumeric"/>'s methods are used
        /// by the BOTestFactory to create valid values taking into account InterPropRules
        /// </summary>
        /// <returns></returns>
        public override object GenerateValidValue()
        {
            return this.GenerateValidValueTyped();
        }

        private string GenerateValidValueTyped()
        {
            PropRuleString propRule = base.GetPropRule<PropRuleString>();
            return ((propRule == null) ? RandomValueGen.GetRandomString() : RandomValueGen.GetRandomString(propRule.MinLength, propRule.MaxLength));
        }
    }
}

