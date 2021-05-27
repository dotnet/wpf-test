// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------
//  Description: A single test case variation.
//  Creator: derekme
//  Date Created: 9/8/05
//---------------------------------------------------------------------
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Annotations.Test.Framework.Internal
{
    internal class TestCase
    {
        #region Ctors

        public TestCase(TestSuite suite, string id, string [] args)
        {
            _suite = suite;
            _id = id;
            _args = (args == null) ? new string[0] : args;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Parse the command line argumenst that are passed to the test.  If an arg like: '/dimension=[A,B,C]' is
        /// found, it will create a new arg array for each permutation of the items declared in the '/dimension' arg.
        /// 
        /// Example: 
        ///   Command Line = XXX.exe param1 param2 /dimension=[var1,var2]        
        ///   Generates Variations = 
        ///         XXX.exe param1 param2 var1
        ///         XXX.exe param1 param2 var2
        /// 
        ///   Command Line = XXX.exe param1 param2 /dimension=[var1,var2]        
        ///   Generates Variations = 
        ///         XXX.exe param1 param2 var1
        ///         XXX.exe param1 param2 var2
        /// </summary>
        /// <param name="args"></param>
        private void CreateVariationCollection(string[] args)
        {
            IList<Dimension> dimensions = null;
            // If no arguments look at the test case definition for variations.
            if (args.Length == 0 || (args.Length == 1 && (args[0].Equals(Id) || new Regex("/[s|S]uite=.*").Match(args[0]).Success)))
            {
                if (TestMethod != null)
                {                    
                    dimensions = new List<Dimension>();
                    if (TestMethod.GetCustomAttributes(typeof(OverrideClassTestDimensions), false).Length == 0)
                    {
                        bool inherit = TestMethod.DeclaringType.GetCustomAttributes(typeof(OverrideClassTestDimensions), false).Length == 0;
                        ProcessTestDimensions(dimensions, TestMethod.DeclaringType.GetCustomAttributes(typeof(TestDimension), inherit));
                    }
                    ProcessTestDimensions(dimensions, TestMethod.GetCustomAttributes(typeof(TestDimension), false));
                }
            }
            else
            {
                dimensions = GetDimensions(ref args);
            }

            _variations = new List<TestVariation>();
            if (dimensions == null || dimensions.Count == 0)
                _variations.Add(new TestVariation(this, args));
            else
                EnumerateTestVariations(args, dimensions);
        }

        /// <summary>
        /// Parse command line args and find all the '/dimension=[XXX]' parameters. Where XXX is a comma
        /// delimited list of parameters.
        /// Modifies the cmdArgs list to remove all /dimension parameters.
        /// </summary>
        /// <returns>List of parameters defined by '/dimension=[XXX]' arg.</returns>
        private IList<Dimension> GetDimensions(ref string[] cmdArgs)
        {
            IList<Dimension> dimensions = new List<Dimension>();

            // Rebuild the full commandline that way we can supports dimension lists that
            // contain whitespace.
            string cmdLine = "";
            foreach (string arg in cmdArgs)
                cmdLine += arg + " ";

            // Parse the command line for all the '/dimension' arguments.
            Match match;
            Regex variationExpression = new Regex("(" + DIMENSION + "=\\[.*?\\])");
            if ((match = variationExpression.Match(cmdLine)).Success)
            {
                do
                {
                    string dimension = match.Groups[1].Value;
                    string parameters = new Regex("=\\[(.*)\\]").Match(dimension).Groups[1].Value;
                    dimensions.Add(new Dimension(parameters.Split(',')));
                    cmdLine = cmdLine.Replace(dimension, "");
                } while ((match = match.NextMatch()).Success);
            }

            // Remove /dimension parameters from arg list.
            cmdArgs = cmdLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            return dimensions;
        }

        /// <summary>
        /// Add an etry in _variations for each permutation of variables across all dimensions.
        /// 
        /// Example: If dimensions.Count == 2 then an entry for each permutation of dimensions[0] and 
        /// dimensions[1] appended to args will be added to _variations.
        /// </summary>
        private void EnumerateTestVariations(string[] cmdArgs, IList<Dimension> dimensions)
        {
            IList<string> dimensionPermutations = EnumerateDimensionPermutations(dimensions);
            foreach (string permutation in dimensionPermutations)
            {
                // Permutations are stored as space delimited strings so split it back into parts.
                string[] permutationParts = permutation.Split(' ');
                string[] testVariation = new string[cmdArgs.Length + permutationParts.Length];

                Array.Copy(cmdArgs, testVariation, cmdArgs.Length);
                // Append variation parameters at the end.
                Array.Copy(permutationParts, 0, testVariation, cmdArgs.Length, permutationParts.Length);

                // Add variation to collection.
                _variations.Add(new TestVariation(this, testVariation));
            }
        }

        /// <summary>
        /// Recusively compute all permutations of each entry of each dimension and return them as a list
        /// of strings.
        /// 
        /// Example:
        ///     dimensions = {[a,b],[1,2,3],[%]}
        ///     Return value = { "a 1 %", "a 2 %", "a 3 %", "b 1 %", "b 2 %", "b 3 %" }
        /// </summary>
        /// <returns>List of all permutations of variables across all dimensions.</returns>
        IList<string> EnumerateDimensionPermutations(IList<Dimension> dimensions)
        {
            IList<string> permutations = new List<string>();
            for (int varIdx = 0; varIdx < dimensions[0].Values.Length; varIdx++)
            {
                string currentVar = dimensions[0].Values[varIdx];

                // If there are no more dimensions to compute permutations for then the current
                // variation is a final permutation.
                if (dimensions.Count == 1)
                {
                    permutations.Add(currentVar);
                }
                // Otherwise, we need to calculate the next level of permutations for the current
                // variation.
                else
                {
                    // Compute all the permutations of the current variation against the next dimension
                    // (e.g. combine dimensions[0] and dimensions[1] into a single dimension.
                    Dimension partialPermutations = EnumerateDimensionPermutations(currentVar, dimensions[1]);

                    // Compress the dimension set by 1.
                    IList<Dimension> remainingDimensions = new List<Dimension>();
                    remainingDimensions.Add(partialPermutations);
                    for (int i = 2; i < dimensions.Count; i++)
                        remainingDimensions.Add(dimensions[i]);

                    // Recurse on the compressed dimension set, return value will be full
                    // set of permutations for this variable across all remainingDimensions.
                    IList<string> permutationsForVar = EnumerateDimensionPermutations(remainingDimensions);
                    foreach (string permutation in permutationsForVar)
                        permutations.Add(permutation);
                }
            }
            return permutations;
        }

        /// <summary>
        /// Compute all permutations of a single variable across 1 dimension.
        /// </summary>
        /// <returns>All permutations of 'permutation' and each variable in the dimension.</returns>
        Dimension EnumerateDimensionPermutations(string permutation, Dimension currentDimension)
        {
            string[] permutations = new string[currentDimension.Values.Length];
            for (int i = 0; i < currentDimension.Values.Length; i++)
                permutations[i] = permutation + " " + currentDimension.Values[i];
            return new Dimension(permutations);
        }

        /// <summary>
        /// Processes N TestDimension attributes and add them to the dimension list.
        /// </summary>
        void ProcessTestDimensions(IList<Dimension> dimensions, object[] attributes)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                string[] values = ((TestDimension)attributes[i]).Values.Split(',');
                Dimension dim = new Dimension(values);
                // Throw if we detect a duplicate Dimension declaration, this probably
                // means that dimensions are being accidentally inherited.
                if (dimensions.Contains(dim))
                    throw new InvalidOperationException("The following dimension was defined multiple times for TestId '" + Id + "': [" + dim.ToString() + "].");
                dimensions.Add(dim);
            }
        }

        #endregion

        #region Public Properties

        public TestSuite Suite { get { return _suite; } }

        public string Id { get { return _id; } }

        /// <summary>
        /// Get all Bugs associated with this TestCase.
        /// </summary>
        public int[] Bugs
        {
            get
            {
                if (_bugs == null)
                {
                    _bugs = new int[0];
                    if (TestMethod != null)
                    {
                        object[] attributes = TestMethod.GetCustomAttributes(typeof(BugId), false);
                        if (attributes.Length > 0)
                        {
                            _bugs = new int[attributes.Length];
                            for (int i = 0; i < attributes.Length; i++)
                            {
                                _bugs[i] = ((BugId)attributes[i]).Id;
                            }
                        }
                    }
                }
                return _bugs;
            }
        }

        /// <summary>
        /// Returns true if "DisableTestCase" is set for this case, meaning it should
        /// not be run unless explicitly requested.
        /// </summary>
        public bool IsDisabled
        {
            get
            {
                if (TestMethod != null)
                {
                    object[] attributes = TestMethod.GetCustomAttributes(typeof(DisabledTestCase), false);
                    return attributes.Length > 0;
                }
                return false;
            }
        }

        public string Keywords
        {
            get
            {
                if (TestMethod != null)
                {
                    object[] attributes = TestMethod.GetCustomAttributes(typeof(Keywords), false);
                    if (attributes.Length > 0)
                    {
                        return ((Keywords)attributes[0]).Value;
                    }
                }
                return string.Empty;
            }
        }

        public int Priority
        {
            get 
            {
                if (TestMethod != null)
                {
                    object[] attributes = TestMethod.GetCustomAttributes(typeof(Priority), false);
                    if (attributes.Length > 0)
                    {
                        return ((Priority)attributes[0]).Value;
                    }
                }
                return -1;
            }
        }

        public IList<TestVariation> Variations 
        { 
            get 
            {
                if (_variations == null)
                {
                    CreateVariationCollection(_args);
                }
                return _variations; 
            } 
        }

        public MethodInfo SetupMethod;
        public MethodInfo CleanupMethod;
        public MethodInfo TestMethod;        

        #endregion

        #region Fields

        private static string DIMENSION = "/dimension";

        private TestSuite _suite;
        private string _id;
        private string[] _args;
        private IList<TestVariation> _variations;
        private int[] _bugs;

        #endregion

        /// <summary>
        /// Encapsulates the values of a single TestDimension. 
        /// </summary>
        /// <remarks>Used to more efficiently detect and report duplicate dimension declarations.</remarks>
        class Dimension
        {
            #region Constructor

            public Dimension(string[] values)
            {
                _values = values;

                // Values order doesn't matter, sort them so that the ToString
                // representation will be the same regardless of order, that way
                // we can detect duplicate dimension definitions more robustly.
                Array.Sort(_values);
            }

            #endregion

            #region Public Properties

            public string[] Values
            {
                get { return _values; }
            }

            #endregion

            #region Public Methods

            public override string ToString()
            {
                if (_asString == null)
                {
                    _asString = string.Empty;
                    for (int i = 0; i < Values.Length; i++)
                        _asString += (i == Values.Length - 1) ? Values[i] : Values[i] + ",";
                }
                return _asString;
            }

            /// <summary>
            /// Dimensions are equal if their sorted list of Values is equal.
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                return obj.ToString().Equals(this.ToString());
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            #endregion

            #region Fields

            private string[] _values;
            private string _asString = null;

            #endregion
        }
    }    
}
