// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: A trackball controls that allows rotation and scaling
// of a 3D scene.  It will require further work to be a general
// component.
//
//
//

 
/*
Trackball is a little controller for rotating 3D visuals.  It's going
to own/use/replace the transform on the Model3DGroup at the top
of the RetainedVisual3D.  There are other, perhaps better, ways to do
this but this'll work for now.  (Generally I'd have the trackball
adjust the camera matrix, but that's a little bit painful with the
non-matrix cameras.

Clicking and dragging rotates.  If the right button is down when the
click is initiated then it scales.

Rotation is around the lookatpoint of the first slave's
ProjectionCamera.  This will fail if it doesn't have a projection
camera.

While the user is dragging the trackball maintains a delta rotation on
top of the initial rotation that the trackball stored and was using at
the beginning of the rotation.  This is so that eventually I could
handle an "escape" key and revert to the state before the drag started.

Furthermore it's very useful to have double-click (or something)
restore the state before any user interaction.  Rotation's are not
that bad but with translation and scale control it's easy to lose your
place.
*/

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Samples;

public class Trackball
{
    public Trackball()
    {
        Reset();
    }
    
    public void Attach(FrameworkElement element)
    {
        element.MouseMove += this.MouseMoveHandler;
        element.MouseLeftButtonDown += this.MouseDownHandler;
        element.MouseLeftButtonUp += this.MouseUpHandler;
    }
    
    public void Detach(FrameworkElement element)
    {
        element.MouseMove -= this.MouseMoveHandler;
        element.MouseLeftButtonDown -= this.MouseDownHandler;
        element.MouseLeftButtonUp -= this.MouseUpHandler;
    }
    
    public List<Viewport3D> Slaves
    {
        get
        {
            if (_slaves == null)
                _slaves = new List<Viewport3D>();

            return _slaves;
        }
        set
        {
            _slaves = value;
        }
    }

    public bool Enabled
    {
        get
        {
            return _enabled && (_slaves != null) && (_slaves.Count > 0);
        }
        set
        {
            _enabled=value;
        }
    }

    // Updates the matrices of the slaves using the rotation quaternion.
    private void UpdateSlaves(Quaternion q, double s)
    {
        // 
        RotateTransform3D rotation = new RotateTransform3D(new AxisAngleRotation3D(q.Axis,q.Angle));
        ScaleTransform3D scale = new ScaleTransform3D(new Vector3D(s,s,s));
        Transform3DGroup rotateAndScale = new Transform3DGroup();
        rotateAndScale.Children.Add(rotation);
        rotateAndScale.Children.Add(scale);

        if (_slaves != null)
        {
            foreach (Viewport3D i in _slaves)
            {
                foreach(ModelVisual3D mv in i.Children)
                {
                    mv.Transform = rotateAndScale;
                }
            }
        }
    }
    

    private void MouseMoveHandler(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!Enabled) return;
        e.Handled = true;
        
        UIElement el = (UIElement)sender;
        if (el.IsMouseCaptured)
        {
            Vector delta = _point - e.MouseDevice.GetPosition(el);
            delta /= 2;
            // We can redefine this 2D mouse delta as a 3D mouse delta
            // where "into the screen" is Z
            Vector3D mouse = new Vector3D(delta.X,-delta.Y,0);
            Vector3D axis = Vector3D.CrossProduct(mouse, new Vector3D(0,0,1));
            double len = axis.Length;
            if (len < 0.00001 || _scaling)
                _rotationDelta = new Quaternion(new Vector3D(0,0,1),0);
            else
                _rotationDelta = new Quaternion(axis,len);
            Quaternion q = _rotationDelta*_rotation;
            if (_scaling)
            {
                _scaleDelta = Math.Exp(delta.Y/10);
            }
            else
            {
                _scaleDelta = 1;
            }
            UpdateSlaves(q,_scale*_scaleDelta);
        }
    }

    private void MouseDownHandler(object sender, MouseButtonEventArgs e)
    {
        if (!Enabled) return;
        e.Handled = true;

        UIElement el = (UIElement)sender;
        _point = e.MouseDevice.GetPosition(el);
        _scaling = (e.RightButton == MouseButtonState.Pressed);

        el.CaptureMouse();
    }

    private void MouseUpHandler(object sender, MouseButtonEventArgs e)
    {
        if (!_enabled) return;
        e.Handled = true;
        
        // Stuff the current initial + delta into initial so when we next move we
        // start at the right place.
        _rotation = _rotationDelta*_rotation;
        _scale = _scaleDelta*_scale;
        UIElement el = (UIElement)sender;
        el.ReleaseMouseCapture();
    }
    
    private void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
    {
        Reset();
    }

    private void Reset()
    {
        _rotation = Quaternion.Identity;
        _scale = 1;
        // Clear delta too, because if reset is called because of a double click then the mouse
        // up handler will also be called and this way it won't do anything.
        _rotationDelta = Quaternion.Identity;
        _scaleDelta = 1;
        UpdateSlaves(_rotation,_scale);
    }

    // The state of the trackball
    private bool _enabled;
    private double _scale;
    private Quaternion _rotation;
    private List<Viewport3D> _slaves;
    
    // The state of the current drag
    private bool _scaling;              // Are we scaling?  NOTE otherwise we're rotating
    private double _scaleDelta;          // Change to scale because of this drag
    private Quaternion _rotationDelta;  // Change to rotation because of this drag
    private Point _point;               // Initial point of drag
}
