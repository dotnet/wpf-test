using System;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Reflection;
using SWC = System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using SWF = System.Windows.Forms;
using System.Windows.Controls;

///
/// <TestCase>
/// Enabled
/// </TestCase>
/// <summary>
/// Verify that the WF's Enabled Property works
/// </summary>
/// <history>
///  [sameerm]   3/28/2006   Created
///  [sameerm]   3/30/2006   Incorporated review feedback.
/// </history>
///
///
public class Enabled : ReflectBase
{

    private SWC.TextBox _avTxtBox;
    private ElementHost _eh1;

    #region Testcase setup
    public Enabled(string[] args) : base(args) { }


    protected override void InitTest(TParams p)
    {
        this.Size = new System.Drawing.Size(500, 500);
        base.InitTest(p);
    }
    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        return base.BeforeScenario(p, scenario);
    }
    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        Controls.Clear();
        _avTxtBox = null;
        _eh1 = null;
        this.Text = "";
        base.AfterScenario(p, scenario, result);
    }

    [Scenario("@ EH with single control.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        try
        {
            _avTxtBox = new SWC.TextBox();
            _avTxtBox.Text = "I am a TextBox";
            //Add the ElementHost control
            _eh1 = new ElementHost();
            _eh1.Child = _avTxtBox;
            _eh1.Size = new System.Drawing.Size(200, 200);
            _eh1.BackColor = System.Drawing.Color.Crimson;
            _eh1.Child = _avTxtBox;
            this.Controls.Add(_eh1);

            bool originallyEnabled = _eh1.Enabled;
            bool newEnabled = !originallyEnabled;

            _eh1.Enabled = newEnabled;
            this.Text = newEnabled.ToString();
            Utilities.SleepDoEvents(5, 100);
            if (_eh1.Enabled == originallyEnabled || _avTxtBox.IsEnabled == originallyEnabled)
            {
                p.log.WriteLine(String.Format("Enabled property on EH. Expected: {0}, Actual: {1} ", newEnabled, _eh1.Enabled.ToString()));
                p.log.WriteLine(String.Format("Enabled property on AvButton. Expected: {0}, Actual: {1} ", newEnabled, _avTxtBox.IsEnabled.ToString()));
                sr.IncCounters(false, "Enabled failed", p.log);
            }
            else
            {
                //Reset it back again.
                _eh1.Enabled = originallyEnabled;
                this.Text = originallyEnabled.ToString();
                Utilities.SleepDoEvents(5, 100);
                SWF.Application.DoEvents();
                if (_eh1.Enabled != originallyEnabled &&  _avTxtBox.IsEnabled != originallyEnabled)
                {
                    p.log.WriteLine(String.Format("Enabled property on EH. Expected: {0}, Actual: {1} ", originallyEnabled, _eh1.Enabled.ToString()));
                    p.log.WriteLine(String.Format("Enabled property on AvButton. Expected: {0}, Actual: {1} ", originallyEnabled, _avTxtBox.IsEnabled.ToString()));
                    sr.IncCounters(false, "Enabled failed", p.log);
                        sr.IncCounters(false, String.Format("Allow drop property did not change. Expected: {0}, Actual: {1} ", originallyEnabled, _eh1.Cursor.ToString()), p.log);
                    }
                
                sr.IncCounters(true);
            }
        }
        catch (Exception ex)
        {
            log.WriteLine("Exception occured: " + ex.ToString());
        }
        Utilities.SleepDoEvents(1, 1000);
        return sr;
    }


    [Scenario("EH with composite control")]
    public ScenarioResult Scenario2(TParams p)
    {

        ScenarioResult sr = new ScenarioResult();
        try
        {
            StackPanel stkPanel = new StackPanel();

            System.Windows.Controls.Button btn = new SWC.Button();
            btn.Content = "I am a TextBox";

            SWC.ListBox list = new SWC.ListBox();
            list.Items.Add("One");
            list.Items.Add("Two");

            SWC.Label lbl = new SWC.Label();
            lbl.Content = "I am a Label";

            stkPanel.Children.Add(btn);
            stkPanel.Children.Add(list);
            stkPanel.Children.Add(lbl);

            //Add the ElementHost control
            _eh1 = new ElementHost();
            _eh1.Child = stkPanel;
            _eh1.Size = new System.Drawing.Size(200, 200);
            _eh1.BackColor = System.Drawing.Color.Crimson;
            this.Controls.Add(_eh1);

            bool originallyEnabled = _eh1.Enabled;
            bool newEnabled = !originallyEnabled;

            _eh1.Enabled = newEnabled;
            this.Text = _eh1.Enabled.ToString();
            Utilities.SleepDoEvents(5, 100);
            if (_eh1.Enabled == originallyEnabled || stkPanel.IsEnabled == originallyEnabled || btn.IsEnabled == originallyEnabled || list.IsEnabled == originallyEnabled
                      || lbl.IsEnabled == originallyEnabled)
            {
                p.log.WriteLine(String.Format("Enabled property on EH. Expected: {0}, Actual: {1} ", newEnabled, _eh1.Enabled.ToString()));
                p.log.WriteLine(String.Format("Enabled property on StackPanel. Expected: {0}, Actual: {1} ", newEnabled, stkPanel.IsEnabled.ToString()));
                p.log.WriteLine(String.Format("Enabled property on StackPanel. Expected: {0}, Actual: {1} ", newEnabled, stkPanel.IsEnabled.ToString()));
                p.log.WriteLine(String.Format("Enabled property on Button. Expected: {0}, Actual: {1} ", newEnabled, btn.IsEnabled.ToString()));
                p.log.WriteLine(String.Format("Enabled property on ListBox. Expected: {0}, Actual: {1} ", newEnabled, list.IsEnabled.ToString()));
                p.log.WriteLine(String.Format("Enabled property on Label. Expected: {0}, Actual: {1} ", newEnabled, lbl.IsEnabled.ToString()));
                sr.IncCounters(false, "Enabled failed", p.log);
                
            }
            else
            {
                //Reset it back again.
                _eh1.Enabled = originallyEnabled;
                this.Text = _eh1.Enabled.ToString();
                Utilities.SleepDoEvents(5, 100);
                if (_eh1.Enabled != originallyEnabled || stkPanel.IsEnabled != originallyEnabled 
                    || btn.IsEnabled != originallyEnabled || list.IsEnabled != originallyEnabled || lbl.IsEnabled != originallyEnabled)
                {
                    p.log.WriteLine(String.Format("Enable property  ElementHost. Expected: {0}, Actual: {1} ", originallyEnabled, _eh1.Enabled.ToString()));
                    p.log.WriteLine(String.Format("Enable property. StackPanel   Expected: {0}, Actual: {1} ", originallyEnabled, stkPanel.IsEnabled.ToString()));
                    p.log.WriteLine(String.Format("Enable property. Button   Expected: {0}, Actual: {1} ", originallyEnabled, btn.IsEnabled.ToString()));
                    p.log.WriteLine(String.Format("Enable property. List Expected: {0}, Actual: {1} ", originallyEnabled, list.IsEnabled.ToString()));
                    p.log.WriteLine(String.Format("Enable property. Label Expected: {0}, Actual: {1} ", originallyEnabled, lbl.IsEnabled.ToString()));
                    sr.IncCounters(false, "Enabled failed" , p.log);
                    
                }
                sr.IncCounters(true);
            }
        }
        catch (Exception ex)
        {
            log.WriteLine("Exception occured: " + ex.ToString());
        }
        return sr;
    }

    [Scenario("EH control Enabled transitions (true -> true, true->false, false ->true, false, false)")]
    public ScenarioResult Scenario3(TParams p)
    {

        ScenarioResult sr = new ScenarioResult();
        try
        {
            _avTxtBox = new SWC.TextBox();
            _avTxtBox.Text = "I am a TextBox";
            //Add the ElementHost control
            _eh1 = new ElementHost();
            _eh1.Size = new System.Drawing.Size(200, 200);
            _eh1.BackColor = System.Drawing.Color.Cyan;
            _eh1.Child = _avTxtBox;
            this.Controls.Add(_eh1);


            _eh1.Enabled = true;
            this.Text = _eh1.Enabled.ToString();
            Utilities.SleepDoEvents(1, 500);
            sr.IncCounters(_eh1.Enabled == true, String.Format("Enabled property did not change on EH. Expected: {0}, Actual: {1} ", true.ToString(), _eh1.Enabled.ToString()), p.log);
            sr.IncCounters(_avTxtBox.IsEnabled == true, String.Format("Enabled property did not change on Hosted Control. Expected: {0}, Actual: {1} ", true.ToString(), _avTxtBox.IsEnabled.ToString()), p.log);

            _eh1.Enabled = false;
            this.Text = _eh1.Enabled.ToString();
            Utilities.SleepDoEvents(1, 500);
            sr.IncCounters(_eh1.Enabled == false, String.Format("Enabled property did not change on EH. Expected: {0}, Actual: {1} ", false.ToString(), _eh1.Enabled.ToString()), p.log);
            sr.IncCounters(_avTxtBox.IsEnabled == false, String.Format("Enabled property did not change on Hosted Control. Expected: {0}, Actual: {1} ", false.ToString(), _avTxtBox.IsEnabled.ToString()), p.log);

            _eh1.Enabled = true;
            this.Text = _eh1.Enabled.ToString();
            Utilities.SleepDoEvents(1, 500);
            sr.IncCounters(_eh1.Enabled == true, String.Format("Enabled property did not change. Expected: {0}, Actual: {1} ", true.ToString(), _eh1.Enabled.ToString(), p.log), p.log);
            sr.IncCounters(_avTxtBox.IsEnabled == true, String.Format("Enabled property did not change on Hosted Control. Expected: {0}, Actual: {1} ", true.ToString(), _avTxtBox.IsEnabled.ToString()), p.log);

            _eh1.Enabled = false;
            this.Text = _eh1.Enabled.ToString();
            Utilities.SleepDoEvents(1, 500);
            sr.IncCounters(_eh1.Enabled == false, String.Format("Enabled property did not change. Expected: {0}, Actual: {1} ", false.ToString(), _eh1.Enabled.ToString(), p.log), p.log);
            sr.IncCounters(_avTxtBox.IsEnabled == false, String.Format("Enabled property did not change on Hosted Control. Expected: {0}, Actual: {1} ", false.ToString(), _avTxtBox.IsEnabled.ToString()), p.log);
        }
        catch (Exception ex)
        {
            log.WriteLine("Exception occured: " + ex.ToString());
        }
        return sr;
    }

    #endregion
    //==========================================
    // Scenarios
    //==========================================

}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ EH with single control

//@ EH with complex control

//@ EH control with Enabled transitions (true -> true, true->false, false ->true, false, false)