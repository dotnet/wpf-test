// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Windows;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Avalon.Test.CoreUI.Serialization
{
    #region Class NodeForIDictionary
    /// <summary>
    /// This class defines custom clr class with various custom properties
    /// </summary>
    public class NodeForIDictionary : FrameworkElement
    {
        #region Constructor


		/// <summary>
		/// 
		/// </summary>
		public NodeForIDictionary() : base()
		{
//			((IDictionary)_dictionary1).Add(1, "value for first property").
//			((IDictionary)_dictionary1).Add(2, new Button()).
//			((IDictionary)_dictionary1).Add(3, "value for second property").
//			((IDictionary)_dictionary1).Add(4, new Button()).
//			((IDictionary)_dictionary1).Add(5, "value for third property").
//			((IDictionary)_dictionary1).Add(6, new Button()).
//			((IDictionary)_dictionary1).Add(7, "value for fourth property").
//			((IDictionary)_dictionary1).Add(8, new Button()).
		}
        #endregion Constructor

        #region Clr Property    
		/// <summary>
		///  
		/// </summary>
		public MyIDictionary dictionary1
		{
			get { return _dictionary1; }
			set { _dictionary1=value; }
		}

 		MyIDictionary _dictionary1 = new MyIDictionary();
		/// <summary>
		///  
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MyIDictionary dictionary2
		{
			get { return _dictionary2; }
			set { _dictionary2 = value; }
		}

		MyIDictionary _dictionary2 = new MyIDictionary();

		/// <summary>
		///  
		/// </summary>
		public MyIDictionary dictionary3
		{
			get { return _dictionary3; }
		}

		MyIDictionary _dictionary3 = new MyIDictionary();

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MyIDictionary dictionary4
		{
			get { return _dictionary4; }
			set { _dictionary4 = value; }
		}

		MyIDictionary _dictionary4 = new MyIDictionary();
        #endregion Clr Property 

    }
    #endregion Class MyUIElementWithCustomProperties
}
