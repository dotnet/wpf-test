// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace PomParser.Baml.Common
{
    internal enum BamlFlowDirection : byte
    {
        /// <summary>
        /// FlowDirection is inheried in the Baml tree. It means that the original 
        /// element doesn't specify the flow direction property value. The final value
        /// is determined by property inheritance. 
        /// </summary>
        Inherited,

        /// <summary>
        /// Baml element has flow direction property value set to LeftToRight.
        /// </summary>        
        LeftToRight,

        /// <summary>
        /// Baml element has flow direction property value set to RightToLeft.
        /// </summary>                
        RightToLeft,
    }   
}
