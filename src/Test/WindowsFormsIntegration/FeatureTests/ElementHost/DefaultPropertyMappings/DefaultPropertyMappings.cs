using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using System.Windows.Controls;
using SWC = System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

/// <TestCase>
/// DefaultPropertyMappings
/// </TestCase>
/// <summary>
/// Test default Property Mappings for Element Host Control
/// </summary>
/// <history>
///  [sameerm]   3/15/2006   Created
///  [sameerm]   3/23/2006   Inc code review suggestions
///  [sameerm]   3/24/2006   API change PropertyMapDictionary was removed.
///  [sameerm]   3/28/2006   Removed ForeColor from Default props
///  [sameerm]   4/2/2006    Remove some property mappings.
/// </history>


public class PropertyMappings : ReflectBase
{
    #region Testcase setup
    public PropertyMappings(string[] args) : base(args) { }

    [STAThread]
    public static void Main(string[] args)
    {
        System.Windows.Forms.Application.Run(new PropertyMappings(args));
    }

    protected override void InitTest(TParams p)
    {
        base.InitTest(p);
        //defaultProps.Add("AllowDrop");
        defaultProps.Add("BackColor");
        defaultProps.Add("BackgroundImage");
        defaultProps.Add("BackgroundImageLayout");
        defaultProps.Add("Cursor");
        defaultProps.Add("Enabled");
        defaultProps.Add("Font");
        defaultProps.Add("ImeMode");
        defaultProps.Add("RightToLeft");
        defaultProps.Add("Visible");

    }

    #region Class Vars and other definitions
    ElementHost eh;
    private static System.Windows.Controls.CheckBox avChk;                        // our Avalon button
    private List<String> defaultProps = new List<String>();
    #endregion

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        eh = null;
        this.ClientSize = new System.Drawing.Size(300, 200);
        // add Avalon button 
        avChk = new SWC.CheckBox();
        avChk.Content = "Avalon Button";
        eh = new ElementHost();
        eh.Child = avChk;
        Controls.Add(eh);

        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, System.Reflection.MethodInfo scenario, ScenarioResult result)
    {
        Controls.Clear();
        base.AfterScenario(p, scenario, result);
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("PropertyMap returns name/value collection of default properties.")]
    public ScenarioResult VerifyPropertyMap(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        bool failed = false;

        
        PropertyMap propMap = eh.PropertyMap;
        if (propMap == null)
            sr.IncCounters(false, "Property Map propMap did not return prop map.", p.log);

        ICollection keys = propMap.Keys;
        ICollection values = propMap.Values;

        if (keys.Count != defaultProps.Count)
            sr.IncCounters(false, String.Format("Properties Expected: {0} Actual {1} ", defaultProps.Count, keys.Count), p.log);

        foreach (String k in keys)
        {
            if (!defaultProps.Contains(k))
            {
                sr.IncCounters(false, String.Format("{0} property not found in default Property map", k), p.log);
                failed = true;
                return sr;
            }
        }
        if (!failed)
            sr.IncCounters(true);
        return sr;
    }

    ScenarioResult sr = new ScenarioResult();
    [Scenario("BackColor Transparent")]
    public ScenarioResult BackColorTransparent(TParams p)
    {
        bool bBackColor; 
        ScenarioResult sr = new ScenarioResult();

        Bitmap before = WFCTestLib.Util.Utilities.GetBitmapOfControl(eh, true);
        bBackColor = eh.BackColorTransparent;
        eh.BackColorTransparent = !eh.BackColorTransparent;
        SWF.Application.DoEvents();
        if (eh.BackColorTransparent == bBackColor)
        {
            sr.IncCounters(false, String.Format("BackColorTansparent failed Expected:{0} Actual: {1}", (!bBackColor).ToString(), eh.BackColorTransparent), p.log);
            return sr;
        }

        Bitmap after = WFCTestLib.Util.Utilities.GetBitmapOfControl(eh, true);
        //Compare before and after. We might not do this as this might fail on 16bit color machines.

        bBackColor = eh.BackColorTransparent;
        eh.BackColorTransparent = !eh.BackColorTransparent;
        SWF.Application.DoEvents();
        Bitmap revert = WFCTestLib.Util.Utilities.GetBitmapOfControl(eh, true);
        if (eh.BackColorTransparent == bBackColor)
        {
            sr.IncCounters(false, String.Format("BackColorTansparent failed Expected:{0} Actual: {1}", bBackColor, eh.BackColorTransparent), p.log);
            return sr;
        }
        // Compare revert and after. We might not do this as this might fail on 16bit color machines.

        sr.IncCounters(true);
        return sr;
    }
    #endregion

    
    //Scenario4
    [Scenario("Verify BackColor.")]
    public ScenarioResult VerifyBackColor(TParams p)
    {
        avChk.Width = 80;
        avChk.Height = 50;
        ScenarioResult sr = new ScenarioResult();
        SD.Color newColor;
        SD.Color originalColor = eh.BackColor;

        System.Drawing.KnownColor  ctlColor = KnownColor.Control;
        if (originalColor.ToKnownColor() != ctlColor)
        {
            p.log.WriteLine(String.Format("Default Control Color: Actual {0} -- Expected {1}", eh.BackColor.ToString(), ctlColor.ToString()));
            sr.IncCounters(false, "Default Color failed", p.log);
        }

        do
        {
            newColor = p.ru.GetARGBColor();
        } while (newColor == originalColor);
        eh.BackColor = newColor;
        Utilities.SleepDoEvents(1, 100);

        if (eh.BackColor == originalColor)
        {
            p.log.WriteLine(String.Format("BackColor property did not change. Expected: {0}, Actual: {1} ", newColor, eh.BackColor.ToString()));
            sr.IncCounters(false, "BackColor property failed", p.log);

        }
        else
        {
            //Reset it back again.
            eh.BackColor = originalColor;
            SWF.Application.DoEvents();

            if (eh.BackColor != originalColor)
            {
                p.log.WriteLine(String.Format("BackColor property did not change. Expected: {0}, Actual: {1} ", originalColor, eh.BackColor.ToString()));
                sr.IncCounters(false, "BackColor property failed", p.log);
            }
            sr.IncCounters(true);
        }

        return sr;
    }


    
    [Scenario("Verify BackgroundImage.")]
    public ScenarioResult VerifyBackgroundImage(TParams p)
    {
        Bitmap bmp;
        
        ScenarioResult sr = new ScenarioResult();
        bmp = new System.Drawing.Bitmap("GreenStone.bmp");
        
        eh.BackgroundImage = bmp;
        Utilities.SleepDoEvents(1, 100);
        
       
        if (eh.BackgroundImage != bmp)
        {
            p.log.WriteLine(String.Format("Backgroud Image did not change: Actual {0} -- Expected {1}", eh.BackgroundImage.ToString(), bmp.ToString()));
            sr.IncCounters(false, "Background Image failed.", p.log);
        }
        
        sr.IncCounters(true);
        return sr;
    }


    [Scenario("Verify BackgroundImageLayout.")]
     public ScenarioResult VerifyBackgroundImageLayout(TParams p)
    {
        Bitmap bmp;

        eh.Width = this.Width;
        eh.Height = this.Height;
        ScenarioResult sr = new ScenarioResult();
        avChk.Content = "VerifyBackgroundImageLayout";
        bmp = new System.Drawing.Bitmap("GreenStone.bmp");
        eh.BackgroundImage = bmp;
        Utilities.SleepDoEvents(1, 100);

        bool failed = false;
        Array layouts  = Enum.GetValues(typeof(ImageLayout));

        foreach (ImageLayout  imgLayout in layouts)
        {
            avChk.Content = "Backgound ImageLayout : " + imgLayout.ToString();
            eh.BackgroundImageLayout = imgLayout;
            SWF.Application.DoEvents();
            Utilities.SleepDoEvents(5);

            if (eh.BackgroundImageLayout != imgLayout)
            {
                    p.log.WriteLine(String.Format("BackgroundImageLayout property failed. Expected: {0}, Actual: {1} ", imgLayout, this.eh.BackgroundImageLayout.ToString(), p.log));
                    sr.IncCounters(false, "BackgroundImageLayout failed.", p.log);
                    failed = true;
            }
        }
        if (!failed)
            sr.IncCounters(true);

        
        return sr;
    }


    [Scenario("Verify Cursor.")]
    public ScenarioResult VerifyCursor(TParams p)
    {
        avChk.Width = 80;
        avChk.Height = 50;

        ScenarioResult sr = new ScenarioResult();
        //Cursor
        Cursor originalCursor = eh.Cursor;
        if (originalCursor != Cursors.Arrow)
        {
            p.log.WriteLine(String.Format("Default Cursor -- Expected: {0}, Actual: {1} ", originalCursor, eh.Cursor));
            sr.IncCounters(false);
        }
        Cursor newCursor;
        do
        {
            newCursor = p.ru.GetCursor();
        } while (newCursor == originalCursor);


        eh.Cursor = newCursor;
        SWF.Application.DoEvents();


        if (eh.Cursor == originalCursor)
        {
            p.log.WriteLine(String.Format("Allow drop property did not change. Expected: {0}, Actual: {1} ", newCursor, eh.Cursor.ToString()), p.log);
            sr.IncCounters(false, "Cursor failed.", p.log);
        }
        else
        {
            //Reset it back again.
            eh.Cursor = originalCursor;
            SWF.Application.DoEvents();
            if (eh.Cursor != originalCursor)
            {
                p.log.WriteLine(String.Format("Cursor property did not change. Expected: {0}, Actual: {1} ", originalCursor, eh.Cursor.ToString()));
                sr.IncCounters(false, "Cursor failed.", p.log);
            }
            sr.IncCounters(true);
        }
        return sr;
    }

    [Scenario("Verify Enabled.")]
    public ScenarioResult VerifyEnabled(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        //Enabled
        bool originallyEnabled = eh.Enabled;
        bool enable = !originallyEnabled;

        if (originallyEnabled == false)
        {
            p.log.WriteLine(String.Format("Default Enabled. Expected: {0}, Actual: {1} ", true.ToString(), eh.Enabled.ToString()));
            sr.IncCounters(false);
        }
        eh.Enabled = enable;
        SWF.Application.DoEvents();

        

        if (eh.Enabled == originallyEnabled)
        {
            p.log.WriteLine(String.Format("Enabled property did not change. Expected: {0}, Actual: {1} ", enable, eh.Enabled.ToString()), p.log);
            sr.IncCounters(false, "Cursor failed.", p.log);

        }
        else
        {
            //Reset it back again.
            eh.Enabled = originallyEnabled;
            SWF.Application.DoEvents();
            if (eh.Enabled != originallyEnabled)
            {
                p.log.WriteLine(String.Format("Enabled property did not change. Expected: {0}, Actual: {1} ", originallyEnabled, eh.Cursor.ToString()), p.log);
                sr.IncCounters(false, "Cursor failed.", p.log);

            }
            sr.IncCounters(true);
        }
        return sr;
    }


    [Scenario("Verify Font.")]
    public ScenarioResult VerifyFont(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        Font originalFont = eh.Font;
        string s = eh.Font.ToString();
        
        Font newFont;
        do
        {
            newFont = p.ru.GetFont();
        } while (newFont == originalFont);
        eh.Font = newFont;
        SWF.Application.DoEvents();

        if (eh.Font == originalFont)
        {
            p.log.WriteLine(String.Format("Font drop property did not change. Expected: {0}, Actual: {1} ", newFont, eh.Font.ToString()), p.log);
            sr.IncCounters(false, "Font failed.", p.log);

        }
        else
        {
            //Reset it back again.
            eh.Font = originalFont;
            SWF.Application.DoEvents();
            if (eh.Font != originalFont)
            {
                p.log.WriteLine(String.Format("Font drop property did not change. Expected: {0}, Actual: {1} ", originalFont, eh.Font.ToString()), p.log);
                sr.IncCounters(false, "Cursor failed.", p.log);
            }
            sr.IncCounters(true);
        }
        return sr;
    }
        

    [Scenario("Verify RightToLeft.")]
    public ScenarioResult VerifyRightToLeft(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        bool failed = false;
        RightToLeft originalRTL = eh.RightToLeft;

        if (originalRTL != RightToLeft.No)
        {
            p.log.WriteLine(String.Format("Default RTL : Actual {0} -- Expected {1}", eh.RightToLeft.ToString(), RightToLeft.No.ToString()));
            sr.IncCounters(false, "Default Color failed", p.log);
        }
      
        Array rtls = Enum.GetValues(typeof(RightToLeft));

        foreach (RightToLeft rtl in rtls)
        {
            eh.RightToLeft = rtl;
            SWF.Application.DoEvents();

            if (rtl == RightToLeft.Inherit)
            {
                if (eh.RightToLeft != this.RightToLeft)
                {
                    p.log.WriteLine(String.Format("RightToLeft property was not inherited from Form. Expected: {0}, Actual: {1} ", this.eh.RightToLeft.ToString(), eh.RightToLeft.ToString()), p.log);
                    sr.IncCounters(false, "RightToLeft failed.", p.log);
                    failed = true;
                }
            }
            else
            {
                if (eh.RightToLeft != rtl)
                {
                    p.log.WriteLine(String.Format("RightToLeft property did not change. Expected: {0}, Actual: {1} ", rtl, eh.RightToLeft.ToString()), p.log);
                    sr.IncCounters(false, "RightToLeft failed.", p.log);
                    failed = true;
                }
            }

        }
        if (!failed)
            sr.IncCounters(true);

        return sr;
    }



    [Scenario("Verify Visible.")]
    public ScenarioResult VerifyVisible(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        //defaultProps.Add("Visible");
        bool originallyVisible = eh.Visible;
        if (originallyVisible == false)
        {
            p.log.WriteLine(String.Format("Default Visible : Actual {0} -- Expected {1}", eh.Visible.ToString(), true.ToString()));
            sr.IncCounters(false, "Default Visible failed", p.log);
        }
      

        bool newVisible = !originallyVisible;

        eh.Visible = newVisible;
        SWF.Application.DoEvents();

        if (eh.Visible == originallyVisible)
        {
            p.log.WriteLine(String.Format("Visible property did not change. Expected: {0}, Actual: {1} ", newVisible, eh.Visible.ToString()), p.log);
            sr.IncCounters(false, "Visible failed.", p.log);
        }
        else
        {
            //Reset it back again.
            eh.Visible = originallyVisible;
            SWF.Application.DoEvents();
            if (eh.Visible != originallyVisible)
            {
                p.log.WriteLine(String.Format("Allow drop property did not change. Expected: {0}, Actual: {1} ", originallyVisible, eh.Cursor.ToString()), p.log);
                sr.IncCounters(false, "Visible failed.", p.log);
            }
            sr.IncCounters(true);
        }
        return sr;
    }


}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ PropertyMap returns name/value collection of default properties.
//@ BackColor Transparent
//@ Verify BackColor.
//@ Verify BackgroundImage.
//@ Verify BackgroundImageLayout.
//@ Verify Cursor.
//@ Verify Enabled.
//@ Verify Font.
//@ Verify RightToLeft.
//@ Verify Visible.