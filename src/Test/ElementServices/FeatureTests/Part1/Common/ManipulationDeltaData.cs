// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;       
using System.Windows.Input;
using System.Collections.Generic; 

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// struct for Manipulation Delta Event Args
    /// </summary>
    public struct ManipulationDeltaData
    {
        public bool IsInertial;
        public IInputElement Container;
        public Point Origin;
        public ManipulationDelta Delta;
        public ManipulationDelta Cumulative;
        public ManipulationVelocities Velocites;
        public IEnumerable<IManipulator> Manipulators;
        
        public object OriginalSource;
        public object Source;
        public int TimeStamp; 

        // 

        public ManipulationDeltaData(ManipulationDeltaEventArgs e) 
        {
            Origin = e.ManipulationOrigin; 
            Delta = e.DeltaManipulation; 
            Cumulative = e.CumulativeManipulation; 
            Velocites = e.Velocities;
            IsInertial = e.IsInertial;
            Container = e.ManipulationContainer;
            Manipulators = e.Manipulators;

            OriginalSource = e.OriginalSource;
            Source = e.Source;
            TimeStamp = e.Timestamp;
        }
    }
}
