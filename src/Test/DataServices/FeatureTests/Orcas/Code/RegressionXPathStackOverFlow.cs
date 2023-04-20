// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Xml;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test : Check if the datacontext property is set correctly - the bug was causing this to fail.  
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>            
    [Test(1, "Xml", "RegressionXPathStackOverFlow")]
    public class RegressionXPathStackOverFlow : WindowTest
    {

        #region Public Members

        public RegressionXPathStackOverFlow()            
        {
            InitializeSteps += new TestStep(RunTest);            
        }

        public TestResult RunTest()
        {
            Status("Setup");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(@"<XTC><TEST></TEST></XTC>");
            XmlDataProvider xmlDataProvider = new XmlDataProvider();
            xmlDataProvider.IsAsynchronous = false;
            xmlDataProvider.Document = xmlDoc;

            Grid parentGrid = new Grid();

            Binding parentBinding = new Binding();
            parentBinding.Mode = BindingMode.OneWay;
            parentBinding.Source = xmlDataProvider.Data;
            parentBinding.XPath = "XTC";

            parentGrid.SetBinding(FrameworkElement.DataContextProperty, parentBinding);

            Binding childBinding = new Binding();
            childBinding.XPath = "TEST";

            Grid childGrid = new Grid();
            childGrid.SetBinding(FrameworkElement.DataContextProperty, childBinding);

            // Complete the tree.
            parentGrid.Children.Add(childGrid);
            Window.Content = parentGrid;
            
            // Wait for stackoverflow if ex[ected.
            WaitForPriority(DispatcherPriority.SystemIdle);

            // Check if the datacontext property is set correctly - the bug was causing this to fail.
            if (childGrid.DataContext == null)
            {
                LogComment("Child grids DataContext was not set correctly.");
                return TestResult.Fail;
            }
            
            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        #endregion
    }
}
