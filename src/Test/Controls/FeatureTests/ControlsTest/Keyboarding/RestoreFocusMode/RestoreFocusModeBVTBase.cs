using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    public abstract class RestoreFocusModeBVTBase : XamlTest
    {
        #region Private Members

        FrameworkElement targetElement;

        #endregion

        public RestoreFocusModeBVTBase(string fileName)
            : base(fileName)
        {
            CleanUpSteps += new TestStep(CleanUp);
        }

        #region Protected Members

        protected virtual TestResult Setup()
        {
            Status("Setup");

            targetElement = (FrameworkElement)RootElement.FindName("targetElement");

            if (targetElement == null)
            {
                throw new NullReferenceException("Fail: the TargetElement is null.");
            }

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        protected virtual TestResult CleanUp()
        {
            targetElement = null;
            return TestResult.Pass;
        }

        protected virtual TestResult RunTest()
        {
            using (MouseClickRestoreFocusModeValidator mouseClickRestoreFocusModeValidator = new MouseClickRestoreFocusModeValidator(targetElement))
            {
                mouseClickRestoreFocusModeValidator.Run();
            }

            using (AltTabRestoreFocusModeValidator altTabRestoreFocusModeValidator = new AltTabRestoreFocusModeValidator(targetElement))
            {
                altTabRestoreFocusModeValidator.Run();
            }

            HwndSource hwndSource = PresentationSource.FromVisual(targetElement) as HwndSource;

            if (hwndSource == null)
            {
                throw new NullReferenceException("Fail: the HwndSource is null.");
            }

            ValidateHwndSourceRestoreFocusMode(hwndSource);

            return TestResult.Pass;
        }

        protected abstract void ValidateHwndSourceRestoreFocusMode(HwndSource hwndSource);

        #endregion
    } 
}


