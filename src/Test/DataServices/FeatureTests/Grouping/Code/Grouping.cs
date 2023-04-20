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
using Microsoft.Test.Discovery;
using System.ComponentModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{

    /// <summary>
    /// <description>
    /// 


    [Test(1, "Grouping", "MultiLevelGroupingBVT")]
    public class MultiLevelGroupingBVT : GroupingBaseTest
    {
        public MultiLevelGroupingBVT()
            : base(@"multilevelgroup.xaml")
        {
        }

        protected override TestResult Init()
        {
            intArray = createArray(4,2,1,1,5,2,2,1);

            // apply data grouping
            ItemsControl ic = (ItemsControl)Util.FindElement(RootElement, "itemscontrol");
            ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(ic.ItemsSource);
            using (lcv.DeferRefresh())
            {
                PropertyGroupDescription pgd;

                pgd = new PropertyGroupDescription();
                pgd.PropertyName = "Team";
                lcv.GroupDescriptions.Add(pgd);

                pgd = new PropertyGroupDescription();
                pgd.PropertyName = "Title";
                lcv.GroupDescriptions.Add(pgd);
            }

            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }

    }

    /// <summary>
    /// <description>
    /// 


    [Test(1, "Grouping", "ImplicitGroupingBvt")]
    public class ImplicitGroupingBvt : GroupingBaseTest
    {
        public ImplicitGroupingBvt() : base(@"Implicitgroup.xaml")
        {
        }

        protected override TestResult Init()
        {
            intArray = new int[] {3, 1, 2, 2};
            intArray = createArray(3, 1, 2, 2);

            // apply data grouping
            ItemsControl ic = (ItemsControl)Util.FindElement(RootElement, "itemscontrol");
            ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(ic.ItemsSource);
            PropertyGroupDescription pgd = new PropertyGroupDescription();
            pgd.PropertyName = "Extension";
            lcv.GroupDescriptions.Add(pgd);

            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }

    }

    /// <summary>
    /// <area>Grouping</area>

    /// <description>
    /// 


    public abstract class GroupingBaseTest : XamlTest
    {
        protected int[] intArray;

        public GroupingBaseTest() : this(@"Implicitgroup.xaml")
        {
        }
        public GroupingBaseTest (string filename) : base(filename)
        {
            InitializeSteps += new TestStep(Init);
            RunSteps += new TestStep(VerifyGrouping);
        }

        protected abstract TestResult Init();

        protected int[] createArray(params int[] intArray)
        {
            return intArray;
        }

        private TestResult VerifyGrouping()
        {
             FrameworkElement[] elements = Util.FindElements(RootElement, "cnt");
             if (elements.Length != intArray.Length)
             {
                 LogComment("Incorrect amount of groups!  Expected:" + intArray.Length + "; Actual: " + elements.Length);
                 return TestResult.Fail;
             }

             for (int i = 0; i < intArray.Length; i++)
             {
                 TextBlock groupCount = elements[i] as TextBlock;
                 if (groupCount.Text != intArray[i].ToString())
                 {
                     LogComment("Items Grouped is incorrect Expected: " + intArray[i].ToString() + " Actual: " + groupCount.Text);
                     return TestResult.Fail;
                 }
             }

            return TestResult.Pass;
        }

    }
}




