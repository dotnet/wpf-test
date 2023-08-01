// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Windows.Media;
using System.ComponentModel;

namespace Avalon.Test.CoreUI.Resources
{
    /// <summary>
    /// All property engine verifiers in this class
    /// </summary>
    public static partial class Verifiers
    {
        /// <summary>
        /// Used to verify a series of PropertyTrigger-related test cases
        /// </summary>
        /// <param name="root">Root element of the VisualTree</param>
        /// <returns>true when verification completes, false otherwise.</returns>
        public static bool MergedDictionaryVerifier(StackPanel root)
        {

            VerifyDataTemplates(root);
            VerifyControlTemplates(root);
            VerifyStyle(root);

            return true;
        }

        private static void VerifyDataTemplates(StackPanel root)
        {
            CoreLogger.LogStatus("Verifying DataTemplate Lookup in external MergedDictionary: ");

            ListBox lb1 = (ListBox)root.FindName("lb1");

            System.Xml.XmlElement df = lb1.Items[0] as System.Xml.XmlElement;
            ListBoxItem ds = (ListBoxItem)lb1.ItemContainerGenerator.ContainerFromItem(df);

            Border bord1 = (Border)VisualTreeHelper.GetChild(ds, 0);
            ContentPresenter cp = bord1.Child as ContentPresenter;

            StackPanel sp1 = (StackPanel)VisualTreeHelper.GetChild(cp, 0);
            TextBlock tb1 = sp1.Children[0] as TextBlock;


            if (tb1.Text != "East of Eden")
            {
                throw new Microsoft.Test.TestValidationException("Data Template not picked up correctly from external resource dictionary.");
            }
            else
            {
                CoreLogger.LogStatus("Ok", ConsoleColor.Green);
            }
        }

        private static void VerifyControlTemplates(StackPanel root)
        {
            CoreLogger.LogStatus("Verifying ControlTemplate Lookup in external MergedDictionary: ");

            Button b1 = (Button)root.FindName("b1");
            ControlTemplate ct = b1.Template;
            Button cp = (Button)ct.FindName("CP", b1);

            Brush actual = (Brush)cp.Background;

            CoreLogger.LogStatus(actual.ToString());

            if (actual != Brushes.Green)
            {
                throw new Microsoft.Test.TestValidationException("Control Template not picked up correctly from external resource dictionary.");
            }
            else
            {
                CoreLogger.LogStatus("Ok", ConsoleColor.Green);
            }
        }

        private static void VerifyStyle(StackPanel root)
        {
            CoreLogger.LogStatus("Verifying DataTemplate Lookup in external MergedDictionary: ");

            Button b2 = (Button)root.FindName("b2");
            Brush actual = (Brush) b2.Background;
            
            
            if (actual != Brushes.Blue)
            {
                throw new Microsoft.Test.TestValidationException("Style not picked up correctly from external resource dictionary.");
            }
            else
            {
                CoreLogger.LogStatus("Ok", ConsoleColor.Green); 
            }
        }

    }
}
