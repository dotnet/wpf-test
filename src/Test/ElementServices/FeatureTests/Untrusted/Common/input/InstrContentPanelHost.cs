// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using Microsoft.Test.Input;
using System.ComponentModel;

namespace Avalon.Test.CoreUI.CoreInput.Common.Controls
{
    /// <summary>
    /// This class represents an instrumented host for instrumented content panels (InstrContentPanel, InstrFrameworkContentPanel).
    /// An InstrContentPanelHost stores any number of content panels as children.
    /// It changes its background color on a mouse over. 
    /// Any input event directed to it is passed through to its child content element.
    /// It also contains debug-mode instrumentation showing what events are passing through it.
    /// </summary>
    /// <remarks>
    /// We assume there is at least one child element of type InstrContentPanel or InstrFrameworkContentPanel.
    /// </remarks>
    [ContentProperty("ModelChildren")]
    public class InstrContentPanelHost : FrameworkElement, IContentHost, ILogicalParent
    {

        /// <summary>
        /// 



        ReadOnlyCollection<Rect> IContentHost.GetRectangles(ContentElement e){return null;}

        /// <summary>
        /// Returns elements hosted by the content host as an enumerator class
        /// </summary>
        IEnumerator<IInputElement> IContentHost.HostedElements 
        { 
            get
            {
                return null;
            }
        }
        
        /// <summary>
        /// Called when a UIElement-derived class which is hosted by a IContentHost changes its DesiredSize
        /// NOTE: This method already exists for this class and is not specially implemented for IContentHost.
        /// If this method is called through IContentHost for this class it will fire an assert
        /// </summary>
        /// <param name="child">
        /// Child element whose DesiredSize has changed
        /// </param> 
        void IContentHost.OnChildDesiredSizeChanged(UIElement child)
        {
            return;
        }
        
        /// <summary>
        /// Construct a InstrContentPanelHost.
        /// </summary>
        public InstrContentPanelHost()
        {
            // Pull named identifier from framework Name property.
            _name = this.Name;

            // Initialize collection to nothing.
            _modelChildren = new LogicalChildCollection(this, true);

            // Initialize event handlers.
            AddHandler(InputManagerHelper.PreviewInputReportEvent, new RoutedEventHandler(OnPreviewInputReport), false);
            AddHandler(InputManagerHelper.InputReportEvent, new RoutedEventHandler(OnInputReport), false);
            AddHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown), false);
            AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), false);
            AddHandler(Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(OnPreviewMouseUp), false);
            AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), false);
        }


        /// <summary>
        /// Construct a InstrContentPanelHost with a named identifier and add a starter child to it.
        /// </summary>
        /// <param name="name"></param>
        public InstrContentPanelHost(string name): this()
        {
            _name = name;
            _leaf = new InstrContentPanel(name + "Leaf", name + "CE", this);
            _modelChildren.Add(_leaf);
        }
        /// <summary>
        /// This method is to be called to update the logical tree. 
        /// </summary>
        /// <param name="child"></param>
        public void CallAddLogicalChild(object child)
        {
            AddLogicalChild(child);
        }
           


        /// <summary>
        /// Caption belonging to our element.
        /// </summary>
        /// <value>String of text.</value>
        /// <remarks>This string is taken from the underlying content element.</remarks>
        public string Caption
        {
            get
            {
                string caption = "";

                if (_modelChildren.Count > 0)
                {
                    IHasText icp = _modelChildren[0] as IHasText;

                    if (icp != null)
                    {
                        caption = icp.Text;
                    }
                }

                return caption;
            }
        }


        /// <summary>
        /// Visual background for our element.
        /// </summary>
        /// <value>A brush object.</value>
        public Brush Background
        {
            get { return _background; }
            set
            {
                if (ChangeBackgroundOnMouseEnter) {
                    _background = value;
                    InvalidateVisual();
                }
            }
        }
        private Brush _background = s_defaultBrush;


        /// <summary>
        /// Should we change background when mouse enters or leaves our element?
        /// </summary>
        /// <value>Boolean value.</value>
        public bool ChangeBackgroundOnMouseEnter
        {
            get { return _bChangeBackgroundOnMouseEnter; }
            set
            {
                _bChangeBackgroundOnMouseEnter = value; 
            }
        }
        private bool _bChangeBackgroundOnMouseEnter = false;

        /// <summary>
        /// Add child to this host.
        /// </summary>
        /// <param name="o">Content element.</param>
        public void AddChild(object o)
        {
            ContentElement icp = o as ContentElement;

            if (icp != null)
            {
                _modelChildren.Add(icp);
            }
        }

        /// <summary>
        /// Hit test for the element from a given point.
        /// </summary>
        /// <param name="p">Point to hit test against.</param>
        /// <returns>An element inside this host that accepts input.</returns>
        IInputElement IContentHost.InputHitTest(Point p)
        {
            // Always return content element if it exists
            if (_modelChildren.Count > 0)
            {
                return (IInputElement)_modelChildren[0];
            }

            if (_leaf != null)
            {
                return _leaf;
            }
            else
            {
                // No valid content element found ... just use the host itself.
                return this;
            }
        }


        /// <summary>
        /// Property to hold children.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LogicalChildCollection ModelChildren
        {
            get
            {
                return _modelChildren;
            }
        }
        /// <summary>
        /// Decide on how to measure our element.
        /// </summary>
        /// <param name="constraint">Constraint given to us.</param>
        /// <returns>Constraint to return.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size contentSize = new Size(200, 200);

            if (!double.IsPositiveInfinity(constraint.Width))
                contentSize.Width = constraint.Width;
            if (!double.IsPositiveInfinity(constraint.Height))
                contentSize.Height = constraint.Height;

            return contentSize;
        }

        internal void AddContentPanelChild(object o)
        {
            AddChild(o);
        }


        /// <summary>
        /// Decide on how to render our element.
        /// </summary>
        /// <param name="ctx">Drawing context.</param>
        protected override void OnRender(DrawingContext ctx)
        {
            // Draw text based on contents of underlying element.
            FormattedText text = new FormattedText(this.Caption, Language.GetSpecificCulture(), FlowDirection.LeftToRight, new Typeface("Arial"), 8, Brushes.White);

            ctx.DrawRectangle(this.Background, null, new Rect(new Point(), RenderSize));
            ctx.DrawText(text, new Point(20, 20));
        }


        /// <summary>
        /// Standard MouseEnter handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (e != null)
            {
                PushMessage("MouseEnter");
            }

            Background = s_mouseOverBrush;
            base.OnMouseEnter(e);
        }
        
        /// <summary>
        /// Standard MouseLeave handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (e != null)
            {
                PushMessage("OnMouseLeave");
            }

            Background = s_defaultBrush;
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Standard IsKeyboardFocusWithinChanged handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            PushMessage("IsKeyboardFocusWithinChanged (" + ((bool)e.OldValue) + "-->" + ((bool)e.NewValue) + ")");
        }


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
            Point ptDevice = position;

            PushMessage("PreviewMouseMove (" + ptDevice.X + "," + ptDevice.Y + ")");
            base.OnPreviewMouseMove(e);
        }


        /// <summary>
        /// Standard MouseMove handler.
        /// </summary>
        /// <param name="e">Event-specific arguments.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point position = e.GetPosition(null);
            Point ptDevice = position;

            PushMessage("MouseMove (" + ptDevice.X + "," + ptDevice.Y + ")");
            base.OnMouseMove(e);
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
            if (this == e.Source)
            {
                CaptureMouse();
                Focus();
            }

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
            if (this == e.Source)
            {
                ReleaseMouseCapture();
            }

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
            Debug.WriteLine("Hello from InstrContentPanelHost: " + _name + " - " + msg);
        }

        private static Brush s_defaultBrush = Brushes.Gray;

        private static Brush s_mouseOverBrush = Brushes.DarkRed;

        private string _name;

        private InstrContentPanel _leaf;
        private LogicalChildCollection _modelChildren;
    }
}
