using Habanero.Base;
using System;
using System.Linq;
using Habanero.BO.Rules;

namespace Habanero.Testability
{
    /// <summary>
    /// This is a base class for a valid value generator of any type.
    /// Generally the <see cref="IPropDef"/>'s <see cref="IPropRule"/>s will 
    /// be taken into consideration when determining the valid value.
    /// E.g. Int, Decimal.
    /// Although this class is public it is primarily intended to be used internally by 
    /// the <see cref="BOTestFactory{T}"/> and <see cref="BOTestFactory"/>.
    /// </summary>
    public abstract class ValidValueGenerator
    {
        protected ValidValueGenerator(IPropDef propDef)
        {
            if (propDef == null)
            {
                throw new ArgumentNullException("propDef");
            }
            this.PropDef = propDef;
        }
        /// <summary>
        /// Generates a valid value taking into account only the <see cref="IPropRule"/>s. I.e. any <see cref="InterPropRule"/>s 
        /// will not be taken into account. The <see cref="IValidValueGeneratorNumeric"/>'s methods are used
        /// by the BOTestFactory to create valid values taking into account InterPropRules
        /// </summary>
        /// <returns></returns>
        public abstract object GenerateValidValue();
        protected TRule GetPropRule<TRule>() where TRule: IPropRule
        {
            return this.PropDef.PropRules.OfType<TRule>().FirstOrDefault();
        }

        protected IPropDef PropDef { get; private set; }
    }


}

