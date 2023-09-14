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
    /// Popup PlacementMode Top, Bottom, Left and Right Test
    /// </summary>
    [Test(0, "Popup", TestCaseSecurityLevel.FullTrust, "PopupPlacementTopBottomLeftRightTest")]
    public class PopupPlacementTopBottomLeftRightTest : XamlTest
    {
        private bool hasEnoughRoomToRender, doesPopupSize, isTargetRenderOnScreen;
        private Popup popup;
        private FrameworkElement placementTarget;
        private PlacementMode placementMode;
        private string ID;

        [Variation("1", "DetermineSizeByPopup.xaml", PlacementMode.Top, true, true, true)]
        [Variation("2", "DetermineSizeByPopup.xaml", PlacementMode.Top, false, true, true)]
        [Variation("3", "DetermineSizeByContent.xaml", PlacementMode.Top, true, false, true)]
        [Variation("4", "DetermineSizeByContent.xaml", PlacementMode.Top, false, false, true)]
        [Variation("5", "DetermineSizeByPopup.xaml", PlacementMode.Top, true, true, false)]
        [Variation("6", "DetermineSizeByPopup.xaml", PlacementMode.Top, false, true, false)]
        [Variation("7", "DetermineSizeByContent.xaml", PlacementMode.Top, true, false, false)]
        [Variation("8", "DetermineSizeByContent.xaml", PlacementMode.Top, false, false, false)]

        [Variation("9", "DetermineSizeByPopup.xaml", PlacementMode.Bottom, true, true, true)]
        [Variation("10", "DetermineSizeByPopup.xaml", PlacementMode.Bottom, false, true, true)]
        [Variation("11", "DetermineSizeByContent.xaml", PlacementMode.Bottom, true, false, true)]
        [Variation("12", "DetermineSizeByContent.xaml", PlacementMode.Bottom, false, false, true)]
        [Variation("13", "DetermineSizeByPopup.xaml", PlacementMode.Bottom, true, true, false)]
        [Variation("14", "DetermineSizeByPopup.xaml", PlacementMode.Bottom, false, true, false)]
        [Variation("15", "DetermineSizeByContent.xaml", PlacementMode.Bottom, true, false, false)]
        [Variation("16", "DetermineSizeByContent.xaml", PlacementMode.Bottom, false, false, false)]

        [Variation("17", "DetermineSizeByPopup.xaml", PlacementMode.Left, true, true, true)]
        [Variation("18", "DetermineSizeByPopup.xaml", PlacementMode.Left, false, true, true)]
        [Variation("19", "DetermineSizeByContent.xaml", PlacementMode.Left, true, false, true)]
        [Variation("20", "DetermineSizeByContent.xaml", PlacementMode.Left, false, false, true)]
        [Variation("21", "DetermineSizeByPopup.xaml", PlacementMode.Left, true, true, false)]
        [Variation("22", "DetermineSizeByPopup.xaml", PlacementMode.Left, false, true, false)]
        [Variation("23", "DetermineSizeByContent.xaml", PlacementMode.Left, true, false, false)]
        [Variation("24", "DetermineSizeByContent.xaml", PlacementMode.Left, false, false, false)]

        [Variation("25", "DetermineSizeByPopup.xaml", PlacementMode.Right, true, true, true)]
        [Variation("26", "DetermineSizeByPopup.xaml", PlacementMode.Right, false, true, true)]
        [Variation("27", "DetermineSizeByContent.xaml", PlacementMode.Right, true, false, true)]
        [Variation("28", "DetermineSizeByContent.xaml", PlacementMode.Right, false, false, true)]
        [Variation("29", "DetermineSizeByPopup.xaml", PlacementMode.Right, true, true, false)]
        [Variation("30", "DetermineSizeByPopup.xaml", PlacementMode.Right, false, true, false)]
        [Variation("31", "DetermineSizeByContent.xaml", PlacementMode.Right, true, false, false)]
        [Variation("32", "DetermineSizeByContent.xaml", PlacementMode.Right, false, false, false)]
        public PopupPlacementTopBottomLeftRightTest(string ID, string fileName, PlacementMode placementMode, bool hasEnoughRoomToRender, bool doesPopupSize, bool isTargetRenderOnScreen)
            : base(fileName)
        {
            this.ID = ID;
            this.hasEnoughRoomToRender = hasEnoughRoomToRender;
            this.doesPopupSize = doesPopupSize;
            this.isTargetRenderOnScreen = isTargetRenderOnScreen;
            this.placementMode = placementMode;
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

            PopupPlacementValidatorBase validator = new PopupPlacementTopBottomLeftRightValidator(popup, placementMode, placementTarget, hasEnoughRoomToRender, doesPopupSize, isTargetRenderOnScreen, Window);
            validator.Run();
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            
            popup.IsOpen = false;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }
    }
}
