namespace Habanero.Testability
{
    using Habanero.Base;
    using System;
    /// <summary>
    /// Generates a new Guid when GenerateValidValue is called
    /// </summary>
    public class ValidValueGeneratorGuid : ValidValueGenerator
    {
        /// <summary>
        /// Constructs the valid value generator for this propdef
        /// </summary>
        /// <param name="propDef"></param>
        public ValidValueGeneratorGuid(IPropDef propDef) : base(propDef)
        {
        }
        /// <summary>
        /// Creates a new Guid.
        /// </summary>
        /// <returns></returns>
        public override object GenerateValidValue()
        {
            return Guid.NewGuid();
        }
    }
}

