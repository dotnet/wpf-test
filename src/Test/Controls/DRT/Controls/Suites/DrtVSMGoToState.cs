// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using System.Windows.Shapes;

namespace DRT
{
    public class DrtVSMGoToStateSuite : DrtTestSuite
    {
        public DrtVSMGoToStateSuite()
            : base("VSMGoToState")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            if (!_isPageLoaded)
            {
                string fileName = DRT.BaseDirectory + "DrtVSMGoToState.xaml";
                LoadXamlPage(fileName);
                _isPageLoaded = true;
            }
            else
            {
                Keyboard.Focus(null);
            }

            if (!DRT.KeepAlive)
            {
                List<DrtTest> tests = new List<DrtTest>();

                tests.Add(new DrtTest(TestMouseOver));
                tests.Add(new DrtTest(TestDisabled));
                tests.Add(new DrtTest(TestPressed));
                tests.Add(new DrtTest(TestFocused));
                tests.Add(new DrtTest(TestElementState));
                tests.Add(new DrtTest(TestValidation));
                tests.Add(new DrtTest(TestUnloadCompleted));

                return tests.ToArray();
            }
            else
            {
                return new DrtTest[] { };
            }
        }

        List<Control> _allControls = new List<Control>();
        Button _theButton;
        Grid _mainGrid;
        TextBlock _theTextBlock;
        Rectangle _elementStateRect;
        TextBox _validation1, _validation2;
        ValidationData _validationData;
        private void LoadXamlPage(string fileName)
        {
            System.IO.Stream stream = File.OpenRead(fileName);
            Visual root = (Visual)XamlReader.Load(stream);
            InitTree(root);

            if (DRT.KeepAlive)
            {
                FrameworkElement rootBorder = (FrameworkElement)DRT.FindVisualByID("Root_Border", root);
                DRT.MainWindow.SizeToContent = SizeToContent.Manual;
                Matrix deviceTransform = DRT.MainWindow.CompositionTarget.TransformToDevice;
                Point pt = deviceTransform.Transform(new Point(rootBorder.Width, rootBorder.Height));
                DrtBase.SetWindowPos(DRT.MainWindow.Handle, IntPtr.Zero, 0, 0, (int)pt.X, (int)pt.Y, DrtBase.SWP_NOMOVE | DrtBase.SWP_NOZORDER);
                rootBorder.ClearValue(FrameworkElement.WidthProperty);
                rootBorder.ClearValue(FrameworkElement.HeightProperty);
            }

            DRT.Show(root);

            DrtBase.WaitForRender();

            _allControls.Add((Control)DRT.FindVisualByID("theButton", root));
            _allControls.Add((Control)DRT.FindVisualByID("theRepeatButton", root));
            _allControls.Add((Control)DRT.FindVisualByID("theToggleButton", root));
            _allControls.Add((Control)DRT.FindVisualByID("theCheckBox", root));
            _allControls.Add((Control)DRT.FindVisualByID("theRadioButton", root));
            _allControls.Add((Control)DRT.FindVisualByID("theListBoxItem", root));
            _allControls.Add((Control)DRT.FindVisualByID("theThumb", root));
            _allControls.Add((Control)DRT.FindVisualByID("theScrollBar", root));
            _allControls.Add((Control)DRT.FindVisualByID("theComboBox", root));
            _allControls.Add((Control)DRT.FindVisualByID("theSlider", root));
            _allControls.Add((Control)DRT.FindVisualByID("theTextBox", root));
            _allControls.Add((Control)DRT.FindVisualByID("thePasswordBox", root));
            _allControls.Add((Control)DRT.FindVisualByID("theProgressBar", root));
            _allControls.Add((Control)DRT.FindVisualByID("theProgressBarI", root));

            _theButton = (Button)DRT.FindVisualByID("theButton", root);
            _mainGrid = (Grid)DRT.FindVisualByID("MainGrid", root);
            _theTextBlock = (TextBlock)DRT.FindVisualByID("theTextBlock", root);
            _elementStateRect = (Rectangle)DRT.FindVisualByID("elementStateRect", root);
            _validation1 = (TextBox)DRT.FindVisualByID("validation1", root);
            _validation2 = (TextBox)DRT.FindVisualByID("validation2", root);
            
            _validationData = new ValidationData();
            Binding binding = new Binding("ValidationDP1");
            binding.Source = _validationData;
            binding.ValidationRules.Add(new VSMValidationRule());
            _validation1.SetBinding(TextBox.TextProperty, binding);
            
            binding = new Binding("ValidationDP2");
            binding.Source = _validationData;
            binding.ValidationRules.Add(new VSMValidationRule());
            _validation2.SetBinding(TextBox.TextProperty, binding);
        }

        private void InitTree(DependencyObject root)
        {

        }

        private void TestMouseOver()
        {
            foreach (var c in _allControls)
            {
                if (!(c is ProgressBar))
                {
                    DRT.MoveMouse(DRT.RootElement as UIElement, 0.0, 0.0);
                    DrtBase.WaitForRender();
                    AssertInState(c, "CommonStates", "Normal");
                    DRT.MoveMouse(c, 0.5, 0.5);
                    DrtBase.WaitForRender();
                    AssertInState(c, "CommonStates", "MouseOver");
                }
            }
        }

        private void TestDisabled()
        {
            DRT.MoveMouse(DRT.RootElement as UIElement, 0.0, 0.0);
            DrtBase.WaitForRender();
            
            foreach (var c in _allControls)
            {
                if (!(c is ProgressBar))
                {
                    AssertInState(c, "CommonStates", "Normal");
                    c.IsEnabled = false;
                    DrtBase.WaitForRender();
                    AssertInState(c, "CommonStates", "Disabled");
                    c.IsEnabled = true;
                    DrtBase.WaitForRender();
                    AssertInState(c, "CommonStates", "Normal");
                }
            }
        }

        private void TestPressed()
        {
            foreach (var c in _allControls)
            {
                if ((c is ButtonBase))
                {
                    DRT.MoveMouse(DRT.RootElement as UIElement, 0.0, 0.0);
                    DrtBase.WaitForRender();
                    AssertInState(c, "CommonStates", "Normal");
                    DRT.MoveMouse(c, 0.5, 0.5);
                    DrtBase.WaitForRender();
                    AssertInState(c, "CommonStates", "MouseOver");
                    DRT.MouseButtonDown();
                    DrtBase.WaitForRender();
                    AssertInState(c, "CommonStates", "Pressed");
                    DRT.MouseButtonUp();
                    DrtBase.WaitForRender();
                    AssertInState(c, "CommonStates", "MouseOver");
                }
            }
        }

        private void TestFocused()
        {
            foreach (var c in _allControls)
            {
                if (!(c is ProgressBar || c is ScrollBar))
                {
                    _theTextBlock.Focus();
                    DrtBase.WaitForRender();
                    AssertInState(c, "FocusStates", "Unfocused");
                    if (c.Focus())
                    {
                        DrtBase.WaitForRender();
                        AssertInState(c, "FocusStates", "Focused");
                    }
                }
            }
        }

        private void TestElementState()
        {
            DRT.Assert(((SolidColorBrush)_elementStateRect.Fill).Color == Colors.Green);
            VisualStateManager.GoToElementState(_elementStateRect, "Hot", true);
            DrtBase.WaitForRender();
            DRT.Assert(((SolidColorBrush)_elementStateRect.Fill).Color == Colors.Red);
            VisualStateManager.GoToElementState(_elementStateRect, "Cold", true);
            DrtBase.WaitForRender();
            DRT.Assert(((SolidColorBrush)_elementStateRect.Fill).Color == Colors.Blue);
        }
        
        private void TestValidation()
        {
            _validationData.ValidationDP1 = "invalid";
            DrtBase.WaitForRender();
            _validationData.ValidationDP2 = "invalid";
            DrtBase.WaitForRender();
            _validationData.ValidationDP1 = "valid";
            DrtBase.WaitForRender();
            _validationData.ValidationDP2 = "valid";
            DrtBase.WaitForRender();
        }
        
        private class ValidationData : DependencyObject
        {
            public static readonly DependencyProperty ValidationDP1Property = DependencyProperty.Register("ValidationDP1", typeof(string), typeof(DrtVSMGoToStateSuite));
            public static readonly DependencyProperty ValidationDP2Property = DependencyProperty.Register("ValidationDP2", typeof(string), typeof(DrtVSMGoToStateSuite));
            
            public string ValidationDP1
            {
                get
                {
                    return (string)GetValue(ValidationDP1Property);
                }
                set
                {
                    SetValue(ValidationDP1Property, value);
                }
            }
            
            public string ValidationDP2
            {
                get
                {
                    return (string)GetValue(ValidationDP2Property);
                }
                set
                {
                    SetValue(ValidationDP2Property, value);
                }
            }
        }
        
        private void TestUnloadCompleted()
        {
            var groups = VisualStateManager.GetVisualStateGroups((FrameworkElement)VisualTreeHelper.GetChild(_theButton, 0));
            DRT.Assert(groups != null);
            VisualStateGroup group = null;
            foreach(VisualStateGroup g in groups)
            {
                if (g.Name == "ChangedEventStates")
                {
                    group = g;
                }
            }
            
            DRT.Assert(group != null);
            
            VisualStateManager.GoToState(_theButton, "ChangedEvent_1", false);
            DispatcherTimer failTimer = null;
            group.CurrentStateChanged += (s, e) => 
            { 
                if (failTimer != null) 
                    failTimer.Stop(); 
                DRT.Resume(); 
            };
            
            VisualStateManager.GoToState(_theButton, "ChangedEvent_2", true); // background border goes red over 2 seconds
            _mainGrid.Children.Remove(_theButton);
            
            failTimer= new DispatcherTimer(DispatcherPriority.Normal);
            failTimer.Tick += (s2, e2) =>
            {
                DRT.Assert(false, "We did not get CurrentStateChanged for unloaded element.");
            };
            failTimer.Interval = TimeSpan.FromSeconds(5); 
            failTimer.Start();
            
            DRT.Suspend();
        }

        private void AssertInState(Control c, string group, string state)
        {
            var vsg = GetGroup(c, group);
            DRT.Assert(vsg != null);
            var currentState = vsg.CurrentState;
            DRT.Assert(currentState != null);
            DRT.Assert(currentState.Name == state);
        }

        private VisualStateGroup GetGroup(Control c, string group)
        {
            foreach (VisualStateGroup vsg in VisualStateManager.GetVisualStateGroups(VisualTreeHelper.GetChild(c, 0) as FrameworkElement))
            {
                if (vsg.Name == group)
                {
                    return vsg;
                }
            }

            return null;
        }

        

        bool _isPageLoaded;
    }
    
    public class VSMValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;
            string name = value as string;
            
            if (name != null && name == "invalid")
            {
                result = new ValidationResult(false, "yikes!");
            }

            return result;
        }
    }
}
