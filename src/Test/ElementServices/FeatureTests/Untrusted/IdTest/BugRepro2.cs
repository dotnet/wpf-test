// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows;

using Avalon.Test.CoreUI;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.ElementServices.Untrusted.IdTest
{
    /// <summary>
    /// Regression tests 

    [Test(1, "IdTest", TestCaseSecurityLevel.PartialTrust, "BugRepro2", SupportFiles=@"FeatureTests\ElementServices\BugRepro2.xaml")]
    public class BugRepro2 : TestCase
    {
        /// <summary>
        /// Regression test for 

        #region Constructor
        public BugRepro2()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            Stream xamlStream = File.OpenRead("BugRepro2.xaml");
            Page page = (Page)XamlReader.Load(xamlStream);

            Grid grid1 = (Grid)LogicalTreeHelper.FindLogicalNode((DependencyObject)page.Content,"grid1");
            if (grid1 == null)
            {
                throw new TestValidationException("grid1 is null");
            }
            Button MyButton = (Button)grid1.FindName("MyButton");
            if (MyButton == null)
            {
                throw new TestValidationException("MyButton is null");
            }

            DataTemplate dt = (DataTemplate)MyButton.FindResource("MyDataTemplate");
            Grid g = (Grid)dt.LoadContent();
            Ellipse ellipse = (Ellipse)g.FindName("MyEllipse");
            if (ellipse != null)
            {
                return TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("ellipse is null");
                return TestResult.Fail;
            }
        }
        #endregion
    }
}
