// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Collections;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// This Test case applies styles based on DataType.  It cover 3 cases
    /// 1) ClR Data types
    /// 2) XML Data types
    /// 3) Hierarchial data with MenuItems to poplulate the submenu's.
    /// 4) TODO: Should add cases to use DataSet
    /// </description>
    /// <relatedBugs>


    /// </relatedBugs>
    /// </summary>
    [Test(1, "Styles", "StyleByType")]
    public class StyleByType : XamlTest
    {
        private SolidColorBrush _devColor = Brushes.Cyan;
        private SolidColorBrush _pmColor = Brushes.Red;
        private SolidColorBrush _testColor = Brushes.Green;

        /// <summary>
        /// Tests the StyleSelector functionality.
        /// </summary>
        public StyleByType()
            : base(@"StyleByType.xaml")
        {
            RunSteps += new TestStep(ValidateObj);
            RunSteps += new TestStep(ValidateXML);

            RunSteps += new TestStep(ValidateHierarchy);
        }

        private string FindNode(XmlNodeList _xmlnode, string _searchstr)
        {
            for (int i = 0; i < _xmlnode.Count; i++)
            {
                for (int ii = 0; ii < _xmlnode[i].Attributes.Count; ii++)
                {
                    if (_xmlnode[i].Attributes[ii].InnerText == _searchstr)
                        return _xmlnode[i].Name;
                }
            }

            return "Didn't find Node";
        }

        private TestResult ValidateXML()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            Status("Validating XML");
            string strValue = "";
            XmlDataProvider xmlsrc = ((DockPanel)RootElement).FindResource("Library") as XmlDataProvider;
            XmlDocument _xmldoc = xmlsrc.Document;

            ListBox _listbox = LogicalTreeHelper.FindLogicalNode(RootElement, "lb1") as ListBox;

            // Needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && _listbox == null)
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
                _listbox = LogicalTreeHelper.FindLogicalNode(RootElement, "lb1") as ListBox;
            }

            FrameworkElement[] _visualCollection = Util.FindDataVisuals(_listbox, _listbox.ItemsSource);

            foreach (FrameworkElement viscol in _visualCollection)
            {
                UIElementCollection _visualDataCollection = ((StackPanel)viscol).Children as UIElementCollection;

                switch (_visualDataCollection.Count)
                {
                    case 1:
                        strValue = FindNode(_xmldoc.SelectNodes("root/Book"), ((TextBlock)_visualDataCollection[0]).Text);
                        break;

                    case 2:
                        strValue = FindNode(_xmldoc.SelectNodes("root/Magazine"), ((TextBlock)_visualDataCollection[0]).Text);
                        break;

                    case 3:
                        strValue = FindNode(_xmldoc.SelectNodes("root/CD"), ((TextBlock)_visualDataCollection[0]).Text);
                        break;
                    default:
                        LogComment("The visual collection count value was unexpected.  Count was '" + _visualDataCollection.Count +  "'");
                        return TestResult.Fail;

                }
                if (strValue == "Didn't find Node")
                {
                    LogComment("Cound find '" + ((TextBlock)_visualDataCollection[0]).Text + "'");
                    return TestResult.Fail;
                }
                else
                    Status("Found string value '" + ((TextBlock)_visualDataCollection[0]).Text + " in XML node '" + strValue + "'");

            }

            return TestResult.Pass;
        }

        private TestResult ValidateObj()
        {
            Status("Validating Obj");

            FrameworkElement[] _visualCollection;
            TestResult _result = TestResult.Pass;

            ListBox _listbox = LogicalTreeHelper.FindLogicalNode(RootElement, "lb2") as ListBox;

            //Checking Dev
            _visualCollection = FindListBoxItemsWithBackground(_listbox, _devColor);

            
            if (_visualCollection.Length != 4)
            {
                LogComment("ArrayLength of Dev's was incorrect. Expected: 4  Actual: " + _visualCollection.Length.ToString());
                _result = TestResult.Fail;
            }
            foreach (FrameworkElement dev in _visualCollection)
            {
                if (((Dev)((ListBoxItem)dev).Content).Title != "Dev" || ((ListBoxItem)dev).Background != Brushes.Cyan)
                {
                    LogComment("Expected: Dev.Title='Dev' Actual: " + ((Dev)((ListBoxItem)dev).Content).Title + "  Expected: ListBoxItem.Background=" + Brushes.Cyan + "  Actual: " + ((ListBoxItem)dev).Background.ToString());
                    _result = TestResult.Fail;
                }
            }

            //Check for test
            _visualCollection = FindListBoxItemsWithBackground(_listbox, _testColor);

            if (_visualCollection.Length != 3)
            {
                LogComment("ArrayLength of Test's was incorrect. Expected: 3  Actual: " + _visualCollection.Length.ToString());
                _result = TestResult.Fail;
            }
            foreach (FrameworkElement test in _visualCollection)
            {
                if (((Tester)((ListBoxItem)test).Content).Title != "Tester" || ((ListBoxItem)test).Background != Brushes.Green)
                {
                    LogComment("Expected: Tester.Title='Tester' Actual: " + ((Tester)((ListBoxItem)test).Content).Title + "  Expected: ListBoxItem.Background=" + Brushes.Green + "  Actual: " + ((ListBoxItem)test).Background.ToString());
                    _result = TestResult.Fail;
                }
            }

            //Check for PM
            _visualCollection = FindListBoxItemsWithBackground(_listbox, _pmColor);

            if (_visualCollection.Length != 2)
            {
                LogComment("ArrayLength of PM's was incorrect. Expected: 2  Actual: " + _visualCollection.Length.ToString());
                _result = TestResult.Fail;
            }

            foreach (FrameworkElement pm in _visualCollection)
            {
                if (((PM)((ListBoxItem)pm).Content).Title != "PM" || ((ListBoxItem)pm).Background != Brushes.Red)
                {
                    LogComment("Expected: PM.Title='PM' Actual: " + ((PM)((ListBoxItem)pm).Content).Title + "  Expected: ListBoxItem.Background=" + Brushes.Green + "  Actual: " + ((ListBoxItem)pm).Background.ToString());
                    _result = TestResult.Fail;
                }
            }

            return _result;
        }

        private TestResult ValidateHierarchy()
        {
            Status("Validating Hierarchical Data");

            WaitForPriority(DispatcherPriority.SystemIdle);

            ObjectDataProvider DSO = RootElement.FindResource("MLB") as ObjectDataProvider;

            int retryCount = 0;
            while ((DSO.Data == null) && (retryCount++ < 10))
            {
                Status("DSO.Data was null, retrying on slow machine...");
                Thread.Sleep(1000);
            }

            ListLeagueList _leaguelist = DSO.Data as ListLeagueList;
            Menu menu1 = (Menu)LogicalTreeHelper.FindLogicalNode(RootElement, "menu1");

            FrameworkElement[] _visualcollection = Util.FindDataVisuals(menu1, _leaguelist["National League"].Divisions[2].Teams);

            if (_visualcollection == null)
            {
                LogComment("Visual collection was null! ");
                return TestResult.Fail;
            }
            if (_visualcollection.Length!=5)
            {
                LogComment("Visual collection length - Expected: 5  Actual: " + _visualcollection.Length.ToString());
                return TestResult.Fail;
            }

            int i = 0;
            foreach (FrameworkElement visualElement in _visualcollection)
            {
                if (_leaguelist["National League"].Divisions[2].Teams[i].Name == ((TextBlock)visualElement).Text)
                {
                    i++;
                }
                else
                {
                    LogComment("Expected: '" + _leaguelist["National League"].Divisions[2].Teams[i].Name.ToString() + "'  Actual: '" + ((TextBlock)visualElement).Text.ToString() + "'");
                    return TestResult.Fail;
                }

            }

            return TestResult.Pass;
        }

        ListBoxItem[] FindListBoxItemsWithBackground(ListBox _listbox, SolidColorBrush testColor)
        {
            ArrayList results = new ArrayList();
            FrameworkElement[] listBoxItems = Util.FindElementsWithType(_listbox, typeof(ListBoxItem));
            foreach (FrameworkElement lbi in listBoxItems)
            {
                if (((ListBoxItem)lbi).Background.Equals(testColor))
                {
                    results.Add(lbi);
                }
            }

            return (ListBoxItem[])(results.ToArray(typeof(ListBoxItem)));
        }
    }


    public class EmployeeStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            FrameworkElement fe = container as FrameworkElement;
            if (item is Dev)        return (Style)fe.FindResource("DevContainerStyle");
            if (item is PM)         return (Style)fe.FindResource("PMContainerStyle");
            if (item is Tester)     return (Style)fe.FindResource("TesterContainerStyle");
            if (item is Employee)   return (Style)fe.FindResource("EmployeeContainerStyle");
            return null;
        }
    }
}

