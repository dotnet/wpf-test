// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;       
using System.Windows.Input;
using System.Collections.Generic; 

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// struct for properties in ManipulationInertiaStartingEventArgs
    /// </summary>
    public struct ManipulationInertiaStartingData
    {
        public Point Origin;
        public ManipulationVelocities InitialVelocites;
        // 
        public InertiaExpansionBehavior ExpansionBehavior; 
        public IInputElement Container; 
        public InertiaRotationBehavior RotationBehavior; 
        public InertiaTranslationBehavior TranslationBehavior;
        public IEnumerable<IManipulator> Manipulators;

        public ManipulationInertiaStartingData(ManipulationInertiaStartingEventArgs e) 
        {
            Origin = e.ManipulationOrigin; 
            InitialVelocites = e.InitialVelocities;
            ExpansionBehavior = e.ExpansionBehavior;
            RotationBehavior = e.RotationBehavior;
            TranslationBehavior = e.TranslationBehavior;
            Container = e.ManipulationContainer;
            Manipulators = e.Manipulators; 
        }
    }
}
