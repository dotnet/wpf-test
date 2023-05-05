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
    #region Class NodeForIEnumerable

    /// <summary>
    /// This class defines custom clr class with various custom properties
    /// </summary>
    public class NodeForIEnumerable : FrameworkElement
    {
        /// <summary>
        /// MyIEnumerable enumerable3
        /// </summary>
        private readonly MyIEnumerable _enumerable3 = new MyIEnumerable();

        /// <summary>
        /// MyIEnumerable enumerable1
        /// </summary>
        private MyIEnumerable _enumerable1 = new MyIEnumerable();

        /// <summary>
        /// MyIEnumerable enumerable2
        /// </summary>
        private MyIEnumerable _enumerable2 = new MyIEnumerable();

        /// <summary>
        /// MyIEnumerable enumerable4
        /// </summary>
        private MyIEnumerable _enumerable4 = new MyIEnumerable();

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeForIEnumerable"/> class.
        /// </summary>
        public NodeForIEnumerable() : base()
        {
            _enumerable3 = new MyIEnumerable();
        }

        #endregion Constructor

        #region Clr Property    

        /// <summary>
        /// Gets or sets the enumerable1.
        /// </summary>
        /// <value>The enumerable1.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MyIEnumerable Enumerable1
        {
            get
            {
                return _enumerable1;
            }

            set
            {
                _enumerable1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the enumerable2.
        /// </summary>
        /// <value>The enumerable2.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MyIEnumerable Enumerable2
        {
            get
            {
                return _enumerable2;
            }

            set
            {
                _enumerable2 = value;
            }
        }

        /// <summary>
        /// Gets the enumerable3.
        /// </summary>
        /// <value>The enumerable3.</value>
        public MyIEnumerable Enumerable3
        {
            get
            {
                return _enumerable3;
            }
        }

        /// <summary>
        /// Gets or sets the enumerable4.
        /// </summary>
        /// <value>The enumerable4.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MyIEnumerable Enumerable4
        {
            get
            {
                return _enumerable4;
            }

            set
            {
                _enumerable4 = value;
            }
        }

        #endregion Clr Property 
    }

    #endregion Class MyUIElementWithCustomProperties
}
