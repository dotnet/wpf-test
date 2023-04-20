// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Data;
using System;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Test.Input;
using System.Windows.Media;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug with ContentPresenter's template not
    ///  updating based upon whether new content string has or does not
    ///  have an AccessKey.
    /// </summary>
    [Test(1, "Regressions.Part1", "AccessKeyContentPresenter")]
    public class AccessKeyContentPresenter : WindowTest
    {
        #region Private Data

        private Button _myButton;
        private ContextMenu _myContextMenu;
        private MenuItem _myMenuItem;
        private DataModel _myDataModel;
        private bool _buttonAccessKeyInvoked;
        private bool _menuItemAccessKeyInvoked;
        private CultureInfo _currentInputLanguage;

        #endregion

        #region Constructors

        public AccessKeyContentPresenter()
        {
            InitializeSteps += new TestStep(Setup);  
            RunSteps += new TestStep(ValidateInitialNoAccessKey);
            RunSteps += new TestStep(ValidateHasAccessKey);
            RunSteps += new TestStep(ValidateNoAccessKey);  
        }

        #endregion

        #region Private Members        

        private TestResult Setup()
        {
            // Create a button and a context menu with access key handlers
            _myDataModel = new DataModel();
            Window.DataContext = _myDataModel;
            StackPanel myStackPanel = new StackPanel();
            Window.Content = myStackPanel;
            //get current input language
            _currentInputLanguage = InputLanguageManager.GetInputLanguage(Window);
            _myButton = new Button();
            Binding buttonBinding = new Binding("MenuText");
            BindingOperations.SetBinding(_myButton, Button.ContentProperty, buttonBinding);
            myStackPanel.Children.Add(_myButton);
            _myContextMenu = new ContextMenu();
            myStackPanel.ContextMenu = _myContextMenu;
            _myMenuItem = new MenuItem();
            _myContextMenu.Items.Add(_myMenuItem);
            Binding menuItemBinding = new Binding("MenuText");
            BindingOperations.SetBinding(_myMenuItem, MenuItem.HeaderProperty, menuItemBinding);
            AccessKeyManager.AddAccessKeyPressedHandler(_myButton, Button_AccessKeyPressed);
            AccessKeyManager.AddAccessKeyPressedHandler(_myMenuItem, MenuItem_AccessKeyPressed);
            return TestResult.Pass;
        }
        
        private TestResult ValidateInitialNoAccessKey()
        {
            ResetSentinels();

            InvokeAccessKeys();

            return ValidateSentinels(false);
        }

        private TestResult ValidateHasAccessKey()
        {
            ResetSentinels();

            _myDataModel.MenuText = "Menu Text With _Access Key";

            InvokeAccessKeys();

            return ValidateSentinels(true);
        }

        private TestResult ValidateNoAccessKey()
        {
            ResetSentinels();

            _myDataModel.MenuText = "Menu Text With No Access Key";

            InvokeAccessKeys();

            return ValidateSentinels(false);
        }

        private TestResult ValidateSentinels(bool shouldHaveAccessKeys)
        {
            // On slow machines this test has occasionally gotten here before buttons have been populated...
            WaitForPriority(DispatcherPriority.SystemIdle);

            bool buttonHasAccessKey = Util.FindElementsWithType(_myButton, typeof(AccessText)).Length != 0;
            bool menuItemHasAccessKey = Util.FindElementsWithType(_myMenuItem, typeof(AccessText)).Length != 0;

            if (buttonHasAccessKey != shouldHaveAccessKeys)
            {
                LogComment("Button logical tree access key mismatch.");
                return TestResult.Fail;
            }
            if (menuItemHasAccessKey != shouldHaveAccessKeys)
            {
                LogComment("MenuItem logical tree access key mismatch.");
                return TestResult.Fail;
            }
            if (_buttonAccessKeyInvoked != shouldHaveAccessKeys)
            {
                LogComment("Button AccessKey method invoke mismatch.");
                return TestResult.Fail;
            }
            if (_menuItemAccessKeyInvoked != shouldHaveAccessKeys)
            {
                LogComment("MenuItem AccessKey method invoke mismatch.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private void InvokeAccessKeys()
        {
            //Set the Input Language to English to ensure the test invoke the right AccessKey
            if (InputLanguageManager.Current.CurrentInputLanguage.Name.ToLower() != "en-us")
            {
                InputLanguageManager.SetInputLanguage(Window, CultureInfo.GetCultureInfo("en-us"));
            }
            WaitForPriority(DispatcherPriority.SystemIdle);

            UserInput.MouseRightClickCenter(_myButton);

            WaitForPriority(DispatcherPriority.SystemIdle);

            UserInput.KeyPress(System.Windows.Input.Key.LeftShift, true);
            UserInput.KeyPress("A");
            UserInput.KeyPress(System.Windows.Input.Key.LeftShift, false);

            WaitForPriority(DispatcherPriority.SystemIdle);
            //Set the Input Language back to original language          
            InputLanguageManager.SetInputLanguage(Window, _currentInputLanguage);
        }

        private void ResetSentinels()
        {
            _buttonAccessKeyInvoked = false;
            _menuItemAccessKeyInvoked = false;
        }

        private void ChangeMenuText()
        {
            DataModel dm = (DataModel)Window.DataContext;
            dm.MenuText = "Menu Text With _Access Key";
        }

        private void Button_AccessKeyPressed(object sender, System.Windows.Input.AccessKeyPressedEventArgs e)
        {
            _buttonAccessKeyInvoked = true;
        }

        private void MenuItem_AccessKeyPressed(object sender, System.Windows.Input.AccessKeyPressedEventArgs e)
        {
            _menuItemAccessKeyInvoked = true;
        }

        #endregion


        public class DataModel : INotifyPropertyChanged
        {
            private string _menuText = "Menu Text With No Access Key";

            public string MenuText
            {
                get
                {
                    return this._menuText;
                }
                set
                {
                    this._menuText = value;
                    var handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("MenuText"));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
