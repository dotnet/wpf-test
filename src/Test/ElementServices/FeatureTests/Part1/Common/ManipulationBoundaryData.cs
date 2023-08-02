// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;       
using System.Windows.Input; 

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// struct for a manipulation boundary feedback
    /// </summary>
    public struct ManipulationBoundaryData
    {
        public ManipulationDelta BoundaryFeedback;
        public IInputElement Container;

        public object OriginalSource;
        public object Source;
        public int TimeStamp;

        public ManipulationBoundaryData(ManipulationBoundaryFeedbackEventArgs e) 
        {
            BoundaryFeedback = e.BoundaryFeedback; 
            Container = e.ManipulationContainer;

            OriginalSource = e.OriginalSource;
            Source = e.Source;
            TimeStamp = e.Timestamp;
        }
    }
}
