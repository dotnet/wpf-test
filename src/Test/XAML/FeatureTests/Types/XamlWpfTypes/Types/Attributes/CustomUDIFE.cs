// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;

namespace Microsoft.Test.Xaml.Types.Attributes
{
    /// <summary>
    /// Custom FE for DefaultValue scenario
    /// </summary>
    public class CustomUDIFE : FrameworkElement
    {
        /// <summary> Order array </summary>
        private readonly ArrayList _array;

        /// <summary> Text value </summary>
        private string _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIFE"/> class.
        /// </summary>
        public CustomUDIFE()
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
    /// Custom FE for UsableDuringInitialization scenario
    /// </summary>
    [UsableDuringInitialization(true)]
    public class CustomUDIFE_UDI : CustomUDIFE
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIFE_UDI"/> class.
        /// </summary>
        public CustomUDIFE_UDI()
            : base()
        {
        }
    }

    /// <summary>
    /// Custom FE for UsableDuringInitialization(false) scenario
    /// </summary>
    [UsableDuringInitialization(false)]
    public class CustomUDIFE_UDIfalse : CustomUDIFE
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIFE_UDIfalse"/> class.
        /// </summary>
        public CustomUDIFE_UDIfalse()
            : base()
        {
        }
    }

    /// <summary>
    /// Custom FE for UsableDuringInitialization(false) inheriting from UsableDuringInitialization scenario
    /// </summary>
    [UsableDuringInitialization(false)]
    public class CustomUDIFE_SubUDI_UDIfalse : CustomUDIFE_UDI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIFE_SubUDI_UDIfalse"/> class.
        /// </summary>
        public CustomUDIFE_SubUDI_UDIfalse()
            : base()
        {
        }
    }

    /// <summary>
    /// Custom FE for UsableDuringInitialization inheriting from UsableDuringInitialization(false) scenario
    /// </summary>
    [UsableDuringInitialization(true)]
    public class CustomUDIFE_SubUDIfalse_UDI : CustomUDIFE_UDIfalse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomUDIFE_SubUDIfalse_UDI"/> class.
        /// </summary>
        public CustomUDIFE_SubUDIfalse_UDI()
            : base()
        {
        }
    }
}
