// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Media;

namespace Avalon.Test.CoreUI.CoreInput.Common.Controls
{
    /// <summary>
    /// This class represents an instrumented shape element (InstrShape).
    /// It contains debug-mode instrumentation showing what events are passing throught it.
    /// </summary>
    /// <example>
    /// This example fragment shows how to initialize a window source with an InstrShape as the root element.
    /// <code>
    ///        Window _win = new Window().
    ///        _win.Content = new InstrShape().
    /// </code>
    /// </example>
    public class InstrShape: Shape
    {
        /// <summary>
        /// Construct an instrumented panel.
        /// </summary>
        public InstrShape()
            : base()
        {
            Width = 100;
            Height = 100;
        }

        /// <summary>
        /// Construct an instrumented panel with fill color.
        /// </summary>
        /// <param name="fillBrush">Brush to fill interior of shape with.</param>
        public InstrShape(Brush fillBrush)
            : this()
        {
            this.Fill = fillBrush;
        }

        /// <summary>
        /// This element's color.
        /// </summary>
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        private Color _color = Color.FromRgb(0x00, 0xff, 0xbb);

        /// <summary>
        /// Override to render an instrumented shape.
        /// </summary>
        /// <param name="ctx">Drawing context.</param>
        protected override void OnRender(DrawingContext ctx)
        {
            FormattedText text = new FormattedText("IS",
                new CultureInfo("en-US"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                8,
                this.TextBrush);
            ctx.DrawText(text, new Point(20, 20));
            ctx.DrawGeometry(this.Fill, new Pen(this.TextBrush, 3.0d), this.DefiningGeometry);
        }

        /// <summary>
        /// What brush is used to draw the text?
        /// </summary>
        public Brush TextBrush
        {
            get
            {
                return _textBrush;
            }
            set
            {
                _textBrush = value;
            }
        }
        private Brush _textBrush = Brushes.White;

        /// <summary>
        /// Get the lines that define this shape.
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                // Squat oval
                if (_definingGeometry == null)
                {
                    _definingGeometry = new EllipseGeometry(new Point(Width / 2, Height / 2), Width / 2, Height / 2);
                }
                return _definingGeometry;
            }
        }
        private Geometry _definingGeometry = null;

        #region Input overrides

        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {

            // Log some debugging data
            Debug.WriteLine(" [" + e.RoutedEvent.Name + "]");
            //Point pt = e.GetPosition(null).
            Debug.WriteLine("   Hello from InstrShape: " + e.Key.ToString() + "," + e.KeyStates.ToString());

            // Keep routing this event.
            e.Handled = false;

            base.OnKeyDown(e);
        }


        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            // Log some debugging data
            Debug.WriteLine(" [" + e.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrShape: " + e.Key.ToString() + "," + e.KeyStates.ToString());

            // Keep routing this event.
            e.Handled = false;

            base.OnKeyUp(e);
        }

        /// <summary>
        /// Standard mouse move handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point position = e.GetPosition(this);

            // Log some debugging data
            Debug.WriteLine(" [" + e.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrShape: " + position.X + "," + position.Y);

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Standard focus event handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            // Log some debugging data
            Debug.WriteLine(" [" + e.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrShape: old focus='" + e.OldFocus + "',new focus='" + e.NewFocus + "'");

            // Keep routing this event.
            e.Handled = false;

            base.OnGotKeyboardFocus(e);
        }

        /// <summary>
        /// Standard focus event handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            // Log some debugging data
            Debug.WriteLine(" [" + e.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrShape: old focus='" + e.OldFocus + "',new focus='" + e.NewFocus + "'");

            // Keep routing this event.
            e.Handled = false;

            base.OnLostKeyboardFocus(e);
        }

        /// <summary>
        /// Standard left mouse button down event handler. 
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // Log some debugging data
            Debug.WriteLine(" [" + e.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrShape: " + e.ButtonState.ToString() + "," + e.ChangedButton.ToString() + "," + e.ClickCount);

            // Request focus for ourselves.
            this.Focus();

            // Keep routing this event.
            e.Handled = false;

            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Standard left mouse button up event handler. 
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // Log some debugging data
            Debug.WriteLine(" [" + e.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrShape: " + e.ButtonState.ToString() + "," + e.ChangedButton.ToString() + "," + e.ClickCount);

            // Request focus for ourselves.
            this.Focus();

            // Keep routing this event.
            e.Handled = false;

            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Standard MouseWheel handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            Debug.WriteLine(" [" + e.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrShape: Delta=" + e.Delta.ToString());

            // Keep routing this event.
            e.Handled = false;

            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Standard query cursor event handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnQueryCursor(QueryCursorEventArgs e)
        {
            // Log some debugging data
            Debug.WriteLine(" [" + e.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrShape: Cursor='" + ((e.Cursor != null) ? e.Cursor.ToString() : "") + "'");

            // Keep routing this event.
            e.Handled = false;

            base.OnQueryCursor(e);
        }

        #endregion
    }
}
