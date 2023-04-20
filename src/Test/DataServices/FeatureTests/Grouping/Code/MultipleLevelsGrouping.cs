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

    /// This tests having a grouping hierarchy with more than one level.
    /// This test case covers both CLR object and XmlDataProvider.
    /// </description>
    /// </summary>
    [Test(1, "Collections", "MultipleLevelsGrouping")]
    public class MultipleLevelsGrouping : XamlTest
    {
        private CollectionViewSource _clrCvs;
        private ObservableCollection<CountryWithExtraInfo> _countries;
        private GroupingVerifier _groupingVerifier;
        private ReadOnlyObservableCollection<XmlNode> _xmlCountryCollection;
        private CollectionViewSource _xmlCvs;
        private ExpectedGroup[] _expectedGroups;

        public MultipleLevelsGrouping()
            : base(@"MultipleLevelsGrouping.xaml")
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

            WaitForPriority(DispatcherPriority.SystemIdle);

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
            if (!VerifyGroups(_countries, _clrCvs)) { return TestResult.Fail; }
            return TestResult.Pass;
        }

        TestResult AddItemsAndVerifyClr()
        {
            Status("AddItemsAndVerifyClr");
            CountryWithExtraInfo newCountry = new CountryWithExtraInfo("New country", "New capital", new DateTime(1990, 12, 12), 9999999, GovernmentType.Democracy, "Asia", "Empty group");
            _countries.Add(newCountry);
            if (!VerifyAddItems(_countries, _clrCvs)) { return TestResult.Fail; }
            return TestResult.Pass;
        }

        TestResult RemoveItemsAndVerifyClr()
        {
            Status("RemoveItemsAndVerifyClr");
            _countries.RemoveAt(16);
            if (!VerifyGroups(_countries, _clrCvs)) { return TestResult.Fail; }
            return TestResult.Pass;
        }

        TestResult VerifyGroupsXml()
        {
            Status("VerifyGroupsXml");
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
            hemisphereElement.InnerText = "Empty group";
            countryElement.AppendChild(countryNameElement);
            countryElement.AppendChild(capitalElement);
            countryElement.AppendChild(independencyDayElement);
            countryElement.AppendChild(populationElement);
            countryElement.AppendChild(governmentElement);
            countryElement.AppendChild(regionElement);
            countryElement.AppendChild(hemisphereElement);

            _xmlCountryCollection[0].ParentNode.AppendChild(countryElement);

            if (!VerifyAddItems(_xmlCountryCollection, _xmlCvs)) { return TestResult.Fail; }

            return TestResult.Pass;
        }

        TestResult RemoveItemsAndVerifyXml()
        {
            Status("RemoveItemsAndVerifyXml");
            XmlNode removeChild = _xmlCountryCollection[16];
            removeChild.ParentNode.RemoveChild(removeChild);
            if (!VerifyGroups(_xmlCountryCollection, _xmlCvs)) { return TestResult.Fail; }
            return TestResult.Pass;
        }

        #region Helper Methods
        private bool VerifyGroups(IList list, CollectionViewSource cvs)
        {
            // Empty group
            ExpectedGroup group0 = new ExpectedGroup("Empty group", new object[] { });

            // Eastern Hemisphere
            ExpectedGroup group10 = new ExpectedGroup("Middle East", new object[] { list[0], list[1] });
            ExpectedGroup group11 = new ExpectedGroup("Europe", new object[] { list[3], list[5] });
            ExpectedGroup group12 = new ExpectedGroup("Asia", new object[] { list[4], list[8], list[12] });
            ExpectedGroup group13 = new ExpectedGroup("Oceania", new object[] { list[6] });
            ExpectedGroup group14 = new ExpectedGroup("Africa", new object[] { list[7], list[9], list[10] });
            ExpectedGroup group15 = new ExpectedGroup("Southeast Asia", new object[] { list[11] });
            ExpectedGroup group1 = new ExpectedGroup("Eastern Hemisphere", new object[] { group10, group11, group12, group13, group14, group15 });

            // Western Hemisphere
            ExpectedGroup group20 = new ExpectedGroup("The Caribbean", new object[] { list[2] });
            ExpectedGroup group21 = new ExpectedGroup("Central America", new object[] { list[13] });
            ExpectedGroup group22 = new ExpectedGroup("South America", new object[] { list[14] });
            ExpectedGroup group23 = new ExpectedGroup("North America", new object[] { list[15] });
            ExpectedGroup group2 = new ExpectedGroup("Western Hemisphere", new object[] { group20, group21, group22, group23 });

            _expectedGroups = new ExpectedGroup[] { group0, group1, group2 };

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
            ExpectedGroup group00 = new ExpectedGroup("Asia", new object[] { list[16] });
            ExpectedGroup group0 = new ExpectedGroup("Empty group", new object[] { group00 });

            _expectedGroups[0] = group0;

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




