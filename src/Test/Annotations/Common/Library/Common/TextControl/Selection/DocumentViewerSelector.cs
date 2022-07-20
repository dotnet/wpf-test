// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Module capable of making selections on DocumentViewer control.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Security.Permissions;

namespace Avalon.Test.Annotations
{
	class DocumentViewerSelector : ADocumentViewerBaseSelector
	{
        public DocumentViewerSelector(DocumentViewer dv)
            : base(dv)
        {
            // Empty.
        }

        public override object CreateStartPointer(DocumentPage page, object textView)
        {
            return ITextView_GetTextPositionFromPoint.Invoke(textView, new object[] { new Point(0,0), true });
        }

		public override object CreateEndPointer(DocumentPage page, object textView)
		{
			return ITextView_GetTextPositionFromPoint.Invoke(textView, new object[] { new Point(page.Size.Width, page.Size.Height), true });
		}
	}    
}
