// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Data;
using System.ComponentModel;
using System.Globalization;
using System.Data;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression coverage for bug where Collection Order is not preserved with Grouping
    /// </description>

    /// </summary>
    [Test(1, "Regressions.Part1", "GroupingCollectionOrder")]
    public class GroupingCollectionOrder : AvalonTest
    {
        #region Constructors

        public GroupingCollectionOrder()
        {
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Validate()
        {
            PlacesDataTable pdt = new PlacesDataTable();
            DataView dv = pdt.DefaultView;
            BindingListCollectionView blcv = (BindingListCollectionView)CollectionViewSource.GetDefaultView(dv);
            blcv.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtBeginning;

            dv.Sort ="Name ASC";            
            blcv.GroupDescriptions.Add(new PropertyGroupDescription(null, new ReproGrouper()));
           

            blcv.AddNew();
            ((DataRowView)blcv.CurrentAddItem)["Name"] = "Omaha";
            ((DataRowView)blcv.CurrentAddItem)["State"] = "NE";
            blcv.CommitNew();

            CollectionViewGroup ContainsOGroup = (CollectionViewGroup)(blcv.Groups[3]);
            DataRowView dataRowView1 = (DataRowView)ContainsOGroup.Items[5];
            DataRowView dataRowView2 = (DataRowView)ContainsOGroup.Items[6];

            string actualName = (string)dataRowView1["Name"];
            string expectedName = (string)dataRowView2["Name"];

            if (actualName.CompareTo(expectedName) >= 0)
            {
                LogComment("Sort was by name ascending but " + actualName + " should be after " + expectedName);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion

    }

    #region Helper Classes

    public class ReproGrouper : IValueConverter
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
