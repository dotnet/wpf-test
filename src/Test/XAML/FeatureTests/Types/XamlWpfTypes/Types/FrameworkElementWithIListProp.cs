// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Test.Xaml.Types
{
    #region Class FrameworkElementWithIListProp

    /// <summary>
    /// This class defines custom clr class with various custom properties
    /// </summary>
    public class FrameworkElementWithIListProp : FrameworkElement
    {
        /// <summary>
        /// ArrayList ilistProp
        /// </summary>
        private readonly ArrayList _ilistProp;

        /// <summary>
        /// DependencyProperty IListDPProperty
        /// </summary>
        private static readonly DependencyProperty s_IListDPProperty =
            DependencyProperty.Register(
                "IListDP",
                typeof(ArrayList),
                typeof(FrameworkElementWithIListProp),
                new PropertyMetadata(new ArrayList()));

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkElementWithIListProp"/> class.
        /// </summary>
        public FrameworkElementWithIListProp() : base()
        {
            _ilistProp = new ArrayList();
            SetValue(s_IListDPProperty, new ArrayList());
        }

        #endregion Constructor

        #region ILIST

        /// <summary>
        /// Gets the Ilistprop.
        /// </summary>
        /// <value>The Ilistprop.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList IListProp
        {
            get
            {
                return _ilistProp;
            }
        }

        /// <summary>
        /// Gets the IlistDP.
        /// </summary>
        /// <param name="e">The DependencyObjecte.</param>
        /// <returns>ArrayList value</returns>
        public static ArrayList GetIListDP(DependencyObject e)
        {
            return (ArrayList) e.GetValue(s_IListDPProperty);
        }

        /// <summary>
        /// Sets the IlistDP.
        /// </summary>
        /// <param name="e">The DependencyObject e.</param>
        /// <param name="myProperty">My property.</param>
        public static void SetIListDP(DependencyObject e, ArrayList myProperty)
        {
            e.SetValue(s_IListDPProperty, myProperty);
        }

        #endregion IList
    }

    #endregion Class FrameworkElementWithIListProp
}
