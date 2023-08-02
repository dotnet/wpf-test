//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Diagnostics;       // Debug
using System.Globalization;     // CultureInfo
using System.Windows;
using System.Windows.Controls;  // Control
using System.Windows.Controls.Primitives; //ButtonBase
using System.Windows.Data;      // IValueConverter
using System.Windows.Input;
using System.Windows.Media;     
using System.Windows.Threading; // DispatcherPriority
using WpfControlToolkit;


namespace WpfControlToolkit
{
    /// <summary>
    /// The DatePicker control allows the user to enter or select a date and display it in 
    /// the specified format. User can limit the date that can be selected by setting the 
    /// selection range.  You might consider using a DatePicker control instead of a MonthCalendar 
    /// if you need custom date formatting and limit the selection to just one date.
    /// </summary>
    [TemplatePart(Name = "PART_EditableTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_DatePickerCalendar", Type = typeof(MonthCalendar))]
    public class DatePicker : Control
    {

        #region Constructor

        /// <summary>
        ///     Setup globally defined data.
        /// </summary>
        static DatePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatePicker), new FrameworkPropertyMetadata(typeof(DatePicker)));

            EventManager.RegisterClassHandler(typeof(DatePicker), ContextMenuService.ContextMenuOpeningEvent, new ContextMenuEventHandler(OnContextMenuOpen), true);
            EventManager.RegisterClassHandler(typeof(DatePicker), ContextMenuService.ContextMenuClosingEvent, new ContextMenuEventHandler(OnContextMenuClose), true);
        }

        /// <summary>
        ///     Instantiates an instance of this class.
        /// </summary>
        public DatePicker() : base()
        {
        }

        #endregion


        #region Parts

        /// <summary>
        /// Called when the Template's tree has been generated
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Detach from any previous template's TextBox
            if (_textBox != null)
            {
                _textBox.LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(OnEditableTextBoxLostFocus);
                _textBox.KeyDown -= new KeyEventHandler(OnEditableTextBoxKeyDown);
                _textBox.TextChanged -= new TextChangedEventHandler(OnEditableTextBoxTextChanged);
            }

            base.OnApplyTemplate();

            // Attach to the new template's controls
            _textBox = GetTemplateChild(c_EditableTextBoxTemplateName) as TextBox;
            if (_textBox != null)
            {
                _textBox.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnEditableTextBoxLostFocus);
                _textBox.KeyDown += new KeyEventHandler(OnEditableTextBoxKeyDown);
                _textBox.TextChanged += new TextChangedEventHandler(OnEditableTextBoxTextChanged);

                if (UsesTextBoxDisplay)
                {
                    UpdateEditableTextBox();
                }
            }

            _calendar = GetTemplateChild(c_DatePickerCalendarTemplateName) as MonthCalendar;
        }

        #endregion


        #region Popup

        /// <summary>
        ///     The DependencyProperty for the IsDropDownOpen property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
                DependencyProperty.Register(
                        "IsDropDownOpen",
                        typeof(bool),
                        typeof(DatePicker),
                        new FrameworkPropertyMetadata(
                                BooleanBoxes.FalseBox,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                new PropertyChangedCallback(OnIsDropDownOpenChanged),
                                new CoerceValueCallback(CoerceIsDropDownOpen)));

        /// <summary>
        ///     Whether or not the popup for this control is currently open.
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>
        ///     Coerce IsDropDownOpen with IsLoaded, so set IsDropDownOpen isn't true before the UI is ready.
        /// </summary>
        private static object CoerceIsDropDownOpen(DependencyObject d, object value)
        {
            if ((bool)value)
            {
                DatePicker dp = (DatePicker)d;
                if (!dp.IsLoaded)
                {
                    // Defer setting IsDropDownOpen to true after Loaded event is fired to show popup window correctly
                    dp.Loaded += new RoutedEventHandler(dp.OpenOnLoad);
                    return BooleanBoxes.FalseBox;
                }
            }

            return value;
        }

        private void OpenOnLoad(object sender, RoutedEventArgs e)
        {
            CoerceValue(IsDropDownOpenProperty);
            Loaded -= new RoutedEventHandler(OpenOnLoad);
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)d;
            datePicker.OnIsDropDownOpenChanged((bool)e.NewValue);
        }

        private void OnIsDropDownOpenChanged(bool isDropDownOpened)
        {
            if (isDropDownOpened)
            {
                // In edit mode, if the text has been changed before opening the drop-down content
                // parse the text and get the correct Value before the popup window is showed
                if (IsTextChanged && UsesTextBoxDisplay && (_textBox != null))
                {
                    UpdateValueFromEditableTextBox();
                }

                // Update the date in the calendar 
                if (_calendar != null)
                {
                    DateTime? value = Value;
                    if (value != _calendar.SelectedDate)
                    {
                        _calendar.SelectedDate = value;
                    }
                }

                // When the drop down opens, take capture
                CaptureToSubtree();

                // Queue an operation to scroll and focus the calendar.
                // We don't know exactly when the Popup will be open and ready,
                // which is why we are delaying this work until later.
                if (_calendar != null)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (DispatcherOperationCallback)delegate(object arg)
                    {
                        DatePicker dp = (DatePicker)arg;
                        if (dp._calendar != null)
                        {
                            DateTime? value = dp.Value;
                            if (value.HasValue)
                            {
                                dp._calendar.Goto(value.Value);
                            }
                            else
                            {
                                dp._calendar.Goto(DateTime.Today);
                            }
                        }
                        return null;
                    },
                    this);
                }

                OnDropDownOpened(new RoutedEventArgs(DropDownOpenedEvent));
            }
            else
            {
                // If focus is within the subtree, make sure we have the focus so that focus isn't in the closed popup
                if (IsKeyboardFocusWithin)
                {
                    // Delay changing focus until the dust settles from the popup closing
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (DispatcherOperationCallback)delegate(object arg)
                    {
                        DatePicker dp = (DatePicker)arg;
                        if (dp.IsKeyboardFocusWithin)
                        {
                            if ((dp._textBox != null) && dp.UsesTextBoxDisplay)
                            {
                                dp._textBox.Focus();
                            }
                            else
                            {
                                dp.Focus();
                            }
                        }
                        return null;
                    },
                    this);
                }

                if (HasMouseCapture)
                {
                    Mouse.Capture(null);
                }

                OnDropDownClosed(new RoutedEventArgs(DropDownClosedEvent));
            }
        }

        /// <summary>
        ///     An event that occurs when the drop down opened.
        /// </summary>
        public static readonly RoutedEvent DropDownOpenedEvent = EventManager.RegisterRoutedEvent("DropDownOpened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DatePicker));

        /// <summary>
        ///  An event that occurs when the drop down closed.
        /// </summary>
        public static readonly RoutedEvent DropDownClosedEvent = EventManager.RegisterRoutedEvent("DropDownClosed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DatePicker));

        /// <summary>
        ///     Add / Remove DropDownOpened handler
        /// </summary>
        public event RoutedEventHandler DropDownOpened
        {
            add { AddHandler(DropDownOpenedEvent, value); }
            remove { RemoveHandler(DropDownOpenedEvent, value); }
        }

        /// <summary>
        ///     Add / Remove DropDownClosed handler
        /// </summary>
        public event RoutedEventHandler DropDownClosed
        {
            add { AddHandler(DropDownClosedEvent, value); }
            remove { RemoveHandler(DropDownClosedEvent, value); }
        }

        /// <summary>
        ///     Raise DropDownOpened event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDropDownOpened(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        ///     Raise DropDownClosed event
        /// </summary>
        protected virtual void OnDropDownClosed(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion


        #region Editing

        /// <summary>
        ///     The DependencyProperty for the UsesTextBoxDisplay property
        /// </summary>
        public static readonly DependencyProperty UsesTextBoxDisplayProperty =
                DependencyProperty.Register(
                        "UsesTextBoxDisplay",
                        typeof(bool),
                        typeof(DatePicker),
                        new FrameworkPropertyMetadata(
                                BooleanBoxes.FalseBox,
                                new PropertyChangedCallback(OnUsesTextBoxDisplayChanged)));
        
        /// <summary>
        ///     Determines whether the text portion of the control uses a TextBox
        ///     instead of simply displaying unselectable, uneditable text.
        /// </summary>
        public bool UsesTextBoxDisplay
        {
            get { return (bool)GetValue(UsesTextBoxDisplayProperty); }
            set { SetValue(UsesTextBoxDisplayProperty, BooleanBoxes.Box(value)); }
        }

        private static void OnUsesTextBoxDisplayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker datepicker = (DatePicker)d;

            if ((bool)e.NewValue)
            {
                datepicker.UpdateEditableTextBox();
            }
        }

        /// <summary>
        ///     The DependencyProperty for the IsReadOnly Property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
                TextBox.IsReadOnlyProperty.AddOwner(typeof(DatePicker));

        /// <summary>
        ///     When the DatePicker uses a TextBox to display the date, whether the TextBox is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, BooleanBoxes.Box(value)); }
        }

        #endregion

        #region Value and Text

        /// <summary>
        ///     The key needed set a read-only property.
        /// </summary>
        private static readonly DependencyPropertyKey TextPropertyKey =
                DependencyProperty.RegisterReadOnly(
                        "Text",
                        typeof(string),
                        typeof(DatePicker),
                        new FrameworkPropertyMetadata(String.Empty));

        /// <summary>
        ///     The DependencyProperty for the Text property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = TextPropertyKey.DependencyProperty;

        /// <summary>
        ///     The Value formatted as text. If the Value is null, it should be equal to NullValueText.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            private set { SetValue(TextPropertyKey, value); }
        }

        /// <summary>
        /// The DependencyProperty for the Value property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register(
                        "Value",
                        typeof(DateTime?),
                        typeof(DatePicker),
                        new FrameworkPropertyMetadata(
                                (DateTime?)null,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                                new PropertyChangedCallback(OnValueChanged),
                                new CoerceValueCallback(CoerceValue)));

        /// <summary>
        /// The DateTime value of DatePicker
        /// </summary>
        public DateTime? Value
        {
            get { return (DateTime?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        ///     The user may have typed in changes to the TextBox,
        ///     but they may not have been parsed and reflected in Value.
        ///     Call this method to immediately parse what is in the TextBox.
        /// </summary>
        /// <returns>The value of IsValid after parsing.</returns>
        public bool ParsePendingTextChanges()
        {
            if (IsTextChanged && UsesTextBoxDisplay && (_textBox != null))
            {
                UpdateValueFromEditableTextBox();
            }

            return IsValid;
        }

        private static object CoerceValue(DependencyObject d, object value)
        {
            DatePicker datepicker = (DatePicker)d;

            if (value != null)
            {
                // Keep values between the minimum and maximum date
                DateTime newValue = ((DateTime)value).Date;

                DateTime min = datepicker.MinDate;
                if (newValue < min)
                {
                    return min;
                }

                DateTime max = datepicker.MaxDate;
                if (newValue > max)
                {
                    return max;
                }
            }

            return value;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker datepicker = (DatePicker)d;
            DateTime? oldValue = (DateTime?)e.OldValue;
            DateTime? newValue = (DateTime?)e.NewValue;

            datepicker.UpdateIsValid();
            datepicker.UpdateTextFromDate(newValue);

            if (datepicker.IsDropDownOpen && (datepicker._calendar != null))
            {
                datepicker._calendar.SelectedDate = newValue;
            }

            RoutedPropertyChangedEventArgs<DateTime?> routedArgs =
                    new RoutedPropertyChangedEventArgs<DateTime?>(oldValue, newValue, ValueChangedEvent);

            datepicker.OnValueChanged(routedArgs);
        }

        /// <summary>
        /// An event reporting that the Value property changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<DateTime?> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        /// <summary>
        /// Event ID correspond to Value changed event
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<DateTime?>), typeof(DatePicker));

        /// <summary>
        ///     This method is invoked when the Value property changes.
        /// </summary>
        /// <param name="e">RoutedPropertyChangedEventArgs contains the old and new value.</param>
        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            RaiseEvent(e);
        }

        /// <summary>
        ///     The DependencyProperty for the DateConverter Property
        /// </summary>
        public static readonly DependencyProperty DateConverterProperty =
                DependencyProperty.Register(
                        "DateConverter",
                        typeof(IValueConverter),
                        typeof(DatePicker),
                        new FrameworkPropertyMetadata(
                                (IValueConverter)null,
                                new PropertyChangedCallback(OnDateConverterChanged)));


        /// <summary>
        ///     This property is used to parse/format between Value and Text.
        /// </summary>
        /// <remarks>
        ///     ConvertBack is used to customize the parsing logic
        ///     Convert is used to customimze the formatting logic
        ///     If the converter can't parse the input text correctly, throw FormatException will fire InvalidEntry event.
        /// </remarks>
        public IValueConverter DateConverter
        {
            get { return (IValueConverter)GetValue(DateConverterProperty); }
            set { SetValue(DateConverterProperty, value); }
        }

        private static void OnDateConverterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DatePicker)d).UpdateTextFromValue();
        }

        /// <summary>
        /// The DependencyProperty for the NullValueText Property
        /// </summary>
        public static readonly DependencyProperty NullValueTextProperty =
                DependencyProperty.Register(
                        "NullValueText",
                        typeof(string),
                        typeof(DatePicker),
                        new FrameworkPropertyMetadata(
                                String.Empty,
                                new PropertyChangedCallback(OnNullValueTextChanged)));

        /// <summary>
        ///     This property indicates which input string should convert the Value of DatePicker into the null value.
        /// </summary>
        public string NullValueText
        {
            get { return (string)GetValue(NullValueTextProperty); }
            set { SetValue(NullValueTextProperty, value); }
        }

        private static void OnNullValueTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker datepicker = (DatePicker)d;

            if (!datepicker.Value.HasValue)
            {
                datepicker.UpdateTextFromDate(null);
            }
        }

        /// <summary>
        ///     The key needed set a read-only property.
        /// </summary>
        private static readonly DependencyPropertyKey IsValidPropertyKey =
                DependencyProperty.RegisterReadOnly(
                        "IsValid",
                        typeof(bool),
                        typeof(DatePicker),
                        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        ///     The DependencyProperty for the IsValid property.
        /// </summary>
        public static readonly DependencyProperty IsValidProperty = IsValidPropertyKey.DependencyProperty;

        /// <summary>
        ///     A property indicating whether the Value is valid or not.
        /// </summary>
        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            private set { SetValue(IsValidPropertyKey, value); }
        }

        private void UpdateIsValid()
        {
            IsValid = Value.HasValue;
        }

        /// <summary>
        ///     InvalidEntry event
        /// </summary>
        public static readonly RoutedEvent InvalidEntryEvent = EventManager.RegisterRoutedEvent("InvalidEntry", RoutingStrategy.Bubble, typeof(InvalidEntryEventHandler), typeof(DatePicker));

        /// <summary>
        ///     Add / Remove InvalidEntry handler
        /// </summary>
        public event InvalidEntryEventHandler InvalidEntry
        {
            add { AddHandler(InvalidEntryEvent, value); }
            remove { RemoveHandler(InvalidEntryEvent, value); }
        }

        /// <summary>
        ///     This event is invoked when datepicker can't parse the input string correctly
        /// </summary>
        protected virtual void OnInvalidEntry(InvalidEntryEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion


        #region Max/MinDate

        /// <summary>
        ///     The Property for the MinDate property.
        /// </summary>
        public static readonly DependencyProperty MinDateProperty =
            MonthCalendar.MinDateProperty.AddOwner(typeof(DatePicker),
                    new FrameworkPropertyMetadata(
                            new DateTime(1753, 1, 1), /* The default value */
                            new PropertyChangedCallback(OnMinDateChanged),
                            new CoerceValueCallback(OnCoerceDateTime)));

        /// <summary>
        ///     The minimum date of DatePicker.
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
            DatePicker datepicker = (DatePicker)d;
            datepicker.CoerceValue(MaxDateProperty);
            datepicker.CoerceValue(ValueProperty);
        }

        /// <summary>
        ///     The Property for the MaxDate property.
        /// </summary>
        public static readonly DependencyProperty MaxDateProperty =
            MonthCalendar.MaxDateProperty.AddOwner(typeof(DatePicker),
                    new FrameworkPropertyMetadata(
                            new DateTime(9998, 12, 31), /* The default value */
                            new PropertyChangedCallback(OnMaxDateChanged),
                            new CoerceValueCallback(CoerceMaxDate)));

        /// <summary>
        ///     The maximum date of DatePicker.
        /// </summary>
        public DateTime MaxDate
        {
            get { return (DateTime)GetValue(MaxDateProperty); }
            set { SetValue(MaxDateProperty, value); }
        }

        private static void OnMaxDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DatePicker datepicker = (DatePicker)d;
            datepicker.CoerceValue(ValueProperty);
        }

        private static object CoerceMaxDate(DependencyObject d, object value)
        {
            DatePicker datepicker = (DatePicker)d;
            DateTime newValue = ((DateTime)value).Date;

            DateTime min = datepicker.MinDate;
            if (newValue < min)
            {
                return min;
            }

            return value;
        }

        #endregion


        #region Styles

        /// <summary>
        ///     The DependencyProperty for the MonthCalendarStyle Property
        /// </summary>
        public static readonly DependencyProperty MonthCalendarStyleProperty =
                DependencyProperty.Register(
                        "MonthCalendarStyle",
                        typeof(Style),
                        typeof(DatePicker),
                        new FrameworkPropertyMetadata(
                                (Style)null,
                                FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///     The style of drop-down MonthCalendar
        /// </summary>
        public Style MonthCalendarStyle
        {
            get { return (Style)GetValue(MonthCalendarStyleProperty); }
            set { SetValue(MonthCalendarStyleProperty, value); }
        }

        /// <summary>
        ///     The DependencyProperty for the DropDownButtonStyle property.
        /// </summary>
        public static readonly DependencyProperty DropDownButtonStyleProperty =
                DependencyProperty.Register(
                        "DropDownButtonStyle",
                        typeof(Style),
                        typeof(DatePicker),
                        new FrameworkPropertyMetadata((Style)null));

        /// <summary>
        ///     DropDownButtonStyle property
        /// </summary>
        public Style DropDownButtonStyle
        {
            get { return (Style)GetValue(DropDownButtonStyleProperty); }
            set { SetValue(DropDownButtonStyleProperty, value); }
        }

        #endregion


        #region Input

        /// <summary>
        ///     Close the popup if DatePicker loses mouse capture.
        /// </summary>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            if (!HasMouseCapture)
            {
                if (IsDropDownOpen)
                {
                    if (Mouse.Captured == null)
                    {
                        CaptureToSubtree();
                    }
                    else if (!DatePickerHelper.IsDescendant(this, Mouse.Captured as Visual) && !IsContextMenuOpen)
                    {
                        IsDropDownOpen = false;
                    }

                    e.Handled = true;
                }
            }
        }

        /// <summary>
        ///     Takes mouse capture.
        /// </summary>
        private void CaptureToSubtree()
        {
            if (_calendar != null)
            {
                Mouse.Capture(_calendar, CaptureMode.SubTree);
            }
            else
            {
                Mouse.Capture(this, CaptureMode.SubTree);
            }
        }
        
        /// <summary>
        ///     Handles any mouse button being pressed.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            // If the mouse is clicked outside of the popup, then close it.
            // This occurs if the OriginalSource is the calendar or the DatePicker itself.
            // Otherwise the event would be handled or one of the inner components.
            if (IsDropDownOpen && HasMouseCapture && ((e.OriginalSource == _calendar) || (e.OriginalSource == this)))
            {
                IsDropDownOpen = false;
                e.Handled = true;
            }
        }

        /// <summary>
        ///     Handles the left mouse button being released.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (IsDropDownOpen)
            {
                Visual reference = (_calendar != null) ? (Visual)_calendar : (Visual)this;
                if (DatePickerHelper.IsDescendant(reference, e.OriginalSource as Visual))
                {
                    // A selection was made
                    IsDropDownOpen = false;
                    CommitSelection();
                }

                e.Handled = true;
            }
        }

        /// <summary>
        ///     Handles the left mouse button being pressed.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
 	        base.OnMouseLeftButtonDown(e);
            e.Handled = true;

            if (!IsKeyboardFocusWithin && !IsContextMenuOpen)
            {
                // Move focus appropriately
                if (IsDropDownOpen)
                {
                    if (_calendar != null)
                    {
                        _calendar.Focus();
                    }
                    else
                    {
                        Focus();
                    }
                }
                else
                {
                    if ((_textBox != null) && UsesTextBoxDisplay)
                    {
                        _textBox.Focus();
                    }
                    else
                    {
                        Focus();
                    }
                }
            }
        }

        /// <summary>
        ///     Handles any keyboard key being pressed.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            Key key = e.Key;
            if (key == Key.System)
            {
                key = e.SystemKey;
            }

            if (IsDropDownOpen)
            {
                switch (key)
                {
                    case Key.Enter:
                        IsDropDownOpen = false;
                        CommitSelection();
                        e.Handled = true;
                        break;

                    case Key.Escape:
                        IsDropDownOpen = false;
                        e.Handled = true;
                        break;

                    case Key.Up:
                    case Key.Down:
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                        {
                            IsDropDownOpen = false;
                            e.Handled = true;
                        }
                        break;

                    case Key.F4:
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == 0)
                        {
                            IsDropDownOpen = false;
                            e.Handled = true;
                        }
                        break;

                    case Key.Tab:
                        IsDropDownOpen = false;
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                switch (key)
                {
                    case Key.Up:
                    case Key.Down:
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                        {
                            IsDropDownOpen = true;
                            e.Handled = true;
                        }
                        break;

                    case Key.F4:
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == 0)
                        {
                            IsDropDownOpen = true;
                            e.Handled = true;
                        }
                        break;
                }
            }
        }

        /// <summary>
        ///     Called when this element gets focus.
        /// </summary>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            // If we're an editable datepicker, forward focus to the TextBox element
            if (!e.Handled && (e.NewFocus == this) && UsesTextBoxDisplay && (_textBox != null))
            {
                _textBox.Focus();
                e.Handled = true;
            }
        }

        /// <summary>
        ///     If EditableTextBoxSite loses focus and Text has been changed, DatePicker will parse Text
        /// </summary>
        private void OnEditableTextBoxLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if ((_textBox != null) && UsesTextBoxDisplay && IsTextChanged && !IsContextMenuOpen)
            {
                UpdateValueFromEditableTextBox();
                e.Handled = true;
            }
        }

        /// <summary>
        ///     If Key.Enter is pressed, DatePicker will parse the text.
        /// </summary>
        private void OnEditableTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter) && (_textBox != null) && UsesTextBoxDisplay)
            {
                UpdateValueFromEditableTextBox();
                e.Handled = true;
            }
        }

        /// <summary>
        ///     Mark that text has changed.
        /// </summary>
        private void OnEditableTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            IsTextChanged = true;
        }

        /// <summary>
        ///     Update the value based on the calendar's selection.
        /// </summary>
        private void CommitSelection()
        {
            Value = _calendar.SelectedDate;
        }

        /// <summary>
        ///     Mark that the ContextMenu is open.
        /// </summary>
        private static void OnContextMenuOpen(object sender, ContextMenuEventArgs e)
        {
            ((DatePicker)sender).IsContextMenuOpen = true;
        }

        /// <summary>
        ///     Mark that the ContexMenu is closed.
        /// </summary>
        private static void OnContextMenuClose(object sender, ContextMenuEventArgs e)
        {
            ((DatePicker)sender).IsContextMenuOpen = false;
        }

        #endregion


        #region Parsing

        /// <summary>
        ///     Parse the input string, if the input string is a valid date, return the Date, else return null
        /// </summary>
        /// <param name="text">The input string</param>
        /// <remarks>
        ///     If the input entry equals NullValueText, Value will be set to null
        ///     If the input entry is a valid date, Value will be set to the input date
        ///     If the input entry isn't a valid date, InvalidEntry event will be fired, Value will still keep the old value (don't set to null)
        /// </remarks>
        private void UpdateValueFromEditableTextBox()
        {
            if (IsParsing && (_textBox == null))
                return;

            string text = _textBox.Text;

            DateTime? date = null;
            bool isValidDate = true;
            IsTextChanged = false;
            IsParsing = true;

            if (text == NullValueText)
            {
                IsParsing = false;
            }
            else
            {
                // Use DateConverter to parse if available
                object ret = null;
                try
                {
                    CultureInfo cultureInfo = Language != null ? Language.GetSpecificCulture() : null;
                    if (DateConverter != null)
                    {
                        ret = DateConverter.ConvertBack(text, typeof(DateTime), null, cultureInfo);
                    }
                    else
                    {
                        ret = _defaultDateConverter.ConvertBack(text, typeof(DateTime), null, cultureInfo);
                    }
                }
                catch (FormatException)
                {
                    isValidDate = false;
                }
                finally
                {
                    IsParsing = false;
                }

                if (ret is DateTime)
                {
                    date = new DateTime?((DateTime)ret);
                }
            }

            if (isValidDate)
            {
                // Update Value with the valid date
                DateTime? oldValue = Value;
                Value = date;
                UpdateTextFromValue();
            }
            else
            {
                // If the input entry isn't a valid date, fire InvalidEntry event
                IgnoreUpdateEditableTextBox = true;
                try
                {
                    Value = null;
                }
                finally
                {
                    IgnoreUpdateEditableTextBox = false;
                }

                if (_textBox != null)
                {
                    if (!_textBox.IsFocused)
                    {
                        _textBox.Focus();
                    }
                    _textBox.SelectAll();
                }

                InvalidEntryEventArgs args = new InvalidEntryEventArgs(InvalidEntryEvent, text);
                OnInvalidEntry(args);
            }
        }

        /// <summary>
        /// Format Value property to a formatted string
        /// </summary>
        private string UpdateTextFromValue()
        {
            return UpdateTextFromDate(Value);
        }

        private string UpdateTextFromDate(DateTime? date)
        {
            string text;

            if (date.HasValue)
            {
                CultureInfo cultureInfo = Language != null ? Language.GetSpecificCulture() : null;
                object o = null;
                if (DateConverter != null)
                {
                    o = DateConverter.Convert(date.Value, typeof(string), null, cultureInfo);
                }
                else
                {
                    o = _defaultDateConverter.Convert(date.Value, typeof(string), null, cultureInfo);
                }

                text = Convert.ToString(o, cultureInfo);
            }
            else
            {
                text = NullValueText;
            }

            Text = text;
            if (UsesTextBoxDisplay && !IgnoreUpdateEditableTextBox)
            {
                UpdateEditableTextBox();
            }

            return text;
        }

        /// <summary>
        /// Update the Text to the TextBox in editable mode
        /// </summary>
        private void UpdateEditableTextBox()
        {
            // If the DatePicker is editable, it must have an EditableTextBoxSite
            if (_textBox != null)
            {
                string text = Text;
                if (!string.Equals(_textBox.Text, text, StringComparison.Ordinal))
                {
                    _textBox.Text = text;
                    IsTextChanged = false; //Ignore internal set Text fired TextChanged event
                }

                // If we have focus and the IsDropDownOpen is false, set the focus to the TextBox
                if (IsKeyboardFocusWithin)
                {
                    if (!IsDropDownOpen)
                    {
                        _textBox.Focus();
                    }
                    _textBox.SelectAll();
                }
            }
        }

        #endregion


        #region Flags and State

        private bool GetFlag(Flags flag)
        {
            return (_flags & flag) == flag;
        }

        private void SetFlag(Flags flag, bool set)
        {
            if (set)
            {
                _flags |= flag;
            }
            else
            {
                _flags &= (~flag);
            }
        }

        private bool IsContextMenuOpen
        {
            get { return GetFlag(Flags.IsContextMenuOpen); }
            set { SetFlag(Flags.IsContextMenuOpen, value); }
        }

        private bool IsTextChanged
        {
            get { return GetFlag(Flags.IsTextChanged); }
            set { SetFlag(Flags.IsTextChanged, value); }
        }

        private bool IsParsing
        {
            get { return GetFlag(Flags.IsParsing); }
            set { SetFlag(Flags.IsParsing, value); }
        }

        private bool IgnoreUpdateEditableTextBox
        {
            get { return GetFlag(Flags.IgnoreUpdateEditableTextBox); }
            set { SetFlag(Flags.IgnoreUpdateEditableTextBox, value); }
        }

        private bool HasMouseCapture
        {
            get
            {
                if (_calendar != null)
                {
                    return Mouse.Captured == _calendar;
                }
                else
                {
                    return Mouse.Captured == this;
                }
            }
        }

        [Flags]
        private enum Flags
        {
            //True if user has changed the text of TextBox
            IsTextChanged = 0x00000001,
            //Avoid reentry the parse process in DoParse()
            IsParsing = 0x00000002,
            IsContextMenuOpen = 0x00000004,
            //True to ignore updating EditableTextBox when Value is changed
            IgnoreUpdateEditableTextBox = 0x00000020,
        }

        #endregion


        #region Miscellaneous

        /// <summary>
        /// Returns a string representation for this control.
        /// "...DatePicker, Value:06/02/2006"
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();

            if (Value.HasValue)
            {
                s += ", Value:" + Value.Value.ToShortDateString();
            }

            return s;
        }

        #endregion


        #region Fields

        private TextBox _textBox;
        private MonthCalendar _calendar;
        private Flags _flags;

        //Default DateConverter, it's used if user doesn't provide the DateConverter
        private readonly IValueConverter _defaultDateConverter = new DateTimeValueConverter();

        // Part names used in the style. The class TemplatePartAttribute should use the same names
        private const string c_EditableTextBoxTemplateName = "PART_EditableTextBox";
        private const string c_DatePickerCalendarTemplateName = "PART_DatePickerCalendar";

        #endregion
    }
}
