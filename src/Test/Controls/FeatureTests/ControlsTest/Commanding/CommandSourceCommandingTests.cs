using System;
using System.Windows.Controls;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Avalon.Test.ComponentModel.Actions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Threading;
using Microsoft.Test.Threading;

namespace Avalon.Test.ComponentModel
{
    [Test(0, "Commanding", "CommandSourceCommandingTests", Keywords = "Localization_Suite,MicroSuite")]
    
    /// <summary>
    /// Excersize the Command features of:
    /// a)  ALL exposed ButtonBase subclasses in the "FrameworkElement" assembly
    /// b)  MenuItem class
    /// --------------------------------------
    /// The tests verify ICommandSource behavior with respect to:
    /// 1. source fires "PreviewCanExecute" and "CanExecute" events when
    ///    its Command property is assigned a RoutedCommand
    /// 2. source is 'enabled' or 'disabled' in agreement with how
    ///    CanExecuteRoutedEventArgs.CanExecute = true/false was set
    ///    in its CanExecute event handler.
    /// 3. source fires "PreviewExecuted" and "Executed" events
    ///    when ENABLED but not DISABLED source is left-clicked.
    /// </summary>
    public class CommandSourceCommandingTests : XamlTest
    {
        #region Private Members
        Panel panel;

        private MenuItem AddMenuItem(Panel panel, String header)
        {
            Menu menu = new Menu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = header;
            menu.Items.Add(menuItem);
            panel.Children.Add(menu);
            return menuItem;
        }
        #endregion

        #region Public Members

        public CommandSourceCommandingTests()
            : base(@"CommandSourceCommandingTests.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunMenuItemCommandingTests);
            RunSteps += new TestStep(RunButtonBaseCommandingTests);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            panel = (Panel)RootElement.FindName("panel");
            if (panel == null)
            {
                throw new TestValidationException("Panel is null");
            }

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            typeof(EventHelper).InvokeMember("sender", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            typeof(EventHelper).InvokeMember("actualEventArgs", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            panel = null;
            return TestResult.Pass;
        }

        public TestResult RunMenuItemCommandingTests()
        {
            WaitForPriority(DispatcherPriority.Render);
            ICommandSource menuItem = AddMenuItem(panel, "Menu Item");

            CommandingActions.RunCommandingTests(menuItem);

            DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        public TestResult RunButtonBaseCommandingTests()
        {
            WaitForPriority(DispatcherPriority.Render);
            Assembly assembly = Assembly.GetAssembly(typeof(FrameworkElement));
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsSubclassOf(typeof(ButtonBase)))
                {
                    ButtonBase buttonBaseObj = Activator.CreateInstance(type) as ButtonBase;
                    buttonBaseObj.Content = type.Name;
                    panel.Children.Add(buttonBaseObj);
                    if (buttonBaseObj is ToggleButton)
                    {
                        ((ToggleButton)buttonBaseObj).IsThreeState = false;
                        ((ToggleButton)buttonBaseObj).IsChecked = false;
                        CommandingActions.RunCommandingTests(buttonBaseObj);

                        panel.Children.Remove(buttonBaseObj);
                        buttonBaseObj = Activator.CreateInstance(type) as ButtonBase;
                        buttonBaseObj.Content = type.Name;
                        panel.Children.Add(buttonBaseObj);

                        ((ToggleButton)buttonBaseObj).IsThreeState = false;
                        ((ToggleButton)buttonBaseObj).IsChecked = null;
                        CommandingActions.RunCommandingTests(buttonBaseObj);

                        ((ToggleButton)buttonBaseObj).IsThreeState = false;
                        ((ToggleButton)buttonBaseObj).IsChecked = true;
                        CommandingActions.RunCommandingTests(buttonBaseObj);

                        ((ToggleButton)buttonBaseObj).IsThreeState = true;
                        ((ToggleButton)buttonBaseObj).IsChecked = false;
                        CommandingActions.RunCommandingTests(buttonBaseObj);

                        ((ToggleButton)buttonBaseObj).IsThreeState = true;
                        ((ToggleButton)buttonBaseObj).IsChecked = null;
                        CommandingActions.RunCommandingTests(buttonBaseObj);

                        ((ToggleButton)buttonBaseObj).IsThreeState = true;
                        ((ToggleButton)buttonBaseObj).IsChecked = true;
                        CommandingActions.RunCommandingTests(buttonBaseObj);
                    }
                    else
                    {
                        CommandingActions.RunCommandingTests(buttonBaseObj);
                    }
                    DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                }
            }

            return TestResult.Pass;
        }
        #endregion
    }
}
