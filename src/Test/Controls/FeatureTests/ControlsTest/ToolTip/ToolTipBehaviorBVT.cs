using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Interop;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Avalon.Test.ComponentModel.Actions;

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// <description>
    /// ToolTipBehaviorBVT
    /// Open and Close ToolTip.
    /// </description>
    /// </summary>
    [Test(0, "ToolTip", "OpenAndCloseToolTip", Keywords = "Localization_Suite")]
    public class ToolTipBehaviorBVT : ToolTipBehaviorBVTBase
    {
        public ToolTipBehaviorBVT()
            : base()
        {
            RunSteps += new TestStep(OpenAndCloseToolTip);
        }
    }

    /// <summary>
    /// <description>
    /// KeyboardToolTipBehaviorBVT
    /// Open and Close ToolTip using 4.8+ Keyboard feature.
    /// </description>
    /// </summary>
    [Test(0, "ToolTip", "OpenAndCloseToolTipKeyboard", Keywords = "Localization_Suite", Versions = "4.8+")]
    public class KeyboardToolTipBehaviorBVT : ToolTipBehaviorBVTBase
    {
        public KeyboardToolTipBehaviorBVT()
            : base()
        {
            RunSteps += new TestStep(OpenAndCloseToolTipFocus);
            RunSteps += new TestStep(OpenAndCloseToolTipShortcut);
        }
    }

    /// <summary>
    /// <description>
    /// ReentrantClose
    /// Closing tooltip causes re-entrant call to close the tooltip.
    /// </description>
    /// </summary>
    [Test(1, "ToolTip", "ReentrantClose", Versions = "4.6+")]
    public class ToolTipReentrantClose : ToolTipBehaviorBVTBase
    {
        [Variation("ToolTipClosing")]
        [Variation("ToolTipClosed")]
        public ToolTipReentrantClose(string eventName)
            : base()
        {
            switch (eventName)
            {
                case "ToolTipClosing":
                    InitializeSteps += new TestStep(AddToolTipClosingHandler);
                    break;
                case "ToolTipClosed":
                    InitializeSteps += new TestStep(AddToolTipClosedHandler);
                    break;
             }

            RunSteps += new TestStep(OpenAndCloseToolTip);
        }

        TestResult AddToolTipClosedHandler()
        {
            ToolTip tooltip = MyButton.ToolTip as ToolTip;
            if (tooltip == null)
            {
                throw new TestValidationException("ToolTip is null.");
            }

            // this is one way to get user code to run when tooltip.IsOpen changes
            Binding binding = new Binding("IsOpen");
            binding.Source = tooltip;
            binding.Converter = new ToolTipConverter();
            BindingOperations.SetBinding(MyButton, Button.TagProperty, binding);

            return TestResult.Pass;
        }

        TestResult AddToolTipClosingHandler()
        {
            ToolTip tooltip = MyButton.ToolTip as ToolTip;
            if (tooltip == null)
            {
                throw new TestValidationException("ToolTip is null.");
            }

            MyButton.ToolTipClosing += OpenContextMenu;
            return TestResult.Pass;
        }

        void OpenContextMenu(object sender, EventArgs e)
        {
            OpenContextMenu();
        }

        public static void OpenContextMenu()
        {
            ContextMenu contextMenu = MyButton.ContextMenu;
            contextMenu.PlacementTarget = MyButton;
            //contextMenu.Placement = PlacementMode.Bottom;

            // Opening the context menu causes  a re-entrant call to
            // PopupControlService.RaiseToolTipClosingEvent.  This has caused
            // crashes.  The point of this test is merely
            // to exercise this codepath.
            contextMenu.IsOpen = true;
        }

        public class ToolTipConverter : IValueConverter
        {
            bool _wasOpen = false;

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                bool isOpen = (bool)value;
                if (_wasOpen && !isOpen)
                {
                    ToolTipReentrantClose.OpenContextMenu();
                }
                _wasOpen = isOpen;
                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }
    }

    public class ToolTipBehaviorBVTBase : XamlTest
    {
        #region Private Members
        static Button button;
        System.Windows.Point offsetPoint;
        DispatcherFrame closeFrame = new DispatcherFrame();

        protected static Button MyButton { get { return button; } }
        #endregion

        #region Public Members

        public ToolTipBehaviorBVTBase()
            : base(@"ToolTipBehavior.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            button = (Button)RootElement.FindName("button");
            if (button == null)
            {
                throw new TestValidationException("Button is null");
            }

            offsetPoint = new System.Windows.Point(50, 50);
            ToolTip tooltip = button.ToolTip as ToolTip;
            if (tooltip == null)
            {
                throw new TestValidationException("ToolTip is null.");
            }
            tooltip.PlacementTarget = button;
            tooltip.Placement = PlacementMode.Custom;
            tooltip.CustomPopupPlacementCallback = delegate(Size popupSize, Size targetSize, Point offset)
            {
                CustomPopupPlacement placement1 =
                   new CustomPopupPlacement(offsetPoint, PopupPrimaryAxis.Vertical);

                CustomPopupPlacement[] placements =
                        new CustomPopupPlacement[] { placement1 };
                return placements;
            };
            tooltip.HasDropShadow = true;
            tooltip.IsOpen = false;
            tooltip.Closed += OnToolTipClosing;

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            ToolTip tooltip = button.ToolTip as ToolTip;
            tooltip.Closed -= OnToolTipClosing;
            LogComment("Wait for tooltip to close");
            Dispatcher.PushFrame(closeFrame);
            WaitFor(3000);

            button = null;
            typeof(EventHelper).InvokeMember("sender", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            typeof(EventHelper).InvokeMember("actualEventArgs", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            return TestResult.Pass;
        }

        public TestResult OpenAndCloseToolTip()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            ToolTipActions.OpenAndCloseToolTip(button, offsetPoint);

            return TestResult.Pass;
        }

        public TestResult OpenAndCloseToolTipFocus()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            ToolTipActions.OpenAndCloseToolTip(button, offsetPoint, ToolTipActions.TriggerAction.KeyboardFocus);

            return TestResult.Pass;
        }

        public TestResult OpenAndCloseToolTipShortcut()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            ToolTipActions.OpenAndCloseToolTip(button, offsetPoint, ToolTipActions.TriggerAction.KeyboardShortcut);

            return TestResult.Pass;
        }

        private void OnToolTipClosing(object sender, EventArgs e)
        {
            closeFrame.Continue = false;
        }

        #endregion
    }
}
