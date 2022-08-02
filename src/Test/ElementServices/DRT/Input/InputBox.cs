// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Input;

public class InputBox : FrameworkElement //UIElement
{
    public InputBox(string name, bool leaf)
    {
        _children = new VisualCollection(this);
    
        // FrameworkElement has a default Focusable = false
        Focusable = true;

        _name = name;
        
        // If we are not the leaf box, make one.
        if(!leaf)
        {
            _leaf = new InputBox(name + "Leaf", true);
            _children.Add(_leaf);
            AddLogicalChild(_leaf);
        }

        AddHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown), false);
        AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), false);
        AddHandler(Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(OnPreviewMouseUp), false);
        AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), false);

        AddHandler(TextCompositionManager.PreviewTextInputStartEvent, new TextCompositionEventHandler(OnPreviewTextInputStart), false);
        AddHandler(TextCompositionManager.TextInputStartEvent, new TextCompositionEventHandler(OnTextInputStart), false);

        // Poke our background color property to start out our background color.
        if(IsMouseOver)
        {
            if(_leaf == null)
            {
                Background = Brushes.Red;
            }
            else
            {
                Background = Brushes.DarkRed;
            }
        }
        else
        {
            if(_leaf == null)
            {
                Background = Brushes.LightGray;
            }
            else
            {
                Background = Brushes.Gray;
            }
        }
    }

    public Brush Background {get {return _background;} set {_background = value;InvalidateVisual();}}

    
    public void ChangeTree()
    {

        PushMessage("ChangeTree: set focus to _leaf");
        _leaf.Focus();

        PushMessage("ChangeTree: remove leaf from visual tree");
        _children.Remove(_leaf);

        PushMessage("ChangeTree: remove leaf from logical tree");
        RemoveLogicalChild(_leaf);

        _leaf = new InputBox(_name + "NewLeaf", true);

        PushMessage("ChangeTree: add leaf to visual tree");
        _children.Add(_leaf);

        PushMessage("ChangeTree: add leaf to logical tree");
        AddLogicalChild(_leaf);

        PushMessage("ChangeTree: set focus to (new)_leaf");
        _leaf.Focus();

        PushMessage("ChangeTree: Invalidating Measure/Arrange");
        InvalidateMeasure();
        InvalidateArrange();
    }

        /// <summary>
        ///   Derived class must implement to support Visual children. The method must return
        ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: 
        ///       During this virtual call it is not valid to modify the Visual tree. 
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {            
            if(_children == null)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }
            if(index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            return _children[index];
        }
        
        /// <summary>
        ///  Derived classes override this property to enable the Visual code to enumerate 
        ///  the Visual children. Derived classes need to return the number of children
        ///  from this method.
        ///
        ///    By default a Visual does not have any children.
        ///
        ///  Remark: During this virtual method the Visual tree must not be modified.
        /// </summary>        
        protected override int VisualChildrenCount
        {           
            get 
            { 
                if(_children == null)
                {
                    throw new ArgumentOutOfRangeException("_children is null");
                }                
                return _children.Count; 
            }
        }    



    protected override Size MeasureOverride(Size constraint)
    {
        if(_leaf != null)
        {
            Size childConstraint = new Size(constraint.Width/2, constraint.Height/2);

            _leaf.Measure(childConstraint);
        }
        
        return constraint;
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        if(_leaf != null)
        {
            double cx = arrangeBounds.Width/2;
            double cy = arrangeBounds.Height/2;
            Size childArrangeBounds = new Size(cx, cy);

            _leaf.Arrange(new Rect(new Point(cx, cy), childArrangeBounds));
        }
        return arrangeBounds;
    }

    protected override void OnRender(DrawingContext ctx)
    {
        ctx.DrawRectangle( 
            _background,
            null,
            new Rect(new Point(), RenderSize));
        
        Typeface typeface = new Typeface("MingLiU");
        Brush pen = Brushes.Black;

        string s = null;
        double y = 0;
        double cy = RenderSize.Height/MAX_MESSAGES;

        for(int i = 0; i < MAX_MESSAGES; i++)
        {
            s = _messages[i];
            
            if(s != null)
            {
                FormattedText text = new FormattedText(s, Language.GetSpecificCulture(), FlowDirection.LeftToRight, typeface, cy, pen);
                ctx.DrawText(text, new Point(5, y));
                y += cy;
            }
        }
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
        if(IsMouseOver)
        {
            if(_leaf == null)
            {
                Background = Brushes.Red;
            }
            else
            {
                Background = Brushes.DarkRed;
            }
        }

        if (e != null)
        {
	    PushMessage("MouseEnter");

            // Check that IsMouseOver is true 
            // for the current node and all its ancestors
            FrameworkElement node = this;
            while (node != null)
            {
                // Check that IsMouseOver is true
                if (!node.IsMouseOver)
                {
                    throw new Exception("OnMouseEnter, IsMouseOver must be true");
                }
                
                // Get Parent
                node = (FrameworkElement)VisualTreeHelper.GetParent(node);
            }
        }
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        if (!IsMouseOver)
        {
            if (_leaf == null)
            {
                Background = Brushes.LightGray;
            }
            else
            {
                Background = Brushes.Gray;
            }
        }

        if (e != null)
        {
            PushMessage("MouseLeave");

            // Three possibilities
            //
            //  1> MouseLeave to enter an element in 
            //  a different subtree then IsMouseOver 
            //  must be false throughout the ancestry
            //
            //  2> MouseLeave to enter a parent 
            //  element the IsMouseOver must be 
            //  false until that ancestor and true then on
            //
            //  3> MouseLeave to enter a child element 
            //  then IsMouseOver should stay true 
            //  throughout the ancestry
            bool expectedValue = false;
            FrameworkElement node = this;
            while (node != null)
            {
                // Check that IsMouseOver 
                // has the expectedValue
                if (node.IsMouseOver != expectedValue)
                {
                    // Case when mouse leaves to enter a parent
                    if (node == Mouse.DirectlyOver)
                    {
                        expectedValue = true;
                    }
                    else
                    {
                        // Case where mouse leaves to enter a child
                        int count = VisualTreeHelper.GetChildrenCount(node);
                        for(int i = 0; i < count; i++)
                        {
                            FrameworkElement feChild = VisualTreeHelper.GetChild(node, i) as FrameworkElement;
                            
                            if (feChild != null && feChild.IsMouseOver)
                            {
                                expectedValue = true;
                                break;
                            }
                        }

                        // Erroneous value for IsMouseOver
                        if (expectedValue == false)
                        {
                            throw new Exception("OnMouseLeave, IsMouseOver must be " + expectedValue + " for " + _name);
                        }
                    }
                }
                
                // Get Parent
                node = (FrameworkElement)VisualTreeHelper.GetParent(node);
            }
        }
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
        PushMessage("PreviewKeyDown (" + e.Key + "=" + KeyStatesToString(e.KeyStates) + "," + "repeat=" + e.IsRepeat + ")");
        base.OnPreviewKeyDown(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        PushMessage("KeyDown (" + e.Key + "=" + KeyStatesToString(e.KeyStates) + "," + "repeat=" + e.IsRepeat + ")");
        base.OnKeyDown(e);

        if(_leaf != null && e.Key == Key.Delete)
        {
            _children.Remove(_leaf);
            _leaf = null;
        }
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

        if(e.Key == Key.Delete)
        {
//            throw new ApplicationException("Should never get a delete key!");
        }
    }

    protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        string oldFocus = "null";
        if(e.OldFocus != null)
        {
            if(e.OldFocus is InputBox)
            {
                oldFocus = ((InputBox)e.OldFocus)._name;
            }
            else
            {
                oldFocus = e.OldFocus.ToString();
            }
        }

        string newFocus = "null";
        if(e.NewFocus != null)
        {
            if(e.NewFocus is InputBox)
            {
                newFocus = ((InputBox)e.NewFocus)._name;
            }
            else
            {
                newFocus = e.NewFocus.ToString();
            }
        }

        PushMessage("PreviewGotKeyboardFocus (" + oldFocus + "-->" + newFocus + ")");
        base.OnPreviewGotKeyboardFocus(e);
    }

    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        string oldFocus = "null";
        if(e.OldFocus != null)
        {
            if(e.OldFocus is InputBox)
            {
                oldFocus = ((InputBox)e.OldFocus)._name;
            }
            else
            {
                oldFocus = e.OldFocus.ToString();
            }
        }

        string newFocus = "null";
        if(e.NewFocus != null)
        {
            if(e.NewFocus is InputBox)
            {
                newFocus = ((InputBox)e.NewFocus)._name;
            }
            else
            {
                newFocus = e.NewFocus.ToString();
            }
        }

        PushMessage("GotKeyboardFocus (" + oldFocus + "-->" + newFocus + ")");
        base.OnGotKeyboardFocus(e);

        // Check that IsKeyboardFocusWithin is true 
        // for the current node and all its ancestors
        FrameworkElement node = this;
        while (node != null)
        {
            // Check that IsKeyboardFocusWithin is true
            if (!node.IsKeyboardFocusWithin)
            {
                throw new Exception("OnGotKeyboardFocus, IsKeyboardFocusWithin must be true");
            }
            
            // Get Parent
            node = (FrameworkElement)VisualTreeHelper.GetParent(node);
        }
    }

    protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        string oldFocus = "null";
        if(e.OldFocus != null)
        {
            if(e.OldFocus is InputBox)
            {
                oldFocus = ((InputBox)e.OldFocus)._name;
            }
            else
            {
                oldFocus = e.OldFocus.ToString();
            }
        }

        string newFocus = "null";
        if(e.NewFocus != null)
        {
            if(e.NewFocus is InputBox)
            {
                newFocus = ((InputBox)e.NewFocus)._name;
            }
            else
            {
                newFocus = e.NewFocus.ToString();
            }
        }
        
        PushMessage("PreviewLostKeyboardFocus (" + oldFocus + "-->" + newFocus + ")");
        base.OnPreviewLostKeyboardFocus(e);
    }

    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
        string oldFocus = "null";
        if(e.OldFocus != null)
        {
            if(e.OldFocus is InputBox)
            {
                oldFocus = ((InputBox)e.OldFocus)._name;
            }
            else
            {
                oldFocus = e.OldFocus.ToString();
            }
        }

        string newFocus = "null";
        if(e.NewFocus != null)
        {
            if(e.NewFocus is InputBox)
            {
                newFocus = ((InputBox)e.NewFocus)._name;
            }
            else
            {
                newFocus = e.NewFocus.ToString();
            }
        }
        
        PushMessage("LostKeyboardFocus (" + oldFocus + "-->" + newFocus + ")");
        base.OnLostKeyboardFocus(e);
        
        // Three possibilities
        //
        //  1> LostKeyboardFocus to enter an element in 
        //  a different subtree then IsKeyboardFocusWithin 
        //  must be false throughout the ancestry
        //
        //  2> LostKeyboardFocus to enter a parent 
        //  element the IsKeyboardFocusWithin must be 
        //  false until that ancestor and true then on
        //
        //  3> LostKeyboardFocus to enter a child element 
        //  then IsKeyboardFocusWithin should stay true 
        //  throughout the ancestry
        bool expectedValue = false;
        FrameworkElement node = this;
        while (node != null)
        {
            // Check that IsKeyboardFocusWithin 
            // has the expectedValue
            if (node.IsKeyboardFocusWithin != expectedValue)
            {
                // Case when lost focus to enter a parent
                if (node.IsKeyboardFocused)
                {
                    expectedValue = true;
                }
                else
                {
                    // Case where lost focus to enter a child
                    int count = VisualTreeHelper.GetChildrenCount(node);
                    for(int i = 0; i < count; i++)
                    {
                        if (((FrameworkElement)(VisualTreeHelper.GetChild(node,i))).IsKeyboardFocusWithin)
                        {
                            expectedValue = true;
                            break;
                        }
                    }
        
                    // Erroneous value for IsMouseOver
                    if (expectedValue == false)
                    {
                        throw new Exception("OnLostKeyboardFocus, IsKeyboardFocusWithin must be " + expectedValue + " for " + _name);
                    }
                }
            }
            
            // Get Parent
            node = (FrameworkElement)VisualTreeHelper.GetParent(node);
        }
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
        for(int i = 0; i < (MAX_MESSAGES -1 ); i++)
        {
            _messages[i] = _messages[i+1];
        }
        _messages[MAX_MESSAGES - 1] = msg;
        InvalidateVisual();

        DrtInput.VerifyInput(_name + ": " + msg);
    }

    // The default KeyStates.ToString has some issues:
    // 1) Since KeyStates.Up is 0, it only shows up in the string if no other
    //    bit is set (such as toggled).
    // 2) The toggled bit is included, which would be fine except that for a
    //    DRT the initial state of the keyboard is not predictable.
    private string KeyStatesToString( KeyStates keyStates)
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
    
    private Brush _background;
    private const int MAX_MESSAGES = 20;
    private string[] _messages = new string[MAX_MESSAGES];
    private string _name;
    private VisualCollection _children;        
//    private string _text;
    InputBox _leaf;
}


