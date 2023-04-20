// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.Xaml;

namespace Microsoft.Test.Xaml.Types.Attributes
{
    /// <summary>
    /// Custom Object for DefaultValue scenario
    /// </summary>
    public class CustomUDIObject : object
    {
        /// <summary> Order array </summary>
        private readonly ArrayList _array;

        /// <summary> Text value </summary>
        private string _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIObject"/> class.
        /// </summary>
        public CustomUDIObject()
        {
            _array = new ArrayList();
        }

        /// <summary>
        /// Gets the order array.
        /// </summary>
        /// <value>The order array.</value>
        public ArrayList OrderArray
        {
            get
            {
                return _array;
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text value.</value>
        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
                LogAction("Setting Text");
            }
        }

        /// <summary>
        /// Logs the action.
        /// </summary>
        /// <param name="text">The text value.</param>
        public void LogAction(string text)
        {
            _array.Add(text);
        }
    }

    /// <summary>
    /// Custom Object for UsableDuringInitialization scenario
    /// </summary>
    [UsableDuringInitialization(true)]
    public class CustomUDIObject_UDI : CustomUDIObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIObject_UDI"/> class.
        /// </summary>
        public CustomUDIObject_UDI()
            : base()
        {
        }
    }

    /// <summary>
    /// Custom Object for UsableDuringInitialization(false) scenario
    /// </summary>
    [UsableDuringInitialization(false)]
    public class CustomUDIObject_UDIfalse : CustomUDIObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIObject_UDIfalse"/> class.
        /// </summary>
        public CustomUDIObject_UDIfalse()
            : base()
        {
        }
    }

    /// <summary>
    /// Custom Object for UsableDuringInitialization(false) inheriting from UsableDuringInitialization scenario
    /// </summary>
    [UsableDuringInitialization(false)]
    public class CustomUDIObject_SubUDI_UDIfalse : CustomUDIObject_UDI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIObject_SubUDI_UDIfalse"/> class.
        /// </summary>
        public CustomUDIObject_SubUDI_UDIfalse()
            : base()
        {
        }
    }

    /// <summary>
    /// Custom Object for UsableDuringInitialization inheriting from UsableDuringInitialization(false) scenario
    /// </summary>
    [UsableDuringInitialization(true)]
    public class CustomUDIObject_SubUDIfalse_UDI : CustomUDIObject_UDIfalse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIObject_SubUDIfalse_UDI"/> class.
        /// </summary>
        public CustomUDIObject_SubUDIfalse_UDI()
            : base()
        {
        }
    }
}
