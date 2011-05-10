using Habanero.Base;
using Habanero.BO.Rules;

namespace Habanero.Testability.CF
{
    /// <summary>
    /// An inverface for Objects that are of type that is generally numeric.
    /// e.g. DateTime, Single, Double, Decimal, Int, Long.
    /// This interface provides an interface for generating a Random Number that is Greater
    /// than or less than a certain value. This is primarily used
    /// for generating valid values where <see cref="InterPropRule"/>s exist.
    /// The Methods will generate a valid value taking the <see cref="IPropRule"/> and the
    /// minValue/maxValue derived from the <see cref="InterPropRule"/> into account.
    /// </summary>
    public interface IValidValueGeneratorNumeric
    {
        /// <summary>
        /// Generates a Valid Value taking <see cref="IPropRule"/> and minValue into 
        /// account.
        /// </summary>
        /// <param name="minValue"></param>
        /// <returns></returns>
        object GenerateValidValueGreaterThan(object minValue);
        /// <summary>
        /// Generates a Valid Value taking <see cref="IPropRule"/> and maxValue into 
        /// account.
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        object GenerateValidValueLessThan(object maxValue);
    }
}

