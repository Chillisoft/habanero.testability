using Habanero.Base;
using Habanero.BO.Rules;

namespace Habanero.Testability.CF
{
    /// <summary>
    /// This will generate a valid value for the Enum identified by
    /// <see cref="IPropDef"/>.<see cref="IPropDef.PropertyType"/>
    /// </summary>
    public class ValidValueGeneratorEnum : ValidValueGenerator
    {
        public ValidValueGeneratorEnum(ISingleValueDef propDef) : base(propDef)
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
            return RandomValueGen.GetRandomEnum(base.SingleValueDef.PropertyType);
        }
    }
}

