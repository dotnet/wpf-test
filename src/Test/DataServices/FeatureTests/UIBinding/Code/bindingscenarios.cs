// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test;
using System.Windows.Documents;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{

	/// <summary>
	/// <description>
	///  Checks the following:
	///  Binding to a Static Class.Property in separate binary
	///  Simple binding to Freezable
	///  ID Lookup
    ///  Coverage for certain methods of FrameworkElement and FrameworkContentElement
	/// </description>
	/// </summary>
    [Test(2, "Binding", "BindingScenarios")]
   public class BindingScenarios : XamlTest
	{
        Canvas _canvas;
        public BindingScenarios()
            : base(@"variousbindings.xaml")
        {
            InitializeSteps += new TestStep(Init);
            // Checking 3 things: 
            // Binding to a Static Class.Property in separate binary
            // Simple binding to Freezable
            // ID Lookup
            RunSteps += new TestStep(Verify);
            // Coverage for certain methods of FrameworkElement and FrameworkContentElement
            RunSteps += new TestStep(CoverageFE);
            RunSteps += new TestStep(CoverageFCE);
        }


        private TestResult Init()
        {
            Status("Init");
            WaitForPriority(DispatcherPriority.Normal);
            ObservableCollection<SortItem> ocs = new ObservableCollection<SortItem>();
            ocs.Add(new SortItem("AliceBlue", 0.5));
            ocs.Add(new SortItem("AntiqueWhite", 0.5));
            ocs.Add(new SortItem("Aqua", 0.5));
            ocs.Add(new SortItem("Azure", 0.5));
            ocs.Add(new SortItem("Beige", 0.5));
            ocs.Add(new SortItem("Bisque", 0.5));
            CollectionViewSource cvs = new CollectionViewSource();
            cvs.Source = ocs;
            _canvas = LogicalTreeHelper.FindLogicalNode(RootElement, "canvas") as Canvas;
            _canvas.DataContext = ocs;
            return TestResult.Pass;
        }

       private TestResult Verify()
        {
            Status("Verify");
            //Checking Id lookup
            Label label = LogicalTreeHelper.FindLogicalNode(_canvas, "label") as Label;
            if (label.Foreground != Brushes.Red)
            {
                LogComment("label forground Actual: " + label.Foreground + "  Expected: Red");
                return TestResult.Fail;
            }

            //Checking Binding to Freezable.
            DockPanel dockpanel = LogicalTreeHelper.FindLogicalNode(_canvas, "dp") as DockPanel;
            if (((LinearGradientBrush)dockpanel.Background).GradientStops[1].Color != ((SolidColorBrush)Brushes.AliceBlue).Color)
            {
                LogComment("Binding to freezable failed! ");
                return TestResult.Fail;
            }

            //Checking setting DataContext Static Class.Property in seperate Assemebly
            TextBlock textblock = LogicalTreeHelper.FindLogicalNode(_canvas, "text") as TextBlock;
            if (textblock.Text != "Howdy!")
            {
                LogComment("TextBlock Text Actual: " + textblock.Text + "  Expected: Howdy!");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult CoverageFE()
        {
            StackPanel sp = LogicalTreeHelper.FindLogicalNode(RootElement, "sp") as StackPanel;
            TextBlock tb = LogicalTreeHelper.FindLogicalNode(RootElement, "tb") as TextBlock;

            // SetBinding(System.Windows.DependencyProperty dp,string path)
            // add_DataContextChanged(System.Windows.DependencyPropertyChangedEventHandler value)
            sp.DataContextChanged += new DependencyPropertyChangedEventHandler(sp_DataContextChanged);
            sp.DataContext = sp;
            TestResult res = WaitForSignal("contextChanged");
            if (res != TestResult.Pass) 
            {
                LogComment("Fail - DataContextChanged event handler was not called");
                return res; 
            }
            tb.SetBinding(TextBlock.ForegroundProperty, "Background");
            if (tb.Foreground != sp.Background)
            {
                LogComment("Fail - Expected Foreground: " + sp.Background + ". Actual: " + tb.Foreground);
                return TestResult.Fail;
            }

            // remove_DataContextChanged(System.Windows.DependencyPropertyChangedEventHandler value)
            sp.DataContextChanged -= new DependencyPropertyChangedEventHandler(sp_DataContextChanged);
            sp.DataContext = null;
            TestResult res2 = WaitForSignal("contextChanged", 500);
            if (res2 == TestResult.Pass) 
            {
                LogComment("Fail - DataContextChanged event handler was not removed");
                return TestResult.Fail; 
            }

            // GetBindingExpression(System.Windows.DependencyProperty dp)
            BindingExpression be = tb.GetBindingExpression(TextBlock.ForegroundProperty);
            if (be.ParentBinding.Path.Path != "Background")
            {
                LogComment("Fail - Expected path: Background. Actual: " + be.ParentBinding.Path);
                return TestResult.Fail;
            }

           return TestResult.Pass;
        }

       private TestResult CoverageFCE()
       {
           StackPanel sp2 = LogicalTreeHelper.FindLogicalNode(RootElement, "sp2") as StackPanel;
           Paragraph para = LogicalTreeHelper.FindLogicalNode(RootElement, "para") as Paragraph;

           // add_DataContextChanged(System.Windows.DependencyPropertyChangedEventHandler value)
           sp2.DataContextChanged += new DependencyPropertyChangedEventHandler(sp_DataContextChanged);
           sp2.DataContext = sp2;
           TestResult res = WaitForSignal("contextChanged");
           if (res != TestResult.Pass)
           {
               LogComment("Fail - DataContextChanged event handler was not called");
               return res;
           }
           para.SetBinding(Paragraph.ForegroundProperty, "Background");
           if (para.Foreground != sp2.Background)
           {
               LogComment("Fail - Expected Foreground: " + sp2.Background + ". Actual: " + para.Foreground);
               return TestResult.Fail;
           }

           // remove_DataContextChanged(System.Windows.DependencyPropertyChangedEventHandler value)
           sp2.DataContextChanged -= new DependencyPropertyChangedEventHandler(sp_DataContextChanged);
           sp2.DataContext = null;
           TestResult res2 = WaitForSignal("contextChanged", 500);
           if (res2 == TestResult.Pass)
           {
               LogComment("Fail - DataContextChanged event handler was not removed");
               return TestResult.Fail;
           }

           // GetBindingExpression(System.Windows.DependencyProperty dp)
           BindingExpression be = para.GetBindingExpression(Paragraph.ForegroundProperty);
           if (be.ParentBinding.Path.Path != "Background")
           {
               LogComment("Fail - Expected path: Background. Actual: " + be.ParentBinding.Path);
               return TestResult.Fail;
           }

           return TestResult.Pass;
       }

       void sp_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
       {
           Signal("contextChanged", TestResult.Pass);
       }
        
    }
}
