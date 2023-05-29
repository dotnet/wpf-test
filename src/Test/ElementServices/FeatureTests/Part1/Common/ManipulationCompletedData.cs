// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;       
using System.Windows.Input;
using System.Collections.Generic; 

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// struct for Manipulation Completed Event Args
    /// </summary>
    public struct ManipulationCompletedData
    {
        public IInputElement Container;
        public Point Origin;
        public ManipulationDelta Total;
        public ManipulationVelocities Velocites;
        public bool IsInertia;
        public IEnumerable<IManipulator> Manipulators;

        public object OriginalSource;
        public object Source;
        public int TimeStamp;   //

        public ManipulationCompletedData(ManipulationCompletedEventArgs e) 
        {
            Container = e.ManipulationContainer;
            Origin = e.ManipulationOrigin; 
            Total = e.TotalManipulation; 
            Velocites = e.FinalVelocities;
            IsInertia = e.IsInertial;
            Manipulators = e.Manipulators;

            OriginalSource = e.OriginalSource;
            Source = e.Source;
            TimeStamp = e.Timestamp;
        }
    }
}
