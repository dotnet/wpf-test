using System;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Test press escape key to dismiss menuitem focus
    /// </description>
    /// </summary>
    // Disabling for .NET Core 3, Fix and re-enable.
    //[Test(0, "MenuMode", "MenuModePressEscapeKeyToDismissMenuItemFocus")]
    public class PressEscapeToDismissMenuItemFocusTest : StepsTest
    {
        private string firstXAML, secondXAML, focusTarget, id;
        private bool acquireHwndFocusInMenuMode;

        #region Public Members

        [Variation("1", "", "FocusMenuInOtherWindowMenuModeBVT.xaml", "SimpleMenus.xaml", true)]
        [Variation("2", "", "FocusMenuInOtherWindowMenuModeBVT.xaml", "SimpleMenus.xaml", false)]
        [Variation("3", "Win32TextBox", "FocusMenuInOtherWindowMenuModeBVT.xaml", "SimpleMenus.xaml", true)]
        [Variation("4", "Win32TextBox", "FocusMenuInOtherWindowMenuModeBVT.xaml", "SimpleMenus.xaml", false)]
        [Variation("5", "WPFTextBox", "FocusMenuInOtherWindowMenuModeBVT.xaml", "SimpleMenus.xaml", true)]
        [Variation("6", "WPFTextBox", "FocusMenuInOtherWindowMenuModeBVT.xaml", "SimpleMenus.xaml", false)]
        [Variation("7", "WinFormsControl", "FocusMenuInOtherWindowMenuModeBVT.xaml", "SimpleMenus.xaml", true)]
        [Variation("8", "WinFormsControl", "FocusMenuInOtherWindowMenuModeBVT.xaml", "SimpleMenus.xaml", false)]

        [Variation("9", "", "FocusMenuInSameWindowMenuModeBVT.xaml", "", true)]
        [Variation("10", "", "FocusMenuInSameWindowMenuModeBVT.xaml", "", false)]
        [Variation("11", "Win32TextBox", "FocusMenuInSameWindowMenuModeBVT.xaml", "", true)]
        [Variation("12", "Win32TextBox", "FocusMenuInSameWindowMenuModeBVT.xaml", "", false)]
        [Variation("13", "WPFTextBox", "FocusMenuInSameWindowMenuModeBVT.xaml", "", true)]
        [Variation("14", "WPFTextBox", "FocusMenuInSameWindowMenuModeBVT.xaml", "", false)]
        [Variation("15", "WinFormsControl", "FocusMenuInSameWindowMenuModeBVT.xaml", "", true)]
        [Variation("16", "WinFormsControl", "FocusMenuInSameWindowMenuModeBVT.xaml", "", false)]
        public PressEscapeToDismissMenuItemFocusTest(string id, string focusTarget, string firstXAML, string secondXAML, bool acquireHwndFocusInMenuMode)
        {
            this.id = id;
            this.focusTarget = focusTarget;
            this.firstXAML = firstXAML;
            this.secondXAML = secondXAML;
            this.acquireHwndFocusInMenuMode = acquireHwndFocusInMenuMode;
            RunSteps += new TestStep(Test);
        }

        public TestResult Test()
        {
            LogComment(String.Format("# TestID: {0} #", id));

            MenuModeValidatorBase validator = new PressEscapeToDismissMenuItemFocusValidator(focusTarget, firstXAML, secondXAML, acquireHwndFocusInMenuMode);
            validator.Run();

            return TestResult.Pass;
        }

        #endregion
    }
}
