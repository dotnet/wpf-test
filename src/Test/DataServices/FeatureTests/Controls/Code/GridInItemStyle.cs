// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Threading; using System.Windows.Threading;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using System;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Scenario: ItemTemplate of ItemsControl contains a Grid with a row and as many
	/// columns as properties in the data source object.
	/// </description>
	/// </summary>
    [Test(3, "Controls", "GridInItemStyle")]
	public class GridInItemStyle : XamlTest
	{
		ListBox _lb;
//		ObservableCollection aldc;
        ObservableCollection<MyStar> _aldc;

        public GridInItemStyle(): base(@"GridInItemStyle.xaml")
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(VerifyItems);
		}

        private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);

            _aldc = new ObservableCollection<MyStar>();
            _aldc.Add(new MyStar("Mike", "Piazza", 24));
            _aldc.Add(new MyStar("Mark", "McGwire", 26));
            _aldc.Add(new MyStar("Jay", "Bell", 30));
            _aldc.Add(new MyStar("Matt", "Williams", 21));
            _aldc.Add(new MyStar("Barry", "Larkin", 29));
            _aldc.Add(new MyStar("Sammy", "Sosa", 26));
            _aldc.Add(new MyStar("Larry", "Walker", 28));
            _aldc.Add(new MyStar("Tony", "Gwynn", 32));


            _lb = Util.FindElement(RootElement, "lb") as ListBox;
			if (_lb == null)
			{
				LogComment("Fail - Unable to reference lb element (ListBox)");
				return TestResult.Fail;
			}

            _lb.ItemsSource = _aldc;
            WaitForPriority(DispatcherPriority.Render);

            LogComment("Setup was successful");
			return TestResult.Pass;
		}

        private TestResult VerifyItems()
		{
			Status("VerifyItems");
			WaitForPriority(DispatcherPriority.Render);

			StarVerifier sv = new StarVerifier(_lb);
			IVerifyResult res = sv.Verify(_aldc);
			LogComment(res.Message);
			return res.Result;
		}
	}
}
