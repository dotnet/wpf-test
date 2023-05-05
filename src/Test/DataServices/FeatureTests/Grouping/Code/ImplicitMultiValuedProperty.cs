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

    /// It categorizes a few data item in more than one group by using the ValueConverter property of
    /// PropertyGroupDescription and return a collection of groups.
    /// This test case covers both CLR object and XmlDataProvider.
    /// </description>
    /// </summary>
    [Test(2, "Grouping", "ImplicitMultiValuedProperty")]
    public class ImplicitMultiValuedProperty : XamlTest
    {
        private CollectionViewSource _clrCvs;
        private ObservableCollection<CountryWithExtraInfo> _countries;
        private GroupingVerifier _groupingVerifier;
        private ReadOnlyObservableCollection<XmlNode> _xmlCountryCollection;
        private CollectionViewSource _xmlCvs;
        private ExpectedGroup[] _expectedGroups;

        public ImplicitMultiValuedProperty()
            : base(@"ImplicitMultiValuedProperty.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            // Clr
            RunSteps +=new TestStep(VerifyGroupsClr);
            RunSteps += new TestStep(AddItemsAndVerifyClr);
            RunSteps += new TestStep(RemoveItemsAndVerifyClr);
            // Xml
            RunSteps += new TestStep(VerifyGroupsXml);
            RunSteps += new TestStep(AddItemsAndVerifyXml);
            RunSteps += new TestStep(RemoveItemsAndVerifyXml);
        }

        TestResult Setup()
        {
            Status("Setup");

            CountriesDataSource clrCountries = (CountriesDataSource)(this.RootElement.Resources["clrCountries"]);
            _countries = clrCountries.Countries;
            XmlDataProvider xmlCountries = (XmlDataProvider)(this.RootElement.Resources["xmlCountries"]);
            _xmlCountryCollection = (ReadOnlyObservableCollection<XmlNode>)DataSourceHelper.WaitForData(xmlCountries);

            _clrCvs = (CollectionViewSource)(this.RootElement.Resources["clrCvs"]);
            _xmlCvs = (CollectionViewSource)(this.RootElement.Resources["xmlCvs"]);

            _groupingVerifier = new GroupingVerifier();

            return TestResult.Pass;
        }

        TestResult VerifyGroupsClr()
        {
            Status("VerifyGroupsClr");
            WaitForPriority(DispatcherPriority.SystemIdle);
            if (!VerifyGroups(_countries, _clrCvs)) { return TestResult.Fail; }            
            return TestResult.Pass;
        }

        TestResult AddItemsAndVerifyClr()
        {
            Status("AddItemsAndVerifyClr");
            CountryWithExtraInfo newCountry = new CountryWithExtraInfo("New country", "New capital", new DateTime(1990, 12, 12), 9999999, GovernmentType.Democracy, "Asia", "Eastern Hemisphere");
            _countries.Add(newCountry);
            WaitForPriority(DispatcherPriority.SystemIdle);
            if (!VerifyAddItems(_countries, _clrCvs)) { return TestResult.Fail; }
            return TestResult.Pass;
        }

        TestResult RemoveItemsAndVerifyClr()
        {
            Status("RemoveItemsAndVerifyClr");
            _countries.RemoveAt(16);
            WaitForPriority(DispatcherPriority.SystemIdle);
            if (!VerifyGroups(_countries, _clrCvs)) { return TestResult.Fail; }
            return TestResult.Pass;
        }

        TestResult VerifyGroupsXml()
        {
            Status("VerifyGroupsXml");
            WaitForPriority(DispatcherPriority.SystemIdle);
            if (!VerifyGroups(_xmlCountryCollection, _xmlCvs)) { return TestResult.Fail; }            
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
            yearElement.InnerText = "1990";
            XmlElement monthElement = document.CreateElement("Month");
            monthElement.InnerText = "12";
            XmlElement dayElement = document.CreateElement("Day");
            dayElement.InnerText = "12";
            independencyDayElement.AppendChild(yearElement);
            independencyDayElement.AppendChild(monthElement);
            independencyDayElement.AppendChild(dayElement);
            XmlElement populationElement = document.CreateElement("Population");
            populationElement.InnerText = "9999999";
            XmlElement governmentElement = document.CreateElement("Government");
            governmentElement.InnerText = "Democracy";
            XmlElement regionElement = document.CreateElement("Region");
            regionElement.InnerText = "Asia";
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

            WaitForPriority(DispatcherPriority.SystemIdle);
            if (!VerifyAddItems(_xmlCountryCollection, _xmlCvs)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        TestResult RemoveItemsAndVerifyXml()
        {
            Status("RemoveItemsAndVerifyXml");
            XmlNode removeChild = _xmlCountryCollection[16];
            removeChild.ParentNode.RemoveChild(removeChild);
            WaitForPriority(DispatcherPriority.SystemIdle);
            if (!VerifyGroups(_xmlCountryCollection, _xmlCvs)) { return TestResult.Fail; }
            return TestResult.Pass;
        }

        #region Helper Methods
        // Unlike in the NonUniformSubgrouping test case, the group types for the XML scenario are not all
        // string, they are whatever type was returned by the Converter. This allowed me to reuse the
        // verification logic I wrote for CLR objects.
        private bool VerifyGroups(IList list, CollectionViewSource cvs)
        {
            ExpectedGroup group0 = new ExpectedGroup(BeenThere.BeenThereIn1980s, new object[] { list[0], list[5], list[10] });
            ExpectedGroup group1 = new ExpectedGroup(BeenThere.BeenThereIn1990s, new object[] { list[1], list[10] });
            ExpectedGroup group2 = new ExpectedGroup(BeenThere.BeenThereIn2000s, new object[] { list[1], list[5], list[10] });
            ExpectedGroup group3 = new ExpectedGroup(null, new object[] { list[2], list[11] });
            ExpectedGroup group4 = new ExpectedGroup("San Marino", new object[] { list[3] });
            ExpectedGroup group5 = new ExpectedGroup(BeenThere.NeverBeenThere, new object[] { list[4], list[7], list[8], list[9], list[14], list[15] });
            ExpectedGroup group6 = new ExpectedGroup(list[6], new object[] { list[6] });
            ExpectedGroup group7 = new ExpectedGroup(list[12], new object[] { list[12] });
            ExpectedGroup group8 = new ExpectedGroup("El Salvador", new object[] { list[13] });

            _expectedGroups = new ExpectedGroup[] { group0, group1, group2, group3, group4, group5, group6, group7, group8 };

            VerifyResult groupingResult = (VerifyResult)(_groupingVerifier.Verify(_expectedGroups, cvs.View.Groups));
            if (groupingResult.Result != TestResult.Pass)
            {
                LogComment(groupingResult.Message);
                return false;
            }

            return true;
        }

        private bool VerifyAddItems(IList list, CollectionViewSource cvs)
        {
            ExpectedGroup group0 = new ExpectedGroup(BeenThere.BeenThereIn1980s, new object[] { list[0], list[5], list[10], list[16] });
            ExpectedGroup group1 = new ExpectedGroup(BeenThere.BeenThereIn1990s, new object[] { list[1], list[10], list[16] });
            ExpectedGroup group2 = new ExpectedGroup(BeenThere.BeenThereIn2000s, new object[] { list[1], list[5], list[10], list[16] });

            _expectedGroups[0] = group0;
            _expectedGroups[1] = group1;
            _expectedGroups[2] = group2;

            VerifyResult groupingResult = (VerifyResult)(_groupingVerifier.Verify(_expectedGroups, cvs.View.Groups));
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




