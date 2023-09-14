using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Test;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Test Popup PlacementMode Top, Bottom, Left, Right on following scenarios:
    ///    - PlacementTarget that renders on screen has enough space to render Popup
    ///    - PlacementTarget that renders on screen doesn't have enough space to render Popup
    ///    - PlacementTarget that renders off screen has enough space to render Popup
    ///    - PlacementTarget that renders off screen doesn't have enough space to render Popup
    /// </summary>
    public class PopupPlacementTopBottomLeftRightValidator : PopupPlacementValidatorBase
    {
        private PlacementMode placementMode;
        private double expectedPopupPositionX, expectedPopupPositionY;
        private bool isTargetRenderedOnScreen;

        public PopupPlacementTopBottomLeftRightValidator(Popup popup, PlacementMode placementMode, FrameworkElement placementTarget, bool hasEnoughRoomToRender, bool doesPopupSize, bool isTargetRenderedOnScreen, Window window)
            : base(popup, placementTarget, hasEnoughRoomToRender, doesPopupSize, window)
        {
            this.placementMode = placementMode;
            this.isTargetRenderedOnScreen = isTargetRenderedOnScreen;

            popup.Placement = placementMode;
        }

        protected override void Setup()
        {
            switch (placementMode)
            {
                case PlacementMode.Top:
                case PlacementMode.Bottom:
                    if (isTargetRenderedOnScreen)
                    {
                        mainWindow.Left = 0;
                    }
                    else
                    {
                        // For now, we test left offset screen. We could add right and other scenarios when we have time
                        mainWindow.Left = -(sizePopupElement.Width / 2);
                    }
                    break;
                case PlacementMode.Left:
                case PlacementMode.Right:
                    if (isTargetRenderedOnScreen)
                    {
                        mainWindow.Top = 0;
                    }
                    else
                    {
                        // For now, we test top offset screen. We could add bottom and other scenarios when we have time
                        mainWindow.Top = -(sizePopupElement.Height / 2);
                    }
                    break;
                default:
                    throw new NotSupportedException(String.Format("UnSupported PlacementMode {0}", placementMode));
            }

            if (hasEnoughRoomToRender)
            {
                switch (placementMode)
                {
                    // Make enough room to place popup on PlacementTarget based on PlacementMode
                    case PlacementMode.Top:
                        mainWindow.Top = sizePopupElement.Height + sizePopupElement.Height / 2;
                        break;
                    case PlacementMode.Bottom:
                        mainWindow.Top = SystemParameters.PrimaryScreenHeight - (placementTarget.ActualHeight + sizePopupElement.Height + sizePopupElement.Height / 2);
                        break;
                    case PlacementMode.Left:
                        mainWindow.Left = sizePopupElement.Width + sizePopupElement.Width / 2;
                        break;
                    case PlacementMode.Right:
                        mainWindow.Left = SystemParameters.PrimaryScreenWidth - (placementTarget.ActualWidth + sizePopupElement.Width + sizePopupElement.Width / 2);
                        break;
                    default:
                        throw new NotSupportedException(String.Format("UnSupported PlacementMode {0}", placementMode));
                }
            }
            else
            {
                switch (placementMode)
                {
                    // Make not enough room to place popup on PlacementTarget based on PlacementMode to force popup render on the opposite side of PlacementMode
                    case PlacementMode.Top:
                        mainWindow.Top = sizePopupElement.Height / 2;
                        break;
                    case PlacementMode.Bottom:
                        mainWindow.Top = SystemParameters.PrimaryScreenHeight - (placementTarget.ActualHeight + sizePopupElement.Height / 2);
                        break;
                    case PlacementMode.Left:
                        mainWindow.Left = sizePopupElement.Width / 2;
                        break;
                    case PlacementMode.Right:
                        mainWindow.Left = SystemParameters.PrimaryScreenWidth - (placementTarget.ActualWidth + sizePopupElement.Width / 2);
                        break;
                    default:
                        throw new NotSupportedException(String.Format("UnSupported PlacementMode {0}", placementMode));
                }
            }
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            if (isTargetRenderedOnScreen)
            {
                expectedPopupPositionX = placementTarget.PointToScreen(new Point()).X;
                expectedPopupPositionY = placementTarget.PointToScreen(new Point()).Y;
            }
            else
            {
                expectedPopupPositionX = 0;
                expectedPopupPositionY = 0;
            }
        }

        protected override void GetExpectedPopupPosition()
        {
            switch (placementMode)
            {
                case PlacementMode.Top:
                    if (placementTarget.PointToScreen(new Point()).Y > sizePopupElement.Height)
                    {
                        // If placementTarget top point is bigger than popup height, place popup on top of target
                        // Expected top point equals placementTarget top point subtract popup height 
                        expectedPopupPosition = new Point(expectedPopupPositionX, placementTarget.PointToScreen(new Point()).Y - sizePopupElement.Height);
                    }
                    else
                    {
                        // If placementTarget top point is smaller than popup height, place popup on bottom of target
                        // Expected top point equals placementTarget top point plus placementTarget height 
                        expectedPopupPosition = new Point(expectedPopupPositionX, placementTarget.PointToScreen(new Point()).Y + placementTarget.ActualHeight);
                    }
                    break;
                case PlacementMode.Bottom:
                    if (SystemParameters.PrimaryScreenHeight - (placementTarget.PointToScreen(new Point()).Y + placementTarget.ActualHeight) > sizePopupElement.Height)
                    {
                        // If screen height subtract placementTarget top point is bigger than popup height, place popup on bottom of target
                        // Expected top point equals placementTarget top point plus placementTarget height 
                        expectedPopupPosition = new Point(expectedPopupPositionX, placementTarget.PointToScreen(new Point()).Y + placementTarget.ActualHeight);
                    }
                    else
                    {
                        // If screen height subtract placementTarget top point is smaller than popup height, place popup on top of target
                        // Expected top point equals placementTarget top point subtract popup height 
                        expectedPopupPosition = new Point(expectedPopupPositionX, placementTarget.PointToScreen(new Point()).Y - sizePopupElement.Height);
                    }
                    break;
                case PlacementMode.Left:
                    if (placementTarget.PointToScreen(new Point()).X > sizePopupElement.Width)
                    {
                        // If placementTarget top point is bigger than popup width, place popup on left of target
                        // Expected top point equals placementTarget top point subtract popup width 
                        expectedPopupPosition = new Point(placementTarget.PointToScreen(new Point()).X - sizePopupElement.Width, expectedPopupPositionY);
                    }
                    else
                    {
                        // If placementTarget top point is smaller than popup width, place popup on bottom of target
                        // Expected top point equals placementTarget top point plus placementTarget width 
                        expectedPopupPosition = new Point(placementTarget.PointToScreen(new Point()).X + placementTarget.ActualWidth, expectedPopupPositionY);
                    }
                    break;
                case PlacementMode.Right:
                    if (SystemParameters.PrimaryScreenWidth - (placementTarget.PointToScreen(new Point()).X + placementTarget.ActualWidth) > sizePopupElement.Width)
                    {
                        // If screen width subtract placementTarget top point is bigger than popup width, place popup on right of target
                        // Expected top point equals placementTarget top point plus placementTarget width 
                        expectedPopupPosition = new Point(placementTarget.PointToScreen(new Point()).X + placementTarget.ActualWidth, expectedPopupPositionY);
                    }
                    else
                    {
                        // If screen width subtract placementTarget top point is smaller than popup width, place popup on left of target
                        // Expected top point equals placementTarget top point subtract popup width 
                        expectedPopupPosition = new Point(placementTarget.PointToScreen(new Point()).X - sizePopupElement.Width, expectedPopupPositionY);
                    }
                    break;
                default:
                    throw new NotSupportedException(String.Format("UnSupported PlacementMode {0}", placementMode));
            }
        }
    }
}
