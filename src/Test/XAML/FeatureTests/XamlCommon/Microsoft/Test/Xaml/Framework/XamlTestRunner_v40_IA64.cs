// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.TestTypes;

namespace Microsoft.Test.Xaml.Framework
{
    /// <summary>
    /// Class for running Xaml Tests
    /// </summary>
    public static class XamlTestRunner
    {
        #region Static Methods

        /// <summary>
        /// Entry point for Xaml Test Framework
        /// </summary>
        public static void RunTest()
        {
            string testType = DriverState.DriverParameters["XamlTestType"].ToUpperInvariant();
            XamlTestType test;
            switch (testType)
            {
                case "CDFMETHODTEST":
                    test = new CDFMethodTest();
                    break;
                
                default:
                    throw new Exception("Unrecognized Test Type");
            }

            test.Run();
        }

        #endregion
    }
}
