// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// This tests the scenario where there's a two way bind between the SelectedItem and a datasource.
    /// If the value in the source is not a valid SelectedItem, SelectedItem becomes null but the source
    /// does not change.
	/// </description>
    /// </summary>
    [Test(3, "Controls", "BindToSelectedItem")]
	public class BindToSelectedItem : XamlTest
	{
		private ListBox _lb;
        private SelectedItemsVerifier _siv;
        private ObservableCollection<Place> _places;
        private SelectedItemSource _selectedItemSource;

        public BindToSelectedItem()
            : base(@"BindToSelectedItem.xaml")
        {
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(BindBadSource);
            RunSteps += new TestStep(ChangeToGoodSource);
            RunSteps += new TestStep(ChangeSelection);
        }

        private TestResult Setup()
		{
			Status("Setup");

			_lb = Util.FindElement(RootElement, "lb") as ListBox;
			if (_lb == null)
			{
				LogComment("Fail - Unable to reference lb element (ListBox)");
				return TestResult.Fail;
			}

            _places = RootElement.Resources["places"] as ObservableCollection<Place>;
            if (_places == null)
            {
                LogComment("Fail - Unable to reference the source Places");
                return TestResult.Fail;
            }

            _siv = new SelectedItemsVerifier(_lb);
            _selectedItemSource = new SelectedItemSource();

            LogComment("Setup was successful");
			return TestResult.Pass;
		}

        private TestResult BindBadSource()
        {
            Status("BindBadSource");

            _lb.SelectedItem = _places[3];

            _selectedItemSource.PlaceSource = new Place("Snohomish", "WA");

            Binding b = new Binding();
            b.Source = _selectedItemSource;
            b.Mode = BindingMode.TwoWay;
            b.Path = new PropertyPath("PlaceSource");
            _lb.SetBinding(ListBox.SelectedItemProperty, b);

            // Source: its a two-way bind and target is null but source did not change
            if (_selectedItemSource.PlaceSource == null)
            {
                LogComment("Fail - Source is null");
                return TestResult.Fail;
            }
            if (((Place)(_selectedItemSource.PlaceSource)).Name != "Snohomish" ||
                ((Place)(_selectedItemSource.PlaceSource)).State != "WA")
            {
                LogComment("Fail - Source not as expected");
                return TestResult.Fail;
            }

            // Target: the source has a bad value for SelectedItem so nothing is selected
            VerifyResult result = _siv.Verify(_places[3]) as VerifyResult;
            LogComment(result.Message);
            return (result.Result);
		}

        private TestResult ChangeToGoodSource()
        {
            Status("ChangeToGoodSource");

            _selectedItemSource.PlaceSource = _places[4] as Place;

            // Target: The place in index 4 (Portland) is now selected
            VerifyResult result = _siv.Verify(_places[4]) as VerifyResult;
            LogComment(result.Message);
            return (result.Result);
        }

        private TestResult ChangeSelection()
        {
            Status("ChangeSelection");

            _lb.SelectedItem = _places[3];

            // Source: The place in index 3 (Kirkland) should now be in the source
            if (_selectedItemSource.PlaceSource != _places[3])
            {
                LogComment("Fail - DataSource did not change according to selection change in the UI");
                return TestResult.Fail;
            }

            LogComment("ChangeSelection was successful");
            return TestResult.Pass;
        }
    }
}

