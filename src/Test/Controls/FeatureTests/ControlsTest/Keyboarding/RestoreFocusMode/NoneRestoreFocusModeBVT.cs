using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    [Test(0, "KeyboardNavigation", "RestoreFocusMode", Disabled = true)]
    public class NoneRestoreFocusModeBVT : RestoreFocusModeBVTBase
    {
        #region Private Members

        static NoneRestoreFocusModeBVT()
        {
            System.Windows.Input.Keyboard.DefaultRestoreFocusMode = RestoreFocusMode.None;
        }

        #endregion

        public NoneRestoreFocusModeBVT() : this("Win32EditBoxRestoreFocusModeTest.xaml") { }

        [Variation("Win32EditBoxRestoreFocusModeTest.xaml")]
        [Variation("TextBoxRestoreFocusModeTest.xaml")]
        public NoneRestoreFocusModeBVT(string fileName)
            : base(fileName)
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
        }

        #region Protected Members

        protected override void ValidateHwndSourceRestoreFocusMode(HwndSource hwndSource)
        {
            if (hwndSource.RestoreFocusMode != RestoreFocusMode.None)
            {
                throw new TestValidationException("Fail: HwndSource.RestoreFocusMode is not None after set Keyboard.DefaultRestoreFocusMode to None.");
            }
        }

        #endregion
    }
}


