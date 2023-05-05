// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests sorting an ObservableCollection in vietnamese, slovik and in the invariant culture.
    /// </description>
    /// </summary>
    // [DISABLED_WHILE_PORTING]
    // [Test(3, "Views", "IntlSortTest")]
    public class IntlSortTest: AvalonTest
    {
        ListCollectionView _lcv;
        CollectionViewOrderVerifier _cvov;
        public IntlSortTest()
        {
            InitializeSteps +=new TestStep(Init);
            RunSteps += new TestStep(SortVietnamese);
            RunSteps += new TestStep(SortSlovik);
            RunSteps += new TestStep(SortInvariantCulture);
        }

        private TestResult Init()
        {
	    SortDataItems sdi = new SortDataItems();
            _lcv = CollectionViewSource.GetDefaultView(sdi) as ListCollectionView;
            _cvov = new CollectionViewOrderVerifier(_lcv);
            return TestResult.Pass;
        }
        private TestResult SortVietnamese()
        {
            Status("Sorting Vietnamese");
            _lcv.Culture = new CultureInfo("vi-VN");
            //
            _lcv.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));

            IVerifyResult ivr = _cvov.Verify(5, 4, 1, 2, 3, 0);
            if (ivr.Result != TestResult.Pass)
            {
                LogComment(ivr.Message);
                return ivr.Result;
            }
            LogComment("SortVietnamese was successful");
            return TestResult.Pass;
        }
        private TestResult SortSlovik()
        {
            Status("Sorting Slovik");
            _lcv.Culture = new CultureInfo("sk-SK");
            using(_lcv.DeferRefresh())
            {
                _lcv.SortDescriptions.Clear();
                _lcv.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
            }

            IVerifyResult ivr = _cvov.Verify(5, 1, 4, 2, 3, 0);
            if (ivr.Result != TestResult.Pass)
            {
                LogComment(ivr.Message);
                return ivr.Result;
            }
            LogComment("SortSlovik was successful");
            return TestResult.Pass;
        }
        private TestResult SortInvariantCulture()
        {
            Status("Sorting InvariantCulture");
            _lcv.Culture = CultureInfo.InvariantCulture;
            using(_lcv.DeferRefresh())
            {
                _lcv.SortDescriptions.Clear();
                _lcv.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
            }

            IVerifyResult ivr = _cvov.Verify(5, 4, 2, 3, 1, 0);
            if (ivr.Result != TestResult.Pass)
            {
                LogComment(ivr.Message);
                return ivr.Result;
            }
            LogComment("SortInvariantCulture was successful");
            return TestResult.Pass;
        }

        //This code is just to get current sorted order and isn't used in a test
//      private TestResult verify()
//      {
//          IList il = lcv.Collection as IList;
//          foreach (object currentItem in lcv)
//          {
//              GlobalLog.Status("Array  " + il.IndexOf(currentItem));
//          }
//
//          return TestResult.Pass;
//      }
    }
}


