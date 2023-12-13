// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections;

namespace Microsoft.Test.Graphics
{
    public partial class GraphicsStress : Window
    {
        private void TabBrowsingAction()
        {
            ArrayList tabItems = new ArrayList();
            foreach (TabItem tab in Tab1.Items)
            {
                if (tab != null && tab is TabItem)
                {
                    tabItems.Add(tab);
                }
            }
            if (tabItems.Count == 0)
            {
                throw new System.ApplicationException("Xaml fails to load");
            }
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < tabItems.Count; j++)
                {
	            //Setting the Timeout between each action to be 3 seconds.
        	    //Longer timeout is needed because this tests involve a lot of Mouse movement
                    XamlTestHelper.AddStep(ClickTab, tabItems[j], 3000);
                }
            }
        }

        private object ClickTab(object tab)
        {
            if (tab == null)
            {
                throw new System.ApplicationException("tab should not be null in ClickTab");
            }


            //Bounding box of the element relative to the screen coordinate.
            Rect relativeBound = Input.GetScreenRelativeRect((UIElement)tab);

            //HitPoint position relative to the screen coordinate.
            Point hitpoint = new Point(relativeBound.TopLeft.X + relativeBound.Width / 2,
                            relativeBound.TopLeft.Y + relativeBound.Height / 2);

            XamlTestHelper.LogStatus("Current hitpoint = {" + hitpoint + "}");

            Input.ClickScreenPoint((int)(hitpoint.X), (int)(hitpoint.Y));
            return null;
        }
    }
}
