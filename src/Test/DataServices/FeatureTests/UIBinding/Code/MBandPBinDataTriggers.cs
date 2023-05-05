// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Testing MultiBinding and PriorityBinding inside the DataTrigger 
    ///    and MultiDataTrigger.
    /// </description>
    /// </summary>

    [Test(1, "Binding", "MBandPBinDataTriggers")]
    public class MBandPBinDataTriggers : XamlTest
    {
        private ListBox _lb;
        private Style _listBoxItemStyle;
        private Style _listBoxItemStyleMulti;
        private Style _listBoxItemStylewithPB;
        private Style _listBoxItemStylewithPBwithmultidatatrigger;
        private ListBoxItem _li;

        public MBandPBinDataTriggers()
            : base(@"MBandPBinDataTriggers.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(MBinDatatrigger);
            RunSteps += new TestStep(MBinMultiDatatrigger);
            RunSteps += new TestStep(PBinDatatrigger);
            RunSteps += new TestStep(PBinMultiDatatrigger);
        }

        private TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            //Getting all the styles.
            _listBoxItemStyle = RootElement.Resources["listBoxItemStyle"] as Style;
            _listBoxItemStyleMulti = RootElement.Resources["listBoxItemStyleMulti"] as Style;
            _listBoxItemStylewithPB = RootElement.Resources["listBoxItemStylewithPB"] as Style;
            _listBoxItemStylewithPBwithmultidatatrigger = RootElement.Resources["listBoxItemStylewithPBwithmultidatatrigger"] as Style;

            if (_listBoxItemStyle == null)
            {
                LogComment("Fail - Unable to reference Style listBoxItemStyle");
                return TestResult.Fail;
            }
            if (_listBoxItemStyleMulti == null)
            {
                LogComment("Fail - Unable to reference Style listBoxItemStyleMulti");
                return TestResult.Fail;
            }
            if (_listBoxItemStylewithPB == null)
            {
                LogComment("Fail - Unable to reference Style listBoxItemStylewithPB");
                return TestResult.Fail;
            }
            if (_listBoxItemStylewithPBwithmultidatatrigger == null)
            {
                LogComment("Fail - Unable to reference Style listBoxItemStylewithPBwithmultidatatrigger");
                return TestResult.Fail;
            }
            // Getting the ListBox.
            _lb = LogicalTreeHelper.FindLogicalNode(RootElement, "lb") as ListBox;
            if (_lb == null)
            {
                LogComment("Fail - Not able to reference ListBox lb");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private TestResult MBinDatatrigger()
        {
            
            WaitForPriority(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            //Adding Style to the ListBox.
            return ApplyStyleandverify(_listBoxItemStyle);
        }
        private TestResult MBinMultiDatatrigger()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            //Adding Style to the ListBox.
            return ApplyStyleandverify(_listBoxItemStyleMulti);
            
        }
        private TestResult PBinDatatrigger()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            //Adding Style to the ListBox.
            return ApplyStyleandverify(_listBoxItemStylewithPB);
        }

        private TestResult PBinMultiDatatrigger()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            //Adding Style to the ListBox.
            return ApplyStyleandverify(_listBoxItemStylewithPBwithmultidatatrigger);  
        }
        private TestResult ApplyStyleandverify(Style s)
        {
            Status("Adding Style to the ListBox.");
            _lb.ItemContainerStyle = (Style)s;
            //Call function to verify the ListBoxItem color.
            return VerifyListboxItemcolor();
        }

        private TestResult VerifyListboxItemcolor()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            // Get the ItemsCollection.
            System.Windows.Controls.ItemCollection coll = _lb.Items;
            // Get the ListBoxItem.
            for (int i = 0; i < coll.Count; ++i)
            {
               object lio = _lb.ItemContainerGenerator.ContainerFromItem(coll[i]);
               _li = lio as ListBoxItem;
               if ( ((_li.Content as Writer).FirstName == "Carl"))
               {
                   Status("Got the ListBoxItem");
                   break;
               }
               continue;
            }
            Status("Check the BackGroundcolor of the ListBoxItem");
            // See if the ListBoxItem color is Brown
            if ((_li.Background) == ((Brushes.Brown)))
            {
                LogComment("ListBoxItem Background color is brown" + _li.Background.ToString());
                return TestResult.Pass;
            }
            else
            {
                LogComment("ListBoxItem Background color is not brown" + _li.Background.ToString());
                return TestResult.Fail;
            }
        }
    }
}


