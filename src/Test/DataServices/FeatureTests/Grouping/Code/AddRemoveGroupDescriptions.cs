// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test;
using System.ComponentModel;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Initially this scenario contains one GroupDescription with PropertyName set.
    /// This tests adding a GroupDescription with a Converter, removing it, and clearing all GroupDescriptions.
    /// Data sources used: CLR, ADO.NET and XML.
    /// </description>
    /// </summary>
    [Test(1, "Grouping", "AddRemoveGroupDescriptions")]
    public class AddRemoveGroupDescriptions : XamlTest
    {
        private GroupingVerifier _groupingVerifier;
        private Places _places;
        private PlacesDataTable _placesDataTable;
        private ReadOnlyObservableCollection<XmlNode> _xmlPlacesCollection;
        private CollectionViewSource _cvs1;
        private CollectionViewSource _cvs2;
        private CollectionViewSource _cvs3;

        public AddRemoveGroupDescriptions()
            : base(@"AddRemoveGroupDescriptions.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps +=new TestStep(AddGroupDescription);
            RunSteps += new TestStep(RemoveGroupDescription);
            RunSteps += new TestStep(ClearGroupDescriptions);
        }

        // Verifies initial structure of Groups
        TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.SystemIdle);

            _groupingVerifier = new GroupingVerifier();
            Page page = (Page)(this.Window.Content);
            _places = (Places)(page.Resources["places"]);
            _placesDataTable = (PlacesDataTable)(page.Resources["placesDataTable"]);
            XmlDataProvider xmlPlaces = (XmlDataProvider)(page.Resources["xmlPlaces"]);
            _cvs1 = (CollectionViewSource)(page.Resources["cvs1"]);
            _cvs2 = (CollectionViewSource)(page.Resources["cvs2"]);
            _cvs3 = (CollectionViewSource)(page.Resources["cvs3"]);
            _xmlPlacesCollection = (ReadOnlyObservableCollection<XmlNode>)DataSourceHelper.WaitForData(xmlPlaces);

            if (!VerifyGroupsOneGD(_places, _cvs1)) { return TestResult.Fail; }
            if (!VerifyGroupsOneGD(_placesDataTable.DefaultView, _cvs2)) { return TestResult.Fail; }
            if (!VerifyGroupsOneGD(_xmlPlacesCollection, _cvs3)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        // Adds a GroupDescription that groups Names by first letter
        TestResult AddGroupDescription()
        {
            Status("AddGroupDescription");

            PropertyGroupDescription gd = new PropertyGroupDescription("Name");
            gd.Converter = new StringFirstLetterConverter();
            _cvs1.GroupDescriptions.Add(gd);
            _cvs2.GroupDescriptions.Add(gd);
            _cvs3.GroupDescriptions.Add(gd);

            if (!VerifyGroupsTwoGD(_places, _cvs1)) { return TestResult.Fail; }
            if (!VerifyGroupsTwoGD(_placesDataTable.DefaultView, _cvs2)) { return TestResult.Fail; }
            if (!VerifyGroupsTwoGD(_xmlPlacesCollection, _cvs3)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        // Removes the second GroupDescription added
        TestResult RemoveGroupDescription()
        {
            Status("RemoveGroupDescription");

            _cvs1.GroupDescriptions.RemoveAt(1);
            _cvs2.GroupDescriptions.RemoveAt(1);
            _cvs3.GroupDescriptions.RemoveAt(1);

            if (!VerifyGroupsOneGD(_places, _cvs1)) { return TestResult.Fail; }
            if (!VerifyGroupsOneGD(_placesDataTable.DefaultView, _cvs2)) { return TestResult.Fail; }
            if (!VerifyGroupsOneGD(_xmlPlacesCollection, _cvs3)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        // Clears all GroupDescriptions
        TestResult ClearGroupDescriptions()
        {
            Status("ClearGroupDescriptions");

            _cvs1.GroupDescriptions.Clear();
            _cvs2.GroupDescriptions.Clear();
            _cvs3.GroupDescriptions.Clear();

            if (!VerifyGroupsNoGD(_cvs1)) { return TestResult.Fail; }
            if (!VerifyGroupsNoGD(_cvs2)) { return TestResult.Fail; }
            if (!VerifyGroupsNoGD(_cvs3)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        #region HelperMethods
        // Verifies that there are no GroupDescriptions in this CVS
        private bool VerifyGroupsNoGD(CollectionViewSource cvs)
        {
            VerifyResult groupingResult = (VerifyResult)(_groupingVerifier.Verify(null, cvs.View.Groups));
            if (groupingResult.Result != TestResult.Pass)
            {
                LogComment(groupingResult.Message);
                return false;
            }

            return true;
        }

        // Verifies the groups when there is one GroupDescription with PropertyName=State
        private bool VerifyGroupsOneGD(IList list, CollectionViewSource cvs)
        {
            ExpectedGroup group0 = new ExpectedGroup("WA", new object[] { list[0], list[1], list[2], list[3], list[10] });
            ExpectedGroup group1 = new ExpectedGroup("OR", new object[] { list[4] });
            ExpectedGroup group2 = new ExpectedGroup("CA", new object[] { list[5], list[6], list[7], list[8], list[9] });

            ExpectedGroup[] expectedGroups = new ExpectedGroup[] { group0, group1, group2 };

            VerifyResult groupingResult = (VerifyResult)(_groupingVerifier.Verify(expectedGroups, cvs.View.Groups));
            if (groupingResult.Result != TestResult.Pass)
            {
                LogComment(groupingResult.Message);
                return false;
            }

            return true;
        }

        // Verifies the groups when there are two GroupDescriptions:
        // - One with PropertyName=State
        // - Another one that groups the Name property by first letter
        private bool VerifyGroupsTwoGD(IList list, CollectionViewSource cvs)
        {
            ExpectedGroup group00 = new ExpectedGroup("S", new object[] { list[0] });
            ExpectedGroup group01 = new ExpectedGroup("R", new object[] { list[1] });
            ExpectedGroup group02 = new ExpectedGroup("B", new object[] { list[2], list[10] });
            ExpectedGroup group03 = new ExpectedGroup("K", new object[] { list[3] });

            ExpectedGroup group10 = new ExpectedGroup("P", new object[] { list[4] });

            ExpectedGroup group20 = new ExpectedGroup("S", new object[] { list[5], list[7], list[8], list[9] });
            ExpectedGroup group21 = new ExpectedGroup("L", new object[] { list[6] });

            ExpectedGroup group0 = new ExpectedGroup("WA", new object[] { group00, group01, group02, group03 });
            ExpectedGroup group1 = new ExpectedGroup("OR", new object[] { group10 });
            ExpectedGroup group2 = new ExpectedGroup("CA", new object[] { group20, group21 });

            ExpectedGroup[] expectedGroups = new ExpectedGroup[] { group0, group1, group2 };

            VerifyResult groupingResult = (VerifyResult)(_groupingVerifier.Verify(expectedGroups, cvs.View.Groups));
            if (groupingResult.Result != TestResult.Pass)
            {
                LogComment(groupingResult.Message);
                return false;
            }

            return true;
        }

        #endregion
    }
}




