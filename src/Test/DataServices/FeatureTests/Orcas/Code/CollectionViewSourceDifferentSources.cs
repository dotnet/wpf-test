// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using System.Windows.Controls;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Data;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// This tests using CollectionViewSource with several different source collections.
    /// Scenario 1 - Source is CompositeCollection
    /// Scenario 2 - Set CollectionViewType
    /// Scenario 3 - Source is IEnumerable
    /// Scenario 4 - Source is IList
    /// Scenario 5 - Source is IBindingList
    /// Scenario 6 - Source derives from CollectionView
    /// Scenario 7 - Source implements ICollectionView (but does not derive from CollectionView)
    /// Scenario 8 - Source is XmlDataProvider
    /// Scenario 9 - Source is BindingList<T>
	/// </description>
    /// <relatedTasks>


    /// </relatedTasks>
    /// <relatedBugs>













    /// </relatedBugs>
	/// </summary>
    [Test(1, "Views", "CollectionViewSourceDifferentSourcesOrcas")]
    public class CollectionViewSourceDifferentSourcesOrcas : XamlTest
    {
        CollectionViewSource _cvs1;
        CollectionViewSource _cvs3;
        CollectionViewSource _cvs4;
        CollectionViewSource _cvs5;
        CollectionViewSource _cvs6;
        CollectionViewSource _cvs7;
        CollectionViewSource _cvs8;
        CollectionViewSource _cvs9;
        GroupingVerifier _groupingVerifier;

        public CollectionViewSourceDifferentSourcesOrcas()
            : base(@"Markup\CollectionViewSourceDifferentSources.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            // Scenario 1 - Source is CompositeCollection
            RunSteps += new TestStep(SourceIsCompositeCollection); // 
            RunSteps += new TestStep(FilterCompositeCollection);
            RunSteps += new TestStep(SortCompositeCollection);
            RunSteps += new TestStep(GroupByCompositeCollection);
            // Scenario 2 - Set CollectionViewType
            RunSteps += new TestStep(WrongCollectionType);
            RunSteps += new TestStep(TypeIsNotCollection);
            RunSteps += new TestStep(DefaultType);
            RunSteps += new TestStep(CustomCollection);
            RunSteps += new TestStep(CustomCollectionImplementsICollectionView);
            // Scenario 3 - Source is IEnumerable
            RunSteps += new TestStep(SourceIsIEnumerable);
            RunSteps += new TestStep(FilterIEnumerable);
            RunSteps += new TestStep(SortIEnumerable);
            RunSteps += new TestStep(GroupByIEnumerable);
            // Scenario 4 - Source is IList
            RunSteps += new TestStep(SourceIsIList);
            RunSteps += new TestStep(FilterIList);
            RunSteps += new TestStep(SortIList);
            RunSteps += new TestStep(GroupByIList);
            // Scenario 5 - Source is IBindingList
            RunSteps += new TestStep(SourceIsIBindingList);
            RunSteps += new TestStep(CustomFilterIsIBindingList);
            RunSteps += new TestStep(SortIsIBindingList);
            RunSteps += new TestStep(GroupByIBindingList);
            RunSteps += new TestStep(FilterIsIBindingList);
            // Scenario 6 - Source derives from CollectionView
            RunSteps += new TestStep(SourceIsCollectionView);
            // Scenario 7 - Source implements ICollectionView (but does not derive from CollectionView)
            RunSteps += new TestStep(SourceIsICollectionView);
            // Scenario 8 - Source is XmlDataProvider
            RunSteps += new TestStep(SourceIsXmlDataProvider);
            RunSteps += new TestStep(FilterIsXmlDataProvider);
            RunSteps += new TestStep(SortIsXmlDataProvider);
            RunSteps += new TestStep(GroupByXmlDataProvider);
            RunSteps += new TestStep(GroupNoPropertyNameIsXmlDataProvider);
            RunSteps += new TestStep(SortNoPropertyNameIsXmlDataProvider);
            // Scenario 9 - Source is BindingList<T>
            RunSteps += new TestStep(SourceIsBindingListT);
            RunSteps += new TestStep(CustomFilterIsBindingListT);
            RunSteps += new TestStep(SortIsBindingListT);
            RunSteps += new TestStep(GroupByBindingListT);
            RunSteps += new TestStep(FilterIsBindingListT);
        }

        TestResult Setup()
        {
            Status("Setup");
            _groupingVerifier = new GroupingVerifier();
            WaitForPriority(DispatcherPriority.SystemIdle);
            return TestResult.Pass;
        }

        #region Scenario 1 - Source is CompositeCollection
        TestResult SourceIsCompositeCollection()
        {
            Status("SourceIsCompositeCollection");
            StackPanel sp1 = (StackPanel)(LogicalTreeHelper.FindLogicalNode(RootElement, "sp1"));
            _cvs1 = (CollectionViewSource)(sp1.Resources["cvs1"]);
            CollectionViewSource cvs11 = (CollectionViewSource)(sp1.Resources["cvs11"]);
            ListBox lb1 = (ListBox)(sp1.Children[0]);
            ListBox lb11 = (ListBox)(sp1.Children[1]);

            CollectionView icv = (CollectionView)(_cvs1.View);
            
            if (!CheckCanProperties(icv, false, false, false)) { return TestResult.Fail; }

            
            if (!CheckCurrency(_cvs1, cvs11, lb1, lb11)) { return TestResult.Fail; }

            SetBindingInListBox(lb1, _cvs1);

            return TestResult.Pass;
        }

        TestResult FilterCompositeCollection()
        {
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            _cvs1.Filter += new FilterEventHandler(MyDoNothingFilter);
            return TestResult.Pass;
        }

        TestResult SortCompositeCollection()
        {
            
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            _cvs1.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            return TestResult.Pass;
        }

        TestResult GroupByCompositeCollection()
        {
            
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            PropertyGroupDescription pgd = new PropertyGroupDescription("Name");
            _cvs1.GroupDescriptions.Add(pgd);
            return TestResult.Pass;
        }

        void MyDoNothingFilter(object item, FilterEventArgs args)
        {
            args.Accepted = true;
        }

        #endregion

        #region Scenario 2 - WrongCollectionType
        TestResult WrongCollectionType()
        {
            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            GreekGods greekGods = new GreekGods();
            CollectionViewSource cvs = new CollectionViewSource();
            ((ISupportInitialize)cvs).BeginInit();
            cvs.CollectionViewType = typeof(BindingListCollectionView);
            cvs.Source = greekGods;
            ((ISupportInitialize)cvs).EndInit();

            return TestResult.Fail;
        }

        TestResult TypeIsNotCollection()
        {
            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            GreekGods greekGods = new GreekGods();
            CollectionViewSource cvs = new CollectionViewSource();
            ((ISupportInitialize)cvs).BeginInit();
            cvs.CollectionViewType = typeof(Button);
            cvs.Source = greekGods;
            ((ISupportInitialize)cvs).EndInit();
            return TestResult.Fail;
        }

        
        TestResult DefaultType()
        {
            GreekGods greekGods = new GreekGods();
            CollectionViewSource cvs = new CollectionViewSource();
            ((ISupportInitialize)cvs).BeginInit();
            cvs.CollectionViewType = typeof(ListCollectionView);
            cvs.Source = greekGods;
            ((ISupportInitialize)cvs).EndInit();
            if (cvs.View.GetType() != typeof(ListCollectionView))
            {
                LogComment("Fail - Expected type: " + typeof(ListCollectionView) + ". Actual: " + cvs.View.GetType());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult CustomCollection()
        {
            GreekGods greekGods = new GreekGods();
            CollectionViewSource cvs = new CollectionViewSource();
            ((ISupportInitialize)cvs).BeginInit();
            cvs.CollectionViewType = typeof(MyGreekGodsCollection);
            cvs.Source = greekGods;
            ((ISupportInitialize)cvs).EndInit();
            if (cvs.View.GetType() != typeof(MyGreekGodsCollection))
            {
                LogComment("Fail - Expected type: " + typeof(MyGreekGodsCollection) + ". Actual: " + cvs.View.GetType());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult CustomCollectionImplementsICollectionView()
        {
            GreekGods greekGods = new GreekGods();
            CollectionViewSource cvs = new CollectionViewSource();
            ((ISupportInitialize)cvs).BeginInit();
            cvs.CollectionViewType = typeof(MyCollView);
            cvs.Source = greekGods;
            ((ISupportInitialize)cvs).EndInit();
            if (cvs.View.GetType() != typeof(MyCollView))
            {
                LogComment("Fail - Expected type: " + typeof(MyCollView) + ". Actual: " + cvs.View.GetType());
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion

        #region Scenario 3 - Source is IEnumerable
        ListBox _lb3;
        TestResult SourceIsIEnumerable()
        {
            Status("SourceIsIEnumerable");
            StackPanel sp3 = (StackPanel)(LogicalTreeHelper.FindLogicalNode(RootElement, "sp3"));
            _cvs3 = (CollectionViewSource)(sp3.Resources["cvs3"]);
            CollectionViewSource cvs33 = (CollectionViewSource)(sp3.Resources["cvs33"]);
            _lb3 = (ListBox)(sp3.Children[0]);
            ListBox lb33 = (ListBox)(sp3.Children[1]);

            CollectionView icv = (CollectionView)(_cvs3.View);
            if (!CheckCanProperties(icv, true, true, true)) { return TestResult.Fail; }

            if (!CheckCurrency(_cvs3, cvs33, _lb3, lb33)) { return TestResult.Fail; }

            SetBindingInListBox(_lb3, _cvs3);

            // GetEnumerator used to be called constantly, but we are now
            // supposed to be more efficient. The three calls are due to
            // setting up CollectionViews for two ListBoxes, and another due to
            // setting up another binding. There is no formal specification as
            // to when GetEnumerator should or should not be called, so the
            // current number of calls we are expecting is simply to catch any
            // change in behavior.
            MyIEnumerable myie = (MyIEnumerable)_cvs3.Source;
            if (myie.NumGetEnumeratorCalls != 3)
                return TestResult.Fail;

            return TestResult.Pass;
        }

        TestResult FilterIEnumerable()
        {
            _cvs3.Filter += new FilterEventHandler(IEnumerableFilter);
            ArrayList listAfterFiltering = new ArrayList();
            listAfterFiltering.Add("b");
            listAfterFiltering.Add("c");
            listAfterFiltering.Add("d");
            if (!CheckItemsInView(listAfterFiltering, _cvs3)) { return TestResult.Fail; }
            _cvs3.Filter -= new FilterEventHandler(IEnumerableFilter);

            // Refer to comment in the SourceIsIEnumerable method.
            MyIEnumerable myie = (MyIEnumerable)_cvs3.Source;
            if (myie.NumGetEnumeratorCalls != 3)
                return TestResult.Fail;

            return TestResult.Pass;
        }

        TestResult SortIEnumerable()
        {
            _cvs3.SortDescriptions.Add(new SortDescription("Value", ListSortDirection.Descending));
            ArrayList listAfterSorting = new ArrayList();
            listAfterSorting.Add("d");
            listAfterSorting.Add("c");
            listAfterSorting.Add("b");
            listAfterSorting.Add("a");
            if (!CheckItemsInView(listAfterSorting, _cvs3)) { return TestResult.Fail; }
            _cvs3.SortDescriptions.Clear();

            // Refer to comment in the SourceIsIEnumerable method.
            MyIEnumerable myie = (MyIEnumerable)_cvs3.Source;
            if (myie.NumGetEnumeratorCalls != 3)
                return TestResult.Fail;

            return TestResult.Pass;
        }

        TestResult GroupByIEnumerable()
        {
            PropertyGroupDescription pgd = new PropertyGroupDescription();
            pgd.Converter = new IEnumerableGroupConverter();
            _cvs3.GroupDescriptions.Add(pgd);

            ExpectedGroup group0 = new ExpectedGroup("Good Grades", new object[] { "a", "b" });
            ExpectedGroup group1 = new ExpectedGroup("Bad Grades", new object[] { "c", "d" });

            ExpectedGroup[] expectedGroups = new ExpectedGroup[] { group0, group1};
            ReadOnlyObservableCollection<object> actualGroups = _cvs3.View.Groups;

            VerifyResult result = (VerifyResult)(_groupingVerifier.Verify(expectedGroups, actualGroups));
            if (result.Result == TestResult.Fail)
            {
                LogComment(result.Message);
                return TestResult.Fail;
            }

            _cvs3.GroupDescriptions.Clear();

            // Refer to comment in the SourceIsIEnumerable method.
            MyIEnumerable myie = (MyIEnumerable)_cvs3.Source;
            if (myie.NumGetEnumeratorCalls != 3)
                return TestResult.Fail;

            return TestResult.Pass;
        }

        void IEnumerableFilter(object item, FilterEventArgs args)
        {
            string strItem = (string)(args.Item);
            if (strItem == "a")
            {
                args.Accepted = false;
            }
            else
            {
                args.Accepted = true;
            }
        }

        private class IEnumerableGroupConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is string)
                {
                    string s = value as string;
                    if (s == "a" || s == "b")
                    {
                        return "Good Grades";
                    }
                    else
                    {
                        return "Bad Grades";
                    }
                }
                return null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return null;
            }
        }
        #endregion

        #region Scenario 4 - Source is IList
        TestResult SourceIsIList()
        {
            Status("SourceIsIList");
            StackPanel sp4 = (StackPanel)(LogicalTreeHelper.FindLogicalNode(RootElement, "sp4"));
            _cvs4 = (CollectionViewSource)(sp4.Resources["cvs4"]);
            CollectionViewSource cvs44 = (CollectionViewSource)(sp4.Resources["cvs44"]);
            ListBox lb4 = (ListBox)(sp4.Children[0]);
            ListBox lb44 = (ListBox)(sp4.Children[1]);

            ListCollectionView icv = (ListCollectionView)(_cvs4.View);
            if (!CheckCanProperties(icv, true, true, true)) { return TestResult.Fail; }

            if (!CheckCurrency(_cvs4, cvs44, lb4, lb44)) { return TestResult.Fail; }

            SetBindingInListBox(lb4, _cvs4);

            return TestResult.Pass;
        }

        TestResult FilterIList()
        {
            _cvs4.Filter += new FilterEventHandler(IListFilter);
            ArrayList listAfterFiltering = new ArrayList();
            Places places = (Places)(_cvs4.Source);
            foreach (Place place in places)
            {
                if ((place.Name).StartsWith("S"))
                {
                    listAfterFiltering.Add(place);
                }
            }
            if (!CheckItemsInView(listAfterFiltering, _cvs4)) { return TestResult.Fail; }
            _cvs4.Filter -= new FilterEventHandler(IListFilter);
            return TestResult.Pass;
        }

        TestResult SortIList()
        {
            _cvs4.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            Places places = (Places)(_cvs4.Source);
            ArrayList listAfterSorting = new ArrayList();
            listAfterSorting.Add(places[2]); // Bellevue
            listAfterSorting.Add(places[10]); // Bellingham
            listAfterSorting.Add(places[3]); // Kirkland
            listAfterSorting.Add(places[6]); // Los Angeles
            listAfterSorting.Add(places[4]); // Portland
            listAfterSorting.Add(places[1]); // Redmond
            listAfterSorting.Add(places[7]); // San Diego
            listAfterSorting.Add(places[5]); // San Francisco
            listAfterSorting.Add(places[8]); // San Jose
            listAfterSorting.Add(places[9]); // Santa Ana
            listAfterSorting.Add(places[0]); // Seattle
            if (!CheckItemsInView(listAfterSorting, _cvs4)) { return TestResult.Fail; }
            _cvs4.SortDescriptions.Clear();
            return TestResult.Pass;
        }

        TestResult GroupByIList()
        {
            _cvs4.GroupDescriptions.Add(new PropertyGroupDescription("State"));

            Places places = (Places)(_cvs4.Source);

            ExpectedGroup group0 = new ExpectedGroup("WA", new object[] { places[0], places[1], places[2], places[3], places[10] });
            ExpectedGroup group1 = new ExpectedGroup("OR", new object[] { places[4] });
            ExpectedGroup group2 = new ExpectedGroup("CA", new object[] { places[5], places[6], places[7], places[8], places[9] });

            ExpectedGroup[] expectedGroups = new ExpectedGroup[] { group0, group1, group2};
            ReadOnlyObservableCollection<object> actualGroups = _cvs4.View.Groups;

            VerifyResult result = (VerifyResult)(_groupingVerifier.Verify(expectedGroups, actualGroups));
            if (result.Result == TestResult.Fail)
            {
                LogComment(result.Message);
                return TestResult.Fail;
            }

            _cvs4.GroupDescriptions.Clear();

            return TestResult.Pass;
        }

        void IListFilter(object item, FilterEventArgs args)
        {
            Place placeItem = (Place)(args.Item);
            if ((placeItem.Name).StartsWith("S"))
            {
                args.Accepted = true;
            }
            else
            {
                args.Accepted = false;
            }
        }

        #endregion

        #region Scenario 5 - Source is IBindingList
        TestResult SourceIsIBindingList()
        {
            Status("SourceIsIBindingList");
            StackPanel sp5 = (StackPanel)(LogicalTreeHelper.FindLogicalNode(RootElement, "sp5"));
            _cvs5 = (CollectionViewSource)(sp5.Resources["cvs5"]);
            CollectionViewSource cvs55 = (CollectionViewSource)(sp5.Resources["cvs55"]);
            ListBox lb5 = (ListBox)(sp5.Children[0]);
            ListBox lb55 = (ListBox)(sp5.Children[1]);

            BindingListCollectionView icv = (BindingListCollectionView)(_cvs5.View);
            
            if (!CheckCanProperties(icv, true, false, true)) { return TestResult.Fail; }

            // BindingListCollectionView has an extra property - CanCustomFilter
            if (icv.CanCustomFilter != true)
            {
                LogComment("Fail - CanCustomFilter should be true but is false");
                return TestResult.Fail;
            }

            if (!CheckCurrency(_cvs5, cvs55, lb5, lb55)) { return TestResult.Fail; }

            SetBindingInListBox(lb5, _cvs5);

            return TestResult.Pass;
        }

        TestResult FilterIsIBindingList()
        {
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            _cvs5.Filter += new FilterEventHandler(MyDoNothingFilter);
            return TestResult.Pass;
        }

        TestResult CustomFilterIsIBindingList()
        {
            BindingListCollectionView blcv = (BindingListCollectionView)(_cvs5.View);
            blcv.CustomFilter = "State <> 'WA'";
            PlacesDataTable places = (PlacesDataTable)(_cvs5.Source);
            ArrayList listAfterSorting = new ArrayList();
            listAfterSorting.Add(places.DefaultView[0]); // Portland
            listAfterSorting.Add(places.DefaultView[1]); // San Francisco
            listAfterSorting.Add(places.DefaultView[2]); // Los Angeles
            listAfterSorting.Add(places.DefaultView[3]); // San Diego
            listAfterSorting.Add(places.DefaultView[4]); // San Jose
            listAfterSorting.Add(places.DefaultView[5]); // Santa Ana

            if (!CheckItemsInView(listAfterSorting, _cvs5)) { return TestResult.Fail; }
            blcv.CustomFilter = "";
            return TestResult.Pass;
        }

        TestResult SortIsIBindingList()
        {
            _cvs5.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            PlacesDataTable places = (PlacesDataTable)(_cvs5.Source);
            ArrayList listAfterSorting = new ArrayList();
            listAfterSorting.Add(places.DefaultView[2]); // Bellevue
            listAfterSorting.Add(places.DefaultView[10]); // Bellingham
            listAfterSorting.Add(places.DefaultView[3]); // Kirkland
            listAfterSorting.Add(places.DefaultView[6]); // Los Angeles
            listAfterSorting.Add(places.DefaultView[4]); // Portland
            listAfterSorting.Add(places.DefaultView[1]); // Redmond
            listAfterSorting.Add(places.DefaultView[7]); // San Diego
            listAfterSorting.Add(places.DefaultView[5]); // San Francisco
            listAfterSorting.Add(places.DefaultView[8]); // San Jose
            listAfterSorting.Add(places.DefaultView[9]); // Santa Ana
            listAfterSorting.Add(places.DefaultView[0]); // Seattle

            if (!CheckItemsInView(listAfterSorting, _cvs5)) { return TestResult.Fail; }
            _cvs5.SortDescriptions.Clear();
            return TestResult.Pass;
        }

        TestResult GroupByIBindingList()
        {
            _cvs5.GroupDescriptions.Add(new PropertyGroupDescription("State"));

            PlacesDataTable places = (PlacesDataTable)(_cvs5.Source);

            ExpectedGroup group0 = new ExpectedGroup("WA", new object[] { places.DefaultView[0], places.DefaultView[1], places.DefaultView[2], places.DefaultView[3], places.DefaultView[10] });
            ExpectedGroup group1 = new ExpectedGroup("OR", new object[] { places.DefaultView[4] });
            ExpectedGroup group2 = new ExpectedGroup("CA", new object[] { places.DefaultView[5], places.DefaultView[6], places.DefaultView[7], places.DefaultView[8], places.DefaultView[9] });

            ExpectedGroup[] expectedGroups = new ExpectedGroup[] { group0, group1, group2 };
            ReadOnlyObservableCollection<object> actualGroups = _cvs5.View.Groups;

            VerifyResult result = (VerifyResult)(_groupingVerifier.Verify(expectedGroups, actualGroups));
            if (result.Result == TestResult.Fail)
            {
                LogComment(result.Message);
                return TestResult.Fail;
            }

            _cvs5.GroupDescriptions.Clear();

            return TestResult.Pass;
        }

        #endregion

        #region Scenario 6 - Source is CollectionView
        TestResult SourceIsCollectionView()
        {
            Status("SourceIsCollectionView");
            
            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            StackPanel sp6 = (StackPanel)(LogicalTreeHelper.FindLogicalNode(RootElement, "sp6"));
            _cvs6 = (CollectionViewSource)(sp6.Resources["cvs6"]);

            GreekGods gg = new GreekGods();
            ListCollectionView lcv = (ListCollectionView)(CollectionViewSource.GetDefaultView(gg));
            _cvs6.Source = lcv;

            return TestResult.Pass;
        }
        #endregion

        #region Scenario 7 - Source implements ICollectionView (but does not derive from CollectionView)
        TestResult SourceIsICollectionView()
        {
            Status("SourceIsICollectionView");
            
            SetExpectedErrorTypeInStep(typeof(ArgumentException));
            StackPanel sp7 = (StackPanel)(LogicalTreeHelper.FindLogicalNode(RootElement, "sp7"));
            _cvs7 = (CollectionViewSource)(sp7.Resources["cvs7"]);

            MyCollView mcv = new MyCollView(new Places());
            _cvs7.Source = mcv;

            return TestResult.Pass;
        }
        #endregion

        #region Scenario 8 - Source is XmlDataProvider
        
        TestResult SourceIsXmlDataProvider()
        {
            Status("SourceIsXmlDataProvider");
            WaitForPriority(DispatcherPriority.SystemIdle);
            StackPanel sp8 = (StackPanel)(LogicalTreeHelper.FindLogicalNode(RootElement, "sp8"));
            _cvs8 = (CollectionViewSource)(sp8.Resources["cvs8"]);
            CollectionViewSource cvs88 = (CollectionViewSource)(sp8.Resources["cvs88"]);
            ListBox lb8 = (ListBox)(sp8.Children[0]);
            ListBox lb88 = (ListBox)(sp8.Children[1]);
            WaitForPriority(DispatcherPriority.Background);
            ListCollectionView icv = (ListCollectionView)(_cvs8.View);

            // Needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && (icv == null))
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                System.Threading.Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
                icv = (ListCollectionView)(_cvs8.View);
            }

            
            if (!CheckCanProperties(icv, true, true, true)) { return TestResult.Fail; }
            if (!CheckCurrency(_cvs8, cvs88, lb8, lb88)) { return TestResult.Fail; }

            SetBindingInListBox(lb8, _cvs8);

            return TestResult.Pass;
        }

        TestResult FilterIsXmlDataProvider()
        {
            _cvs8.Filter += new FilterEventHandler(XmlDataProviderFilter);

            StackPanel sp8 = (StackPanel)(LogicalTreeHelper.FindLogicalNode(RootElement, "sp8"));
            XmlDataProvider xdp = (XmlDataProvider)(sp8.Resources["xdp"]);
            ReadOnlyObservableCollection<XmlNode> nodes = (ReadOnlyObservableCollection<XmlNode>)(xdp.Data);

            ArrayList listAfterFiltering = new ArrayList();
            listAfterFiltering.Add(nodes[4]);
            if (!CheckItemsInView(listAfterFiltering, _cvs8)) { return TestResult.Fail; }

            _cvs8.Filter -= new FilterEventHandler(XmlDataProviderFilter);
            return TestResult.Pass;
        }

        void XmlDataProviderFilter(object item, FilterEventArgs args)
        {
            XmlElement element = (XmlElement)(args.Item);
            XmlNode title = element.ChildNodes[0];
            if (title.InnerText == "The Usual Suspects")
            {
                args.Accepted = true;
            }
            else
            {
                args.Accepted = false;
            }
        }

        TestResult SortIsXmlDataProvider()
        {
            _cvs8.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));

            StackPanel sp8 = (StackPanel)(LogicalTreeHelper.FindLogicalNode(RootElement, "sp8"));
            XmlDataProvider xdp = (XmlDataProvider)(sp8.Resources["xdp"]);
            ReadOnlyObservableCollection<XmlNode> nodes = (ReadOnlyObservableCollection<XmlNode>)(xdp.Data);

            ArrayList listAfterSorting = new ArrayList();
            listAfterSorting.Add(nodes[1]); // Finding Nemo
            listAfterSorting.Add(nodes[2]); // Inside C#
            listAfterSorting.Add(nodes[0]); // Microsoft C# Language Specification
            listAfterSorting.Add(nodes[3]); // Pirates of the Caribbean
            listAfterSorting.Add(nodes[4]); // The Usual Suspects
            listAfterSorting.Add(nodes[5]); // Xml in Action
            if (!CheckItemsInView(listAfterSorting, _cvs8)) { return TestResult.Fail; }
            _cvs8.SortDescriptions.Clear();

            return TestResult.Pass;
        }

        TestResult GroupByXmlDataProvider()
        {
            _cvs8.GroupDescriptions.Add(new PropertyGroupDescription("Price"));

            StackPanel sp8 = (StackPanel)(LogicalTreeHelper.FindLogicalNode(RootElement, "sp8"));
            XmlDataProvider xdp = (XmlDataProvider)(sp8.Resources["xdp"]);
            ReadOnlyObservableCollection<XmlNode> nodes = (ReadOnlyObservableCollection<XmlNode>)(xdp.Data);

            ExpectedGroup group0 = new ExpectedGroup("29.99", new object[] { nodes[0], nodes[2], nodes[5] });
            ExpectedGroup group1 = new ExpectedGroup("19.99", new object[] { nodes[1], nodes[3], nodes[4] });

            ExpectedGroup[] expectedGroups = new ExpectedGroup[] { group0, group1 };
            ReadOnlyObservableCollection<object> actualGroups = _cvs8.View.Groups;

            VerifyResult result = (VerifyResult)(_groupingVerifier.Verify(expectedGroups, actualGroups));
            if (result.Result == TestResult.Fail)
            {
                LogComment(result.Message);
                return TestResult.Fail;
            }

            _cvs8.GroupDescriptions.Clear();

            return TestResult.Pass;
        }

        
        TestResult GroupNoPropertyNameIsXmlDataProvider()
        {
            _cvs8.GroupDescriptions.Add(new PropertyGroupDescription());

            StackPanel sp8 = (StackPanel)(LogicalTreeHelper.FindLogicalNode(RootElement, "sp8"));
            XmlDataProvider xdp = (XmlDataProvider)(sp8.Resources["xdp"]);
            ReadOnlyObservableCollection<XmlNode> nodes = (ReadOnlyObservableCollection<XmlNode>)(xdp.Data);

            ExpectedGroup group0 = new ExpectedGroup(nodes[0], new object[] { nodes[0] });
            ExpectedGroup group1 = new ExpectedGroup(nodes[1], new object[] { nodes[1] });
            ExpectedGroup group2 = new ExpectedGroup(nodes[2], new object[] { nodes[2] });
            ExpectedGroup group3 = new ExpectedGroup(nodes[3], new object[] { nodes[3] });
            ExpectedGroup group4 = new ExpectedGroup(nodes[4], new object[] { nodes[4] });
            ExpectedGroup group5 = new ExpectedGroup(nodes[5], new object[] { nodes[5] });

            ExpectedGroup[] expectedGroups = new ExpectedGroup[] { group0, group1, group2, group3, group4, group5 };
            ReadOnlyObservableCollection<object> actualGroups = _cvs8.View.Groups;

            VerifyResult result = (VerifyResult)(_groupingVerifier.Verify(expectedGroups, actualGroups));
            if (result.Result == TestResult.Fail)
            {
                LogComment(result.Message);
                return TestResult.Fail;
            }

            _cvs8.GroupDescriptions.Clear();

            return TestResult.Pass;
        }

        TestResult SortNoPropertyNameIsXmlDataProvider()
        {
            // this throws become XmlElement does not implement IComparable
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            _cvs8.SortDescriptions.Add(new SortDescription());
            _cvs8.SortDescriptions.Clear();
            return TestResult.Fail;
        }
        #endregion

        #region Scenario 9 - Source is BindingList<T>
        TestResult SourceIsBindingListT()
        {
            Status("SourceIsBindingListT");
            StackPanel sp9 = (StackPanel)(LogicalTreeHelper.FindLogicalNode(RootElement, "sp9"));
            _cvs9 = (CollectionViewSource)(sp9.Resources["cvs9"]);
            CollectionViewSource cvs99 = (CollectionViewSource)(sp9.Resources["cvs99"]);
            ListBox lb9 = (ListBox)(sp9.Children[0]);
            ListBox lb99 = (ListBox)(sp9.Children[1]);

            BindingListCollectionView tcv = (BindingListCollectionView)(_cvs9.View);
            
            if (!CheckCanProperties(tcv, true, false, true)) { return TestResult.Fail; }

            // BindingListCollectionView has an extra property - CanCustomFilter
            if (tcv.CanCustomFilter != true)
            {
                LogComment("Fail - CanCustomFilter should be true but is false");
                return TestResult.Fail;
            }

            if (!CheckCurrency(_cvs9, cvs99, lb9, lb99)) { return TestResult.Fail; }

            SetBindingInListBox(lb9, _cvs9);

            return TestResult.Pass;
        }

        TestResult FilterIsBindingListT()
        {
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            _cvs9.Filter += new FilterEventHandler(MyDoNothingFilter);
            return TestResult.Pass;
        }

        TestResult CustomFilterIsBindingListT()
        {
            BindingListCollectionView bltcv = (BindingListCollectionView)(_cvs9.View);
            bltcv.CustomFilter = "State < 'W'";
            PlacesBindingListT places = (PlacesBindingListT)(_cvs9.Source);
            ArrayList listAfterSorting = new ArrayList();
            listAfterSorting.Add(places[0]); // Portland
            listAfterSorting.Add(places[1]); // San Francisco
            listAfterSorting.Add(places[2]); // Los Angeles
            listAfterSorting.Add(places[3]); // San Diego
            listAfterSorting.Add(places[4]); // San Jose
            listAfterSorting.Add(places[5]); // Santa Ana

            if (!CheckItemsInView(listAfterSorting, _cvs9)) { return TestResult.Fail; }
            bltcv.CustomFilter = "";
            return TestResult.Pass;
        }

        TestResult SortIsBindingListT()
        {
            _cvs9.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            PlacesBindingListT places = (PlacesBindingListT)(_cvs9.Source);
            ArrayList listAfterSorting = new ArrayList();
            listAfterSorting.Add(places[2]); // Bellevue
            listAfterSorting.Add(places[10]); // Bellingham
            listAfterSorting.Add(places[3]); // Kirkland
            listAfterSorting.Add(places[6]); // Los Angeles
            listAfterSorting.Add(places[4]); // Portland
            listAfterSorting.Add(places[1]); // Redmond
            listAfterSorting.Add(places[7]); // San Diego
            listAfterSorting.Add(places[5]); // San Francisco
            listAfterSorting.Add(places[8]); // San Jose
            listAfterSorting.Add(places[9]); // Santa Ana
            listAfterSorting.Add(places[0]); // Seattle

            if (!CheckItemsInView(listAfterSorting, _cvs9)) { return TestResult.Fail; }
            _cvs9.SortDescriptions.Clear();
            return TestResult.Pass;
        }

        TestResult GroupByBindingListT()
        {
            _cvs9.GroupDescriptions.Add(new PropertyGroupDescription("State"));

            PlacesBindingListT places = (PlacesBindingListT)(_cvs9.Source);

            ExpectedGroup group0 = new ExpectedGroup("WA", new object[] { places[0], places[1], places[2], places[3], places[10] });
            ExpectedGroup group1 = new ExpectedGroup("OR", new object[] { places[4] });
            ExpectedGroup group2 = new ExpectedGroup("CA", new object[] { places[5], places[6], places[7], places[8], places[9] });

            ExpectedGroup[] expectedGroups = new ExpectedGroup[] { group0, group1, group2 };
            ReadOnlyObservableCollection<object> actualGroups = _cvs9.View.Groups;

            VerifyResult result = (VerifyResult)(_groupingVerifier.Verify(expectedGroups, actualGroups));
            if (result.Result == TestResult.Fail)
            {
                LogComment(result.Message);
                return TestResult.Fail;
            }

            _cvs9.GroupDescriptions.Clear();

            return TestResult.Pass;
        }

        #endregion

        #region Validation methods
        bool CheckCanProperties(ICollectionView cv, bool canSort, bool canFilter, bool canGroupBy)
        {
            if (cv.CanSort != canSort)
            {
                LogComment("Fail - Expected CanSort: " + canSort + ". Actual: " + cv.CanSort);
                return false;
            }
            if (cv.CanFilter != canFilter)
            {
                LogComment("Fail - Expected CanFilter: " + canFilter + ". Actual: " + cv.CanFilter);
                return false;
            }
            if (cv.CanGroup != canGroupBy)
            {
                LogComment("Fail - Expected CanGroup: " + canGroupBy + ". Actual: " + cv.CanGroup);
                return false;
            }
            return true;
        }

        bool CheckCurrency(CollectionViewSource cvs1, CollectionViewSource cvs2, ListBox lb1, ListBox lb2)
        {
            ICollectionView icv1 = cvs1.View;
            ICollectionView icv2 = cvs2.View;

            // Verify that 2 list boxes bound to 2 views of the same collection don't sync with eachother
            // but selection and currency are in sync for each list box
            Binding b1 = new Binding();
            b1.Source = cvs1;
            lb1.SetBinding(ItemsControl.ItemsSourceProperty, b1);
            Binding b2 = new Binding();
            b2.Source = cvs2;
            lb2.SetBinding(ItemsControl.ItemsSourceProperty, b2);
            ResetSelectionAndCurrency(lb1, lb2, icv1, icv2);

            icv1.MoveCurrentToLast();

            if (lb1.SelectedItem != icv1.CurrentItem)
            {
                LogComment("Fail - Currency and selection not in sync in first list box.");
                return false;
            }
            if (lb2.SelectedItem != icv2.CurrentItem)
            {
                LogComment("Fail - Currency and selection not in sync in second list box.");
                return false;
            }
            if (lb1.SelectedItem == lb2.SelectedItem)
            {
                LogComment("Fail - Two list boxes bound to different CVSs are in sync, they should not be.");
                return false;
            }

            // Verify that 2 list boxes bound to the same view sync with eachother and currency and selection
            // sync for each list box
            lb1.SetBinding(ItemsControl.ItemsSourceProperty, b1);
            lb2.SetBinding(ItemsControl.ItemsSourceProperty, b1);
            ResetSelectionAndCurrency(lb1, lb2, icv1, icv2);

            icv1.MoveCurrentToLast();
            icv1.MoveCurrentToPrevious();

            if (lb1.SelectedItem != icv1.CurrentItem)
            {
                LogComment("Fail - Currency and selection not in sync in first list box.");
                return false;
            }
            if (lb2.SelectedItem != icv1.CurrentItem)
            {
                LogComment("Fail - Currency and selection not in sync in second list box.");
                return false;
            }
            if (lb1.SelectedItem != lb2.SelectedItem)
            {
                LogComment("Fail - Two list boxes bound to the same CVS are not in sync, they should be.");
                return false;
            }

            // Verify that 2 list boxes bound to the default view of a particular collection don't sync
            // with eachother and selection and currency are not in sync for each list box

            Binding b3 = new Binding();
            b3.Source = cvs1.Source;
            lb1.SetBinding(ItemsControl.ItemsSourceProperty, b3);
            lb2.SetBinding(ItemsControl.ItemsSourceProperty, b3);
            ResetSelectionAndCurrency(lb1, lb2, icv1, icv2);

            icv1.MoveCurrentToFirst();

            if (lb1.SelectedItem == icv1.CurrentItem)
            {
                LogComment("Fail - Currency and selection in sync in first list box.");
                return false;
            }
            if (lb2.SelectedItem == icv1.CurrentItem)
            {
                LogComment("Fail - Currency and selection in sync in second list box.");
                return false;
            }
            if (lb1.SelectedItem != null)
            {
                LogComment("Fail - There should be nothing selected in first list box.");
                return false;
            }
            if (lb2.SelectedItem != null)
            {
                LogComment("Fail - There should be nothing selected in second list box.");
                return false;
            }

            return true;
        }

        void ResetSelectionAndCurrency(ListBox lb1, ListBox lb2, ICollectionView icv1, ICollectionView icv2)
        {
            lb1.UnselectAll();
            lb2.UnselectAll();
            icv1.MoveCurrentToFirst();
            icv2.MoveCurrentToFirst();
        }

        void SetBindingInListBox(ListBox lb, CollectionViewSource cvs)
        {
            Binding b = new Binding();
            b.Source = cvs;
            lb.SetBinding(ItemsControl.ItemsSourceProperty, b);
        }

        bool CheckItemsInView(ArrayList expectedItems, CollectionViewSource cvs)
        {
            int listCount = expectedItems.Count;
            int viewCount = 0;
            foreach (object item in cvs.View)
            {
                if (!expectedItems.Contains(item))
                {
                    LogComment("Fail - View should not contain item " + item);
                    return false;
                }
                viewCount++;
            }
            if (listCount != viewCount)
            {
                LogComment("Fail - Fail - There should be " + listCount + " element in the view, instead there are " + viewCount);
                return false;
            }
            return true;
        }
        #endregion
    }
}
