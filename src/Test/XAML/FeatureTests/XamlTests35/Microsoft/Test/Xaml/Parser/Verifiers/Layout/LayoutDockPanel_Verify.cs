// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Layout
{
    /// <summary/>
    public static class LayoutDockPanel_Verify
    {
        /// <summary>
        /// Verification method for Dockpanel.xaml from layout group
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;
            GlobalLog.LogStatus("Inside LayoutXamlVerifiers.DockPanelVerify()...");

            DockPanel myPanel = rootElement as DockPanel;

            if (null == myPanel)
            {
                GlobalLog.LogEvidence("Should be DockPanel");
                result = false;
            }

            UIElementCollection myChildren = myPanel.Children;

            if (myChildren.Count != 5)
            {
                GlobalLog.LogEvidence("Should have 5 children");
                result = false;
            }
            else
            {
                GlobalLog.LogStatus("Number of children OK");
            }


            DockPanel dockpanel1 = (DockPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "DockPanel1");
            DockPanel dockpanel2 = (DockPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "DockPanel2");
            DockPanel dockpanel3 = (DockPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "DockPanel3");
            DockPanel dockpanel4 = (DockPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "DockPanel4");
            DockPanel dockpanel5 = (DockPanel) LogicalTreeHelper.FindLogicalNode(rootElement, "DockPanel5");


            if (null == dockpanel1
                || null == dockpanel2
                || null == dockpanel3
                || null == dockpanel4
                || null == dockpanel5)
            {
                GlobalLog.LogEvidence("some Child Dockpanels are missed");
                result = false;
            }

            //Verify root panel
            if (!Color.Equals(((SolidColorBrush)(((Panel) myPanel).Background)).Color, Colors.Yellow))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Panel)myPanel).Background)).Color, Colors.Yellow)");
                result = false;
            }

            // backgroud for children
            if (!Color.Equals(((SolidColorBrush)(((Panel) dockpanel1).Background)).Color, Colors.Gray))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Panel)dockpanel1).Background)).Color, Colors.Gray)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(((Panel) dockpanel2).Background)).Color, Colors.RoyalBlue))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Panel)dockpanel2).Background)).Color, Colors.RoyalBlue)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(((Panel) dockpanel3).Background)).Color, Colors.Gray))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Panel)dockpanel3).Background)).Color, Colors.Gray)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(((Panel) dockpanel4).Background)).Color, Colors.Gray))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Panel)dockpanel4).Background)).Color, Colors.Gray)");
                result = false;
            }
            if (!Color.Equals(((SolidColorBrush)(((Panel) dockpanel5).Background)).Color, Colors.LightGray))
            {
                GlobalLog.LogEvidence("!Color.Equals(((SolidColorBrush)(((Panel)dockpanel5).Background)).Color, Colors.LightGray)");
                result = false;
            }

            //size  for children
            if (((FrameworkElement) dockpanel1).Height != 100)
            {
                GlobalLog.LogEvidence("((FrameworkElement)dockpanel1).Height != 100");
                result = false;
            }
            if (((FrameworkElement) dockpanel2).Width != 200)
            {
                GlobalLog.LogEvidence("((FrameworkElement)dockpanel2).Width != 200");
                result = false;
            }
            if (((FrameworkElement) dockpanel4).Height != 100)
            {
                GlobalLog.LogEvidence("((FrameworkElement)dockpanel4).Height != 100");
                result = false;
            }
            if (((FrameworkElement) dockpanel3).Width != 50)
            {
                GlobalLog.LogEvidence("((FrameworkElement)dockpanel3).Width != 50");
                result = false;
            }

            //Verify Dock
            if (DockPanel.GetDock(dockpanel1) != Dock.Top)
            {
                GlobalLog.LogEvidence("DockPanel.GetDock(dockpanel1) != Dock.Top");
                result = false;
            }
            if (DockPanel.GetDock(dockpanel2) != Dock.Left)
            {
                GlobalLog.LogEvidence("DockPanel.GetDock(dockpanel2)!= Dock.Left");
                result = false;
            }
            if (DockPanel.GetDock(dockpanel3) != Dock.Right)
            {
                GlobalLog.LogEvidence("DockPanel.GetDock(dockpanel3) != Dock.Right");
                result = false;
            }
            if (DockPanel.GetDock(dockpanel4) != Dock.Bottom)
            {
                GlobalLog.LogEvidence("DockPanel.GetDock(dockpanel4) != Dock.Bottom");
                result = false;
            }

            return result;
        }
    }
}
