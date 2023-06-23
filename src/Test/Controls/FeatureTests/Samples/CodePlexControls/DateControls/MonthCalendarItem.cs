//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfControlToolkit
{
    /// <summary>
    ///     Represents a single day within the MonthCalendar control.
    /// </summary>
    public class MonthCalendarItem : ContentControl
    {
        /// <summary>
        ///     Static Constructor
        /// </summary>
        static MonthCalendarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthCalendarItem), new FrameworkPropertyMetadata(typeof(MonthCalendarItem)));
            FocusableProperty.OverrideMetadata(typeof(MonthCalendarItem), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));
        }

        /// <summary>
        ///     The DependencyProperty for IsSelected property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
                DependencyProperty.Register(
                        "IsSelected",
                        typeof(bool),
                        typeof(MonthCalendarItem),
                        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        ///     Whether this item is selected or not.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, BooleanBoxes.Box(value)); }
        }

        private static readonly DependencyPropertyKey IsWithinVisibleMonthPropertyKey =
                DependencyProperty.RegisterReadOnly(
                        "IsWithinVisibleMonth",
                        typeof(bool),
                        typeof(MonthCalendarItem),
                        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        ///     The DependencyProperty for IsWithinVisibleMonth property
        /// </summary>
        public static readonly DependencyProperty IsWithinVisibleMonthProperty = IsWithinVisibleMonthPropertyKey.DependencyProperty;

        /// <summary>
        ///     Whether this item is within the current month.
        /// </summary>
        public bool IsWithinVisibleMonth
        {
            get { return (bool)GetValue(IsWithinVisibleMonthProperty); }
            private set { SetValue(IsWithinVisibleMonthPropertyKey, BooleanBoxes.Box(value)); }
        }

        private static readonly DependencyPropertyKey IsTodayPropertyKey =
                DependencyProperty.RegisterReadOnly(
                        "IsToday",
                        typeof(bool),
                        typeof(MonthCalendarItem),
                        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        ///     The DependencyProperty for IsWithinVisibleMonth property
        /// </summary>
        public static readonly DependencyProperty IsTodayProperty = IsWithinVisibleMonthPropertyKey.DependencyProperty;

        /// <summary>
        ///     Whether this item represents "today."
        /// </summary>
        public bool IsToday
        {
            get { return (bool)GetValue(IsTodayProperty); }
            private set { SetValue(IsTodayPropertyKey, value); }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // This will notify the parent element of this child element.
            // If this element is removed from the subtree of the parent
            // MonthCalendar control, then it will not be notified of this change.

            MonthCalendar monthCalendar = this.TemplatedParent as MonthCalendar;
            if (monthCalendar != null)
            {
                monthCalendar.RegisterCalendarItem(this);
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (newContent is DateTime)
            {
                DateTime day = (DateTime)newContent;
                MonthCalendar calendar = TemplatedParent as MonthCalendar;
                if (calendar != null)
                {
                    DateTime visibleMonth = calendar.VisibleMonth;
                    IsWithinVisibleMonth = (visibleMonth.Month == day.Month);
                    IsSelected = false; // MonthCalendar will update the selection state
                }

                IsToday = (day == DateTime.Today);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            MakeActiveItem(IsShiftKeyDown);
            e.Handled = true;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (OnlyLeftButtonDown(e))
            {
                MakeActiveItem(true);
                e.Handled = true;
            }
        }

        private void MakeActiveItem(bool extendSelection)
        {
            MonthCalendar calendar = TemplatedParent as MonthCalendar;
            if (calendar != null)
            {
                calendar.MoveActiveItem((DateTime)Content, extendSelection);
            }
        }

        private static bool OnlyLeftButtonDown(MouseEventArgs e)
        {
            return ((e.LeftButton == MouseButtonState.Pressed) &&
                    (e.MiddleButton == MouseButtonState.Released) &&
                    (e.RightButton == MouseButtonState.Released) &&
                    (e.XButton1 == MouseButtonState.Released) &&
                    (e.XButton2 == MouseButtonState.Released));
        }

        private static bool IsShiftKeyDown
        {
            get
            {
                return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            }
        }
    }
}
