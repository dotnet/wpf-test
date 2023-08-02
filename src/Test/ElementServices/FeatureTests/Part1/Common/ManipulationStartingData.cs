// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;       
using System.Windows.Input;
using System.Windows.Input.Manipulations;
using System.Collections.Generic;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// struct for Manipulation Starting Event Args
    /// </summary>
    public struct ManipulationStartingData
    {
        public ManipulationModes Mode;
        public IInputElement Container;
        public ManipulationPivot Pivot;
        public bool IsSingleTouchEnabled;
        public IEnumerable<IManipulator> Manipulators;
        // 

        public ManipulationStartingData(ManipulationStartingEventArgs e)
        {
            Mode = e.Mode;
            Container = e.ManipulationContainer;
            Pivot = e.Pivot;
            IsSingleTouchEnabled = e.IsSingleTouchEnabled;
            Manipulators = e.Manipulators; 
        }
    }
}
