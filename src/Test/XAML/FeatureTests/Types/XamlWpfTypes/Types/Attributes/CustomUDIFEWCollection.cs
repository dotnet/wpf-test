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
    /// Custom FE with Collection for DefaultValue scenario
    /// </summary>
    [ContentProperty("Children")]
    public class CustomUDIFEWCollection : CustomUDIFE
    {
        /// <summary>
        /// CustomUDICollection_FE Children 
        /// </summary>
        private readonly CustomUDICollection_FE _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIFEWCollection"/> class.
        /// </summary>
        public CustomUDIFEWCollection()
            : base()
        {
            _children = new CustomUDICollection_FE(this);
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public CustomUDICollection_FE Children
        {
            get
            {
                return _children;
            }
        }
    }

    /// <summary>
    /// Custom FE with Collection for DefaultValue scenario
    /// </summary>
    [UsableDuringInitialization(true)]
    public class CustomUDIFEWCollection_UDI : CustomUDIFEWCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIFEWCollection_UDI"/> class.
        /// </summary>
        public CustomUDIFEWCollection_UDI()
            : base()
        {
        }
    }

    /// <summary>
    /// Custom FE with Collection for DefaultValue scenario
    /// </summary>
    [UsableDuringInitialization(false)]
    public class CustomUDIFEWCollection_UDIfalse : CustomUDIFEWCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIFEWCollection_UDIfalse"/> class.
        /// </summary>
        public CustomUDIFEWCollection_UDIfalse()
            : base()
        {
        }
    }

    /// <summary>
    /// Custom FE with Collection for DefaultValue scenario
    /// </summary>
    [UsableDuringInitialization(false)]
    public class CustomUDIFEWCollection_SubUDI_UDIfalse : CustomUDIFEWCollection_UDI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIFEWCollection_SubUDI_UDIfalse"/> class.
        /// </summary>
        public CustomUDIFEWCollection_SubUDI_UDIfalse()
            : base()
        {
        }
    }

    /// <summary>
    /// Custom FE with Collection for DefaultValue scenario
    /// </summary>
    [UsableDuringInitialization(true)]
    public class CustomUDIFEWCollection_SubUDIfalse_UDI : CustomUDIFEWCollection_UDIfalse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIFEWCollection_SubUDIfalse_UDI"/> class.
        /// </summary>
        public CustomUDIFEWCollection_SubUDIfalse_UDI()
            : base()
        {
        }
    }
}
