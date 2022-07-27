// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace DRT
{
    public class DrtRadioButtonListSuite : DrtTestSuite
    {
        public DrtRadioButtonListSuite() : base("RadioButtonList")
        {
            Contact = "Microsoft";
        }

        private ListBox _radiobuttonlist;
        private RadioButton _rb4;
        private RadioButton _rb5;
        private RadioButton _rb6;
        private RadioButton _rb7;
        private int _selectionChangedFiredNum = 0;

        private Style CreateRadioButtonListStyle()
        {
            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border), "Border");
            border.SetValue(Border.BackgroundProperty, Brushes.Transparent);
            FrameworkElementFactory radioButton = new FrameworkElementFactory(typeof(RadioButton), "RadioButton");
            radioButton.SetValue(FrameworkElement.FocusableProperty, false);
            radioButton.SetValue(ContentControl.ContentProperty, new TemplateBindingExtension(ContentPresenter.ContentProperty));
            radioButton.SetValue(RadioButton.IsHitTestVisibleProperty, false);
            Binding isChecked = new Binding("IsSelected");
            isChecked.Mode = BindingMode.TwoWay;
            isChecked.RelativeSource = RelativeSource.TemplatedParent;
            radioButton.SetBinding(RadioButton.IsCheckedProperty, isChecked);
            Style radioButtonListStyle = new Style(typeof(ListBoxItem));
            ControlTemplate template = new ControlTemplate(typeof(ListBoxItem));
            border.AppendChild(radioButton);
            template.VisualTree = border;
            radioButtonListStyle.Setters.Add(new Setter(Control.TemplateProperty, template));
            return radioButtonListStyle;
        }


        public override DrtTest[] PrepareTests()
        {
            // Build tree
            StackPanel root = new StackPanel();     // Returned root object
            root.Orientation = System.Windows.Controls.Orientation.Horizontal;
            root.Background = Brushes.Pink;

            _radiobuttonlist = new ListBox();
            _radiobuttonlist.SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
            _radiobuttonlist.Resources.Add(typeof(ListBoxItem), CreateRadioButtonListStyle());

            _radiobuttonlist.Items.Add("RadioButton 1");
            _radiobuttonlist.Items.Add("RadioButton 2");
            _radiobuttonlist.Items.Add("RadioButton 3");

            root.Children.Add(_radiobuttonlist);

            // Create stand alone RadioButtons
            _rb4 = new RadioButton();
            _rb5 = new RadioButton();
            _rb6 = new RadioButton();
            _rb7 = new RadioButton();
            _rb4.Content = "No group name 4";
            _rb5.Content = "No group name 5";
            _rb6.Content = "with group name 6";
            _rb7.Content = "with group name 7";
            root.Children.Add(_rb4);
            root.Children.Add(_rb5);
            root.Children.Add(_rb6);
            root.Children.Add(_rb7);
            _rb6.GroupName = "same";
            _rb7.GroupName = "same";

            DRT.Show(root);

            return new DrtTest[]
                {
                    new DrtTest(VerifyInitialState),
                    new DrtTest(MouseTest),
                    new DrtTest(MouseTestVerify),
                    new DrtTest(KeyDownTest),
                    new DrtTest(KeyDownTest1Verify),
                    new DrtTest(MouseTestNoRBL),
                    new DrtTest(MouseTestVerifyNoRBL),
                };
        }

        private void VerifyInitialState()
        {
            VerifyState(false, false, false);
            VerifyAndResetSelectionChanged(0);
            AutomationPeer peer = UIElementAutomationPeer.CreatePeerForElement(_radiobuttonlist);
            ISelectionProvider sp = (ISelectionProvider)peer.GetPattern(PatternInterface.Selection);
            DRT.Assert(sp.CanSelectMultiple == false, "RadioButtonList should be single selection only");
            DRT.Assert(sp.IsSelectionRequired == false, "RadioButtonList.IsSelectionRequired should be false by default");
        }

        private void MouseTest()
        {
            DRT.MoveMouse(_radiobuttonlist, 0.5, 0.5);
            DRT.MouseButtonDown();
            DRT.MouseButtonUp();
        }

        private void MouseTestVerify()
        {
            VerifyState(false, true, false);
            VerifyAndResetSelectionChanged(1);
        }

        private void MouseTestNoRBL()
        {
            DRT.MoveMouse(_rb4, 0.5, 0.5);
            DRT.MouseButtonDown();
            DRT.MouseButtonUp();
            DRT.MoveMouse(_rb5, 0.5, 0.5);
            DRT.MouseButtonDown();
            DRT.MouseButtonUp();
            DRT.MoveMouse(_rb6, 0.5, 0.5);
            DRT.MouseButtonDown();
            DRT.MouseButtonUp();
            DRT.MoveMouse(_rb7, 0.5, 0.5);
            DRT.MouseButtonDown();
            DRT.MouseButtonUp();
        }

        private void MouseTestVerifyNoRBL()
        {
            VerifyStateNoRBL(false, true, false, true);
        }

        private void KeyDownTest()
        {
            DRT.SendKeyboardInput(Key.Down, true);
            DRT.SendKeyboardInput(Key.Down, false);
        }

        private void KeyDownTest1Verify()
        {
            VerifyState(false, false, true);
            VerifyAndResetSelectionChanged(1);
        }

        private void VerifyState(bool state1, bool state2, bool state3)
        {
            if (state1)
                DRT.Assert(_radiobuttonlist.SelectedIndex == 0, "SelectedIndex should be 0, was " + _radiobuttonlist.SelectedIndex);
            else if (state2)
                DRT.Assert(_radiobuttonlist.SelectedIndex == 1, "SelectedIndex should be 1, was " + _radiobuttonlist.SelectedIndex);
            else if (state3)
                DRT.Assert(_radiobuttonlist.SelectedIndex == 2, "SelectedIndex should be 2, was " + _radiobuttonlist.SelectedIndex);
            else
                DRT.Assert(_radiobuttonlist.SelectedIndex == -1, "SelectedIndex should be -1, was " + _radiobuttonlist.SelectedIndex);
        }

        private void VerifyStateNoRBL(bool state4, bool state5, bool state6, bool state7)
        {
            DRT.Assert(_rb4.IsChecked == state4, "_rb4.IsChecked should be " + state4);
            DRT.Assert(_rb5.IsChecked == state5, "_rb5.IsChecked should be " + state5);
            DRT.Assert(_rb6.IsChecked == state6, "_rb6.IsChecked should be " + state6);
            DRT.Assert(_rb7.IsChecked == state7, "_rb7.IsChecked should be " + state7);
        }

        private void VerifyAndResetSelectionChanged(int number)
        {
            DRT.Assert(_selectionChangedFiredNum == number, "SelectionChangedEvent should be fired " + number + "times. Was " + _selectionChangedFiredNum);
            _selectionChangedFiredNum = 0;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectionChangedFiredNum++;
        }

    }
}

