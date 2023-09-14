using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Test;
using Avalon.Test.ComponentModel.Utilities;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// It compares where a popup is actually positioned to the way you expect to be positioned. 
    /// 
    /// Client just need to call Run method that encapsulates Popup Placement test contract below to run tests.
    ///   protected abstract method Setup() - defer to concrete class to move window to right location based on the input condition such as PlacementMode
    ///   protected virtual method OpenPopup() - set Popup.IsOpen to true to open Popup 
    ///                                          set it to virtual to provide extendibility for different ways of opening Popup in concrete classes
    ///   private method GetActualPopupPointToScreen() - get actual Popup top-left point by using PointToScreen() method
    ///                                                  set it to private because it is the best/only way of getting actual Popup top-left point
    ///   protected abstract method GetExpectedPopupPointToScreen() - defer to concrete class to get the expected popup top-left point
    ///   protected virtual method Validate() - validate the expected popup top-left point against actual popup top-left point
    ///                                         set it to virtual to provide extendibility for different ways of validation
    /// </summary>
    public abstract class PopupPlacementValidatorBase
    {
        public PopupPlacementValidatorBase(Popup popup, FrameworkElement placementTarget, bool hasEnoughRoomToRender,
            bool doesPopupSize, Window window)
        {
            this.popup = popup;
            this.placementTarget = placementTarget;
            this.mainWindow = window;
            this.hasEnoughRoomToRender = hasEnoughRoomToRender;

            popupPanel = popup.Child as Panel;
            if (popupPanel == null)
            {
                throw new NullReferenceException("popupPanel");
            }

            if (doesPopupSize)
            {
                sizePopupElement = popup;
            }
            else
            {
                sizePopupElement = popupPanel;
            }

            popup.PlacementTarget = placementTarget;
        }

        #region protected members

        protected Popup popup;
        protected FrameworkElement placementTarget;
        protected Panel popupPanel;
        protected Window mainWindow;
        protected bool hasEnoughRoomToRender;
        protected FrameworkElement sizePopupElement;
        protected Point actualPopupPosition, expectedPopupPosition;

        protected abstract void Setup();

        protected virtual void OpenPopup()
        {
            popup.IsOpen = true;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
        }

        private void GetActualPopupPosition()
        {
            actualPopupPosition = popupPanel.PointToScreen(new Point());
        }

        protected abstract void GetExpectedPopupPosition();

        // Ensure expected Popup top-left point equals to actual Popup.PointToScreen top-left point
        protected virtual void Validate()
        {
            Assert.AssertEqual(String.Format("The expected Popup Position {0} doesn't equal to actual Position {1}", expectedPopupPosition, actualPopupPosition), expectedPopupPosition, actualPopupPosition);
        }

        #endregion

        #region public method

        public void Run()
        {
            Setup();
            OpenPopup();
            GetActualPopupPosition();
            GetExpectedPopupPosition();
            Validate();
        }

        #endregion
    }
}
