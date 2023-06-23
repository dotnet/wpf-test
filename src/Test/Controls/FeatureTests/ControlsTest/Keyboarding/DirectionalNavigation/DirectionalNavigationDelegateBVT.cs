using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Delegate test scenarios testing some complex action
    /// </summary>
    [Test(0, "KeyboardNavigation", "DirectionalNavigationDelegateBVT")]
    public class DirectionalNavigationDelegateBVT : XamlTest
    {
        [Variation("KeyboardNavigationBVT.xaml")]
        public DirectionalNavigationDelegateBVT(string fileName)
            : base(fileName)
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
        }

        Button topButton;
        Button bottomButton;

        private TestResult Setup()
        {
            Status("Setup");

            Panel panel = (Panel)RootElement.FindName("panel");
            if (panel == null)
            {
                throw new NullReferenceException("Fail: the panel is null.");
            }
            KeyboardNavigation.SetDirectionalNavigation(panel, KeyboardNavigationMode.Once);

            topButton = (Button)RootElement.FindName("topButton");
            if (topButton == null)
            {
                throw new NullReferenceException("Fail: the fromElement is null.");
            }

            bottomButton = (Button)RootElement.FindName("bottomButton");
            if (bottomButton == null)
            {
                throw new NullReferenceException("Fail: the toElement is null.");
            }

            // Ensure the window has focus
            InputHelper.MouseClickWindowChrome(Window);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            topButton = null;
            bottomButton = null;
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            using (KeyboardNavigationValidator validator = KeyboardNavigationValidator.GetValidator)
            {
                validator.DirectionalNavigate(topButton, bottomButton, delegate()
                {
                    // make sure the focus stays in container after pressed down arrow key twice
                    for (int i = 0; i < 2; i++)
                    {
                        Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Down);
                        DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    }
                });

                validator.DirectionalNavigate(bottomButton, topButton, delegate()
                {
                    // make sure the focus stays in container after pressed down arrow key twice
                    for (int i = 0; i < 2; i++)
                    {
                        Microsoft.Test.Input.Keyboard.Type(Microsoft.Test.Input.Key.Up);
                        DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    }
                });
            }

            return TestResult.Pass;
        }
    }
}


