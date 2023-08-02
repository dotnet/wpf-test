using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Reflection;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Delegate test scenarios because we need to test some complex action
    /// </summary>
    [Test(0, "AccessKeys", "AccessKeyMenuItemEventDelegateBVT", Keywords = "Localization_Suite")]
    public class AccessKeyMenuItemEventDelegateBVT : XamlTest
    {
        [Variation("AccessKeysBVT.xaml", Keywords = "MicroSuite")]
        public AccessKeyMenuItemEventDelegateBVT(string fileName)
            : base(fileName)
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
        }

        MenuItem menuitem;

        private TestResult Setup()
        {
            Status("Setup");

            Panel panel = (Panel)RootElement.FindName("panel");
            if (panel == null)
            {
                throw new NullReferenceException("Fail: the panel is null.");
            }
            KeyboardNavigation.SetDirectionalNavigation(panel, KeyboardNavigationMode.Once);

            menuitem = (MenuItem)RootElement.FindName("submenuitem");
            if (menuitem == null)
            {
                throw new NullReferenceException("Fail: the menuitem is null.");
            }

            // Ensure the window has focus
            InputHelper.MouseClickWindowChrome(Window);

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            menuitem = null;
            typeof(EventHelper).InvokeMember("sender", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            typeof(EventHelper).InvokeMember("actualEventArgs", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            using (AccessKeyValidator validator = AccessKeyValidator.GetValidator)
            {
                RoutedEventArgs routedEventArgs = new RoutedEventArgs(MenuItem.ClickEvent);
                AccessKeyValidator.GetValidator.Validate<RoutedEventArgs>(menuitem, "Click", routedEventArgs, true, delegate()
                {
                    Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.LeftAlt);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.F);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.N);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.LeftAlt);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

                });
                // Cleanup: press the escape key to close the opened menuitem
                Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Escape);
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

                routedEventArgs = new RoutedEventArgs(MenuItem.ClickEvent);
                AccessKeyValidator.GetValidator.Validate<RoutedEventArgs>(menuitem, "Click", routedEventArgs, false, delegate()
                {
                    Microsoft.Test.Input.Keyboard.Press(Microsoft.Test.Input.Key.LeftAlt);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.F);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Z);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    Microsoft.Test.Input.Keyboard.Release(Microsoft.Test.Input.Key.LeftAlt);
                    DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                });
            }

            return TestResult.Pass;
        }
    }
}


