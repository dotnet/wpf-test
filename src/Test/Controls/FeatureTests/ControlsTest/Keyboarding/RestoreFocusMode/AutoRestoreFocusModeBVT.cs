using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    [Test(0, "KeyboardNavigation", "RestoreFocusMode", Disabled = true)]
    public class AutoRestoreFocusModeBVT : RestoreFocusModeBVTBase
    {
        #region Private Members

        static AutoRestoreFocusModeBVT()
        {
            System.Windows.Input.Keyboard.DefaultRestoreFocusMode = RestoreFocusMode.Auto;
        }

        #endregion

        public AutoRestoreFocusModeBVT() : this("Win32EditBoxRestoreFocusModeTest.xaml") { }

        [Variation("Win32EditBoxRestoreFocusModeTest.xaml")]
        [Variation("TextBoxRestoreFocusModeTest.xaml")]
        public AutoRestoreFocusModeBVT(string fileName)
            : base(fileName)
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
        }

        #region Protected Members

        protected override void ValidateHwndSourceRestoreFocusMode(HwndSource hwndSource)
        {
            if (hwndSource.RestoreFocusMode != RestoreFocusMode.Auto)
            {
                throw new TestValidationException("Fail: HwndSource.RestoreFocusMode is not Auto after set Keyboard.DefaultRestoreFocusMode to Auto.");
            }
        }

        #endregion
    }
}


