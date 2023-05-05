// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.Specialized;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Globalization;
using System.Data;
using System.ComponentModel;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where Currency does not move to new item from IECV.AddNew for a BLCV over a DataView when the view is being grouped
    /// </description>
    /// </summary>
    [Test(1, "Regressions.Part1", "AddNewBLCVCurrency")]
    public class AddNewBLCVCurrency : AvalonTest
    {
        #region Constructors

        public AddNewBLCVCurrency()
            : base()
        {
            InitializeSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members


        private TestResult Validate()
        {
            // Apply Grouping and add new.     
            PlacesDataTable pdt = new PlacesDataTable();
            DataView dv = pdt.DefaultView;
            BindingListCollectionView blcv = (BindingListCollectionView)CollectionViewSource.GetDefaultView(dv);
            blcv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;

            blcv.GroupDescriptions.Add(new PropertyGroupDescription(null, new Grouper()));

            blcv.AddNew();

            if (blcv.CurrentItem != blcv.CurrentAddItem || blcv.CurrentPosition != 1)
            {
                LogComment("Current Item was not updated correctly.");
                return TestResult.Fail;
            }
        
            return TestResult.Pass;
        }

        #endregion

    }

    #region Helper Classes

    public class Grouper : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataRowView drv = (DataRowView)value;

            if (((string)drv["Name"]).Contains("o") || ((string)drv["Name"]).Contains("O"))
            {
                return new string[] { (string)drv["State"], "ContainsO" };
            }
            else
            {
                return (string)drv["State"];
            }
        }

        #region IValueConverter Members


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    #endregion
}
