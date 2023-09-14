using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Xml;
using Avalon.Test.ComponentModel.Validations;
using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Avalon.Test.ComponentModel.UnitTests
{
    [TargetType(typeof(Popup))]
    public class PopupPlacementBottomBVT : IUnitTest
    {
        public PopupPlacementBottomBVT()
        {
        }

        /// <summary>
        /// Test Popup Placement Bottom
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Panel panel = (Panel)obj;
            Popup popup = panel.Children[0] as Popup;
            CheckBox checkbox = panel.Children[1] as CheckBox;

            UserInput.MouseLeftClickCenter(checkbox);
            QueueHelper.WaitTillQueueItemsProcessed();            

            if (popup.Placement != PlacementMode.Bottom)
            {
                TestLog.Current.LogDebug("Fail: Popup PlacementMode is not Bottom.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

    }
    [TargetType(typeof(Popup))]
    public class PopupPlacementCenterBVT : IUnitTest
    {
        public PopupPlacementCenterBVT()
        {
        }

        /// <summary>
        /// Test Popup Placement Center
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Panel panel = (Panel)obj;
            Popup popup = panel.Children[0] as Popup;
            CheckBox checkbox = panel.Children[1] as CheckBox;
            popup.Placement = PlacementMode.Center;

            UserInput.MouseLeftClickCenter(checkbox);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (popup.Placement != PlacementMode.Center)
            {
                TestLog.Current.LogDebug("Fail: Popup PlacementMode is not Center.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

    }
    [TargetType(typeof(Popup))]
    public class PopupPlacementAbsoluteBVT : IUnitTest
    {
        public PopupPlacementAbsoluteBVT()
        {
        }

        /// <summary>
        /// Test Popup Placement Absolute
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Panel panel = (Panel)obj;
            Popup popup = panel.Children[0] as Popup;
            CheckBox checkbox = panel.Children[1] as CheckBox;
            popup.Placement = PlacementMode.Absolute;

            UserInput.MouseLeftClickCenter(checkbox);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (popup.Placement != PlacementMode.Absolute)
            {
                TestLog.Current.LogDebug("Fail: Popup PlacementMode is not Absolute.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

    }
    [TargetType(typeof(Popup))]
    public class PopupPlacementRightBVT : IUnitTest
    {
        public PopupPlacementRightBVT()
        {
        }

        /// <summary>
        /// Test Popup Placement Right
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Panel panel = (Panel)obj;
            Popup popup = panel.Children[0] as Popup;
            CheckBox checkbox = panel.Children[1] as CheckBox;
            popup.Placement = PlacementMode.Right;

            UserInput.MouseLeftClickCenter(checkbox);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (popup.Placement != PlacementMode.Right)
            {
                TestLog.Current.LogDebug("Fail: Popup PlacementMode is not Right.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

    }

    [TargetType(typeof(Popup))]
    public class PopupIsOpenBVT : IUnitTest
    {
        public PopupIsOpenBVT()
        {
        }

        /// <summary>
        /// Test Popup IsOpen
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Panel panel = (Panel)obj;
            Popup popup = panel.Children[0] as Popup;
            CheckBox checkbox = panel.Children[1] as CheckBox;

            UserInput.MouseLeftClickCenter(checkbox);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (!popup.IsOpen)
            {
                TestLog.Current.LogDebug("Fail: Popup IsOpen is not true.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

    }

    [TargetType(typeof(Popup))]
    public class PopupOffsetBVT : IUnitTest
    {
        public PopupOffsetBVT()
        {
        }

        /// <summary>
        /// Test Popup Offset
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            double length = 30;
            Panel panel = (Panel)obj;
            Popup popup = panel.Children[0] as Popup;
            CheckBox checkbox = panel.Children[1] as CheckBox;
            popup.HorizontalOffset = length;
            popup.VerticalOffset = length;

            UserInput.MouseLeftClickCenter(checkbox);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (popup.HorizontalOffset != length || popup.VerticalOffset != length)
            {
                TestLog.Current.LogDebug("Fail: Popup Offset is not 30.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

    }

    [TargetType(typeof(Popup))]
    public class PopupCloseModeModalBVT : IUnitTest
    {
        public PopupCloseModeModalBVT()
        {
        }

        /// <summary>
        /// Test Popup CloseMode is Modal
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            //double length = 30;
            Panel panel = (Panel)obj;
            Popup popup = panel.Children[0] as Popup;
            CheckBox checkbox = panel.Children[1] as CheckBox;
            popup.StaysOpen = false;

            UserInput.MouseLeftClickCenter(checkbox);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (!popup.IsOpen)
            {
                TestLog.Current.LogDebug("Fail: Popup IsOpen is not true.");
                return TestResult.Fail;
            }

            UserInput.MouseLeftDown(panel, (int)panel.Width - 2, (int)panel.Height - 2);
            UserInput.MouseLeftUp(panel, (int)panel.Width - 2, (int)panel.Height - 2);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (popup.IsOpen)
            {
                TestLog.Current.LogDebug("Fail: Popup IsOpen is true.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

    }

    [TargetType(typeof(Popup))]
    public class PopupCloseModeModalessBVT : IUnitTest
    {
        public PopupCloseModeModalessBVT()
        {
        }

        /// <summary>
        /// Test Popup CloseMode is Modaless
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            //double length = 30;
            Panel panel = (Panel)obj;
            Popup popup = panel.Children[0] as Popup;
            CheckBox checkbox = panel.Children[1] as CheckBox;
            popup.StaysOpen = true;

            UserInput.MouseLeftClickCenter(checkbox);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (!popup.IsOpen)
            {
                TestLog.Current.LogDebug("Fail: Popup IsOpen is not true.");
                return TestResult.Fail;
            }

            UserInput.MouseLeftDown(panel, (int)panel.Width - 2, (int)panel.Height - 2);
            UserInput.MouseLeftUp(panel, (int)panel.Width - 2, (int)panel.Height - 2);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (!popup.IsOpen)
            {
                TestLog.Current.LogDebug("Fail: Popup IsOpen is not true.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}


