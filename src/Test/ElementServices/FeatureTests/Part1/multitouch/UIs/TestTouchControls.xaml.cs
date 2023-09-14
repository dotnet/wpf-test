// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Test.Input.MultiTouch;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// 





    public partial class TestTouchControls : Window
    {
        #region Fields
 
        private int _touchDevice1 = 0;   
        private int _touchDevice2 = 0;  
 
        #endregion
 
        #region Constructor

        public TestTouchControls()
        {
            InitializeComponent();

            Attach();
        }
 
        #endregion
 
        #region Event Handlers
 
        void OnStylusDown(object sender, StylusDownEventArgs e)
        {
            // get the location for this touch
            Point p = e.GetPosition(this);   
 
            // attribute an id with a touch point
            if (_touchDevice1 == 0)
            {
                _touchDevice1 = e.StylusDevice.Id;
#if adhoc
                //// move the rectangle to the given location
                //Touch1.SetValue(Canvas.LeftProperty, p.X - Touch1.Width / 2);
                //Touch1.SetValue(Canvas.TopProperty, p.Y - Touch1.Height / 2);

#endif
            }
            else if (_touchDevice2 == 0)
            {
                _touchDevice2 = e.StylusDevice.Id;

                //// move the rectangle to the given location
                //Touch2.SetValue(Canvas.LeftProperty, p.X - Touch2.Width / 2);
                //Touch2.SetValue(Canvas.TopProperty, p.Y - Touch2.Height / 2);
            }
        }
 
        void OnStylusMove(object sender, StylusEventArgs e)
        {
            Point p = e.GetPosition(this);
           
            // determine which touch this belongs to
            if (_touchDevice1 == e.StylusDevice.Id)
            {
                //// move the rectangle to the given location
                //Touch1.SetValue(Canvas.LeftProperty, p.X - Touch1.Width / 2);
                //Touch1.SetValue(Canvas.TopProperty, p.Y - Touch1.Height / 2);
            }
            else if (_touchDevice2 == e.StylusDevice.Id)
            {
                //// move the rectangle to the given location
                //Touch2.SetValue(Canvas.LeftProperty, p.X - Touch2.Width / 2);
                //Touch2.SetValue(Canvas.TopProperty, p.Y - Touch2.Height / 2);
            }
        }
 
        void OnStylusUp(object sender, StylusEventArgs e)
        {
            // reinitialize touch id and hide the rectangle
 	        if (e.StylusDevice.Id == _touchDevice1)
            {
                //Touch1.SetValue(Canvas.LeftProperty, -Touch1.Width);
                _touchDevice1 = 0;
            }
            else if (e.StylusDevice.Id == _touchDevice2)
            {
                //Touch2.SetValue(Canvas.LeftProperty, -Touch2.Width);
                _touchDevice2 = 0;
            }
        }

        private void OnStylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        {

        }

        private void OnTouchDown(object sender, TouchEventArgs e)
        {

        }

        private void OnTouchMove(object sender, TouchEventArgs e)
        {

        }

        private void OnTouchUp(object sender, TouchEventArgs e)
        {

        }

        private void OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {

        }

        #endregion

        #region Helpers

        void Attach()
        {
            //

            this.StylusDown += new StylusDownEventHandler(OnStylusDown);
            this.StylusMove += new StylusEventHandler(OnStylusMove);
            this.StylusUp += new StylusEventHandler(OnStylusUp);            
        }

        void Detach()
        {
            //

            this.StylusDown -= new StylusDownEventHandler(OnStylusDown);
            this.StylusMove -= new StylusEventHandler(OnStylusMove);
            this.StylusUp -= new StylusEventHandler(OnStylusUp);               
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Detach();
            this.Close();
        }

        #endregion

        private void OnControlPanelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnControlPanelMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void OnControlPanelMouseMove(object sender, MouseEventArgs e)
        {

        }

        private void OnControlPanelContactDown(object sender, TouchEventArgs e)
        {

        }

        private void OnControlPanelContactChanged(object sender, TouchEventArgs e)
        {

        }

        private void OnControlPanelContactLeave(object sender, TouchEventArgs e)
        {

        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void OnTreeViewTouchDown(object sender, TouchEventArgs e)
        {

        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnLoadButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnWindowTransparencySliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void bulkTweakModeCheckBoxChanged(object sender, RoutedEventArgs e)
        {

        }

        private void onDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

    }
}
