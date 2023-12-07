// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

//
// AutoGen Test for WFBUtil
//
public class WFBUtil
{
    // class vars
    private static string s_events;

    // member functions
    public static void ResetEvents() { s_events = ""; }
    public static string GetEvents() { return s_events; }

    private static void LogEvent(string evt)
    {
        s_events += evt + ':';
    }

    // AddHandlers function
    public static void AddHandlers(System.Windows.Forms.Button wfb)
    {
        ResetEvents();

        // add event handlers
        wfb.DoubleClick += new System.EventHandler(wfb_DoubleClick);
        wfb.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(wfb_MouseDoubleClick);
        wfb.AutoSizeChanged += new System.EventHandler(wfb_AutoSizeChanged);
        wfb.ImeModeChanged += new System.EventHandler(wfb_ImeModeChanged);
        wfb.BackColorChanged += new System.EventHandler(wfb_BackColorChanged);
        wfb.BackgroundImageChanged += new System.EventHandler(wfb_BackgroundImageChanged);
        wfb.BackgroundImageLayoutChanged += new System.EventHandler(wfb_BackgroundImageLayoutChanged);
        wfb.BindingContextChanged += new System.EventHandler(wfb_BindingContextChanged);
        wfb.CausesValidationChanged += new System.EventHandler(wfb_CausesValidationChanged);
        wfb.ClientSizeChanged += new System.EventHandler(wfb_ClientSizeChanged);
        wfb.ContextMenuStripChanged += new System.EventHandler(wfb_ContextMenuStripChanged);
        wfb.CursorChanged += new System.EventHandler(wfb_CursorChanged);
        wfb.DockChanged += new System.EventHandler(wfb_DockChanged);
        wfb.EnabledChanged += new System.EventHandler(wfb_EnabledChanged);
        wfb.FontChanged += new System.EventHandler(wfb_FontChanged);
        wfb.ForeColorChanged += new System.EventHandler(wfb_ForeColorChanged);
        wfb.LocationChanged += new System.EventHandler(wfb_LocationChanged);
        wfb.MarginChanged += new System.EventHandler(wfb_MarginChanged);
        wfb.RegionChanged += new System.EventHandler(wfb_RegionChanged);
        wfb.RightToLeftChanged += new System.EventHandler(wfb_RightToLeftChanged);
        wfb.SizeChanged += new System.EventHandler(wfb_SizeChanged);
        wfb.TabIndexChanged += new System.EventHandler(wfb_TabIndexChanged);
        wfb.TabStopChanged += new System.EventHandler(wfb_TabStopChanged);
        wfb.TextChanged += new System.EventHandler(wfb_TextChanged);
        wfb.VisibleChanged += new System.EventHandler(wfb_VisibleChanged);
        wfb.Click += new System.EventHandler(wfb_Click);
        wfb.ControlAdded += new System.Windows.Forms.ControlEventHandler(wfb_ControlAdded);
        wfb.ControlRemoved += new System.Windows.Forms.ControlEventHandler(wfb_ControlRemoved);
        wfb.DragDrop += new System.Windows.Forms.DragEventHandler(wfb_DragDrop);
        wfb.DragEnter += new System.Windows.Forms.DragEventHandler(wfb_DragEnter);
        wfb.DragOver += new System.Windows.Forms.DragEventHandler(wfb_DragOver);
        wfb.DragLeave += new System.EventHandler(wfb_DragLeave);
        wfb.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(wfb_GiveFeedback);
        wfb.HandleCreated += new System.EventHandler(wfb_HandleCreated);
        wfb.HandleDestroyed += new System.EventHandler(wfb_HandleDestroyed);
        wfb.HelpRequested += new System.Windows.Forms.HelpEventHandler(wfb_HelpRequested);
        wfb.Invalidated += new System.Windows.Forms.InvalidateEventHandler(wfb_Invalidated);
        wfb.PaddingChanged += new System.EventHandler(wfb_PaddingChanged);
        wfb.Paint += new System.Windows.Forms.PaintEventHandler(wfb_Paint);
        wfb.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(wfb_QueryContinueDrag);
        wfb.QueryAccessibilityHelp += new System.Windows.Forms.QueryAccessibilityHelpEventHandler(wfb_QueryAccessibilityHelp);
        wfb.Enter += new System.EventHandler(wfb_Enter);
        wfb.GotFocus += new System.EventHandler(wfb_GotFocus);
        wfb.KeyDown += new System.Windows.Forms.KeyEventHandler(wfb_KeyDown);
        wfb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(wfb_KeyPress);
        wfb.KeyUp += new System.Windows.Forms.KeyEventHandler(wfb_KeyUp);
        wfb.Layout += new System.Windows.Forms.LayoutEventHandler(wfb_Layout);
        wfb.Leave += new System.EventHandler(wfb_Leave);
        wfb.LostFocus += new System.EventHandler(wfb_LostFocus);
        wfb.MouseClick += new System.Windows.Forms.MouseEventHandler(wfb_MouseClick);
        wfb.MouseCaptureChanged += new System.EventHandler(wfb_MouseCaptureChanged);
        wfb.MouseDown += new System.Windows.Forms.MouseEventHandler(wfb_MouseDown);
        wfb.MouseEnter += new System.EventHandler(wfb_MouseEnter);
        wfb.MouseLeave += new System.EventHandler(wfb_MouseLeave);
        wfb.MouseHover += new System.EventHandler(wfb_MouseHover);
        wfb.MouseMove += new System.Windows.Forms.MouseEventHandler(wfb_MouseMove);
        wfb.MouseUp += new System.Windows.Forms.MouseEventHandler(wfb_MouseUp);
        wfb.MouseWheel += new System.Windows.Forms.MouseEventHandler(wfb_MouseWheel);
        wfb.Move += new System.EventHandler(wfb_Move);
        wfb.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(wfb_PreviewKeyDown);
        wfb.Resize += new System.EventHandler(wfb_Resize);
        wfb.ChangeUICues += new System.Windows.Forms.UICuesEventHandler(wfb_ChangeUICues);
        wfb.StyleChanged += new System.EventHandler(wfb_StyleChanged);
        wfb.SystemColorsChanged += new System.EventHandler(wfb_SystemColorsChanged);
        wfb.Validating += new System.ComponentModel.CancelEventHandler(wfb_Validating);
        wfb.Validated += new System.EventHandler(wfb_Validated);
        wfb.ParentChanged += new System.EventHandler(wfb_ParentChanged);
        wfb.Disposed += new System.EventHandler(wfb_Disposed);
    }

    // event handlers
    static void wfb_DoubleClick(System.Object sender, System.EventArgs e)
    {
        LogEvent("DoubleClick");
    }

    static void wfb_MouseDoubleClick(System.Object sender, System.Windows.Forms.MouseEventArgs e)
    {
        LogEvent("MouseDoubleClick");
    }

    static void wfb_AutoSizeChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("AutoSizeChanged");
    }

    static void wfb_ImeModeChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("ImeModeChanged");
    }

    static void wfb_BackColorChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("BackColorChanged");
    }

    static void wfb_BackgroundImageChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("BackgroundImageChanged");
    }

    static void wfb_BackgroundImageLayoutChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("BackgroundImageLayoutChanged");
    }

    static void wfb_BindingContextChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("BindingContextChanged");
    }

    static void wfb_CausesValidationChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("CausesValidationChanged");
    }

    static void wfb_ClientSizeChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("ClientSizeChanged");
    }

    static void wfb_ContextMenuStripChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("ContextMenuStripChanged");
    }

    static void wfb_CursorChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("CursorChanged");
    }

    static void wfb_DockChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("DockChanged");
    }

    static void wfb_EnabledChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("EnabledChanged");
    }

    static void wfb_FontChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("FontChanged");
    }

    static void wfb_ForeColorChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("ForeColorChanged");
    }

    static void wfb_LocationChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("LocationChanged");
    }

    static void wfb_MarginChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("MarginChanged");
    }

    static void wfb_RegionChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("RegionChanged");
    }

    static void wfb_RightToLeftChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("RightToLeftChanged");
    }

    static void wfb_SizeChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("SizeChanged");
    }

    static void wfb_TabIndexChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("TabIndexChanged");
    }

    static void wfb_TabStopChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("TabStopChanged");
    }

    static void wfb_TextChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("TextChanged");
    }

    static void wfb_VisibleChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("VisibleChanged");
    }

    static void wfb_Click(System.Object sender, System.EventArgs e)
    {
        LogEvent("Click");
    }

    static void wfb_ControlAdded(System.Object sender, System.Windows.Forms.ControlEventArgs e)
    {
        LogEvent("ControlAdded");
    }

    static void wfb_ControlRemoved(System.Object sender, System.Windows.Forms.ControlEventArgs e)
    {
        LogEvent("ControlRemoved");
    }

    static void wfb_DragDrop(System.Object sender, System.Windows.Forms.DragEventArgs e)
    {
        LogEvent("DragDrop");
    }

    static void wfb_DragEnter(System.Object sender, System.Windows.Forms.DragEventArgs e)
    {
        LogEvent("DragEnter");
    }

    static void wfb_DragOver(System.Object sender, System.Windows.Forms.DragEventArgs e)
    {
        LogEvent("DragOver");
    }

    static void wfb_DragLeave(System.Object sender, System.EventArgs e)
    {
        LogEvent("DragLeave");
    }

    static void wfb_GiveFeedback(System.Object sender, System.Windows.Forms.GiveFeedbackEventArgs e)
    {
        LogEvent("GiveFeedback");
    }

    static void wfb_HandleCreated(System.Object sender, System.EventArgs e)
    {
        LogEvent("HandleCreated");
    }

    static void wfb_HandleDestroyed(System.Object sender, System.EventArgs e)
    {
        LogEvent("HandleDestroyed");
    }

    static void wfb_HelpRequested(System.Object sender, System.Windows.Forms.HelpEventArgs hlpevent)
    {
        LogEvent("HelpRequested");
    }

    static void wfb_Invalidated(System.Object sender, System.Windows.Forms.InvalidateEventArgs e)
    {
        LogEvent("Invalidated");
    }

    static void wfb_PaddingChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("PaddingChanged");
    }

    static void wfb_Paint(System.Object sender, System.Windows.Forms.PaintEventArgs e)
    {
        LogEvent("Paint");
    }

    static void wfb_QueryContinueDrag(System.Object sender, System.Windows.Forms.QueryContinueDragEventArgs e)
    {
        LogEvent("QueryContinueDrag");
    }

    static void wfb_QueryAccessibilityHelp(System.Object sender, System.Windows.Forms.QueryAccessibilityHelpEventArgs e)
    {
        LogEvent("QueryAccessibilityHelp");
    }

    static void wfb_Enter(System.Object sender, System.EventArgs e)
    {
        LogEvent("Enter");
    }

    static void wfb_GotFocus(System.Object sender, System.EventArgs e)
    {
        LogEvent("GotFocus");
    }

    static void wfb_KeyDown(System.Object sender, System.Windows.Forms.KeyEventArgs e)
    {
        LogEvent("KeyDown");
    }

    static void wfb_KeyPress(System.Object sender, System.Windows.Forms.KeyPressEventArgs e)
    {
        LogEvent("KeyPress");
    }

    static void wfb_KeyUp(System.Object sender, System.Windows.Forms.KeyEventArgs e)
    {
        LogEvent("KeyUp");
    }

    static void wfb_Layout(System.Object sender, System.Windows.Forms.LayoutEventArgs e)
    {
        LogEvent("Layout");
    }

    static void wfb_Leave(System.Object sender, System.EventArgs e)
    {
        LogEvent("Leave");
    }

    static void wfb_LostFocus(System.Object sender, System.EventArgs e)
    {
        LogEvent("LostFocus");
    }

    static void wfb_MouseClick(System.Object sender, System.Windows.Forms.MouseEventArgs e)
    {
        LogEvent("MouseClick");
    }

    static void wfb_MouseCaptureChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("MouseCaptureChanged");
    }

    static void wfb_MouseDown(System.Object sender, System.Windows.Forms.MouseEventArgs e)
    {
        LogEvent("MouseDown");
    }

    static void wfb_MouseEnter(System.Object sender, System.EventArgs e)
    {
        LogEvent("MouseEnter");
    }

    static void wfb_MouseLeave(System.Object sender, System.EventArgs e)
    {
        LogEvent("MouseLeave");
    }

    static void wfb_MouseHover(System.Object sender, System.EventArgs e)
    {
        LogEvent("MouseHover");
    }

    static void wfb_MouseMove(System.Object sender, System.Windows.Forms.MouseEventArgs e)
    {
        LogEvent("MouseMove");
    }

    static void wfb_MouseUp(System.Object sender, System.Windows.Forms.MouseEventArgs e)
    {
        LogEvent("MouseUp");
    }

    static void wfb_MouseWheel(System.Object sender, System.Windows.Forms.MouseEventArgs e)
    {
        LogEvent("MouseWheel");
    }

    static void wfb_Move(System.Object sender, System.EventArgs e)
    {
        LogEvent("Move");
    }

    static void wfb_PreviewKeyDown(System.Object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
    {
        LogEvent("PreviewKeyDown");
    }

    static void wfb_Resize(System.Object sender, System.EventArgs e)
    {
        LogEvent("Resize");
    }

    static void wfb_ChangeUICues(System.Object sender, System.Windows.Forms.UICuesEventArgs e)
    {
        LogEvent("ChangeUICues");
    }

    static void wfb_StyleChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("StyleChanged");
    }

    static void wfb_SystemColorsChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("SystemColorsChanged");
    }

    static void wfb_Validating(System.Object sender, System.ComponentModel.CancelEventArgs e)
    {
        LogEvent("Validating");
    }

    static void wfb_Validated(System.Object sender, System.EventArgs e)
    {
        LogEvent("Validated");
    }

    static void wfb_ParentChanged(System.Object sender, System.EventArgs e)
    {
        LogEvent("ParentChanged");
    }

    static void wfb_Disposed(System.Object sender, System.EventArgs e)
    {
        LogEvent("Disposed");
    }

}
