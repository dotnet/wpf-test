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

    ElementHost elementHost;
    SWF.Form form;
    SWC.Button button;
    public override DrtTest[] PrepareTests()
    {
        form = new SWF.Form();
        elementHost = new ElementHost();
        button = new SWC.Button();
        button.Content = "Avalon button";
        elementHost.Child = button;
        form.Controls.Add(elementHost);
        form.Show();

        return new DrtTest[] 
            {
                new DrtTest(TestVisibility),
                new DrtTest(TestResize)
            };
    }

    private void TestVisibility()
    {
        elementHost.Visible = false;
        DRT.Assert(!elementHost.Child.IsVisible, "PropertyMapping didn't work for ElementHost visible (false)");
        elementHost.Visible = true;
        DRT.Assert(elementHost.Child.IsVisible, "PropertyMapping didn't work for ElementHost visible (true)");
    }

    private void TestResize()
    {
        elementHost.AutoSize = true;
        form.PerformLayout();
        SWF.Application.DoEvents();
        double originalWidth = button.ActualWidth;
        form.PerformLayout();
        button.Content += " with a bunch of text";
        SWF.Application.DoEvents();
        DRT.Assert(button.ActualWidth > originalWidth, "ElementHost layout issue: Width didn't grow as hosted element grew");

        elementHost.AutoSize = false;
        elementHost.Width = 150;
        form.PerformLayout();
        SWF.Application.DoEvents();
        originalWidth = button.ActualWidth;
        form.PerformLayout();
        button.Content += " and even more text";
        SWF.Application.DoEvents();
        DRT.AssertEqual(originalWidth, button.ActualWidth, "ElementHost layout issue: Width grew when host's Width was explicitly set");
    }
}