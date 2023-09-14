using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Threading;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// MenuModeValidatorBase
    /// </summary>
    public abstract class MenuModeValidatorBase
    {
        private string firstXAML, focusTarget;

        protected string secondXAML;
        protected Window firstWindow;
        protected Window secondWindow;

        public MenuModeValidatorBase(string focusTarget, string firstXAML, string secondXAML, bool acquireHwndFocusInMenuMode)
        {
            HwndSource.DefaultAcquireHwndFocusInMenuMode = acquireHwndFocusInMenuMode;

            if (String.IsNullOrEmpty(firstXAML))
            {
                throw new ArgumentNullException("Fail: firstXAML is empty");
            }

            this.focusTarget = focusTarget;
            this.firstXAML = firstXAML;
            this.secondXAML = secondXAML;
        }

        private void LaunchApp()
        {
            Application app = new Application();
            app.Run(firstWindow);
        }

        protected virtual void Setup()
        {
            // Set ResourceAssembly so we can load XAML with relative Uris from this assembly.
            Application.ResourceAssembly = Assembly.GetExecutingAssembly();

            firstWindow = (Window)Application.LoadComponent(new Uri(firstXAML, UriKind.RelativeOrAbsolute));
            firstWindow.Width = 300;
            firstWindow.Height = 300;
            firstWindow.Top = 0;
            firstWindow.Left = 0;
            firstWindow.Show();

            if (String.IsNullOrEmpty(secondXAML))
            {
                secondWindow = firstWindow;
            }
            else
            {
                secondWindow = (Window)Application.LoadComponent(new Uri(secondXAML, UriKind.RelativeOrAbsolute));
                secondWindow.Width = 300;
                secondWindow.Height = 300;
                secondWindow.Top = 0;
                secondWindow.Left = 350;
                secondWindow.Show();
            }

            if (String.IsNullOrEmpty(focusTarget))
            {
                // Ensure the first window has focus and active
                InputHelper.MouseClickWindowChrome(firstWindow);
                DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
            }
            else
            {
                // Set focus on target element that automatic make the window active
                FrameworkElement targetElement = (FrameworkElement)firstWindow.FindName(focusTarget);

                if (targetElement == null)
                {
                    throw new TestValidationException("Fail: targetElement is null");
                }

                InputHelper.MouseClickCenter(targetElement, System.Windows.Input.MouseButton.Left);
                DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
            }
        }

        protected abstract void Test();

        public void Run()
        {
            Setup();
            Test();
            LaunchApp();
        }
    }
}
