// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Threading; 
using System.Windows.Threading;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using System.Windows.Data;
using System.Xml;
using System.Collections.ObjectModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// A tab control items control is populated with a bound collection that has XML as
    /// the data source.
    /// This test case uses an ItemContainerStyle to set the data to the header and content and 
    /// data templates them. It then verifies that the data is correct.
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(2, "Controls", "XMLTabControl")]
    public class XMLTabControl : XamlTest
    {
        TabControl _myTabControl;
        XmlDataProvider _xmlProvider;
        XmlNodeList _magazines;

        public XMLTabControl() : base(@"XMLTabControl.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestElementCount);
            RunSteps += new TestStep(TestDataContent);
            RunSteps += new TestStep(TestHeader);
            RunSteps += new TestStep(TestContent);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.SystemIdle);
            Util.WaitForXmlDataProviderReady("xds", RootElement, 30);
            _myTabControl = Util.FindElement(RootElement, "myTabControl") as TabControl;
            _xmlProvider = (XmlDataProvider)(this.RootElement.Resources["xds"]);
            ReadOnlyObservableCollection<XmlNode> rootElements = (ReadOnlyObservableCollection<XmlNode>)(_xmlProvider.Data);
            _magazines = rootElements[0].ChildNodes;

            return TestResult.Pass;
        }

        private TestResult TestElementCount()
        {
            Status("TestElementCount");
            WaitForPriority(DispatcherPriority.Background);

            int expectedCount = 4;
            int actualCount = _myTabControl.Items.Count;
            if (expectedCount != actualCount)
            {
                LogComment("Fail - Expected count of items:" + expectedCount + ". Actual:" + actualCount);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult TestDataContent()
        {
            Status("TestDataContent");

            for (int i = 0; i < _myTabControl.Items.Count; i++)
            {
                if (_myTabControl.Items[i] != _magazines[i])
                {
                    LogComment("Fail - Expected element in position " + i + ": " + _magazines[i].InnerText +
                        ". Actual: " + _myTabControl.Items[i]);
                    return TestResult.Fail;
                }
            }

            return TestResult.Pass;
        }

        private TestResult TestHeader()
        {
            Status("TestHeader");

            for (int i = 0; i < _myTabControl.Items.Count; i++)
            {
                TabItem tabItem = _myTabControl.ItemContainerGenerator.ContainerFromIndex(i) as TabItem;
                // Both "tabItem.Header" and "magazines[i].ChildNodes[0]" are XmlElements (the Title element)
                if (tabItem.Header != _magazines[i].ChildNodes[0])
                {
                    LogComment("Fail - Expected header in position " + i + ": " + _magazines[i].ChildNodes[0].InnerText + 
                        ". Actual: " + tabItem.Header);
                    return TestResult.Fail;
                }
            }

            return TestResult.Pass;
        }

        private TestResult TestContent()
        {
            Status("TestContent");

            for (int i = 0; i < _myTabControl.Items.Count; i++)
            {
                TabItem tabItem = _myTabControl.ItemContainerGenerator.ContainerFromIndex(i) as TabItem;
                if (tabItem.Content != _magazines[i].Attributes[0])
                {
                    LogComment("Fail - Expected content in position " + i + ": " + _magazines[i].Attributes[0].InnerText +
                        ". Actual: " + tabItem.Content);
                    return TestResult.Fail;
                }
            }

            return TestResult.Pass;
        }
    }
}
