// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System;

public class DrtCommandingElement : FrameworkElement
{
    static DrtCommandingElement()
    {
        if (!_inputFilerStarted)
        {
            InputManager.Current.PostProcessInput += new ProcessInputEventHandler(PostProcessDRTInput);
            _inputFilerStarted = true;
        }

        CommandBinding testCopyCommandBinding = new CommandBinding(ApplicationCommands.Copy);
        testCopyCommandBinding.Executed += new ExecutedRoutedEventHandler(OnCommandExecute);
        testCopyCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnCommandQueryStatus);
        CommandManager.RegisterClassCommandBinding(typeof(DrtCommandingElement), testCopyCommandBinding);

        CommandBinding testCutCommandBinding = new CommandBinding(ApplicationCommands.Cut);
        testCutCommandBinding.Executed += new ExecutedRoutedEventHandler(OnCommandExecute);
        testCutCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnCommandQueryStatus);
        CommandManager.RegisterClassCommandBinding(typeof(DrtCommandingElement), testCutCommandBinding);

        CommandBinding testTogglePlayPauseCommandBinding = new CommandBinding(MediaCommands.TogglePlayPause);
        testTogglePlayPauseCommandBinding.Executed += new ExecutedRoutedEventHandler(OnCommandExecute);
        testTogglePlayPauseCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnCommandQueryStatus);
        CommandManager.RegisterClassCommandBinding(typeof(DrtCommandingElement), testTogglePlayPauseCommandBinding);


        CommandManager.RegisterClassInputBinding(typeof(DrtCommandingElement), 
                                                 new KeyBinding(ContextMenuCommand, 
                                                                new KeyGesture(Key.K, ModifierKeys.Control)));
    }

    public DrtCommandingElement(string name)
    {
        // FrameworkElement has a default Focusable = false
        Focusable = true;

        _name = name;
        
        AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), false);
        AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), false);

        // Poke our background color property to start out our background color.
        Background = IsMouseOver ? Brushes.Red : Brushes.LightGray;

        EnsureCommandBindingCollection();
    }

#region Commands
    public static RoutedCommand ContextMenuCommand = new RoutedCommand("ContextMenu", typeof(DrtCommandingElement));

    public Brush Background { get { return _background; } set { _background = value; InvalidateVisual(); } }

    protected override void OnRender(DrawingContext ctx)
    {
        ctx.DrawRectangle(_background, null, new Rect(new Point(), RenderSize));

        Typeface typeface = new Typeface("MingLiU");
        Brush pen = Brushes.Black;
        string s = null;
        double y = 0;
        double cy = RenderSize.Height / MAX_MESSAGES;

        for (int i = 0; i < MAX_MESSAGES; i++)
        {
            s = _messages[i];
            if (s != null)
            {
                FormattedText text = new FormattedText(s, Language.GetSpecificCulture(), FlowDirection.LeftToRight, typeface, cy, pen);

                ctx.DrawText(text, new Point(5, y));
                y += cy;
            }
        }
    }

    private static void PostProcessDRTInput(object sender, ProcessInputEventArgs e)
    {
        //    Non-Text Input
        //    MouseDown/MouseUp
        if (!e.StagingItem.Input.Handled)
        {
            DrtCommandingElement drtElement = e.StagingItem.Input.Source as DrtCommandingElement;
            if (drtElement != null)
            {
                if (e.StagingItem.Input.RoutedEvent == Keyboard.KeyUpEvent)
                {
                    drtElement.LogKeyEvent(((KeyEventArgs)e.StagingItem.Input));
                }
                else if (e.StagingItem.Input.RoutedEvent == Keyboard.KeyDownEvent)
                {
                    drtElement.LogKeyEvent(((KeyEventArgs)e.StagingItem.Input));
                }
                else if (e.StagingItem.Input.RoutedEvent == Mouse.MouseDownEvent)
                {
                    drtElement.LogMouseEvent(((MouseButtonEventArgs)e.StagingItem.Input));
                }
                else if (e.StagingItem.Input.RoutedEvent == Mouse.MouseUpEvent)
                {
                    drtElement.LogMouseEvent(((MouseButtonEventArgs)e.StagingItem.Input));
                }
            }
        }
    }

    private void LogMouseEvent (MouseButtonEventArgs e)
    {
        string eventName = (e.RoutedEvent == Mouse.MouseDownEvent ? "MouseDown" : "MouseUp");
        PushMessage(eventName + " (" + e.ChangedButton + "=" + e.ButtonState + ")");
    }

    private void LogKeyEvent (KeyEventArgs e)
    {
        string eventName = (e.RoutedEvent == Keyboard.KeyDownEvent ? "KeyDown" : "KeyUp");
        string keyState = (e.KeyStates & KeyStates.Down) == KeyStates.Down ? "Down" : "Up";
        PushMessage(eventName + " (" + ((e.Key == Key.System) ? e.SystemKey : e.Key) + "=" + keyState + ")");
    }

    private static void OnCommandExecute(object target, ExecutedRoutedEventArgs args)
    {
        DrtCommandingElement drtElement = target as DrtCommandingElement;
        drtElement.PushMessage("Execute Command (" + ((RoutedCommand)args.Command).Name + ")");
    }

    private static void OnCommandQueryStatus(object target, CanExecuteRoutedEventArgs args)
    {
        DrtCommandingElement drtElement = target as DrtCommandingElement;
        drtElement.PushMessage("QueryEnabled Command (" + ((RoutedCommand)args.Command).Name + ")");
        args.CanExecute = true;
    }

    private void EnsureCommandBindingCollection()
    {
        ContextMenuCommand.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Alt));
        
        CommandBinding contextMenuCommandBinding = new CommandBinding(ContextMenuCommand);
        contextMenuCommandBinding.Executed += new ExecutedRoutedEventHandler(OnCommandExecute);
        contextMenuCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnCommandQueryStatus);
        this.CommandBindings.Add(contextMenuCommandBinding);

        this.InputBindings.Add(new MouseBinding(ApplicationCommands.Copy, new MouseGesture(MouseAction.RightClick)));
        this.InputBindings.Add(new KeyBinding(ApplicationCommands.Copy, new KeyGesture(Key.Delete, ModifierKeys.Shift)));
    }

    #endregion Commands

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (this == e.Source && e.ChangedButton == MouseButton.Left)		
	{
           CaptureMouse();
           Focus();
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (this == e.Source && e.ChangedButton == MouseButton.Left)
        {
            ReleaseMouseCapture();
        }
    }

    protected override void OnTextInput(TextCompositionEventArgs e)
    {
        PushMessage("TextInput (" + e.Text + ")");
    }

    private void PushMessage(string msg)
    {
        for (int i = 0; i < (MAX_MESSAGES - 1); i++)
        {
            _messages[i] = _messages[i + 1];
        }

        _messages[MAX_MESSAGES - 1] = msg;
        InvalidateVisual();
        DrtCommanding.VerifyInput(_name + ": " + msg);
    }

    private static bool _inputFilerStarted = false;
    private Brush _background;
    private const int MAX_MESSAGES = 20;
    private string[] _messages = new string[MAX_MESSAGES];
    private string _name;
}
