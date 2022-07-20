// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DRT
{
    /// <summary>
    ///     Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        private const int MaxWatchEvents = 30;

        private bool AddingWatchEvents
        {
            get
            {
                return ShowTrace.IsChecked == true;
            }
        }

        private void AddWatchEvent(string s)
        {
            if (WatchList.Items.Count >= MaxWatchEvents)
            {
                WatchList.Items.RemoveAt(0);
            }

            if (AddingWatchEvents)
            {
                WatchList.Items.Add(new MakeStringUnique() { TheString = s });
            }
        }

        private class MakeStringUnique
        {
            public string TheString
            {
                get;
                set;
            }

            public override string ToString()
            {
                return TheString;
            }
        }

        private void OnReset(object sender, RoutedEventArgs e)
        {
            Rect1.Reset();
            Rect2.Reset();
            Rect3.Reset();

            AddWatchEvent("----- Reset -----");
        }

        private void OnUseMouseChecked(object sender, RoutedEventArgs e)
        {
            if (UseMouse.IsChecked == true)
            {
                MouseTouchDevice.UseMouse();
            }
            else
            {
                MouseTouchDevice.StopMouse();
            }
        }

        private void OnShowTouchesChecked(object sender, RoutedEventArgs e)
        {
            if (ShowTouches.IsChecked == true)
            {
                StartTimer();
                TouchesTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                StopTimer();
                TouchesTextBox.Visibility = Visibility.Collapsed;
            }
        }

        private void StartTimer()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer(DispatcherPriority.Normal);
                _timer.Interval = TimeSpan.FromMilliseconds(100d);
                _timer.Tick += OnTimerTick;
            }

            _timer.Start();
        }

        private void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            string str = string.Empty;
            str += "Color -- > TouchesOver --> TouchesDirectlyOver\n";
            str += "Blue  " + GetTouchCount(Rect3.TouchesOver) + "   " + GetTouchCount(Rect3.TouchesDirectlyOver) + "\n";
            str += "Green " + GetTouchCount(Rect2.TouchesOver) + "   " + GetTouchCount(Rect2.TouchesDirectlyOver) + "\n";
            str += "Red " + GetTouchCount(Rect1.TouchesOver) + "   " + GetTouchCount(Rect1.TouchesDirectlyOver) + "\n";
            str += "Violet " + GetTouchCount(Rect5.TouchesOver) + "   " + GetTouchCount(Rect5.TouchesDirectlyOver) + "\n";
            str += "Orange " + GetTouchCount(Rect4.TouchesOver) + "   " + GetTouchCount(Rect4.TouchesDirectlyOver) + "\n";
            str += "Canvas " + GetTouchCount(RectCanvas.TouchesOver) + "   " + GetTouchCount(RectCanvas.TouchesDirectlyOver) + "\n";
            TouchesTextBox.Text = str;
        }

        private int GetTouchCount(IEnumerable<TouchDevice> ienum)
        {
            int count = 0;
            foreach (TouchDevice device in ienum)
            {
                count++;
            }
            return count;
        }

        private bool _stop;

        private void OnStylusDown(object sender, StylusDownEventArgs e)
        {
            AddWatchEvent("StylusDown");
        }

        private void OnStylusUp(object sender, StylusEventArgs e)
        {
            AddWatchEvent("StylusUp");
        }

        private void OnStylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        {
            AddWatchEvent(String.Format("Gesture ({0}) - {1}", e.StylusDevice.TabletDevice.Type, e.SystemGesture));
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddWatchEvent("MouseDown");
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            AddWatchEvent("MouseMove");
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            AddWatchEvent("MouseUp");
        }

        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            AddWatchEvent("TouchDown");
        }

        private void OnTouchMove(object sender, TouchEventArgs e)
        {
            AddWatchEvent("TouchMove");
        }

        private void OnTouchUp(object sender, TouchEventArgs e)
        {
            AddWatchEvent("TouchUp");
        }

        private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            AddWatchEvent("Started");
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var manipulation = e.CumulativeManipulation;
            AddWatchEvent(String.Format("Delta {2}({0:F2},{1:F2})", manipulation.Translation.X, manipulation.Translation.Y, e.IsInertial ? "- Inertia " : String.Empty));
            if (_stop && e.IsInertial)
            {
                _stop = false;
                e.StartInertia();
            }
        }

        private void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            AddWatchEvent("InertiaStarting");
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            var manipulation = e.TotalManipulation;
            AddWatchEvent(String.Format("Completed ({0:F2},{1:F2})", manipulation.Translation.X, manipulation.Translation.Y));
        }

        protected override void OnStylusDown(StylusDownEventArgs e)
        {
            AddWatchEvent(String.Format("StylusDown ({0}) ID={1}", e.StylusDevice.TabletDevice.Type, e.StylusDevice.Id));
        }

        protected override void OnStylusUp(StylusEventArgs e)
        {
            AddWatchEvent(String.Format("StylusUp ({0}) ID={1}", e.StylusDevice.TabletDevice.Type, e.StylusDevice.Id));
        }

        private DispatcherTimer _timer;
    }
}
