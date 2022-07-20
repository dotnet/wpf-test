// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Module capable of making selections on FlowDocumentPageViewer control.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Security.Permissions;
using Annotations.Test.Reflection;

namespace Avalon.Test.Annotations
{
	class FlowDocumentPageViewerSelector : ADocumentViewerBaseSelector
    {
        #region Constructor

        public FlowDocumentPageViewerSelector(FlowDocumentPageViewer fdpv)
            : base(fdpv)
        {
            // Empty.
        }

        #endregion

        #region Public Methods

        public override object CreateStartPointer(DocumentPage page, object textView)
        {
            object collection = ITextView_TextSegments.GetValue(textView, null);
            object textSegment = Item.GetValue(collection, new object[] { 0 });
            object startPoint = TextSegment_Start.GetValue(textSegment, null);

            startPoint = ReflectionHelper.InvokeMethod(startPoint, "CreatePointer", new object[] { LogicalDirection.Forward });
            if (!((bool)ITextPointer_IsAtInsertionPosition.GetValue(startPoint, null)))
            {
                ITextPointer_MoveToNextInsertionPosition.Invoke(startPoint, new object[] { LogicalDirection.Forward });
            }

            return startPoint;
        }

		public override object CreateEndPointer(DocumentPage page, object textView)
		{
            return FindLastInsertionPosition(textView);
        }

        #endregion
    }
}
