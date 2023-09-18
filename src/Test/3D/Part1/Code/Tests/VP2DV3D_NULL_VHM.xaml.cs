// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/// <summary>
/// Part1 Bug #453513 : Viewport2DVisual3D.GetIsVisualHostMaterial(NULL) and 
///                     Viewport2DVisual3D.SetIsVisualHostMaterial(NULL, true) should
///                     throw ArgumentNullException.
/// </summary>

using System;
using System.Windows;
using System.Windows.Media.Media3D;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    public partial class VP2DV3D_NULL_VHM : Window
    {
        public VP2DV3D_NULL_VHM()
        {
            InitializeComponent();
        }

        private void Verify(object sender, RoutedEventArgs e)
        {
            ((Part1Application)(Application.Current)).Log.LogStatus("Testing Viewport2DVisual3D.GetIsVisualHostMaterial(NULL) ...");
            ((Part1Application)(Application.Current)).Log.LogStatus("Expected: ArgumentNullException");
            try
            {
                Boolean res = Viewport2DVisual3D.GetIsVisualHostMaterial((Material)null);
                ((Part1Application)(Application.Current)).Log.LogStatus("Actual: Did not throw any exception - instead returned " + res);
                ((Part1Application)(Application.Current)).Log.AddFailure("No exception was thrown (was expecting ArgumentNullException).");
            }
            catch (ArgumentNullException)
            {
                ((Part1Application)(Application.Current)).Log.LogStatus("Actual: ArgumentNullException");
            }

            ((Part1Application)(Application.Current)).Log.LogStatus("Testing Viewport2DVisual3D.SetIsVisualHostMaterial(NULL, true) ...");
            ((Part1Application)(Application.Current)).Log.LogStatus("Expected: ArgumentNullException");
            try
            {
                Viewport2DVisual3D.SetIsVisualHostMaterial((Material)null, true);
                ((Part1Application)(Application.Current)).Log.LogStatus("Actual: Did not throw any exception");
                ((Part1Application)(Application.Current)).Log.AddFailure("No exception was thrown (was expecting ArgumentNullException).");
            }
            catch (ArgumentNullException)
            {
                ((Part1Application)(Application.Current)).Log.LogStatus("Actual: ArgumentNullException");
            }

            this.Close();
        }
    }
}