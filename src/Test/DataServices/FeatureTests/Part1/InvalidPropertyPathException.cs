// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Markup;
using System.IO;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where Invalid PropertyPath should cause an exception, but it doesn't.
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "InvalidPropertyPathException", SupportFiles = @"FeatureTests\DataServices\Content\InvalidPropertyPathException.xaml")]
    public class InvalidPropertyPathException : AvalonTest
    {
        #region Constructors

        public InvalidPropertyPathException()            
        {
                InitializeSteps += new TestStep(Setup);           
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {

            FileStream file = new FileStream(@"InvalidPropertyPathException.xaml", FileMode.Open);           
            
            try
            {
                XamlReader.Load(file);                
            }
            catch (XamlParseException)
            {
                LogComment("XamlParseException was thrown");
                return TestResult.Pass;
            }

            LogComment("XamlParseException NOT thrown!");
            return TestResult.Fail;
        }

        #endregion
		
    }	
}
