// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using System.Threading;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// This test verify that DisplayMemberPath works with CLR, DataSet, XML data
	/// Changes DisplayMemberPath value and changes DataConext 
	/// </description>
	/// </summary>

    [Test(0, "Controls", "DisplayMemberPathTest")]
    public class DisplayMemberPathTest : XamlTest 
	{
        ListBox _ds_listbox;
        ListBox _xml_listbox;
        ListBox _clr_listbox;

        public DisplayMemberPathTest()
            : base("displaymemberpathtest.xaml")
		{
            InitializeSteps += new TestStep(datasetTest_InitializeSteps);
            RunSteps += new TestStep(InitialVerify);
            RunSteps += new TestStep(ChangeDisplayMemberPath);
            RunSteps += new TestStep(VerifyChangeDisplayMemberPath);
            RunSteps += new TestStep(ChangeDataContext);
            RunSteps += new TestStep(VerifyChangeDataContext);
            RunSteps += new TestStep(SetItemTemplate);
        }
        TestResult datasetTest_InitializeSteps()
        {
            _ds_listbox = LogicalTreeHelper.FindLogicalNode(RootElement, "lb3") as ListBox;
            if (_ds_listbox == null)
                return TestResult.Fail;
            _clr_listbox = LogicalTreeHelper.FindLogicalNode(RootElement, "lb2") as ListBox;
            if (_clr_listbox == null)
                return TestResult.Fail;
            _xml_listbox = LogicalTreeHelper.FindLogicalNode(RootElement, "lb1") as ListBox;
            if (_xml_listbox == null)
                return TestResult.Fail;

            _ds_listbox.DataContext = new DataTableSource();
            _ds_listbox.DisplayMemberPath = "Date";
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }
        TestResult InitialVerify()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);

            Util.WaitForItemsControlPopulation(_ds_listbox, 30);
            Util.WaitForItemsControlPopulation(_xml_listbox, 30);
            Util.WaitForItemsControlPopulation(_clr_listbox, 30);

            FrameworkElement[] clr_visualelements = Util.FindDataVisuals(_clr_listbox, _clr_listbox.ItemsSource);

            // Need to retry a few times on slow machines since the visual may not exist yet.
            // This is needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && clr_visualelements.Length == 0)
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
                clr_visualelements = Util.FindDataVisuals(_clr_listbox, _clr_listbox.ItemsSource);
            }


            if (((TextBlock)clr_visualelements[0]).Text.Length == 0)
            {
                LogComment("DisplayMemberPath failed with CLR collection");
                return TestResult.Fail;
            }

            FrameworkElement[] ds_visualelements = Util.FindDataVisuals(_ds_listbox, _ds_listbox.ItemsSource);
            if (((TextBlock)ds_visualelements[0]).Text.Length == 0)
            {
                LogComment("DisplayMemberPath failed with ADO DataTable");
                return TestResult.Fail;
            }
            FrameworkElement[] xml_visualelements = Util.FindDataVisuals(_xml_listbox, _xml_listbox.ItemsSource);
            if (((TextBlock)xml_visualelements[0]).Text.Length == 0)
            {
                LogComment("DisplayMemberPath failed with XML collection");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
        TestResult ChangeDisplayMemberPath()
        {
            Status("Changing DisplayMemberPath");
            _ds_listbox.DisplayMemberPath = "Name";
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }
        TestResult VerifyChangeDisplayMemberPath()
        {
            Status("Verifying the change to DisplayMemberPath");
            FrameworkElement[] ds_visualelements = Util.FindDataVisuals(_ds_listbox, _ds_listbox.ItemsSource);
            if (((TextBlock)ds_visualelements[0]).Text != "TableData 0")
            {
                LogComment("Expected: 'TableData 0' Actual: '" + ((TextBlock)ds_visualelements[0]).Text + "'");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
        TestResult ChangeDataContext()
        {
            Status("Changing DataContext");
            _ds_listbox.DataContext = _clr_listbox.ItemsSource;
            WaitForPriority(DispatcherPriority.Render);
            return TestResult.Pass;
        }
        TestResult VerifyChangeDataContext()
        {
            FrameworkElement[] ds_visualelements = Util.FindDataVisuals(_ds_listbox, _ds_listbox.ItemsSource);
            if (((TextBlock)ds_visualelements[0]).Text != "Beatriz")
            {
                LogComment("Expected: 'Beatriz' Actual: '" + ((TextBlock)ds_visualelements[0]).Text + "'");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
        TestResult SetItemTemplate()
        {
            Status("Setting ItemTemplate");
            DataTemplate itemTemplate = new DataTemplate();
            FrameworkElementFactory text = new FrameworkElementFactory(typeof(TextBlock));
            text.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            itemTemplate.VisualTree = text;

            //Shouldn't be able to set ItemTemplate or ItemTemplateSelector with DisplayMemberPath set.
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            _clr_listbox.ItemTemplate = itemTemplate;

            return TestResult.Pass;
        }
    }
}
