// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading; using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Xml;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Testing inline namespaced xml data within an XmlDataProvider
    /// </description>
    /// </summary>
    [Test(2, "DataSources", "XmlDataSrcInlineTest")]
    public class XmlDataSrcInlineTest : XamlTest
    {
        private bool _dataChanged = false;

        private XmlDataProvider _dso;

        private TextBlock _testText;

        XmlNamespaceManager _ns;

        [Variation("XmlDsInline.xaml")]
        [Variation("XmlDsInline_Inherit.xaml")]
        public XmlDataSrcInlineTest(string xamlfile)
            : base(xamlfile)
        {
            this.LoadCompleted += new LoadCompletedEventHandler(myLoadCompleted);
            RunSteps += new TestStep(GetDataSource);
            RunSteps += new TestStep(InitialVerify);

            RunSteps += new TestStep(ChangeValueRefresh);
            RunSteps += new TestStep(ChangeVerify);

            RunSteps += new TestStep(PropChangeRefresh);
            RunSteps += new TestStep(RefreshVerify);

        }

        private void myLoadCompleted(object sender, NavigationEventArgs e)
        {
            Status("Find the Data Source");
            _dso = RootElement.Resources["DSO"] as XmlDataProvider;
            Status("Add event handlers");
            _dso.DataChanged += new EventHandler(myDataChangedEventHandler);
            LogComment("Referenced the Data Source correctly, event handlers added");
        }

        private TestResult GetDataSource()
        {
            LogComment("Testing Xml Data Source");
            
            Status("Find the name space manager");
            _ns = RootElement.Resources["NS"] as XmlNamespaceManager;
            Status("Find the TextBlock element");
            _testText = (TextBlock)Util.FindElement(((DockPanel)RootElement), "testText");

            if (_testText == null)
            {
                LogComment("Unable to find TextBlock element");
                return TestResult.Fail;
            }

            LogComment("Referenced the elements");
            return TestResult.Pass;
        }


        private TestResult InitialVerify()
        {
            TestResult result = TestResult.Pass;

            if (_testText.Text == "")
            {
                result = WaitForSignal();

                if (result != TestResult.Pass)
                {
                    LogComment("TIMEOUT while waiting for initial transfer");
                    return TestResult.Fail;
                }
            }

            result = CheckValues(true, "root/a:Magazine[1]", null, null, "Hockey Digest", _ns, "Initial Verify");
            return result;
        }

        private TestResult ChangeValueRefresh()
        {
            _testText.Text = "New Value";
            _dso.Refresh();
            return TestResult.Pass;
        }

        private TestResult ChangeVerify()
        {
            TestResult result = WaitForSignal();
            if (result != TestResult.Pass)
            {
                LogComment("TIMEOUT while waiting for the refresh to occur");
                return TestResult.Fail;
            }
            WaitForPriority(DispatcherPriority.Background);
            result = CheckValues(true, "root/a:Magazine[1]", null, null, "Hockey Digest", _ns, "Change Verify");
            return result;
        }

        private TestResult PropChangeRefresh()
        {
            _dso.XPath = "root/a:Magazine[2]";
            _dso.Refresh();
            return TestResult.Pass;
        }


        private TestResult RefreshVerify()
        {
            TestResult result = WaitForSignal();

            if (result != TestResult.Pass)
            {
                LogComment("TIMEOUT occured while waiting for refresh after changing xpath value");
                return TestResult.Fail;
            }
            WaitForPriority(DispatcherPriority.Background);

            result = CheckValues(true, "root/a:Magazine[2]", null, null, "Car n Track", _ns, "Refresh Verify");

            return result;
        }

        public void myDataChangedEventHandler(object sender, EventArgs args)
        {
            LogComment("DataChanged Fired");
            _dataChanged = true;
            Signal(TestResult.Pass);
        }


        private TestResult CheckValues(bool expDataChanged, string expXPath, string expSource, string expDataString, string expText, XmlNamespaceManager expNs, string stepName)
        {
            // Needed for slower machines (like VMs with low memory)
            WaitForPriority(DispatcherPriority.SystemIdle);
            
            bool pass = true;

            if (_dataChanged != expDataChanged)
            {
                LogComment("FAILED: DataChanged flag value, expected:" + expDataChanged + " actual:" + _dataChanged);
                pass = false;
            }

            if (_dso.XPath != expXPath)
            {
                LogComment("FAILED: dso.XPath value, expected:" + expXPath + " actual:" + _dso.XPath);
                pass = false;
            }

            if (expSource == null)
            {
                if (_dso.Source != null)
                {
                    LogComment("FAILED: dso.Source value, expected:null actual:" + _dso.Source);
                    pass = false;
                }
            }
            else
            {
                if (_dso.Source == null)
                {
                    LogComment("FAILED: dso.Source value, expected:" + expSource + " actual:null");
                    pass = false;
                }
            }

            if (expDataString != null)
            {
                if (_dso.Data.ToString() != expDataString)
                {
                    LogComment("FAILED: dso.Data value, expected:" + expDataString + " actual:" + _dso.Data.ToString());
                    pass = false;
                }
            }

            // Needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && (_testText.Text != expText))
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
            }

            if (_testText.Text != expText)
            {
                LogComment("FAILED: testText.Text value, expected:" + expText + " actual:" + _testText.Text);
                pass = false;
            }

            if (!(Util.CompareObjects(expNs, _dso.XmlNamespaceManager)))
            {
                LogComment("FAILED: dso.XmlNamespaceManager value, expected:" + expNs.ToString() + " actual:" + _dso.XmlNamespaceManager.ToString());
                pass = false;
            }

            if (pass)
            {
                LogComment("Properties are as expected for " + stepName);
                return TestResult.Pass;
            }
            else
            {
                LogComment("Properties were not as expected for " + stepName);
                return TestResult.Fail;
            }
        }
    }
}

