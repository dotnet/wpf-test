using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Input;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Encapsulates RangeBase controls Scroll event behavior test algorithm.
    /// </summary>
    public class ScrollEventTest : IRangeBaseTest
    {
        public void Run(RangeBase rangeBase)
        {
            string eventName = "Scroll";
            if (rangeBase is ScrollBar)
            {
                // Vertical scenario
                ScrollBarVerticalScrollEvent((ScrollBar)rangeBase, eventName);

                // Horizontal scenario
                ScrollBarHorizontalScrollEvent((ScrollBar)rangeBase, eventName);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void ScrollBarVerticalScrollEvent(ScrollBar scrollBar, string eventName)
        {
            // Setup
            scrollBar.Orientation = Orientation.Vertical;
            scrollBar.Width = 20;
            scrollBar.Height = 50;
            scrollBar.Value = 0;
            DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);

            // Test
            ScrollBarEventTestInfo testInfo = new ScrollBarEventTestInfo();
            testInfo.RangeBase = scrollBar;
            testInfo.EventName = eventName;
            testInfo.NewValue = 0.1;
            testInfo.ScrollEventType = ScrollEventType.SmallIncrement;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.LineDown;
            ScrollBarScrollEventTest(testInfo);

            testInfo.NewValue = 0;
            testInfo.ScrollEventType = ScrollEventType.SmallDecrement;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.LineUp;
            ScrollBarScrollEventTest(testInfo);

            testInfo.NewValue = 1.0;
            testInfo.ScrollEventType = ScrollEventType.LargeIncrement;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.PageDown;
            ScrollBarScrollEventTest(testInfo);

            testInfo.NewValue = 0;
            testInfo.ScrollEventType = ScrollEventType.LargeDecrement;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.PageUp;
            ScrollBarScrollEventTest(testInfo);
        }

        private static void ScrollBarHorizontalScrollEvent(ScrollBar scrollBar, string eventName)
        {
            // Setup
            scrollBar.Orientation = Orientation.Horizontal;
            scrollBar.Width = 200;
            scrollBar.Height = 20;
            scrollBar.Value = 0;
            DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);

            // Test
            ScrollBarEventTestInfo testInfo = new ScrollBarEventTestInfo();
            testInfo.RangeBase = scrollBar;
            testInfo.EventName = eventName;
            testInfo.NewValue = 0.1;
            testInfo.ScrollEventType = ScrollEventType.SmallIncrement;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.LineRight;
            ScrollBarScrollEventTest(testInfo);

            testInfo.NewValue = 0;
            testInfo.ScrollEventType = ScrollEventType.SmallDecrement;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.LineLeft;
            ScrollBarScrollEventTest(testInfo);

            testInfo.NewValue = 1.0;
            testInfo.ScrollEventType = ScrollEventType.LargeIncrement;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.PageRight;
            ScrollBarScrollEventTest(testInfo);

            testInfo.NewValue = 0;
            testInfo.ScrollEventType = ScrollEventType.LargeDecrement;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.PageLeft;
            ScrollBarScrollEventTest(testInfo);
        }

        private static void ScrollBarScrollEventTest(ScrollBarEventTestInfo testInfo)
        {
            ScrollEventArgs scrollEventArgs = new ScrollEventArgs(testInfo.ScrollEventType, testInfo.NewValue);
            scrollEventArgs.Source = testInfo.RangeBase;
            EventTriggerCallback mouseLeftClickCallback = delegate()
            {
                UserInput.MouseLeftClickCenter(ScrollBarHelper.GetRepeatButton((ScrollBar)testInfo.RangeBase, testInfo.ScrollBarRepeatButtonCommandName));
                DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
            };
            EventHelper.ExpectEvent<EventArgs>(mouseLeftClickCallback, testInfo.RangeBase, testInfo.EventName, scrollEventArgs);
        }
    }
}
