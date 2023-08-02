// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using Microsoft.Test.Input;
using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.CoreInput.Common.Controls
{
    /// <summary>
    /// This class represents an instrumented content element (InstrCoreContentPanel).
    /// An InstrCoreContentPanel is a core content element that contains nothing but text.
    /// It also contains debug-mode instrumentation showing what events are passing through it.
    /// </summary>
    [ContentProperty("Text")]
    public class InstrCoreContentPanel : ContentElement, IHasText
    {
        /// <summary>
        /// Construct a content panel.
        /// </summary>
        public InstrCoreContentPanel()
        {
            // Add instrumentation and fun stuff
            AddHandler(InputManagerHelper.PreviewInputReportEvent, new RoutedEventHandler(OnPreviewInputReport), false);
            AddHandler(InputManagerHelper.InputReportEvent, new RoutedEventHandler(OnInputReport), false);
            AddHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown), false);
            AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), false);
            AddHandler(Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(OnPreviewMouseUp), false);
            AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), false);
        }


        /// <summary>
        /// Construct a content panel with a specified name and text, and attach it to a host.
        /// </summary>
        /// <param name="text">Text contents for this element.</param>
        public InstrCoreContentPanel(string text)
            : this()
        {
            _text = text;
        }

        /// <summary>
        /// Text belonging to this element.
        /// </summary>
        /// <value>A string of text.</value>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }


        /// <summary>
        /// Textual contents of this element.
        /// </summary>
        private string _text = "";

        #region Input overrides
        private void OnPreviewInputReport(object sender, RoutedEventArgs e)
        {
            InputEventArgs ie = e as InputEventArgs;
            if (ie != null)
            {
                InputReportEventArgsWrapper irArgs = new InputReportEventArgsWrapper(ie);
                PushMessage("PreviewInputReport (" + irArgs.Report.Type + ") [" + e.RoutedEvent.Name + "]");
            }
        }


        private void OnInputReport(object sender, RoutedEventArgs e)
        {
            InputEventArgs ie = e as InputEventArgs;
            if (ie != null)
            {
                InputReportEventArgsWrapper irArgs = new InputReportEventArgsWrapper(ie);
                PushMessage("InputReport (" + irArgs.Report.Type + ") [" + e.RoutedEvent.Name + "]");
            }
        }


        /// <summary>
        /// Standard PreviewMouseMove handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            Point position = e.GetPosition(null);

            PushMessage("PreviewMouseMove (" + position.X + "," + position.Y + ")");
            base.OnPreviewMouseMove(e);
        }


        /// <summary>
        /// Standard MouseMove handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point position = e.GetPosition(null);

            PushMessage("MouseMove (" + position.X + "," + position.Y + ")");
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Standard MouseEnter handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            PushMessage("MouseEnter");
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Standard MouseLeave handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            PushMessage("MouseLeave");
            base.OnMouseLeave(e);
        }


        /// <summary>
        /// Standard PreviewMouseDown handler.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PushMessage("PreviewMouseDown (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
        }


        /// <summary>
        /// Standard MouseDown handler.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            PushMessage("MouseDown (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
        }


        /// <summary>
        /// Standard PreviewMouseUp handler.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            PushMessage("PreviewMouseUp (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
        }


        /// <summary>
        /// Standard MouseUp handler.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            PushMessage("MouseUp (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
        }


        /// <summary>
        /// Standard PreviewMouseLeftButtonDown handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            PushMessage("PreviewMouseLeftButtonDown (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
            base.OnPreviewMouseLeftButtonDown(e);
        }


        /// <summary>
        /// Standard MouseLeftButtonDown handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            PushMessage("MouseLeftButtonDown (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
            base.OnMouseLeftButtonDown(e);
        }


        /// <summary>
        /// Standard PreviewMouseLeftButtonUp handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            PushMessage("PreviewMouseLeftButtonUp (" + e.ChangedButton + "=" + e.ButtonState + ")");
            base.OnPreviewMouseLeftButtonUp(e);
        }


        /// <summary>
        /// Standard MouseLeftButtonUp handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            PushMessage("MouseLeftButtonUp (" + e.ChangedButton + "=" + e.ButtonState + ")");
            base.OnMouseLeftButtonUp(e);
        }


        /// <summary>
        /// Standard PreviewMouseRightButtonDown handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            PushMessage("PreviewMouseRightButtonDown (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
            base.OnPreviewMouseRightButtonDown(e);
        }


        /// <summary>
        /// Standard MouseRightButtonDown handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            PushMessage("MouseRightButtonDown (" + e.ChangedButton + "=" + e.ButtonState + "," + "ClickCount=" + e.ClickCount + ")");
            base.OnMouseRightButtonDown(e);
        }


        /// <summary>
        /// Standard PreviewMouseRightButtonUp handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            PushMessage("PreviewMouseRightButtonUp (" + e.ChangedButton + "=" + e.ButtonState + ")");
            base.OnPreviewMouseRightButtonUp(e);
        }


        /// <summary>
        /// Standard MouseRightButtonUp handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            PushMessage("MouseRightButtonUp (" + e.ChangedButton + "=" + e.ButtonState + ")");
            base.OnMouseRightButtonUp(e);
        }


        /// <summary>
        /// Standard PreviewMouseWheel handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            PushMessage("PreviewMouseWheel (" + e.Delta + ")");
            base.OnPreviewMouseWheel(e);
        }


        /// <summary>
        /// Standard MouseWheel handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            PushMessage("MouseWheel (" + e.Delta + ")");
            base.OnMouseWheel(e);
        }


        /// <summary>
        /// Standard GotMouseCapture handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnGotMouseCapture(MouseEventArgs e)
        {
            PushMessage("GotMouseCapture");
            base.OnGotMouseCapture(e);
        }


        /// <summary>
        /// Standard LostMouseCapture handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            PushMessage("LostMouseCapture");
            base.OnLostMouseCapture(e);
        }


        /// <summary>
        /// Standard PreviewKeyDown handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            PushMessage("PreviewKeyDown (" + e.Key + "=" + e.KeyStates + ")");
            base.OnPreviewKeyDown(e);
        }


        /// <summary>
        /// Standard KeyDown handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            PushMessage("KeyDown (" + e.Key + "=" + e.KeyStates + ")");
            base.OnKeyDown(e);
        }


        /// <summary>
        /// Standard PreviewKeyUp handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            PushMessage("PreviewKeyUp (" + e.Key + "=" + e.KeyStates + ")");
            base.OnPreviewKeyUp(e);
        }


        /// <summary>
        /// Standard KeyUp handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            PushMessage("KeyUp (" + e.Key + "=" + e.KeyStates + ")");
            base.OnKeyUp(e);
        }


        /// <summary>
        /// Standard PreviewGotKeyboardFocus handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            PushMessage("PreviewGotKeyboardFocus (" + e.OldFocus + "-->" + e.NewFocus + ")");
            base.OnPreviewGotKeyboardFocus(e);
        }


        /// <summary>
        /// Standard GotKeyboardFocus handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            PushMessage("GotKeyboardFocus (" + e.OldFocus + "-->" + e.NewFocus + ")");
            base.OnGotKeyboardFocus(e);
        }


        /// <summary>
        /// Standard PreviewLostKeyboardFocus handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            PushMessage("PreviewLostKeyboardFocus (" + e.OldFocus + "-->" + e.NewFocus + ")");
            base.OnPreviewLostKeyboardFocus(e);
        }


        /// <summary>
        /// Standard LostKeyboardFocus handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            PushMessage("LostKeyboardFocus (" + e.OldFocus + "-->" + e.NewFocus + ")");
            base.OnLostKeyboardFocus(e);
        }


        /// <summary>
        /// Standard PreviewTextInput handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            PushMessage("PreviewTextInput (" + e.Text + ")");
            base.OnPreviewTextInput(e);
        }


        /// <summary>
        /// Standard TextInput handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            PushMessage("TextInput (" + e.Text + ")");
            base.OnTextInput(e);
        }

        /// <summary>
        /// Standard QueryCursor handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnQueryCursor(QueryCursorEventArgs args)
        {
            PushMessage("QueryCursor (" + ((args.Cursor != null) ? args.Cursor.ToString() : "") + ")");
            base.OnQueryCursor(args);
        }

        private void PushMessage(string msg)
        {
            Debug.WriteLine("Hello from InstrCoreContentPanel: " + msg);
        }
        #endregion
    }
}
