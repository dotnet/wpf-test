// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Customized DocumentViewerBase and FlowDocumentPageViewer. 
//

using System;
using System.Windows;                       // FrameworkPropertyMetadata, ResourceDictionary
using System.Windows.Controls;              // FlowDocumentPageViewer
using System.Windows.Controls.Primitives;   // DocumentViewerBase

namespace DRT
{
    /// <summary>
    /// Customized DocumentViewerBase.
    /// </summary>
    internal sealed class CustomDocumentViewer : DocumentViewerBase
    {
        /// <summary>
        /// Initializes class-wide settings.
        /// </summary>
        static CustomDocumentViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomDocumentViewer), new FrameworkPropertyMetadata(typeof(CustomDocumentViewer)));
        }
    }

    /// <summary>
    /// Customized FlowDocumentPageViewer.
    /// </summary>
    internal sealed class CustomFlowDocumentPageViewer : FlowDocumentPageViewer
    {
    }
}
