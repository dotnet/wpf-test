// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides an interactive case for navigating visuals and the Automation tree

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/Interactive/TreeNavigator.cs $")]

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
    using System.Windows.Automation;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using System.Windows.Data;

    #endregion Namespaces.

    /// <summary>
    /// Provides an interactive test case for navigating
    /// visuals and the Automation tree.
    /// </summary>
    public class TreeNavigator: CustomTestCase
    {
        #region Public methods.
        
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            // The window has the following layout.
            // TopPanel
            //   XamlPanel
            //     XamlBox
            //     ApplyXamlButton
            //   OptionPanel
            //   QueryPanel
            //     QueryBox
            //     RunQueryButton
            //   QueryResultsBox
            //   DesignCanvas
            
            // Controls that we do not need to keep references to.
            Button applyXamlButton;
            Button runQueryButton;
            DockPanel queryPanel;
            DockPanel topPanel;
            DockPanel xamlPanel;
            ListBox optionPanel;
            
            // Create top panel.
            topPanel = new DockPanel();
            topPanel.LastChildFill = true;
            // Create panel to host XAML editing controls.
            xamlPanel = new DockPanel();
            xamlPanel.LastChildFill = true;
            DockPanel.SetDock(xamlPanel, Dock.Top);
            
            applyXamlButton = new Button();
            DockPanel.SetDock(applyXamlButton, Dock.Right);
            applyXamlButton.Content = "Apply XAML";
            applyXamlButton.Click += ApplyXamlClick;
            xamlPanel.Children.Add(applyXamlButton);
            
            _xamlBox = new TextBox();
            //DockPanel.SetDock(xamlBox, Dock.Fill);
            _xamlBox.AcceptsReturn = true;
            _xamlBox.Text = 
                "<DockPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
                Environment.NewLine +
                "  <Button>My button</Button>" +
                "</DockPanel>";
            _xamlBox.Height = 120;
            _xamlBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _xamlBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            xamlPanel.Children.Add(_xamlBox);
            
            topPanel.Children.Add(xamlPanel);
            
            // Create panel to host XPath query controls.
            queryPanel = new DockPanel();
            queryPanel.LastChildFill = true;
            DockPanel.SetDock(queryPanel, Dock.Top);

            runQueryButton = new Button();
            DockPanel.SetDock(runQueryButton, Dock.Right);
            runQueryButton.Content = "Query";
            runQueryButton.Click += RunQueryClick;
            queryPanel.Children.Add(runQueryButton);
            
            _queryBox = new TextBox();
            //DockPanel.SetDock(queryBox, Dock.Fill);
            _queryBox.Text = ".//*";
            queryPanel.Children.Add(_queryBox);
            
            topPanel.Children.Add(queryPanel);
            
            // Create panel to host option buttons.
            optionPanel = new ListBox();
            optionPanel.Resources.Add(typeof(ListBoxItem), CreateRadioButtonListStyle());
            optionPanel.BorderBrush = Brushes.Transparent;
            KeyboardNavigation.SetDirectionalNavigation(optionPanel, KeyboardNavigationMode.Cycle);
            DockPanel.SetDock(optionPanel, Dock.Top);
            topPanel.Children.Add(optionPanel);
            
            _optionVisualTree = new ListBoxItem();
            _optionVisualTree.Content = "Query designed visual tree.";
            _optionVisualTree.IsSelected = true;

            _optionLocalAutomation = new ListBoxItem();
            _optionLocalAutomation.Content = "Query designed automation tree.";
            _optionLocalAutomation.ToolTip = "Not implemented yet; uses desktop.";

            _optionDesktopAutomation = new ListBoxItem();
            _optionDesktopAutomation.Content = "Query desktop automation tree.";
            
            optionPanel.Items.Add(_optionVisualTree);
            optionPanel.Items.Add(_optionLocalAutomation);
            optionPanel.Items.Add(_optionDesktopAutomation);
            
            // Create box to display results.
            _queryResultsBox = new TextBox();
            DockPanel.SetDock(_queryResultsBox, Dock.Top);
            _queryResultsBox.Height = 120;
            _queryResultsBox.AcceptsReturn = true;
            _queryResultsBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _queryResultsBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            topPanel.Children.Add(_queryResultsBox);

            // Create the control to host applied XAML.
            _designCanvas = new Canvas();
            //DockPanel.SetDock(designCanvas, Dock.Fill);
            _designCanvas.Background = Brushes.Blue;
            topPanel.Children.Add(_designCanvas);
            
            MainWindow.Content = topPanel;

            MainWindow.Show();
        }

        /// <summary>
        /// Return Style for ListBoxItem that will have similar
        /// functionality as RadioButton. RadioButtonList is gone
        /// and ListBox with styling is used as a replacement
        /// </summary>
        /// <returns></returns>
        private Style CreateRadioButtonListStyle()
        {

            FrameworkElementFactory radioButton = new FrameworkElementFactory(typeof(RadioButton), "RadioButton");
            radioButton.SetValue(FrameworkElement.FocusableProperty, false);
            radioButton.SetValue(ContentControl.ContentProperty, new TemplateBindingExtension(ContentPresenter.ContentProperty));

            Binding isChecked = new Binding("IsSelected");
            isChecked.Mode = BindingMode.TwoWay;
            isChecked.RelativeSource = RelativeSource.TemplatedParent;
            radioButton.SetBinding(RadioButton.IsCheckedProperty, isChecked);

            Style radioButtonListStyle = new Style(typeof(ListBoxItem));
            ControlTemplate template = new ControlTemplate(typeof(ListBoxItem));
            template.VisualTree = radioButton;
            radioButtonListStyle.Setters.Add(new Setter(Control.TemplateProperty, template));

            return radioButtonListStyle;
        }

        #endregion Public methods.
        
        #region Private methods.
        
        /// <summary>
        /// Creates an Avaln tree from the user XAML and places
        /// it in the design canvas.
        /// </summary>
        private void ApplyXamlClick(object sender, RoutedEventArgs e)
        {
            object parsedObject;
            FrameworkElement element;
            
            try
            {
                parsedObject = XamlUtils.ParseToObject(_xamlBox.Text);
            }
            catch(System.Windows.Markup.XamlParseException exception)
            {
                MessageBox.Show(exception.ToString());
                return;
            }
            
            element = parsedObject as FrameworkElement;
            if (element == null)
            {
                MessageBox.Show(
                    "Topmost element must be a FrameworkElement-derived type.");
                return;
            }
            
            if (_designCanvas.Children.Count > 0)
            {
                _designCanvas.Children.RemoveAt(0);
            }
            _designCanvas.Children.Add(element);
        }
        
        /// <summary>
        /// Runs the XPath query on the topmost element of the
        /// design canvas and lists results to the user.
        /// </summary>
        private void RunVisualQuery(string xpathQuery, 
            System.Text.StringBuilder results)
        {
            Visual[] visuals;   // Visuals returned.
            
            if (_designCanvas.Children.Count == 0)
            {
                results.Append("Please apply some XAML to explore first.");
                return;
            }
            visuals = XPathNavigatorUtils.ListVisuals(_designCanvas.Children[0], xpathQuery);

            results.Append("Visual query results (" + visuals.Length + " matches)");
            results.Append(Environment.NewLine);
            for (int i = 0; i < visuals.Length; i++)
            {
                results.Append(visuals[i].ToString());
                results.Append(Environment.NewLine);
            }
        }

        /// <summary>
        /// Runs the XPath query, optionally on the topmost element of the
        /// design canvas and lists results to the user.
        /// </summary>
        private void RunAutomationQuery(bool useCanvasRoot, 
            string xpathQuery, System.Text.StringBuilder results)
        {
            AutomationElement[] elements;
            AutomationElement rootElement;
            
            if (useCanvasRoot)
            {
                rootElement = (AutomationElement)AutomationElement.RootElement;
            }
            else
            {
                rootElement = (AutomationElement)AutomationElement.RootElement;
            }

            elements = XPathNavigatorUtils.ListAutomationElements(rootElement, xpathQuery);
            results.Append("Automation query results (" + elements.Length + " matches)");
            results.Append(Environment.NewLine);
            foreach(AutomationElement e in elements)
            {
                results.Append(e.GetCurrentPropertyValue(AutomationElement.NameProperty));
                results.Append(Environment.NewLine);
            }
        }

        /// <summary>
        /// Runs the XPath query on the topmost element of the
        /// design canvas and lists results to the user.
        /// </summary>
        private void RunQueryClick(object sender, RoutedEventArgs e)
        {
            string xpathQuery;
            System.Text.StringBuilder results;

            xpathQuery = _queryBox.Text;
            results = new System.Text.StringBuilder();
            
            try
            {
                if ((bool)_optionVisualTree.IsSelected)
                {
                    RunVisualQuery(xpathQuery, results);
                }
                else if ((bool)_optionLocalAutomation.IsSelected)
                {
                    RunAutomationQuery(true, xpathQuery, results);
                }
                else // optionDesktopAutomation.IsChecked
                {
                    RunAutomationQuery(false, xpathQuery, results);
                }
            }
            catch (Exception exception)
            {
                results.Append(Environment.NewLine);
                results.Append(exception.ToString());
            }
            
            _queryResultsBox.Text = results.ToString();
        }
        
        #endregion Private methods.
        
        #region Private fields.

        /// <summary>Canvas in which to place applied XAML.</summary>
        private Canvas _designCanvas;

        /// <summary>Queries the visual tree.</summary>
        private ListBoxItem _optionVisualTree;
        
        /// <summary>Queries the automation tree from the design root.</summary>
        private ListBoxItem _optionLocalAutomation;

        /// <summary>Queries the automation tree from the desktop.</summary>
        private ListBoxItem _optionDesktopAutomation;

        /// <summary>Control with XPath query text.</summary>
        private TextBox _queryBox;

        /// <summary>Control for query results.</summary>
        private TextBox _queryResultsBox;

        /// <summary>Control with XAML to inspect.</summary>
        private TextBox _xamlBox;

        #endregion Private fields.
    }
}
