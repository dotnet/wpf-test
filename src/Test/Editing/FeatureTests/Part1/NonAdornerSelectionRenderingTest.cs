// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verifies rendering of selection in Editing controls with various
//  values of SelectionBrush and SelectionOpacity
//  Also covers API testing for SelectionBrush and SelectionOpacity
//  This test enables non-adorner selection rendering.

using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Media = System.Windows.Media;

using Microsoft.Test.Discovery;
using Microsoft.Test.Imaging;
using MTI = Microsoft.Test.Input;

using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Microsoft.Test.Editing
{
    [Test(2, "Selection", "NonAdorner_SelectionRenderingTest_Editable_Controls", Versions = "4.7.2+", MethodParameters = "/TestCaseType:NonAdornerSelectionRenderingTest /Pri=0", Timeout = 200)]
    [Test(1, "Selection", "NonAdorner_SelectionRenderingTest_FlowDocument_Viewers", Versions = "4.7.2+", MethodParameters = "/TestCaseType:NonAdornerSelectionRenderingTest /Pri=1", Timeout = 270)]
    // This test verifies rendering of SelectionBrush in controls where it is supported: 
    // TextBox, RichTextBox, PasswordBox, FlowDocumentReader, FlowDocumentScrollViewer, FlowDocumentPageViewer    
    public class NonAdornerSelectionRenderingTest : SelectionRenderingTest
    {
        #region Static Constructor

        static NonAdornerSelectionRenderingTest()
        {
            AppContext.SetSwitch("Switch.System.Windows.Controls.Text.UseAdornerForTextboxSelectionRendering", false);
        }

        #endregion
    }
}