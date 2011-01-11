using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.Rules;

namespace Habanero.Testability
{
    /// <summary>
    /// Generates random values from a text file.  If a filename is not specified, the generator will look for a filename
    /// that has the name of the property (e.g. FirstName) with a ".txt" extension.  Each item should appear on a different
    /// line in the text file.  Once the values have been read from the file once, the items are cached in memory for rapid
    /// test performance.
    /// </summary>
    public class ValidValueGeneratorTextFile : ValidValueGenerator
    {
        private static Dictionary<string, List<string>> _cachedSampleValues;
       
        public ValidValueGeneratorTextFile(IPropDef propDef):this(propDef, null)
        {
            
        }
        /// <summary>
        /// Constructs a new instance of the generator
        /// </summary>
        /// <param name="propDef">The definition of the property for which random values will be generated</param>
        /// <param name="fileName">The filename that contains the available values, including the path.  If the path is relative,
        /// it will relate to the execution directory.  If no filename is specified, the filename used will be the
        /// property name (e.g. FirstName) with a ".txt" extension.</param>
        public ValidValueGeneratorTextFile(IPropDef propDef, string fileName)
            : base(propDef)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = propDef.PropertyName + ".txt";
            }
            FileName = fileName;
        }

        /// <summary>
        /// Gets the filename from which the sample values will be loaded
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the collection of sample values read from the file.
        /// </summary>
        public List<string> SampleValues { get; private set; }

        /// <summary>
        /// Gets the cached sample values per filename
        /// </summary>
        public static Dictionary<string, List<string>> CachedSampleValues
        {
            get
            {
                if (_cachedSampleValues == null) _cachedSampleValues = new Dictionary<string, List<string>>();
                return _cachedSampleValues;
            }
        }

        /// <summary>
        /// Generates a valid value taking into account only the <see cref="IPropRule"/>s. I.e. any <see cref="InterPropRule"/>s 
        /// will not be taken into account. The <see cref="IValidValueGeneratorNumeric"/>'s methods are used
        /// by the BOTestFactory to create valid values taking into account InterPropRules
        /// </summary>
        /// <returns></returns>
        public override object GenerateValidValue()
        {
            CreateSampleValues();

            var randomPos = RandomValueGen.GetRandomInt(0, SampleValues.Count - 1);
            object sampleValue = SampleValues[randomPos];
            if (this.SingleValueDef.PropertyType != typeof(string))
            {
                var typeConverter = new TypeConverter();
                sampleValue = typeConverter.ConvertTo(sampleValue, this.SingleValueDef.PropertyType);
            }
            return sampleValue;
        }

        private void CreateSampleValues()
        {
            if (SampleValues == null || SampleValues.Count == 0)
            {
                if (CachedSampleValues.ContainsKey(FileName))
                {
                    SampleValues = CachedSampleValues[FileName];
                }
                else
                {
                    SampleValues = GetStringsFromFile();
                    CachedSampleValues.Add(FileName, SampleValues);
                }
            }
            if (SampleValues.Count == 0)
            {
                throw new HabaneroApplicationException("The test data file " + FileName + " does not contain any data to use for random data generation.");
            }
        }

        private List<string> GetStringsFromFile()
        {
            StreamReader streamReader = File.OpenText(FileName);
            var items = new List<string>();
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                if (line == null) break;
                items.Add(line);
            }
            streamReader.Close();
            return items;
        }
    }
}