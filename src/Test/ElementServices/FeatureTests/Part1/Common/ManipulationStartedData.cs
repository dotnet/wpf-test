// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;       
using System.Windows.Input;
using System.Collections.Generic; 

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// struct for Manipulation Started Event Args
    /// </summary>
    public struct ManipulationStartedData
    {
        public IInputElement Container;
        public Point Origin;
        public IEnumerable<IManipulator> Manipulators;
        
        public ManipulationStartedData(ManipulationStartedEventArgs e)
        {
            Container = e.ManipulationContainer; 
            Origin = e.ManipulationOrigin;
            Manipulators = e.Manipulators; 
        }
    }
}
