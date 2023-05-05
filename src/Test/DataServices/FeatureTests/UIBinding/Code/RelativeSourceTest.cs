// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Testing RelativePath with /ItemsControl/TemplatedParent mixed
	/// </description>
	/// </summary>
    [Test(1, "Binding", "RelativeSourceTest")]
    public class RelativeSourceTest : XamlTest 
	{

        public RelativeSourceTest()
            : base("RelativeSource.xaml")

        {
	        RunSteps += new TestStep(Verify);
        }

        private int indexOf(DependencyObject parent, DependencyObject child)
        {
            bool foundIndex = false;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            int i;
            for(i = 0; i < count; i++)
            {
                if(child == VisualTreeHelper.GetChild(parent,i))
                {
                    foundIndex = true;
                    break;
                }
            }
            if (foundIndex) return i;
            else return -1;
        }

        TestResult Verify()
        {
            TextBlock tb;
            ArrayList ar = new ArrayList();

            ListBox lb = LogicalTreeHelper.FindLogicalNode(RootElement, "lb2") as ListBox;
            DependencyObject layout = Util.FindVisualByPropertyValue(Panel.IsItemsHostProperty, true, lb, false);
            int count = VisualTreeHelper.GetChildrenCount(layout);
            
            for(int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(layout,i);
                if ("500" != ((TextBlock)Util.FindVisualByID("t0", child)).Text)
                    ar.Add("Expected Visual: 500 - Actual: " + ((TextBlock)Util.FindVisualByID("t0", child)).Text);
                if ("content1" != ((TextBlock)Util.FindVisualByID("t1", child)).Text)
                    ar.Add("Expected Visual: content1 - Actual: " + ((TextBlock)Util.FindVisualByID("t1", child)).Text);
                if ("TableData 0" != ((TextBlock)Util.FindVisualByID("t2", child)).Text)
                    ar.Add("Expected Visual: TableData 0 -  Actual: " + ((TextBlock)Util.FindVisualByID("t2", child)).Text);

                tb = ((TextBlock)Util.FindVisualByID("t3", child));
                if (indexOf(layout, child) == 0)
                {
                    if ("Select1" != tb.Text)
                        ar.Add("Expected Visual: Select1 - Actual: " + ((TextBlock)Util.FindVisualByID("t3", child)).Text);
                }
                else if (indexOf(layout, child) == 1)
                {
                    if ("Select2" != tb.Text)
                        ar.Add("Expected Visual: Select2 - Actual: " + ((TextBlock)Util.FindVisualByID("t3", child)).Text);
                }
                else
                {
                    if ("content2" != tb.Text)
                        ar.Add("Expected Visual: Select2 - Actual: " + ((TextBlock)Util.FindVisualByID("t3", child)).Text);
                }
                if (indexOf(layout, child) == 0)
                {
                    if ("Select1" != tb.Text)
                        ar.Add("Expected Visual: Select1 - Actual: " + ((TextBlock)Util.FindVisualByID("t3", child)).Text);
                }
                else if (indexOf(layout, child) == 1)
                {
                    if ("Select2" != tb.Text)
                        ar.Add("Expected Visual: Select2 - Actual: " + ((TextBlock)Util.FindVisualByID("t3", child)).Text);
                }
                else
                {
                    if ("content2" != tb.Text)
                        ar.Add("Expected Visual: Select2 - Actual: " + ((TextBlock)Util.FindVisualByID("t3", child)).Text);
                }

                tb = ((TextBlock)Util.FindVisualByID("t4", child));
                if (indexOf(layout, child) == 0)
                {
                    if ("Select1" != tb.Text)
                        ar.Add("Expected Visual: Select1 - Actual: " + ((TextBlock)Util.FindVisualByID("t4", child)).Text);
                }
                else
                {
                    if ("TableData 0" != tb.Text)
                        ar.Add("Expected Visual: TableData 0 - Actual: " + ((TextBlock)Util.FindVisualByID("t4", child)).Text);
                }

                tb = ((TextBlock)Util.FindVisualByID("t5", child));
                if (indexOf(layout, child) < 2)
                {
                    if ("t5" != tb.Text)
                        ar.Add("Expected Visual: t5 - Actual: " + ((TextBlock)Util.FindVisualByID("t5", child)).Text);
                }
                else
                {
                    if ("Select3" != tb.Text)
                        ar.Add("Expected Visual: Select3 - Actual: " + ((TextBlock)Util.FindVisualByID("t5", child)).Text);
                }

                tb = ((TextBlock)Util.FindVisualByID("t6", child));
                if (indexOf(layout, child) < 2)
                {
                    if ("t6" != tb.Text)
                        ar.Add("Expected Visual: t5 - Actual: " + ((TextBlock)Util.FindVisualByID("t6", child)).Text);
                }
                else
                {
                    if ("TableData 0" != tb.Text)
                        ar.Add("Expected Visual: TableData 0 - Actual: " + ((TextBlock)Util.FindVisualByID("t6", child)).Text);
                }

                tb = ((TextBlock)Util.FindVisualByID("t7", child));
                if (indexOf(layout, child) < 2)
                {
                    if ("lb2" != tb.Text)
                        ar.Add("Expected Visual: t5 - Actual: " + ((TextBlock)Util.FindVisualByID("t7", child)).Text);
                }
                else
                {
                    if ("ic2" != tb.Text)
                        ar.Add("Expected Visual: TableData 0 - Actual: " + ((TextBlock)Util.FindVisualByID("t7", child)).Text);
                }

                tb = ((TextBlock)Util.FindVisualByID("t8", child));
                if (indexOf(layout, child) < 2)
                {
                    if ("listbox" != tb.Text)
                        ar.Add("Expected Visual: t5 - Actual: " + ((TextBlock)Util.FindVisualByID("t8", child)).Text);
                }
                else
                {
                    if ("lb2" != tb.Text)
                        ar.Add("Expected Visual: TableData 0 - Actual: " + ((TextBlock)Util.FindVisualByID("t8", child)).Text);
                }


            }
            ar.Sort();
            if (ar.Count > 0)
            {
                for (int i = 0; i < ar.Count; i++)
                    LogComment(ar[i].ToString());
                return TestResult.Fail;
            }
            else
            {
                return TestResult.Pass;
            }
        }
        }



	/// <summary>
	/// <description>
	/// Testing RelativePath with /ContentControl 
	/// </description>
	/// </summary>
	[Test(1, "Binding", "ContententControlRelativeSource")]
        public class ContententControlRelativeSource : XamlTest 
	{

        public ContententControlRelativeSource()
            : base("contentcontrolRelativeSource.xaml")

        {
           InitializeSteps += new TestStep(Init);
           RunSteps += new TestStep(Verify);
           RunSteps += new TestStep(ErrorCheck);
        }

        TestResult Init()
        {
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
	}


        TestResult ErrorCheck()
        {
            Binding b = new Binding();
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            b.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent, typeof(ItemsControl), 1);
            LogComment("The expected Exception wasn't of type InvalidOperationException"); 
            return TestResult.Fail;
        }
        
        TestResult Verify()
        {

            ListBox lb = LogicalTreeHelper.FindLogicalNode(RootElement, "listbox") as ListBox;
            DependencyObject layout = Util.FindVisualByPropertyValue(Panel.IsItemsHostProperty, true, lb, false);

                if ("Content" != ((TextBlock)Util.FindVisualByID("t1", VisualTreeHelper.GetChild(layout, 0))).Text)
                {
                    LogComment("Expected Visual: Content - Actual: " + ((TextBlock)Util.FindVisualByID("t1", VisualTreeHelper.GetChild(layout, 0))).Text);
                }

                if ("Content1" != ((TextBlock)Util.FindVisualByID("t2", VisualTreeHelper.GetChild(layout, 0))).Text)
                {
                    LogComment("Expected Visual: Content1 - Actual: " + ((TextBlock)Util.FindVisualByID("t1", VisualTreeHelper.GetChild(layout, 0))).Text);
                }

                if ("ListBoxItem" != ((TextBlock)Util.FindVisualByID("t3", VisualTreeHelper.GetChild(layout, 0))).Text)
                {
                    LogComment("Expected Visual: ListBoxItem - Actual: " + ((TextBlock)Util.FindVisualByID("t1", VisualTreeHelper.GetChild(layout, 0))).Text);
                }


                if (((TextBlock)Util.FindVisualByID("t4", VisualTreeHelper.GetChild(layout, 0))).Text.EndsWith("CollecionViewSource"))
                {
                    LogComment("Expected Visual: CollecionViewSource - Actual: " + ((TextBlock)Util.FindVisualByID("t1", VisualTreeHelper.GetChild(layout, 0))).Text);
                }

            return TestResult.Pass;
        }



}

}
