// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression coverage for bug where property changed notifications arent raised on changing the source for XmlDataProvider.
    /// Test must be Full Trust due to setting DataProvider.Source to a non-site of origin Uri
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "PropertyChangeNotificationXmlDataProvider", SecurityLevel=TestCaseSecurityLevel.FullTrust)]
    public class PropertyChangeNotificationXmlDataProvider : XamlTest
    {
        #region Private Data

        private XmlDataProvider _dataProvider;
        private TextBlock _myTextBlock;
        private string _textBlockSource;

        #endregion

        #region Constructors

        public PropertyChangeNotificationXmlDataProvider()
            : base(@"PropertyChangeNotificationXmlDataProvider.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            WaitForPriority(DispatcherPriority.Background);
            
            _dataProvider = (XmlDataProvider)RootElement.FindResource("xdp");
            _myTextBlock = (TextBlock)RootElement.FindName("myTextBlock");

            if (_dataProvider == null || _myTextBlock == null)
            {
                LogComment("XAML element not found.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult Validate()
        {   
            // Change the source for the XmlDataProvider
            _dataProvider.Source = new Uri("http://foo.bar/");

            // Wait for changes to complete
            WaitForPriority(DispatcherPriority.DataBind);

            // Check Value            
            _textBlockSource = _myTextBlock.Text;

            if (_textBlockSource.CompareTo(_dataProvider.Source.ToString()) != 0)
            {
                LogComment("Source was not updated.");
                LogComment("Expected: " + _dataProvider.Source.ToString());
                LogComment("Actual: " + _myTextBlock.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
    }
}