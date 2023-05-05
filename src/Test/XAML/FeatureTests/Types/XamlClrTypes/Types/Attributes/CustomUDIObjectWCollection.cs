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
    /// Custom Object with Collection for DefaultValue scenario
    /// </summary>
    [ContentProperty("Children")]
    public class CustomUDIObjectWCollection : CustomUDIObject
    {
        /// <summary> CustomUDICollection_Object Children </summary>
        private readonly CustomUDICollection_Object _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIObjectWCollection"/> class.
        /// </summary>
        public CustomUDIObjectWCollection() : base()
        {
            _children = new CustomUDICollection_Object(this);
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public CustomUDICollection_Object Children
        {
            get
            {
                return _children;
            }
        }
    }

    /// <summary>
    /// Custom Object with Collection for UsableDuringInitialization scenario
    /// </summary>
    [UsableDuringInitialization(true)]
    public class CustomUDIObjectWCollection_UDI : CustomUDIObjectWCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIObjectWCollection_UDI"/> class.
        /// </summary>
        public CustomUDIObjectWCollection_UDI()
            : base()
        {
        }
    }

    /// <summary>
    /// Custom Object with Collection for UsableDuringInitialization(false) scenario
    /// </summary>
    [UsableDuringInitialization(false)]
    public class CustomUDIObjectWCollection_UDIfalse : CustomUDIObjectWCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIObjectWCollection_UDIfalse"/> class.
        /// </summary>
        public CustomUDIObjectWCollection_UDIfalse()
            : base()
        {
        }
    }

    /// <summary>
    /// Custom Object with Collection for UsableDuringInitialization(false) inheriting from UsableDuringInitialization scenario
    /// </summary>
    [UsableDuringInitialization(false)]
    public class CustomUDIObjectWCollection_SubUDI_UDIfalse : CustomUDIObjectWCollection_UDI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIObjectWCollection_SubUDI_UDIfalse"/> class.
        /// </summary>
        public CustomUDIObjectWCollection_SubUDI_UDIfalse()
            : base()
        {
        }
    }

    /// <summary>
    /// Custom Object with Collection for UsableDuringInitialization inheriting from UsableDuringInitialization(false) scenario
    /// </summary>
    [UsableDuringInitialization(true)]
    public class CustomUDIObjectWCollection_SubUDIfalse_UDI : CustomUDIObjectWCollection_UDIfalse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIObjectWCollection_SubUDIfalse_UDI"/> class.
        /// </summary>
        public CustomUDIObjectWCollection_SubUDIfalse_UDI()
            : base()
        {
        }
    }
}
