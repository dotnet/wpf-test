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
	/// TODO
	/// </description>
	/// </summary>
    [Test(1, "DataSources", SupportFiles=@"FeatureTests\DataServices\Magazine.xml")]
    public class XmlDataSrcTest : XamlTest
    {
        private string _xamlFileName;

        [Variation("xmldatasrc1.xaml")]
        [Variation("XmlDataSrc.xaml")]
        public XmlDataSrcTest(string filename)
            : base(filename)
        {
            _xamlFileName = filename;
            RunSteps += new TestStep (Test);
        }

        TestResult Test ()
        {
            //Waiting for the databinding call to complete
            WaitForPriority (DispatcherPriority.Background);
            WaitFor(2000);
            DockPanel dp = new DockPanel ();
            TextBlock st = ((DockPanel)RootElement).Children[0] as TextBlock;

            if (st == null)
            {
                LogComment ("Could not find the bound text element");
                return TestResult.Fail;
            }

            if (st.Text != "Popular Science")
            {
                LogComment ("Bound value was not the expected value. Expected: \'Popular Science\' Actual:\'" + st.Text + "\'");
                return TestResult.Fail;
            }

            LogComment ("Text had the expected value for " + _xamlFileName);

            return TestResult.Pass;
        }
    }
}

