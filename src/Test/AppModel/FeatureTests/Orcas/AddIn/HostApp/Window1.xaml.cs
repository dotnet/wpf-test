// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Security;
using System.AddIn.Hosting;
using System.Collections.ObjectModel;


namespace Microsoft.Test.AddIn
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        private string _addInParameters;
        private HostCountClicksAddInView _hostCountClicksAddInView;
        private HostSequenceFocusView _hostSequenceFocusView;
        private HostDragDropView _hostDragDropView;
        private HostWebOCView _hostWebOCView;
        private HostWinFormsHostView _hostWinFormsHostView;
        private string _addInSourcePath;
        private Type _addInType;
        private AddInSecurityLevel _addInSecurityLevel;

        public Window1()
        {
            InitializeComponent();
            _addInParameters = "";
            _addInSourcePath = Environment.CurrentDirectory;
            _addInType = null;

            listSecurity.Items.Add(AddInSecurityLevel.Internet);
            listSecurity.Items.Add(AddInSecurityLevel.Intranet);
            listSecurity.Items.Add(AddInSecurityLevel.FullTrust);
            listSecurity.SelectedIndex = 0;
            
            comboAddIn.Items.Add(typeof(HostSequenceFocusView));
            comboAddIn.Items.Add(typeof(HostCountClicksAddInView));
            comboAddIn.Items.Add(typeof(HostDragDropView));
            comboAddIn.Items.Add(typeof(HostWebOCView));
            comboAddIn.Items.Add(typeof(HostWinFormsHostView));
            comboAddIn.SelectedIndex = 0;
        }

        private void GetClickCount(object sender, RoutedEventArgs args)
        {
            if (_hostCountClicksAddInView != null)
            {
                MessageBox.Show("Click Count = " + _hostCountClicksAddInView.Clicks.ToString());
            }
            if (_hostSequenceFocusView != null)
            {

            }
        }

        private void InitializeAddIn(object sender, RoutedEventArgs args)
        {
            if (_hostCountClicksAddInView != null)
            {
                _hostCountClicksAddInView.Initialize(_addInParameters);
            }
            if (_hostSequenceFocusView != null)
            {
                _hostSequenceFocusView.Initialize(_addInParameters);
            }
            if (_hostDragDropView != null)
            {
                _hostDragDropView.Initialize(_addInParameters);
            }
            if (_hostWebOCView != null)
            {
                _hostWebOCView.Initialize(_addInParameters);
            }
            if (_hostWinFormsHostView != null)
            {
                _hostWinFormsHostView.Initialize(_addInParameters);
            }
        }

        private FrameworkElement GetAddInUI()
        {
            FrameworkElement addHostElement = null;
            _addInSecurityLevel = (AddInSecurityLevel)listSecurity.SelectedItem;
            _addInType = (Type)comboAddIn.SelectedItem;

            //Check to see if new AddIns have been installed
            string[] warnings = AddInStore.Rebuild(_addInSourcePath);
            if (warnings.Length > 0)
            {
                string msg = "Pipeline Warnings";
                foreach (string warning in warnings)
                {
                    msg += "\n" + warning;
                }
                MessageBox.Show(msg);
                return null;
            }

            //Look for AddIns in our root directory and store the results
            Collection<AddInToken> tokens = AddInStore.FindAddIns(_addInType, _addInSourcePath);

            //Which token to activate?
            if (tokens.Count < 1)
            {
                throw new ArgumentException("Could not find any AddIns of type: " + _addInType.ToString());
            }
 
            //Activate the selected AddInToken in a new AppDomain with the appropriate security level
            switch(_addInType.ToString())
            {
                case "Microsoft.Test.AddIn.HostCountClicksAddInView":
                    _hostCountClicksAddInView = (HostCountClicksAddInView)tokens[0].Activate<HostCountClicksAddInView>(_addInSecurityLevel);
                    addHostElement = _hostCountClicksAddInView.GetAddInUserInterface();
                break;

                case "Microsoft.Test.AddIn.HostSequenceFocusView":
                    _hostSequenceFocusView = (HostSequenceFocusView)tokens[0].Activate<HostSequenceFocusView>(_addInSecurityLevel);
                    addHostElement = _hostSequenceFocusView.GetAddInUserInterface();
                break;

                case "Microsoft.Test.AddIn.HostDragDropView":
                    _hostDragDropView = (HostDragDropView)tokens[0].Activate<HostDragDropView>(_addInSecurityLevel);
                    addHostElement = _hostDragDropView.GetAddInUserInterface();
                break;

                case "Microsoft.Test.AddIn.HostWebOCView":
                    _hostWebOCView = (HostWebOCView)tokens[0].Activate<HostWebOCView>(_addInSecurityLevel);
                    addHostElement = _hostWebOCView.GetAddInUserInterface();
                break;

                case "Microsoft.Test.AddIn.HostWinFormsHostView":
                    _hostWinFormsHostView = (HostWinFormsHostView)tokens[0].Activate<HostWinFormsHostView>(_addInSecurityLevel);
                    addHostElement = _hostWinFormsHostView.GetAddInUserInterface();
                break;
            }

            return addHostElement;
        }


        private void AddAddIn(object sender, RoutedEventArgs args)
        {
            FrameworkElement addHostElement = GetAddInUI();
            if (addHostElement != null)
            {
                RootPanel.Children.Add(addHostElement);
            }
        }

        private void RemoveAddIns(object sender, RoutedEventArgs args)
        {
            if (_hostCountClicksAddInView != null)
            {
                AddInController.GetAddInController(_hostCountClicksAddInView).Shutdown();
                _hostCountClicksAddInView = null;
            }
            if (_hostSequenceFocusView != null)
            {
                AddInController.GetAddInController(_hostSequenceFocusView).Shutdown();
                _hostSequenceFocusView = null;
            }
            if (_hostDragDropView != null)
            {
                AddInController.GetAddInController(_hostDragDropView).Shutdown();
                _hostDragDropView = null;
            }
            if (_hostWebOCView != null)
            {
                AddInController.GetAddInController(_hostWebOCView).Shutdown();
                _hostWebOCView = null;
            }
            if (_hostWinFormsHostView != null)
            {
                AddInController.GetAddInController(_hostWinFormsHostView).Shutdown();
                _hostWinFormsHostView = null;
            }
            RootPanel.Children.Clear();
        }

        private void AddAddInNewWindow(object sender, RoutedEventArgs args)
        {
            FrameworkElement addHostElement = GetAddInUI();
            if (addHostElement != null)
            {
                ConfigureAddInDialogBox dlg = new ConfigureAddInDialogBox(addHostElement);
                dlg.ShowDialog();
            }
        }

        public class ConfigureAddInDialogBox : Window
        {
            DockPanel _addInUIHostDockPanel;

            public ConfigureAddInDialogBox(FrameworkElement addInOptionsUI)
            {
                _addInUIHostDockPanel = new DockPanel();

                WindowStartupLocation = WindowStartupLocation.CenterOwner;
                ShowInTaskbar = false;
                MinWidth = 300;
                MinHeight = 200;
                ResizeMode = ResizeMode.CanResizeWithGrip;
                SizeToContent = SizeToContent.WidthAndHeight;
                Content = _addInUIHostDockPanel;
                _addInUIHostDockPanel.Children.Add(addInOptionsUI);
            }
        }

    }
}
