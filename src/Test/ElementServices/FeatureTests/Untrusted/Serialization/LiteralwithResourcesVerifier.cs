// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Common;
using System.Windows.Media;
using System.Windows.Documents;
using Avalon.Test.CoreUI.Parser;
using System.Collections;

using Microsoft.Test.Serialization.CustomElements;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Holds verification routines for LiteralwithResources.xaml.
    /// </summary>
    public class LiteralwithResourcesVerifier
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Verify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside LiteralwithResourcesVerifier.Verify()...");

            DockPanel myPanel = uie as DockPanel;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be DockPanel");
            }

            Dock myDock = DockPanel.GetDock(myPanel);
            UIElementCollection myChildren = myPanel.Children;

            if (myChildren.Count != 1)
            {
                throw new Microsoft.Test.TestValidationException("Should has only 1 child");
            }

            //verifying resources

            CoreLogger.LogStatus("Verifying Resources ...");

            ResourceDictionary myResources = myPanel.Resources;

            VerifyElement.VerifyBool(null == myResources, false);
            VerifyElement.VerifyInt(myResources.Count, 1);

            String[] myKeys = new String[1];

            myResources.Keys.CopyTo(myKeys, 0);
            foreach (string key in myKeys)
                CoreLogger.LogStatus("key: " + key);

            CoreLogger.LogStatus("Verify foreground ...");
            if (false == myResources.Contains("foreground"))
                CoreLogger.LogStatus("no resources for foreground");
            else
            {
                Type myType = myResources["foreground"].GetType();

                if (null == myType)
                    CoreLogger.LogStatus("null myResources[foreground]");
                else
                    CoreLogger.LogStatus("Type1: " + myType.FullName);
            }

            SolidColorBrush myForeground = myResources["foreground"] as SolidColorBrush;

            VerifyElement.VerifyBool(null == myForeground, false);
            VerifyElement.VerifyColor(myForeground.Color, Colors.Red);

            //verifying node

            UIElement child = myChildren[0];
            NodeForLiteralwithResources nodeWithLiteral = child as NodeForLiteralwithResources;

            if (null == nodeWithLiteral)
            {
                throw new Microsoft.Test.TestValidationException("Should HaveNodeForLiteralwithResources");
            }

            if(((SolidColorBrush)(nodeWithLiteral.MyText.Foreground)).Color 
                != ((SolidColorBrush)Brushes.Red).Color)
            {
                throw new Microsoft.Test.TestValidationException("Foreground should be"
                    + ((SolidColorBrush)Brushes.Red).Color.ToString()
                    +", actually: " .ToString()
                    + ((SolidColorBrush)(nodeWithLiteral.MyText.Foreground)).Color.ToString());
            }
        }
    }
}
