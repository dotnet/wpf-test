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
    /// Non-uniform subgrouping. In this scenario, the second level
    /// of grouping depends on the first level of grouping. The listCollectionView.GroupBySelector property
    /// is used to accomplish this scenario. This test case covers both CLR object and XmlDataProvider.
    /// </description>
    /// <relatedBugs>


    /// </relatedBugs>
    /// </summary>
    [Test(3, "Collections", "NonUniformSubgrouping")]
    public class NonUniformSubgrouping : XamlTest
    {
        private ListBox _clrListBox;
        private ListBox _xmlListBox;
        private CollectionViewSource _clrCvs;
        private CollectionViewSource _xmlCvs;
        private GroupingVerifier _groupingVerifier;
        private ObservableCollection<CountryWithExtraInfo> _countries;
        private ReadOnlyObservableCollection<XmlNode> _xmlCountryCollection;
        private ExpectedGroup[] _expectedGroupsClr;
        private ExpectedGroup[] _expectedGroupsXml;

        public NonUniformSubgrouping()
            : base(@"NonUniformSubgrouping.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            // Clr
            RunSteps += new TestStep(VerifyGroupsClr);
            RunSteps += new TestStep(AddItemsAndVerifyClr);
            RunSteps += new TestStep(RemoveItemsAndVerifyClr);
            // Notice that changing the property set to PropertyName does not cause the items to be regrouped
            // because the cost of listening for property changes would be too high
            // Xml
            RunSteps += new TestStep(VerifyGroupsXml);
            RunSteps += new TestStep(AddItemsAndVerifyXml);
            RunSteps += new TestStep(RemoveItemsAndVerifyXml);
        }

        TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.Render);

            _clrListBox = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "clrListBox"));
            _xmlListBox = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "xmlListBox"));
            _clrCvs = (CollectionViewSource)(this.RootElement.Resources["clrCvs"]);
            _xmlCvs = (CollectionViewSource)(this.RootElement.Resources["xmlCvs"]);

            CountriesDataSource clrCountries = (CountriesDataSource)(this.RootElement.Resources["clrCountries"]);
            _countries = clrCountries.Countries;
            XmlDataProvider xmlCountries = (XmlDataProvider)(this.RootElement.Resources["xmlCountries"]);
            _xmlCountryCollection = (ReadOnlyObservableCollection<XmlNode>)DataSourceHelper.WaitForData(xmlCountries);

            ListCollectionView clrView = (ListCollectionView)(_clrCvs.View);
            ListCollectionView xmlView = (ListCollectionView)(_xmlCvs.View);

            clrView.GroupBySelector = new GroupDescriptionSelectorCallback(ClrNonUniformCallback);
            xmlView.GroupBySelector = new GroupDescriptionSelectorCallback(XmlNonUniformCallback);

            _groupingVerifier = new GroupingVerifier();

            return TestResult.Pass;
        }

        private PropertyGroupDescription ClrNonUniformCallback(CollectionViewGroup group, int level)
        {
            if ((level == 1) && ((GovernmentType)(group.Name) == GovernmentType.Republic))
            {
                return new PropertyGroupDescription("IndependenceDay.Year");
            }
            if ((level == 2) && (group.Name is Int32))
            {
                return new PropertyGroupDescription("IndependenceDay.Month");
            }
            return null;
        }

        private PropertyGroupDescription XmlNonUniformCallback(CollectionViewGroup group, int level)
        {
            int intYear;
            if ((level == 1) && ((string)(group.Name) == "Republic"))
            {
                return new PropertyGroupDescription("IndependenceDay/Year");
            }
            if ((level == 2) && (Int32.TryParse(group.Name.ToString(), out intYear)))
            {
                return new PropertyGroupDescription("IndependenceDay/Month");
            }
            return null;
        }

        TestResult VerifyGroupsClr()
        {
            Status("VerifyGroupsClr");
            // Group 0 - Republic
            ExpectedGroup group000 = new ExpectedGroup(5, new object[] { _countries[0] });
            ExpectedGroup group010 = new ExpectedGroup(9, new object[] { _countries[3] });
            ExpectedGroup group020 = new ExpectedGroup(10, new object[] { _countries[4] });
            ExpectedGroup group021 = new ExpectedGroup(8, new object[] { _countries[5], _countries[8] });
            ExpectedGroup group022 = new ExpectedGroup(9, new object[] { _countries[12] });
            ExpectedGroup group030 = new ExpectedGroup(1, new object[] { _countries[6] });
            ExpectedGroup group040 = new ExpectedGroup(6, new object[] { _countries[10] });
            ExpectedGroup group050 = new ExpectedGroup(11, new object[] { _countries[11] });
            ExpectedGroup group060 = new ExpectedGroup(9, new object[] { _countries[13] });
            ExpectedGroup group070 = new ExpectedGroup(5, new object[] { _countries[14] });
            ExpectedGroup group080 = new ExpectedGroup(9, new object[] { _countries[15] });

            ExpectedGroup group00 = new ExpectedGroup(1990, new object[] { group000 });
            ExpectedGroup group01 = new ExpectedGroup(301, new object[] { group010 });
            ExpectedGroup group02 = new ExpectedGroup(1991, new object[] { group020, group021, group022 });
            ExpectedGroup group03 = new ExpectedGroup(1968, new object[] { group030 });
            ExpectedGroup group04 = new ExpectedGroup(1977, new object[] { group040 });
            ExpectedGroup group05 = new ExpectedGroup(1975, new object[] { group050 });
            ExpectedGroup group06 = new ExpectedGroup(1821, new object[] { group060 });
            ExpectedGroup group07 = new ExpectedGroup(1966, new object[] { group070 });
            ExpectedGroup group08 = new ExpectedGroup(1810, new object[] { group080 });

            ExpectedGroup group0 = new ExpectedGroup(GovernmentType.Republic, new object[] { group00, group01, group02, group03, group04, group05, group06, group07, group08 });

            // Group 1 - Monarchy
            ExpectedGroup group10 = new ExpectedGroup("Middle East", new object[] { _countries[1] });
            ExpectedGroup group1 = new ExpectedGroup(GovernmentType.Monarchy, new object[] { group10 });

            // Group 2 - Democracy
            ExpectedGroup group20 = new ExpectedGroup("The Caribbean", new object[] { _countries[2] });
            ExpectedGroup group21 = new ExpectedGroup("Africa", new object[] { _countries[7], _countries[9] });
            ExpectedGroup group2 = new ExpectedGroup(GovernmentType.Democracy, new object[] { group20, group21 });

            _expectedGroupsClr = new ExpectedGroup[] { group0, group1, group2 };
            ReadOnlyObservableCollection<object> actualGroups = _clrCvs.View.Groups;

            VerifyResult groupingResult = (VerifyResult)(_groupingVerifier.Verify(_expectedGroupsClr, actualGroups));
            if (groupingResult.Result != TestResult.Pass)
            {
                LogComment(groupingResult.Message);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // Adds an item in the non-uniform level 1 group (GovernmentType.Republic) and verifies that a correct 
        // subgroup (level 2 = IndependenceDay.Year) and sub-sub group (level 3 = IndependenceDay.Month) 
        // are created for that item (note that this item does not belong to any existing sub groups).
        TestResult AddItemsAndVerifyClr()
        {
            Status("AddItemsAndVerifyClr");

            _countries.Add(new CountryWithExtraInfo("New country", "New capital", new DateTime(2000, 12, 20), 1000000, GovernmentType.Republic, "New Region", "Eastern Hemisphere"));
            
            ExpectedGroup group090 = new ExpectedGroup(12, new object[] { _countries[16] });
            ExpectedGroup group09 = new ExpectedGroup(2000, new object[] { group090 });

            _expectedGroupsClr[0] = new ExpectedGroup(GovernmentType.Republic, new object[] { 
                _expectedGroupsClr[0].Items[0], _expectedGroupsClr[0].Items[1], _expectedGroupsClr[0].Items[2], 
                _expectedGroupsClr[0].Items[3], _expectedGroupsClr[0].Items[4], _expectedGroupsClr[0].Items[5], 
                _expectedGroupsClr[0].Items[6], _expectedGroupsClr[0].Items[7], _expectedGroupsClr[0].Items[8], group09 });

            VerifyResult groupingResult = (VerifyResult)(_groupingVerifier.Verify(_expectedGroupsClr, _clrCvs.View.Groups));
            if (groupingResult.Result != TestResult.Pass)
            {
                LogComment(groupingResult.Message);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        // Removes all items (there's only one) from the non-uniform level 1 group GovernmentType.Republic that 
        // have subgroup year = 1990 and sub-sub group month = 5. Verifies that the sub group is removed from 
        // the group.
        TestResult RemoveItemsAndVerifyClr()
        {
            Status("RemoveItemsAndVerifyClr");
            _countries.RemoveAt(0);

            _expectedGroupsClr[0] = new ExpectedGroup(GovernmentType.Republic, new object[] { 
                _expectedGroupsClr[0].Items[1], _expectedGroupsClr[0].Items[2], 
                _expectedGroupsClr[0].Items[3], _expectedGroupsClr[0].Items[4], _expectedGroupsClr[0].Items[5], 
                _expectedGroupsClr[0].Items[6], _expectedGroupsClr[0].Items[7], _expectedGroupsClr[0].Items[8], 
                _expectedGroupsClr[0].Items[9] });

            VerifyResult groupingResult = (VerifyResult)(_groupingVerifier.Verify(_expectedGroupsClr, _clrCvs.View.Groups));
            if (groupingResult.Result != TestResult.Pass)
            {
                LogComment(groupingResult.Message);
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        // The types of the groups in the XML scenario are all string (and not integer or enum like in the CLR
        // scenario) because they are formed implicitly based on XML data which doesn't know about those types.
        // This made it so I couldn't reuse the verification logic I wrote for CLR object.
        TestResult VerifyGroupsXml()
        {
            Status("VerifyGroupsXml");
            // Group 0 - Republic
            ExpectedGroup group000 = new ExpectedGroup("5", new object[] { _xmlCountryCollection[0] });
            ExpectedGroup group010 = new ExpectedGroup("9", new object[] { _xmlCountryCollection[3] });
            ExpectedGroup group020 = new ExpectedGroup("10", new object[] { _xmlCountryCollection[4] });
            ExpectedGroup group021 = new ExpectedGroup("8", new object[] { _xmlCountryCollection[5], _xmlCountryCollection[8] });
            ExpectedGroup group022 = new ExpectedGroup("9", new object[] { _xmlCountryCollection[12] });
            ExpectedGroup group030 = new ExpectedGroup("1", new object[] { _xmlCountryCollection[6] });
            ExpectedGroup group040 = new ExpectedGroup("6", new object[] { _xmlCountryCollection[10] });
            ExpectedGroup group050 = new ExpectedGroup("11", new object[] { _xmlCountryCollection[11] });
            ExpectedGroup group060 = new ExpectedGroup("9", new object[] { _xmlCountryCollection[13] });
            ExpectedGroup group070 = new ExpectedGroup("5", new object[] { _xmlCountryCollection[14] });
            ExpectedGroup group080 = new ExpectedGroup("9", new object[] { _xmlCountryCollection[15] });

            ExpectedGroup group00 = new ExpectedGroup("1990", new object[] { group000 });
            ExpectedGroup group01 = new ExpectedGroup("301", new object[] { group010 });
            ExpectedGroup group02 = new ExpectedGroup("1991", new object[] { group020, group021, group022 });
            ExpectedGroup group03 = new ExpectedGroup("1968", new object[] { group030 });
            ExpectedGroup group04 = new ExpectedGroup("1977", new object[] { group040 });
            ExpectedGroup group05 = new ExpectedGroup("1975", new object[] { group050 });
            ExpectedGroup group06 = new ExpectedGroup("1821", new object[] { group060 });
            ExpectedGroup group07 = new ExpectedGroup("1966", new object[] { group070 });
            ExpectedGroup group08 = new ExpectedGroup("1810", new object[] { group080 });

            ExpectedGroup group0 = new ExpectedGroup("Republic", new object[] { group00, group01, group02, group03, group04, group05, group06, group07, group08 });

            // Group 1 - Monarchy
            ExpectedGroup group10 = new ExpectedGroup("Middle East", new object[] { _xmlCountryCollection[1] });
            ExpectedGroup group1 = new ExpectedGroup("Monarchy", new object[] { group10 });

            // Group 2 - Democracy
            ExpectedGroup group20 = new ExpectedGroup("The Caribbean", new object[] { _xmlCountryCollection[2] });
            ExpectedGroup group21 = new ExpectedGroup("Africa", new object[] { _xmlCountryCollection[7], _xmlCountryCollection[9] });
            ExpectedGroup group2 = new ExpectedGroup("Democracy", new object[] { group20, group21 });

            _expectedGroupsXml = new ExpectedGroup[] { group0, group1, group2 };
            ReadOnlyObservableCollection<object> actualGroups = _xmlCvs.View.Groups;

            VerifyResult groupingResult = (VerifyResult)(_groupingVerifier.Verify(_expectedGroupsXml, actualGroups));
            if (groupingResult.Result != TestResult.Pass)
            {
                LogComment(groupingResult.Message);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult AddItemsAndVerifyXml()
        {
            Status("AddItemsAndVerifyXml");

            XmlDocument document = _xmlCountryCollection[0].OwnerDocument;
            XmlElement countryElement = document.CreateElement("Country");
            XmlElement countryNameElement = document.CreateElement("CountryName");
            countryNameElement.InnerText = "New country";
            XmlElement capitalElement = document.CreateElement("Capital");
            capitalElement.InnerText = "New capital";
            XmlElement independencyDayElement = document.CreateElement("IndependenceDay");
            XmlElement yearElement = document.CreateElement("Year");
            yearElement.InnerText = "2000";
            XmlElement monthElement = document.CreateElement("Month");
            monthElement.InnerText = "12";
            XmlElement dayElement = document.CreateElement("Day");
            dayElement.InnerText = "20";
            independencyDayElement.AppendChild(yearElement);
            independencyDayElement.AppendChild(monthElement);
            independencyDayElement.AppendChild(dayElement);
            XmlElement populationElement = document.CreateElement("Population");
            populationElement.InnerText = "1000000";
            XmlElement governmentElement = document.CreateElement("Government");
            governmentElement.InnerText = "Republic";
            XmlElement regionElement = document.CreateElement("Region");
            regionElement.InnerText = "New Region";
            XmlElement hemisphereElement = document.CreateElement("Hemisphere");
            hemisphereElement.InnerText = "Eastern Hemisphere";
            countryElement.AppendChild(countryNameElement);
            countryElement.AppendChild(capitalElement);
            countryElement.AppendChild(independencyDayElement);
            countryElement.AppendChild(populationElement);
            countryElement.AppendChild(governmentElement);
            countryElement.AppendChild(regionElement);
            countryElement.AppendChild(hemisphereElement);

            _xmlCountryCollection[0].ParentNode.AppendChild(countryElement);

            ExpectedGroup group090 = new ExpectedGroup("12", new object[] { _xmlCountryCollection[16] });
            ExpectedGroup group09 = new ExpectedGroup("2000", new object[] { group090 });

            _expectedGroupsXml[0] = new ExpectedGroup("Republic", new object[] { 
                _expectedGroupsXml[0].Items[0], _expectedGroupsXml[0].Items[1], _expectedGroupsXml[0].Items[2], 
                _expectedGroupsXml[0].Items[3], _expectedGroupsXml[0].Items[4], _expectedGroupsXml[0].Items[5], 
                _expectedGroupsXml[0].Items[6], _expectedGroupsXml[0].Items[7], _expectedGroupsXml[0].Items[8], group09 });

            VerifyResult groupingResult = (VerifyResult)(_groupingVerifier.Verify(_expectedGroupsXml, _xmlCvs.View.Groups));
            if (groupingResult.Result != TestResult.Pass)
            {
                LogComment(groupingResult.Message);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult RemoveItemsAndVerifyXml()
        {
            Status("RemoveItemsAndVerifyXml");
            XmlNode removeChild = _xmlCountryCollection[0];
            removeChild.ParentNode.RemoveChild(removeChild);

            _expectedGroupsXml[0] = new ExpectedGroup("Republic", new object[] { 
                _expectedGroupsXml[0].Items[1], _expectedGroupsXml[0].Items[2], 
                _expectedGroupsXml[0].Items[3], _expectedGroupsXml[0].Items[4], _expectedGroupsXml[0].Items[5], 
                _expectedGroupsXml[0].Items[6], _expectedGroupsXml[0].Items[7], _expectedGroupsXml[0].Items[8], 
                _expectedGroupsXml[0].Items[9] });

            VerifyResult groupingResult = (VerifyResult)(_groupingVerifier.Verify(_expectedGroupsXml, _xmlCvs.View.Groups));
            if (groupingResult.Result != TestResult.Pass)
            {
                LogComment(groupingResult.Message);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}




