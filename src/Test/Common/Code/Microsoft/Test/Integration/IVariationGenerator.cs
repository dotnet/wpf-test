// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVariationGenerator : ITestContract
    {
        /// <summary>
        /// 
        /// </summary>
        List<VariationItem> Generate();
        
        /// <summary>
        /// 
        /// </summary>
        IVariationGeneratorCollection Dependencies
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        void SetPriorityLimit(int priority);

        /// <summary>
        /// 
        /// </summary>
        int PriorityLimit
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        ITestContract DefaultTestContract
        {
            get;
        }
    }
}
