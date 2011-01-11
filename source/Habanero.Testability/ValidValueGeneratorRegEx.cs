using System;
using System.Diagnostics;
using System.IO;
using Habanero.Base;
using Habanero.BO.Rules;

//using Rex;

namespace Habanero.Testability
{
    /// <summary>
    /// Generates valid values that match a given regular expression.
    /// The generation is done using Rex.exe, a Microsoft Research Project executable that must be placed in the
    /// working directory of the application.  The executable is run as a command-line application that runs slowly the
    /// first time and then faster once the operating system stores/caches the exe.
    /// 
    /// You can find the homepage of Rex here: http://research.microsoft.com/en-us/projects/rex/
    /// You can find additional commentary/limitations on Rex here: http://www.kodefuguru.com/post/2010/05/03/Generate-Matches-for-Regular-Expressions-Using-Rex.aspx
    /// </summary>
    /// <remarks>
    /// A potential future improvement of this test is to allow you to build up a cache of values.  For instance, you could
    /// store the first 100 values generated and then access that cache rather than regenerate.  This could give you a significant
    /// performance boost.  The main trouble is how to set that cache size - where do you pass the size through to the generator?
    /// </remarks>
    public class ValidValueGeneratorRegEx : ValidValueGenerator
    {
        /// <summary>
        /// Creates a new instance of the generator
        /// </summary>
        /// <param name="singleValueDef">The property definition that indicates which property the values are being generated for</param>
        /// <param name="regExPhrase">The regular expression against which generated values must match</param>
        public ValidValueGeneratorRegEx(ISingleValueDef singleValueDef, string regExPhrase)
            : base(singleValueDef)
        {
            if (regExPhrase == null) throw new ArgumentNullException("regExPhrase");
            if (regExPhrase == "") throw new ArgumentException("Must supply a non-empty regular expression", "regExPhrase");
            RegExPhrase = regExPhrase;
        }

        /// <summary>
        /// Gets the regular expression against which generated values must match
        /// </summary>
        public string RegExPhrase { get; private set; }

        /// <summary>
        /// Generates a valid value taking into account only the <see cref="IPropRule"/>s. I.e. any <see cref="InterPropRule"/>s 
        /// will not be taken into account. The <see cref="IValidValueGeneratorNumeric"/>'s methods are used
        /// by the BOTestFactory to create valid values taking into account InterPropRules
        /// </summary>
        /// <returns>Returns a string value</returns>
        public override object GenerateValidValue()
        {
            return GenerateValueUsingRex();
        }

        private object GenerateValueUsingRex()
        {
            if (!File.Exists("rex.exe"))
            {
                throw new FileNotFoundException("Cannot find Rex.exe - this is the Rex executable from the Rex Microsoft Research Project which is used to generate random values against a regular expression.  Please place this file in the working directory of the application.");
            }

            var process = new Process
                              {
                                  StartInfo =
                                      {
                                          UseShellExecute = false,
                                          RedirectStandardOutput = true,
                                          CreateNoWindow = true,
                                          FileName = "rex.exe",
                                          Arguments = RegExPhrase + " /encoding:ASCII" //Encoding type prevents cryptic unicode characters
                                      }
                              };
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;

            //ERIC: Rex can be referenced and accessed using the code below, but it's built against x86 so results in BadImageFormatException.
            //  You can get this code working by changing the Testability project to run on x86 rather than any PC (in Build settings).
            //var rexSettings = new RexSettings(RegExPhrase) { encoding = CharacterEncoding.ASCII };
            //var results = RexEngine.GenerateMembers(rexSettings);
            //foreach (var result in results)
            //{
            //    return result;
            //}
            //throw new Exception("Unable to generate a random value according to the regular expression");
        }
    }
}