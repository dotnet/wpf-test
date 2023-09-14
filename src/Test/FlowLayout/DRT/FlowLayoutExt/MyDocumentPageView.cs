// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Provides a view port for a page of content for a DocumentPage.
//

using System.Windows.Automation;            // AutomationPattern
using System.Windows.Automation.Provider;   // IAutomationPatternProvider
using System.Windows.Controls;              // StretchDirection
using System.Windows.Controls.Primitives;   // DocumentViewerBase
using System.Windows.Documents;             // IDocumentPaginator
using System.Windows.Media;                 // Visual
using System.Windows.Threading;             // Dispatcher


namespace DRT
{
    /// <summary> 
    /// Provides a view port for a page of content for a DocumentPage.
    /// </summary>
    public class MyDocumentPageView : DocumentPageView
    {
        /// <summary> 
        /// Create an instance of a DocumentPageView.
        /// </summary>
        /// <remarks>
        /// This does basic initialization of the DocumentPageView.  All subclasses
        /// must call the base constructor to perform this initialization.
        /// </remarks>
        public MyDocumentPageView() : base()
        {
        }
        
        /// <summary>
        /// Gets or sets the transform of this Visual.
        /// </summary>
        public Transform Transform
        {
        //USED BY SPLITPAGE:-
            get
            {
                return this.VisualTransform;
            }
            set
            {
                this.VisualTransform = value;
            }
        }    
    }
}