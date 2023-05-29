// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Holds verification routines for IDScope tests.
 *
 
  
 * Revision:         $Revision: $
 
 * Filename:         $Source: $
********************************************************************/

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Avalon.Test.CoreUI.Common;
using System.Windows.Media;
using System.Windows.Documents;
using Avalon.Test.CoreUI.Parser;
using System.Collections;
using Microsoft.Test.Serialization.CustomElements;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Holds verification routines for Attache Routed Events.
    /// </summary>
    public class AttachedRoutedEventVerifiers
    {
        /// <summary>
        /// Verifies routine for AttachedRoutedEvent1.xaml
        /// </summary>
        public static void AttachedRoutedEvent1Verify(UIElement root)
        {
            CoreLogger.LogStatus("Inside AttachedRoutedEventVerifiers.AttachedRoutedEvent1Verify()...");
            
            //Find the subButton.
            Button subButton = (Button)LogicalTreeHelper.FindLogicalNode(root, "subButton");

            VerifyElement.VerifyBool(null == subButton, false);

            //Find the buttonInResources.
            Button buttonInResources = subButton.Content as Button;
            VerifyElement.VerifyBool(null == buttonInResources, false);

            //Find Border in VisualTree.
            int count = VisualTreeHelper.GetChildrenCount(buttonInResources);
            VerifyElement.VerifyInt(count, 1);
            Border border = VisualTreeHelper.GetChild(buttonInResources,0) as Border;            
            CoreLogger.LogStatus("Is the border the visual child?" + VisualTreeHelper.GetChild(buttonInResources,0).ToString());
            VerifyElement.VerifyBool(null != border, true);

            //Raise event on the Border.
            MouseEventArgs args = new MouseEventArgs(InputManager.Current.PrimaryMouseDevice, 0);
            args.RoutedEvent=Mouse.MouseMoveEvent; 
            border.RaiseEvent(args);

            //Verify that event handers on Border, subButton and button have all be executed. 
            CoreLogger.LogStatus("Verify that event handers have been invoked. ");
            TextBox text = (TextBox)LogicalTreeHelper.FindLogicalNode(root, "text");
            VerifyElement.VerifyString(text.Text, " From border From  From subButton From button");
        }
    }
}

