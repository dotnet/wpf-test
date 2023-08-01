// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Holds verification routines for xaml-based Enum syntax support tests.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: $
 
 * Filename:         $Source: $
********************************************************************/

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

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Holds verification routines for xaml-based Enum syntax support tests.
    /// </summary>
    public class EnumSyntaxVerifier
    {
        /// <summary>
        /// Verifies Avalon Parser's Enum support for:
        /// &lt;Button DockPanel.Dock="*Dock.Right" /&gt;
        /// </summary>
        public static void Verify1(UIElement root)
        {
            CoreLogger.LogStatus("Inside EnumSyntaxVerifier.Verify1()...");

            //
            // Verify first Button's Dock property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement1'. Should be Button...");

            Button button = (Button)LogicalTreeHelper.FindLogicalNode(root, "TargetElement1");

            Dock myDock = DockPanel.GetDock(button);

            CoreLogger.LogStatus("Verifying first Button's Dock property is Right...");
            if (myDock != Dock.Right)
            {
                throw new Microsoft.Test.TestValidationException("First Button's Dock property != Right.");
            }

            //
            // Verify second Button's Dock property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement2'. Should be Button...");
            button = (Button)LogicalTreeHelper.FindLogicalNode(root, "TargetElement2");

            myDock = DockPanel.GetDock(button);

            CoreLogger.LogStatus("Verifying second Button's Dock property is Bottom...");
            if (myDock != Dock.Bottom)
            {
                throw new Microsoft.Test.TestValidationException("Second Button's Dock property != Bottom.");
            }
        }
        /// <summary>
        /// Verifies Avalon Parser's Enum support for:
        /// &lt;Button cc:CustomEnumControl.CustomDock="*Dock.Bottom" /&gt;
        /// </summary>
        public static void Verify2(UIElement root)
        {
            CoreLogger.LogStatus("Inside EnumSyntaxVerifier.Verify2()...");

            //
            // Verify first Button's Dock property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement1'. Should be Button...");

            Button button = (Button)LogicalTreeHelper.FindLogicalNode(root, "TargetElement1");

            Dock myDock = (Dock)button.GetValue(CustomEnumControl.CustomDockProperty);

            CoreLogger.LogStatus("Verifying first Button's CustomDock property is Right...");
            if (myDock != Dock.Right)
            {
                throw new Microsoft.Test.TestValidationException("First Button's CustomDock property != Right.");
            }

            //
            // Verify second Button's Dock property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement2'. Should be Button...");

            button = (Button)LogicalTreeHelper.FindLogicalNode(root, "TargetElement2");

            myDock = (Dock)button.GetValue(CustomEnumControl.CustomDockProperty);

            CoreLogger.LogStatus("Verifying first Button's CustomDock property is Top...");
            if (myDock != Dock.Bottom)
            {
                throw new Microsoft.Test.TestValidationException("Second Button's CustomDock property != Bottom.");
            }
        }
        /// <summary>
        /// Verifies Avalon Parser's Enum support for:
        /// &lt;Button cc:CustomEnumControl.Custom="*cc:CustomEnum.Value1" /&gt;
        /// </summary>
        public static void Verify3(UIElement root)
        {
            CoreLogger.LogStatus("Inside EnumSyntaxVerifier.Verify3()...");

            //
            // Verify first Button's Custom property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement1'. Should be Button...");

            Button button = (Button)LogicalTreeHelper.FindLogicalNode(root, "TargetElement1");

            CustomEnum myCustomEnum = (CustomEnum)button.GetValue(CustomEnumControl.CustomProperty);

            CoreLogger.LogStatus("Verifying first Button's Custom property is Value1...");
            if (myCustomEnum != CustomEnum.Value1)
            {
                throw new Microsoft.Test.TestValidationException("First Button's Custom property != Value1.");
            }

            //
            // Verify second Button's Custom property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement2'. Should be Button...");

            button = (Button)LogicalTreeHelper.FindLogicalNode(root, "TargetElement2");

            myCustomEnum = (CustomEnum)button.GetValue(CustomEnumControl.CustomProperty);

            CoreLogger.LogStatus("Verifying second Button's Custom property is Value2...");
            if (myCustomEnum != CustomEnum.Value2)
            {
                throw new Microsoft.Test.TestValidationException("Second Button's Custom property != Value2.");
            }
        }

        /// <summary>
        /// Verifies Avalon Parser's Enum support for:
        /// &lt;CustomEnumControl cc:CustomEnumControl.Custom="*cc:CustomEnum.Value1" /&gt;
        /// </summary>
        public static void Verify4(UIElement root)
        {
            CoreLogger.LogStatus("Inside EnumSyntaxVerifier.Verify4()...");

            //
            // Verify first CustomEnumControl's Custom property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement1'. Should be CustomEnumControl...");

            CustomEnumControl customControl = (CustomEnumControl)LogicalTreeHelper.FindLogicalNode(root, "TargetElement1");

            CustomEnum myCustomEnum = (CustomEnum)customControl.GetValue(CustomEnumControl.CustomProperty);

            CoreLogger.LogStatus("Verifying first CustomEnumControl's Custom property is Value1...");
            if (myCustomEnum != CustomEnum.Value1)
            {
                throw new Microsoft.Test.TestValidationException("First CustomEnumControl's Custom property != Value1.");
            }

            //
            // Verify second CustomEnumControl's Custom property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement2'. Should be CustomEnumControl...");

            customControl = (CustomEnumControl)LogicalTreeHelper.FindLogicalNode(root, "TargetElement2");

            myCustomEnum = (CustomEnum)customControl.GetValue(CustomEnumControl.CustomProperty);

            CoreLogger.LogStatus("Verifying second CustomEnumControl's Custom property is Value2...");
            if (myCustomEnum != CustomEnum.Value2)
            {
                throw new Microsoft.Test.TestValidationException("Second CustomEnumControl's Custom property != Value2.");
            }
        }

        /// <summary>
        /// Verifies Avalon Parser's for public static members in property values.  For example:
        /// &lt;Button cc:CustomEnumControl.CustomAttachedBool="*cc:CustomEnumControl.PublicBoolClrProperty" /&gt;
        /// </summary>
        public static void Verify5(UIElement root)
        {
            CoreLogger.LogStatus("Inside EnumSyntaxVerifier.Verify5()...");

            //
            // Verify first Button's CustomAttachedBool property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement1'. Should be Button...");

            Button button = (Button)LogicalTreeHelper.FindLogicalNode(root, "TargetElement1");

            CoreLogger.LogStatus("Verifying first Button's CustomAttachedBool property is false...");
            
            bool boolVal = (bool)button.GetValue(CustomEnumControl.CustomAttachedBoolProperty);

            if (boolVal != false)
            {
                throw new Microsoft.Test.TestValidationException("First Button's CustomAttachedBool property != false.");
            }

            //
            // Verify second Button's CustomAttachedBool property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement2'. Should be Button...");

            button = (Button)LogicalTreeHelper.FindLogicalNode(root, "TargetElement2");

            CoreLogger.LogStatus("Verifying second Button's CustomAttachedBool property is false...");

            boolVal = (bool)button.GetValue(CustomEnumControl.CustomAttachedBoolProperty);

            if (boolVal != false)
            {
                throw new Microsoft.Test.TestValidationException("Second Button's CustomAttachedBool property != false.");
            }
        }

        /// <summary>
        /// Verifies Avalon Parser's support for public static members in property values.  For example:
        /// &lt;CustomEnumControl cc:CustomEnumControl.CustomAttachedBool="*cc:CustomEnumControl.PublicStaticBoolClrProperty" /&gt;
        /// </summary>
        public static void Verify6(UIElement root)
        {
            CoreLogger.LogStatus("Inside EnumSyntaxVerifier.Verify6()...");

            //
            // Verify first CustomEnumControl's CustomAttachedBool property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement1'. Should be CustomEnumControl...");

            CustomEnumControl customControl = (CustomEnumControl)LogicalTreeHelper.FindLogicalNode(root, "TargetElement1");

            bool boolVal = (bool)customControl.GetValue(CustomEnumControl.CustomAttachedBoolProperty);

            CoreLogger.LogStatus("Verifying first CustomEnumControl's CustomAttachedBool property is false...");
            if (boolVal != false)
            {
                throw new Microsoft.Test.TestValidationException("First CustomEnumControl's CustomAttachedBool property != false.");
            }

            //
            // Verify second CustomEnumControl's CustomAttachedBool property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement2'. Should be CustomEnumControl...");

            customControl = (CustomEnumControl)LogicalTreeHelper.FindLogicalNode(root, "TargetElement2");

            boolVal = (bool)customControl.GetValue(CustomEnumControl.CustomAttachedBoolProperty);

            CoreLogger.LogStatus("Verifying second CustomEnumControl's CustomAttachedBool property is false...");
            if (boolVal != false)
            {
                throw new Microsoft.Test.TestValidationException("Second CustomEnumControl's CustomAttachedBool property != false.");
            }
        }

        /// <summary>
        /// Verifies Avalon Parser's support for Enum values and public static
        /// properties and fields in resource references, 
        /// for example:
        /// &lt;Button DockPanel.Dock="{*Dock.Right}" /&gt;
        /// </summary>
        public static void Verify7(UIElement root)
        {
            CoreLogger.LogStatus("Inside EnumSyntaxVerifier.Verify7()...");

            //
            // Verify first Button's Dock property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement1'. Should be Button...");

            Button button = (Button)LogicalTreeHelper.FindLogicalNode(root, "TargetElement1");

            Dock dock = (Dock)button.GetValue(DockPanel.DockProperty);

            CoreLogger.LogStatus("Verifying first Button's Dock property is Right...");
            if (dock != Dock.Right)
            {
                throw new Microsoft.Test.TestValidationException("First Button's Dock property != Right.");
            }

            //
            // Verify second Button's Custom property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement2'. Should be Button...");

            button = (Button)LogicalTreeHelper.FindLogicalNode(root, "TargetElement2");

            CustomEnum customEnum = (CustomEnum)button.GetValue(CustomEnumControl.CustomProperty);

            CoreLogger.LogStatus("Verifying second Button's Custom property is Value2...");
            if (customEnum != CustomEnum.Value2)
            {
                throw new Microsoft.Test.TestValidationException("Second Button's Custom property != Value2.");
            }

            //
            // Verify third Button's CustomAttachedBool property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement3'. Should be Button...");

            button = (Button)LogicalTreeHelper.FindLogicalNode(root, "TargetElement3");

            bool boolVal = (bool)button.GetValue(CustomEnumControl.CustomAttachedBoolProperty);

            CoreLogger.LogStatus("Verifying third Button's CustomAttachedBool property is true...");
            if (boolVal != true)
            {
                throw new Microsoft.Test.TestValidationException("third Button's CustomAttachedBool property != true.");
            }

            //
            // Verify fourth Button's CustomAttachedBool property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement4'. Should be Button...");

            button = (Button)LogicalTreeHelper.FindLogicalNode(root, "TargetElement4");

            boolVal = (bool)button.GetValue(CustomEnumControl.CustomAttachedBoolProperty);

            CoreLogger.LogStatus("Verifying fourth Button's CustomAttachedBool property is true...");
            if (boolVal != true)
            {
                throw new Microsoft.Test.TestValidationException("Fourth Button's CustomAttachedBool property != true.");
            }
        }

        /// <summary>
        /// Verifies Avalon Parser's support for Enum values and public static
        /// properties and fields in resource references, 
        /// for example:
        /// &lt;CustomEnumControl DockPanel.Dock="{*Dock.Right}" /&gt;
        /// </summary>
        public static void Verify8(UIElement root)
        {
            CoreLogger.LogStatus("Inside EnumSyntaxVerifier.Verify8()...");

            //
            // Verify first CustomEnumControl's Dock property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement1'. Should be CustomEnumControl...");

            CustomEnumControl CustomEnumControl = (CustomEnumControl)LogicalTreeHelper.FindLogicalNode(root, "TargetElement1");

            Dock dock = (Dock)CustomEnumControl.GetValue(DockPanel.DockProperty);

            CoreLogger.LogStatus("Verifying first CustomEnumControl's Dock property is Right...");
            if (dock != Dock.Right)
            {
                throw new Microsoft.Test.TestValidationException("First CustomEnumControl's Dock property != Right.");
            }

            //
            // Verify second CustomEnumControl's Custom property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement2'. Should be CustomEnumControl...");

            CustomEnumControl = (CustomEnumControl)LogicalTreeHelper.FindLogicalNode(root, "TargetElement2");

            CustomEnum customEnum = (CustomEnum)CustomEnumControl.GetValue(CustomEnumControl.CustomProperty);

            CoreLogger.LogStatus("Verifying second CustomEnumControl's Custom property is Value2...");
            if (customEnum != CustomEnum.Value2)
            {
                throw new Microsoft.Test.TestValidationException("Second CustomEnumControl's Custom property != Value2.");
            }

            //
            // Verify third CustomEnumControl's CustomAttachedBool property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement3'. Should be CustomEnumControl...");

            CustomEnumControl = (CustomEnumControl)LogicalTreeHelper.FindLogicalNode(root, "TargetElement3");

            bool boolVal = (bool)CustomEnumControl.GetValue(CustomEnumControl.CustomAttachedBoolProperty);

            CoreLogger.LogStatus("Verifying third CustomEnumControl's CustomAttachedBool property is true...");
            if (boolVal != true)
            {
                throw new Microsoft.Test.TestValidationException("third CustomEnumControl's CustomAttachedBool property != true.");
            }

            //
            // Verify fourth CustomEnumControl's CustomAttachedBool property.
            //
            CoreLogger.LogStatus("Getting 'TargetElement4'. Should be CustomEnumControl...");

            CustomEnumControl = (CustomEnumControl)LogicalTreeHelper.FindLogicalNode(root, "TargetElement4");

            boolVal = (bool)CustomEnumControl.GetValue(CustomEnumControl.CustomAttachedBoolProperty);

            CoreLogger.LogStatus("Verifying fourth CustomEnumControl's CustomAttachedBool property is true...");
            if (boolVal != true)
            {
                throw new Microsoft.Test.TestValidationException("Fourth CustomEnumControl's CustomAttachedBool property != true.");
            }
        }
    }
}
