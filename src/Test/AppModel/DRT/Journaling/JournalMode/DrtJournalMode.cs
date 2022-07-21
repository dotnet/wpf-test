// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Security.Permissions;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Input;

namespace Test
{
    /// <summary>
    /// This DRT is designed to test JournalEntry.KeepAlive. It navigates to a xaml page,
    /// a programmatic tree, and a programmatic tree with KeepAlive set to false.
    /// It then navigates all the way back and all the way forward, checking the tree and the persisted state of the 
    /// TextBox.
    /// </summary>

    class DrtJournalKeepAliveApplication : Application
    {
        static bool s_failed = false;
        int _state = 0;
        
        NavigationWindow _mainWindow;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            _mainWindow = new NavigationWindow();
            _mainWindow.SandboxExternalContent = false;
            _mainWindow.Width = 200;
            _mainWindow.Height = 100;
            _mainWindow.Show();
            DoStateMachine();
        }
    
        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            DoStateMachine();
        }
        
        object _uriDefault; // will be mapped to Uri
        object _programmaticDefault; // will be mapped to KeepAlive
        object _objectKeepAlive;

        private void DoStateMachine()
        {
            if (s_failed)
                return;

            ArrayList backEntries = new ArrayList();
            ArrayList forwardEntries = new ArrayList();
            switch (_state)
            {
                case 0 :
                    //**********************************************************************************
                    // Location: nowhere
                    //**********************************************************************************
                    CheckBackForwardState(false, false);
                    // Navigate to a xaml page by URI--JournalEntry.KeepAlive=false; JournalEntry.Name="by uri"
                    _mainWindow.Navigate(new Uri("pack://siteoforigin:,,,/DrtFiles/JournalMode/DrtJournalModeDefault.xaml", UriKind.RelativeOrAbsolute));
                    break;
                case 1:
                    //**********************************************************************************
                    // Location: DrtJournalModeDefault.xaml
                    //**********************************************************************************
                    CheckBackForwardState(false, false);
                    // Save the tree and modify it
                    _uriDefault = _mainWindow.Content;
                    AssertNotNull(_mainWindow.Source, "Should have URI for URI navigation");
                    AssertIsTrue(_mainWindow.Source.ToString().EndsWith("DrtJournalModeDefault.xaml"), "Should have the correct URI but got:" + _mainWindow.Source.ToString());
                    this.SetTextBox(_mainWindow.Content, "New content");
                    // Navigate to a programmatic tree--use the default JournalEntry.KeepAlive JournalEntry.Name="KeepAlive default for obj"
                    _mainWindow.Navigate(this.GetProgrammaticTree(false, "KeepAlive default for obj"));
                    break;
                case 2:
                    //**********************************************************************************
                    // Location: ProgrammaticTree Default = KeepAlive
                    //**********************************************************************************
                    CheckBackForwardState(true, false);

                    //**********************************************************************************
                    // Check NavigationWindow button drop down menu: 
                    // Current state: BackButton: "by uri"; Forward: null
                    //**********************************************************************************                                        
                    backEntries.Add("by uri");                    
                    CheckDropDownMenu(backEntries, forwardEntries);

                    // Save the tree and modify it
                    _programmaticDefault = _mainWindow.Content;
                    AssertNull(_mainWindow.Source, "Should not have URI for programmatic tree");
                    this.SetTextBox(_mainWindow.Content, "New content");
                    // Navigate to a programmatic tree--JournalEntry.KeepAlive=false JournalEntry.Name="KeepAlive false for obj"
                    _mainWindow.Navigate(this.GetProgrammaticTree(false, "KeepAlive false for obj"));
                    break;
                case 3:
                    //**********************************************************************************
                    // Location: ProgrammaticTree Remain KeepAlive
                    //**********************************************************************************
                    CheckBackForwardState(true, false);

                    //**********************************************************************************
                    // Check NavigationWindow button drop down menu: 
                    // Current state: BackButton: "by uri" + "KeepAlive default for obj"; Forward: null
                    //**********************************************************************************  
                    backEntries.Add("KeepAlive default for obj");
                    backEntries.Add("by uri");                    
                    CheckDropDownMenu(backEntries, forwardEntries);

                    // Save the tree
                    _objectKeepAlive = _mainWindow.Content;
                    AssertNull(_mainWindow.Source, "Should not have URI for programmatic tree");
                    this.SetTextBox(_mainWindow.Content, "New content");
                    // Go back to the previous page (serialized programmatic)
                    _mainWindow.GoBack();
                    break;
                case 4:
                    //**********************************************************************************
                    // Location: ProgrammaticTree Default = KeepAlive
                    //**********************************************************************************
                    CheckBackForwardState(true, true);

                    //**********************************************************************************
                    // Check NavigationWindow button drop down menu: 
                    // Current state: BackButton: "by uri"; Forward: "KeepAlive false for obj"
                    //**********************************************************************************                                        
                    backEntries.Add("by uri");
                    forwardEntries.Add("KeepAlive false for obj");
                    CheckDropDownMenu(backEntries, forwardEntries);

                    AssertNull(_mainWindow.Source, "Should not have URI for programmatic tree");
                    AssertIsTrue(_programmaticDefault == _mainWindow.Content, "Should have kept the tree if a programmatic tree is journaled by KeepAlive");
                    AssertIsTrue(this.GetTextBox(_mainWindow.Content) == "New content", "Should have maintained modified state of TextBox");
                    _mainWindow.GoBack(); // to default URI
                    break;
                case 5:                    
                    //**********************************************************************************
                    // Location: DrtJournalModeDefault.xaml
                    //**********************************************************************************
                    CheckBackForwardState(false, true);

                    //**********************************************************************************
                    // Check NavigationWindow button drop down menu: 
                    // Current state: Forward: "KeepAlive default for obj", "KeepAlive false for obj"
                    //**********************************************************************************                                        
                    forwardEntries.Add("KeepAlive default for obj");
                    forwardEntries.Add("KeepAlive false for obj");
                    CheckDropDownMenu(backEntries, forwardEntries);

                    AssertNotNull(_mainWindow.Source, "Should have URI for tree navigated to by URI");
                    AssertIsTrue(_mainWindow.Source.ToString().EndsWith("DrtJournalModeDefault.xaml"), "Should have the correct URI");
                    AssertIsFalse(_uriDefault == _mainWindow.Content, "Journaling by URI should not keep the tree");
                    AssertIsTrue(this.GetTextBox(_mainWindow.Content) == "New content", "Should maintain modified TextBox");
                    _mainWindow.GoForward(); // to KeepAlive programmatic tree
                    break;
                case 6 :
                    //**********************************************************************************
                    // Location: ProgrammaticTree Default = KeepAlive
                    //**********************************************************************************
                    CheckBackForwardState(true, true);

                    //**********************************************************************************
                    // Check NavigationWindow button drop down menu: 
                    // Current state: BackButton: "by uri" ; Forward: KeepAlive false for obj
                    //**********************************************************************************                                        
                    backEntries.Add("by uri");
                    forwardEntries.Add("KeepAlive false for obj");
                    CheckDropDownMenu(backEntries, forwardEntries);

                    AssertIsTrue(_programmaticDefault == _mainWindow.Content, "Should have kept the tree if a programmatic tree is journaled by KeepAlive (going forward)");
                    AssertIsTrue(this.GetTextBox(_mainWindow.Content) == "New content", "Should have maintained modified state of TextBox (going forward)");

                    _mainWindow.GoForward(); // to default URI
                    break;
               case 7 :
                    //**********************************************************************************
                    // Location: ProgrammaticTree Remain KeepAlive
                    //**********************************************************************************
                    CheckBackForwardState(true, false);
                    AssertIsTrue(_objectKeepAlive == _mainWindow.Content, "Should have kept the tree since a programmatic tree is always kept alive");
                    AssertIsTrue(this.GetTextBox(_mainWindow.Content) == "New content", "Should have maintained modified state of TextBox (going forward)");
                    this.Shutdown();
                    break;
            }
            ++_state;
        }

        private void AssertIsFalse(bool testCondition, string message)
        {
            if (testCondition)
                throw new ApplicationException("[state=" + _state + "] " + message);
        }

        private void AssertIsTrue(bool testCondition, string message)
        {
            if (!testCondition)
                throw new ApplicationException("[state=" + _state + "] " + message);
        }

        private void AssertNotNull(object obj, string message)
        {
            if (obj == null)
                throw new ApplicationException("[state=" + _state + "] " + message);
        }

        private void AssertNull(object obj, string message)
        {
            if (obj != null)
                throw new ApplicationException("[state=" + _state + "] " + message);
        }
        
        private void CheckBackForwardState(bool canGoBack, bool canGoForward)
        {
            if (_mainWindow.CanGoBack != canGoBack || 
		        ((bool)_mainWindow.GetValue(NavigationWindow.CanGoBackProperty)) != canGoBack)
                throw new ApplicationException("[state=" + _state + "] CanGoBack state is incorrect");
            if (_mainWindow.CanGoForward != canGoForward || 
		        ((bool)_mainWindow.GetValue(NavigationWindow.CanGoForwardProperty)) != canGoForward)
                throw new ApplicationException("[state=" + _state + "] CanGoForward state is incorrect");

        }

        private FrameworkElement GetProgrammaticTree(bool keepAlive, string name)
        {
            Grid panel = new Grid();

            JournalEntry.SetKeepAlive(panel, keepAlive);
            JournalEntry.SetName(panel, name);

            TextBox textBox = new TextBox();
            panel.Children.Add(textBox);
            
            textBox.Text = "JournalEntry.KeepAlive=" + keepAlive + "; JournalEntry.Name=" + name;

            return panel;
        }

        private void SetTextBox(object root, string id)
        {
            Grid panel = root as Grid;

            AssertNotNull(panel, "Setup: Should have found a panel");
            AssertIsTrue(panel.Children.Count == 1, "Setup: Panel should have one child");

            TextBox textBox = panel.Children[0] as TextBox;

            AssertNotNull(textBox, "Setup: Should have found a TextBox as the child of the panel");
            textBox.Text = id;
        }

        private string GetTextBox(object root)
        {
            Grid panel = root as Grid;

            AssertNotNull(panel, "Setup: Should have found a panel");
            AssertIsTrue(panel.Children.Count == 1, "Panel should have one child");

            TextBox textBox = panel.Children[0] as TextBox;

            AssertNotNull(textBox, "Setup: Should have found a TextBox as the child of the panel");

            return textBox.Text;
        }

        private Button GetBackButton()
        {
            return (Button)FindVisualByPropertyValue(Button.CommandProperty, NavigationCommands.BrowseBack);
        }

        private Button GetForwardButton()
        {
            return (Button)FindVisualByPropertyValue(Button.CommandProperty, NavigationCommands.BrowseForward);
        }


        private void CheckDropDownMenu(ArrayList backDropDown, ArrayList forwardDropDown)
        {
            Menu menu = (Menu)FindVisualByType(typeof(Menu), _mainWindow);
            MenuItem menuItem = (MenuItem)menu.Items[0];

            int count = menuItem.Items.Count;
            if (count != (backDropDown.Count + forwardDropDown.Count + 1))
            {
                throw new ApplicationException("MenuItem.Count[" + count + "] is different from actual count[" + (backDropDown.Count + forwardDropDown.Count + 1) + "]");
            }

            for (int index = 0; index < forwardDropDown.Count; ++index)
            {
                JournalEntry journalEntry = (JournalEntry)menuItem.Items[index];
                if (journalEntry.Name.CompareTo(forwardDropDown[forwardDropDown.Count - 1 - index]) != 0)
                {
                    throw new ApplicationException("JournalEntry name [" + journalEntry.Name + "] is different from actual name[" + forwardDropDown[forwardDropDown.Count - 1 - index] + "]");
                }
            }

            DependencyObject current = menuItem.Items[forwardDropDown.Count] as DependencyObject;

            if (current == null || ((string)current.GetValue(JournalEntry.NameProperty)) != "Current Page")
            {
                throw new ApplicationException("MenuItem[" + forwardDropDown.Count + "] is not Current Page");
            }

            for (int index = forwardDropDown.Count + 1; index < count; ++index)
            {
                JournalEntry journalEntry = (JournalEntry)menuItem.Items[index];
                if (journalEntry.Name.CompareTo(backDropDown[index - forwardDropDown.Count - 1]) != 0)
                {
                    throw new ApplicationException("JournalEntry name [" + journalEntry.Name + "] is different from actual name[" + backDropDown[index - forwardDropDown.Count - 1] + "]");
                }
            }
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        /// <returns></returns>
        public DependencyObject FindVisualByPropertyValue(DependencyProperty dp, object value, DependencyObject node, bool includeNode)
        {
            // see if the node itself has the right value
            if (includeNode)
            {
                object nodeValue = node.GetValue(dp);
                if (Object.Equals(value, nodeValue))
                    return node;
            }

            // if not, recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(node);
            for(int i = 0; i < count; i++)
            {
                DependencyObject result = FindVisualByPropertyValue(dp, value, VisualTreeHelper.GetChild(node,i), true);
                if (result != null)
                    return result;
            }

            // not found
            return null;
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <returns></returns>
        public DependencyObject FindVisualByPropertyValue(DependencyProperty dp, object value, DependencyObject node)
        {
            return FindVisualByPropertyValue(dp, value, node, true);
        }

        /// <summary>
        /// Do a depth-first search of the visual tree (starting at the root)
        /// looking for a node with a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <returns></returns>
        /// <example>
        /// For example, to find the element with ID "foo", call
        ///  DRT.FindVisualByPropertyValue(IDProperty, "foo");
        /// </example>
        public DependencyObject FindVisualByPropertyValue(DependencyProperty dp, object value)
        {
            return FindVisualByPropertyValue(dp, value, this.MainWindow);
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given type.
        /// </summary>
        /// <param name="type">type of desired node</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        public DependencyObject FindVisualByType(Type type, DependencyObject node, bool includeNode)
        {
            // see if the node itself has the right type
            if (includeNode)
            {
                if (type == node.GetType())
                    return node;
            }

            // if not, recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(node);
            for(int i = 0; i < count; i++)
            {
                DependencyObject result = FindVisualByType(type, VisualTreeHelper.GetChild(node,i), true);
                if (result != null)
                    return result;
            }

            // not found
            return null;
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given type.
        /// </summary>
        /// <param name="type">type of desired node</param>
        /// <param name="node">starting node for the search</param>
        public DependencyObject FindVisualByType(Type type, DependencyObject node)
        {
            return FindVisualByType(type, node, true);
        }

        [STAThread]
        public static int Main()
        {
            try
            {
                new DrtJournalKeepAliveApplication().Run();
            }
            catch (ApplicationException e)
            {
                s_logger.Log(e.Message);
                s_failed = true;
            }

            if (s_failed)
            {
                s_logger.Log("Failed.");
                return 1;
            }
            else
            {
                s_logger.Log("Passed.");
                return 0;
            }
        }


        private static DRT.Logger s_logger = new DRT.Logger("DrtJournalMode", "Microsoft", "Testing JournalMode property");

    }
}

