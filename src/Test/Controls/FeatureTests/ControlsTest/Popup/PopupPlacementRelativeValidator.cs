using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Test;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// It tests PlacementTarget has enough space and not enough space to render Popup on Relative PlacementMode
    /// </summary>
    public class PopupPlacementRelativeValidator : PopupPlacementValidatorBase
    {
        public PopupPlacementRelativeValidator(Popup popup, FrameworkElement placementTarget, bool hasEnoughRoomToRender, bool doesPopupSize, Window window)
            : base(popup, placementTarget, hasEnoughRoomToRender, doesPopupSize, window) { }
        protected override void Setup()
        {
            if (hasEnoughRoomToRender)
            {
                // Make enough room to place popup relative to placementTarget
                mainWindow.Left = 0;
                mainWindow.Top = 0;
            }
            else
            {
                // Make not enough room to place popup relative to placementTarget
                mainWindow.Left = SystemParameters.PrimaryScreenWidth - (placementTarget.ActualWidth / 2);
                mainWindow.Top = SystemParameters.PrimaryScreenHeight - (placementTarget.ActualHeight / 2);
            }

            popup.Placement = PlacementMode.Relative;
        }

        protected override void GetExpectedPopupPosition()
        {
            if (hasEnoughRoomToRender)
            {
                expectedPopupPosition = placementTarget.PointToScreen(new Point());
            }
            else
            {
                // Ensure popup renders on screen
                expectedPopupPosition = new Point(SystemParameters.PrimaryScreenWidth - popupPanel.ActualWidth, SystemParameters.PrimaryScreenHeight - popupPanel.ActualHeight);
            }
        }
    }
}
