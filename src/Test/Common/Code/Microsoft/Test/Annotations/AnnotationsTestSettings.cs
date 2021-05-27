// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using Annotations.Test.Framework.Internal;

namespace Microsoft.Test.Annotations
{
    /// <summary>
    /// Contract for creating/decoding DriverParameters between Annotation adaptor and driver.
    /// </summary>
    public class AnnotationsTestSettings
    {
        #region Private Data

        private bool usage = false;

        private const string targetAssemblyKey = "targetassembly";
        private const string suiteKey = "suite";
        private const string testIdKey = "testid";
        private const string commandLineKey = "commandline";

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize from property bag.
        /// </summary>
        /// <param name="definition"></param>
        public AnnotationsTestSettings()
        {
            usage = DriverState.DriverParameters.ContainsProperty("?");

            if (!DriverState.DriverParameters.ContainsProperty(targetAssemblyKey))
                throw new ArgumentException("Missing required parameter: " + targetAssemblyKey);
            if (!DriverState.DriverParameters.ContainsProperty(suiteKey))
                throw new ArgumentException("Missing required parameter: " + suiteKey);

            // If usage has been requested then the testId and commandline are not required.
            if (!Usage)
            {
                if (!DriverState.DriverParameters.ContainsProperty(testIdKey))
                    throw new ArgumentException("Missing required parameter: " + testIdKey);
                // Commandline may be null, but we'll change it to empty string.
                if (!DriverState.DriverParameters.ContainsProperty(commandLineKey))
                    DriverState.DriverParameters[commandLineKey] = "";
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// True if usage information has been requested.
        /// </summary>
        public bool Usage
        {
            get { return usage; }
        }

        /// <summary>
        /// Name of Assembly containing TestSuite.
        /// </summary>
        public string TargetAssemblyName
        {
            get { return DriverState.DriverParameters[targetAssemblyKey]; }
        }

        /// <summary>
        /// Name of TestSuite containing TestId.
        /// </summary>
        public string SuiteName
        {
            get { return DriverState.DriverParameters[suiteKey]; }
        }

        /// <summary>
        /// Id of test.
        /// </summary>
        public string TestId
        {
            get { return DriverState.DriverParameters[testIdKey]; }
        }

        /// <summary>
        /// Commandline for configuration a variation.
        /// </summary>
        public string CommandLine
        {
            get { return DriverState.DriverParameters[commandLineKey]; }
        }

        #endregion

        #region Public Methods

        internal static ContentPropertyBag ToDriverParameters(Assembly testAssembly, TestVariation variation)
        {
            ContentPropertyBag driverParams = new ContentPropertyBag();
            // Use assembly partial name because we will be loading assembly from the
            // local directory, not the GAC.
            driverParams[targetAssemblyKey] = testAssembly.GetName().Name;
            driverParams[suiteKey] = variation.TestCase.Suite.GetType().FullName;
            driverParams[testIdKey] = variation.TestCase.Id;
            driverParams[commandLineKey] = string.Join(" ", variation.Parameters);
            return driverParams;
        }

        /// <summary>
        /// Output usage.
        /// </summary>
        public static void PrintUsage()
        {
            Console.WriteLine("    {0}\t\tName of assembly to execute.", targetAssemblyKey);
            Console.WriteLine("    {1}\t\tFull class name of TestSuite to inspect.", suiteKey);
            Console.WriteLine("    {2}\t\tName of test to execute.", testIdKey);
            Console.WriteLine("    {3}\t\tCommand line which controls specific test variation to run.", commandLineKey);
            Console.WriteLine();
            Console.WriteLine("    {1} /?\t\tPrint usage information for a specific TestSuite.", suiteKey);
        }

        #endregion
    }
}
