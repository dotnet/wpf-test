using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Test.Pict.MatrixOutput;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// VariationGenerator
    /// </summary>
    public static class VariationGenerator
    {
        /// <summary>
        /// Generate pict created variations
        /// </summary>
        /// <param name="fileName">pict file name</param>
        /// <param name="args">pict args</param>
        /// <param name="excludes">pict excludes variations</param>
        /// <returns>A list of PairwiseTestCase</returns>
        public static List<PairwiseTestCase> GeneratePictVariations(string fileName, string args, string excludes)
        {
            List<int> excludedTests = new List<int>();
            string[] excludeIndexes = excludes.Split(new char[] { ',' });
            foreach (string excludeIndex in excludeIndexes)
            {
                // To support empty string such as ""
                if (!String.IsNullOrEmpty(excludeIndex))
                {
                    excludedTests.Add(Convert.ToInt32(excludeIndex));
                }
            }

            List<PairwiseTestCase> pairwiseTestCases = new List<PairwiseTestCase>();

            PairwiseTestCase[] pictGeneratedVariations = PairwiseHelper.GenerateTestsFromFile(fileName, args);

            for (int index = 0; index < pictGeneratedVariations.Length; index++)
            {
                if (!excludedTests.Contains(index))
                {
                    pairwiseTestCases.Add(pictGeneratedVariations[index]);
                }
            }

            return pairwiseTestCases;
        }

        /// <summary>
        /// Generate manual created variations
        /// </summary>
        /// <param name="xamlsElement">A XmlElement reference of a list of xaml file names</param>
        /// <param name="variations">A XmlNodeList reference of a list of variations</param>
        /// <returns>A list of name value pair variations</returns>
        public static List<Dictionary<string, string>> GenerateVariations(XmlElement xamlsElement, XmlNodeList variations)
        {
            List<Dictionary<string, string>> testVariations = new List<Dictionary<string, string>>();

            foreach (XmlElement xamlElement in xamlsElement)
            {
                foreach (XmlElement variation in variations)
                {
                    Dictionary<string, string> testVariation = new Dictionary<string, string>();
                    testVariation.Add("XamlFileName", xamlElement.Attributes["Name"].Value);

                    foreach (XmlAttribute attribute in variation.Attributes)
                    {
                        testVariation.Add(attribute.Name, attribute.Value);
                    }
                    testVariations.Add(testVariation);
                }
            }

            return testVariations;
        }

        /// <summary>
        /// Manual created variations override pict generated variations
        /// </summary>
        /// <param name="pictVariations">A list of pict created variations</param>
        /// <param name="variations">A list of manual defined variations</param>
        /// <returns>A list of name value pair variations</returns>
        public static List<Dictionary<string, string>> CombineVariations(List<PairwiseTestCase> pictVariations, List<Dictionary<string, string>> variations)
        {
            List<Dictionary<string, string>> testVariations = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> variation in variations)
            {
                foreach (PairwiseTestCase pictVariation in pictVariations)
                {
                    // Create a place holder for variations because Dictionary is reference type
                    Dictionary<string, string> testVariation = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> keyValuePair in variation)
                    {
                        testVariation.Add(keyValuePair.Key, keyValuePair.Value);
                    }

                    foreach (PairwiseTestParameter pictParameter in pictVariation.Parameters)
                    {
                        if (!variation.ContainsKey(pictParameter.Name))
                        {
                            testVariation.Add(pictParameter.Name, pictParameter.Value);
                        }
                    }

                    testVariations.Add(testVariation);
                }
            }

            return testVariations;
        }
    }
}


