// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Markup;
using System.Threading;
using System.Windows.Threading;

namespace Avalon.Test.CoreUI.Serialization
{
    #region Class FrameworkElementWithIListProp
    /// <summary>
    /// This class defines custom clr class with various custom properties
    /// </summary>
    public class FrameworkElementWithIListProp : FrameworkElement
    {
        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public FrameworkElementWithIListProp() : base()
        {
            _ilistProp = new ArrayList(); 
            SetValue(s_IListDPProperty, new ArrayList());
        }
        #endregion Constructor

        #region ILIST

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList IListProp
        {
            get { return _ilistProp; }
        }

        private ArrayList _ilistProp;

        private static DependencyProperty s_IListDPProperty =
            DependencyProperty.Register(
                "IListDP", 
                typeof(ArrayList), 
                typeof(FrameworkElementWithIListProp), 
                new PropertyMetadata(new ArrayList()));

                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <returns></returns>
                public static ArrayList GetIListDP(DependencyObject e)
                {
                    return (ArrayList)e.GetValue(s_IListDPProperty);
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="e"></param>
                /// <param name="myProperty"></param>
                public static void SetIListDP(DependencyObject e, ArrayList myProperty)
                {
                    e.SetValue(s_IListDPProperty, myProperty);
                }

        #endregion IList


    }
    #endregion Class FrameworkElementWithIListProp
}
