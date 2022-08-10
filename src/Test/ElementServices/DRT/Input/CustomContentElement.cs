// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

public class CustomContentElement : FrameworkContentElement
{
    public CustomContentElement(string name)
    {
        _name = name;
        
        AddHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown), false);
        AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), false);
        AddHandler(Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(OnPreviewMouseUp), false);
        AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), false);

        AddHandler(TextCompositionManager.PreviewTextInputStartEvent, new TextCompositionEventHandler(OnPreviewTextInputStart), false);
        AddHandler(TextCompositionManager.TextInputStartEvent, new TextCompositionEventHandler(OnTextInputStart), false);
    }

    protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
    {
        PushMessage("IsKeyboardFocusWithinChanged (" + ((bool)e.OldValue) + "-->" + ((bool)e.NewValue) + ")");
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
        Point position = e.GetPosition(null);
        Point ptDevice = DrtInput.Window.CompositionTarget.TransformToDevice.Transform(position);

        PushMessage("PreviewMouseMove (" + ptDevice.X + "," + ptDevice.Y + ")");
        base.OnPreviewMouseMove(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        Point position = e.GetPosition(null);
        Point ptDevice = DrtInput.Window.CompositionTarget.TransformToDevice.Transform(position);

        PushMessage("MouseMove (" + ptDevice.X + "," + ptDevice.Y + ")");
        base.OnMouseMove(e);
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
        PushMessage("MouseEnter");
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        PushMessage("MouseLeave");
        base.OnMouseLeave(e);
    }

    private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        PushMessage("PreviewMouseDown (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        PushMessage("MouseDown (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
    }

    private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        PushMessage("PreviewMouseUp (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        PushMessage("MouseUp (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
    }
    
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        PushMessage("PreviewMouseLeftButtonDown (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
        base.OnPreviewMouseLeftButtonDown(e);
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        PushMessage("MouseLeftButtonDown (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
        if(this == e.Source)
        {
            CaptureMouse();
            Focus();
        }

        base.OnMouseLeftButtonDown(e);
    }

    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        PushMessage("PreviewMouseLeftButtonUp (" + e.ChangedButton + "=" + e.ButtonState + ")");
        base.OnPreviewMouseLeftButtonUp(e);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        PushMessage("MouseLeftButtonUp (" + e.ChangedButton + "=" + e.ButtonState + ")");
        if(this == e.Source)
        {
            ReleaseMouseCapture();
        }

        base.OnMouseLeftButtonUp(e);
    }

    protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
    {
        PushMessage("PreviewMouseRightButtonDown (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
        base.OnPreviewMouseRightButtonDown(e);
    }

    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
        PushMessage("MouseRightButtonDown (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
        base.OnMouseRightButtonDown(e);
    }

    protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
    {
        PushMessage("PreviewMouseRightButtonUp (" + e.ChangedButton + "=" + e.ButtonState + ")");
        base.OnPreviewMouseRightButtonUp(e);
    }

    protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
    {
        PushMessage("MouseRightButtonUp (" + e.ChangedButton + "=" + e.ButtonState + ")");
        base.OnMouseRightButtonUp(e);
    }

    protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
    {
        PushMessage("PreviewMouseWheel (" + e.Delta + ")");
        base.OnPreviewMouseWheel(e);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        PushMessage("MouseWheel (" + e.Delta + ")");
        base.OnMouseWheel(e);
    }

    protected override void OnGotMouseCapture(MouseEventArgs e)
    {
        PushMessage("GotMouseCapture");
        base.OnGotMouseCapture(e);
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
        PushMessage("LostMouseCapture");
        base.OnLostMouseCapture(e);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        PushMessage("PreviewKeyDown (" + e.Key + "=" + KeyStatesToString(e.KeyStates) + ")");
        base.OnPreviewKeyDown(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        PushMessage("KeyDown (" + e.Key + "=" + KeyStatesToString(e.KeyStates) + ")");
        base.OnKeyDown(e);
    }

    protected override void OnPreviewKeyUp(KeyEventArgs e)
    {
        PushMessage("PreviewKeyUp (" + e.Key + "=" + KeyStatesToString(e.KeyStates) + ")");
        base.OnPreviewKeyUp(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        PushMessage("KeyUp (" + e.Key + "=" + KeyStatesToString(e.KeyStates) + ")");
        base.OnKeyUp(e);
    }

    protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        PushMessage("PreviewGotKeyboardFocus (" + e.OldFocus + "-->" + e.NewFocus + ")");
        base.OnPreviewGotKeyboardFocus(e);
    }

    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        PushMessage("GotKeyboardFocus (" + e.OldFocus + "-->" + e.NewFocus + ")");
        base.OnGotKeyboardFocus(e);
    }

    protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        PushMessage("PreviewLostKeyboardFocus (" + e.OldFocus + "-->" + e.NewFocus + ")");
        base.OnPreviewLostKeyboardFocus(e);
    }

    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        PushMessage("LostKeyboardFocus (" + e.OldFocus + "-->" + e.NewFocus + ")");
        base.OnLostKeyboardFocus(e);
    }

    private void OnPreviewTextInputStart(object sender, TextCompositionEventArgs e)
    {
        PushMessage("PreviewTextInputStart (" + e.Text + ")");
    }

    private void OnTextInputStart(object sender, TextCompositionEventArgs e)
    {
        PushMessage("TextInputStart (" + e.Text + ")");
    }

    protected override void OnPreviewTextInput(TextCompositionEventArgs e)
    {
        PushMessage("PreviewTextInput (" + e.Text + ")");
        base.OnPreviewTextInput(e);
    }

    protected override void OnTextInput(TextCompositionEventArgs e)
    {
        PushMessage("TextInput (" + e.Text + ")");
        base.OnTextInput(e);
    }
    
    private void PushMessage(string msg)
    {
        ((CustomContentHost)Parent).PushChildMessage(_name, msg);
    }

    // The default KeyStates.ToString has some issues:
    // 1) Since KeyStates.Up is 0, it only shows up in the string if no other
    //    bit is set (such as toggled).
    // 2) The toggled bit is included, which would be fine except that for a
    //    DRT the initial state of the keyboard is not predictable.
    private string KeyStatesToString(KeyStates keyStates)
    {
        if((keyStates & KeyStates.Down) == KeyStates.Down)
        {
            return "Down";
        }
        else
        {
            return "Up";
        }
    }
    
    private string _name;
}

