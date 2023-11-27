// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms.Integration;
using System.Windows;
using System.Windows.Threading;

using SD = System.Drawing;
using SW = System.Windows;
using SWC = System.Windows.Controls;
using SWM = System.Windows.Media;
using SWF = System.Windows.Forms;
using SWS = System.Windows.Markup;
using SWA = System.Windows.Automation;
using SWI = System.Windows.Input;

using DRT;


public sealed class ElementHostTestSuite : DrtTestSuite
{
    public ElementHostTestSuite() : base("ElementHostTestSuite") { }

    ElementHost _elementHost;
    SWF.Form _form;
    SWC.Button _button;
    public override DrtTest[] PrepareTests()
    {
        _form = new SWF.Form();
        _elementHost = new ElementHost();
        _button = new SWC.Button();
        _button.Content = "Avalon button";
        _elementHost.Child = _button;
        _form.Controls.Add(_elementHost);
        _form.Show();

        return new DrtTest[] 
            {
                new DrtTest(TestVisibility),
                new DrtTest(TestResize)
            };
    }

    private void TestVisibility()
    {
        _elementHost.Visible = false;
        DRT.Assert(!_elementHost.Child.IsVisible, "PropertyMapping didn't work for ElementHost visible (false)");
        _elementHost.Visible = true;
        DRT.Assert(_elementHost.Child.IsVisible, "PropertyMapping didn't work for ElementHost visible (true)");
    }

    private void TestResize()
    {
        _elementHost.AutoSize = true;
        _form.PerformLayout();
        SWF.Application.DoEvents();
        double originalWidth = _button.ActualWidth;
        _form.PerformLayout();
        _button.Content += " with a bunch of text";
        SWF.Application.DoEvents();
        DRT.Assert(_button.ActualWidth > originalWidth, "ElementHost layout issue: Width didn't grow as hosted element grew");

        _elementHost.AutoSize = false;
        _elementHost.Width = 150;
        _form.PerformLayout();
        SWF.Application.DoEvents();
        originalWidth = _button.ActualWidth;
        _form.PerformLayout();
        _button.Content += " and even more text";
        SWF.Application.DoEvents();
        DRT.AssertEqual(originalWidth, _button.ActualWidth, "ElementHost layout issue: Width grew when host's Width was explicitly set");
    }
}
