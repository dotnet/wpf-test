using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

//
// Testcase:    TopLevelForm
// Description: Verify that a TopLevel Form cannot be added to a WFH
// Author:      a-wboyde
//
namespace WindowsFormsHostTests
{

public class TopLevelForm : WPFReflectBase
{
    #region Testcase setup
    public TopLevelForm(string[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        base.InitTest(p);
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Verify that an exception is thrown when a toplevel form is added to a WFH")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // create panel for main window
        StackPanel sp = new StackPanel();
        this.Content = sp;


        // create first host with simple WF control
        p.log.WriteLine("");
        p.log.WriteLine("Adding WFH with WF control");
        WindowsFormsHost wfh1 = new WindowsFormsHost();
        sp.Children.Add(wfh1);

        // add WF button to WFH
        System.Windows.Forms.Button wfBtn = new System.Windows.Forms.Button();
        wfBtn.Text = "Winforms Button";
        wfh1.Child = wfBtn;


        // create second host to try to add Toplevel Form to
        p.log.WriteLine("");
        p.log.WriteLine("Trying to add WFH with TopLevel WF Form");
        WindowsFormsHost wfh2 = new WindowsFormsHost();
        sp.Children.Add(wfh2);
        System.Windows.Forms.Form wfForm1 = new System.Windows.Forms.Form();
        p.log.WriteLine("wfForm1.TopLevel = {0}", wfForm1.TopLevel);

        // try to add Toplevel Form
        try
        {
            wfh2.Child = wfForm1;
            WPFMiscUtils.IncCounters(sr, p.log, false, "Should not be able to add TopLevel");
        }
        catch (System.ArgumentException e)
        {
            // got expected exception
            p.log.WriteLine("Was not able to add toplevel form to host (as expected) - got exception:");
            p.log.WriteLine(e.Message);
            sr.IncCounters(true);
        }
        catch (Exception e)
        {
            // got exception, but wrong one
            p.log.WriteLine(e.Message);
            sr.IncCounters(false);
        }


        // create third host to try to add Non-Toplevel Form to
        p.log.WriteLine("");
        p.log.WriteLine("Trying to add WFH with Non-TopLevel WF Form");
        WindowsFormsHost wfh3 = new WindowsFormsHost();
        sp.Children.Add(wfh3);
        System.Windows.Forms.Form wfForm2 = new System.Windows.Forms.Form();
        wfForm2.TopLevel = false;
        p.log.WriteLine("wfForm2.TopLevel = {0}", wfForm2.TopLevel);

        // try to add Non-Toplevel Form
        try
        {
            wfh3.Child = wfForm2;
            p.log.WriteLine("Was able to add non-toplevel form to host");
            sr.IncCounters(true);
        }
        catch (Exception e)
        {
            // got exception
            p.log.WriteLine(e.Message);
            sr.IncCounters(false);
        }

        return sr;
    }

    #endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify that an exception is thrown when a toplevel form is added to a WFH
