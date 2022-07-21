// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Module capable of making selection on a FlowDocumentScrollViewer control.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Annotations.Test.Reflection;

namespace Avalon.Test.Annotations
{
    public class FlowDocumentScrollViewerSelector : SelectionModule
    {
        #region Constructor

        public FlowDocumentScrollViewerSelector(FlowDocumentScrollViewer fdsv) 
            : base(fdsv)
        {
            // Empty.
        }

        #endregion

        #region Overrides

        public override IDocumentPaginatorSource Document
        {
            get
            {
                return Viewer.Document;
            }
        }

        public override object StartOfDocument
        {
            get
            {
                return ITextContainer_Start.GetValue(TextContainer, null);
            }
        }

        public override object EndOfDocument
        {
            get
            {
                return ITextContainer_End.GetValue(TextContainer, null);
            }
        }

        public override object CreatePointer(object pointer, int offset)
        {
            return ITextPointer_CreatePointer.Invoke(pointer, new object[] { offset });
        }

        #endregion     

        #region Properties

        protected object TextContainer
        {
            get
            {
                return ITextView_TextContainer.GetValue(TextView, null);
            }
        }

        protected object TextView
        {
            get
            {
                return ReflectionHelper.InvokeMethod(Viewer, "GetTextView");
            }
        }

        protected FlowDocumentScrollViewer Viewer
        {
            get
            {
                return targetControl as FlowDocumentScrollViewer;
            }
        }

        #endregion

    }
}
