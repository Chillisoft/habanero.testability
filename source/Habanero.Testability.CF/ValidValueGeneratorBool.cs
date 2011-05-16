using Habanero.Base;
using Habanero.BO.Rules;

namespace Habanero.Testability
{
    /// <summary>
    /// Generates a valid value for PropDef of type DateTime.
    /// </summary>
    public class ValidValueGeneratorBool : ValidValueGenerator
    {
        public ValidValueGeneratorBool(IPropDef propDef) : base(propDef)
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
            return RandomValueGen.GetRandomBoolean();
        }
    }
}

