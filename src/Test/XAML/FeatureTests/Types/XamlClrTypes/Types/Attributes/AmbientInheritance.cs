// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.Xaml;

namespace Microsoft.Test.Xaml.Types.Attributes
{
    /// <summary>
    /// Custom type for the inheriting of ambient properties scenario
    /// </summary>
    public class AmbientInherit : AmbientMultiProp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientInherit"/> class.
        /// </summary>
        public AmbientInherit() : base()
        {
        }
    }

    /// <summary>
    /// Custom type for the setting a parent property to ambient scenario 
    /// [AmbientProperty("Name")]  no longer possible to do this.
    /// </summary>
    public class AmbientInheritSetParent : AmbientMultiProp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientInheritSetParent"/> class.
        /// </summary>
        public AmbientInheritSetParent()
            : base()
        {
        }
    }
}
