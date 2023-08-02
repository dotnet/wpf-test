// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Verification routines for Document-related xaml tests.
 * Contributor: Microsoft
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Common;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Collections;
using Avalon.Test.CoreUI.Parser;
using Microsoft.Test.Serialization.CustomElements;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Verification routines for Document-related xaml tests.
    /// </summary>
    public class DocumentXamlVerifiers
    {
        /// <summary>
        /// Verifies a FixedDocument tree is created correctly from markup.
        /// </summary>
        public static void BugRepro5Verify(UIElement uiElement)
        {
            DependencyObject root = (DependencyObject)TreeHelper.FindLogicalRoot(uiElement);

            _VerifyLogicalRoot(root, uiElement, "MyDocumentViewer");
            _VerifyLogicalRoot(root, uiElement, "MyFixedDocument");
            _VerifyLogicalRoot(root, uiElement, "MyPageContent");
            _VerifyLogicalRoot(root, uiElement, "MyFixedPage");
            _VerifyLogicalRoot(root, uiElement, "MyTextBlock");
        }
        private static void _VerifyLogicalRoot(DependencyObject root, DependencyObject subTreeRoot, string id)
        {
            object obj = TreeHelper.FindNodeById(subTreeRoot, id);

            if (root != TreeHelper.FindLogicalRoot((DependencyObject)obj))
            {
                throw new Microsoft.Test.TestValidationException("The logical root is not correct.");
            }
        }
    }
}

