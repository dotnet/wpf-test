using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Input;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Encapsulates RangeBase controls ValueChanged event behavior test algorithm.
    /// </summary>
    public class ValueChangedEventTest : IRangeBaseTest
    {
        public void Run(RangeBase rangeBase)
        {
            string eventName = "ValueChanged";
            if (rangeBase is ScrollBar)
            {
                // Horizontal scenario
                ScrollBarHorizontalValueChangedEvent((ScrollBar)rangeBase, eventName);

                // Vertical scenario
                ScrollBarVerticalValueChangedEvent((ScrollBar)rangeBase, eventName);
            }
            else if (rangeBase is ProgressBar)
            {
                rangeBase.Width = 200;
                rangeBase.Height = 20;
                DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);

                RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs = new RoutedPropertyChangedEventArgs<double>(0, 50, RangeBase.ValueChangedEvent);
                routedPropertyChangedEventArgs.Source = rangeBase;
                EventTriggerCallback changeRangeValueCallback = delegate()
                {
                    ((ProgressBar)rangeBase).Value = 50;
                    DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                };
                EventHelper.ExpectEvent<EventArgs>(changeRangeValueCallback, rangeBase, eventName, routedPropertyChangedEventArgs);
            }
            else if (rangeBase is Slider)
            {
                // Horizontal scenario
                ((Slider)rangeBase).Orientation = Orientation.Horizontal;
                ((Slider)rangeBase).Width = 200;
                ((Slider)rangeBase).Height = 20;
                DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                SliderValueChangedEventTest((Slider)rangeBase, eventName);

                // Vertical scenario
                ((Slider)rangeBase).Orientation = Orientation.Vertical;
                ((Slider)rangeBase).Width = 20;
                ((Slider)rangeBase).Height = 50;
                DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                SliderValueChangedEventTest((Slider)rangeBase, eventName);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static void SliderValueChangedEventTest(Slider slider, string eventName)
        {
            SliderEventTestInfo testInfo = new SliderEventTestInfo();
            testInfo.RangeBase = slider;
            testInfo.EventName = eventName;
            testInfo.OldValue = 0;
            testInfo.NewValue = 1.0;
            testInfo.SliderRepeatButtonCommandName = SliderRepeatButtonCommandName.IncreaseLarge;
            MouseClickToScrollSlider(testInfo);

            testInfo.OldValue = 1.0;
            testInfo.NewValue = 0;
            testInfo.SliderRepeatButtonCommandName = SliderRepeatButtonCommandName.DecreaseLarge;
            MouseClickToScrollSlider(testInfo);
        }

        private static void MouseClickToScrollSlider(SliderEventTestInfo testInfo)
        {
            RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs = new RoutedPropertyChangedEventArgs<double>(testInfo.OldValue, testInfo.NewValue, RangeBase.ValueChangedEvent);
            routedPropertyChangedEventArgs.Source = testInfo.RangeBase;
            EventTriggerCallback changeRangeValueCallback = delegate()
            {
                UserInput.MouseLeftClickCenter(SliderHelper.FindRepeatButton((Slider)testInfo.RangeBase, testInfo.SliderRepeatButtonCommandName));
                DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
            };
            EventHelper.ExpectEvent<EventArgs>(changeRangeValueCallback, testInfo.RangeBase, testInfo.EventName, routedPropertyChangedEventArgs);
        }

        private static void ScrollBarVerticalValueChangedEvent(ScrollBar scrollBar, string eventName)
        {
            // Setup
            scrollBar.Orientation = Orientation.Vertical;
            scrollBar.Width = 20;
            scrollBar.Height = 50;
            scrollBar.Value = 0;
            DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);

            ScrollBarEventTestInfo testInfo = new ScrollBarEventTestInfo();
            testInfo.RangeBase = scrollBar;
            testInfo.EventName = eventName;
            testInfo.OldValue = 0;
            testInfo.NewValue = 0.1;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.LineDown;
            MouseClickToScrollScrollBar(testInfo);

            testInfo.OldValue = 0.1;
            testInfo.NewValue = 0;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.LineUp;
            MouseClickToScrollScrollBar(testInfo);

            testInfo.OldValue = 0;
            testInfo.NewValue = 1.0;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.PageDown;
            MouseClickToScrollScrollBar(testInfo);

            testInfo.OldValue = 1.0;
            testInfo.NewValue = 0;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.PageUp;
            MouseClickToScrollScrollBar(testInfo);
        }

        private static void ScrollBarHorizontalValueChangedEvent(ScrollBar scrollBar, string eventName)
        {
            // Setup
            scrollBar.Orientation = Orientation.Horizontal;
            scrollBar.Width = 200;
            scrollBar.Height = 20;
            scrollBar.Value = 0;
            DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);

            ScrollBarEventTestInfo testInfo = new ScrollBarEventTestInfo();
            testInfo.RangeBase = scrollBar;
            testInfo.EventName = eventName;
            testInfo.OldValue = 0;
            testInfo.NewValue = 0.1;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.LineRight;
            MouseClickToScrollScrollBar(testInfo);

            testInfo.OldValue = 0.1;
            testInfo.NewValue = 0;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.LineLeft;
            MouseClickToScrollScrollBar(testInfo);

            testInfo.OldValue = 0;
            testInfo.NewValue = 1.0;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.PageRight;
            MouseClickToScrollScrollBar(testInfo);

            testInfo.OldValue = 1.0;
            testInfo.NewValue = 0;
            testInfo.ScrollBarRepeatButtonCommandName = ScrollingMode.PageLeft;
            MouseClickToScrollScrollBar(testInfo);
        }

        private static void MouseClickToScrollScrollBar(ScrollBarEventTestInfo testInfo)
        {
            RoutedPropertyChangedEventArgs<double> routedPropertyChangedEventArgs = new RoutedPropertyChangedEventArgs<double>(testInfo.OldValue, testInfo.NewValue, RangeBase.ValueChangedEvent);
            routedPropertyChangedEventArgs.Source = testInfo.RangeBase;
            EventTriggerCallback mouseLeftClickCallback = delegate()
            {
                UserInput.MouseLeftClickCenter(ScrollBarHelper.GetRepeatButton((ScrollBar)testInfo.RangeBase, testInfo.ScrollBarRepeatButtonCommandName));
                DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
            };
            EventHelper.ExpectEvent<EventArgs>(mouseLeftClickCallback, testInfo.RangeBase, testInfo.EventName, routedPropertyChangedEventArgs);
        }
    }
}
