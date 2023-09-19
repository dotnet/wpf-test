// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        _defaultProps.Add("BackColor");
        _defaultProps.Add("BackgroundImage");
        _defaultProps.Add("BackgroundImageLayout");
        _defaultProps.Add("Cursor");
        _defaultProps.Add("Enabled");
        _defaultProps.Add("Font");
        _defaultProps.Add("ImeMode");
        _defaultProps.Add("RightToLeft");
        _defaultProps.Add("Visible");

    }

    #region Class Vars and other definitions
    ElementHost _eh;
    private static System.Windows.Controls.CheckBox s_avChk;                        // our Avalon button
    private List<String> _defaultProps = new List<String>();
    #endregion

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        _eh = null;
        this.ClientSize = new System.Drawing.Size(300, 200);
        // add Avalon button 
        s_avChk = new SWC.CheckBox();
        s_avChk.Content = "Avalon Button";
        _eh = new ElementHost();
        _eh.Child = s_avChk;
        Controls.Add(_eh);

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

        
        PropertyMap propMap = _eh.PropertyMap;
        if (propMap == null)
            sr.IncCounters(false, "Property Map propMap did not return prop map.", p.log);

        ICollection keys = propMap.Keys;
        ICollection values = propMap.Values;

        if (keys.Count != _defaultProps.Count)
            sr.IncCounters(false, String.Format("Properties Expected: {0} Actual {1} ", _defaultProps.Count, keys.Count), p.log);

        foreach (String k in keys)
        {
            if (!_defaultProps.Contains(k))
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

    ScenarioResult _sr = new ScenarioResult();
    [Scenario("BackColor Transparent")]
    public ScenarioResult BackColorTransparent(TParams p)
    {
        bool bBackColor; 
        ScenarioResult sr = new ScenarioResult();

        Bitmap before = WFCTestLib.Util.Utilities.GetBitmapOfControl(_eh, true);
        bBackColor = _eh.BackColorTransparent;
        _eh.BackColorTransparent = !_eh.BackColorTransparent;
        SWF.Application.DoEvents();
        if (_eh.BackColorTransparent == bBackColor)
        {
            sr.IncCounters(false, String.Format("BackColorTansparent failed Expected:{0} Actual: {1}", (!bBackColor).ToString(), _eh.BackColorTransparent), p.log);
            return sr;
        }

        Bitmap after = WFCTestLib.Util.Utilities.GetBitmapOfControl(_eh, true);
        //Compare before and after. We might not do this as this might fail on 16bit color machines.

        bBackColor = _eh.BackColorTransparent;
        _eh.BackColorTransparent = !_eh.BackColorTransparent;
        SWF.Application.DoEvents();
        Bitmap revert = WFCTestLib.Util.Utilities.GetBitmapOfControl(_eh, true);
        if (_eh.BackColorTransparent == bBackColor)
        {
            sr.IncCounters(false, String.Format("BackColorTansparent failed Expected:{0} Actual: {1}", bBackColor, _eh.BackColorTransparent), p.log);
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
        s_avChk.Width = 80;
        s_avChk.Height = 50;
        ScenarioResult sr = new ScenarioResult();
        SD.Color newColor;
        SD.Color originalColor = _eh.BackColor;

        System.Drawing.KnownColor  ctlColor = KnownColor.Control;
        if (originalColor.ToKnownColor() != ctlColor)
        {
            p.log.WriteLine(String.Format("Default Control Color: Actual {0} -- Expected {1}", _eh.BackColor.ToString(), ctlColor.ToString()));
            sr.IncCounters(false, "Default Color failed", p.log);
        }

        do
        {
            newColor = p.ru.GetARGBColor();
        } while (newColor == originalColor);
        _eh.BackColor = newColor;
        Utilities.SleepDoEvents(1, 100);

        if (_eh.BackColor == originalColor)
        {
            p.log.WriteLine(String.Format("BackColor property did not change. Expected: {0}, Actual: {1} ", newColor, _eh.BackColor.ToString()));
            sr.IncCounters(false, "BackColor property failed", p.log);

        }
        else
        {
            //Reset it back again.
            _eh.BackColor = originalColor;
            SWF.Application.DoEvents();

            if (_eh.BackColor != originalColor)
            {
                p.log.WriteLine(String.Format("BackColor property did not change. Expected: {0}, Actual: {1} ", originalColor, _eh.BackColor.ToString()));
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
        
        _eh.BackgroundImage = bmp;
        Utilities.SleepDoEvents(1, 100);
        
       
        if (_eh.BackgroundImage != bmp)
        {
            p.log.WriteLine(String.Format("Backgroud Image did not change: Actual {0} -- Expected {1}", _eh.BackgroundImage.ToString(), bmp.ToString()));
            sr.IncCounters(false, "Background Image failed.", p.log);
        }
        
        sr.IncCounters(true);
        return sr;
    }


    [Scenario("Verify BackgroundImageLayout.")]
     public ScenarioResult VerifyBackgroundImageLayout(TParams p)
    {
        Bitmap bmp;

        _eh.Width = this.Width;
        _eh.Height = this.Height;
        ScenarioResult sr = new ScenarioResult();
        s_avChk.Content = "VerifyBackgroundImageLayout";
        bmp = new System.Drawing.Bitmap("GreenStone.bmp");
        _eh.BackgroundImage = bmp;
        Utilities.SleepDoEvents(1, 100);

        bool failed = false;
        Array layouts  = Enum.GetValues(typeof(ImageLayout));

        foreach (ImageLayout  imgLayout in layouts)
        {
            s_avChk.Content = "Backgound ImageLayout : " + imgLayout.ToString();
            _eh.BackgroundImageLayout = imgLayout;
            SWF.Application.DoEvents();
            Utilities.SleepDoEvents(5);

            if (_eh.BackgroundImageLayout != imgLayout)
            {
                    p.log.WriteLine(String.Format("BackgroundImageLayout property failed. Expected: {0}, Actual: {1} ", imgLayout, this._eh.BackgroundImageLayout.ToString(), p.log));
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
        s_avChk.Width = 80;
        s_avChk.Height = 50;

        ScenarioResult sr = new ScenarioResult();
        //Cursor
        Cursor originalCursor = _eh.Cursor;
        if (originalCursor != Cursors.Arrow)
        {
            p.log.WriteLine(String.Format("Default Cursor -- Expected: {0}, Actual: {1} ", originalCursor, _eh.Cursor));
            sr.IncCounters(false);
        }
        Cursor newCursor;
        do
        {
            newCursor = p.ru.GetCursor();
        } while (newCursor == originalCursor);


        _eh.Cursor = newCursor;
        SWF.Application.DoEvents();


        if (_eh.Cursor == originalCursor)
        {
            p.log.WriteLine(String.Format("Allow drop property did not change. Expected: {0}, Actual: {1} ", newCursor, _eh.Cursor.ToString()), p.log);
            sr.IncCounters(false, "Cursor failed.", p.log);
        }
        else
        {
            //Reset it back again.
            _eh.Cursor = originalCursor;
            SWF.Application.DoEvents();
            if (_eh.Cursor != originalCursor)
            {
                p.log.WriteLine(String.Format("Cursor property did not change. Expected: {0}, Actual: {1} ", originalCursor, _eh.Cursor.ToString()));
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
        bool originallyEnabled = _eh.Enabled;
        bool enable = !originallyEnabled;

        if (originallyEnabled == false)
        {
            p.log.WriteLine(String.Format("Default Enabled. Expected: {0}, Actual: {1} ", true.ToString(), _eh.Enabled.ToString()));
            sr.IncCounters(false);
        }
        _eh.Enabled = enable;
        SWF.Application.DoEvents();

        

        if (_eh.Enabled == originallyEnabled)
        {
            p.log.WriteLine(String.Format("Enabled property did not change. Expected: {0}, Actual: {1} ", enable, _eh.Enabled.ToString()), p.log);
            sr.IncCounters(false, "Cursor failed.", p.log);

        }
        else
        {
            //Reset it back again.
            _eh.Enabled = originallyEnabled;
            SWF.Application.DoEvents();
            if (_eh.Enabled != originallyEnabled)
            {
                p.log.WriteLine(String.Format("Enabled property did not change. Expected: {0}, Actual: {1} ", originallyEnabled, _eh.Cursor.ToString()), p.log);
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
        Font originalFont = _eh.Font;
        string s = _eh.Font.ToString();
        
        Font newFont;
        do
        {
            newFont = p.ru.GetFont();
        } while (newFont == originalFont);
        _eh.Font = newFont;
        SWF.Application.DoEvents();

        if (_eh.Font == originalFont)
        {
            p.log.WriteLine(String.Format("Font drop property did not change. Expected: {0}, Actual: {1} ", newFont, _eh.Font.ToString()), p.log);
            sr.IncCounters(false, "Font failed.", p.log);

        }
        else
        {
            //Reset it back again.
            _eh.Font = originalFont;
            SWF.Application.DoEvents();
            if (_eh.Font != originalFont)
            {
                p.log.WriteLine(String.Format("Font drop property did not change. Expected: {0}, Actual: {1} ", originalFont, _eh.Font.ToString()), p.log);
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
        RightToLeft originalRTL = _eh.RightToLeft;

        if (originalRTL != RightToLeft.No)
        {
            p.log.WriteLine(String.Format("Default RTL : Actual {0} -- Expected {1}", _eh.RightToLeft.ToString(), RightToLeft.No.ToString()));
            sr.IncCounters(false, "Default Color failed", p.log);
        }
      
        Array rtls = Enum.GetValues(typeof(RightToLeft));

        foreach (RightToLeft rtl in rtls)
        {
            _eh.RightToLeft = rtl;
            SWF.Application.DoEvents();

            if (rtl == RightToLeft.Inherit)
            {
                if (_eh.RightToLeft != this.RightToLeft)
                {
                    p.log.WriteLine(String.Format("RightToLeft property was not inherited from Form. Expected: {0}, Actual: {1} ", this._eh.RightToLeft.ToString(), _eh.RightToLeft.ToString()), p.log);
                    sr.IncCounters(false, "RightToLeft failed.", p.log);
                    failed = true;
                }
            }
            else
            {
                if (_eh.RightToLeft != rtl)
                {
                    p.log.WriteLine(String.Format("RightToLeft property did not change. Expected: {0}, Actual: {1} ", rtl, _eh.RightToLeft.ToString()), p.log);
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
        bool originallyVisible = _eh.Visible;
        if (originallyVisible == false)
        {
            p.log.WriteLine(String.Format("Default Visible : Actual {0} -- Expected {1}", _eh.Visible.ToString(), true.ToString()));
            sr.IncCounters(false, "Default Visible failed", p.log);
        }
      

        bool newVisible = !originallyVisible;

        _eh.Visible = newVisible;
        SWF.Application.DoEvents();

        if (_eh.Visible == originallyVisible)
        {
            p.log.WriteLine(String.Format("Visible property did not change. Expected: {0}, Actual: {1} ", newVisible, _eh.Visible.ToString()), p.log);
            sr.IncCounters(false, "Visible failed.", p.log);
        }
        else
        {
            //Reset it back again.
            _eh.Visible = originallyVisible;
            SWF.Application.DoEvents();
            if (_eh.Visible != originallyVisible)
            {
                p.log.WriteLine(String.Format("Allow drop property did not change. Expected: {0}, Actual: {1} ", originallyVisible, _eh.Cursor.ToString()), p.log);
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
