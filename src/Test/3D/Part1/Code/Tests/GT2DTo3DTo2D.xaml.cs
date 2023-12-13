// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/// <summary>
/// Part1 Bug #453522 : Testing GeneratTransfort2DTo3DTo2D on unfreezable 
///                     cameras (no exceptions should be thrown).
/// </summary>

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    public partial class GT2DTo3DTo2D : Window
    {
        /// <summary>
        /// Delegate for placing jobs onto the thread dispatcher.
        /// Used for making asynchronous calls to the 2 Verify methods.
        /// </summary>
        private delegate void Verify_Delegate();

        public GT2DTo3DTo2D()
        {
            InitializeComponent();
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Verify_Delegate(Verify_Step1));
        }

        private void Verify_Step1()
        {
            ((Part1Application)(Application.Current)).Log.LogStatus("Testing GeneralTransform2DTo3DTo2D on unfreezable cameras ...");
            ((Part1Application)(Application.Current)).Log.LogStatus("Expected: No Exception");

            double x = this.Left + (this.ActualWidth / 2);
            double y = this.Top + (this.ActualHeight / 2);

            // moving the mouse pointer over the cube should not throw an exception
            Microsoft.Test.Input.Input.MoveTo(new Point(x, y));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Verify_Delegate(Verify_Step2));
        }

        private void Verify_Step2()
        {
            // if we get to this point, then no exception was thrown
            ((Part1Application)(Application.Current)).Log.LogStatus("Actual: No Exception");
            this.Close();
        }
    }

    public class PointMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Point3D p = new Point3D(System.Convert.ToDouble(values[0]), System.Convert.ToDouble(values[1]), System.Convert.ToDouble(values[2]));
            return p;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}