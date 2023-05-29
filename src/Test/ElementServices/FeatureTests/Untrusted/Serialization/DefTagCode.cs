// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;

using System.Windows;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Common;
using System.Collections;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Def tag verification
    /// </summary>
    public class DefTagCode
    {

        /// <summary>
        /// 
        /// </summary>
        public static void Verify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside DefTagCode.Verify()...");
            
            DockPanel myPanel = uie as DockPanel;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be DockPanel");
            }

            if (!myPanel.LastChildFill)
            {
                throw new Microsoft.Test.TestValidationException("LastChildFill should be true");
            }
            else
            {
                 CoreLogger.LogStatus("Dockpanel OK");
            }

            UIElementCollection myChildren = myPanel.Children;

            if (myChildren.Count != 1)
            {
                throw new Microsoft.Test.TestValidationException("Should has only 1 child");
            }

            UIElement child = myChildren[0];

            Button myButton = child as Button;

            if (null == myButton)
            {
                throw new Microsoft.Test.TestValidationException("Should Have a button");
            }

            if (myButton.FontSize != 72)
            {
              throw new Microsoft.Test.TestValidationException("myButton.FontSize should be 200");
            }

            String myText = ((ContentControl)myButton).Content as String;
            if (0 != String.Compare(myText.Trim(), "ClickMe", true))
            {
                throw new Microsoft.Test.TestValidationException("Text should be ClickMe, actually: >>" + myText);
            }

        }
    }
}
