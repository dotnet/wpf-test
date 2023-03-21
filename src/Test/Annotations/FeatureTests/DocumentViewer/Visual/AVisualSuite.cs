// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Base class for DocumentViewer Visual tests.

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Reflection;
using System.Windows.Threading;
using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations.Anchoring;

using Annotations.Test;
using Annotations.Test.Framework;
using Annotations.Test.Reflection;

namespace Avalon.Test.Annotations
{
    public abstract class AVisualSuite : ADefaultContentSuite
    {
        #region Protected Methods

	        /// <returns>
		/// Position data that is understood by the ADocumentViewerBaseWrapper class for selection of the given type.
		/// </returns>
		public object[] SelectionData(SelectionType type)
		{
			return SelectionMap.SelectionData(ContentMode, type);
		}

		/// <returns>Expected text of the given selection type.</returns>
		public string ExpectedSelectedText(SelectionType type)
		{
			// 
			// Removing hardcoded text selections.
			// Assume that the selection is always correct and just return the selection for that range.
			//	
			object[] data = SelectionData(type);
			DocViewerWrapper.SelectionModule.WriteThrough = false; // Make sure to set back to true!
			// Reflectively invoke so to simplify method call since we don't know how the selection is defined.
			TextRange selection = (TextRange)ReflectionHelper.InvokeMethod(DocViewerWrapper.SelectionModule, "SetSelection", data); 
			DocViewerWrapper.SelectionModule.WriteThrough = true; 
			return selection.Text;
		}

		#endregion Protected Methods
	}
}	

