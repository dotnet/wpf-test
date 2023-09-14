using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Popup PlacementMode Relative Test
    /// </summary>
    [Test(0, "Popup", TestCaseSecurityLevel.FullTrust, "PopupPlacementRelativeTest")]
    public class PopupPlacementRelativeTest : XamlTest
    {
        private bool hasEnoughRoomToRender, doesPopupSize;
        private Popup popup;
        private FrameworkElement placementTarget;
        private string ID;

        // Popup size itself
        [Variation("1", "DetermineSizeByPopup.xaml", true, true)]
        [Variation("2", "DetermineSizeByPopup.xaml", false, true)]

        // Popup Panel size Popup
        [Variation("3", "DetermineSizeByContent.xaml", true, false)]
        [Variation("4", "DetermineSizeByContent.xaml", false, false)]
        public PopupPlacementRelativeTest(string ID, string fileName, bool hasEnoughRoomToRender, bool doesPopupSize)
            : base(fileName)
        {
            this.ID = ID;
            this.hasEnoughRoomToRender = hasEnoughRoomToRender;
            this.doesPopupSize = doesPopupSize;
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
        }

        public TestResult Setup()
        {
            LogComment(String.Format("# Test ID {0} #", ID));

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            popup = (Popup)RootElement.FindName("popup");
            if (popup == null)
            {
                throw new NullReferenceException("popup");
            }

            placementTarget = (FrameworkElement)RootElement.FindName("placementTarget");
            if (placementTarget == null)
            {
                throw new NullReferenceException("placementTarget");
            }

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult RunTest()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            PopupPlacementValidatorBase validator = new PopupPlacementRelativeValidator(popup, placementTarget, hasEnoughRoomToRender, doesPopupSize, Window);
            validator.Run();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            
            popup.IsOpen = false;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }
    }
}
