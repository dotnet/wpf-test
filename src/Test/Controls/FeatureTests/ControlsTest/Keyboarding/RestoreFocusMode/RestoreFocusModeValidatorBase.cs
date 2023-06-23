using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// RestoreFocusMode test framework
    /// 
    /// Overview:
    /// It defines a contract to help people to understand how to test RestoreFocusMode feature in Run method.
    /// It forces the derived test types to implement LoseWindowFocus and SetFocusBackOnWindow methods to provide concrete actions to test this feature. 
    /// It provides some basic validations that people can reuse or override.
    /// It implements the IDisposable pattern for cleanup, so please use "using" statement for the concrete test type.
    /// 
    /// Useage:
    /// using (RestoreFocusModeValidatorBase validator = new MouseClickRestoreFocusModeValidator(targetElement))
    /// {
    ///     validator.Run();
    /// }
    /// </summary>
    public abstract class RestoreFocusModeValidatorBase : IDisposable
    {
        public RestoreFocusModeValidatorBase(FrameworkElement targetElement)
        {
            if (targetElement == null)
            {
                throw new NullReferenceException("Fail: the TargetElement is null.");
            }

            if (!targetElement.Focusable)
            {
                throw new ArgumentException("Fail: the TargetElement is not focusable.");
            }

            if (!targetElement.IsVisible)
            {
                throw new ArgumentException("Fail: the TargetElement is not visible.");
            }

            if (!targetElement.IsHitTestVisible)
            {
                throw new ArgumentException("Fail: the TargetElement is not hittest-visible.");
            }

            if (targetElement is HwndHost)
            {
                isWin32Control = true;
            }

            window = Window.GetWindow(targetElement);

            if (window == null)
            {
                throw new NullReferenceException("Fail: the Window is null.");
            }

            this.targetElement = targetElement;
        }

        #region Private Members

        private bool isWin32Control = false;
        private bool isDisposed = false;
        private FrameworkElement targetElement;

        private void MoveMouseCursorToElementCenter(FrameworkElement targetElement)
        {
            Point elementTopLeftPointToScreenLogicalPixel = targetElement.PointToScreen(new Point());
            Point elementMiddlePointToScreenLogicalPixel = new Point(elementTopLeftPointToScreenLogicalPixel.X + targetElement.ActualWidth / 2, elementTopLeftPointToScreenLogicalPixel.Y + targetElement.ActualHeight / 2);
            if (!InputHelper.IsMouseClickWithinClientArea(window, elementMiddlePointToScreenLogicalPixel))
            {
                throw new ArgumentOutOfRangeException("Fail: the element mouse click point is off window.");
            }

            System.Drawing.Point elementCenterPointToScreen = new System.Drawing.Point(DpiHelper.ConvertToPhysicalPixel(elementMiddlePointToScreenLogicalPixel.X), DpiHelper.ConvertToPhysicalPixel(elementMiddlePointToScreenLogicalPixel.Y));
            Microsoft.Test.Input.Mouse.MoveTo(elementCenterPointToScreen);
        }

        #endregion

        #region Protected Members

        protected Window window;

        protected abstract void LoseWindowFocus();

        protected abstract void SetFocusBackOnWindow();

        protected virtual void ValidateSetFocusBackOnWindow()
        {
            switch (System.Windows.Input.Keyboard.DefaultRestoreFocusMode)
            {
                case RestoreFocusMode.Auto:
                    if (isWin32Control)
                    {
                    }
                    else
                    {
                        if (!targetElement.IsKeyboardFocused)
                        {
                            throw new TestValidationException("Fail: the wpf control doesn't have keyboard focus after Alt+Tab to get Keyboard focus in RestoreFocusMode.Auto.");
                        }
                    }
                    break;
                case RestoreFocusMode.None:
                    if (isWin32Control)
                    {
                        if (((IKeyboardInputSink)targetElement).HasFocusWithin())
                        {
                            throw new TestValidationException("Fail: the win32 control still has keyboard focus after Alt+Tab to get Keyboard focus in RestoreFocusMode.None.");
                        }
                    }
                    else
                    {
                        if (targetElement.IsKeyboardFocused)
                        {
                            throw new TestValidationException("Fail: the wpf control still has keyboard focus after Alt+Tab to get Keyboard focus in RestoreFocusMode.None.");
                        }
                    }
                    break;
            }
        }

        protected virtual void ValidateLostWindowFocus()
        {
            if (isWin32Control)
            {
                if (((IKeyboardInputSink)targetElement).HasFocusWithin())
                {
                    throw new TestValidationException("Fail: the win32 control still has keyboard focus after Alt+Tab to lose Keyboard focus.");
                }
            }
            else
            {
                if (targetElement.IsKeyboardFocused)
                {
                    throw new TestValidationException("Fail: the wpf control still has keyboard focus after Alt+Tab to lose Keyboard focus.");
                }
            }
        }

        protected virtual void SetFocusOnTargetElement()
        {
            MoveMouseCursorToElementCenter(targetElement);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

            if (isWin32Control)
            {
                if (!((IKeyboardInputSink)targetElement).HasFocusWithin())
                {
                    throw new TestValidationException("Fail: the win32 control doesn't have keyboard focus after click.");
                }
            }
            else
            {
                if (!targetElement.IsKeyboardFocused)
                {
                    throw new TestValidationException("Fail: the wpf control doesn't have keyboard focus after click.");
                }
            }

            if (!window.IsActive)
            {
                throw new TestValidationException("Fail: the window is inactive before try to lose window focus.");
            }
        }

        #endregion

        /// <summary>
        /// Run test in following order, set focus on targetElement, lose window foucs, 
        /// validate lost window focus, set focus back on window, validate set focus back on window.
        /// </summary>
        public void Run()
        {
            SetFocusOnTargetElement();

            LoseWindowFocus();

            ValidateLostWindowFocus();

            SetFocusBackOnWindow();

            ValidateSetFocusBackOnWindow();
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            // When finalizer calls Microsoft.Test.Input.WpfMouse.Reset() will cause exception below.
            // "The calling thread must be STA, because many UI components require this."
            // So, remove finalizer.
            if (!isDisposed)
            {
                Microsoft.Test.Input.Mouse.Reset();
                DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);
                Microsoft.Test.Input.Keyboard.Reset();
                DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

                isDisposed = true;
            }
        }

        public void Cleanup()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
