// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Test.Xaml.Types
{
    #region Class NodeForIDictionary

    /// <summary>
    /// This class defines custom clr class with various custom properties
    /// </summary>
    public class NodeForIDictionary : FrameworkElement
    {
        /// <summary>
        /// MyIDictionary dictionary3
        /// </summary>
        private readonly MyIDictionary _dictionary3 = new MyIDictionary();

        /// <summary>
        /// MyIDictionary dictionary1
        /// </summary>
        private MyIDictionary _dictionary1 = new MyIDictionary();

        /// <summary>
        /// MyIDictionary dictionary2 
        /// </summary>
        private MyIDictionary _dictionary2 = new MyIDictionary();

        /// <summary>
        /// MyIDictionary dictionary4
        /// </summary>
        private MyIDictionary _dictionary4 = new MyIDictionary();

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeForIDictionary"/> class.
        /// </summary>
        public NodeForIDictionary()
            : base()
        {
            //// ((IDictionary)_dictionary1).Add(1, "value for first property").
            //// ((IDictionary)_dictionary1).Add(2, new Button()).
            //// ((IDictionary)_dictionary1).Add(3, "value for second property").
            //// ((IDictionary)_dictionary1).Add(4, new Button()).
            //// ((IDictionary)_dictionary1).Add(5, "value for third property").
            //// ((IDictionary)_dictionary1).Add(6, new Button()).
            //// ((IDictionary)_dictionary1).Add(7, "value for fourth property").
            //// ((IDictionary)_dictionary1).Add(8, new Button()).
        }

        #endregion Constructor

        #region Clr Property

        /// <summary>
        /// Gets or sets the dictionary1.
        /// </summary>
        /// <value>The dictionary1.</value>
        public MyIDictionary Dictionary1
        {
            get
            {
                return _dictionary1;
            }

            set
            {
                _dictionary1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the dictionary2.
        /// </summary>
        /// <value>The dictionary2.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MyIDictionary Dictionary2
        {
            get
            {
                return _dictionary2;
            }

            set
            {
                _dictionary2 = value;
            }
        }

        /// <summary>
        /// Gets the dictionary3.
        /// </summary>
        /// <value>The dictionary3.</value>
        public MyIDictionary Dictionary3
        {
            get
            {
                return _dictionary3;
            }
        }

        /// <summary>
        /// Gets or sets the dictionary4.
        /// </summary>
        /// <value>The dictionary4.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MyIDictionary Dictionary4
        {
            get
            {
                return _dictionary4;
            }

            set
            {
                _dictionary4 = value;
            }
        }

        #endregion Clr Property
    }

    #endregion Class MyUIElementWithCustomProperties
}
