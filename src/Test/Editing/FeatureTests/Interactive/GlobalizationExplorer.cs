// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides an application to test globalization features.


[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 17 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Text/BVT/Interactive/TreeNavigator.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;

    using System.ComponentModel.Design;
    using Drawing = System.Drawing;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;
    using Test.Uis.Data;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// An interactive application to test globalization features.
    /// </summary>
    public class GlobalizationExplorer: CustomTestCase
    {
        #region Public methods.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            CreateUserInterface();
        }

        #endregion Public methods.

        #region Private methods.

        private void CreateUserInterface()
        {
            _topPanel = new DockPanel();
            _topPanel.LastChildFill = true;
            _instructions = new FlowDocumentScrollViewer();
            _instructions.Document = new FlowDocument();
            _inputBox = new TextBox();
            _listKeyboardLayouts = new Button();
            _monitorKeyboardBox = new CheckBox();
            _showLocaleButton = new Button();
            _activateKeyboard = new Button();
            _sendInputButton = new Button();
            _resultBox = new TextBox();

            new UIElementWrapper(_instructions).XamlText =
                "<Paragraph>Use this interactive application to test " +
                "globalization features.</Paragraph>" +
                "<Paragraph>Remember that <Bold>keyboard layout</Bold> " +
                "is just an older name for <Bold>input locale</Bold>.</Paragraph>";
            _instructions.FontSize = 16;
            _instructions.Margin = new Thickness(8, 4, 8, 4);
            DockPanel.SetDock(_instructions, Dock.Top);

            _inputBox.FontFamily = new FontFamily("Courier New");
            _inputBox.FontSize = 14;
            DockPanel.SetDock(_inputBox, Dock.Top);

            _listKeyboardLayouts.Content = "List Input Locales";
            _listKeyboardLayouts.Click += ListKeyboardLayoutsClick;
            DockPanel.SetDock(_listKeyboardLayouts, Dock.Top);

            _monitorKeyboardBox.Content = "Monitor Keyboard Input";
            _monitorKeyboardBox.Checked += MonitorKeyboardChecked;
            DockPanel.SetDock(_monitorKeyboardBox, Dock.Top);

            _showLocaleButton.Content = "Show Active Locale";
            _showLocaleButton.Click += ShowLocaleClick;
            DockPanel.SetDock(_showLocaleButton, Dock.Top);

            _activateKeyboard.Content = "Activate Input Locale";
            _activateKeyboard.Click += ActivateKeyboardClick;
            DockPanel.SetDock(_activateKeyboard, Dock.Top);

            _sendInputButton.Content = "Send Input";
            _sendInputButton.Click += SendInputButton;
            DockPanel.SetDock(_sendInputButton, Dock.Top);

            _resultBox.AcceptsReturn = true;
            _resultBox.FontFamily = new FontFamily("Courier New");
            _resultBox.FontSize = 14;
            _resultBox.IsReadOnly = true;
            _resultBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            //DockPanel.SetDock(_resultBox, Dock.Fill);

            _topPanel.Children.Add(_instructions);
            _topPanel.Children.Add(_inputBox);
            _topPanel.Children.Add(_listKeyboardLayouts);
            _topPanel.Children.Add(_monitorKeyboardBox);
            _topPanel.Children.Add(_showLocaleButton);
            _topPanel.Children.Add(_activateKeyboard);
            _topPanel.Children.Add(_sendInputButton);
            _topPanel.Children.Add(_resultBox);

            MainWindow.Content = _topPanel;
        }

        private void ActivateKeyboardClick(object sender, RoutedEventArgs e)
        {
            string inputLocale;
            string result;

            inputLocale = _inputBox.Text.Trim();
            result = "Current locale: " + KeyboardInput.GetActiveInputLocaleString();
            result += "\r\nLoading " + inputLocale + "...";
            try
            {
                KeyboardInput.SetActiveInputLocale(inputLocale);
                result += "\r\nCurrent locale: " + KeyboardInput.GetActiveInputLocaleString();
            }
            catch(Exception exception)
            {
                result += exception.ToString();
            }
            _resultBox.Text = result;
        }

        /// <summary>
        /// Lists all available keyboard layouts on the system.
        /// </summary>
        private void ListKeyboardLayoutsClick(object sender, RoutedEventArgs e)
        {
            StringBuilder builder;  // String builder for resulting text.
            IntPtr[] hkls;          // Array of keyboard layouts (input locales).
            int layoutCount;        // Number of available layouts.

            builder = new StringBuilder();
            layoutCount = 1024;
            hkls = new IntPtr[layoutCount];
            layoutCount = Win32.SafeGetKeyboardLayoutList(layoutCount, hkls);

            builder.Append("Number of keyboard layouts: " + layoutCount);
            builder.AppendLine();
            for (int i = 0; i < layoutCount; i++)
            {
                LanguageIdentifierData language;
                string inputLocaleString;

                inputLocaleString = ((uint)(hkls[i])).ToString("x8", System.Globalization.CultureInfo.InvariantCulture);
                builder.Append("Keyboard layout " + inputLocaleString);
                builder.Append(" - ");
                language = LanguageIdentifierData.FindByIdentifier(inputLocaleString.Substring(4));
                builder.Append((language == null)? "unknown" : language.Language);
                builder.AppendLine();
            }
            _resultBox.Text = builder.ToString();
        }

        private void MonitorKeyboardChecked(object sender, RoutedEventArgs e)
        {
            if ((bool)_monitorKeyboardBox.IsChecked && !_monitoringSetup)
            {
                InputManager manager;

                _monitoringSetup = true;
                manager = InputManager.Current;
                manager.PreProcessInput += new PreProcessInputEventHandler(PreProcessInput);
            }
        }

        private void PreProcessInput(object sender, PreProcessInputEventArgs args)
        {
            string description;
            if (!(bool)_monitorKeyboardBox.IsChecked)
            {
                return;
            }
            if (args == null || args.StagingItem == null ||
                args.StagingItem.Input == null ||
                args.StagingItem.Input.Device == null ||
                args.StagingItem.Input.Device.GetType().Name.IndexOf("eyboard") == -1)
            {
                return;
            }
            description = "PreProcess: Canceled - " + args.Canceled;
            if (args.StagingItem.Input is System.Windows.Input.KeyboardFocusChangedEventArgs)
            {
                description += " - focus changed";
            }
            //else if (InputReportEventArgsWrapper.IsCorrectType(args.StagingItem.Input))
            //{
                // Get System.Windows.Input.Key
                // Get System.Windows.Input.KeyboardDevice.ScanCode
                // Get System.Windows.Input.RawKeyboardActions.KeyDown
            //}
            else
            {
                description = "other";
            }
            _resultBox.Text += "\r\n" + description;
        }

        private void SendInputButton(object sender, RoutedEventArgs e)
        {
            _resultBox.Focus();
            _resultBox.IsReadOnly = false;
            _resultBox.Clear();
            KeyboardInput.TypeString(_inputBox.Text);
        }

        private void ShowLocaleClick(object sender, RoutedEventArgs e)
        {
            string inputLocale;                 // Current input locale.
            string result;                      // Result of operation.
            LanguageIdentifierData language;    // Language data for input locale.

            inputLocale = KeyboardInput.GetActiveInputLocaleString();
            result = "Current locale: " + inputLocale;
            language = LanguageIdentifierData.FindByIdentifier(inputLocale.Substring(4));
            result += "\r\nLanguage:       " + ((language == null)? "unknown" : language.Language);
            _resultBox.Text = result;
        }

        #endregion Private methods.

        #region Private fields.

        private DockPanel _topPanel;
        private FlowDocumentScrollViewer _instructions;
        private TextBox _inputBox;
        private CheckBox _monitorKeyboardBox;
        private Button _listKeyboardLayouts;
        private Button _showLocaleButton;
        private Button _activateKeyboard;
        private Button _sendInputButton;
        private TextBox _resultBox;
        private bool _monitoringSetup;

        #endregion Private fields.
    }
}


