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
    /// Regression Test  - Multiple popups with StaysOpen=False don't work as expected.
    /// </summary>
    [Test(1, "Popup", TestCaseSecurityLevel.FullTrust, "ChainedAutoClosePopups", Versions="4.7.1+")]
    public class ChainedAutoClosePopups : XamlTest
    {
        #region Private Members
        Button[] buttons = new Button[5];
        Popup[] popups = new Popup[5];
        #endregion

        #region Public Members

        public ChainedAutoClosePopups()
            : base(@"ChainedAutoClosePopups.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestMultipleAutoClosePopups);
            CleanUpSteps += new TestStep(CleanUp);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            for (int i=0; i<5; ++i)
            {
                buttons[i] = (Button)RootElement.FindName("button" + (i+1));
                popups[i] = (Popup)RootElement.FindName("popup" + (i+1));

                if (buttons[i] == null)
                {
                    throw new TestValidationException("button" + (i+1) + " is null");
                }
                if (popups[i] == null)
                {
                    throw new TestValidationException("popup" + (i+1) + " is null");
                }

                buttons[i].Click += new RoutedEventHandler(OnClick);
            }

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        void OnClick(object sender, RoutedEventArgs e)
        {
            int index;

            // find which button was clicked
            for (index = 0; index < 5; ++index)
            {
                if (sender == buttons[index])
                    break;
            }

            // open the corresponding popup
            popups[index].IsOpen = true;
        }

        public TestResult CleanUp()
        {
            for (int i=0; i<5; ++i)
            {
                buttons[i] = null;
                popups[i] = null;
            }
            return TestResult.Pass;
        }

        /// <summary>
        /// Open and close many levels of auto-close popups, confirm that they open/close
        /// </summary>
        /// <returns></returns>
        public TestResult TestMultipleAutoClosePopups()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            for (int i=0; i<5; ++i)
            {
                if (!VerifyOpen(0)) return TestResult.Fail;

                // open the first i popups (by clicking the first i buttons)
                TestLog.Current.LogStatus("Open {0} popups", i+1);
                for (int j=0; j<=i; ++j)
                {
                    UserInput.MouseLeftClickCenter(buttons[j]);
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                }

                if (!VerifyOpen(i+1)) return TestResult.Fail;

                // close the first i popups one by one, by clicking in the surrounding popup
                TestLog.Current.LogStatus("Close {0} popups, clicking on parent", i+1);
                for (int j=i; j>=0; --j)
                {
                    UserInput.MouseLeftDown(buttons[j], -4, -4);
                    UserInput.MouseLeftUp(buttons[j], -4, -4);

                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    if (!VerifyOpen(j)) return TestResult.Fail;
                }
            }

            for (int i=0; i<5; ++i)
            {
                if (!VerifyOpen(0)) return TestResult.Fail;

                // open the first i popups (by clicking the first i buttons)
                TestLog.Current.LogStatus("Open {0} popups", i+1);
                for (int j=0; j<=i; ++j)
                {
                    UserInput.MouseLeftClickCenter(buttons[j]);
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                }

                if (!VerifyOpen(i+1)) return TestResult.Fail;

                // close the first i popups one by one, by clicking outside all popups
                TestLog.Current.LogStatus("Close {0} popups, clicking outside all", i+1);
                for (int j=i; j>=0; --j)
                {
                    UserInput.MouseLeftDown(buttons[0], -4, -4);
                    UserInput.MouseLeftUp(buttons[0], -4, -4);

                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    if (!VerifyOpen(j)) return TestResult.Fail;
                }
            }

            return TestResult.Pass;
        }

        // verify that the first n popups are open, and the rest are closed
        bool VerifyOpen(int n)
        {
            bool result = true;
            for (int i=0; i<5; ++i)
            {
                bool expected = (i<n);
                bool actual = popups[i].IsOpen;
                if (expected != actual)
                {
                    TestLog.Current.LogStatus("Popup{0}.IsOpen is wrong.  Expected '{1}', actual '{2}'",
                        i, expected, actual);
                    result = false;
                }                       

            }

            return result;
        }

        #endregion
    }
}

