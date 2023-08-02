using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Regression Test 
    /// In WPF HwndKeyboardInputProvider tries to track focus on child hwnds to 
    /// be able to restore focus correctly at a later point. WS_POPUP was accidentally 
    /// being treated as a child hwnd and hence was being tracked. I fixed the logic 
    /// so that we track only WS_CHILD hwnds only.
    /// Note: we need to set the test to FullTrust because we are calling win32 apis.
    /// </summary>
    [Test(1, "Popup", TestCaseSecurityLevel.FullTrust, "PopupRegressionTest65")]
    public class PopupRegressionTest65 : XamlTest
    {
        #region Private Members
        Button button;
        #endregion

        #region Public Members

        public PopupRegressionTest65()
            : base(@"PopupRegressionTest65.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(MouseClickButton);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            button = (Button)RootElement.FindName("button");
            if (button == null)
            {
                throw new TestValidationException("Button is null");
            }

            // Create a popup and show it.
            HwndSourceParameters wparams = new HwndSourceParameters("Popup window", 300, 200);
            wparams.WindowStyle = unchecked((int)/*WS_POPUP*/0x80000000 | /*WS_VISIBLE*/0x10000000
                | /*caption*/0x00C00000 | /*SYSMENU*/0x00080000);
            wparams.ParentWindow = new WindowInteropHelper(Window).Handle;
            HwndSource w = new HwndSource(wparams);

            // Set the main window top and left offset to 300, so we can mouse click on the button later.
            Window.Top = 300;
            Window.Left = 300;

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            button = null;
            typeof(EventHelper).InvokeMember("sender", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            typeof(EventHelper).InvokeMember("actualEventArgs", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            return TestResult.Pass;
        }

        /// <summary>
        /// Mouse click button on main window and validate click event fired.
        /// </summary>
        /// <returns></returns>
        public TestResult MouseClickButton()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            RoutedEventArgs routedEventArgs = new RoutedEventArgs(ButtonBase.ClickEvent);
            routedEventArgs.Source = button;

            EventTriggerCallback mouseLeftClickCallback = delegate()
            {
                UserInput.MouseLeftClickCenter(button);
                DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
            };
            EventHelper.ExpectEvent<RoutedEventArgs>(mouseLeftClickCallback, button, "Click", routedEventArgs);

            return TestResult.Pass;
        }

        #endregion
    }
}
