// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DRT
{
    public class DrtFocusVisualSuite : DrtTestSuite
    {
        public DrtFocusVisualSuite() : base("FocusVisual")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            DRT.LoadXamlFile("FocusVisual.xaml");

            _b1 = DRT.FindElementByID("b1") as Button;
            _b2 = DRT.FindElementByID("b2") as Button;
            _cb1 = DRT.FindElementByID("cb1") as CheckBox;
            _textblock = DRT.FindElementByID("textblock") as TextBlock;
            _hl = DRT.FindElementByID("hl") as Hyperlink;

            DRT.Assert(_b1 != null, "Couldn't find b1 in the tree");
            DRT.Assert(_b2 != null, "Couldn't find b2 in the tree");
            DRT.Assert(_cb1 != null, "Couldn't find cb1 in the tree");
            DRT.Assert(_textblock != null, "Couldn't find textblock in the tree");
            DRT.Assert(_hl != null, "Couldn't find hl in the tree");

            _b1.Content = "Test started";

            if (!DRT.KeepAlive)
            {
                return new DrtTest[]
                {
                    new DrtTest(VerifyInit),
                    new DrtTest(PressTab),
                    new DrtTest(VerifyFocusVisual),
                    new DrtTest(PressTab),
                    new DrtTest(VerifyFocusVisual),
                    new DrtTest(PressTab),
                    new DrtTest(VerifyFocusVisual),
                    new DrtTest(PressTab),
                    new DrtTest(VerifyFocusVisual),
                };
            }
            else
            {
                return new DrtTest[] { };
            }
        }

        Button _b1;
        Button _b2;
        CheckBox _cb1;
        TextBlock _textblock;
        Hyperlink _hl;
        UIElement _active;
        int _step;

        private void PressTab()
        {
            DRT.PressKey(Key.Tab);
            _step++;
            if (_step == 1)
                _active = _b1;
            if (_step == 2)
                _active = _b2;
            if (_step == 3)
                _active = _cb1;
            if (_step == 4)
                _active = _textblock;
        }

        private void VerifyInit()
        {
            AdornerLayer adornerlayer = AdornerLayer.GetAdornerLayer(_b1);
            DRT.Assert(adornerlayer != null, "AdornerLayer is null. It should be valid if AdornerDecorator exist in the parent chain");
            if (adornerlayer == null)
                return;

            Adorner[] adorners = adornerlayer.GetAdorners(_b1);
            DRT.Assert(adorners == null, "AdornerLayer should not have any Adorners");

            _step = 0;
        }

        private void VerifyFocusVisual()
        {
            AdornerLayer adornerlayer = AdornerLayer.GetAdornerLayer(_active);
            DRT.Assert(adornerlayer != null, "AdornerLayer is null. It should be valid if AdornerDecorator exist in the parent chain");
            if (adornerlayer == null)
                return;

            Adorner[] adorners = adornerlayer.GetAdorners(_active);
            DRT.Assert(adorners != null, "GetAdorners should not return null");
            DRT.Assert(adorners.Length == 1, "AdornerLayer should have one Adorner");
            Adorner adorner = adorners[0];
            DRT.Assert(adorner.AdornedElement == _active, "AdornedElement should be " + _active + ", was " + adorner.AdornedElement);
        }

  
    }
}
