// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* Test Input state around opening and closing dialogs
    See INTEGRATION: Input state around opening and closing dialogs - PS#5049
    This CustomControl opens a DialogBox on mouse/keyboard property change.
    This control defines two custom properties, which is data bound to mouse/keyboard
    property in XAML. DialogBox can be opened when these custom properties changed. This custom
    control is used in the test to verify mouse/keyboard events and property values are in sync
    when dialogbox is opened and closed
*/

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using Microsoft.Test.Threading;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Markup;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Win32;
using System.Windows.Input;

using System.Runtime.InteropServices;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Indicate when the Dialogbox should be opened and closed
    /// </summary>
    public enum DialogTestType
    {
        /// <summary>
        /// Dialog should open when custom property (bound to Mouse property in XAML) changes
        /// </summary>
        BoundMouseProperty,

        /// <summary>
        /// Dialog should open when Mouse property changes
        /// </summary>
        MouseProperty,

        /// <summary>
        /// Dialog should open when custom property (bound to Keyboard property in XAML) changes
        /// </summary>
        BoundKeyboardProperty,

        /// <summary>
        /// Dialog should open when Keyboard property changes
        /// </summary>
        KeyboardProperty
    }

    /// <summary>
    /// DialogOpenOnMousePropertyChange that opens dialog based on DialogTestType value
    /// </summary>
    public class DialogOpenOnPropertyChange : ContentControl
    {

        /// <summary>
        /// TestType Property - dialog opens based on the value of this property
        /// </summary>
        public static readonly DependencyProperty TestTypeProperty = DependencyProperty.Register("TestType", typeof(DialogTestType), typeof(DialogOpenOnPropertyChange));
        
        /// <summary>
        /// Custom Property - used to bind to a MouseProperty in XAML file, used for testing
        /// </summary>
        public static readonly DependencyProperty IsBoundMouseOverProperty = DependencyProperty.Register("IsBoundMouseOver", typeof(bool), typeof(DialogOpenOnPropertyChange));

        /// <summary>
        /// Custom Property - used to bind to a KeyboardProperty in XAML file, used for testing
        /// </summary>
        public static readonly DependencyProperty IsBoundKeyboardFocusedProperty = DependencyProperty.Register("IsBoundKeyboardFocused", typeof(bool), typeof(DialogOpenOnPropertyChange));


        /// <summary>
        /// 
        /// </summary>
        public DialogOpenOnPropertyChange()
            : base()
        {
            CoreLogger.LogStatus("DialogOpenOnPropertyChange constructor");
        }

        /// <summary>
        /// PropertyChange event handler
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            switch (TestType)
            {
                case DialogTestType.BoundMouseProperty:
                    if (e.Property == IsBoundMouseOverProperty)
                    {
                        DisplayDialog(e);
                    }
                    break;

                case DialogTestType.MouseProperty:
                    if (e.Property == IsMouseOverProperty)
                    {
                        DisplayDialog(e);
                    }
                    break;

                case DialogTestType.BoundKeyboardProperty:
                    if (e.Property == IsBoundKeyboardFocusedProperty)
                    {
                        DisplayDialog(e);
                    }
                    break;

                case DialogTestType.KeyboardProperty:
                    if (e.Property == IsKeyboardFocusedProperty)
                    {
                        DisplayDialog(e);
                    }
                    break;
            }
        }

        /// <summary>
        /// Display dialog
        /// </summary>
        /// <param name="e"></param>
        private void DisplayDialog(DependencyPropertyChangedEventArgs e)
        {
            if ( ((bool)e.NewValue == true) && (!_secondTime) )
            {
                _secondTime = true;
                CoreLogger.LogStatus("Before dialog opened");

                //Dialog
                _win = new Window();
                //win.Top = 500;
                //win.Left = 500;

                NativeStructs.RECT rc = NativeStructs.RECT.Empty;
                NativeMethods.GetWindowRect(new HandleRef(null, NativeMethods.GetActiveWindow()), ref rc);

                //NativeMethods.SetWindowPos(NativeMethods.GetActiveWindow()
                //    , NativeConstants.HWND_TOP, 10, 10, rc.Width, rc.Height, NativeConstants.SWP_SHOWWINDOW);
                //DispatcherHelper.DoEvents(1000);
                //NativeMethods.GetWindowRect(new HandleRef(null, NativeMethods.GetActiveWindow()), ref rc);

                _win.Top = rc.bottom+10;
                _win.Left = rc.left+10;

                _win.Height = 100.0;
                _win.Width = 100.0;
                _button = new Button();
                _button.Click += new RoutedEventHandler(button_Click);
                _button.Content = "test";
                _win.Content = _button;
                _win.ContentRendered +=new EventHandler(win_ContentRendered);
                _win.ShowDialog();

                CoreLogger.LogStatus("After dialog opened");
            }
        }

        Button _button;
        Window _win;
        bool _secondTime;

        private void win_ContentRendered(object sender, EventArgs args)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new DispatcherOperationCallback(MouseClickOnButton), null);
        }

        /// <summary>
        /// Click on the button to dismiss the dialog
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private object MouseClickOnButton(object e)
        {
            MouseHelper.Move(_button, MouseLocation.Center);
            MouseHelper.PressButton();
            MouseHelper.ReleaseButton();
            return null;
        }

        /// <summary>
        /// Close the dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void button_Click(object sender, RoutedEventArgs args)
        {
            _win.Close();
        }


        /// <summary>
        /// TestType Property - dialog opens based on the value of this property
        /// </summary>
        public DialogTestType TestType
        {
            get
            {
                return (DialogTestType)this.GetValue(DialogOpenOnPropertyChange.TestTypeProperty);
            }
            set
            {
                this.SetValue(DialogOpenOnPropertyChange.TestTypeProperty, value);
            }

        }

        /// <summary>
        /// Custom Property - used to bind to a MouseProperty in XAML file, used for testing
        /// </summary>
        public bool IsBoundMouseOver
        {
            get 
            {
                return (bool)this.GetValue(DialogOpenOnPropertyChange.IsBoundMouseOverProperty); 
            }
            set 
            {
                this.SetValue(DialogOpenOnPropertyChange.IsBoundMouseOverProperty, value); 
            }
        }

        /// <summary>
        /// Custom Property - used to bind to a KeyboardProperty in XAML file, used for testing
        /// </summary>
        public bool IsBoundKeyboardFocused
        {
            get
            {
                return (bool)this.GetValue(DialogOpenOnPropertyChange.IsBoundKeyboardFocusedProperty);
            }
            set
            {
                this.SetValue(DialogOpenOnPropertyChange.IsBoundKeyboardFocusedProperty, value);
            }
        }
    }
}
