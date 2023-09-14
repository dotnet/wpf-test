using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Test RestoreFocusMode per thread framework
    /// 
    /// Overview:
    /// It provides the basic test infrastructure for testing RestoreFocusMode on new worker thread scenario.
    /// If forces the derived test types to implement AttachContentToWindow method to provide a control is tested on.
    /// 
    /// Usage:
    /// Win32ControlsRestoreFocusModePerThreadValidator validator = new Win32ControlsRestoreFocusModePerThreadValidator(RestoreFocusMode.Auto);
    /// validator.Run();
    /// </summary>
    public abstract class RestoreFocusModePerThreadValidatorBase
    {
        public RestoreFocusModePerThreadValidatorBase(RestoreFocusMode restoreFocusMode)
        {
            this.restoreFocusMode = restoreFocusMode;
        }

        private RestoreFocusMode restoreFocusMode;

        protected virtual void SetupWPFWindow()
        {
            System.Windows.Input.Keyboard.DefaultRestoreFocusMode = restoreFocusMode;

            Window window = new Window();
            window.Width = 300;
            window.Height = 300;
            window.Show();

            if (!window.IsActive)
            {
                throw new TestValidationException("Fail: the new thread window is inactive.");
            }

            AttachContentToWindow(window);

            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
        }

        protected abstract void AttachContentToWindow(Window window);

        protected virtual void RunRestoreFocusModeTests(FrameworkElement targetElement)
        {
            // Test win32 control EditBox
            using (MouseClickRestoreFocusModeValidator validator = new MouseClickRestoreFocusModeValidator(targetElement))
            {
                validator.Run();
            }

            using (AltTabRestoreFocusModeValidator validator = new AltTabRestoreFocusModeValidator(targetElement))
            {
                validator.Run();
            }
        }

        /// <summary>
        /// Run RestoreFocusMode tests on a new thread.
        /// </summary>
        public void Run()
        {
            Thread workThread = new Thread(SetupWPFWindow);
            workThread.SetApartmentState(ApartmentState.STA);
            workThread.IsBackground = true;
            workThread.Start();

            workThread.Join();
        }
    }
}
