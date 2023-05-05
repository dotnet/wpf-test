// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Media;
using System.ComponentModel;
using Microsoft.Test;
using System.Reflection;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
	/// <description>
	/// Provides coverage for bugs related to data sources.
    /// - Master/Detail over ADO data stops working after re-binding
	/// </description>
	/// </summary>
    [Test(2, "DataSources", TestCaseSecurityLevel.FullTrust, "SourcesBugsCoverage")]
    public class SourcesBugsCoverage : XamlTest
    {
        public SourcesBugsCoverage()
            : base(@"SourcesBugsCoverage.xaml")
        {
            
            RunSteps += new TestStep(MasterDetailADO);
        }

        private TestResult MasterDetailADO()
        {
            Status("MasterDetailADO");
            WaitForPriority(DispatcherPriority.SystemIdle);

            // setup
            ListBox lb = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb"));
            TextBlock tb1 = (TextBlock)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "tb1"));
            PlacesDataTable places = (PlacesDataTable)(this.RootElement.Resources["src2"]);

            // verify master-detail
            lb.SelectedIndex = 1;
            Util.AssertEquals("Redmond", tb1.Text);

            // cleanup
            MethodInfo _miCleanup = typeof(BindingOperations).GetMethod("Cleanup",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            GC.WaitForPendingFinalizers();
            while ((bool)_miCleanup.Invoke(null, null))
            {
                GC.WaitForPendingFinalizers();
            }

            // rebind
            Binding binding = new Binding();
            binding.Source = places;
            lb.ClearValue(ListBox.ItemsSourceProperty);
            BindingOperations.SetBinding(lb, ListBox.ItemsSourceProperty, binding);

            // verify master-detail 
            lb.SelectedIndex = 2;
            Util.AssertEquals("Bellevue", tb1.Text);

            return TestResult.Pass;
        }
    }
}

