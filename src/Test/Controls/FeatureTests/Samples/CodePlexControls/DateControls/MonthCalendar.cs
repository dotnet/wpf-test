//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace WpfControlToolkit
{
    /// <summary>
    ///     The month calendar control implements a calendar-like user interface,
    ///     that provides the user with a very intuitive and recognizable method
    ///     of selecting a date, a contiguous or discrete range of dates using
    ///     a visual display. Users can customize the look of the calendar portion
    ///     of the control by setting titles, dates, fonts and backgrounds.
    /// </summary>
    public class MonthCalendar : Control
    {

        #region Constructors

        /// <summary>
        ///     Setup globally defined data.
        /// </summary>
        static MonthCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthCalendar), new FrameworkPropertyMetadata(typeof(MonthCalendar)));

            _gotoCommand = new RoutedCommand("Goto", typeof(MonthCalendar));
            CommandManager.RegisterClassCommandBinding(typeof(MonthCalendar), new CommandBinding(MonthCalendar.GotoCommand, new ExecutedRoutedEventHandler(OnExecuteGotoCommand), new CanExecuteRoutedEventHandler(OnCanExecuteGotoCommand)));

            _nextCommand = new RoutedCommand("Next", typeof(MonthCalendar));
            CommandManager.RegisterClassCommandBinding(typeof(MonthCalendar), new CommandBinding(MonthCalendar.NextCommand, new ExecutedRoutedEventHandler(OnExecuteNextCommand), new CanExecuteRoutedEventHandler(OnCanExecuteNextCommand)));
            CommandManager.RegisterClassInputBinding(typeof(MonthCalendar), new InputBinding(MonthCalendar.NextCommand, new KeyGesture(Key.PageDown)));

            _previousCommand = new RoutedCommand("Previous", typeof(MonthCalendar));
            CommandManager.RegisterClassCommandBinding(typeof(MonthCalendar), new CommandBinding(MonthCalendar.PreviousCommand, new ExecutedRoutedEventHandler(OnExecutePreviousCommand), new CanExecuteRoutedEventHandler(OnCanExecutePreviousCommand)));
            CommandManager.RegisterClassInputBinding(typeof(MonthCalendar), new InputBinding(MonthCalendar.PreviousCommand, new KeyGesture(Key.PageUp)));

            IsTabStopProperty.OverrideMetadata(typeof(MonthCalendar), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(MonthCalendar), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(MonthCalendar), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            TemplateProperty.OverrideMetadata(typeof(MonthCalendar), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTemplateChanged)));
        }

        /// <summary>
        ///     Instantiates an instance of this class.
        /// </summary>
        public MonthCalendar() : base()
        {
        }

        #endregion


        #region Currently displayed date information

        /// <summary>
        ///     The DependencyProperty for VisibleMonth property.
        /// </summary>
        public static readonly DependencyProperty VisibleMonthProperty =
                DependencyProperty.Register(
                        "VisibleMonth",
                        typeof(DateTime),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                DateTime.Today /* default value */,
                                new PropertyChangedCallback(OnVisibleMonthChanged),
                                new CoerceValueCallback(CoerceVisibleMonth)),
                        new ValidateValueCallback(IsValidDate));

        /// <summary>
        ///     The fully visible month being displayed.
        /// </summary>
        /// <remarks>
        ///     Only the Year and Month fields are accurate. The Day and other fields are not guaranteed.
        /// </remarks>
        public DateTime VisibleMonth
        {
            get { return (DateTime)GetValue(VisibleMonthProperty); }
            set { SetValue(VisibleMonthProperty, value); }
        }

        private static object CoerceVisibleMonth(DependencyObject d, object value)
        {
            // Ensure that the visible month is between MinDate and MaxDate

            MonthCalendar calendar = (MonthCalendar)d;
            DateTime newValue = ((DateTime)value).Date;

            DateTime min = calendar.MinDate;
            if (newValue < min)
            {
                return min;
            }

            DateTime max = calendar.MaxDate;
            if (newValue > max)
            {
                return max;
            }

            return newValue;
        }

        private static void OnVisibleMonthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar calendar = (MonthCalendar)d;
            DateTime oldDate = (DateTime)e.OldValue;
            DateTime newDate = (DateTime)e.NewValue;

            // The old date is not equal to the new date in the Year and Month fields
            if (MonthCalendarHelper.CompareYearMonth(oldDate, newDate) != 0)
            {
                calendar.ResetFirstVisibleDay();

                // Raise the event
                calendar.OnVisibleMonthChanged(new RoutedPropertyChangedEventArgs<DateTime>(oldDate, newDate, VisibleMonthChangedEvent));
            }
        }

        /// <summary>
        ///     An event fired when the visible month changed.
        /// </summary>
        public static readonly RoutedEvent VisibleMonthChangedEvent = EventManager.RegisterRoutedEvent(
            "VisibleMonthChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<DateTime>), typeof(MonthCalendar));

        /// <summary>
        ///     An event fired when the visible month changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<DateTime> VisibleMonthChanged
        {
            add { AddHandler(VisibleMonthChangedEvent, value); }
            remove { RemoveHandler(VisibleMonthChangedEvent, value); }
        }

        /// <summary>
        ///     Called when the visible month changed and raises the VisibleMonthChanged event.
        /// </summary>
        /// <param name="e">RoutedPropertyChangedEventArgs contains the old and new value.</param>
        protected virtual void OnVisibleMonthChanged(RoutedPropertyChangedEventArgs<DateTime> e)
        {
            RaiseEvent(e);
        }

        private static readonly DependencyPropertyKey FirstVisibleDayPropertyKey =
                DependencyProperty.RegisterReadOnly(
                        "FirstVisibleDay",
                        typeof(DateTime),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(DateTime.Today));

        /// <summary>
        ///     The DependencyProperty for the FirstVisibleDay property.
        /// </summary>
        public static readonly DependencyProperty FirstVisibleDayProperty = FirstVisibleDayPropertyKey.DependencyProperty;

        /// <summary>
        ///     The date of the first visible day being displayed.
        /// </summary>
        public DateTime FirstVisibleDay
        {
            get { return (DateTime)GetValue(FirstVisibleDayProperty); }
            private set { SetValue(FirstVisibleDayPropertyKey, value); }
        }

        /// <summary>
        ///     Called when the Template's tree has been generated.
        ///     Sets the FirstVisibleDay property to its correct value.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ResetFirstVisibleDay();
        }

        /// <summary>
        ///     Sets the FirstVisibleDay to its correct value.
        /// </summary>
        private void ResetFirstVisibleDay()
        {
            FirstVisibleDay = CalendarDataGenerator.CalculateLeadingDate(FirstDateOfVisibleMonth, FirstDayOfWeek);
            UpdateSelectionState(null, null);
        }

        /// <summary>
        ///     Determines if the specified date is within the visible date range.
        /// </summary>
        /// <param name="date">The date to test.</param>
        /// <returns>True if the date is within the visible date range. False otherwise.</returns>
        private bool IsWithinVisibleRange(DateTime date)
        {
            return MonthCalendarHelper.IsWithinRange(date, FirstVisibleDay, LastVisibleDay);
        }

        /// <summary>
        ///     The last date that is visible.
        /// </summary>
        private DateTime LastVisibleDay
        {
            get
            {
                if (_children != null)
                {
                    return FirstVisibleDay + new TimeSpan(_children.Count - 1, 0, 0, 0);
                }
                else
                {
                    return FirstVisibleDay;
                }
            }
        }

        /// <summary>
        ///     First date of the currently visible month.
        /// </summary>
        private DateTime FirstDateOfVisibleMonth
        {
            get
            {
                DateTime visibleMonth = VisibleMonth;
                return new DateTime(visibleMonth.Year, visibleMonth.Month, 1);
            }
        }

        /// <summary>
        ///     Last date of the currently visible month.
        /// </summary>
        private DateTime LastDateOfVisibleMonth
        {
            get
            {
                try
                {
                    return FirstDateOfVisibleMonth.AddMonths(1).AddMilliseconds(-1);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return DateTime.MaxValue;
                }
            }
        }

        /// <summary>
        ///     Scroll the current visible month based on delta and direction
        /// </summary>
        /// <param name="delta">The number and direction of months. (Negative goes backwards, positive goes forward)</param>
        private void ScrollVisibleMonth(int delta)
        {
            //NOTE:
            // To read the graph below, please use fixed width font.
            //
            // Date range:                              Min                               Max
            //                                           |--------------------------------|
            //                                           .                                .
            // Valid cases:                              .                                .
            //                                           .                                .
            // Case 1:                                   .     [..................]       .
            //                                           .  FstVsM                        .
            //                                           .                                .
            // Case 2:                                   [..................]             .
            //                                        FstVsM                              .
            //                                           .                                .
            // Case 3:                                   .             [..................]
            //                                           .          FstVsM                .
            //                                           .                                .
            // Case 4:                                   [.........................................]
            //                                        FstVsM                              .
            //                                           .                                .
            //                                           .                                .
            // Case 5:                                   .             [.......................]
            //                                           .           FstVsM               .
            //                                           .                                .
            // Valid initial but no returnable cases:    .                                .
            //                                           .                                .
            // Case 6:                      [..................]                          .
            //                           FstVsM                                           .
            //                                           .                                .
            // Case 7:                                   .                                .       [..................]
            //                                           .                                .    FstVsM
            //
            // Invalid case(s)
            //                                           .                                .
            // Case 8:                          [.........................................]
            //                               FstVsM

            int monthCount = Math.Abs(delta);

            DateTime firstMonth = VisibleMonth;
            bool checkFirstMonth = false;

            // Scroll to the previous page
            if (delta < 0)
            {
                if (MonthCalendarHelper.CompareYearMonth(VisibleMonth, MinDate) > 0) // case 1/3/5
                {
                    checkFirstMonth = true;
                    try
                    {
                        firstMonth = VisibleMonth.AddMonths(delta);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        firstMonth = MinDate; // case 2/4
                    }
                }
            }
            // Scroll to the next page
            else
            {
                try
                {
                    if (MonthCalendarHelper.CompareYearMonth(LastDateOfVisibleMonth, MaxDate) < 0) // case 1-4
                    {
                        firstMonth = VisibleMonth.AddMonths(delta);
                        DateTime lastMonth = firstMonth.AddMonths(monthCount - 1);

                        // if lastMonth is greater than MaxDate, scroll back to the appropriate position
                        if (MonthCalendarHelper.CompareYearMonth(lastMonth, MaxDate) > 0) // case 5
                        {
                            checkFirstMonth = true;
                            firstMonth = MaxDate.AddMonths(-(monthCount - 1)); // case 3
                        }
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    checkFirstMonth = true;
                    firstMonth = MaxDate.AddMonths(-(monthCount - 1)); // case 3
                }
            }

            // check in case the firstMonth is less than MinDate
            if (checkFirstMonth && MonthCalendarHelper.CompareYearMonth(firstMonth, MinDate) < 0) // case 8
            {
                firstMonth = MinDate; // change to case 4
            }

            VisibleMonth = firstMonth;

            // Determine whether to move the current active date (focus).
            if (!MonthCalendarHelper.IsWithinRange(_currentActive, FirstDateOfVisibleMonth, LastDateOfVisibleMonth))
            {
                _currentActive = FirstDateOfVisibleMonth;
                _anchor = null;
                MonthCalendarItem item = GetContainerFromDate(_currentActive);
                if (item != null)
                {
                    item.Focus();
                }
            }
        }

        #endregion


        #region Commands

        /// <summary>
        ///     Go to month
        /// </summary>
        /// <remarks>
        ///     If the argument is null, GotoCommand will switch to DateTime.Now
        ///     If the argument is a DateTime, GotoCommand will switch to the month from the argument.
        /// </remarks>
        public static RoutedCommand GotoCommand
        {
            get { return _gotoCommand; }
        }

        private int CalculateGotoOffset(object parameter)
        {
            int offset = 0;
            if (parameter == null)
            {
                offset = MonthCalendarHelper.SubtractByMonth(DateTime.Today, VisibleMonth);
            }
            else if (parameter is DateTime)
            {
                offset = MonthCalendarHelper.SubtractByMonth((DateTime)parameter, VisibleMonth);
            }

            return offset;
        }

        private static void OnCanExecuteGotoCommand(object target, CanExecuteRoutedEventArgs args)
        {
            MonthCalendar calendar = (MonthCalendar)target;

            int offset = calendar.CalculateGotoOffset(args.Parameter);
            if (offset != 0)
            {
                DateTime newValue = calendar.VisibleMonth.AddMonths(offset);

                args.CanExecute = (MonthCalendarHelper.CompareYearMonth(newValue, calendar.VisibleMonth) != 0
                                   && MonthCalendarHelper.IsWithinRange(newValue, calendar.MinDate, calendar.MaxDate));
                args.Handled = true;
            }
        }

        private static void OnExecuteGotoCommand(object target, ExecutedRoutedEventArgs args)
        {
            MonthCalendar calendar = (MonthCalendar)target;

            int offset = calendar.CalculateGotoOffset(args.Parameter);
            if (offset != 0)
            {
                calendar.ScrollVisibleMonth(offset);
                args.Handled = true;
            }
        }

        public void Goto(DateTime date)
        {
            int offset = CalculateGotoOffset(date);
            if (offset != 0)
            {
                ScrollVisibleMonth(offset);
            }

            _currentActive = date;
            MonthCalendarItem item = GetContainerFromDate(_currentActive);
            if (item != null)
            {
                item.Focus();
            }
        }

        /// <summary>
        ///     Make the next month the visible month.
        /// </summary>
        public static RoutedCommand NextCommand
        {
            get { return _nextCommand; }
        }

        /// <summary>
        ///     Make the previous month the visible month.
        /// </summary>
        public static RoutedCommand PreviousCommand
        {
            get { return _previousCommand; }
        }

        private static void OnCanExecuteNextCommand(object target, CanExecuteRoutedEventArgs args)
        {
            MonthCalendar calendar = (MonthCalendar)target;
            args.CanExecute = MonthCalendarHelper.CompareYearMonth(calendar.VisibleMonth, calendar.MaxDate) < 0;
        }

        private static void OnExecuteNextCommand(object target, ExecutedRoutedEventArgs args)
        {
            MonthCalendar calendar = (MonthCalendar)target;
            calendar.ScrollVisibleMonth(1);
        }

        private static void OnCanExecutePreviousCommand(object target, CanExecuteRoutedEventArgs args)
        {
            MonthCalendar calendar = (MonthCalendar)target;
            args.CanExecute = (MonthCalendarHelper.CompareYearMonth(calendar.VisibleMonth, calendar.MinDate) > 0);
        }

        private static void OnExecutePreviousCommand(object target, ExecutedRoutedEventArgs args)
        {
            MonthCalendar calendar = (MonthCalendar)target;
            calendar.ScrollVisibleMonth(-1);
        }

        #endregion


        #region Selection

        /// <summary>
        ///     The collection holding the dates that are selected.
        /// </summary>
        /// <remarks>
        ///     The dates are not ordered within the collection.
        /// </remarks>
        public ObservableCollection<DateTime> SelectedDates
        {
            get
            {
                if (_selectedDates == null)
                {
                    _selectedDates = new ObservableCollection<DateTime>();
                    _selectedDates.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSelectedDatesCollectionChanged);
                }

                return _selectedDates;
            }
        }

        /// <summary>
        ///     The DependencyProperty for SelectedDate property.
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
                DependencyProperty.Register(
                        "SelectedDate",
                        typeof(DateTime?),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                (DateTime?)null,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                new PropertyChangedCallback(OnSelectedDateChanged),
                                new CoerceValueCallback(OnCoerceNullableDateTime)),
                        new ValidateValueCallback(IsValidNullableDate));

        /// <summary>
        ///     The date at index 0 in the current selection or returns null if the selection is empty.
        /// </summary>
        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        private static object OnCoerceNullableDateTime(DependencyObject d, object value)
        {
            DateTime? date = (DateTime?)value;
            if (date.HasValue)
            {
                return new DateTime?(date.Value.Date);
            }
            else
            {
                return date;
            }
        }

        /// <summary>
        ///     Validates a date.
        /// </summary>
        /// <returns>Returns false if the value isn't null and is outside CalendarDataGenerator.MinDate~MaxDate range.  Otherwise, returns true.</returns>
        private static bool IsValidNullableDate(object value)
        {
            DateTime? date = (DateTime?)value;

            return !date.HasValue ||
                MonthCalendarHelper.IsWithinRange(date.Value, CalendarDataGenerator.MinDate, CalendarDataGenerator.MaxDate);
        }

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar calendar = (MonthCalendar)d;
            if (!calendar._isChangingSelection)
            {
                calendar.SetSelectedDate((DateTime?)e.NewValue);
            }
        }

        private void SetSelectedDate(DateTime? newDate)
        {
            ObservableCollection<DateTime> selectedDates = SelectedDates;
            int count = selectedDates.Count;
            List<DateTime> unselected = new List<DateTime>(count);
            List<DateTime> selected = new List<DateTime>(1);

            _isChangingSelection = true;
            try
            {
                // Clear the previous selection
                for (int i = 0; i < count; i++)
                {
                    unselected.Add(selectedDates[i]);
                }
                selectedDates.Clear();

                if (newDate.HasValue)
                {
                    // Select the new date
                    DateTime newDateValue = newDate.Value;
                    if (unselected.Contains(newDateValue))
                    {
                        unselected.Remove(newDateValue);
                    }
                    else
                    {
                        selected.Add(newDateValue);
                    }

                    selectedDates.Add(newDateValue);
                }
            }
            finally
            {
                _isChangingSelection = false;
            }

            // Fire the change event
            if ((unselected.Count > 0) || (selected.Count > 0))
            {
                UpdateSelectionState(unselected, selected);
                InvokeDateSelectedChangedEvent(unselected, selected);
            }
        }

        /// <summary>
        ///     An event fired when the selection changed.
        /// </summary>
        public static readonly RoutedEvent DateSelectionChangedEvent = EventManager.RegisterRoutedEvent(
            "DateSelectionChanged", RoutingStrategy.Bubble, typeof(DateSelectionChangedEventHandler), typeof(MonthCalendar));

        /// <summary>
        ///     An event fired when this selection changed.
        /// </summary>
        public event DateSelectionChangedEventHandler DateSelectionChanged
        {
            add { AddHandler(DateSelectionChangedEvent, value); }
            remove { RemoveHandler(DateSelectionChangedEvent, value); }
        }

        /// <summary>
        ///     Called when the selection changed and raises the DateSelectionChanged event.
        /// </summary>
        protected virtual void OnDateSelectionChanged(DateSelectionChangedEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        ///     Raise the DateSelectionChanged event.
        /// </summary>
        private void InvokeDateSelectedChangedEvent(List<DateTime> unselected, List<DateTime> selected)
        {
            DateSelectionChangedEventArgs args = new DateSelectionChangedEventArgs(unselected, selected);
            args.Source = this;
            OnDateSelectionChanged(args);
        }

        /// <summary>
        ///     Handle changes made directly to the SelectedDates collection.
        /// </summary>
        private void OnSelectedDatesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isChangingSelection)
                return;

            if (SelectedDates.Count > MaxSelectionCount)
            {
                // 
                throw new InvalidOperationException("Number of selected items exceeds MaxSelectionCount.");
            }

            // Validate new dates
            int count = e.NewItems.Count;
            List<DateTime> selected = new List<DateTime>(count);
            DateTime minDate = MinDate;
            DateTime maxDate = MaxDate;
            for (int i = 0; i < count; i++)
            {
                DateTime newDate = (DateTime)e.NewItems[i];
                if (!MonthCalendarHelper.IsWithinRange(newDate, MinDate, maxDate))
                {
                    // 
                    throw new InvalidOperationException("Selected date falls outside of the range from MinDate to MaxDate.");
                }

                selected.Add(newDate);
            }

            // Prepare removed dates for the event
            count = e.OldItems.Count;
            List<DateTime> unselected = new List<DateTime>(count);
            for (int i = 0; i < count; i++)
            {
                unselected.Add((DateTime)e.OldItems[i]);
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UpdateSelectedDate(null);
                UpdateSelectionState(null, null);
            }
            else
            {
                UpdateSelectedDate(unselected);
                UpdateSelectionState(unselected, selected);
            }

            // Fire the change event
            InvokeDateSelectedChangedEvent(unselected, selected);
        }

        /// <summary>
        ///     Updates the IsSelected property on the registered MonthCalendarItem children.
        /// </summary>
        private void UpdateSelectionState(List<DateTime> unselected, List<DateTime> selected)
        {
            if (_children != null)
            {
                if ((unselected != null) || (selected != null))
                {
                    if (unselected != null)
                    {
                        int count = unselected.Count;
                        for (int i = 0; i < count; i++)
                        {
                            MonthCalendarItem child = GetContainerFromDate(unselected[i]);
                            if (child != null)
                            {
                                child.IsSelected = false;
                            }
                        }
                    }

                    if (selected != null)
                    {
                        int count = selected.Count;
                        for (int i = 0; i < count; i++)
                        {
                            MonthCalendarItem child = GetContainerFromDate(selected[i]);
                            if (child != null)
                            {
                                child.IsSelected = true;
                            }
                        }
                    }
                }
                else
                {
                    int count = _children.Count;
                    for (int i = 0; i < count; i++)
                    {
                        MonthCalendarItem child = _children[i];
                        child.IsSelected = false;
                    }

                    if (_selectedDates != null)
                    {
                        count = _selectedDates.Count;
                        for (int i = 0; i < count; i++)
                        {
                            MonthCalendarItem child = GetContainerFromDate(_selectedDates[i]);
                            if (child != null)
                            {
                                child.IsSelected = true;
                            }
                        }
                    }
                }
            }
        }

        private void UpdateSelectedDate(List<DateTime> unselected)
        {
            DateTime? currentSelectedDate = SelectedDate;
            if (!currentSelectedDate.HasValue ||
                ((unselected != null) && unselected.Contains(currentSelectedDate.Value)) ||
                ((unselected == null) && !SelectedDates.Contains(currentSelectedDate.Value)))
            {
                _isChangingSelection = true;
                try
                {
                    if (SelectedDates.Count > 0)
                    {
                        SelectedDate = SelectedDates[0];
                    }
                    else
                    {
                        SelectedDate = null;
                    }
                }
                finally
                {
                    _isChangingSelection = false;
                }
            }
        }

        #endregion


        #region Control Settings

        /// <summary>
        ///     The DependencyProperty for FirstDayOfWeek property
        /// </summary>
        public static readonly DependencyProperty FirstDayOfWeekProperty =
                DependencyProperty.Register(
                        "FirstDayOfWeek",
                        typeof(DayOfWeek),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                DayOfWeek.Sunday /* default value */,
                                new PropertyChangedCallback(OnFirstDayOfWeekChanged)),
                                new ValidateValueCallback(IsValidFirstDayOfWeek));

        /// <summary>
        ///     The first day of the week as displayed in the month calendar.
        /// </summary>
        public DayOfWeek FirstDayOfWeek
        {
            get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
            set { SetValue(FirstDayOfWeekProperty, value); }
        }

        private static void OnFirstDayOfWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MonthCalendar)d).ResetFirstVisibleDay();
        }

        private static bool IsValidFirstDayOfWeek(object value)
        {
            DayOfWeek day = (DayOfWeek)value;

            return day == DayOfWeek.Sunday
                || day == DayOfWeek.Monday
                || day == DayOfWeek.Tuesday
                || day == DayOfWeek.Wednesday
                || day == DayOfWeek.Thursday
                || day == DayOfWeek.Friday
                || day == DayOfWeek.Saturday;
        }

        /// <summary>
        ///     The DependencyProperty for the MinDate property.
        /// </summary>
        public static readonly DependencyProperty MinDateProperty =
                DependencyProperty.Register(
                        "MinDate",
                        typeof(DateTime),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                CalendarDataGenerator.MinDate, /* The default value */
                                new PropertyChangedCallback(OnMinDateChanged),
                                new CoerceValueCallback(OnCoerceDateTime)),
                        new ValidateValueCallback(IsValidDate));

        /// <summary>
        ///     The minimum date of the calendar.
        /// </summary>
        public DateTime MinDate
        {
            get { return (DateTime)GetValue(MinDateProperty); }
            set { SetValue(MinDateProperty, value); }
        }

        private static object OnCoerceDateTime(DependencyObject d, object value)
        {
            return ((DateTime)value).Date;
        }

        private static void OnMinDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar calendar = (MonthCalendar)d;

            DateTime oldMaxDate = calendar.MaxDate;
            DateTime oldVisibleMonth = calendar.VisibleMonth;
            calendar.CoerceValue(MaxDateProperty);
            calendar.CoerceValue(VisibleMonthProperty);

            // If MaxDate or VisibleMonth hasn't been changed by CoerceValue, then we should update the SelectedDates.
            if (MonthCalendarHelper.CompareYearMonthDay(oldMaxDate, calendar.MaxDate) == 0
                && MonthCalendarHelper.CompareYearMonth(oldVisibleMonth, calendar.VisibleMonth) == 0)
            {
                calendar.OnMaxMinDateChanged((DateTime)e.NewValue, calendar.MaxDate);
            }
        }

        /// <summary>
        ///     Validate input value in MonthCalendar (MinDate, MaxDate, VisibleMonth)
        /// </summary>
        /// <returns>Returns False if value is outside CalendarDataGenerator.MinDate~MaxDate range.  Otherwise, returns True.</returns>
        private static bool IsValidDate(object value)
        {
            DateTime date = (DateTime)value;
            return (date >= CalendarDataGenerator.MinDate) &&
                    (date <= CalendarDataGenerator.MaxDate);
        }

        /// <summary>
        ///     The DependencyProperty for the MaxDate property.
        /// </summary>
        public static readonly DependencyProperty MaxDateProperty =
                DependencyProperty.Register(
                        "MaxDate",
                        typeof(DateTime),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                CalendarDataGenerator.MaxDate, /* The default value */
                                new PropertyChangedCallback(OnMaxDateChanged),
                                new CoerceValueCallback(CoerceMaxDate)),
                        new ValidateValueCallback(IsValidDate));

        /// <summary>
        ///     The maximum date of calendar.
        /// </summary>
        public DateTime MaxDate
        {
            get { return (DateTime)GetValue(MaxDateProperty); }
            set { SetValue(MaxDateProperty, value); }
        }

        private static object CoerceMaxDate(DependencyObject d, object value)
        {
            MonthCalendar calendar = (MonthCalendar)d;
            DateTime newValue = (DateTime)OnCoerceDateTime(d, value);

            DateTime min = calendar.MinDate;
            if (newValue < min)
            {
                return min;
            }

            return value;
        }

        private static void OnMaxDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar calendar = (MonthCalendar)d;

            DateTime oldVisibleMonth = calendar.VisibleMonth;
            calendar.CoerceValue(VisibleMonthProperty);

            // If VisibleMonth hasn't been changed by CoerceValue, we should update the SelectedDates.
            if (MonthCalendarHelper.CompareYearMonth(oldVisibleMonth, calendar.VisibleMonth) == 0)
            {
                calendar.OnMaxMinDateChanged(calendar.MinDate, (DateTime)e.NewValue);
            }
        }

        /// <summary>
        ///     Update the selected dates when max/min date has been changed.
        /// </summary>
        /// <param name="minDate">New MinDate</param>
        /// <param name="maxDate">New MaxDate</param>
        private void OnMaxMinDateChanged(DateTime minDate, DateTime maxDate)
        {
            List<DateTime> unselected = new List<DateTime>();
            ObservableCollection<DateTime> selectedDates = SelectedDates;
            int count = selectedDates.Count;

            _isChangingSelection = true;
            try
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    DateTime date = selectedDates[i];
                    if (!MonthCalendarHelper.IsWithinRange(date, minDate, maxDate))
                    {
                        unselected.Add(date);
                        selectedDates.RemoveAt(i);
                    }
                }
            }
            finally
            {
                _isChangingSelection = false;
            }

            if (unselected.Count > 0)
            {
                UpdateSelectedDate(unselected);
                UpdateSelectionState(unselected, null);
                InvokeDateSelectedChangedEvent(unselected, null);
            }
        }

        /// <summary>
        ///     The DependencyProperty for MaxSelectionCount property
        /// </summary>
        public static readonly DependencyProperty MaxSelectionCountProperty =
                DependencyProperty.Register(
                        "MaxSelectionCount",
                        typeof(int),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(
                                1 /* default value */,
                                new PropertyChangedCallback(OnMaxSelectionCountChanged)),
                                new ValidateValueCallback(IsValidMaxSelectionCount));

        /// <summary>
        ///     The maximum number of days that can be selected at one time.
        /// </summary>
        public int MaxSelectionCount
        {
            get { return (int)GetValue(MaxSelectionCountProperty); }
            set { SetValue(MaxSelectionCountProperty, value); }
        }

        private static void OnMaxSelectionCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar calendar = (MonthCalendar)d;
            int newValue = (int)e.NewValue;

            if (newValue < calendar.SelectedDates.Count)
            {
                calendar.TrimSelectedDates(newValue);
            }
        }

        private static bool IsValidMaxSelectionCount(object o)
        {
            return (int)o > 0;
        }

        /// <summary> 
        ///     Delete redundant selected dates to fit the new MaxSelectionCount value
        /// </summary>
        private void TrimSelectedDates(int limit)
        {
            ObservableCollection<DateTime> selectedDates = SelectedDates;

            int numToRemove = selectedDates.Count - limit;
            Debug.Assert(numToRemove > 0, "TrimSelectedDates should only be called when MaxSelectionCount is less than the number of selected dates.");

            List<DateTime> unselected = new List<DateTime>(numToRemove);

            _isChangingSelection = true;
            try
            {
                for (int i = 0; i < numToRemove; i++)
                {
                    DateTime unselect = selectedDates[0];
                    unselected.Add(unselect);
                    selectedDates.RemoveAt(0);
                }
            }
            finally
            {
                _isChangingSelection = false;
            }

            UpdateSelectedDate(unselected);
            UpdateSelectionState(unselected, null);
            InvokeDateSelectedChangedEvent(unselected, null);
        }


        /// <summary>
        ///     The DependencyProperty for ShowsTitle property
        /// </summary>
        public static readonly DependencyProperty ShowsTitleProperty =
                DependencyProperty.Register(
                        "ShowsTitle",
                        typeof(bool),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        ///     Indicates whether the control displays the title or not.
        /// </summary>
        public bool ShowsTitle
        {
            get { return (bool)GetValue(ShowsTitleProperty); }
            set { SetValue(ShowsTitleProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        ///     The DependencyProperty for ShowsWeekNumbers property
        /// </summary>
        public static readonly DependencyProperty ShowsWeekNumbersProperty =
                DependencyProperty.Register(
                        "ShowsWeekNumbers",
                        typeof(bool),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        ///     Indicates whether the control displays week numbers.
        /// </summary>
        public bool ShowsWeekNumbers
        {
            get { return (bool)GetValue(ShowsWeekNumbersProperty); }
            set { SetValue(ShowsWeekNumbersProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        ///     The DependencyProperty for ShowsDayHeaders property
        /// </summary>
        public static readonly DependencyProperty ShowsDayHeadersProperty =
                DependencyProperty.Register(
                        "ShowsDayHeaders",
                        typeof(bool),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        ///     Indicates whether the control displays day headers.
        /// </summary>
        public bool ShowsDayHeaders
        {
            get { return (bool)GetValue(ShowsDayHeadersProperty); }
            set { SetValue(ShowsDayHeadersProperty, BooleanBoxes.Box(value)); }
        }

        #endregion

        #region Style and Template Settings

        /// <summary>
        ///     The DependencyProperty for the DayTemplate property.
        /// </summary>
        public static readonly DependencyProperty DayTemplateProperty =
                DependencyProperty.Register(
                        "DayTemplate",
                        typeof(DataTemplate),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata((DataTemplate)null));

        /// <summary>
        ///     DayTemplate is the template that describes how to convert a DateTime into the visualization for that day.
        /// </summary>
        public DataTemplate DayTemplate
        {
            get { return (DataTemplate)GetValue(DayTemplateProperty); }
            set { SetValue(DayTemplateProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for the DayTemplateSelector property.
        /// </summary>
        public static readonly DependencyProperty DayTemplateSelectorProperty =
                DependencyProperty.Register(
                        "DayTemplateSelector",
                        typeof(DataTemplateSelector),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata((DataTemplateSelector)null));

        /// <summary>
        ///     Allows the selection of different templates based on arbitrary data.
        /// </summary>
        public DataTemplateSelector DayTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DayTemplateSelectorProperty); }
            set { SetValue(DayTemplateSelectorProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for the DayContainerStyle property.
        /// </summary>
        public static readonly DependencyProperty DayContainerStyleProperty =
                DependencyProperty.Register(
                        "DayContainerStyle",
                        typeof(Style),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata((Style)null));

        /// <summary>
        ///     The style applied to each MonthCalendarItem container element.
        /// </summary>
        public Style DayContainerStyle
        {
            get { return (Style)GetValue(DayContainerStyleProperty); }
            set { SetValue(DayContainerStyleProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for the DayContainerStyleSelector property.
        /// </summary>
        public static readonly DependencyProperty DayContainerStyleSelectorProperty =
                DependencyProperty.Register(
                        "DayContainerStyleSelector",
                        typeof(StyleSelector),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata((StyleSelector)null));

        /// <summary>
        ///     Allows selection of different styles based on arbitaray data.
        /// </summary>
        public StyleSelector DayContainerStyleSelector
        {
            get { return (StyleSelector)GetValue(DayContainerStyleSelectorProperty); }
            set { SetValue(DayContainerStyleSelectorProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for the TitleStyle property.
        /// </summary>
        public static readonly DependencyProperty TitleStyleProperty =
                DependencyProperty.Register(
                        "TitleStyle",
                        typeof(Style),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata((Style)null));

        /// <summary>
        ///     The Style applied to the MonthCalendarTitle element.
        /// </summary>
        public Style TitleStyle
        {
            get { return (Style)GetValue(TitleStyleProperty); }
            set { SetValue(TitleStyleProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for the DayHeaderStyle property.
        /// </summary>
        public static readonly DependencyProperty DayHeaderStyleProperty =
                DependencyProperty.Register(
                        "DayHeaderStyle",
                        typeof(Style),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata((Style)null));

        /// <summary>
        ///     The Style applied to the MonthCalendarDayHeader elements.
        /// </summary>
        public Style DayHeaderStyle
        {
            get { return (Style)GetValue(DayHeaderStyleProperty); }
            set { SetValue(DayHeaderStyleProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for the WeekNumberStyle property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberStyleProperty =
                DependencyProperty.Register(
                        "WeekNumberStyle",
                        typeof(Style),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata((Style)null));

        /// <summary>
        ///     The Style applied to the MonthCalendarWeekNumber elements.
        /// </summary>
        public Style WeekNumberStyle
        {
            get { return (Style)GetValue(WeekNumberStyleProperty); }
            set { SetValue(WeekNumberStyleProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for the PreviousButtonStyle property.
        /// </summary>
        public static readonly DependencyProperty PreviousButtonStyleProperty =
                DependencyProperty.Register(
                        "PreviousButtonStyle",
                        typeof(Style),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata((Style)null));

        /// <summary>
        ///     The Style applied to the RepeatButton used to go to the previous month.
        /// </summary>
        public Style PreviousButtonStyle
        {
            get { return (Style)GetValue(PreviousButtonStyleProperty); }
            set { SetValue(PreviousButtonStyleProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for the NextButtonStyle property.
        /// </summary>
        public static readonly DependencyProperty NextButtonStyleProperty =
                DependencyProperty.Register(
                        "NextButtonStyle",
                        typeof(Style),
                        typeof(MonthCalendar),
                        new FrameworkPropertyMetadata((Style)null));

        /// <summary>
        ///     The Style applied to the RepeatButton used to go to the next month.
        /// </summary>
        public Style NextButtonStyle
        {
            get { return (Style)GetValue(NextButtonStyleProperty); }
            set { SetValue(NextButtonStyleProperty, value); }
        }

        #endregion


        #region Input

        /// <summary>
        ///     Called when a keyboard key is pressed.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            bool isShiftKeyDown = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            bool isControlKeyDown = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                    e.Handled = true;

                    if (isControlKeyDown)
                    {
                        if (e.Key == Key.Left)
                        {
                            // Move to the previous month
                            TryExecute(PreviousCommand);
                        }
                        else
                        {
                            // Move to the next month
                            TryExecute(NextCommand);
                        }
                    }
                    else
                    {
                        // Move to the next or previous day
                        MoveActiveItem(e.Key == Key.Left ? -1 : 1, isShiftKeyDown);
                    }
                    break;

                case Key.Up:
                case Key.Down:
                    e.Handled = true;

                    if (!isControlKeyDown)
                    {
                        // Move to the next or previous week
                        MoveActiveItem(e.Key == Key.Up ? -7 : 7, isShiftKeyDown);
                    }
                    break;

                case Key.PageUp:
                    if (!isControlKeyDown && !isShiftKeyDown)
                    {
                        e.Handled = true;

                        // Go to the previous month
                        TryExecute(PreviousCommand);
                    }
                    break;

                case Key.PageDown:
                    if (!isControlKeyDown && !isShiftKeyDown)
                    {
                        e.Handled = true;

                        // Go to the next month
                        TryExecute(NextCommand);
                    }
                    break;

                case Key.Home:
                    // Move to the first day of the current month
                    MoveActiveItem((FirstDateOfVisibleMonth > MinDate.Date) ? FirstDateOfVisibleMonth : MinDate, isShiftKeyDown);
                    e.Handled = true;
                    break;

                case Key.End:
                    // Move to the last day of the current month
                    MoveActiveItem((LastDateOfVisibleMonth < MaxDate) ? LastDateOfVisibleMonth.Date : MaxDate, isShiftKeyDown);
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        ///     If a command is enabled, executes it.
        /// </summary>
        private void TryExecute(RoutedCommand command)
        {
            if (command.CanExecute(null, this))
            {
                command.Execute(null, this);
            }
        }

        internal void MoveActiveItem(DateTime newDate, bool extendSelection)
        {
            TimeSpan diff = newDate - _currentActive;
            MoveActiveItem(diff.Days, extendSelection);
        }

        private void MoveActiveItem(int days, bool extendSelection)
        {
            List<DateTime> unselectedDates = null;
            List<DateTime> selectedDates = null;
            ObservableCollection<DateTime> currentlySelectedDates = SelectedDates;

            int totalDays = Math.Abs(days);
            DateTime newDate = _currentActive + new TimeSpan(days, 0, 0, 0);

            if (!MonthCalendarHelper.IsWithinRange(newDate, MinDate, MaxDate))
            {
                return;
            }

            int maxSelectionCount = MaxSelectionCount;

            _isChangingSelection = true;
            try
            {
                if (!extendSelection || (maxSelectionCount == 1))
                {
                    // Clear the previous selection and make the new date the selection

                    unselectedDates = new List<DateTime>();
                    unselectedDates.AddRange(currentlySelectedDates);
                    currentlySelectedDates.Clear();

                    if (unselectedDates.Contains(newDate))
                    {
                        unselectedDates.Remove(newDate);
                    }
                    else
                    {
                        selectedDates = new List<DateTime>();
                        selectedDates.Add(newDate);
                    }

                    currentlySelectedDates.Add(newDate);
                    _currentActive = newDate;
                    _anchor = null;
                }
                else
                {
                    selectedDates = new List<DateTime>();
                    unselectedDates = new List<DateTime>();
                    List<DateTime> previouslySelected = null;

                    if (_anchor == null)
                    {
                        _anchor = _currentActive;

                        // The anchor moved, clear out any previous selection
                        int count = currentlySelectedDates.Count;
                        if (count > 0)
                        {
                            previouslySelected = new List<DateTime>();
                            for (int i = 0; i < count; i++)
                            {
                                previouslySelected.Add(currentlySelectedDates[i]);
                            }
                            currentlySelectedDates.Clear();
                        }
                    }

                    TimeSpan oneDay = new TimeSpan(days > 0 ? 1 : -1, 0, 0, 0);
                    DateTime currentDay = _currentActive;
                    for (int i = 0; i <= totalDays; i++, currentDay += oneDay)
                    {
                        int compareAnchor = MonthCalendarHelper.CompareYearMonthDay(currentDay, (DateTime)_anchor);
                        int compareNew = MonthCalendarHelper.CompareYearMonthDay(currentDay, newDate);
                        if (((compareAnchor > 0) && (compareNew > 0)) ||
                            ((compareAnchor < 0) && (compareNew < 0)))
                        {
                            if (currentDay != newDate)
                            {
                                // Remove the day
                                if (currentlySelectedDates.Contains(currentDay))
                                {
                                    currentlySelectedDates.Remove(currentDay);
                                    unselectedDates.Add(currentDay);
                                }
                            }
                        }
                        else
                        {
                            // Limit the amount of days being added
                            if (currentlySelectedDates.Count >= maxSelectionCount)
                            {
                                break;
                            }

                            // Add the day
                            if (!currentlySelectedDates.Contains(currentDay))
                            {
                                currentlySelectedDates.Add(currentDay);
                                if ((previouslySelected != null) && previouslySelected.Contains(currentDay))
                                {
                                    // It was previously selected and we were going to remove it, but it's selected again
                                    previouslySelected.Remove(currentDay);
                                }
                                else
                                {
                                    selectedDates.Add(currentDay);
                                }
                            }
                        }

                        _currentActive = currentDay;
                    }

                    if (previouslySelected != null)
                    {
                        int count = previouslySelected.Count;
                        for (int i = 0; i < count; i++)
                        {
                            unselectedDates.Add(previouslySelected[i]);
                        }
                    }
                }

                DateTime visibleMonth = VisibleMonth;
                if (_currentActive.Month != visibleMonth.Month)
                {
                    int offset = MonthCalendarHelper.SubtractByMonth(_currentActive, visibleMonth);
                    ScrollVisibleMonth(offset);
                }

                // Set focus to the new active container
                MonthCalendarItem newActiveContainer = GetContainerFromDate(_currentActive);
                if (newActiveContainer != null)
                {
                    newActiveContainer.Focus();
                }

            }
            finally
            {
                _isChangingSelection = false;
            }

            if (((unselectedDates != null) && (unselectedDates.Count > 0)) ||
                ((selectedDates != null) && (selectedDates.Count > 0)))
            {
                UpdateSelectedDate(unselectedDates);
                UpdateSelectionState(unselectedDates, selectedDates);
                InvokeDateSelectedChangedEvent(unselectedDates, selectedDates);
            }
        }

        #endregion


        #region Calendar Days

        /// <summary>
        ///     Allows children in the template to notify the parent that it displays dates.
        /// </summary>
        /// <param name="item"></param>
        internal void RegisterCalendarItem(MonthCalendarItem item)
        {
            if (_children == null)
            {
                _children = new List<MonthCalendarItem>(42);
            }

            _children.Add(item);
        }

        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MonthCalendar calendar = (MonthCalendar)d;
            if (calendar._children != null)
            {
                calendar._children.Clear();
            }
        }

        /// <summary>
        ///     Return the UI element corresponding to the given date.
        ///     Returns null if the date is not visible.
        /// </summary>
        private MonthCalendarItem GetContainerFromDate(DateTime date)
        {
            if ((_children != null) && IsWithinVisibleRange(date))
            {
                // By default, the days will be in order, so attempt to access it via index.
                // Otherwise, fallback to a search.
                DateTime firstVisibleDay = FirstVisibleDay;
                TimeSpan span = date - firstVisibleDay;
                int daySpan = span.Days;
                if ((daySpan >= 0) && (daySpan < _children.Count))
                {
                    MonthCalendarItem child = _children[daySpan];

                    if (MonthCalendarHelper.IsWithinRange((DateTime)child.Content, date, date))
                    {
                        return child;
                    }
                }

                int count = _children.Count;
                for (int i = 0; i < count; i++)
                {
                    MonthCalendarItem child = _children[i];
                    if (MonthCalendarHelper.IsWithinRange((DateTime)child.Content, date, date))
                    {
                        return child;
                    }
                }
            }

            return null;
        }

        #endregion


        #region Miscellaneous

        /// <summary>
        ///     Returns a string representation of this control for debugging purposes.
        /// </summary>
        public override string ToString()
        {
            return base.ToString() + " VisibleMonth: " + VisibleMonth.ToShortDateString() + ", SelectedDates.Count: " + SelectedDates.Count.ToString();
        }

        #endregion


        #region Fields

        private static RoutedCommand _gotoCommand = null;
        private static RoutedCommand _nextCommand = null;
        private static RoutedCommand _previousCommand = null;

        private DateTime _currentActive = DateTime.Today;
        private DateTime? _anchor = null;
        private List<MonthCalendarItem> _children;
        private ObservableCollection<DateTime> _selectedDates;
        private bool _isChangingSelection;

        #endregion
    }
}
