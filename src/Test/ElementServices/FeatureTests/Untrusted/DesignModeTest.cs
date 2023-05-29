// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.ComponentModel;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.DesignerPropertiesTest
{

    /// <summary>
    /// This is a check box that maps its checked property
    /// to the IsInDesignMode flag.  It does this using code
    /// instead of data binding or templates to verify that
    /// code values work.
    /// </summary>
    public class DerivedCheckBox : CheckBox
    {
        /// <summary>
        /// </summary>        
        public DerivedCheckBox()
        {
            IsChecked = DesignerProperties.GetIsInDesignMode(this);
        }

        /// <summary>
        /// </summary>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == DesignerProperties.IsInDesignModeProperty)
            {
                IsChecked = DesignerProperties.GetIsInDesignMode(this);
            }
        }
    }
    
    /// <summary>
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class DesignModeTest : IHostedTest
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("0" , @"PropertyEngine", TestCaseSecurityLevel.FullTrust, @"Verify behavior of IsInDesignMode Property")]
        [TestCaseSupportFile("MainWindow.xaml")]
        [TestCaseSupportFile("FrameContent.xaml")]
        [TestCaseSupportFile("UserControlContent.xaml")]
        public void RunTest() 
        {

            //Compile and Launch an application.
            GenericCompileHostedCase.RunCase(this, "EntryPoint",
                HostType.Window,null, null, new string[] {"MainWindow.xaml","FrameContent.xaml", "UserControlContent.xaml"});            
        }

        /// <summary>
        /// This is the entry point for the Compile Application
        /// </summary>
        public void EntryPoint()
        {

            Assembly asm = Assembly.GetEntryAssembly();

            CoreLogger.LogStatus("Log: " + asm.GetName().Name);


            Type t = asm.GetType("Avalon.Test.CoreUI.DesignerPropertiesTest.MainWindow");

            if (t == null)
            {
                throw new Exception("The test case was not running properly.");
            }
            
            // First scenario, verify the proeprty is not
            // marked as serializable.

            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(
                DesignerProperties.IsInDesignModeProperty, t);

            if (dpd.SerializationVisibility != DesignerSerializationVisibility.Hidden)
            {
                CoreLogger.LogTestResult(false, "IsInDesignMode property should be hidden from serialization.");
            }

            // Second scenario, verify the property does the
            // right thing.
            object o = Activator.CreateInstance(t);

            ((Window)o).Loaded += OnWindowLoaded; 

            ((Window)o).Show();


            // 






            _testContainer.RequestStartDispatcher();

        }

        delegate void Callback(Window window);

        /// <summary>
        /// When the window is loaded, we post a message to ourselves to do the
        /// validation.  This is important because we need to give
        /// the frames a chance to navigate.
        /// </summary>
        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.ApplicationIdle,
                new Callback(OnApplicationIdle), (Window)sender);
        }

        /// <summary>
        /// When our idle event is raised we can validate the state of
        /// our properties.
        /// </summary>
        private void OnApplicationIdle(Window w) 
        {
            // 

            DesignerProperties.SetIsInDesignMode(w, true);


            // Validate the values of each check boxes IsChecked property.  To
            // do this we recurse through all the children on the window, keeping
            // track of the expected value of the design mode property along the
            // way.  The property's expected value starts out true, and will switch
            // to false if there is a panel that explicitly sets the property.  We
            // also special case frames, since properties don't inherit outside of
            // a frame.

            bool passed = ValidateTree(
                true, /* expected final passing result */
                false,/* default expected value for property */
                "",   /* Starting string used to build a path to any failures for logging */
                w     /* object to start the validation with */
                );

            if (passed)
            {
                CoreLogger.LogTestResult(true, "Test Pass");
            }
            else
            {
                CoreLogger.LogTestResult(false, "Test failed");
            }

            _testContainer.EndTest();
        }

        /// <summary>
        /// Validates the UI tree to ensure the IsInDesignMode property is correct.
        /// </summary>
        private bool ValidateTree(
            bool passResult,
            bool expectedPropertyValue,
            string path,
            DependencyObject element)
        {

            // If a value is locally set for our design mode property, that value
            // becomes the new expected value.

            object localValue = element.ReadLocalValue(DesignerProperties.IsInDesignModeProperty);
            if (localValue != DependencyProperty.UnsetValue)
            {
                expectedPropertyValue = (bool)localValue;
            }

            // We use the content property of the group boxes in the tree
            // to build a path to the combo box.  We use this when logging
            // errors.

            GroupBox gb = element as GroupBox;
            if (gb != null) path += "->" + gb.Header.ToString();

            // Look for a check box.  If we find one, check the value of its
            // IsChecked property.  If it doesn't match what we expect, fail
            // the test.

            CheckBox cb = element as CheckBox;

            if (cb != null && cb.IsChecked != expectedPropertyValue)
            {
                CoreLogger.LogStatus(string.Format("Incorrect IsInDesignMode for '{0}' under path '{1}'.",
                    cb.Content, path));
                passResult = false;
            }

            // Now walk the children of this dependency object.  We still continue even if our check failed
            // so we can log all errors at once.

            int cnt = VisualTreeHelper.GetChildrenCount(element);
            for (int idx = 0; idx < cnt; idx++)
            {
                DependencyObject d = VisualTreeHelper.GetChild(element, idx);
                if (d != null) 
                {
                    passResult = ValidateTree(passResult, expectedPropertyValue, path, d);
                }    
            }

            return passResult;            
        }        


        /// <summary>
        /// Represents the current ITestContainer for this IHostedTest.
        /// </summary>
        ITestContainer IHostedTest.TestContainer
        {
            get
            {
                return _testContainer;
            }
            set
            {
                _testContainer = value;
            }
        }

        /// <summary>
        /// </summary>
        ITestContainer _testContainer = null;
        
        
    }
}
