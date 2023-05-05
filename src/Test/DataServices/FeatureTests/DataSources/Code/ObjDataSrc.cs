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
using System.Windows.Navigation;
using System.Windows.Media;
using System.ComponentModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;


namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// This test binds a data source to the Text of a TextBlock element and verifies 
	/// the Text's content is populated with the correct data.
	/// </description>
	/// </summary>
    [Test(0, "DataSources", "objDataSrcTest")]
    public class objDataSrcTest : XamlTest
    {
        [Variation("ObjDataSrc.xaml")]
        public objDataSrcTest(string file)
            : base(file)
        {
            RunSteps += new TestStep (Test);
        }

        TestResult Test ()
        {
            //Waiting for the databinding call to complete
            WaitForPriority (DispatcherPriority.Background);

            Status("Datermine if the data provider has Data");
            ObjectDataProvider dso = RootElement.FindResource("DSO") as ObjectDataProvider;
            if (dso == null)
            {
                LogComment("Could not find ObjectDataProvider 'DSO'");
                return TestResult.Fail;
            }

            if (dso.Data == null)
            {
                LogComment("Data is null");
                return TestResult.Fail;
            }

            TextBlock st = (TextBlock)Util.FindElement(RootElement, "SnapMe");
            
            if (st == null)
            {
                LogComment ("Could not find the bound text element");
                return TestResult.Fail;
            }

            if (st.Text != "Record 0")
            {
                LogComment ("Incorrect value for the bound text. Expected: \'Record 0\' Actual: \'" + st.Text + "\'");
                return TestResult.Fail;
            }
            
            LogComment ("Text had the expected value");

            return TestResult.Pass;
        }
    }

}

