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
    #region Class NodeForIEnumerable
    /// <summary>
    /// This class defines custom clr class with various custom properties
    /// </summary>
    public class NodeForIEnumerable : FrameworkElement
    {
        #region Constructor


		/// <summary>
		/// 
		/// </summary>
		public NodeForIEnumerable() : base()
		{
			_enumerable3 = new MyIEnumerable();
		}
        #endregion Constructor

        #region Clr Property    
		/// <summary>
		///  
		/// </summary>
                [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MyIEnumerable enumerable1
		{
			get { return _enumerable1; }
			set { _enumerable1=value; }
		}

 		MyIEnumerable _enumerable1 = new MyIEnumerable();
		/// <summary>
		///  
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MyIEnumerable enumerable2
		{
			get { return _enumerable2; }
			set { _enumerable2 = value; }
		}

		MyIEnumerable _enumerable2 = new MyIEnumerable();

		/// <summary>
		///  
		/// </summary>
		public MyIEnumerable enumerable3
		{
			get { return _enumerable3; }
		}

		MyIEnumerable _enumerable3 = new MyIEnumerable();

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MyIEnumerable enumerable4
		{
			get { return _enumerable4; }
			set { _enumerable4 = value; }
		}

		MyIEnumerable _enumerable4 = new MyIEnumerable();
        #endregion Clr Property 

    }
    #endregion Class MyUIElementWithCustomProperties
}
