using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

//
// AutoGen Test for WFHUtil
//
public class WFHUtil
{
    // class vars
    private static string _events;

    // member functions
    public static void ResetEvents() { _events = ""; }
    public static string GetEvents() { return _events; }

    private static void LogEvent(string evt)
    {
        _events += evt + ':';
    }

    // AddHandlers function
    public static void AddHandlers(WindowsFormsHost wfh)
    {
        ResetEvents();

        // add event handlers
        wfh.DataContextChanged += new System.Windows.DependencyPropertyChangedEventHandler(wfh_DataContextChanged);
        wfh.RequestBringIntoView += new System.Windows.RequestBringIntoViewEventHandler(wfh_RequestBringIntoView);
        wfh.SizeChanged += new System.Windows.SizeChangedEventHandler(wfh_SizeChanged);
        wfh.Initialized += new System.EventHandler(wfh_Initialized);
        wfh.Loaded += new System.Windows.RoutedEventHandler(wfh_Loaded);
        wfh.Unloaded += new System.Windows.RoutedEventHandler(wfh_Unloaded);
        wfh.ToolTipOpening += new System.Windows.Controls.ToolTipEventHandler(wfh_ToolTipOpening);
        wfh.ToolTipClosing += new System.Windows.Controls.ToolTipEventHandler(wfh_ToolTipClosing);
        wfh.ContextMenuOpening += new System.Windows.Controls.ContextMenuEventHandler(wfh_ContextMenuOpening);
        wfh.ContextMenuClosing += new System.Windows.Controls.ContextMenuEventHandler(wfh_ContextMenuClosing);
        wfh.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(wfh_PreviewMouseDown);
        wfh.MouseDown += new System.Windows.Input.MouseButtonEventHandler(wfh_MouseDown);
        wfh.PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler(wfh_PreviewMouseUp);
        wfh.MouseUp += new System.Windows.Input.MouseButtonEventHandler(wfh_MouseUp);
        wfh.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(wfh_PreviewMouseLeftButtonDown);
        wfh.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(wfh_MouseLeftButtonDown);
        wfh.PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(wfh_PreviewMouseLeftButtonUp);
        wfh.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(wfh_MouseLeftButtonUp);
        wfh.PreviewMouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(wfh_PreviewMouseRightButtonDown);
        wfh.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(wfh_MouseRightButtonDown);
        wfh.PreviewMouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(wfh_PreviewMouseRightButtonUp);
        wfh.MouseRightButtonUp += new System.Windows.Input.MouseButtonEventHandler(wfh_MouseRightButtonUp);
        wfh.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(wfh_PreviewMouseMove);
        wfh.MouseMove += new System.Windows.Input.MouseEventHandler(wfh_MouseMove);
        wfh.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(wfh_PreviewMouseWheel);
        wfh.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(wfh_MouseWheel);
        wfh.MouseEnter += new System.Windows.Input.MouseEventHandler(wfh_MouseEnter);
        wfh.MouseLeave += new System.Windows.Input.MouseEventHandler(wfh_MouseLeave);
        wfh.GotMouseCapture += new System.Windows.Input.MouseEventHandler(wfh_GotMouseCapture);
        wfh.LostMouseCapture += new System.Windows.Input.MouseEventHandler(wfh_LostMouseCapture);
        wfh.QueryCursor += new System.Windows.Input.QueryCursorEventHandler(wfh_QueryCursor);
        wfh.PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(wfh_PreviewStylusDown);
        wfh.StylusDown += new System.Windows.Input.StylusDownEventHandler(wfh_StylusDown);
        wfh.PreviewStylusUp += new System.Windows.Input.StylusEventHandler(wfh_PreviewStylusUp);
        wfh.StylusUp += new System.Windows.Input.StylusEventHandler(wfh_StylusUp);
        wfh.PreviewStylusMove += new System.Windows.Input.StylusEventHandler(wfh_PreviewStylusMove);
        wfh.StylusMove += new System.Windows.Input.StylusEventHandler(wfh_StylusMove);
        wfh.PreviewStylusInAirMove += new System.Windows.Input.StylusEventHandler(wfh_PreviewStylusInAirMove);
        wfh.StylusInAirMove += new System.Windows.Input.StylusEventHandler(wfh_StylusInAirMove);
        wfh.StylusEnter += new System.Windows.Input.StylusEventHandler(wfh_StylusEnter);
        wfh.StylusLeave += new System.Windows.Input.StylusEventHandler(wfh_StylusLeave);
        wfh.PreviewStylusInRange += new System.Windows.Input.StylusEventHandler(wfh_PreviewStylusInRange);
        wfh.StylusInRange += new System.Windows.Input.StylusEventHandler(wfh_StylusInRange);
        wfh.PreviewStylusOutOfRange += new System.Windows.Input.StylusEventHandler(wfh_PreviewStylusOutOfRange);
        wfh.StylusOutOfRange += new System.Windows.Input.StylusEventHandler(wfh_StylusOutOfRange);
        wfh.PreviewStylusSystemGesture += new System.Windows.Input.StylusSystemGestureEventHandler(wfh_PreviewStylusSystemGesture);
        wfh.StylusSystemGesture += new System.Windows.Input.StylusSystemGestureEventHandler(wfh_StylusSystemGesture);
        wfh.GotStylusCapture += new System.Windows.Input.StylusEventHandler(wfh_GotStylusCapture);
        wfh.LostStylusCapture += new System.Windows.Input.StylusEventHandler(wfh_LostStylusCapture);
        wfh.StylusButtonDown += new System.Windows.Input.StylusButtonEventHandler(wfh_StylusButtonDown);
        wfh.StylusButtonUp += new System.Windows.Input.StylusButtonEventHandler(wfh_StylusButtonUp);
        wfh.PreviewStylusButtonDown += new System.Windows.Input.StylusButtonEventHandler(wfh_PreviewStylusButtonDown);
        wfh.PreviewStylusButtonUp += new System.Windows.Input.StylusButtonEventHandler(wfh_PreviewStylusButtonUp);
        wfh.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(wfh_PreviewKeyDown);
        wfh.KeyDown += new System.Windows.Input.KeyEventHandler(wfh_KeyDown);
        wfh.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(wfh_PreviewKeyUp);
        wfh.KeyUp += new System.Windows.Input.KeyEventHandler(wfh_KeyUp);
        wfh.PreviewGotKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(wfh_PreviewGotKeyboardFocus);
        wfh.GotKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(wfh_GotKeyboardFocus);
        wfh.PreviewLostKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(wfh_PreviewLostKeyboardFocus);
        wfh.LostKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(wfh_LostKeyboardFocus);
        wfh.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(wfh_PreviewTextInput);
        wfh.TextInput += new System.Windows.Input.TextCompositionEventHandler(wfh_TextInput);
        wfh.PreviewQueryContinueDrag += new System.Windows.QueryContinueDragEventHandler(wfh_PreviewQueryContinueDrag);
        wfh.QueryContinueDrag += new System.Windows.QueryContinueDragEventHandler(wfh_QueryContinueDrag);
        wfh.PreviewGiveFeedback += new System.Windows.GiveFeedbackEventHandler(wfh_PreviewGiveFeedback);
        wfh.GiveFeedback += new System.Windows.GiveFeedbackEventHandler(wfh_GiveFeedback);
        wfh.PreviewDragEnter += new System.Windows.DragEventHandler(wfh_PreviewDragEnter);
        wfh.DragEnter += new System.Windows.DragEventHandler(wfh_DragEnter);
        wfh.PreviewDragOver += new System.Windows.DragEventHandler(wfh_PreviewDragOver);
        wfh.DragOver += new System.Windows.DragEventHandler(wfh_DragOver);
        wfh.PreviewDragLeave += new System.Windows.DragEventHandler(wfh_PreviewDragLeave);
        wfh.DragLeave += new System.Windows.DragEventHandler(wfh_DragLeave);
        wfh.PreviewDrop += new System.Windows.DragEventHandler(wfh_PreviewDrop);
        wfh.Drop += new System.Windows.DragEventHandler(wfh_Drop);
        wfh.IsMouseDirectlyOverChanged += new System.Windows.DependencyPropertyChangedEventHandler(wfh_IsMouseDirectlyOverChanged);
        wfh.IsKeyboardFocusWithinChanged += new System.Windows.DependencyPropertyChangedEventHandler(wfh_IsKeyboardFocusWithinChanged);
        wfh.IsMouseCapturedChanged += new System.Windows.DependencyPropertyChangedEventHandler(wfh_IsMouseCapturedChanged);
        wfh.IsMouseCaptureWithinChanged += new System.Windows.DependencyPropertyChangedEventHandler(wfh_IsMouseCaptureWithinChanged);
        wfh.IsStylusDirectlyOverChanged += new System.Windows.DependencyPropertyChangedEventHandler(wfh_IsStylusDirectlyOverChanged);
        wfh.IsStylusCapturedChanged += new System.Windows.DependencyPropertyChangedEventHandler(wfh_IsStylusCapturedChanged);
        wfh.IsStylusCaptureWithinChanged += new System.Windows.DependencyPropertyChangedEventHandler(wfh_IsStylusCaptureWithinChanged);
        wfh.IsKeyboardFocusedChanged += new System.Windows.DependencyPropertyChangedEventHandler(wfh_IsKeyboardFocusedChanged);
        wfh.LayoutUpdated += new System.EventHandler(wfh_LayoutUpdated);
        wfh.GotFocus += new System.Windows.RoutedEventHandler(wfh_GotFocus);
        wfh.LostFocus += new System.Windows.RoutedEventHandler(wfh_LostFocus);
        wfh.IsEnabledChanged += new System.Windows.DependencyPropertyChangedEventHandler(wfh_IsEnabledChanged);
        wfh.IsHitTestVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(wfh_IsHitTestVisibleChanged);
        wfh.IsVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(wfh_IsVisibleChanged);
        wfh.FocusableChanged += new System.Windows.DependencyPropertyChangedEventHandler(wfh_FocusableChanged);
    }

    // event handlers
    static void wfh_DataContextChanged(System.Object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        LogEvent("DataContextChanged");
    }

    static void wfh_RequestBringIntoView(System.Object sender, System.Windows.RequestBringIntoViewEventArgs e)
    {
        LogEvent("RequestBringIntoView");
    }

    static void wfh_SizeChanged(System.Object sender, System.Windows.SizeChangedEventArgs e)
    {
        LogEvent("SizeChanged");
    }

    static void wfh_Initialized(System.Object sender, System.EventArgs e)
    {
        LogEvent("Initialized");
    }

    static void wfh_Loaded(System.Object sender, System.Windows.RoutedEventArgs e)
    {
        LogEvent("Loaded");
    }

    static void wfh_Unloaded(System.Object sender, System.Windows.RoutedEventArgs e)
    {
        LogEvent("Unloaded");
    }

    static void wfh_ToolTipOpening(System.Object sender, System.Windows.Controls.ToolTipEventArgs e)
    {
        LogEvent("ToolTipOpening");
    }

    static void wfh_ToolTipClosing(System.Object sender, System.Windows.Controls.ToolTipEventArgs e)
    {
        LogEvent("ToolTipClosing");
    }

    static void wfh_ContextMenuOpening(System.Object sender, System.Windows.Controls.ContextMenuEventArgs e)
    {
        LogEvent("ContextMenuOpening");
    }

    static void wfh_ContextMenuClosing(System.Object sender, System.Windows.Controls.ContextMenuEventArgs e)
    {
        LogEvent("ContextMenuClosing");
    }

    static void wfh_PreviewMouseDown(System.Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        LogEvent("PreviewMouseDown");
    }

    static void wfh_MouseDown(System.Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        LogEvent("MouseDown");
    }

    static void wfh_PreviewMouseUp(System.Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        LogEvent("PreviewMouseUp");
    }

    static void wfh_MouseUp(System.Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        LogEvent("MouseUp");
    }

    static void wfh_PreviewMouseLeftButtonDown(System.Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        LogEvent("PreviewMouseLeftButtonDown");
    }

    static void wfh_MouseLeftButtonDown(System.Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        LogEvent("MouseLeftButtonDown");
    }

    static void wfh_PreviewMouseLeftButtonUp(System.Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        LogEvent("PreviewMouseLeftButtonUp");
    }

    static void wfh_MouseLeftButtonUp(System.Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        LogEvent("MouseLeftButtonUp");
    }

    static void wfh_PreviewMouseRightButtonDown(System.Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        LogEvent("PreviewMouseRightButtonDown");
    }

    static void wfh_MouseRightButtonDown(System.Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        LogEvent("MouseRightButtonDown");
    }

    static void wfh_PreviewMouseRightButtonUp(System.Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        LogEvent("PreviewMouseRightButtonUp");
    }

    static void wfh_MouseRightButtonUp(System.Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        LogEvent("MouseRightButtonUp");
    }

    static void wfh_PreviewMouseMove(System.Object sender, System.Windows.Input.MouseEventArgs e)
    {
        LogEvent("PreviewMouseMove");
    }

    static void wfh_MouseMove(System.Object sender, System.Windows.Input.MouseEventArgs e)
    {
        LogEvent("MouseMove");
    }

    static void wfh_PreviewMouseWheel(System.Object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        LogEvent("PreviewMouseWheel");
    }

    static void wfh_MouseWheel(System.Object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
        LogEvent("MouseWheel");
    }

    static void wfh_MouseEnter(System.Object sender, System.Windows.Input.MouseEventArgs e)
    {
        LogEvent("MouseEnter");
    }

    static void wfh_MouseLeave(System.Object sender, System.Windows.Input.MouseEventArgs e)
    {
        LogEvent("MouseLeave");
    }

    static void wfh_GotMouseCapture(System.Object sender, System.Windows.Input.MouseEventArgs e)
    {
        LogEvent("GotMouseCapture");
    }

    static void wfh_LostMouseCapture(System.Object sender, System.Windows.Input.MouseEventArgs e)
    {
        LogEvent("LostMouseCapture");
    }

    static void wfh_QueryCursor(System.Object sender, System.Windows.Input.QueryCursorEventArgs e)
    {
        LogEvent("QueryCursor");
    }

    static void wfh_PreviewStylusDown(System.Object sender, System.Windows.Input.StylusDownEventArgs e)
    {
        LogEvent("PreviewStylusDown");
    }

    static void wfh_StylusDown(System.Object sender, System.Windows.Input.StylusDownEventArgs e)
    {
        LogEvent("StylusDown");
    }

    static void wfh_PreviewStylusUp(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("PreviewStylusUp");
    }

    static void wfh_StylusUp(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("StylusUp");
    }

    static void wfh_PreviewStylusMove(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("PreviewStylusMove");
    }

    static void wfh_StylusMove(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("StylusMove");
    }

    static void wfh_PreviewStylusInAirMove(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("PreviewStylusInAirMove");
    }

    static void wfh_StylusInAirMove(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("StylusInAirMove");
    }

    static void wfh_StylusEnter(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("StylusEnter");
    }

    static void wfh_StylusLeave(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("StylusLeave");
    }

    static void wfh_PreviewStylusInRange(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("PreviewStylusInRange");
    }

    static void wfh_StylusInRange(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("StylusInRange");
    }

    static void wfh_PreviewStylusOutOfRange(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("PreviewStylusOutOfRange");
    }

    static void wfh_StylusOutOfRange(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("StylusOutOfRange");
    }

    static void wfh_PreviewStylusSystemGesture(System.Object sender, System.Windows.Input.StylusSystemGestureEventArgs e)
    {
        LogEvent("PreviewStylusSystemGesture");
    }

    static void wfh_StylusSystemGesture(System.Object sender, System.Windows.Input.StylusSystemGestureEventArgs e)
    {
        LogEvent("StylusSystemGesture");
    }

    static void wfh_GotStylusCapture(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("GotStylusCapture");
    }

    static void wfh_LostStylusCapture(System.Object sender, System.Windows.Input.StylusEventArgs e)
    {
        LogEvent("LostStylusCapture");
    }

    static void wfh_StylusButtonDown(System.Object sender, System.Windows.Input.StylusButtonEventArgs e)
    {
        LogEvent("StylusButtonDown");
    }

    static void wfh_StylusButtonUp(System.Object sender, System.Windows.Input.StylusButtonEventArgs e)
    {
        LogEvent("StylusButtonUp");
    }

    static void wfh_PreviewStylusButtonDown(System.Object sender, System.Windows.Input.StylusButtonEventArgs e)
    {
        LogEvent("PreviewStylusButtonDown");
    }

    static void wfh_PreviewStylusButtonUp(System.Object sender, System.Windows.Input.StylusButtonEventArgs e)
    {
        LogEvent("PreviewStylusButtonUp");
    }

    static void wfh_PreviewKeyDown(System.Object sender, System.Windows.Input.KeyEventArgs e)
    {
        LogEvent("PreviewKeyDown");
    }

    static void wfh_KeyDown(System.Object sender, System.Windows.Input.KeyEventArgs e)
    {
        LogEvent("KeyDown");
    }

    static void wfh_PreviewKeyUp(System.Object sender, System.Windows.Input.KeyEventArgs e)
    {
        LogEvent("PreviewKeyUp");
    }

    static void wfh_KeyUp(System.Object sender, System.Windows.Input.KeyEventArgs e)
    {
        LogEvent("KeyUp");
    }

    static void wfh_PreviewGotKeyboardFocus(System.Object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
        LogEvent("PreviewGotKeyboardFocus");
    }

    static void wfh_GotKeyboardFocus(System.Object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
        LogEvent("GotKeyboardFocus");
    }

    static void wfh_PreviewLostKeyboardFocus(System.Object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
        LogEvent("PreviewLostKeyboardFocus");
    }

    static void wfh_LostKeyboardFocus(System.Object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
        LogEvent("LostKeyboardFocus");
    }

    static void wfh_PreviewTextInput(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        LogEvent("PreviewTextInput");
    }

    static void wfh_TextInput(System.Object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        LogEvent("TextInput");
    }

    static void wfh_PreviewQueryContinueDrag(System.Object sender, System.Windows.QueryContinueDragEventArgs e)
    {
        LogEvent("PreviewQueryContinueDrag");
    }

    static void wfh_QueryContinueDrag(System.Object sender, System.Windows.QueryContinueDragEventArgs e)
    {
        LogEvent("QueryContinueDrag");
    }

    static void wfh_PreviewGiveFeedback(System.Object sender, System.Windows.GiveFeedbackEventArgs e)
    {
        LogEvent("PreviewGiveFeedback");
    }

    static void wfh_GiveFeedback(System.Object sender, System.Windows.GiveFeedbackEventArgs e)
    {
        LogEvent("GiveFeedback");
    }

    static void wfh_PreviewDragEnter(System.Object sender, System.Windows.DragEventArgs e)
    {
        LogEvent("PreviewDragEnter");
    }

    static void wfh_DragEnter(System.Object sender, System.Windows.DragEventArgs e)
    {
        LogEvent("DragEnter");
    }

    static void wfh_PreviewDragOver(System.Object sender, System.Windows.DragEventArgs e)
    {
        LogEvent("PreviewDragOver");
    }

    static void wfh_DragOver(System.Object sender, System.Windows.DragEventArgs e)
    {
        LogEvent("DragOver");
    }

    static void wfh_PreviewDragLeave(System.Object sender, System.Windows.DragEventArgs e)
    {
        LogEvent("PreviewDragLeave");
    }

    static void wfh_DragLeave(System.Object sender, System.Windows.DragEventArgs e)
    {
        LogEvent("DragLeave");
    }

    static void wfh_PreviewDrop(System.Object sender, System.Windows.DragEventArgs e)
    {
        LogEvent("PreviewDrop");
    }

    static void wfh_Drop(System.Object sender, System.Windows.DragEventArgs e)
    {
        LogEvent("Drop");
    }

    static void wfh_IsMouseDirectlyOverChanged(System.Object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        LogEvent("IsMouseDirectlyOverChanged");
    }

    static void wfh_IsKeyboardFocusWithinChanged(System.Object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        LogEvent("IsKeyboardFocusWithinChanged");
    }

    static void wfh_IsMouseCapturedChanged(System.Object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        LogEvent("IsMouseCapturedChanged");
    }

    static void wfh_IsMouseCaptureWithinChanged(System.Object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        LogEvent("IsMouseCaptureWithinChanged");
    }

    static void wfh_IsStylusDirectlyOverChanged(System.Object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        LogEvent("IsStylusDirectlyOverChanged");
    }

    static void wfh_IsStylusCapturedChanged(System.Object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        LogEvent("IsStylusCapturedChanged");
    }

    static void wfh_IsStylusCaptureWithinChanged(System.Object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        LogEvent("IsStylusCaptureWithinChanged");
    }

    static void wfh_IsKeyboardFocusedChanged(System.Object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        LogEvent("IsKeyboardFocusedChanged");
    }

    static void wfh_LayoutUpdated(System.Object sender, System.EventArgs e)
    {
        LogEvent("LayoutUpdated");
    }

    static void wfh_GotFocus(System.Object sender, System.Windows.RoutedEventArgs e)
    {
        LogEvent("GotFocus");
    }

    static void wfh_LostFocus(System.Object sender, System.Windows.RoutedEventArgs e)
    {
        LogEvent("LostFocus");
    }

    static void wfh_IsEnabledChanged(System.Object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        LogEvent("IsEnabledChanged");
    }

    static void wfh_IsHitTestVisibleChanged(System.Object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        LogEvent("IsHitTestVisibleChanged");
    }

    static void wfh_IsVisibleChanged(System.Object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        LogEvent("IsVisibleChanged");
    }

    static void wfh_FocusableChanged(System.Object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        LogEvent("FocusableChanged");
    }

}
