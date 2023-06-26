// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  This file contains integration tests for Editing and AddIn

using System;
using System.Reflection;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using System.AddIn;
using System.AddIn.Contract;
using System.AddIn.Pipeline;

using Microsoft.Test.Diagnostics;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Threading;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.Management;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Test.Uis.TextEditing
{
    /// <summary>
    /// This test case tests Editing features inside AddIn. Inside AddIn, Editing controls are hosted and 
    /// editing operations are performed. AddIn is unloaded and loaded couple of times. This test also 
    /// includes coverage for Regression_Bug102
    /// Disabling until Regression_Bug103 is fixed.
    /// </summary>
    [Test(0, "Editor", "EditingInsideAddIn", MethodParameters = "/TestCaseType=EditingInsideAddIn", Timeout=150, Disabled=true)]
    [TestOwner("Microsoft"), TestBugs("102,440"), TestLastUpdatedOn("July 10th, 2007")]
    public class EditingInsideAddIn : CustomTestCase
    {
        private StackPanel _hostPanel;
        private TextBox _hostTB;
        private RichTextBox _hostRTB;
        private PasswordBox _hostPB;
        private Button _loadButton,_unloadButton;

        private Border _border;
        private AppDomain _appDomain;
        private IsolatedAddIn _remoteHost;

        private const string contentToType = "This is a test";
        private const int minimumWaitTime = 500; //switching focus has some delay and hence the wait time for the stability of the test.

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            _hostTB = new TextBox();
            _hostTB.Height = 100;
            _hostTB.AcceptsReturn = true;
            _hostTB.SetValue(SpellCheck.IsEnabledProperty, true);

            _hostRTB = new RichTextBox();
            _hostRTB.Height = 100;
            _hostRTB.Background = Brushes.LightBlue;
            _hostRTB.SetValue(SpellCheck.IsEnabledProperty, true);

            _hostPB = new PasswordBox();

            _unloadButton = new Button();
            _unloadButton.Content = "Unload AppDomain";
            _unloadButton.Click += new RoutedEventHandler(unloadButton_Click);

            _loadButton = new Button();
            _loadButton.Content = "Load AppDomain";
            _loadButton.Click += new RoutedEventHandler(loadButton_Click);            

            _border = new Border();
            _border.BorderThickness = new Thickness(20);
            _border.BorderBrush = Brushes.Red;
            _border.Height = 300;
            _border.Child = CreateAddinHost("Red_AddIn", out _appDomain, out _remoteHost);            

            _hostPanel = new StackPanel();
            _hostPanel.Children.Add(_loadButton);
            _hostPanel.Children.Add(_unloadButton);
            _hostPanel.Children.Add(_hostTB);
            _hostPanel.Children.Add(_hostRTB);
            _hostPanel.Children.Add(_hostPB);
            _hostPanel.Children.Add(_border);

            MainWindow.Content = _hostPanel;
            QueueDelegate(LoadIme);
        }

        private void LoadIme()
        {
            //Important: Ime code path in WPF should be running for this test case intended test scenario
            //Make sure Ime code is running by adding Japanese Ime to keyboard layouts
            if (!KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.Japanese))
            {
                Logger.Current.ReportResult(false, "Unable to install Japanese Ime", false);
                return;
            }
            
            QueueDelegate(PerformTestActions);
        }

        private void PerformTestActions()
        {
            Log("Performing typing oerpations inside editing controls");
            DoTyping();

            Log("Unloading the AppDomain (Repro for Regression_Bug440)...");
            MouseInput.MouseClick(_unloadButton);
            DispatcherHelper.DoEvents();

            Log("Moving the main window (Repro for Regression_Bug102)");
            DoWindowMove();

            Log("Loading the AppDomain...");
            MouseInput.MouseClick(_loadButton);
            DispatcherHelper.DoEvents();

            Log("Perform typing operations inside editing controls again after loading the AppDomain");
            _hostTB.Clear();
            _hostRTB.Document.Blocks.Clear();
            _hostPB.Clear();
            DoTyping();

            Logger.Current.ReportSuccess();            
        }        

        private void DoTyping()
        {
            _hostTB.Focus();
            DispatcherHelper.DoEvents();    

            //Typing in host TextBox
            KeyboardInput.TypeString(contentToType + "{TAB}");
            DispatcherHelper.DoEvents(minimumWaitTime);

            //Typing in host RichTextBox
            KeyboardInput.TypeString(contentToType + "{TAB}");
            DispatcherHelper.DoEvents(minimumWaitTime);

            //Typing in host PasswordBox
            KeyboardInput.TypeString(contentToType + "{TAB}");
            DispatcherHelper.DoEvents(minimumWaitTime);

            //Typing in AddIn TextBox
            KeyboardInput.TypeString(contentToType + "{TAB}");
            DispatcherHelper.DoEvents(minimumWaitTime);

            //Typing in AddIn RichTextBox
            KeyboardInput.TypeString(contentToType + "{TAB}");
            DispatcherHelper.DoEvents(minimumWaitTime);

            //Typing in AddIn PasswordBox
            KeyboardInput.TypeString(contentToType + "{TAB}");
            DispatcherHelper.DoEvents(minimumWaitTime);

            //To execute the "Verify contents in the editing controls" button's click handler
            KeyboardInput.TypeString("{ENTER}");
            DispatcherHelper.DoEvents(minimumWaitTime);
        }

        private void DoWindowMove()
        {
            Point windowTitleBarClickPoint = new Point(MainWindow.Left + (MainWindow.ActualWidth / 2), MainWindow.Top + 10 /*middle of the title bar*/);
            Point windowTitleBarMovePoint = new Point(windowTitleBarClickPoint.X + 50 /* Move the window by 50 pixels*/, windowTitleBarClickPoint.Y);

            //Move the window (Repro for Regression_Bug102)
            MouseInput.MouseDragPressed(windowTitleBarClickPoint, windowTitleBarMovePoint);
            DispatcherHelper.DoEvents(minimumWaitTime);                        
        }

        private void unloadButton_Click(object sender, RoutedEventArgs e)
        {
            AppDomain.Unload(_appDomain);            
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {            
            _border.Child = CreateAddinHost("Red_AddIn", out _appDomain, out _remoteHost);
        }

        private FrameworkElement CreateAddinHost(string name, out AppDomain appDomain, out IsolatedAddIn remoteHost)
        {
            AppDomainSetup appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            appDomain = AppDomain.CreateDomain(name, null, appDomainSetup);            

            //create the remotehost in the isolated appdomain, and create the window
            remoteHost = (IsolatedAddIn)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(IsolatedAddIn).FullName);
            INativeHandleContract contract = remoteHost.CreateHwndHost();

            //set the host ui to a child
            return FrameworkElementAdapters.ContractToViewAdapter(contract);
        }

        //Remote class that allows the host to communicate with the isolated content
        private class IsolatedAddIn : MarshalByRefObject
        {
            TextBox _textBox;
            RichTextBox _richTextBox;
            PasswordBox _passwordBox;
            Button _button;
            StackPanel _panel;

            const string contentToType = "This is a test";

            //This is called from within isolated AppDomain
            public INativeHandleContract CreateHwndHost()
            {
                _textBox = new TextBox();
                _textBox.Name = "addInTB";
                _textBox.Height = 50;
                _textBox.AcceptsReturn = true;
                _textBox.SetValue(SpellCheck.IsEnabledProperty, true);                

                _richTextBox = new RichTextBox();
                _richTextBox.Name = "addInRTB";
                _richTextBox.Height = 50;
                _richTextBox.Background = Brushes.LightBlue;
                _richTextBox.SetValue(SpellCheck.IsEnabledProperty, true);

                _passwordBox = new PasswordBox();
                _passwordBox.Name = "addInPB";

                _button = new Button();
                _button.Content = "Verify contents in the editing controls";
                _button.Click += new RoutedEventHandler(button_Click);

                _panel = new StackPanel();                       
                _panel.Children.Add(_textBox);
                _panel.Children.Add(_richTextBox);
                _panel.Children.Add(_passwordBox);
                _panel.Children.Add(_button);

                //create the hwnd source and return the hwnd
                return FrameworkElementAdapters.ViewToContractAdapter(_panel);
            }

            private void button_Click(object sender, RoutedEventArgs e)
            {
                _button.Background = Brushes.LightGreen; //visual feedback that button's click handler is triggered

                Verifier.Verify(_textBox.Text == contentToType, "Verifying contents inside AddIn TextBox", true);
                UIElementWrapper rtbWrapper = new UIElementWrapper(_richTextBox);
                Verifier.Verify(rtbWrapper.Text.Contains(contentToType), "Verifying contents inside AddIn RichTextBox", true);
                Verifier.Verify(_passwordBox.Password != string.Empty, "Verifying contents inside AddIn PasswordBox", true);
            }                        
        }
    }
}