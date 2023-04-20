// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Threading; 
using System.Windows.Threading;
using Microsoft.Test;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Test style selection with Xml data with Namespaces
	/// </description>
	/// </summary>
    [Test(1, "DataSources", TestCaseSecurityLevel.FullTrust, "XmlStyleNsTest")]
    public class XmlStyleNsTest : XamlTest
    {

        [Variation("XmlStyleNS.xaml")]
        [Variation("XmlStyleNS_Inherit.xaml")]
        public XmlStyleNsTest(string xamlfile)
            : base(xamlfile)
        {
            RunSteps += new TestStep(Verify);
        }

        TestResult Verify ()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Referencing testList element");
            ListBox testList = (ListBox)Util.FindElement(RootElement, "testList");

            if (testList == null)
            {
                LogComment("Unable to reference testList element.");
                return TestResult.Fail;
            }
            
            FrameworkElement[] items = Util.FindElements(testList, "listitem");

            // Needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && items.Length == 0)
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");                
                Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
                items = Util.FindElements(testList, "listitem");
            }

            if (items.Length < 8)
            {
                LogComment("Expected 8 list items, found " + items.Length);
                return TestResult.Fail;
            }

            string[] aList = new string[] { "Hockey Digest", "Car n Track", "Organic Gardening", "Popular Science" };
            string[] bList = new string[] { "Popular Mechanics", "XBox Magazine", "Cosumer Reports", "Newsweek" };
            int i = 0;
            TextBlock text;
            bool pass = true;

            for (i = 0; i < items.Length; i++)
            {
                text = items[i] as TextBlock;
                if (text == null)
                {
                    LogComment("Could not cast item[" + i + "] to a TextBlock");
                    pass = false;
                }
                else
                {
                    if (i < 4)
                    {
                        //a
                        if (text.Text != aList[i] || text.Tag.ToString() == "A")
                        {
                            LogComment("Item [" + i + "] expected Tag=A, Text=" + aList[i] + " actual Tag=" + text.Tag.ToString() + ", Text=" + text.Text);
                            pass = false;
                        }
                    }
                    else
                    {
                        //b
                        if (text.Text != bList[i - 4] || text.Tag.ToString() == "B")
                        {
                            LogComment("Item [" + i + "] expected Tag=B, Text=" + bList[i - 4] + " actual Tag=" + text.Tag.ToString() + ", Text=" + text.Text);
                            pass = false;
                        }
                    }
                }
            }

            if (pass)
                return TestResult.Pass;
            else
                return TestResult.Fail;
        }
    }
}

