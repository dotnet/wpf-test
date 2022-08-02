// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

public class InputBoxGrid : FrameworkElement //UIElement
{
    public InputBoxGrid()
    {
        _children = new VisualCollection(this);        
        _nw = new InputBox("NW", false);
        _ne = new InputBox("NE", false);
        _sw = new InputBox("SW", false);
        _se = new CustomContentHost("SE");

        _children.Add(_nw);
        AddLogicalChild(_nw);
        _children.Add(_ne);
        AddLogicalChild(_ne);
        _children.Add(_sw);
        AddLogicalChild(_sw);
        _children.Add(_se);
        AddLogicalChild(_se);
    }

    public InputBox NW {get {return _nw;}}
    public InputBox NE {get {return _ne;}}
    public InputBox SW {get {return _sw;}}
    public CustomContentHost SE {get {return _se;}}

    public int VerticalDividerPercent
    {
        get 
        { 
            return _verticalDividerPercent; 
        }
    
        set 
        { 
            _verticalDividerPercent = value;
            InvalidateMeasure();
            InvalidateArrange();
        }
    }

    public int HorizontalDividerPercent
    {
        get 
        { 
            return _horizontalDividerPercent; 
        }
    
        set 
        { 
            _horizontalDividerPercent = value;
            InvalidateMeasure();
            InvalidateArrange();
        }
    }

    public void ChangeTree()
    {
        SW.ChangeTree();
    }
    
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        DrtInput.VerifyInput("InputBoxGrid : MouseEnter");
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
        DrtInput.VerifyInput("InputBoxGrid : MouseLeave");
        base.OnMouseLeave(e);
    }

    protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
    {
        DrtInput.VerifyInput("InputBoxGrid : IsKeyboardFocusWithinChanged (" + ((bool)e.OldValue) + "-->" + ((bool)e.NewValue) + ")");
    }
    
    protected override Size MeasureOverride(Size constraint)
    {
        double cx = (constraint.Width * _horizontalDividerPercent /100);
        double cy = (constraint.Height * _verticalDividerPercent /100);
        Size childConstraint;

        childConstraint = new Size(cx, cy);
        _nw.Measure(childConstraint);
        
        childConstraint = new Size(constraint.Width - cx, cy);
        _ne.Measure(childConstraint);
        
        childConstraint = new Size(cx, constraint.Height - cy);
        _sw.Measure(childConstraint);
        
        childConstraint = new Size(constraint.Width - cx, constraint.Height - cy);
        _se.Measure(childConstraint);

        return constraint;
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        double cx = (arrangeBounds.Width * _horizontalDividerPercent/100);
        double cy = (arrangeBounds.Height * _verticalDividerPercent/100);
        Size childArrangeBounds;

        childArrangeBounds = new Size(cx, cy);
        _nw.Arrange(new Rect(new Point(), childArrangeBounds));
        
        childArrangeBounds = new Size(arrangeBounds.Width - cx, cy);
        _ne.Arrange(new Rect(new Point(cx, 0), childArrangeBounds));
        
        childArrangeBounds = new Size(cx, arrangeBounds.Height - cy);
        _sw.Arrange(new Rect(new Point(0, cy), childArrangeBounds));
        
        childArrangeBounds = new Size(arrangeBounds.Width - cx, arrangeBounds.Height - cy);
        _se.Arrange(new Rect(new Point(cx, cy), childArrangeBounds));
        return arrangeBounds;
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

    private VisualCollection _children;              
    private InputBox _nw;
    private InputBox _ne;
    private InputBox _sw;
    private CustomContentHost _se;

    private int _verticalDividerPercent;
    private int _horizontalDividerPercent;
}


