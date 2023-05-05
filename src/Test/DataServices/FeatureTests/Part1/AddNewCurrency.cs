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

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where Currency does not move to new item from IECV.AddNew when the view is being filtered
    /// </description>
    /// </summary>
    [Test(1, "Regressions.Part1", "AddNewCurrency")]
    public class AddNewCurrency : AvalonTest
    {
        #region Constructors

        public AddNewCurrency()
            : base()
        {
            InitializeSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members


        private TestResult Validate()
        {
            // Apply Filter and add new.
            Places places = new Places();
            ListCollectionView listCollectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(places);

            listCollectionView.Filter = new Predicate<object>(delegate(object objectToFilter)
            {
                Place place = (Place)objectToFilter;
                if (place.Name.CompareTo("M") > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            listCollectionView.AddNew();

            // This should be true, but in actuality CurrentItem is null and CurrentPosition is -1.
            if (listCollectionView.CurrentItem != listCollectionView.GetItemAt(listCollectionView.Count - 1) || listCollectionView.CurrentPosition != listCollectionView.Count - 1)
            {
                LogComment("Current Item was not updated correctly.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion

    }    
}
