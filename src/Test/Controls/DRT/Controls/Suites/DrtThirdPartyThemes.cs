// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;

namespace DRT
{
    public class DrtThirdPartyThemes : DrtTestSuite
    {
        public DrtThirdPartyThemes() : base("ThirdPartyThemes")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            DRT.LoadXamlFile("ThirdPartyThemes.xaml");

            if (!DRT.KeepAlive)
            {
                return new DrtTest[]
                    {
                        new DrtTest(VerifyProperties),
                        new DrtTest(Shared),
                    };
            }
            else
            {
                return new DrtTest[] { };
            }
        }

        private void VerifyProperties()
        {
            FrameworkElement button1 = (FrameworkElement)DRT.FindElementByID("Button1");
            FrameworkElement button2 = (FrameworkElement)DRT.FindElementByID("Button2");
            TextBlock crkText = (TextBlock)DRT.FindElementByID("CRKTest");

            DRT.Assert(button1 != null, "Button1 is null");
            DRT.Assert(button2 != null, "Button2 is null");
            DRT.Assert(crkText != null, "CRKTest is null");

            DRT.Assert("Style1".Equals(button1.Tag), "Button1 was not styled");
            DRT.Assert("Style2".Equals(button2.Tag), "Button2 was not styled");
            DRT.Assert("CRKString".Equals(crkText.Text), "ComponentResourceKey lookup failed");
        }

        private void Shared()
        {
            FrameworkElement o = (FrameworkElement)DRT.FindElementByID("Button1");
            Type thirdPartyButton = o.GetType();
            ComponentResourceKey sharedBrushKey = new ComponentResourceKey(thirdPartyButton, "SharedBrush");
            ComponentResourceKey notSharedBrushKey = new ComponentResourceKey(thirdPartyButton, "NotSharedBrush");
            ComponentResourceKey notSharedUIElementKey = new ComponentResourceKey(thirdPartyButton, "NotSharedUIElement");

            Brush sharedBrush1 = (Brush)o.FindResource(sharedBrushKey);
            Brush sharedBrush2 = (Brush)o.FindResource(sharedBrushKey);
            Brush notSharedBrush1 = (Brush)o.FindResource(notSharedBrushKey);
            Brush notSharedBrush2 = (Brush)o.FindResource(notSharedBrushKey);
            UIElement notSharedUIElement1 = (UIElement)o.FindResource(notSharedUIElementKey);
            UIElement notSharedUIElement2 = (UIElement)o.FindResource(notSharedUIElementKey);

            DRT.Assert(sharedBrush1 != null, "SharedBrush (1) not found");
            DRT.Assert(sharedBrush2 != null, "SharedBrush (2) not found");
            DRT.Assert(notSharedBrush1 != null, "NotSharedBrush (1) not found");
            DRT.Assert(notSharedBrush2 != null, "NotSharedBrush (2) not found");
            DRT.Assert(notSharedUIElement1 != null, "NotSharedUIElement (1) not found");
            DRT.Assert(notSharedUIElement2 != null, "NotSharedUIElement (2) not found");

            DRT.Assert(Object.ReferenceEquals(sharedBrush1, sharedBrush2), "SharedBrush reference equality failed");
            DRT.Assert(!Object.ReferenceEquals(notSharedBrush1, notSharedBrush2), "NotSharedBrush are reference equal and shouldn't be");
            DRT.Assert(!Object.ReferenceEquals(notSharedUIElement1, notSharedUIElement2), "NotSharedUIElement are reference equal and shouldn't be");
        }
    }
}
