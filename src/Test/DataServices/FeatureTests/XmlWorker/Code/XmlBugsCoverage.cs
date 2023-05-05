// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
	/// <area>Xml</area>

	/// <priority>2</priority>
    /// <description>
    /// Provides coverage for Xml bugs.
    /// - Verifies that XmlNamespaceMappingCollection will not throw an exception when the enumerator 
    /// is invoked on a collection with non-default mappings
	/// </description>
    /// <relatedBugs>


    /// </relatedBugs>
	/// </summary>
    [Test(2, "Xml", TestCaseSecurityLevel.PartialTrust,"XmlBugsCoverage")]
    public class XmlBugsCoverage : XamlTest
    {
        public XmlBugsCoverage()
            : base(@"XmlBugsCoverage.xaml")
        {
            RunSteps += new TestStep(MappingCollectionException);
            RunSteps += new TestStep(XmlCurrentChangingCommit);
        }

        private TestResult MappingCollectionException()
        {
            Status("MappingCollectionException");

            XmlNamespaceMappingCollection collection = new XmlNamespaceMappingCollection();
            XmlNamespaceMapping mapping = new XmlNamespaceMapping("dpi", new Uri("clr-namespace:UntitleProject1"));
            collection.Add(mapping);
            int count = collection.Count; // this used to throw

            return TestResult.Pass;
        }

        TestResult XmlCurrentChangingCommit()
        {
            // Make sure XML bindings have time to resolve
            WaitForPriority(DispatcherPriority.SystemIdle);

            // Grab the TextBox and change it's text, then change selection on the ListBox.
            // This is supposed to cause the dirty value to be committed, but it wasn't for XML.
            ListBox lb = (ListBox)RootElement.FindName("xTenantList");
            lb.IsSynchronizedWithCurrentItem = true;
            ContentControl cc = (ContentControl)RootElement.FindName("xTenantDetails");
            TextBox tb = (TextBox)Util.FindVisualByType(typeof(TextBox), cc);
            tb.Text = "Foo";
            lb.SelectedIndex = 1;

            WaitForPriority(DispatcherPriority.SystemIdle);

            // Needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && ((lb.Items.Count == 0) || (lb.Items.IsEmpty)))
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
            }

            if (((XmlElement)lb.Items[0]).InnerText != "Foo") return TestResult.Fail;

            return TestResult.Pass;
        }
    }
}
