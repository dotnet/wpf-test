// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;       
using System.Windows.Input; 

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// struct for a manipulator for Surface scenarios
    /// </summary>
    public struct ManipulationData
    {       
        // 

        public Vector Translation;
        public double Rotation; 
        public double Scale; 
        public double Expansion;

        public ManipulationData(Vector translation, double rotation, double scale, double expansion)
        {
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
            Expansion = expansion;
        }
    }
}
