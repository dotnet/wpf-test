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


/// <summary>
///     This sets up a simple hosting scenario with a WinForms button, check some basic layout and property mapping.
/// </summary>
public class DrtWindowsFormsIntegration : DrtBase
{   
    [STAThread]
    public static int Main(string[] args)
    {
        DrtBase drt = new DrtWindowsFormsIntegration();
        return drt.Run(args);
    }

    private DrtWindowsFormsIntegration() 
    {
        WindowTitle = "WindowsFormsIntegration DRT";
        Contact = "scoberry";
        TeamContact = "uifxcdev";
        DrtName = "DrtWindowsFormsIntegration";
        Suites = new DrtTestSuite[] 
        {
            new WindowsFormsHostTestSuite(),
            new ElementHostTestSuite()
        };
    }
}