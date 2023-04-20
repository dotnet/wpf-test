// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    /// Regression Test
    /// 1. Create a collection
    /// 2. Select an item on the collection
    /// 3. Move the item.
    /// 4. Verify the the item is the selected one and not the original index.
    /// </description>

    /// </summary>
    [Test(1, "Views", "RegressionCollectionViewCurrencyMove")]
    public class RegressionCollectionViewCurrencyMove : XamlTest
    {
        #region Private Data

        private CountryList _list;
        private ListBox _listBox;

        #endregion


        #region Public Members

        public RegressionCollectionViewCurrencyMove()
            : base(@"Markup\RegressionCollectionViewCurrencyMove.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
        }

        public TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);

            _list = RootElement.FindResource("countryList") as CountryList;
            if (_list == null)
            {
                LogComment("Unable to reference countryList.");
                return TestResult.Fail;
            }

            _listBox = Util.FindElement(RootElement, "listBox") as ListBox;
            if (_listBox == null)
            {
                LogComment("Unable to reference listBox.");
                return TestResult.Fail;
            }
            
            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        // This method tests if the selection is being correctly updated
        public TestResult RunTest()
        {
            Status("Running Test...");

            // Select Item at index 0.
            _listBox.SelectedItem = _listBox.Items[0];

            // Move the Item.
            _list.Move(0, 5);

            // Verify that selected is item is index 5 now.
            if (_listBox.SelectedItem != _listBox.Items[5])
            {
                LogComment("Moved Item is no longer selected Item!");
                return TestResult.Fail;
            }

            

            LogComment("Add and Remove Items passed.");
            return TestResult.Pass;
        }

        #endregion
    }


    #region Custom collection required for test - kept public so Markup can access it.

    // Collection to be used for manipulation.
    public class CountryList : ObservableCollection<String>
    {
        public CountryList()
        {
            Add("England");
            Add("France");
            Add("Germany");
            Add("Portugal");
            Add("Italy");
            Add("Spain");
        }
    }

    #endregion
}

